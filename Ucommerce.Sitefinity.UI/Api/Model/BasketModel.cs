using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class BasketModel
    {
        public BasketModel()
        {
            this.OrderLines = new List<OrderLineModel>();
            this.Discounts = new List<DiscountModel>();
        }

        public int NumberOfItemsInBasket { get; set; }

        public List<DiscountModel> Discounts { get; set; }

        public List<OrderLineModel> OrderLines { get; set; }

        public string OrderTotal { get; set; }

        public string TaxTotal { get; set; }

        public string DiscountTotal { get; set; }

        public bool HasDiscount { get; set; }

        public string ShippingTotal { get; set; }

        public string PaymentTotal { get; set; }
    }
}
