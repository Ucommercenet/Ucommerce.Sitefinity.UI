using System;
using System.Collections.Generic;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace UCommerce.Sitefinity.UI.Pages
{
    internal class UrlResolver
    {
        public static string GetAbsoluteUrl(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var uri = HttpContext.Current.Request.Url;

            if (uri == null)
            {
                uri = SystemManager.CurrentContext.CurrentSite.GetUri();
            }

            string port = uri.Port != 80 ? (":" + uri.Port) : string.Empty;

            return string.Format("{0}://{1}{2}{3}", uri.Scheme, uri.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }

        public static string GetPageNodeUrl(Guid pageNodeId)
        {
            var nodeUrl = string.Empty;
            var siteMapProvider = SiteMapBase.GetCurrentProvider();

            if (siteMapProvider != null && pageNodeId != Guid.Empty)
            {
                var pageProvider = Telerik.Sitefinity.Modules.Pages.PageManager.GetManager().Provider;
                bool suppressedChecks = pageProvider.SuppressSecurityChecks;
                pageProvider.SuppressSecurityChecks = true;
                var node = siteMapProvider.FindSiteMapNodeFromKey(pageNodeId.ToString());
                pageProvider.SuppressSecurityChecks = suppressedChecks;

                if (node == null)
                {
                    throw new ArgumentException("Invalid details page specified: \"{0}\".".Arrange(pageNodeId));
                }

                var pageSiteNode = node as PageSiteNode;
                if (pageSiteNode != null)
                {
                    nodeUrl = pageSiteNode.UrlWithoutExtension;
                }
            }

            return nodeUrl;
        }

        public static string GetCurrentPageNodeUrl()
        {
            var currentNodeUrl = string.Empty;
            var currentNode = SiteMapBase.GetActualCurrentNode();
            if (currentNode != null)
            {
                currentNodeUrl = currentNode.UrlWithoutExtension;
            }
            else
            {
                var siteMapProvider = SiteMapBase.GetCurrentProvider();
                if (siteMapProvider.CurrentNode != null)
                {
                    var currentPageSiteNode = siteMapProvider.CurrentNode as PageSiteNode;
                    if (currentPageSiteNode != null)
                    {
                        currentNodeUrl = currentPageSiteNode.UrlWithoutExtension;
                    }
                }
            }

            return currentNodeUrl;
        }

        public static Dictionary<string, string> GetQueryStringParameters(IList<string> queryStringBlackList)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            {
                if (queryString == null || queryStringBlackList.Contains(queryString)) continue;

                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }

            return parameters;
        }
    }
}
