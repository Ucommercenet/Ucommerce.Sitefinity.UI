using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
	/// <summary>
	/// The controller class for the Cart MVC widget.
	/// </summary>
	[ControllerToolboxItem(Name = "uCart_MVC", Title = "Cart", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnCart sfMvcIcn")]
	public class CartController : Controller, IPersonalizable
	{
		public Guid? NextStepId { get; set; }
		public Guid? ProductDetailsPageId { get; set; }
		public Guid? RedirectPageId { get; set; }
		public string TemplateName { get; set; } = "Index";

		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
		public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

		public ActionResult Index()
		{
			var model = ResolveModel();
			string message;
			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

			return View(detailTemplateName);
		}

		[HttpGet]
		[RelativeRoute("uc/checkout/cart")]
		public ActionResult Data()
		{
			var model = ResolveModel();
			string message;
			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.Json(new OperationStatusDTO() { Status = "failed", Message = message }, JsonRequestBehavior.AllowGet);
			}

			var vm = model.GetViewModel(Url.Action("UpdateBasket"), Url.Action("RemoveOrderline"));

			var responseDTO = new OperationStatusDTO();
			responseDTO.Status = "success";
			responseDTO.Data.Add("data", vm);

			return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[RelativeRoute("uc/checkout/cart/remove-orderline")]
		public ActionResult RemoveOrderline(int orderlineId)
		{
			var model = ResolveModel();
			var parameters = new System.Collections.Generic.Dictionary<string, object>();
			string message;

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var orderLine = Ucommerce.EntitiesV2.OrderLine.Get(orderlineId);
			var product = Ucommerce.EntitiesV2.Product.FirstOrDefault(p => p.Sku == orderLine.Sku && p.VariantSku == orderLine.VariantSku);
			InsightUcommerce.SendProductInteraction(product, "Remove product from cart", $"{product?.Name} ({product?.Sku})");

			TransactionLibrary.UpdateLineItemByOrderLineId(orderlineId, 0);
			TransactionLibrary.ExecuteBasketPipeline();

			var miniBasketModel = ResolveMiniBasketModel();
			var refresh = miniBasketModel.Refresh();
			return Json(new
			{
				MiniBasketRefresh = refresh,
				orderlineId,
				refresh.Total,
				refresh.DiscountTotal,
				refresh.TaxTotal,
				refresh.SubTotal,
				refresh.OrderLines,
				refresh.CartPageUrl
			});
		}

		[HttpPost]
		[RelativeRoute("uc/checkout/cart/update-basket")]
		public ActionResult UpdateBasket(CartUpdateBasket updateModel)
		{
			var model = ResolveModel();
			var parameters = new System.Collections.Generic.Dictionary<string, object>();
			string message;

			parameters.Add("submitModel", updateModel);

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.Json(new OperationStatusDTO() { Status = "failed", Message = message },
				   JsonRequestBehavior.AllowGet);
			}

			var updatedVM = model.Update(updateModel);

			var miniBasketModel = ResolveMiniBasketModel();

			return Json(new
			{
				MiniBasketRefresh = miniBasketModel.Refresh(),
				updatedVM.OrderTotal,
				updatedVM.DiscountTotal,
				updatedVM.TaxTotal,
				updatedVM.SubTotal,
				updatedVM.OrderLines
			});
		}

		[HttpPost]
		[RelativeRoute("uc/checkout/cart/vouchers/add")]
		public ActionResult AddVoucher(CartUpdateBasket updateModel)
		{
			var model = ResolveModel();
			var parameters = new System.Collections.Generic.Dictionary<string, object>();
			string message;

			parameters.Add("submitModel", updateModel);

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.Json(new OperationStatusDTO() { Status = "failed", Message = message },
					JsonRequestBehavior.AllowGet);
			}

			var updatedVM = model.AddVoucher(updateModel);

			return Json(new
			{
				updatedVM.OrderTotal,
				updatedVM.DiscountTotal,
				updatedVM.TaxTotal,
				updatedVM.SubTotal,
				Voucher = updatedVM.Vouchers,
				updatedVM.OrderLines
			});
		}

		[HttpPost]
		[RelativeRoute("uc/checkout/cart/vouchers/remove")]
		public ActionResult RemoveVoucher(CartUpdateBasket updateModel)
		{
			var model = ResolveModel();
			var parameters = new System.Collections.Generic.Dictionary<string, object>();
			string message;

			parameters.Add("submitModel", updateModel);

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.Json(new OperationStatusDTO() { Status = "failed", Message = message },
					JsonRequestBehavior.AllowGet);
			}

			var updatedVM = model.RemoveVoucher(updateModel);

			return Json(new
			{
				updatedVM.OrderTotal,
				updatedVM.DiscountTotal,
				updatedVM.TaxTotal,
				updatedVM.SubTotal,
				Voucher = updatedVM.Vouchers,
				updatedVM.OrderLines
			});
		}

		protected override void HandleUnknownAction(string actionName)
		{
			this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
		}

		private ICartModel ResolveModel()
		{
			var args = new Castle.MicroKernel.Arguments();

			args.AddProperties(new
			{
				nextStepId = this.NextStepId,
				productDetailsPageId = this.ProductDetailsPageId,
				redirectPageId = this.RedirectPageId
			});

			var container = UCommerceUIModule.Container;
			var model = container.Resolve<ICartModel>(args);

			return model;
		}

		private IMiniBasketModel ResolveMiniBasketModel()
		{
			var args = new Castle.MicroKernel.Arguments();

			args.AddProperties(new
			{
				cartPageId = Guid.Empty
			});

			var container = UCommerceUIModule.Container;
			var model = container.Resolve<IMiniBasketModel>(args);

			return model;
		}

		private string detailTemplateNamePrefix = "Detail.";
	}
}
