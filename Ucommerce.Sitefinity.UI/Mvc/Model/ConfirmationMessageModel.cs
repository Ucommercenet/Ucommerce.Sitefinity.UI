using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Confirmation Message MVC widget.
	/// </summary>
	public class ConfirmationMessageModel : IConfirmationMessageModel
	{
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();

		public virtual ConfirmationMessageViewModel GetViewModel(string headline, string message)
		{
			var viewModel = new ConfirmationMessageViewModel()
			{
				Headline = headline,
				Message = message
			};

			// TODO-REVIEW #5: Can we resolve the order Id here and use it for the interaction object?
			// TODO-REVIEW #6: Invalid interaction (w/o subject) is sent when you place this widget on a page and publish it. Can we avoid this?
			string interactionObject = string.Empty;
			InsightUcommerce.SendInteraction("Checkout > Complete order", interactionObject);

			return viewModel;
		}

		public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
		{
			if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
			{
				message = "The widget is in Page Edit mode.";
				return false;
			}

			message = null;
			return true;
		}
	}
}
