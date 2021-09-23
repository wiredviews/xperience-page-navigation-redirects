using System;
using XperienceCommunity.PageNavigationRedirects;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionPageNavigationRedirectExtensions
    {
        /// <summary>
        /// Adds Page Navigation Redirects with customizable <see cref="PageNavigationRedirectOptions" />
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageNavigationRedirects(this IServiceCollection services, Action<PageNavigationRedirectOptions> configureOptions) =>
            services
                .AddPageNavigationRedirects()
                .Configure<PageNavigationRedirectOptions>(configureOptions);

        /// <summary>
        /// Adds Page Navigation Redirects functionality
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageNavigationRedirects(this IServiceCollection services) =>
            services
                .AddTransient<PageNavigationRedirectsValuesRetriever>()
                .AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(PageCustomDataRedirectResourceFilter));
                })
                .Services
                .AddOptions<PageNavigationRedirectOptions>()
                .Services;
    }
}
