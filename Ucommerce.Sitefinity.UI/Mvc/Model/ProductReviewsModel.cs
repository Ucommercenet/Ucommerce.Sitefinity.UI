using System.Collections.Generic;
using System.Linq;
using UCommerce.Runtime;
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

        public virtual ProductReviewsRenderingViewModel GetReviews()
        {
            var reviewVm = new ProductReviewsRenderingViewModel();
            var clientIp = System.Web.HttpContext.Current.Request.UserHostName;
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            reviewVm.Reviews = currentProduct.ProductReviews.Select(review => new ProductReview
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