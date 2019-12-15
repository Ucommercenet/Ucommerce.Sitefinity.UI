using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for adding item to basket.
    /// </summary>
    public class AddToBasketDTO
    {
        public AddToBasketDTO()
        {
        }

        public int Quantity { get; set; }

        public string Sku { get; set; }

        public List<VariantViewModel> Variants { get; set; }
    }
}