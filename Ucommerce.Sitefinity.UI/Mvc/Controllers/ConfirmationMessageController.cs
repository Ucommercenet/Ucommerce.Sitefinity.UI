using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Confirmation Message MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uConfirmationMessage_MVC",
        Title = "Confirmation Message",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnConfirmationMessage sfMvcIcn")]
    public class ConfirmationMessageController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public string Headline { get; set; }
        public string Message { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            string message;
            var parameters = new Dictionary<string, object>();

            var orderGuid = Request.QueryString["orderGuid"];
            if (!model.CanProcessRequest(parameters, orderGuid, out message))
            {
                return PartialView("_Warning", message);
            }

            var confirmationMessageVM = model.GetViewModel(Headline, Message, orderGuid);
            var detailTemplateName = detailTemplateNamePrefix + TemplateName;

            return View(detailTemplateName, confirmationMessageVM);
        }

        public IConfirmationMessageModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IConfirmationMessageModel>();

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }
    }
}
