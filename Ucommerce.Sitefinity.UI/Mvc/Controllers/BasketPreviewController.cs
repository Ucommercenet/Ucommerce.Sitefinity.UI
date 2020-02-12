using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Basket Preview MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uBasketPreview_MVC", Title = "Basket Preview",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnBasketPreview sfMvcIcn")]
    public class BasketPreviewController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName);
        }

        [HttpGet]
        [Route("uc/checkout/preview")]
        public ActionResult Data()
        {
            var model = ResolveModel();

            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.Json(new OperationStatusDTO() {Status = "failed", Message = message},
                    JsonRequestBehavior.AllowGet);
            }

            var basketPreviewViewModel = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";
            responseDTO.Data.Add("data", basketPreviewViewModel);

            return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RequestPayment()
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var paymentUrl = model.GetPaymentUrl();

            return Redirect(paymentUrl);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IBasketPreviewModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IBasketPreviewModel>(new
            {
                nextStepId = this.NextStepId,
                previousStepId = this.PreviousStepId
            });

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}