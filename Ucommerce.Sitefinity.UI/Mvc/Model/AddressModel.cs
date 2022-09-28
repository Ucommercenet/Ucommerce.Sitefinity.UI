using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Services;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using UCommerce.Sitefinity.UI.Resources;
using ObjectFactory = Ucommerce.Infrastructure.ObjectFactory;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Address MVC widget.
    /// </summary>
    public class AddressModel : IAddressModel
    {
        private readonly IQueryable<Country> _countries;
        private readonly Guid nextStepId;
        private readonly Guid previousStepId;
        public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        public AddressModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            _countries = Country.All();
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
                    if (SystemManager.IsDesignMode)
                    {
                        message = "The widget is in Page Edit mode.";
                        return false;
                    }
                }

                message = null;
                return true;
            }

            if (parameters.ContainsKey("addressRendering") && parameters.ContainsKey("modelState"))
            {
                var addressRendering = parameters["addressRendering"] as AddressSaveViewModel;
                var modelState = parameters["modelState"] as ModelStateDictionary;

                if (!addressRendering.IsShippingAddressDifferent)
                {
                    modelState.Remove("ShippingAddress.FirstName");
                    modelState.Remove("ShippingAddress.LastName");
                    modelState.Remove("ShippingAddress.EmailAddress");
                    modelState.Remove("ShippingAddress.Line1");
                    modelState.Remove("ShippingAddress.PostalCode");
                    modelState.Remove("ShippingAddress.City");
                }
            }

            message = null;
            return true;
        }

        public Dictionary<string, string> ErrorMessage(ModelStateDictionary status)
        {
            var errorList = status.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    .ToArray()
            );
            var errors = new Dictionary<string, string>();

            foreach (var keyValuePair in errorList)
            {
                switch (keyValuePair.Key)
                {
                    case "BillingAddress.FirstName":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.FirstName",
                                Res.Get<UcommerceResources>()
                                    .FirstNameValidation);
                        }

                        break;
                    case "BillingAddress.LastName":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.LastName",
                                Res.Get<UcommerceResources>()
                                    .LastNameValidation);
                        }

                        break;
                    case "BillingAddress.EmailAddress":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.EmailAddress",
                                Res.Get<UcommerceResources>()
                                    .EmailAddressValidation);
                        }

                        break;
                    case "BillingAddress.Line1":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.Line1",
                                Res.Get<UcommerceResources>()
                                    .Line1Validation);
                        }

                        break;
                    case "BillingAddress.PostalCode":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.PostalCode",
                                Res.Get<UcommerceResources>()
                                    .PostalCodeValidation);
                        }

                        break;
                    case "BillingAddress.City":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("BillingAddress.City",
                                Res.Get<UcommerceResources>()
                                    .CityValidation);
                        }

                        break;
                    case "ShippingAddress.FirstName":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.FirstName",
                                Res.Get<UcommerceResources>()
                                    .FirstNameValidation);
                        }

                        break;
                    case "ShippingAddress.LastName":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.LastName",
                                Res.Get<UcommerceResources>()
                                    .LastNameValidation);
                        }

                        break;
                    case "ShippingAddress.EmailAddress":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.EmailAddress",
                                Res.Get<UcommerceResources>()
                                    .EmailAddressValidation);
                        }

                        break;
                    case "ShippingAddress.Line1":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.Line1",
                                Res.Get<UcommerceResources>()
                                    .Line1Validation);
                        }

                        break;
                    case "ShippingAddress.PostalCode":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.PostalCode",
                                Res.Get<UcommerceResources>()
                                    .PostalCodeValidation);
                        }

                        break;
                    case "ShippingAddress.City":
                        if (keyValuePair.Value.Any())
                        {
                            errors.Add("ShippingAddress.City",
                                Res.Get<UcommerceResources>()
                                    .CityValidation);
                        }

                        break;
                }
            }

            return errors;
        }

        public virtual AddressRenderingViewModel GetViewModel()
        {
            var viewModel = new AddressRenderingViewModel();
            OrderAddress shippingInformation;
            OrderAddress billingInformation;
            PurchaseOrder purchaseOrder;
            try
            {
                purchaseOrder = TransactionLibrary.GetBasket();
                shippingInformation =
                    TransactionLibrary.GetBasket()
                        .GetShippingAddress(Ucommerce.Constants.DefaultShipmentAddressName) ?? new OrderAddress();
                billingInformation = TransactionLibrary.GetBasket()
                        .BillingAddress ??
                    new OrderAddress();
            }
            catch (Exception ex)
            {
                Log.Write(ex, ConfigurationPolicy.ErrorLog);
                return null;
            }

            if (!purchaseOrder.OrderLines.Any())
            {
                return null;
            }

            viewModel.BillingAddress.FirstName = billingInformation.FirstName;
            viewModel.BillingAddress.LastName = billingInformation.LastName;
            viewModel.BillingAddress.EmailAddress = billingInformation.EmailAddress;
            viewModel.BillingAddress.PhoneNumber = billingInformation.PhoneNumber;
            viewModel.BillingAddress.MobilePhoneNumber = billingInformation.MobilePhoneNumber;
            viewModel.BillingAddress.Line1 = billingInformation.Line1;
            viewModel.BillingAddress.Line2 = billingInformation.Line2;
            viewModel.BillingAddress.PostalCode = billingInformation.PostalCode;
            viewModel.BillingAddress.City = billingInformation.City;
            viewModel.BillingAddress.State = billingInformation.State;
            viewModel.BillingAddress.Attention = billingInformation.Attention;
            viewModel.BillingAddress.CompanyName = billingInformation.CompanyName;
            viewModel.BillingAddress.Country.CountryId = billingInformation?.Country != null ? billingInformation.Country.CountryId : -1;

            viewModel.ShippingAddress.FirstName = shippingInformation.FirstName;
            viewModel.ShippingAddress.LastName = shippingInformation.LastName;
            viewModel.ShippingAddress.EmailAddress = shippingInformation.EmailAddress;
            viewModel.ShippingAddress.PhoneNumber = shippingInformation.PhoneNumber;
            viewModel.ShippingAddress.MobilePhoneNumber = shippingInformation.MobilePhoneNumber;
            viewModel.ShippingAddress.Line1 = shippingInformation.Line1;
            viewModel.ShippingAddress.Line2 = shippingInformation.Line2;
            viewModel.ShippingAddress.PostalCode = shippingInformation.PostalCode;
            viewModel.ShippingAddress.City = shippingInformation.City;
            viewModel.ShippingAddress.State = shippingInformation.State;
            viewModel.ShippingAddress.Attention = shippingInformation.Attention;
            viewModel.ShippingAddress.CompanyName = shippingInformation.CompanyName;
            viewModel.ShippingAddress.Country.CountryId = shippingInformation.Country != null ? shippingInformation.Country.CountryId : -1;

            viewModel.AvailableCountries = _countries.ToList()
                .Select(x => new SelectListItem
                    { Text = x.Name, Value = x.CountryId.ToString() })
                .ToList();

            viewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            viewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return viewModel;
        }

        public virtual JsonResult Save(AddressSaveViewModel addressRendering)
        {
            var result = new JsonResult();

            if (addressRendering.IsShippingAddressDifferent)
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.ShippingAddress);
                InsightUcommerce.SendInteraction("Checkout > Set address", "billing");
                InsightUcommerce.SendInteraction("Checkout > Set address", "shipping");
            }
            else
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.BillingAddress);
                InsightUcommerce.SendInteraction("Checkout > Set address", "billing & shipping");
            }

            TransactionLibrary.ExecuteBasketPipeline();

            result.Data = new { ShippingUrl = GetNextStepUrl(nextStepId) };
            return result;
        }

        private void EditBillingInformation(AddressSave billingAddress)
        {
            TransactionLibrary.EditBillingInformation(
                billingAddress.FirstName,
                billingAddress.LastName,
                billingAddress.EmailAddress,
                billingAddress.PhoneNumber,
                billingAddress.MobilePhoneNumber,
                billingAddress.CompanyName,
                billingAddress.Line1,
                billingAddress.Line2,
                billingAddress.PostalCode,
                billingAddress.City,
                billingAddress.State,
                billingAddress.Attention,
                billingAddress.CountryId);
        }

        private void EditShippingInformation(AddressSave shippingAddress)
        {
            try
            {
                TransactionLibrary.EditShipmentInformation(
                    Ucommerce.Constants.DefaultShipmentAddressName,
                    shippingAddress.FirstName,
                    shippingAddress.LastName,
                    shippingAddress.EmailAddress,
                    shippingAddress.PhoneNumber,
                    shippingAddress.MobilePhoneNumber,
                    shippingAddress.CompanyName,
                    shippingAddress.Line1,
                    shippingAddress.Line2,
                    shippingAddress.PostalCode,
                    shippingAddress.City,
                    shippingAddress.State,
                    shippingAddress.Attention,
                    shippingAddress.CountryId);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Write(ex);
            }
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = UrlResolver.GetPageNodeUrl(nextStepId);

            return UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        private string GetPreviousStepUrl(Guid previousStepId)
        {
            var previousStepUrl = UrlResolver.GetPageNodeUrl(previousStepId);

            return UrlResolver.GetAbsoluteUrl(previousStepUrl);
        }
    }
}
