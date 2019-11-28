using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information assocaited with an item in an order.
    /// </summary>
    public class OrderlineViewModel
    {
        public string Total { get; set; }
        public int Quantity { get; set; }
        public int OrderLineId { get; set; }
        public string Sku { get; set; }
        public string VariantSku { get; set; }
        public string ProductName { get; set; }
        public string Tax { get; set; }
        public decimal? Discount { get; set; }
        public string ProductUrl { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
    }
}
