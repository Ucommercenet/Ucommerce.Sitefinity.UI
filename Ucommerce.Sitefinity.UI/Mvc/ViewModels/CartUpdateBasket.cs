using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CartUpdateBasket
    {
        public CartUpdateBasket()
        {
            RefreshBasket = new List<UpdateOrderLine>();
        }

        public IList<UpdateOrderLine> RefreshBasket { get; set; }
    }
}