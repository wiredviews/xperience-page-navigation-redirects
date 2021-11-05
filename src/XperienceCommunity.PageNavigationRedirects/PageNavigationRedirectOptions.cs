using CMS.DocumentEngine;
using XperienceCommunity.PageBuilderUtilities;

namespace XperienceCommunity.PageNavigationRedirects
{
    /// <summary>
    /// Configuration settings for Page redirection across the entire application
    /// </summary>
    public class PageNavigationRedirectOptions
    {
        /// <summary>
        /// Defaults to "PageRedirectionType"
        /// </summary>
        /// <value></value>
        public string RedirectionTypeFieldName { get; set; } = "PageRedirectionType";
        /// <summary>
        /// Defaults to "PageInternalRedirectNodeGuid"
        /// </summary>
        /// <value></value>
        public string InternalRedirectNodeGUIDFieldName { get; set; } = "PageInternalRedirectNodeGuid";
        /// <summary>
        /// Defaults to "PageExternalRedirectURL"
        /// </summary>
        /// <value></value>
        public string ExternalRedirectURLFieldName { get; set; } = "PageExternalRedirectURL";
        /// <summary>
        /// Defaults to "PageFirstChildClassName"
        /// </summary>
        /// <value></value>
        public string FirstChildClassNameFieldName { get; set; } = "PageFirstChildClassName";
        /// <summary>
        /// Defaults to "PageUsePermanentRedirects"
        /// </summary>
        /// <value></value>
        public string PageUsePermanentRedirectsFieldName { get; set; } = "PageUsePermanentRedirects";
        /// <summary>
        /// If true then redirects will have a 301 HTTP status code.
        /// This value can be overridden per-Page using <see cref="PageUsePermanentRedirectsFieldName" />.
        /// Defaults to false / 302 HTTP status code redirects. 
        /// </summary>
        /// <value></value>
        public bool UsePermanentRedirect { get; set; } = false;
        /// <summary>
        /// If true, then the Navigation Redirect values will be retrieved from the <see cref="TreeNode.DocumentCustomData" />.
        /// field, otherwise the <see cref="TreeNode.NodeCustomData" /> field will be used.
        /// Defaults to true.
        /// </summary>
        /// <value></value>
        public bool UseDocumentCustomData { get; set; } = true;
        /// <summary>
        /// If true, then visiting the Page in Page Builder <see cref="PageBuilderMode.LivePreview"/> will result in an HTTP redirect.
        /// Defaults to false.
        /// </summary>
        public bool RedirectInLivePreviewMode { get; set; } = false;
    }
}
