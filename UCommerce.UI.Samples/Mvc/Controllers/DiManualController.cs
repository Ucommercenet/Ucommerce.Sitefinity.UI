using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI;
using Ucommerce.UI.Samples.Mvc.Models;

namespace Ucommerce.UI.Samples.Mvc.Controllers
{
    /// <summary>
    /// This sample widget controller resolves dependencies that are manually registered in the DI container after the container is initialized.
    /// </summary>
    [ControllerToolboxItem(Name = "manualDependencyInjection_MVC", Title = "Manual Dependency Injection", SectionName = "UCommerceSamples", CssClass = "sfMvcIcn")]
    public class DiManualController : Controller
    {
        public string TemplateName { get; set; } = "Default";

        public ActionResult Index()
        {
            var model = this.ResolveModel();
            var viewModel = model.GetViewModel();

            return View(this.TemplateName, viewModel);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IDiManualModel ResolveModel()
        {
            return UcommerceUIModule.Container.Resolve<IDiManualModel>();
        }
    }
}