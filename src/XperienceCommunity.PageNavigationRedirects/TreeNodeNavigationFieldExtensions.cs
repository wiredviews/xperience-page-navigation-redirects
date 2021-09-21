using System;
using CMS.DocumentEngine;
using CMS.Helpers;

namespace XperienceCommunity.PageNavigationRedirects
{
    public enum PageRedirectionType
    {
        None,
        Internal,
        External,
        FirstChild
    }

    public static class TreeNodeNavigationFieldExtensions
    {
        public class Fields
        {
            public const string RedirectionType = "PageRedirectionType";
            public const string InternalRedirectNodeGuid = "PageInternalRedirectNodeGuid";
            public const string ExternalRedirectURL = "PageExternalRedirectURL";
        }

        public static PageRedirectionType RedirectionType(this TreeNode page)
        {
            if (!page.DocumentCustomData.TryGetValue(Fields.RedirectionType, out object customDataValue))
            {
                return PageRedirectionType.None;
            }

            if (!Enum.TryParse<PageRedirectionType>(ValidationHelper.GetString(customDataValue, ""), out var type))
            {
                return PageRedirectionType.None;
            }

            return type;
        }

        public static string? ExternalRedirectURL(this TreeNode page)
        {
            if (!page.DocumentCustomData.TryGetValue(Fields.ExternalRedirectURL, out object customDataValue))
            {
                return null;
            }

            string url = ValidationHelper.GetString(customDataValue, "");

            return string.IsNullOrWhiteSpace(url)
                ? null
                : url;
        }

        public static Guid? InternalRedirectNodeGuid(this TreeNode page)
        {
            if (!page.DocumentCustomData.TryGetValue(Fields.InternalRedirectNodeGuid, out object customDataValue))
            {
                return null;
            }

            var nodeGuid = ValidationHelper.GetGuid(customDataValue, default);

            return nodeGuid == default
                ? null
                : nodeGuid;
        }
    }
}
