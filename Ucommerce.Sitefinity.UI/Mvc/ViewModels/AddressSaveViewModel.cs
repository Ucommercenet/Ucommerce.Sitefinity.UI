namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class vizualized after saving the information related to the addresses associated with an order.
    /// </summary>
    public class AddressSaveViewModel
    {
        public AddressSaveViewModel()
        {
            ShippingAddress = new AddressSave();
            BillingAddress = new AddressSave();
            IsShippingAddressDifferent = false;
        }

        public AddressSave ShippingAddress { get; set; }
        public AddressSave BillingAddress { get; set; }
        public bool IsShippingAddressDifferent { get; set; }
    }
}
