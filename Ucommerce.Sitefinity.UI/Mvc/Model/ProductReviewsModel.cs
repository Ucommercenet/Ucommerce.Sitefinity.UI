using System.Collections.Generic;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class ProductReviewsModel : IReviewsModel
    {
        public bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = null;
            return true;
        }

        public virtual ProductReviewsRenderingViewModel GetReviews(int? productId)
        {
            var reviewVm = new ProductReviewsRenderingViewModel();
            var clientIp = System.Web.HttpContext.Current.Request.UserHostName;
            Product currentProduct;

            if (productId.HasValue && productId >= 0)
            {
                currentProduct = UCommerce.EntitiesV2.Product.Get(productId.Value);
            }
            else
            {
                currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;
            }
            

            reviewVm.Reviews = currentProduct.ProductReviews.Select(review => new ViewModels.ProductReview
            {
                Name = review.Customer.FirstName + " " + review.Customer.LastName,
                Email = review.Customer.EmailAddress,
                Title = review.ReviewHeadline,
                CreatedOn = review.CreatedOn,
                Comments = review.ReviewText,
                Ip = review.Ip,
                Rating = review.Rating
            }).ToList();
            reviewVm.Ip = clientIp;

            return reviewVm;
        }
    }
}