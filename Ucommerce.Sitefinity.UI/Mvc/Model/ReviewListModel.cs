using System.Collections.Generic;
using System.Linq;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class ReviewListModel : IReviewListModel
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

        public virtual ReviewListRenderingViewModel GetReviews()
        {
            var reviewVm = new ReviewListRenderingViewModel();

            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            reviewVm.Reviews = currentProduct.ProductReviews.Select(review => new Review
            {
                Name = review.Customer.FirstName + " " + review.Customer.LastName,
                Email = review.Customer.EmailAddress,
                Title = review.ReviewHeadline,
                CreatedOn = review.CreatedOn,
                Comments = review.ReviewText,
                Rating = review.Rating
            }).ToList();

            return reviewVm;
        }
    }
}