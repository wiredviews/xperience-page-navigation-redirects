using System.Linq;
using System.Threading.Tasks;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XperienceCommunity.PageNavigationRedirects
{
    /// <summary>
    /// Redirects the current request to another URL based on the <see cref="PageExtensions.RedirectionType(TreeNode)"/> value
    /// </summary>
    public class PageCustomDataRedirectResourceFilter : IAsyncResourceFilter
    {
        private readonly IPageDataContextRetriever contextRetriever;
        private readonly IPageUrlRetriever urlRetriever;
        private readonly IPageRetriever pageRetriever;

        public PageCustomDataRedirectResourceFilter(
            IPageDataContextRetriever contextRetriever,
            IPageUrlRetriever urlRetriever,
            IPageRetriever pageRetriever)
        {
            this.contextRetriever = contextRetriever;
            this.urlRetriever = urlRetriever;
            this.pageRetriever = pageRetriever;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!contextRetriever.TryRetrieve<TreeNode>(out var data))
            {
                return;
            }

            var page = data.Page;

            var redirectionType = page.RedirectionType();

            if (redirectionType == PageRedirectionType.None)
            {
                return;
            }

            if (redirectionType == PageRedirectionType.External)
            {
                string? url = page.ExternalRedirectURL();

                if (url is string)
                {
                    context.Result = new RedirectResult(url, false);
                }

                return;
            }

            if (redirectionType == PageRedirectionType.Internal)
            {
                var nodeGuid = page.InternalRedirectNodeGuid();

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
                var children = await pageRetriever.RetrieveAsync<TreeNode>(
                    q => q
                        .WhereEquals(nameof(TreeNode.NodeParentID), page.NodeID)
                        .OrderBy(nameof(TreeNode.NodeOrder))
                        .TopN(1),
                    c => c.Key($"{nameof(PageCustomDataRedirectResourceFilter)}|{PageRedirectionType.FirstChild}|{page.NodeID}"),
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
