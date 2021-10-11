using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using CMS.DocumentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace XperienceCommunity.PageNavigationRedirects.Tests
{
    public static class FixtureExtensions
    {
        public static IOptions<PageNavigationRedirectOptions> CreateOptions(this Fixture fixture)
        {
            return Options.Create(new PageNavigationRedirectOptions
            {
                ExternalRedirectURLFieldName = fixture.Create<string>(),
                FirstChildClassNameFieldName = fixture.Create<string>(),
                InternalRedirectNodeGUIDFieldName = fixture.Create<string>(),
                RedirectionTypeFieldName = fixture.Create<string>(),
                UsePermanentRedirect = fixture.Create<bool>()
            });
        }

        public static (ResourceExecutingContext, Task<ResourceExecutedContext>) CreateFilterContexts(this Fixture fixture)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            return (new ResourceExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new List<IValueProviderFactory>()),
            Task.FromResult(new ResourceExecutedContext(
                actionContext,
                new List<IFilterMetadata>())));
        }

        public static void SetPageDatasourceValue(this TreeNode page, string columnName, object? value, PageNavigationRedirectOptions options)
        {
            if (options.UseDocumentCustomData)
            {
                page.DocumentCustomData.SetValue(columnName, value);
            }
            else
            {
                page.NodeCustomData.SetValue(columnName, value);
            }
        }

        public static IHttpContextAccessor CreateHttpContextAccessWithPageBuilder(this Fixture fixture, bool isPageBuilderEnabled)
        {
            var pageBuilderFeature = Substitute.For<IPageBuilderFeature>();
            pageBuilderFeature.EditMode.Returns(isPageBuilderEnabled);
            var features = Substitute.For<IFeatureSet>();
            features.GetFeature<IPageBuilderFeature>().Returns(pageBuilderFeature);

            var context = new DefaultHttpContext();
            context.Items.Add("Kentico.Features", features);

            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext.Returns(context);

            return accessor;
        }
    }
}
