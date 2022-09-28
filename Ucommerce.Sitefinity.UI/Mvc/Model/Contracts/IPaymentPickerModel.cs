using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Payment Picker MVC widget.
    /// </summary>
    public interface IPaymentPickerModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        void CreatePayment(PaymentPickerViewModel createPaymentViewModel);
        PaymentPickerViewModel GetViewModel();
    }
}
