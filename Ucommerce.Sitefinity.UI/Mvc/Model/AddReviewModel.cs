using System;
using System.Collections.Generic;
using System.Web;
using Telerik.Sitefinity.Services;
using Ucommerce.Api;
using Ucommerce.Catalog.Status;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Pipelines;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using ProductReview = Ucommerce.EntitiesV2.ProductReview;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class AddReviewModel : IAddReviewModel
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();
        public IPipeline<ProductReview> ProductReviewPipeline => ObjectFactory.Instance.Resolve<IPipeline<ProductReview>>();
        public IRepository<ProductReviewStatus> ProductReviewStatusRepository => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>();

        public virtual AddReviewDTO Add(AddReviewSubmitModel viewModel)
        {
            Product product;

            if (viewModel.ProductId.HasValue)
            {
                product = Product.Get(viewModel.ProductId.Value);
            }
            else
            {
                product = Product.FirstOrDefault(p => p.Guid == CatalogContext.CurrentProduct.Guid);
            }

            var request = HttpContext.Current.Request;
            var basket = OrderContext.GetBasket();
            var name = viewModel.Name;
            var email = viewModel.Email;
            var rating = viewModel.Rating * 20;
            var reviewHeadline = viewModel.Title;
            var reviewText = viewModel.Comments;

            if (basket.PurchaseOrder.Customer == null)
            {
                basket.PurchaseOrder.Customer = new Customer
                {
                    FirstName = name,
                    LastName = string.Empty,
                    EmailAddress = email
                };
            }
            else
            {
                basket.PurchaseOrder.Customer.FirstName = name;
                if (basket.PurchaseOrder.Customer.LastName == null)
                {
                    basket.PurchaseOrder.Customer.LastName = string.Empty;
                }

                basket.PurchaseOrder.Customer.EmailAddress = email;
            }

            basket.PurchaseOrder.Customer.Save();

            ProductCatalogGroup catalogGroup;

            if (viewModel.CatalogGroupId.HasValue)
            {
                catalogGroup = ProductCatalogGroup.Get(viewModel.CatalogGroupId.Value);
            }
            else
            {
                catalogGroup = ProductCatalogGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentCatalogGroup.Guid);
            }

            var review = new ProductReview
            {
                ProductCatalogGroup = catalogGroup,
                ProductReviewStatus = ProductReviewStatusRepository.SingleOrDefault(s => s.ProductReviewStatusId == (int)ProductReviewStatusCode.New),
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
                Product = product,
                Customer = basket.PurchaseOrder.Customer,
                Rating = rating,
                ReviewHeadline = reviewHeadline,
                ReviewText = reviewText,
                Ip = request.UserHostName
            };

            product.AddProductReview(review);
            ProductReviewPipeline.Execute(review);

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

            ProductReviewPipeline.Execute(review);
            return reviewDTO;
        }

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
    }
}
