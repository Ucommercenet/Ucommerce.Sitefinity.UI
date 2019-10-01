using System.Collections.Generic;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CartUpdateBasket
    {
        public CartUpdateBasket()
        {
            RefreshBasket = new List<UpdateOrderLine>();
        }
        public IList<UpdateOrderLine> RefreshBasket { get; set; }

        public class UpdateOrderLine
        {
            public int OrderLineId { get; set; }

            public int OrderLineQty { get; set; }
        }
    }


}