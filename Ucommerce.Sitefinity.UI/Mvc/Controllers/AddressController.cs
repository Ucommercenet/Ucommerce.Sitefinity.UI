using System;
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
            var model = ResolveModel();

            var modelState = new ModelStateDictionary();

            return model.Save(addressRendering, modelState);
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
