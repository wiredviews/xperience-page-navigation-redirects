using System;
using CMS.DocumentEngine;
using CMS.Helpers;
using Microsoft.Extensions.Options;

namespace XperienceCommunity.PageNavigationRedirects
{
    public class PageNavigationRedirectsValuesRetriever
    {
        private readonly PageNavigationRedirectOptions options;

        public PageNavigationRedirectsValuesRetriever(IOptions<PageNavigationRedirectOptions> options)
        {
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        }

        public PageRedirectionType RedirectionType(TreeNode page)
        {
            var datasource = GetDatasource(page);

            if (!datasource.TryGetValue(options.RedirectionTypeFieldName, out object customDataValue))
            {
                return PageRedirectionType.None;
            }

            if (!Enum.TryParse<PageRedirectionType>(ValidationHelper.GetString(customDataValue, ""), out var type))
            {
                return PageRedirectionType.None;
            }

            return type;
        }

        public string? ExternalRedirectURL(TreeNode page)
        {
            var datasource = GetDatasource(page);

            if (!datasource.TryGetValue(options.ExternalRedirectURLFieldName, out object customDataValue))
            {
                return null;
            }

            string url = ValidationHelper.GetString(customDataValue, "");

            return string.IsNullOrWhiteSpace(url)
                ? null
                : url;
        }

        public Guid? InternalRedirectNodeGUID(TreeNode page)
        {
            var datasource = GetDatasource(page);

            if (!datasource.TryGetValue(options.InternalRedirectNodeGUIDFieldName, out object customDataValue))
            {
                return null;
            }

            var nodeGUID = ValidationHelper.GetGuid(customDataValue, default);

            return nodeGUID == default
                ? null
                : nodeGUID;
        }

        public string? FirstChildClassName(TreeNode page)
        {
            var datasource = GetDatasource(page);

            if (!datasource.TryGetValue(options.FirstChildClassNameFieldName, out object customDataValue))
            {
                return null;
            }

            string className = ValidationHelper.GetString(customDataValue, "").Trim();

            return string.IsNullOrWhiteSpace(className)
                ? null
                : className;
        }

        public bool UsePermanentRedirects(TreeNode page)
        {
            var datasource = GetDatasource(page);

            if (!datasource.TryGetValue(options.PageUsePermanentRedirectsFieldName, out object customDataValue))
            {
                return options.UsePermanentRedirect;
            }

            int value = ValidationHelper.GetInteger(customDataValue, -1);

            return value switch
            {
                0 => false,
                1 => true,
                _ or -1 => options.UsePermanentRedirect,
            };
        }

        private ContainerCustomData GetDatasource(TreeNode page) =>
            options.UseDocumentCustomData
                ? page.DocumentCustomData
                : page.NodeCustomData;
    }
}
