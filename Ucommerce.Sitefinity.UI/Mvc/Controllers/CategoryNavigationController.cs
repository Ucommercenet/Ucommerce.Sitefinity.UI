using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uCategoryNavigation_MVC", Title = "Category Navigation", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "ucIcnCategoryNavigation sfMvcIcn")]

    public class CategoryNavigationController : Controller, IPersonalizable
    {
        public Guid? ImageId { get; set; }

        public string ProviderName { get; set; }

        public bool HideMiniBasket { get; set; }

        public bool AllowChangingCurrency { get; set; } = true;

        public Guid? CategoryPageId { get; set; }

        public Guid? SearchPageId { get; set; }

        public Guid? ProductDetailsPageId { get; set; }

        public string TemplateName { get; set; } = "B4Index";

        public ActionResult Index()
        {
            CategoryNavigationViewModel categoryNavigationViewModel = null;

            try
            {
                var model = this.ResolveModel();
                string message;

                var parameters = new System.Collections.Generic.Dictionary<string, object>();

                if (!model.CanProcessRequest(parameters, out message))
                {
                    return this.PartialView("_Warning", message);
                }

                categoryNavigationViewModel = model.CreateViewModel();

                return this.View(this.TemplateName, categoryNavigationViewModel);
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

        private ICategoryModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<ICategoryModel>(
                new
                {
                    hideMiniBasket = this.HideMiniBasket,
                    allowChangingCurrency = this.AllowChangingCurrency,
                    imageId = this.ImageId,
                    categoryPageId = this.CategoryPageId,
                    searchPageId = this.SearchPageId,
                    productDetailsPageId = this.ProductDetailsPageId 
                });

            return model;
        }
    }
}