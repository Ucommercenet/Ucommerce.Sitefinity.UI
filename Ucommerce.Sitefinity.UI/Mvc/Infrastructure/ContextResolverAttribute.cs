using System.Collections.Generic;
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
        private const string contextInitializedKey = "UCommerceContextInitialized";
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Category> CategoryIndex => ObjectFactory.Instance.Resolve<IIndex<Category>>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SystemManager.CurrentHttpContext.Items[contextInitializedKey] != null && (bool)SystemManager.CurrentHttpContext.Items[contextInitializedKey])
            {
                return;
            }

            List<string> urlSegments = null;

            var currentUrl = SystemManager.CurrentHttpContext.Request.RawUrl;
            if (currentUrl.Contains("?"))
            {
                currentUrl = currentUrl.Substring(0, currentUrl.IndexOf("?"));
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

            if (SystemManager.CurrentHttpContext.Items[contextInitializedKey] == null)
            {
                SystemManager.CurrentHttpContext.Items.Add(contextInitializedKey, true);
            }
            else
            {
                SystemManager.CurrentHttpContext.Items[contextInitializedKey] = true;
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
    }
}
