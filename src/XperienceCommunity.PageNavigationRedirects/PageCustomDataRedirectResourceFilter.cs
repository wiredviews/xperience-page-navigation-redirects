using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using XperienceCommunity.PageBuilderUtilities;

namespace XperienceCommunity.PageNavigationRedirects
{
    /// <summary>
    /// Redirects the current request to another URL based on the <see cref="PageRedirectionType"/> of the current Page
    /// </summary>
    public class PageCustomDataRedirectResourceFilter : IAsyncResourceFilter
    {
        private readonly IPageBuilderContext context;
        private readonly IPageDataContextRetriever contextRetriever;
        private readonly IPageUrlRetriever urlRetriever;
        private readonly IPageRetriever pageRetriever;
        private readonly PageNavigationRedirectsValuesRetriever valuesRetriever;
        private readonly IEventLogService log;
        private readonly PageNavigationRedirectOptions options;

        public PageCustomDataRedirectResourceFilter(
            IPageBuilderContext context,
            IPageDataContextRetriever contextRetriever,
            IPageUrlRetriever urlRetriever,
            IPageRetriever pageRetriever,
            PageNavigationRedirectsValuesRetriever valuesRetriever,
            IEventLogService log,
            IOptions<PageNavigationRedirectOptions> options)
        {
            this.context = context;
            this.contextRetriever = contextRetriever;
            this.urlRetriever = urlRetriever;
            this.pageRetriever = pageRetriever;
            this.valuesRetriever = valuesRetriever;
            this.log = log;
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var result = await HandleRedirectionInternal(context.HttpContext.RequestAborted);

            if (result is object)
            {
                context.Result = result;

                return;
            }

            await next();
        }

        private async Task<IActionResult?> HandleRedirectionInternal(CancellationToken token)
        {
            if (context.IsEditMode)
            {
                return null;
            }

            if (context.IsLivePreviewMode && !options.RedirectInLivePreviewMode)
            {
                return null;
            }

            if (!contextRetriever.TryRetrieve<TreeNode>(out var data))
            {
                return null;
            }

            var page = data.Page;

            var redirectionType = valuesRetriever.RedirectionType(page);

            if (redirectionType == PageRedirectionType.None)
            {
                return null;
            }

            bool usePermanentRedirects = valuesRetriever.UsePermanentRedirects(page);

            if (redirectionType == PageRedirectionType.External)
            {
                string? url = valuesRetriever.ExternalRedirectURL(page);

                if (string.IsNullOrWhiteSpace(url))
                {
                    log.LogError(
                       "PageNavigationRedirects",
                       "REDIRECT_EXTERNAL",
                       $"Page [{page.DocumentID}] [{page.NodeAliasPath}] has no specified External URL for redirection.");

                    return null;
                }

                return new RedirectResult(url, usePermanentRedirects);
            }

            if (redirectionType == PageRedirectionType.Internal)
            {
                var nodeGuid = valuesRetriever.InternalRedirectNodeGUID(page);

                if (!nodeGuid.HasValue)
                {
                    log.LogError(
                        "PageNavigationRedirects",
                        "REDIRECT_INTERNAL",
                        $"Page [{page.DocumentID}] [{page.NodeAliasPath}] has no specified Internal Page Node for redirection.");

                    return null;
                }

                var pages = await pageRetriever.RetrieveAsync<TreeNode>(
                    q => q.WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid.Value),
                    c => c.Key($"{nameof(PageCustomDataRedirectResourceFilter)}|{PageRedirectionType.Internal}|{nodeGuid.Value}"),
                    cancellationToken: token);

                var linkedPage = pages.FirstOrDefault();

                if (linkedPage is null)
                {
                    log.LogError(
                        "PageNavigationRedirects",
                        "REDIRECT_INTERNAL",
                        $"Page [{page.DocumentID}] [{page.NodeAliasPath}] Internal Redirection cannot complete because Page Node [{nodeGuid.Value}], does not exist.");

                    return null;
                }

                var url = urlRetriever.Retrieve(linkedPage);

                return url is null
                    ? null
                    : new RedirectResult(url.RelativePath, usePermanentRedirects);
            }

            if (redirectionType == PageRedirectionType.FirstChild)
            {
                string? firstChildClassName = valuesRetriever.FirstChildClassName(page);

                var children = await pageRetriever.RetrieveAsync<TreeNode>(
                    query =>
                    {
                        query.WhereEquals(nameof(TreeNode.NodeParentID), page.NodeID);

                        if (!string.IsNullOrWhiteSpace(firstChildClassName))
                        {
                            query.WhereEquals(nameof(TreeNode.ClassName), firstChildClassName);
                        }

                        query
                            .OrderBy(nameof(TreeNode.NodeOrder))
                            .TopN(1);
                    },
                    c =>
                    {
                        var keys = new HashSet<string>
                        {
                            nameof(PageCustomDataRedirectResourceFilter),
                            PageRedirectionType.FirstChild.ToString(),
                            page.NodeID.ToString()
                        };

                        if (!string.IsNullOrWhiteSpace(firstChildClassName))
                        {
                            keys.Add(firstChildClassName);
                        }

                        c.Key(CacheHelper.BuildCacheItemName(keys));
                    },
                    cancellationToken: token);

                if (children.Count() == 0)
                {
                    string description = string.IsNullOrWhiteSpace(firstChildClassName)
                        ? $"Page [{page.DocumentID}] [{page.NodeAliasPath}] has no children to redirect to."
                        : $"Page [{page.DocumentID}] [{page.NodeAliasPath}] has no children of Type [{firstChildClassName}] to redirect to.";

                    log.LogError("PageNavigationRedirects", "REDIRECT_FIRST_CHILD", description);

                    return null;
                }

                var firstChild = children.Single();

                var url = urlRetriever.Retrieve(firstChild);

                if (url is null || string.IsNullOrWhiteSpace(url.RelativePath))
                {
                    string description = string.IsNullOrWhiteSpace(firstChildClassName)
                        ? $"Child Page [{firstChild.DocumentID}] [{firstChild.NodeAliasPath}] has no URL to redirect to."
                        : $"Child Page [{firstChild.DocumentID}] [{firstChild.NodeAliasPath}] of Type [{firstChildClassName}] has no URL to redirect to.";

                    log.LogError("PageNavigationRedirects", "REDIRECT_FIRST_CHILD", description);

                    return null;
                }

                return new RedirectResult(url.RelativePath, usePermanentRedirects);
            }

            return null;
        }
    }
}
