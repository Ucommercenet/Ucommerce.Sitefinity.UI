using System;
using System.Collections.Generic;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductDetailViewModel()
        {
            this.Variants = new List<ProductDetailViewModel>();
            this.Routes = new Dictionary<string, string>();
        }

        public string DisplayName { get; set; }

        public string Price { get; set; }

        public string ListPrice { get; set; }

        public string Discount { get; set; }

        public string CategoryDisplayName { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string PrimaryImageMediaUrl { get; set; }

        public string CategoryUrl { get; set; }

        public string ProductUrl { get; set; }

        public bool Discounted { get; set; }

        public string VariantSku { get; set; }

        public string Sku { get; set; }

        public int Rating { get; set; }

        public IList<ProductDetailViewModel> Variants { get; set; }

        public bool IsVariant { get; set; }

        public bool IsProductFamily { get; set; }

        public string ParentProductDisplayName { get; set; }

        public string ParentProductUrl { get; set; }

        public Guid Guid { get; set; }

        public bool AllowOrdering { get; set; }

        public Dictionary<string, string> Routes { get; set; }

        public ICollection<ProductProperty> ProductProperties { get; internal set; }
    }
}
