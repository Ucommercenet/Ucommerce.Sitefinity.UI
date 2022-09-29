﻿using System;
using System.Collections.Generic;
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
    /// The controller class for the Address MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uAddressInformation_MVC",
        Title = "Address Information",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION,
        ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnAddressInformation sfMvcIcn")]
    public class AddressController : Controller, IPersonalizable
    {
        private readonly string detailTemplateNamePrefix = "Detail.";
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        [HttpGet]
        [RelativeRoute("uc/checkout/address")]
        public ActionResult Data()
        {
            var model = ResolveModel();
            string message;

            if (!model.CanProcessRequest(new Dictionary<string, object>(), out message))
            {
                return Json(new OperationStatusDTO
                        { Status = "failed", Message = message },
                    JsonRequestBehavior.AllowGet);
            }

            var viewModel = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";

            if (viewModel == null)
            {
                responseDTO.Status = "failed";
            }

            responseDTO.Data.Add("data", viewModel);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var detailTemplateName = detailTemplateNamePrefix + TemplateName;
            var model = ResolveModel();

            string message;
            var parameters = new Dictionary<string, object>();

            parameters.Add("mode", "index");

            if (!model.CanProcessRequest(parameters, out message))
            {
                return PartialView("_Warning", message);
            }

            return View(detailTemplateName);
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/address")]
        public ActionResult Save(AddressSaveViewModel addressRendering)
        {
            var model = ResolveModel();
            string message;

            var parameters = new Dictionary<string, object>();
            parameters.Add("addressRendering", addressRendering);
            parameters.Add("modelState", ModelState);

            if (!model.CanProcessRequest(parameters, out message))
            {
                return Json(new OperationStatusDTO
                        { Status = "failed", Message = message },
                    JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                model.Save(addressRendering);
                return Json(new OperationStatusDTO
                        { Status = "success" },
                    JsonRequestBehavior.AllowGet);
            }

            var errorList = model.ErrorMessage(ModelState);

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "failed";
            responseDTO.Data.Add("errors", errorList);

            return Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            ActionInvoker.InvokeAction(ControllerContext, "Index");
        }

        private IAddressModel ResolveModel()
        {
            var args = new Arguments();

            args.AddProperties(new
            {
                nextStepId = NextStepId,
                previousStepId = PreviousStepId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IAddressModel>(args);

            return model;
        }
    }
}
