using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uPaymentPicker_MVC", Title = "Payment Picker", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "ucIcnPaymentPicker sfMvcIcn")]
    public class PaymentPickerController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            if (SystemManager.IsDesignMode)
            {
                return this.PartialView("_DesignMode");
            }

            var model = ResolveModel();
            var paymentPickerVM = model.GetViewModel();

            return View(TemplateName, paymentPickerVM);
        }

        [HttpPost]
        public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            var model = ResolveModel();
            var viewModel = model.GetViewModel();

            model.CreatePayment(createPaymentViewModel);

            if (viewModel.NextStepUrl?.Length == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return Redirect(viewModel.NextStepUrl);
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IPaymentPickerModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IPaymentPickerModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    previousStepId = this.PreviousStepId
                });

            return model;
        }
    }
}
