using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Infrastructure;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uFacetsFilter_MVC", Title = "Facets Filter", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class FacetsFilterController : Controller
    {
        public FacetsFilterController(IModelFactory modelFactory)
        {
            this.modelFactory = modelFactory;
        }

        public string TemplateName { get; set; } = "FacetsFilter.Main";

        [RelativeRoute("{categoryName?}/{page?}")]
        public ActionResult Index(string categoryName, int? page)
        {
            try
            {
                var model = this.ResolveModel();
                var viewModel = model.CreateViewModel(categoryName);

                return this.View(this.TemplateName, viewModel);
            }
            catch (Exception ex)
            {
                if (UcommerceUIModule.TryHandleSystemError(ex, out ActionResult actionResult))
                {
                    return actionResult;
                }
                else
                {
                    throw;
                }
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IFacetsFilterModel ResolveModel()
        {
            return modelFactory.CreateFacetsFilterModel();
        }

        private IModelFactory modelFactory;
    }
}