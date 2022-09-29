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
    /// The controller class for the Category Navigation MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uCategoryNavigation_MVC",
        Title = "Category Navigation",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnCategoryNavigation sfMvcIcn")]
    public class CategoryNavigationController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public bool AllowChangingCurrency { get; set; } = true;
        public Guid? CategoryPageId { get; set; }
        public bool HideMiniBasket { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? ProductDetailsPageId { get; set; }
        public string ProviderName { get; set; }
        public Guid? SearchPageId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            CategoryNavigationViewModel categoryNavigationViewModel = null;

            try
            {
                var model = ResolveModel();
                string message;

                var parameters = new Dictionary<string, object>();

                if (!model.CanProcessRequest(parameters, out message))
                {
                    return PartialView("_Warning", message);
                }

                categoryNavigationViewModel = model.CreateViewModel();

                var detailTemplateName = detailTemplateNamePrefix + TemplateName;

                return View(detailTemplateName, categoryNavigationViewModel);
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

        public ActionResult StoreNavigation(Guid categoryPageId)
        {
            CategoryNavigationViewModel categoryNavigationViewModel = null;

            CategoryPageId = categoryPageId;
            try
            {
                var model = ResolveModel();
                string message;

                var parameters = new Dictionary<string, object>();

                if (!model.CanProcessRequest(parameters, out message))
                {
                    return PartialView("_Warning", message);
                }

                categoryNavigationViewModel = model.CreateViewModel();
                return View(categoryNavigationViewModel);
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

        private ICategoryModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                hideMiniBasket = HideMiniBasket,
                allowChangingCurrency = AllowChangingCurrency,
                imageId = ImageId,
                categoryPageId = CategoryPageId,
                searchPageId = SearchPageId,
                productDetailsPageId = ProductDetailsPageId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<ICategoryModel>(args);

            return model;
        }
    }
}
