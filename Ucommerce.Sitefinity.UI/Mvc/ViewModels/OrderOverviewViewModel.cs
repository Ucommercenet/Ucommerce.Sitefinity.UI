using System.Collections.Generic;
using Ucommerce.EntitiesV2;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with an order.
    /// </summary>
    public class OrderOverviewViewModel
    {
        public OrderAddress BillingAddress { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountTotal { get; set; }
        public string NextStepUrl { get; set; }
        public IList<OrderlineViewModel> OrderLines { get; set; }
        public string OrderTotal { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentName { get; set; }
        public string PaymentTotal { get; set; }
        public int RemoveOrderlineId { get; set; }
        public OrderAddress ShipmentAddress { get; set; }
        public decimal ShipmentAmount { get; set; }
        public string ShipmentName { get; set; }
        public string ShippingTotal { get; set; }
        public string SubTotal { get; set; }
        public string TaxTotal { get; set; }
        public OrderOverviewViewModel() => OrderLines = new List<OrderlineViewModel>();
    }
}
