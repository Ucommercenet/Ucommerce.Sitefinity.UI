using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uShippingPicker_MVC", Title = "Shipping Picker", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class ShippingPickerController : Controller
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public ShippingPickerController()
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
        }

        public ActionResult Index()
        {
            var model = ResolveModel();
            var sippingPickerVM = model.GetViewModel();

            return View("Index", sippingPickerVM);
        }

        [HttpPost]
        public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
        {
            var model = ResolveModel();
            model.CreateShipment(createShipmentViewModel);

            return Redirect("/payment");
        }

        public IShippingPickerModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IShippingPickerModel>();

            return model;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
