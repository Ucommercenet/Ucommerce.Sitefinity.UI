using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uSpaCheckoutWidget_MVC", Title = "Spa Checkout Widget", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class SpaCheckoutController : Controller
    {
        public Guid? NextStepId { get; set; }
        public string TemplteName { get; set; } = "Index";

        public ActionResult Index()
        {
            return View(TemplteName);
        }

        [HttpPost]
        public ActionResult Save(AddressSaveViewModel addressRendering, ShippingPickerViewModel createShipmentViewModel, PaymentPickerViewModel createPaymentViewModel)
        {
            var model = ResolveModel();
            var viewModel = model.GetViewModel(addressRendering, createShipmentViewModel, createPaymentViewModel);

            if (viewModel.NextStepUrl?.Length == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return Redirect(viewModel.NextStepUrl);
            }
        }

        public ISpaCheckoutModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<ISpaCheckoutModel>(
                new
                {
                    nextStepId = this.NextStepId,
                });

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
