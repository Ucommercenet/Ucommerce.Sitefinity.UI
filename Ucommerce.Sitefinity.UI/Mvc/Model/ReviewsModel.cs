using System.Collections.Generic;
using System.Linq;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class ReviewsModel : IReviewsModel
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

        public virtual ReviewsRenderingViewModel GetReviews()
        {
            var reviewVm = new ReviewsRenderingViewModel();
            var clientIp = System.Web.HttpContext.Current.Request.UserHostName;
            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            reviewVm.Reviews = currentProduct.ProductReviews.Select(review => new Review
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