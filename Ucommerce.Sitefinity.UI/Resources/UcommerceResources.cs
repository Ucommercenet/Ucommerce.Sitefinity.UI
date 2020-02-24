using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace UCommerce.Sitefinity.UI.Resources
{
    [ObjectInfo("UcommerceResources", ResourceClassId = "UcommerceResources", Title = "UCommerce Resource", TitlePlural = "UCommerce Resources", Description = "Custom resources for UCommerce")]
    class UcommerceResources : Resource
    {
        public UcommerceResources()
        {
        }

        public UcommerceResources(ResourceDataProvider dataProvider) : base(dataProvider)
        {
        }

        [ResourceEntry("WriteReview", Value = "Write a review", Description = "Write a review", LastModified = "2020/02/21")]
        public string AllCategories
        {
            get { return this["Write a review"]; }
        }

        [ResourceEntry("Discount", Value = "Discount", Description = "Discount", LastModified = "2020/02/21")]
        public string Discount
        {
            get { return this["Discount"]; }
        }

        [ResourceEntry("Discounts", Value = "Discounts", Description = "Discounts", LastModified = "2020/02/21")]
        public string Discounts
        {
            get { return this["Discounts"]; }
        }

        [ResourceEntry("Availability", Value = "Availability", Description = "Availability", LastModified = "2020/02/21")]
        public string Availability
        {
            get { return this["Availability"]; }
        }

        [ResourceEntry("InStock", Value = "In Stock", Description = "In Stock", LastModified = "2020/02/21")]
        public string InStock
        {
            get { return this["In Stock"]; }
        }

        [ResourceEntry("OutOfStock", Value = "Out of Stock", Description = "Out of Stock", LastModified = "2020/02/21")]
        public string OutOfStock
        {
            get { return this["Out Of Stock"]; }
        }

        [ResourceEntry("PleaseSelect", Value = "Please select...", Description = "Please select...", LastModified = "2020/02/21")]
        public string PleaseSelect
        {
            get { return this["Please select..."]; }
        }

        [ResourceEntry("AddToWishList", Value = "Add To Wish List", Description = "Add To Wish List", LastModified = "2020/02/21")]
        public string AddToWishList
        {
            get { return this["Add To Wish List"]; }
        }

        [ResourceEntry("AddToCart", Value = "Add To Cart", Description = "Add To Cart", LastModified = "2020/02/21")]
        public string AddToCart
        {
            get { return this["Add To Cart"]; }
        }

        [ResourceEntry("Products", Value = "products", Description = "products", LastModified = "2020/02/21")]
        public string Products
        {
            get { return this["products"]; }
        }

        [ResourceEntry("ProductQuantity", Value = "Product Quantity", Description = "Product Quantity", LastModified = "2020/02/21")]
        public string ProductQuantity
        {
            get { return this["Product Quantity"]; }
        }

        [ResourceEntry("SpecifyQuantity", Value = "When adding product to a cart you must specify the quantity", Description = "When adding product to a cart you must specify the quantity", LastModified = "2020/02/21")]
        public string SpecifyQuantity
        {
            get { return this["When adding product to a cart you must specify the quantity"]; }
        }

        [ResourceEntry("QuantityValidation", Value = "The quantity must be greater than 0 and less than 9,999.", Description = "The quantity must be greater than 0 and less than 9,999.", LastModified = "2020/02/21")]
        public string QuantityValidation
        {
            get { return this["The quantity must be greater than 0 and less than 9,999."]; }
        }

        [ResourceEntry("Quantity", Value = "Quantity", Description = "Quantity", LastModified = "2020/02/21")]
        public string Quantity
        {
            get { return this["Quantity"]; }
        }

        [ResourceEntry("Price", Value = "Price", Description = "Price", LastModified = "2020/02/21")]
        public string Price
        {
            get { return this["Price"]; }
        }

        [ResourceEntry("VAT", Value = "VAT", Description = "VAT", LastModified = "2020/02/21")]
        public string VAT
        {
            get { return this["VAT"]; }
        }

        [ResourceEntry("Total", Value = "Total", Description = "Total", LastModified = "2020/02/21")]
        public string Total
        {
            get { return this["Total"]; }
        }

        [ResourceEntry("Update", Value = "Update", Description = "Update", LastModified = "2020/02/21")]
        public string Update
        {
            get { return this["Update"]; }
        }

        [ResourceEntry("Remove", Value = "Remove", Description = "Remove", LastModified = "2020/02/21")]
        public string Remove
        {
            get { return this["Remove"]; }
        }

        [ResourceEntry("SubTotal", Value = "Sub Total", Description = "Sub Total", LastModified = "2020/02/21")]
        public string SubTotal
        {
            get { return this["Sub Total"]; }
        }

        [ResourceEntry("OrderTotal", Value = "Order Total", Description = "Order Total", LastModified = "2020/02/21")]
        public string OrderTotal
        {
            get { return this["Order Total"]; }
        }

        [ResourceEntry("Continue", Value = "Continue", Description = "Continue", LastModified = "2020/02/21")]
        public string Continue
        {
            get { return this["Continue"]; }
        }

        [ResourceEntry("Description", Value = "Description", Description = "Description", LastModified = "2020/02/21")]
        public string Description
        {
            get { return this["Description"]; }
        }

        [ResourceEntry("BillingAddress", Value = "Billing Address", Description = "Billing Address", LastModified = "2020/02/21")]
        public string BillingAddress
        {
            get { return this["Billing Address"]; }
        }

        [ResourceEntry("ShippingAddress", Value = "Shipping Address", Description = "Shipping Address", LastModified = "2020/02/21")]
        public string ShippingAddress
        {
            get { return this["ComShippingAddresspany"]; }
        }

        [ResourceEntry("FirstName", Value = "First Name", Description = "First Name", LastModified = "2020/02/21")]
        public string FirstName
        {
            get { return this["First Name"]; }
        }

        [ResourceEntry("LastName", Value = "Last Name", Description = "Last Name", LastModified = "2020/02/21")]
        public string LastName
        {
            get { return this["Last Name"]; }
        }

        [ResourceEntry("Email", Value = "E-mail", Description = "E-mail", LastModified = "2020/02/21")]
        public string Email
        {
            get { return this["E-mail"]; }
        }

        [ResourceEntry("Attention", Value = "Attention", Description = "Attention", LastModified = "2020/02/21")]
        public string Attention
        {
            get { return this["Attention"]; }
        }

        [ResourceEntry("MobilePhone", Value = "Mobile Phone", Description = "Mobile Phone", LastModified = "2020/02/21")]
        public string MobilePhone
        {
            get { return this["MobilePhone"]; }
        }

        [ResourceEntry("Phone", Value = "Phone", Description = "Phone", LastModified = "2020/02/21")]
        public string Phone
        {
            get { return this["Phone"]; }
        }

        [ResourceEntry("Street", Value = "Street", Description = "Street", LastModified = "2020/02/21")]
        public string Street
        {
            get { return this["Street"]; }
        }

        [ResourceEntry("Street2", Value = "Street 2", Description = "Street 2", LastModified = "2020/02/21")]
        public string AttenStreet2tion
        {
            get { return this["Street2"]; }
        }

        [ResourceEntry("PostalCode", Value = "Postal Code", Description = "Postal Code", LastModified = "2020/02/21")]
        public string PostalCode
        {
            get { return this["PostalCode"]; }
        }

        [ResourceEntry("City", Value = "City", Description = "City", LastModified = "2020/02/21")]
        public string City
        {
            get { return this["City"]; }
        }

        [ResourceEntry("Country", Value = "Country", Description = "Country", LastModified = "2020/02/21")]
        public string Country
        {
            get { return this["Country"]; }
        }

        [ResourceEntry("Company", Value = "Company", Description = "Company", LastModified = "2020/02/21")]
        public string Company
        {
            get { return this["Company"]; }
        }

        [ResourceEntry("DifferentShippingAddress", Value = "Use a different address for shipping", Description = "Use a different address for shipping", LastModified = "2020/02/21")]
        public string DifferentShippingAddress
        {
            get { return this["Use a different address for shipping"]; }
        }

        [ResourceEntry("ItemNo", Value = "Item no.", Description = "Item no.", LastModified = "2020/02/21")]
        public string ItemNo
        {
            get { return this["Item no."]; }
        }

        [ResourceEntry("OrderDiscounts", Value = "Order Discounts", Description = "Order Discounts", LastModified = "2020/02/21")]
        public string OrderDiscounts
        {
            get { return this["Order Discounts"]; }
        }

        [ResourceEntry("Payment", Value = "Payment", Description = "Payment", LastModified = "2020/02/21")]
        public string Payment
        {
            get { return this["Payment"]; }
        }

        [ResourceEntry("CompleteOrder", Value = "Complete Order", Description = "Complete Order", LastModified = "2020/02/21")]
        public string CompleteOrder
        {
            get { return this["Complete Order"]; }
        }

        [ResourceEntry("EmptyBasket", Value = "Your basket is empty", Description = "Your basket is empty", LastModified = "2020/02/21")]
        public string EmptyBasket
        {
            get { return this["Your basket is empty"]; }
        }

        [ResourceEntry("PaymentMethod", Value = "Payment method", Description = "Payment method", LastModified = "2020/02/21")]
        public string PaymentMethod
        {
            get { return this["Payment method"]; }
        }

        [ResourceEntry("ShippingMethod", Value = "Shipping Method", Description = "Shipping Method", LastModified = "2020/02/21")]
        public string ShippingMethod
        {
            get { return this["Shipping Method"]; }
        }

        [ResourceEntry("ContinueToNextStep", Value = "Continue to next step", Description = "Continue to next step", LastModified = "2020/02/21")]
        public string ContinueToNextStep
        {
            get { return this["Continue to next step"]; }
        }
    }
}
