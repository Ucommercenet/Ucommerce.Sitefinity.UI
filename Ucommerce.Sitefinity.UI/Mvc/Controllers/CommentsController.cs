using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationComments_MVC", Title = "Comments", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnConfirmationComments sfMvcIcn")]
    public class CommentsController : Controller 
    {
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

            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName);
        }
        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private ICommentsModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<ICommentsModel>();

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}