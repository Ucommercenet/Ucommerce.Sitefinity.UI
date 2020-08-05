using System.Collections.Generic;
using System.Linq;
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
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        public IIndex<Category> CategoryIndex => ObjectFactory.Instance.Resolve<IIndex<Category>>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SystemManager.CurrentHttpContext.Items[contextInitializedKey] == null || (bool)SystemManager.CurrentHttpContext.Items[contextInitializedKey] == false)
            {
                List<string> urlSegments = null;

                if (SystemManager.CurrentHttpContext.Request.Url != null)
                {
                    urlSegments = SystemManager.CurrentHttpContext.Request.Url.Segments
                        .Select(i => System.Web.HttpUtility.UrlDecode(i.Replace("/", string.Empty)))
                        .ToList();
                }
                else
                {
                    var currentNode = SiteMapBase.GetActualCurrentNode();

                    if (currentNode != null)
                    {
                        urlSegments = currentNode.Url.Split('/')
                            .Select(i => System.Web.HttpUtility.UrlDecode(i))
                            .ToList();
                    }
                }

                if (urlSegments != null)
                {
                    this.ResolveCurrentProduct(urlSegments);
                    this.ResolveCurrentCategory(urlSegments);
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
        }

        private void ResolveCurrentProduct(List<string> urlSegments)
        {
            if (CatalogContext.CurrentProduct == null)
            {
                var products = ProductIndex.Find()
                    .Where(p => urlSegments.Contains(p.Slug.ToString()))
                    .ToList();

                if (products != null)
                {
                    for (int i = urlSegments.Count - 1; i >= 0; i--)
                    {
                        var product = products.FirstOrDefault(p => urlSegments.Contains(p.Slug));
                        if (product != null)
                        {
                            CatalogContext.CurrentProduct = product;
                            break;
                        }
                    }
                }
            }
        }

        private void ResolveCurrentCategory(List<string> urlSegments)
        {
            if (CatalogContext.CurrentCategory == null)
            {
                var categories = CategoryIndex.Find()
                    .Where(c => urlSegments.Contains(c.Name.ToString()) && c.ProductCatalog == CatalogContext.CurrentCatalog.Guid)
                    .ToList();

                if (categories != null)
                {
                    for (int i = urlSegments.Count - 1; i >= 0; i--)
                    {
                        var cat = categories.FirstOrDefault(p => urlSegments[i] == p.Name);

                        if (cat != null)
                        {
                            CatalogContext.CurrentCategory = cat;
                            break;
                        }
                    }
                }
            }
        }

        private const string contextInitializedKey = "UCommerceContextInitialized";
    }
}
