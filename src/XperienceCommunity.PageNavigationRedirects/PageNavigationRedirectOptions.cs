namespace XperienceCommunity.PageNavigationRedirects
{
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
        /// Defaults to false / 302 HTTP status code redirects. If true then redirects will have a 301 HTTP status code.
        /// This value can be overridden per-Page using <see cref="PageUsePermanentRedirectsFieldName" />
        /// </summary>
        /// <value></value>
        public bool UsePermanentRedirect { get; set; } = false;
    }
}
