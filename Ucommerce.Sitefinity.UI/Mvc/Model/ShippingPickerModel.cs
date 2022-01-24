using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce.Api;
using Ucommerce;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Shipping Picker MVC widget.
	/// </summary>
	public class ShippingPickerModel : IShippingPickerModel
	{
		private Guid nextStepId;
		private Guid previousStepId;
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
		public ITransactionLibrary TransactionLibrary => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ITransactionLibrary>();
		public ICatalogContext CatalogContext => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ICatalogContext>();

		public ShippingPickerModel(Guid? nextStepId = null, Guid? previousStepId = null)
		{
			this.nextStepId = nextStepId ?? Guid.Empty;
			this.previousStepId = previousStepId ?? Guid.Empty;
		}

		public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
		{
			object mode = null;

			if (parameters.TryGetValue("mode", out mode) && mode != null)
			{
				if (mode.ToString() == "index")
				{
					if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
					{
						message = "The widget is in Page Edit mode.";
						return false;
					}
				}

				message = null;
				return true;
			}
			else
			{
				Ucommerce.EntitiesV2.PurchaseOrder basketPurchaseOrder = null;
				try
				{
					basketPurchaseOrder = TransactionLibrary.GetBasket();
				}
				catch (Exception ex)
				{
					Log.Write(ex, ConfigurationPolicy.ErrorLog);
				}

				if (basketPurchaseOrder != null)
				{
					var address = basketPurchaseOrder.GetAddress(Ucommerce.Constants.DefaultShipmentAddressName);

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

				message = "The checkout is not started yet";
				return false;
			}
		}

		public virtual ShippingPickerViewModel GetViewModel()
		{
			var shipmentPickerViewModel = new ShippingPickerViewModel();
			Ucommerce.EntitiesV2.PurchaseOrder basketPurchaseOrder = null;

			try
			{
				basketPurchaseOrder = TransactionLibrary.GetBasket();
			}
			catch (Exception ex)
			{
				Log.Write(ex, ConfigurationPolicy.ErrorLog);
				return null;
			}

			if (!basketPurchaseOrder.OrderLines.Any())
			{
				return null;
			}

			if (TransactionLibrary.HasBasket())
			{
				var shippingCountry = basketPurchaseOrder.GetAddress(Ucommerce.Constants.DefaultShipmentAddressName)?
					.Country;
				if (shippingCountry != null)
				{
					shipmentPickerViewModel.ShippingCountry = shippingCountry.Name;
					var availableShippingMethods = TransactionLibrary.GetShippingMethods(shippingCountry);

					if (basketPurchaseOrder.Shipments.Count > 0)
					{
						shipmentPickerViewModel.SelectedShippingMethodId =
							basketPurchaseOrder.Shipments.FirstOrDefault()?.ShippingMethod.ShippingMethodId ?? 0;
					}
					else
					{
						shipmentPickerViewModel.SelectedShippingMethodId = -1;
					}

					foreach (var availableShippingMethod in availableShippingMethods)
					{
						var priceGroup = Ucommerce.EntitiesV2.PriceGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentPriceGroup.Guid);
						var localizedShippingMethod = availableShippingMethod.ShippingMethodDescriptions.FirstOrDefault(s => s.CultureCode.Equals(CultureInfo.CurrentCulture.ToString()));

						var price = availableShippingMethod.GetPriceForPriceGroup(priceGroup);
						var formattedPrice = new Money((price == null ? 0 : price.Price), basketPurchaseOrder.BillingCurrency.ISOCode);

						if (localizedShippingMethod != null)
						{
							var displayName = localizedShippingMethod.DisplayName;
							if (string.IsNullOrWhiteSpace(displayName)) displayName = availableShippingMethod.Name;

							shipmentPickerViewModel.AvailableShippingMethods.Add(new SelectListItem()
							{
								Selected = shipmentPickerViewModel.SelectedShippingMethodId == availableShippingMethod.ShippingMethodId,
								Text = String.Format(" {0} ({1})", displayName, formattedPrice),
								Value = availableShippingMethod.ShippingMethodId.ToString()
							});
						}
					}
				}
			}

			TransactionLibrary.ExecuteBasketPipeline();

			shipmentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
			shipmentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

			InsightUcommerce.SendInteraction(basketPurchaseOrder, "Checkout", "Shipping Method Selection");

			return shipmentPickerViewModel;
		}

		public virtual void CreateShipment(ShippingPickerViewModel createShipmentViewModel)
		{
			TransactionLibrary.CreateShipment(createShipmentViewModel.SelectedShippingMethodId,
				Ucommerce.Constants.DefaultShipmentAddressName, true);
			TransactionLibrary.ExecuteBasketPipeline();

			InsightUcommerce.SendBasketInteraction("Checkout", $"Shipping Method Selected: {createShipmentViewModel.SelectedShippingMethodId}");
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