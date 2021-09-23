using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.DocumentEngine;
using CMS.Helpers;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        private readonly PageNavigationRedirectValuesRetriever valuesRetriever;

        public PageCustomDataRedirectResourceFilter(
            IPageDataContextRetriever contextRetriever,
            IPageUrlRetriever urlRetriever,
            IPageRetriever pageRetriever,
            PageNavigationRedirectValuesRetriever valuesRetriever)
        {
            this.contextRetriever = contextRetriever;
            this.urlRetriever = urlRetriever;
            this.pageRetriever = pageRetriever;
            this.valuesRetriever = valuesRetriever;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!contextRetriever.TryRetrieve<TreeNode>(out var data))
            {
                return;
            }

            var page = data.Page;

            var redirectionType = valuesRetriever.RedirectionType(page);

            if (redirectionType == PageRedirectionType.None)
            {
                return;
            }

            if (redirectionType == PageRedirectionType.External)
            {
                string? url = valuesRetriever.ExternalRedirectURL(page);

                if (url is string)
                {
                    context.Result = new RedirectResult(url, false);
                }

                return;
            }

            if (redirectionType == PageRedirectionType.Internal)
            {
                var nodeGuid = valuesRetriever.InternalRedirectNodeGUID(page);

                if (!nodeGuid.HasValue)
                {
                    return;
                }

                var pages = await pageRetriever.RetrieveAsync<TreeNode>(
                    q => q.WhereEquals(nameof(TreeNode.NodeGUID), nodeGuid.Value),
                    c => c.Key($"{nameof(PageCustomDataRedirectResourceFilter)}|{PageRedirectionType.Internal}|{nodeGuid.Value}"),
                    cancellationToken: context.HttpContext.RequestAborted);

                var linkedPage = pages.FirstOrDefault();

                if (linkedPage is null)
                {
                    return;
                }

                var url = urlRetriever.Retrieve(linkedPage);

                if (url is null)
                {
                    return;
                }

                context.Result = new RedirectResult(url.RelativePath, false);

                return;
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
                    cancellationToken: context.HttpContext.RequestAborted);

                if (!children.Any())
                {
                    return;
                }

                var firstChild = children.Single();

                var url = urlRetriever.Retrieve(firstChild);

                if (url is null)
                {
                    return;
                }

                context.Result = new RedirectResult(url.RelativePath, false);
            }
        }
    }
}
