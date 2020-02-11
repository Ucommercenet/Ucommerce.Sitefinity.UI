using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;
using UCommerce.Sitefinity.UI.Api.Model;

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
        public string TemplateName { get; set; } = "Index";

        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public CartController()
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
        }

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

            _transactionLibraryInternal.UpdateLineItemByOrderLineId(orderlineId, 0);
            _transactionLibraryInternal.ExecuteBasketPipeline();
            var vm = model.GetViewModel(Url.Action("UpdateBasket"), Url.Action("RemoveOrderline"));

            var miniBasketModel = ResolveMiniBasketModel();

            return Json(new
            {
                MiniBasketRefresh = miniBasketModel.Refresh(),
                orderlineId,
                vm.OrderTotal,
                vm.DiscountTotal,
                vm.TaxTotal,
                vm.SubTotal,
                vm.OrderLines
            });
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/cart/update-basket")]
        public ActionResult UpdateBasket(CartUpdateBasket updateModel)
        {
            var model = ResolveModel();
            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            string message;

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
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

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private ICartModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<ICartModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    productDetailsPageId = this.ProductDetailsPageId
                });

            return model;
        }

        private IMiniBasketModel ResolveMiniBasketModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IMiniBasketModel>(
                new
                {
                    cartPageId = Guid.Empty
                });

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}
