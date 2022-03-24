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

		public virtual ConfirmationMessageViewModel GetViewModel(string headline, string message, string orderGuid)
		{
			var viewModel = new ConfirmationMessageViewModel()
			{
				Headline = headline,
				Message = message
			};

			var purchaseOrder = Ucommerce.EntitiesV2.PurchaseOrder.FirstOrDefault(po => po.OrderGuid.ToString() == orderGuid);
			var orderNumber = purchaseOrder?.OrderNumber;
			if (string.IsNullOrWhiteSpace(orderNumber)) orderNumber = orderGuid;
			InsightUcommerce.SendInteraction("Checkout > Complete order", orderNumber);

			return viewModel;
		}

		public virtual bool CanProcessRequest(Dictionary<string, object> parameters, string orderGuid, out string message)
		{
			if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
			{
				message = "The widget is in Page Edit mode.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(orderGuid))
			{
				message = "No order id was specified - the widget is likely in Page Edit mode.";
				return false;
			}

			message = null;
			return true;
		}
	}
}
