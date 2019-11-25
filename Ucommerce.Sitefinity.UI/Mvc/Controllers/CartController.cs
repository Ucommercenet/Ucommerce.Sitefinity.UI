using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uCart_MVC", Title = "Cart", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class CartController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? ProductDetailsPageId { get; set; }
        public string TemplateName { get; set; } = "Index";

        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly IMiniBasketService _miniBasketService;

        public CartController(IMiniBasketService miniBasketService)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            _miniBasketService = miniBasketService;
        }

        public ActionResult Index()
        {
            if (SystemManager.IsDesignMode)
            {
                return this.PartialView("_DesignMode");
            }

            var model = ResolveModel();
            var vm = model.GetViewModel(Url.Action("UpdateBasket"), Url.Action("RemoveOrderline"));

            return View(TemplateName, vm);
        }

        [HttpPost]
        public ActionResult RemoveOrderline(int orderlineId)
        {
            _transactionLibraryInternal.UpdateLineItemByOrderLineId(orderlineId, 0);
            _transactionLibraryInternal.ExecuteBasketPipeline();
            var model = ResolveModel();
            var vm = model.GetViewModel(Url.Action("UpdateBasket"), Url.Action("RemoveOrderline"));

            return Json(new
            {
                MiniBasketRefresh = _miniBasketService.Refresh(),
                orderlineId,
                vm.OrderTotal,
                vm.DiscountTotal,
                vm.TaxTotal,
                vm.SubTotal,
                vm.OrderLines
            });
        }

        [HttpPost]
        public ActionResult UpdateBasket(CartUpdateBasket updateModel)
        {
            var model = ResolveModel();
            var updatedVM = model.Update(updateModel);

            return Json(new
            {
                MiniBasketRefresh = _miniBasketService.Refresh(),
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
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<ICartModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    productDetailsPageId = this.ProductDetailsPageId
                });

            return model;
        }
    }
}
