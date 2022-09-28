using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing the basket information.
    /// </summary>
    public class BasketDTO
    {
        public List<DiscountDTO> Discounts { get; set; }
        public string DiscountTotal { get; set; }
        public bool HasDiscount { get; set; }
        public int NumberOfItemsInBasket { get; set; }
        public List<OrderLineDTO> OrderLines { get; set; }
        public string OrderTotal { get; set; }
        public string PaymentTotal { get; set; }
        public string ShippingTotal { get; set; }
        public string TaxTotal { get; set; }

        public BasketDTO()
        {
            OrderLines = new List<OrderLineDTO>();
            Discounts = new List<DiscountDTO>();
        }
    }
}
