using System;
using System.Collections.Generic;
using Ucommerce.Api;
using Ucommerce.Catalog.Status;
using Ucommerce.Infrastructure;
using Ucommerce.Pipelines;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	public class AddReviewModel : IAddReviewModel
	{
		public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		public IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();
		public Ucommerce.EntitiesV2.IRepository<Ucommerce.EntitiesV2.ProductReviewStatus> ProductReviewStatusRepository => ObjectFactory.Instance.Resolve<Ucommerce.EntitiesV2.IRepository<Ucommerce.EntitiesV2.ProductReviewStatus>>();
		public IPipeline<Ucommerce.EntitiesV2.ProductReview> ProductReviewPipeline => ObjectFactory.Instance.Resolve<IPipeline<Ucommerce.EntitiesV2.ProductReview>>();

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

		public virtual AddReviewDTO Add(AddReviewSubmitModel viewModel)
		{
			Ucommerce.EntitiesV2.Product product;

			if (viewModel.ProductId.HasValue)
			{
				product = Ucommerce.EntitiesV2.Product.Get(viewModel.ProductId.Value);
			}
			else
			{
				product = Ucommerce.EntitiesV2.Product.FirstOrDefault(p => p.Guid == CatalogContext.CurrentProduct.Guid);
			}

			var request = System.Web.HttpContext.Current.Request;
			var basket = OrderContext.GetBasket();
			var name = viewModel.Name;
			var email = viewModel.Email;
			var rating = viewModel.Rating * 20;
			var reviewHeadline = viewModel.Title;
			var reviewText = viewModel.Comments;

			if (basket.PurchaseOrder.Customer == null)
			{
				basket.PurchaseOrder.Customer = new Ucommerce.EntitiesV2.Customer()
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

			Ucommerce.EntitiesV2.ProductCatalogGroup catalogGroup;

			if (viewModel.CatalogGroupId.HasValue)
			{
				catalogGroup = Ucommerce.EntitiesV2.ProductCatalogGroup.Get(viewModel.CatalogGroupId.Value);
			}
			else
			{
				catalogGroup = Ucommerce.EntitiesV2.ProductCatalogGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentCatalogGroup.Guid);
			}

			var review = new Ucommerce.EntitiesV2.ProductReview
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
	}
}