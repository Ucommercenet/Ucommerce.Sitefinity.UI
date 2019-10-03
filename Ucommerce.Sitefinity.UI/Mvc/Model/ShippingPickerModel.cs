using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class ShippingPickerModel : IShippingPickerModel
    {
        private Guid nextStepId;
        private Guid previousStepId;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public ShippingPickerModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.previousStepId = previousStepId ?? Guid.Empty;
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
        }

        public ShippingPickerViewModel GetViewModel()
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

            shipmentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            shipmentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return shipmentPickerViewModel;
        }

        public string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        public string GetPreviousStepUrl(Guid previousStepId)
        {
            var previousStepUrl = Pages.UrlResolver.GetPageNodeUrl(previousStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(previousStepUrl);
        }

        public void CreateShipment(ShippingPickerViewModel createShipmentViewModel)
        {
            _transactionLibraryInternal.CreateShipment(createShipmentViewModel.SelectedShippingMethodId, UCommerce.Constants.DefaultShipmentAddressName, true);
            _transactionLibraryInternal.ExecuteBasketPipeline();
        }
    }
}
