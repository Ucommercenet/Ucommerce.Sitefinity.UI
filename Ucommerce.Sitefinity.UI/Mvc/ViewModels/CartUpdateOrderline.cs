namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used to update the details of a cart item.
    /// </summary>
    public class CartUpdateOrderline
    {
        public decimal Discount { get; set; }
        public int OrderlineId { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
        public int Quantity { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
    }
}
