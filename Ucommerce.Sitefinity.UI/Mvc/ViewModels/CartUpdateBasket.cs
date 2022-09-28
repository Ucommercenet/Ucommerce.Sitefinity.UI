using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used to update the information in the basket.
    /// </summary>
    public class CartUpdateBasket
    {
        public bool IsRemove { get; set; } = false;
        public IList<UpdateOrderLine> RefreshBasket { get; set; }
        public List<string> Vouchers { get; set; }

        public CartUpdateBasket()
        {
            RefreshBasket = new List<UpdateOrderLine>();
            Vouchers = new List<string>();
        }
    }
}
