using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationEmail_MVC", Title = "Confirmation Email", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "ucIcnConfirmationEmail sfMvcIcn")]
    public class ConfirmationEmailController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            var orderGuid = System.Web.HttpContext.Current.Request.QueryString["orderGuid"];
            string message;

            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            parameters.Add("orderGuid", orderGuid);

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var confirmationEmailVM = model.GetViewModel(orderGuid);

            ViewBag.RowSpan = 4;
            if (confirmationEmailVM.DiscountAmount > 0)
            {
                ViewBag.RowSpan++;
            }
            if (confirmationEmailVM.ShipmentAmount > 0)
            {
                ViewBag.RowSpan++;
            }
            if (confirmationEmailVM.PaymentAmount > 0)
            {
                ViewBag.RowSpan++;
            }

            return View(this.TemplateName, confirmationEmailVM);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        public IConfirmationEmailModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IConfirmationEmailModel>();

            return model;
        }
    }
}
