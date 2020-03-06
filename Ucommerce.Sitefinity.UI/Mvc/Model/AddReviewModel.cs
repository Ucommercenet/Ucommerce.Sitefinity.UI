using System;
using System.Collections.Generic;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class AddReviewModel : IAddReviewModel
    {
        private readonly IRepository<ProductReviewStatus> _productReviewStatusRepository;
        private readonly IOrderContext _orderContext;
        private readonly IPipeline<ProductReview> _productReviewPipeline;

        public AddReviewModel()
        {
            _productReviewStatusRepository = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>();
            _orderContext = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IOrderContext>();
            _productReviewPipeline = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IPipeline<ProductReview>>();
        }
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

        public virtual AddReviewDTO Add(AddReviewSaveViewModel viewModel)
        {
            var product = SiteContext.Current.CatalogContext.CurrentProduct;
            var request = System.Web.HttpContext.Current.Request;
            var basket = _orderContext.GetBasket();
            var name = viewModel.Name;
            var email = viewModel.Email;
            var rating = viewModel.Rating * 20;
            var reviewHeadline = viewModel.Title;
            var reviewText = viewModel.Comments;

            if (basket.PurchaseOrder.Customer == null)
            {
                basket.PurchaseOrder.Customer = new Customer()
                {
                    FirstName = name,
                    LastName = String.Empty,
                    EmailAddress = email
                };
            }
            else
            {
                basket.PurchaseOrder.Customer.FirstName = name;
                if (basket.PurchaseOrder.Customer.LastName == null)
                {
                    basket.PurchaseOrder.Customer.LastName = String.Empty;
                }
                basket.PurchaseOrder.Customer.EmailAddress = email;
            }

            basket.PurchaseOrder.Customer.Save();

            var review = new ProductReview();
            review.ProductCatalogGroup = SiteContext.Current.CatalogContext.CurrentCatalogGroup;
            review.ProductReviewStatus = _productReviewStatusRepository.SingleOrDefault(s => s.Name == "New");
            review.CreatedOn = DateTime.Now;
            review.CreatedBy = "System";
            review.Product = product;
            review.Customer = basket.PurchaseOrder.Customer;
            review.Rating = rating;
            review.ReviewHeadline = reviewHeadline;
            review.ReviewText = reviewText;
            review.Ip = request.UserHostName;

            var reviewDTO = new AddReviewDTO
            {
                Rating = rating,
                Comments = reviewText,
                ReviewHeadline = reviewHeadline,
                CreatedBy = review.CreatedBy,
                CreatedOn = review.CreatedOn.ToString("MMM dd, yyyy"),
                CreatedOnForMeta = review.CreatedOn.ToString("yyyy-MM-dd")
            };

            product.AddProductReview(review);

            _productReviewPipeline.Execute(review);

            return reviewDTO;
        }
    }
}