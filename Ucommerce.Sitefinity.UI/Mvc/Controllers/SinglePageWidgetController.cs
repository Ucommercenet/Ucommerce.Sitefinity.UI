using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uSinglePageWidget_MVC", Title = "Single Page Widget", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class SinglePageWidgetController : Controller
    {
        public object NextStepId { get; private set; }

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Save(AddressSaveViewModel addressRendering, ShippingPickerViewModel createShipmentViewModel, PaymentPickerViewModel createPaymentViewModel)
        {
            var addressCont = new AddressController();
            var shipCont = new ShippingPickerController();
            var paymentCont = new PaymentPickerController();

            addressCont.Save(addressRendering);
            shipCont.CreateShipment(createShipmentViewModel);
            paymentCont.CreatePayment(createPaymentViewModel);

            //var modelState = new ModelStateDictionary();
            //var cartModel = ResolveCartModel();
            //var addressModel = ResolveAddressModel();
            //var shippingPickerModel = ResolveShippingPickerModel();
            //var paymentPickerModel = ResolvePaymentPickerModel();

            //addressModel.Save(addressRendering, modelState);
            //shippingPickerModel.CreateShipment(createShipmentViewModel);
            //paymentPickerModel.CreatePayment(createPaymentViewModel);

            return Redirect("/preview");
        }

        public IShippingPickerModel ResolveShippingPickerModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IShippingPickerModel>();

            return model;
        }

        private IAddressModel ResolveAddressModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IAddressModel>();

            return model;
        }

        private IPaymentPickerModel ResolvePaymentPickerModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IPaymentPickerModel>();

            return model;
        }
        private ICartModel ResolveCartModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<ICartModel>();

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
