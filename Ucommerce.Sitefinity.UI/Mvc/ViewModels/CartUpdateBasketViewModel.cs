using System.Collections.Generic;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CartUpdateBasketViewModel
    {
        public CartUpdateBasketViewModel()
        {
            Orderlines = new List<CartUpdateOrderline>();
        }

        public IList<CartUpdateOrderline> Orderlines { get; set; }
        public string OrderTotal { get; set; }
        public string DiscountTotal { get; set; }
        public string TaxTotal { get; set; }
        public string SubTotal { get; set; }
    }
}