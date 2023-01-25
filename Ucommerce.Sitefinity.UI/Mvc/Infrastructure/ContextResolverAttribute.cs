using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace UCommerce.Sitefinity.UI.Mvc
{
    /// <summary>
    /// Action attribute class that handles the initilization of the Catalog Context.
    /// </summary>
    public class ContextResolverAttribute : ActionFilterAttribute, IActionFilter
    {
        private const string CONTEXT_INITIALIZED_KEY = "UcommerceContextInitialized";
        private const string RESERVED_DOMAINS = "Ucommerce:Reserved:Domains";
        private ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private IIndex<Category> CategoryIndex => ObjectFactory.Instance.Resolve<IIndex<Category>>();
        private IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!TryValidateReservedDomain(out _))
            {
                return;
            }

            if (SystemManager.CurrentHttpContext.Items[CONTEXT_INITIALIZED_KEY] != null
                && (bool)SystemManager.CurrentHttpContext.Items[CONTEXT_INITIALIZED_KEY])
            {
                return;
            }

            List<string> urlSegments = null;

            var currentUrl = SystemManager.CurrentHttpContext.Request.RawUrl;
            if (currentUrl?.Contains("?") ?? false)
            {
                currentUrl = currentUrl.Substring(0, currentUrl.IndexOf("?", StringComparison.Ordinal));
            }

            // NOTE: There are a few urls that aren't relevant
            if (currentUrl?.Contains("/rest-api/") ?? false)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                urlSegments = currentUrl.Split('/')
                    .Select(i => HttpUtility.UrlDecode(i.Replace("/", string.Empty)))
                    .ToList();
            }
            else
            {
                var currentNode = SiteMapBase.GetActualCurrentNode();

                if (currentNode != null)
                {
                    urlSegments = currentNode.Url.Split('/')
                        .Select(HttpUtility.UrlDecode)
                        .ToList();
                }
            }

            if (urlSegments != null)
            {
                ResolveCurrentProduct(urlSegments);
                ResolveCurrentCategory(urlSegments);
            }

            if (SystemManager.CurrentHttpContext.Items[CONTEXT_INITIALIZED_KEY] == null)
            {
                SystemManager.CurrentHttpContext.Items.Add(CONTEXT_INITIALIZED_KEY, true);
            }
            else
            {
                SystemManager.CurrentHttpContext.Items[CONTEXT_INITIALIZED_KEY] = true;
            }
        }

        private void ResolveCurrentCategory(List<string> urlSegments)
        {
            if (CatalogContext.CurrentCategory != null)
            {
                return;
            }

            var categories = CategoryIndex.Find()
                .Where(c => urlSegments.Contains(c.Name.ToString()) && c.ProductCatalog == CatalogContext.CurrentCatalog.Guid)
                .ToList();

            if (categories == null)
            {
                return;
            }

            for (var i = urlSegments.Count - 1; i >= 0; i--)
            {
                var cat = categories.FirstOrDefault(p => urlSegments[i] == p.Name);

                if (cat == null)
                {
                    continue;
                }

                CatalogContext.CurrentCategory = cat;
                break;
            }
        }

        private void ResolveCurrentProduct(List<string> urlSegments)
        {
            if (CatalogContext.CurrentProduct != null)
            {
                return;
            }

            var products = ProductIndex.Find()
                .Where(p => urlSegments.Contains(p.Slug.ToString()))
                .ToList();

            if (products == null)
            {
                return;
            }

            for (var i = urlSegments.Count - 1; i >= 0; i--)
            {
                var product = products.FirstOrDefault(p => urlSegments.Contains(p.Slug));
                if (product == null)
                {
                    continue;
                }

                CatalogContext.CurrentProduct = product;
                break;
            }
        }

        private static bool TryValidateReservedDomain(out string domain)
        {
            domain = null;
            if (HttpContext.Current == null)
            {
                return false;
            }

            var reservedDomainsString = ConfigurationManager.AppSettings[RESERVED_DOMAINS] ?? string.Empty;
            var reservedDomains = reservedDomainsString.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            domain = reservedDomains.FirstOrDefault(d => HttpContext.Current.Request.Url.Host.ToLowerInvariant()
                .Contains(d.ToLowerInvariant()));

            return domain.IsNullOrWhitespace();
        }
    }
}
