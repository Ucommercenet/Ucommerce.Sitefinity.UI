using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationEmail_MVC", Title = "Confirmation Email", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class ConfirmationEmailController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var orderGuid = System.Web.HttpContext.Current.Request.QueryString["orderGuid"];

            if(SystemManager.IsDesignMode)
            {
                return this.PartialView("_DesignMode");
            }
            else if (string.IsNullOrWhiteSpace(orderGuid))
            {
                Log.Write(new Exception("Can't resolve orderGuid! Confirmation Email can't be sent."));
                return this.BlankOrder();
            }

            PurchaseOrder purchaseOrder = new PurchaseOrder();
            var model = ResolveModel();
            var confirmationEmailVM = model.GetViewModel(orderGuid);

            ViewBag.RowSpan = 4;
            if (purchaseOrder.DiscountTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (purchaseOrder.ShippingTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (purchaseOrder.PaymentTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            return View(this.TemplateName, confirmationEmailVM);
        }

        public ActionResult BlankOrder()
        {
            return new EmptyResult();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "BlankOrder");
        }

        public ConfirmationEmailModel ResolveModel()
        {
            var model = new ConfirmationEmailModel();

            return model;
        }
    }
}
