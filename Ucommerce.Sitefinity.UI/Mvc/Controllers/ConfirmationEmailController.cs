using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Confirmation Email MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uConfirmationEmail_MVC", Title = "Confirmation Email", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnConfirmationEmail sfMvcIcn")]
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

            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName, confirmationEmailVM);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        public IConfirmationEmailModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IConfirmationEmailModel>();

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}
