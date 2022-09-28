namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class vizualized after saving the information related to the addresses associated with an order.
    /// </summary>
    public class AddressSaveViewModel
    {
        public AddressSave BillingAddress { get; set; }
        public bool IsShippingAddressDifferent { get; set; }
        public AddressSave ShippingAddress { get; set; }

        public AddressSaveViewModel()
        {
            ShippingAddress = new AddressSave();
            BillingAddress = new AddressSave();
            IsShippingAddressDifferent = false;
        }
    }
}
