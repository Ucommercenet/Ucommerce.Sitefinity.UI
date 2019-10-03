using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
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
            var shipmentPickerViewModel = new ShippingPickerViewModel();
            var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            if (_transactionLibraryInternal.HasBasket())
            {
                var allCountries = UCommerce.Api.TransactionLibrary.GetCountries();
                var shippingCountry = UCommerce.Api.TransactionLibrary.GetCountries().SingleOrDefault(x => x.Name == "Denmark");
                shipmentPickerViewModel.ShippingCountry = shippingCountry.Name;
                var availableShippingMethods = _transactionLibraryInternal.GetShippingMethods(shippingCountry);

                shipmentPickerViewModel.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
                    ? basket.Shipments.FirstOrDefault().ShippingMethod.ShippingMethodId : -1;

                foreach (var availableShippingMethod in availableShippingMethods)
                {
                    var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
                    var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

                    shipmentPickerViewModel.AvailableShippingMethods.Add(new SelectListItem()
                    {
                        Selected = shipmentPickerViewModel.SelectedShippingMethodId == availableShippingMethod.ShippingMethodId,
                        Text = String.Format(" {0} ({1})", availableShippingMethod.Name, formattedprice),
                        Value = availableShippingMethod.ShippingMethodId.ToString()
                    });
                }
            }

            return View("Index", shipmentPickerViewModel);
        }

        [HttpPost]
        public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
        {
            _transactionLibraryInternal.CreateShipment(createShipmentViewModel.SelectedShippingMethodId, UCommerce.Constants.DefaultShipmentAddressName, true);
            _transactionLibraryInternal.ExecuteBasketPipeline();

            return Redirect("/payment");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
