using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace XperienceCommunity.PageNavigationRedirects
{
    /// <summary>
    /// Redirects the current request to another URL based on the <see cref="PageRedirectionType"/> of the current Page
    /// </summary>
    public class PageCustomDataRedirectResourceFilter : IAsyncResourceFilter
    {
        private readonly IPageDataContextRetriever contextRetriever;
        private readonly IPageUrlRetriever urlRetriever;
        private readonly IPageRetriever pageRetriever;
        private readonly PageNavigationRedirectsValuesRetriever valuesRetriever;
        private readonly PageNavigationRedirectOptions options;

        public PageCustomDataRedirectResourceFilter(
            IPageDataContextRetriever contextRetriever,
            IPageUrlRetriever urlRetriever,
            IPageRetriever pageRetriever,
            PageNavigationRedirectsValuesRetriever valuesRetriever,
            IOptions<PageNavigationRedirectOptions> options)
        {
            this.contextRetriever = contextRetriever;
            this.urlRetriever = urlRetriever;
            this.pageRetriever = pageRetriever;
            this.valuesRetriever = valuesRetriever;
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

                return url is null
                    ? null
                    : new RedirectResult(url, usePermanentRedirects);
            }

            if (redirectionType == PageRedirectionType.Internal)
            {
                var nodeGuid = valuesRetriever.InternalRedirectNodeGUID(page);

                if (!nodeGuid.HasValue)
                {
                    return null;
                }

                var pages = await pageRetriever.RetrieveAsync<TreeNode>(
                    q => q.WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid.Value),
                    c => c.Key($"{nameof(PageCustomDataRedirectResourceFilter)}|{PageRedirectionType.Internal}|{nodeGuid.Value}"),
                    cancellationToken: token);

                var linkedPage = pages.FirstOrDefault();

                if (linkedPage is null)
                {
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

                if (!children.Any())
                {
                    return null;
                }

                var firstChild = children.Single();

                var url = urlRetriever.Retrieve(firstChild);

                return url is null
                    ? null
                    : new RedirectResult(url.RelativePath, usePermanentRedirects);
            }

            return null;
        }
    }
}
