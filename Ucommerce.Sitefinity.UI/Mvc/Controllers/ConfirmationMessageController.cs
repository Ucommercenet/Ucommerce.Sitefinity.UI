using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model.Interfaces;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationMessage_MVC", Title = "Confirmation Message", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnConfirmationMessage sfMvcIcn")]
    public class ConfirmationMessageController : Controller, IPersonalizable
    {
        public string Headline { get; set; }
        public string Message { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var confirmationMessageVM = model.GetViewModel(Headline, Message);

            return View(this.TemplateName, confirmationMessageVM);
        }

        public IConfirmationMessageModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IConfirmationMessageModel>();

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
