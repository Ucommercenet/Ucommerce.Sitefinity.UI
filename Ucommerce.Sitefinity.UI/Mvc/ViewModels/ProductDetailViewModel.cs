using System;
using System.Collections.Generic;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the detailed information associated with a product.
    /// </summary>
    public class ProductDetailViewModel
    {
        public string CategoryDisplayName { get; set; }
        public string CategoryUrl { get; set; }
        public string Discount { get; set; }
        public bool Discounted { get; set; }
        public string DisplayName { get; set; }
        public Guid Guid { get; set; }
        public bool IsProductFamily { get; set; }
        public bool IsSellableProduct { get; set; }
        public bool IsVariant { get; set; }
        public string ListPrice { get; set; }
        public string LongDescription { get; set; }
        public string ParentProductDisplayName { get; set; }
        public string ParentProductUrl { get; set; }
        public string Price { get; set; }
        public string PrimaryImageMediaUrl { get; set; }
        public IList<KeyValuePair<string, object>> ProductProperties { get; set; }
        public string ProductUrl { get; set; }
        public int Rating { get; set; }
        public ResultSet<Product> RelatedProducts { get; set; }
        public Dictionary<string, string> Routes { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string VariantSku { get; set; }
        public IList<VariantTypeViewModel> VariantTypes { get; set; }

        public ProductDetailViewModel()
        {
            VariantTypes = new List<VariantTypeViewModel>();
            Routes = new Dictionary<string, string>();
            ProductProperties = new List<KeyValuePair<string, object>>();
        }
    }
}
