using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uMiniBasket_MVC", Title = "Mini Basket", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnMiniBasket sfMvcIcn")]
    public class MiniBasketController : Controller, IPersonalizable
    {
        public Guid? CartPageId { get; set; }
        public string TemplateName { get; set; } = "Index";
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly IMiniBasketService _miniBasketService;

        public MiniBasketController(IMiniBasketService miniBasketService)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            _miniBasketService = miniBasketService;
        }

        public ActionResult Index()
        {
            var miniBasketModel = ResolveModel();
            string message;

            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!miniBasketModel.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var miniBasketRenderingViewModel = miniBasketModel.CreateViewModel(Url.Action("Refresh"));

            if (!_transactionLibraryInternal.HasBasket())
            {
                return View(TemplateName, miniBasketRenderingViewModel);
            }

            return View(TemplateName, miniBasketRenderingViewModel);
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

        private IMiniBasketModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IMiniBasketModel>(
                new
                {
                    cartPageId = this.CartPageId
                });

            return model;
        }
    }
}
