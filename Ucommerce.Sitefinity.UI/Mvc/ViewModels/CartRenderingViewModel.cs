using System.Collections.Generic;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CartRenderingViewModel
    {
        public CartRenderingViewModel()
        {
            OrderLines = new List<OrderlineViewModel>();
        }

        public IList<OrderlineViewModel> OrderLines { get; set; }
        public string OrderTotal { get; set; }
        public string SubTotal { get; set; }
        public string TaxTotal { get; set; }
        public string DiscountTotal { get; set; }
        public string ShippingTotal { get; set; }
        public string PaymentTotal { get; set; }
        public int RemoveOrderlineId { get; set; }
        public string ShipmentName { get; set; }
        public string PaymentName { get; set; }
        public decimal ShipmentAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public OrderAddress ShipmentAddress { get; set; }
        public OrderAddress BillingAddress { get; set; }
        public string RefreshUrl { get; set; }
        public string RemoveOrderlineUrl { get; set; }
        public string NextStepUrl { get; set; }
    }
}