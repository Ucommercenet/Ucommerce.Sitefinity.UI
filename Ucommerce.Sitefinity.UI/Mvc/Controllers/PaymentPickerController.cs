using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uPaymentPicker_MVC", Title = "Payment Picker", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class PaymentPickerController : Controller
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplteName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            var paymentPickerVM = model.GetViewModel();

            return View(TemplteName, paymentPickerVM);
        }

        [HttpPost]
        public ActionResult CreatePayment()
        {
            var model = ResolveModel();
            var viewModel = model.GetViewModel();
            model.CreatePayment(viewModel);

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
