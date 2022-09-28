using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Basket Preview MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uBasketPreview_MVC",
        Title = "Basket Preview",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnBasketPreview sfMvcIcn")]
    public class BasketPreviewController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        [HttpGet]
        [RelativeRoute("uc/checkout/preview")]
        public ActionResult Data()
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

            var basketPreviewViewModel = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";
            responseDTO.Data.Add("data", basketPreviewViewModel);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var detailTemplateName = detailTemplateNamePrefix + TemplateName;
            string message;
            var parameters = new Dictionary<string, object>();
            var model = ResolveModel();
            parameters.Add("mode", "index");

            if (!model.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            return View(detailTemplateName);
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/complete-order")]
        public ActionResult RequestPayment()
        {
            var model = ResolveModel();
            string message;
            var parameters = new Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return Json(new OperationStatusDTO { Status = "failed", Message = message }, JsonRequestBehavior.AllowGet);
            }

            var paymentUrl = model.GetPaymentUrl();

            return Json(new OperationStatusDTO { Status = "success", Message = paymentUrl }, JsonRequestBehavior.AllowGet);
            ;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IBasketPreviewModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                nextStepId = NextStepId,
                previousStepId = PreviousStepId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IBasketPreviewModel>(args);

            return model;
        }
    }
}
