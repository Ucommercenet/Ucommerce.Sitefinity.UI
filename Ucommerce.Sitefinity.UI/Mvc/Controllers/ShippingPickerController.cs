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
    /// The controller class for the Shipping Picker MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uShippingPicker_MVC", Title = "Shipping Picker",
        SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME,
        CssClass = "ucIcnShippingPicker sfMvcIcn")]
    public class ShippingPickerController : Controller, IPersonalizable
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
        [RelativeRoute("uc/checkout/shipping")]
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

            var sippingPickerVM = model.GetViewModel();

            var responseDTO = new OperationStatusDTO();
            responseDTO.Status = "success";

            if (sippingPickerVM == null)
            {
                responseDTO.Status = "failed";
            }

            responseDTO.Data.Add("data", sippingPickerVM);

            return this.Json(responseDTO, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RelativeRoute("uc/checkout/shipping")]
        public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var viewModel = model.GetViewModel();

            model.CreateShipment(createShipmentViewModel);

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

        public IShippingPickerModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IShippingPickerModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    previousStepId = this.PreviousStepId
                });

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}