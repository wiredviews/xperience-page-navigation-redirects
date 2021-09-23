using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

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
    }
}
