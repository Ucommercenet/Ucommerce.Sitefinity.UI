﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.MicroKernel;
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
    [ControllerToolboxItem(Name = "uPaymentPicker_MVC",
        Title = "Payment Picker",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnPaymentPicker sfMvcIcn")]
    public class PaymentPickerController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        [HttpPost]
        [RelativeRoute("uc/checkout/payment")]
        public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            var model = ResolveModel();
            string message;
            var parameters = new Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            model.CreatePayment(createPaymentViewModel);

            if (ModelState.IsValid)
            {
                return Json(new OperationStatusDTO
                        { Status = "success" },
                    JsonRequestBehavior.AllowGet);
            }

            var errorList = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    .ToArray()
            );

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "failed";
            responseDTO.Data.Add("errors", errorList);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RelativeRoute("uc/checkout/payment")]
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

            var paymentPickerVM = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";

            if (paymentPickerVM == null)
            {
                responseDTO.Status = "failed";
            }

            responseDTO.Data.Add("data", paymentPickerVM);

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

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IPaymentPickerModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                nextStepId = NextStepId,
                previousStepId = PreviousStepId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IPaymentPickerModel>(args);

            return model;
        }
    }
}
