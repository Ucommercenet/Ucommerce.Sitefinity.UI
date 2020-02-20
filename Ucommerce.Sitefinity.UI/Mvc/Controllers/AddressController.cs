using Castle.Windsor;
using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Address MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uAddressInformation_MVC", Title = "Address Information", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnAddressInformation sfMvcIcn")]
    public class AddressController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();
            string message;

            if (!model.CanProcessRequest(new System.Collections.Generic.Dictionary<string, object>(), out message))
            {
                return this.PartialView("_Warning", message);
            }

            var viewModel = model.GetViewModel();
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName, viewModel);
        }

        [HttpPost]
        public ActionResult Save(AddressSaveViewModel addressRendering)
        {
            var model = ResolveModel();
            var viewModel = model.GetViewModel();
            string message;

            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            parameters.Add("addressRendering", addressRendering);
            parameters.Add("modelState", ModelState);
            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            if (ModelState.IsValid)
            {
                model.Save(addressRendering);
                if (viewModel.NextStepUrl?.Length == 0)
                {
                    return View(detailTemplateName, viewModel);
                }
                else
                {
                    return Redirect(viewModel.NextStepUrl);
                }
            }
            else
            {
                return View(detailTemplateName, viewModel);
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IAddressModel ResolveModel()
        {
            var container = UCommerceUIModule.Container;
			var args = new Castle.MicroKernel.Arguments
			{
				{ "nextStepId", this.NextStepId },
				{ "previousStepId", this.PreviousStepId }
			};


			var model = container.Resolve<IAddressModel>(args);

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}
