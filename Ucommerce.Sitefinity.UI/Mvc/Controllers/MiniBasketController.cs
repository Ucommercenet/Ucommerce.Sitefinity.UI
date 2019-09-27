using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uMiniBasket_MVC", Title = "Mini Basket", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class MiniBasketController : Controller
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly IMiniBasketService _miniBasketService;

        public MiniBasketController(IMiniBasketService miniBasketService)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            _miniBasketService = miniBasketService;
        }

        public ActionResult Index()
        {
            var miniBasketViewModel = new MiniBasketRenderingViewModel
            {
                IsEmpty = true,
                RefreshUrl = Url.Action("Refresh")
            };

            if (!_transactionLibraryInternal.HasBasket())
            {
                return View("Index", miniBasketViewModel);
            }

            var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            miniBasketViewModel.NumberOfItems = purchaseOrder.OrderLines.Sum(x => x.Quantity);
            miniBasketViewModel.IsEmpty = miniBasketViewModel.NumberOfItems == 0;
            miniBasketViewModel.Total = purchaseOrder.OrderTotal.HasValue ? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency) : new Money(0, purchaseOrder.BillingCurrency);

            return View("Index", miniBasketViewModel);
        }

        [HttpGet]
        public ActionResult Refresh()
        {
            return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
