using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Transactions;
using ObjectFactory = UCommerce.Infrastructure.ObjectFactory;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Shipping Picker MVC widget.
    /// </summary>
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

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            try
            {
                var basket = _transactionLibraryInternal.GetBasket().PurchaseOrder;

                if (basket != null)
                {
                    var address = basket.GetAddress(UCommerce.Constants.DefaultShipmentAddressName);

                    if (address == null)
                    {
                        message = "Address must be specified";
                        return false;
                    }
                    else
                    {
                        message = null;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, ConfigurationPolicy.ErrorLog);
            }

            message = "No basket for current user";
            return false;
        }

        public virtual ShippingPickerViewModel GetViewModel()
        {
            try
            {
                var shipmentPickerViewModel = new ShippingPickerViewModel();
                var basket = _transactionLibraryInternal.GetBasket().PurchaseOrder;
                if (_transactionLibraryInternal.HasBasket())
                {
                    var shippingCountry = basket.GetAddress(UCommerce.Constants.DefaultShipmentAddressName).Country;
                    shipmentPickerViewModel.ShippingCountry = shippingCountry.Name;
                    var availableShippingMethods = _transactionLibraryInternal.GetShippingMethods(shippingCountry);

                    if (basket.Shipments.Count > 0)
                    {
                        shipmentPickerViewModel.SelectedShippingMethodId =
                            basket.Shipments.FirstOrDefault()?.ShippingMethod.ShippingMethodId ?? 0;
                    }
                    else
                    {
                        shipmentPickerViewModel.SelectedShippingMethodId = -1;
                    }

                    foreach (var availableShippingMethod in availableShippingMethods)
                    {
                        var priceGroup = SiteContext.Current.CatalogContext.CurrentPriceGroup;

                        var price = availableShippingMethod.GetPriceForPriceGroup(priceGroup);
                        var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

                        shipmentPickerViewModel.AvailableShippingMethods.Add(new SelectListItem()
                        {
                            Selected = shipmentPickerViewModel.SelectedShippingMethodId ==
                                       availableShippingMethod.ShippingMethodId,
                            Text = String.Format(" {0} ({1})", availableShippingMethod.Name, formattedprice),
                            Value = availableShippingMethod.ShippingMethodId.ToString()
                        });
                    }
                }

                _transactionLibraryInternal.ExecuteBasketPipeline();

                shipmentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
                shipmentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

                return shipmentPickerViewModel;
            }
            catch
            {
                return null;
            }
        }

        public virtual void CreateShipment(ShippingPickerViewModel createShipmentViewModel)
        {
            _transactionLibraryInternal.CreateShipment(createShipmentViewModel.SelectedShippingMethodId,
                UCommerce.Constants.DefaultShipmentAddressName, true);
            _transactionLibraryInternal.ExecuteBasketPipeline();
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        private string GetPreviousStepUrl(Guid previousStepId)
        {
            var previousStepUrl = Pages.UrlResolver.GetPageNodeUrl(previousStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(previousStepUrl);
        }
    }
}