using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Products MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uProducts_MVC",
        Title = "Products",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnProducts sfMvcIcn")]
    public class ProductsController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        private readonly string listTemplateNamePrefix = "List.";
        public string CategoryIds { get; set; }
        public Guid DetailsPageId { get; set; }
        public string DetailTemplateName { get; set; } = "Index";
        public bool EnableCategoryFallback { get; set; }
        public bool EnableSEOPageTitle { get; set; } = true;
        public string FallbackCategoryIds { get; set; }
        public bool IsManualSelectionMode { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public string ListTemplateName { get; set; } = "Index";
        public bool OpenInSamePage { get; set; } = true;
        public string ProductIds { get; set; }

        [RelativeRoute("{categoryName}/p/{productId}")]
        [RelativeRoute("{parentCategory1?}/{categoryName}/p/{productId}")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
        public ActionResult Details()
        {
            var productModel = ResolveModel();
            string message;
            var parameters = new Dictionary<string, object>();

            if (!productModel.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            var viewModel = productModel.CreateDetailsViewModel();
            var templateName = detailTemplateNamePrefix + DetailTemplateName;

            if (EnableSEOPageTitle)
            {
                ViewBag.Title = viewModel.DisplayName;
            }

            return View(templateName, viewModel);
        }

        [OutputCache(Duration = 30, VaryByParam = "*")]
        [RelativeRoute("{categoryName?}")]
        [RelativeRoute("{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
        public ActionResult Index()
        {
            ProductListViewModel viewModel;
            try
            {
                var productModel = ResolveModel();
                string message;
                var parameters = new Dictionary<string, object>();

                if (!productModel.CanProcessRequest(parameters, out message))
                {
                    return PartialView("_Warning", message);
                }

                viewModel = productModel.CreateListViewModel();
                var templateName = listTemplateNamePrefix + ListTemplateName;

                return View(templateName, viewModel);
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

        private IProductModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                itemsPerPage = ItemsPerPage,
                openInSamePage = OpenInSamePage,
                isManualSelectionMode = IsManualSelectionMode,
                enableCategoryFallback = EnableCategoryFallback,
                detailsPageId = DetailsPageId,
                productIds = ProductIds,
                categoryIds = CategoryIds,
                fallbackCategoryIds = FallbackCategoryIds
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IProductModel>(args);

            return model;
        }
    }
}
