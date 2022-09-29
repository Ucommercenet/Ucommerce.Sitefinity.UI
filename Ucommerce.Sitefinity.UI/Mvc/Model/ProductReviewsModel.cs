using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Telerik.Sitefinity.Services;
using Ucommerce.Api;
using Ucommerce.Catalog.Status;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using ProductReview = UCommerce.Sitefinity.UI.Mvc.ViewModels.ProductReview;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class ProductReviewsModel : IReviewsModel
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        public bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = null;
            return true;
        }

        public virtual ProductReviewsRenderingViewModel GetReviews(int? productId)
        {
            var clientIp = HttpContext.Current.Request.UserHostName;
            Product currentProduct = null;

            if (productId >= 0)
            {
                currentProduct = Product.Get(productId.Value);
            }
            else if (CatalogContext.CurrentProduct?.Guid != null)
            {
                currentProduct = Product.FirstOrDefault(x => x.Guid == CatalogContext.CurrentProduct.Guid);
            }

            var reviewVm = new ProductReviewsRenderingViewModel
            {
                Ip = clientIp
            };

            if (currentProduct == null)
            {
                return reviewVm;
            }

            reviewVm.Reviews = currentProduct.ProductReviews.Where(pr => pr.ProductReviewStatus.ProductReviewStatusId == (int)ProductReviewStatusCode.Approved
                    && (pr.CultureCode == null || pr.CultureCode == string.Empty || pr.CultureCode == Thread.CurrentThread.CurrentUICulture.Name))
                .OrderByDescending(pr => pr.CreatedOn)
                .Select(review => new ProductReview
                {
                    Name = review.Customer == null ? "(anonymous)" : review.Customer.FirstName + " " + review.Customer.LastName,
                    Email = review.Customer?.EmailAddress,
                    Title = review.ReviewHeadline,
                    CreatedOn = review.CreatedOn,
                    Comments = review.ReviewText,
                    Ip = review.Ip,
                    Rating = review.Rating
                })
                .ToList();

            return reviewVm;
        }
    }
}
