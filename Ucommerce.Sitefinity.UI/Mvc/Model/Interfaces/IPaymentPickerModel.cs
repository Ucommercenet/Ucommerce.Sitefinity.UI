using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IPaymentPickerModel
    {
        PaymentPickerViewModel GetViewModel();
        void CreatePayment(PaymentPickerViewModel createPaymentViewModel);
    }
}
