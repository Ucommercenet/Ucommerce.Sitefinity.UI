using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uAddReview_MVC", Title = "Add Review",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucAddReview sfMvcIcn")]
    public class AddReviewController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var viewModel = new AddReviewRenderingViewModel();
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
        [RelativeRoute("reviews/add")]
        [RelativeRoute("{parentCategory1?}/reviews/add")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/reviews/add")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/add")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/add")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/add")]
        public ActionResult SubmitReview(AddReviewSaveViewModel reviewModel)
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

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IAddReviewModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IAddReviewModel>();

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}