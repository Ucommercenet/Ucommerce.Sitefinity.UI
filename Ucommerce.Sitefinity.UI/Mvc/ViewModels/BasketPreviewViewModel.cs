using System.Collections.Generic;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to list the summary information associated with an order.
    /// </summary>
    public class BasketPreviewViewModel
    {
        public BasketPreviewViewModel()
        {
            OrderLines = new List<PreviewOrderLine>();
            ShipmentAddressDTO = new Address();
            BillingAddressDTO = new Address();
        }

        public IList<PreviewOrderLine> OrderLines { get; set; }
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
        public string NextStepUrl { get; set; }
        public string PreviousStepUrl { get; set; }
        public Address ShipmentAddressDTO { get; set; }
        public Address BillingAddressDTO { get; set; }
    }
}
