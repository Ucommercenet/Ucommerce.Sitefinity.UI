using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Payment Picker MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uPaymentPicker_MVC", Title = "Payment Picker",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnPaymentPicker sfMvcIcn")]
    public class PaymentPickerController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            var model = ResolveModel();
            parameters.Add("mode", "index");

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            return View(detailTemplateName);
        }

        [HttpGet]
        [RelativeRoute("uc/checkout/payment")]
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

            var paymentPickerVM = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";

            if (paymentPickerVM == null)
            {
                responseDTO.Status = "failed";
            }

            responseDTO.Data.Add("data", paymentPickerVM);

            return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/payment")]
        public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var viewModel = model.GetViewModel();

            model.CreatePayment(createPaymentViewModel);

            if (ModelState.IsValid)
            {
                return this.Json(new OperationStatusDTO() {Status = "success"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errorList = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                var responseDTO = new OperationStatusDTO();
                responseDTO.Status = "failed";
                responseDTO.Data.Add("errors", errorList);

                return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IPaymentPickerModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IPaymentPickerModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    previousStepId = this.PreviousStepId
                });

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}