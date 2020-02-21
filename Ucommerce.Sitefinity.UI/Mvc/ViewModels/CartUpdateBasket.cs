using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used to update the information in the basket.
    /// </summary>
    public class CartUpdateBasket
    {
        public CartUpdateBasket()
        {
            RefreshBasket = new List<UpdateOrderLine>();
        }

        public IList<UpdateOrderLine> RefreshBasket { get; set; }
        public string Voucher { get; set; }
    }
}