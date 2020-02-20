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
	/// The controller class for the Shipping Picker MVC widget.
	/// </summary>
	[ControllerToolboxItem(Name = "uShippingPicker_MVC", Title = "Shipping Picker", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnShippingPicker sfMvcIcn")]
	public class ShippingPickerController : Controller, IPersonalizable
	{
		public Guid? NextStepId { get; set; }
		public Guid? PreviousStepId { get; set; }
		public string TemplateName { get; set; } = "Index";

		public ActionResult Index()
		{
			var model = ResolveModel();
			string message;

			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var sippingPickerVM = model.GetViewModel();
			var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

			return View(detailTemplateName, sippingPickerVM);
		}

		[HttpPost]
		public ActionResult CreateShipment(ShippingPickerViewModel createShipmentViewModel)
		{
			var model = ResolveModel();
			string message;
			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var viewModel = model.GetViewModel();

			model.CreateShipment(createShipmentViewModel);

			if (viewModel.NextStepUrl?.Length == 0)
			{
				return new EmptyResult();
			}
			else
			{
				return Redirect(viewModel.NextStepUrl);
			}
		}

		public IShippingPickerModel ResolveModel()
		{
			var container = UCommerceUIModule.Container;
			var args = new Castle.MicroKernel.Arguments{
				{ "nextStepId",this.NextStepId },
				{ "previousStepId", this.PreviousStepId }
			};

			var model = container.Resolve<IShippingPickerModel>(args);

			return model;
		}

		protected override void HandleUnknownAction(string actionName)
		{
			this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
		}

		private string detailTemplateNamePrefix = "Detail.";
	}
}
