namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class AddToBasketModel
    {
        public AddToBasketModel()
        {
        }

        public int Quantity { get; set; }

        public string Sku { get; set; }

        public string VariantSku { get; set; }
    }
}