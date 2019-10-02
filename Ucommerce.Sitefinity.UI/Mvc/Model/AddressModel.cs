using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class AddressModel : IAddressModel
    {
        private Guid nextStepId;
        private Guid previousStepId;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly IQueryable<Country> _countries;

        public AddressModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            _countries = Country.All();
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.previousStepId = previousStepId ?? Guid.Empty;
        }

        public AddressRenderingViewModel GetViewMode(string saveUrl)
        {
            var viewModel = new AddressRenderingViewModel();

            var shippingInformation = _transactionLibraryInternal.GetBasket().PurchaseOrder.GetShippingAddress(UCommerce.Constants.DefaultShipmentAddressName) ?? new OrderAddress();
            var billingInformation = _transactionLibraryInternal.GetBasket().PurchaseOrder.BillingAddress ?? new OrderAddress();

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
            viewModel.BillingAddress.CountryId = billingInformation.Country != null ? billingInformation.Country.CountryId : -1;

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
            viewModel.ShippingAddress.CountryId = shippingInformation.Country != null ? shippingInformation.Country.CountryId : -1;

            viewModel.AvailableCountries = _countries.ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.CountryId.ToString() }).ToList();

            viewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            viewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);
            viewModel.SaveAddressUrl = saveUrl;

            return viewModel;
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
