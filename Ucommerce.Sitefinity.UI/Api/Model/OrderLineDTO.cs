namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing order item information.
    /// </summary>
    public class OrderLineDTO
    {
        public bool HasDiscount { get; set; }
        public int OrderlineId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public int Quantity { get; set; }
        public string ThumbnailImageMediaUrl { get; set; }
        public string Total { get; set; }
        public string UnitPrice { get; set; }
    }
}
