namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with an item in an order.
    /// </summary>
    public class OrderlineViewModel
    {
        public decimal? Discount { get; set; }
        public int OrderLineId { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public int Quantity { get; set; }
        public string Sku { get; set; }
        public string Tax { get; set; }
        public string ThumbnailName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Total { get; set; }
        public string VariantSku { get; set; }
    }
}
