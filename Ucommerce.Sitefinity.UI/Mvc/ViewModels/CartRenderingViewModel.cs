using System.Collections.Generic;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to list the current state of the order in the cart.
    /// </summary>
    public class CartRenderingViewModel
    {
        public CartRenderingViewModel()
        {
            OrderLines = new List<OrderlineViewModel>();
            Discounts = new List<string>();
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
        public string ProductDetailsPageUrl { get; set; }
        public List<string> Discounts { get; set; }
        public string RedirectUrl { get; set; }
    }
}