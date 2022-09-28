using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Facets Filter MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uFacetsFilter_MVC",
        Title = "Facets Filter",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnFacetsFilter sfMvcIcn")]
    public class FacetsFilterController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public string TemplateName { get; set; } = "Index";

        [OutputCache(Duration = 30, VaryByParam = "*")]
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
                var model = ResolveModel();
                string message;

                var parameters = new Dictionary<string, object>();

                if (!model.CanProcessRequest(parameters, out message))
                {
                    return PartialView("_Warning", message);
                }

                var viewModel = model.CreateViewModel();
                var detailTemplateName = detailTemplateNamePrefix + TemplateName;

                return View(detailTemplateName, viewModel);
            }
            catch (Exception ex)
            {
                if (UCommerceUIModule.TryHandleSystemError(ex, out var actionResult))
                {
                    return actionResult;
                }

                throw;
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IFacetsFilterModel ResolveModel()
        {
            return UCommerceUIModule.Container.Resolve<IFacetsFilterModel>();
        }
    }
}
