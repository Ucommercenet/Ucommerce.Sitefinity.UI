using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Facets Filter MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uFacetsFilter_MVC", Title = "Facets Filter", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnFacetsFilter sfMvcIcn")]
    public class FacetsFilterController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

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
                var model = this.ResolveModel();
                string message;

                var parameters = new System.Collections.Generic.Dictionary<string, object>();

                if (!model.CanProcessRequest(parameters, out message))
                {
                    return this.PartialView("_Warning", message);
                }

                var viewModel = model.CreateViewModel();
                var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

                return this.View(detailTemplateName, viewModel);
            }
            catch (Exception ex)
            {
                if (UCommerceUIModule.TryHandleSystemError(ex, out ActionResult actionResult))
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
            return UCommerceUIModule.Container.Resolve<IFacetsFilterModel>();
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}