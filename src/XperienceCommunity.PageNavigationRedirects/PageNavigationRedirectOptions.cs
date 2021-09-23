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
    }
}
