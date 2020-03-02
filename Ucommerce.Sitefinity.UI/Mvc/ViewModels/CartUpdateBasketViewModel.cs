using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class vizualized after saving the information related to the basket.
    /// </summary>
    public class CartUpdateBasketViewModel
    {
        public CartUpdateBasketViewModel()
        {
            OrderLines = new List<CartUpdateOrderline>();
            Vouchers = new List<string>();
        }

        public IList<CartUpdateOrderline> OrderLines { get; set; }
        public string OrderTotal { get; set; }
        public string DiscountTotal { get; set; }
        public string TaxTotal { get; set; }
        public string SubTotal { get; set; }
        public List<string> Vouchers { get; set; }
    }
}