using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uProducts_MVC", Title = "Products", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "ucIcnProducts sfMvcIcn")]
    public class ProductsController : Controller, IPersonalizable
    {
        public int ItemsPerPage { get; set; } = 10;

        public bool OpenInSamePage { get; set; }

        public Guid DetailsPageId { get; set; }

        public bool IsManualSelectionMode { get; set; }

        public string ProductIds { get; set; }

        public string CategoryIds { get; set; }

        public string ListTemplateName { get; set; } = "Index";

        public string DetailTemplateName { get; set; } = "Index";

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
                if (SystemManager.IsDesignMode)
                {
                    return this.PartialView("_DesignMode");
                }

                var productModel = this.ResolveModel();
                viewModel = productModel.CreateListViewModel();

                return this.View(this.ListTemplateName, viewModel);
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

        [RelativeRoute("{categoryName}/{productId:int}")]
        [RelativeRoute("{parentCategory1?}/{categoryName}/{productId:int}")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName}/{productId:int}")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/{productId:int}")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/{productId:int}")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/{productId:int}")]
        public ActionResult Details()
        {
            var productModel = this.ResolveModel();
            var viewModel = productModel.CreateDetailsViewModel();

            return this.View(this.DetailTemplateName, viewModel);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IProductModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IProductModel>(
                new
                {
                    itemsPerPage = this.ItemsPerPage,
                    openInSamePage = this.OpenInSamePage,
                    isManualSelectionMode = this.IsManualSelectionMode,
                    detailsPageId = this.DetailsPageId,
                    productIds = this.ProductIds,
                    categoryIds = this.CategoryIds
                });

            return model;
        }
    }
}