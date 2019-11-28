namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductViewModel
    {
        public string Sku { get; set; }

        public string VariantSku { get; set; }

        public string Price { get; set; }

        public string Discount { get; set; }

        public string DisplayName { get; set; }

        public string ThumbnailImageMediaUrl { get; set; }

        public string ProductUrl { get; set; }

        public bool IsSellableProduct { get; set; }

        public bool IsVariant { get; set; }
    }
}
