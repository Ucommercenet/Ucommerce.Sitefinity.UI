using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing the basket information.
    /// </summary>
    public class BasketDTO
    {
        public BasketDTO()
        {
            this.OrderLines = new List<OrderLineDTO>();
            this.Discounts = new List<DiscountDTO>();
        }

        public int NumberOfItemsInBasket { get; set; }

        public List<DiscountDTO> Discounts { get; set; }

        public List<OrderLineDTO> OrderLines { get; set; }

        public string OrderTotal { get; set; }

        public string TaxTotal { get; set; }

        public string DiscountTotal { get; set; }

        public bool HasDiscount { get; set; }

        public string ShippingTotal { get; set; }

        public string PaymentTotal { get; set; }
    }
}
