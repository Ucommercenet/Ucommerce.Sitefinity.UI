using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationEmail_MVC", Title = "Confirmation Email", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class ConfirmationEmailController : Controller
    {
        public ActionResult Index()
        {
            var controller = new ConfirmationEmailController();
            var model = ResolveModel();
            var confirmationEmailVM = model.GetViewModel(controller);

            return View("Index", confirmationEmailVM);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        public ConfirmationEmailModel ResolveModel()
        {
            var model = new ConfirmationEmailModel();

            return model;
        }
    }
}
