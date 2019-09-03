using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using UCommerce.Runtime;

namespace Ucommerce.Sitefinity.UI.Mvc.Filters
{
    internal class ContextResolverAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<string> urlSegments = null;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                urlSegments = SystemManager.CurrentHttpContext.Request.Url.Segments.Select(i => i.Replace("/", string.Empty)).ToList();
            }
            else
            {
                var currentNode = SiteMapBase.GetActualCurrentNode();

                if (currentNode != null)
                {
                    urlSegments = currentNode.Url.Split('/').ToList();
                }
            }

            if (urlSegments != null)
            {
                this.ResolveCurrentProduct(urlSegments);
                this.ResolveCurrentCategory(urlSegments);
            }
        }

        private void ResolveCurrentProduct(List<string> urlSegments)
        {
            if (SiteContext.Current.CatalogContext.CurrentProduct == null)
            {
                var products = UCommerce.EntitiesV2.Product.All().Where(p => urlSegments.Contains(p.ProductId.ToString())).ToList();

                if (products != null && products.Count > 0)
                {
                    for (int i = urlSegments.Count - 1; i >= 0; i--)
                    {
                        var product = products.FirstOrDefault(p => urlSegments.Contains(p.Id.ToString()));
                        if (product != null)
                        {
                            SiteContext.Current.CatalogContext.CurrentProduct = product;
                            break;
                        }
                    }
                }
            }
        }

        private void ResolveCurrentCategory(List<string> urlSegments)
        {
            if (SiteContext.Current.CatalogContext.CurrentCategory == null)
            {
                var categories = UCommerce.EntitiesV2.Category.All().Where(p => urlSegments.Contains(p.Name.ToString())).ToList();

                if (categories != null && categories.Count > 0)
                {
                    for (int i = urlSegments.Count - 1; i >= 0; i--)
                    {
                        var cat = categories.FirstOrDefault(p => urlSegments.Contains(p.Name.ToString()));

                        if (cat != null)
                        {
                            SiteContext.Current.CatalogContext.CurrentCategory = cat;
                            break;
                        }
                    }
                }
            }
        }
    }
}
