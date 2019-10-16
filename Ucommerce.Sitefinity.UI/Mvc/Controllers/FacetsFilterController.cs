using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uFacetsFilter_MVC", Title = "Facets Filter", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class FacetsFilterController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "B4Index";

        [RelativeRoute("{categoryName?}")]
        [RelativeRoute("{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        public ActionResult Index()
        {
            try
            {
                if (SystemManager.IsDesignMode)
                {
                    return new EmptyResult();
                }

                var model = this.ResolveModel();
                var viewModel = model.CreateViewModel();

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
            return UcommerceUIModule.Container.Resolve<IFacetsFilterModel>();
        }
    }
}