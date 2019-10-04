using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationMessage_MVC", Title = "Confirmation Message", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class ConfirmationMessageController : Controller
    {
        public string Headline { get; set; }
        public string Message { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            var confirmationMessageVM = model.GetViewModel(Headline, Message);

            return View(TemplateName, confirmationMessageVM);
        }

        public IConfirmationMessageModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IConfirmationMessageModel>();

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
