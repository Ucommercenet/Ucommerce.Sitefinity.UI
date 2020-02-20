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
	/// The controller class for the Payment Picker MVC widget.
	/// </summary>
	[ControllerToolboxItem(Name = "uPaymentPicker_MVC", Title = "Payment Picker", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnPaymentPicker sfMvcIcn")]
	public class PaymentPickerController : Controller, IPersonalizable
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

			var paymentPickerVM = model.GetViewModel();
			var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

			return View(detailTemplateName, paymentPickerVM);
		}

		[HttpPost]
		public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
		{
			var model = ResolveModel();
			string message;
			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!model.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var viewModel = model.GetViewModel();

			model.CreatePayment(createPaymentViewModel);

			if (viewModel.NextStepUrl?.Length == 0)
			{
				return new EmptyResult();
			}
			else
			{
				return Redirect(viewModel.NextStepUrl);
			}
		}

		protected override void HandleUnknownAction(string actionName)
		{
			this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
		}

		private IPaymentPickerModel ResolveModel()
		{
			var container = UCommerceUIModule.Container;
			var args = new Castle.MicroKernel.Arguments {
				{ "nextStepId", this.NextStepId },
				{ "previousStepId", this.PreviousStepId }
			};

			var model = container.Resolve<IPaymentPickerModel>(args);

			return model;
		}

		private string detailTemplateNamePrefix = "Detail.";
	}
}
