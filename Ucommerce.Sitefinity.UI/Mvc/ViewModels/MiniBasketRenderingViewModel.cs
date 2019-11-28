using UCommerce;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing summary information regarding an order.
    /// </summary>
    public class MiniBasketRenderingViewModel
    {
        public int NumberOfItems { get; set; }
        public Money Total { get; set; }
        public bool IsEmpty { get; set; }
        public string RefreshUrl { get; set; }
        public string CartPageUrl { get; set; }
    }
}
