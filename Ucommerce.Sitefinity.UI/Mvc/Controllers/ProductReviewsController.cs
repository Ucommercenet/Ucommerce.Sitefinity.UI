using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uProductReviews_MVC",
        Title = "Product Reviews",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnProductReviews sfMvcIcn")]
    public class ProductReviewsController : Controller, IPersonalizable
    {
        private readonly string listTemplateNamePrefix = "List.";
        public string TemplateName { get; set; } = "Index";

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
            var parameters = new Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return Json(new OperationStatusDTO
                        { Status = "failed", Message = message },
                    JsonRequestBehavior.AllowGet);
            }

            var vm = model.GetReviews(productId);

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";
            responseDTO.Data.Add("Reviews", vm.Reviews);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = ResolveModel();
            string message;
            var parameters = new Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            var listTemplateName = listTemplateNamePrefix + TemplateName;

            return View(listTemplateName);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IReviewsModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IReviewsModel>();

            return model;
        }
    }
}
