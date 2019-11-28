using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IPaymentPickerModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        PaymentPickerViewModel GetViewModel();
        void CreatePayment(PaymentPickerViewModel createPaymentViewModel);
    }
}
