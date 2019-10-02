using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uAddressInformation_MVC", Title = "Address Information", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class AddressController : Controller
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplteName { get; set; } = "Index";
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public AddressController()
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
        }

        public ActionResult Index()
        {
            var model = ResolveModel();
            var viewModel = model.GetViewMode(Url.Action("Save"));

            return View(TemplteName, viewModel);
        }

        [HttpPost]
        public ActionResult Save(AddressSaveViewModel addressRendering)
        {
            if (!addressRendering.IsShippingAddressDifferent)
            {
                this.ModelState.Remove("ShippingAddress.FirstName");
                this.ModelState.Remove("ShippingAddress.LastName");
                this.ModelState.Remove("ShippingAddress.EmailAddress");
                this.ModelState.Remove("ShippingAddress.Line1");
                this.ModelState.Remove("ShippingAddress.PostalCode");
                this.ModelState.Remove("ShippingAddress.City");
            }
            if (!ModelState.IsValid)
            {
                var dictionary = ModelState.ToDictionary(kvp => kvp.Key,
                 kvp => kvp.Value.Errors
                                 .Select(e => e.ErrorMessage).ToArray())
                                 .Where(m => m.Value.Any());

                return Json(new { modelStateErrors = dictionary });
            }

            if (addressRendering.IsShippingAddressDifferent)
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.ShippingAddress);
            }
            else
            {
                EditBillingInformation(addressRendering.BillingAddress);
                EditShippingInformation(addressRendering.BillingAddress);
            }

            //if (Tracker.Current != null)
            //    Tracker.Current.Session.CustomData["FirstName"] = addressRendering.BillingAddress.FirstName;

            _transactionLibraryInternal.ExecuteBasketPipeline();

            return Json(new { ShippingUrl = "/shipping" });
        }

        private void EditShippingInformation(AddressSave shippingAddress)
        {
            _transactionLibraryInternal.EditShipmentInformation(
                UCommerce.Constants.DefaultShipmentAddressName,
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

        private void EditBillingInformation(AddressSave billingAddress)
        {
            _transactionLibraryInternal.EditBillingInformation(
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

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IAddressModel ResolveModel()
        {
            var container = UcommerceUIModule.Container;
            var model = container.Resolve<IAddressModel>(
                new
                {
                    nextStepId = this.NextStepId,
                    previousStepId = this.PreviousStepId
                });

            return model;
        }
    }
}
