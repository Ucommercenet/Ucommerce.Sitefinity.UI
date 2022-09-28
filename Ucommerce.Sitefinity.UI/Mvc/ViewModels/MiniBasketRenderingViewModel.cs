using System.Collections.Generic;
using Ucommerce;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing summary information regarding an order.
    /// </summary>
    public class MiniBasketRenderingViewModel
    {
        public string CartPageUrl { get; set; }
        public string CheckoutPageUrl { get; set; }
        public bool IsEmpty { get; set; }
        public int NumberOfItems { get; set; }
        public IList<OrderlineViewModel> OrderLines { get; set; }
        public string RefreshUrl { get; set; }
        public Money Total { get; set; }

        public MiniBasketRenderingViewModel()
        {
            OrderLines = new List<OrderlineViewModel>();
        }
    }
}
