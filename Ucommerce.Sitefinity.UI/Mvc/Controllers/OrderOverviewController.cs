using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uOrderOverview_MVC", Title = "Order Overview", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class OrderOverviewController : Controller
    {
        public Guid? NextStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            var vm = model.GetViewModel();

            return View(TemplateName, vm);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IOrderOverviewModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IOrderOverviewModel>(
                new
                {
                    nextStepId = this.NextStepId
                });

            return model;
        }
    }
}
