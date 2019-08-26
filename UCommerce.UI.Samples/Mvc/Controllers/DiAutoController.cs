using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI;
using Ucommerce.UI.Samples.Mvc.Models;

namespace Ucommerce.UI.Samples.Mvc.Controllers
{
    /// <summary>
    /// This sample widget controller resolves dependencies that are automatically registered in the DI container on application start.
    /// </summary>
    [ControllerToolboxItem(Name = "autoDependencyInjection_MVC", Title = "Automatic Dependency Injection", SectionName = "UCommerceSamples", CssClass = "sfMvcIcn")]
    public class DiAutoController : Controller
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

        private IDiAutoModel ResolveModel()
        {
            return UcommerceUIModule.Container.Resolve<IDiAutoModel>();
        }
    }
}