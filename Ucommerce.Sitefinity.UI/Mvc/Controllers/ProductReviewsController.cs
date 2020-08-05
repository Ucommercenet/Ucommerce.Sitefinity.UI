using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uProductReviews_MVC", Title = "Product Reviews", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnProductReviews sfMvcIcn")]
    public class ProductReviewsController : Controller, IPersonalizable
    {
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var listTemplateName = this.listTemplateNamePrefix + this.TemplateName;

            return View(listTemplateName);
        }

        [HttpGet]
        [RelativeRoute("reviews/data")]
        [RelativeRoute("{parentCategory1?}/reviews/data")]
        [RelativeRoute("{parentCategory2?}/{parentCategory1?}/reviews/data")]
        [RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/data")]
        [RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/data")]
        [RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/reviews/data")]
        public ActionResult Data(int? productId)
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.Json(new OperationStatusDTO() { Status = "failed", Message = message }, JsonRequestBehavior.AllowGet);
            }

            var vm = model.GetReviews(productId);

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";
            responseDTO.Data.Add("Reviews", vm.Reviews);

            return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IReviewsModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IReviewsModel>();

            return model;
        }

        private string listTemplateNamePrefix = "List.";
    }
}