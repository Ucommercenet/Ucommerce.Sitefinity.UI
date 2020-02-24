using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uReviewForm_MVC", Title = "Review Form",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucReviewForm sfMvcIcn")]
    public class ReviewFormController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var viewModel = new ReviewFormRenderingViewModel();
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName, viewModel);
        }

        [HttpPost]
        [RelativeRoute("submit-review")]
        [RelativeRoute("{parentCategory1?}/submit-review")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/submit-review")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/submit-review")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/submit-review")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/submit-review")]
        public ActionResult SubmitReview(ReviewFormSaveViewModel reviewModel)
        {
            var model = ResolveModel();
            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            string message;

            parameters.Add("submitModel", model);

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.Json(new OperationStatusDTO() {Status = "failed", Message = message},
                    JsonRequestBehavior.AllowGet);
            }

            var viewModel = model.Add(reviewModel);

            return Json(
                new
                {
                    viewModel.Rating,
                    viewModel.ReviewHeadline,
                    viewModel.CreatedBy,
                    CreatedOn = viewModel.CreatedOn.ToString("MMM dd, yyyy"),
                    CreatedOnForMeta = viewModel.CreatedOn.ToString("yyyy-MM-dd"), 
                    Comments = viewModel.ReviewText
                }, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IReviewFormModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IReviewFormModel>();

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}