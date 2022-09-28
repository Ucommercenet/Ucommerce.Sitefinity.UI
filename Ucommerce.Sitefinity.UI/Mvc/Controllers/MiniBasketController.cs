using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Mini Basket MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uMiniBasket_MVC",
        Title = "Mini Basket",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnMiniBasket sfMvcIcn")]
    public class MiniBasketController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public Guid? CartPageId { get; set; }
        public Guid? CheckoutPageId { get; set; }
        public Guid? ProductDetailsPageId { get; set; }
        public string TemplateName { get; set; } = "Index";
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        public ActionResult Index()
        {
            var miniBasketModel = ResolveModel();
            string message;

            var parameters = new Dictionary<string, object>();

            if (!miniBasketModel.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            var miniBasketRenderingViewModel = miniBasketModel.CreateViewModel(Url.Action("Refresh"));
            var detailTemplateName = detailTemplateNamePrefix + TemplateName;

            if (!TransactionLibrary.HasBasket())
            {
                return View(detailTemplateName, miniBasketRenderingViewModel);
            }

            return View(detailTemplateName, miniBasketRenderingViewModel);
        }

        [HttpGet]
        [RelativeRoute("uc/checkout/mini-basket")]
        [RelativeRoute("{parentCategory1?}/uc/checkout/mini-basket")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/uc/checkout/mini-basket")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/uc/checkout/mini-basket")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/uc/checkout/mini-basket")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/uc/checkout/mini-basket")]
        public ActionResult Refresh()
        {
            var model = ResolveModel();
            var viewModel = model.Refresh();
            var parameters = new Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out var message))
            {
                return Json(new OperationStatusDTO { Status = "failed", Message = message }, JsonRequestBehavior.AllowGet);
            }

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";
            responseDTO.Data.Add("data", viewModel);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IMiniBasketModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                cartPageId = CartPageId,
                productDetailsPageId = ProductDetailsPageId,
                checkoutPageId = CheckoutPageId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IMiniBasketModel>(args);

            return model;
        }
    }
}
