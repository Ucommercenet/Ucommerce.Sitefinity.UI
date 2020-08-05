using System.Collections.Generic;
using Ucommerce;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing summary information regarding an order.
    /// </summary>
    public class MiniBasketRenderingViewModel
    {
        public MiniBasketRenderingViewModel()
        {
            OrderLines = new List<OrderlineViewModel>();
        }

        public IList<OrderlineViewModel> OrderLines { get; set; }
        public int NumberOfItems { get; set; }
        public Money Total { get; set; }
        public bool IsEmpty { get; set; }
        public string RefreshUrl { get; set; }
        public string CartPageUrl { get; set; }
        public string CheckoutPageUrl { get; set; }
    }
}
