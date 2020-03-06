using System;
using System.Web.Mvc;
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
    [ControllerToolboxItem(Name = "uAddressInformation_MVC", Title = "Address Information",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnAddressInformation sfMvcIcn")]
    public class AddressController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;
            var model = ResolveModel();

            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            parameters.Add("mode", "index");

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            return View(detailTemplateName);
        }

        [HttpGet]
        [RelativeRoute("uc/checkout/address")]
        public ActionResult Data()
        {
            var model = ResolveModel();
            string message;

            if (!model.CanProcessRequest(new System.Collections.Generic.Dictionary<string, object>(), out message))
            {
                return this.Json(new OperationStatusDTO() {Status = "failed", Message = message},
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

            return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/address")]
        public ActionResult Save(AddressSaveViewModel addressRendering)
        {
            var model = ResolveModel();
            var viewModel = model.GetViewModel();
            string message;

            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            parameters.Add("addressRendering", addressRendering);
            parameters.Add("modelState", ModelState);
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.Json(new OperationStatusDTO() {Status = "failed", Message = message},
                    JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                model.Save(addressRendering);
                return this.Json(new OperationStatusDTO() {Status = "success"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errorList = model.ErrorMessage(ModelState);

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

        private IAddressModel ResolveModel()
        {
            var args = new Castle.MicroKernel.Arguments();

            args.AddProperties(new
            {
                nextStepId = this.NextStepId,
                previousStepId = this.PreviousStepId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IAddressModel>(args);

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}