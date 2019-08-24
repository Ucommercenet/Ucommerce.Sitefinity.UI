using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Infrastructure;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uCategoryNavigation_MVC", Title = "Category Navigation", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]

    public class CategoryNavigationController : Controller
    {
        public CategoryNavigationController(IModelFactory modelFactory)
        {
            this.modelFactory = modelFactory;
        }

        public Guid? ImageId { get; set; }

        public bool HideMiniBasket { get; set; }

        public bool AllowChangingCurrency { get; set; } = true;

        public Guid? CategoryPageId { get; set; }

        public Guid? SearchPageId { get; set; }

        [RelativeRoute("{name?}")]
        public ActionResult Index()
        {
            CategoryNavigationViewModel categoryNavigationViewModel = null;

            try
            {
                var model = this.ResolveModel();
                categoryNavigationViewModel = model.CreateViewModel();

                return this.View(categoryNavigationViewModel);
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
            var model = this.modelFactory.CreateCategoryModel(this.HideMiniBasket, this.AllowChangingCurrency, this.ImageId, this.CategoryPageId, this.SearchPageId);

            return model;
        }

        private IModelFactory modelFactory;
    }
}