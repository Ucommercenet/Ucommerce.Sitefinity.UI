using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uCart_MVC", Title = "Cart", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class CartController : Controller
    {
        public Guid? NextStepId { get; set; }
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
            var model = ResolveModel();
            var vm = model.CreateViewModel(Url.Action("UpdateBasket"), Url.Action("RemoveOrderline"));

            return View(TemplateName, vm);
        }

        [HttpPost]
        public ActionResult RemoveOrderline(int orderlineId)
        {
            _transactionLibraryInternal.UpdateLineItemByOrderLineId(orderlineId, 0);

            return Json(new
            {
                orderlineId
            });
        }

        [HttpPost]
        public ActionResult UpdateBasket(CartUpdateBasket updateModel)
        {
            var model = ResolveModel();
            var updatedVM = model.UpdateViewModel(updateModel);

            return Json(new
            {
                MiniBasketRefresh = _miniBasketService.Refresh(),
                updatedVM.OrderTotal,
                updatedVM.DiscountTotal,
                updatedVM.TaxTotal,
                updatedVM.SubTotal,
                updatedVM.Orderlines
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
                    nextStepId = this.NextStepId
                });

            return model;
        }
    }
}
