using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace UCommerce.Sitefinity.UI.Resources
{
    [ObjectInfo("UcommerceResources", ResourceClassId = "UcommerceResources", Title = "UCommerceResource", TitlePlural = "UCommerceResources", Description = "CustomResourcesUCommerce")]
    class UcommerceResources : Resource
    {
        public UcommerceResources()
        {
        }

        public UcommerceResources(ResourceDataProvider dataProvider) : base(dataProvider)
        {
        }

        [ResourceEntry("UCommerceResource", Value = "UCommerce Resources", Description = "UCommerce Resources", LastModified = "2020/02/21")]
        public string UCommerceResource
        {
            get { return this["UCommerceResource"]; }
        }

        [ResourceEntry("WriteReview", Value = "Write a review", Description = "Write a review", LastModified = "2020/02/21")]
        public string WriteReview
        {
            get { return this["WriteReview"]; }
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
            get { return this["InStock"]; }
        }

        [ResourceEntry("OutOfStock", Value = "Out of Stock", Description = "Out of Stock", LastModified = "2020/02/21")]
        public string OutOfStock
        {
            get { return this["OutOfStock"]; }
        }

        [ResourceEntry("PleaseSelect", Value = "Please select...", Description = "Please select...", LastModified = "2020/02/21")]
        public string PleaseSelect
        {
            get { return this["PleaseSelect"]; }
        }

        [ResourceEntry("AddToWishList", Value = "Add To Wish List", Description = "Add To Wish List", LastModified = "2020/02/21")]
        public string AddToWishList
        {
            get { return this["AddToWishList"]; }
        }

        [ResourceEntry("AddToCart", Value = "Add To Cart", Description = "Add To Cart", LastModified = "2020/02/21")]
        public string AddToCart
        {
            get { return this["AddToCart"]; }
        }

        [ResourceEntry("Products", Value = "products", Description = "products", LastModified = "2020/02/21")]
        public string Products
        {
            get { return this["Products"]; }
        }

        [ResourceEntry("ProductQuantity", Value = "Product Quantity", Description = "Product Quantity", LastModified = "2020/02/21")]
        public string ProductQuantity
        {
            get { return this["ProductQuantity"]; }
        }

        [ResourceEntry("SpecifyQuantity", Value = "When adding product to a cart you must specify the quantity", Description = "When adding product to a cart you must specify the quantity", LastModified = "2020/02/21")]
        public string SpecifyQuantity
        {
            get { return this["SpecifyQuantity"]; }
        }

        [ResourceEntry("QuantityValidation", Value = "The quantity must be greater than 0 and less than 9,999.", Description = "The quantity must be greater than 0 and less than 9,999.", LastModified = "2020/02/21")]
        public string QuantityValidation
        {
            get { return this["QuantityValidation"]; }
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
            get { return this["SubTotal"]; }
        }

        [ResourceEntry("OrderTotal", Value = "Order Total", Description = "Order Total", LastModified = "2020/02/21")]
        public string OrderTotal
        {
            get { return this["OrderTotal"]; }
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

        [ResourceEntry("Add", Value = "Add", Description = "Add", LastModified = "2020/02/27")]
        public string Add
        {
            get { return this["Add"]; }
        }

        [ResourceEntry("Apply", Value = "Apply", Description = "Apply", LastModified = "2020/05/03")]
        public string Apply
        {
            get { return this["Apply"]; }
        }

        [ResourceEntry("HasAVoucher", Value = "HAVE A COUPON CODE?", Description = "HAVE A COUPON CODE?", LastModified = "2020/02/27")]
        public string HasAVoucher
        {
            get { return this["HasAVoucher"]; }
        }

        [ResourceEntry("DiscountCode", Value = "Discount code", Description = "Discount code", LastModified = "2020/02/27")]
        public string DiscountCode
        {
            get { return this["DiscountCode"]; }
        }

        [ResourceEntry("BillingAddress", Value = "Billing Address", Description = "Billing Address", LastModified = "2020/02/21")]
        public string BillingAddress
        {
            get { return this["BillingAddress"]; }
        }

        [ResourceEntry("ShippingAddress", Value = "Shipping Address", Description = "Shipping Address", LastModified = "2020/02/21")]
        public string ShippingAddress
        {
            get { return this["ShippingAddress"]; }
        }

        [ResourceEntry("FirstName", Value = "First Name", Description = "First Name", LastModified = "2020/02/21")]
        public string FirstName
        {
            get { return this["FirstName"]; }
        }

        [ResourceEntry("LastName", Value = "Last Name", Description = "Last Name", LastModified = "2020/02/21")]
        public string LastName
        {
            get { return this["LastName"]; }
        }

        [ResourceEntry("Email", Value = "E-mail", Description = "E-mail", LastModified = "2020/02/21")]
        public string Email
        {
            get { return this["Email"]; }
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
            get { return this["DifferentShippingAddress"]; }
        }

        [ResourceEntry("ItemNo", Value = "Item no.", Description = "Item no.", LastModified = "2020/02/21")]
        public string ItemNo
        {
            get { return this["ItemNo"]; }
        }

        [ResourceEntry("OrderDiscounts", Value = "Order Discounts", Description = "Order Discounts", LastModified = "2020/02/21")]
        public string OrderDiscounts
        {
            get { return this["OrderDiscounts"]; }
        }

        [ResourceEntry("Payment", Value = "Payment", Description = "Payment", LastModified = "2020/02/21")]
        public string Payment
        {
            get { return this["Payment"]; }
        }

        [ResourceEntry("CompleteOrder", Value = "Complete Order", Description = "Complete Order", LastModified = "2020/02/21")]
        public string CompleteOrder
        {
            get { return this["CompleteOrder"]; }
        }

        [ResourceEntry("EmptyBasket", Value = "Your basket is empty", Description = "Your basket is empty", LastModified = "2020/02/21")]
        public string EmptyBasket
        {
            get { return this["EmptyBasket"]; }
        }

        [ResourceEntry("PaymentMethod", Value = "Payment method", Description = "Payment method", LastModified = "2020/02/21")]
        public string PaymentMethod
        {
            get { return this["PaymentMethod"]; }
        }

        [ResourceEntry("ShippingMethod", Value = "Shipping Method", Description = "Shipping Method", LastModified = "2020/02/21")]
        public string ShippingMethod
        {
            get { return this["ShippingMethod"]; }
        }

        [ResourceEntry("ContinueToNextStep", Value = "Continue to next step", Description = "Continue to next step", LastModified = "2020/02/21")]
        public string ContinueToNextStep
        {
            get { return this["ContinueToNextStep"]; }
        }

        [ResourceEntry("FirstNameValidation", Value = "First name is required", Description = "First name is required", LastModified = "2020/02/25")]
        public string FirstNameValidation
        {
            get { return this["FirstNameValidation"]; }
        }

        [ResourceEntry("LastNameValidation", Value = "Last name is required", Description = "Last name is required", LastModified = "2020/02/25")]
        public string LastNameValidation
        {
            get { return this["LastNameValidation"]; }
        }

        [ResourceEntry("EmailAddressValidation", Value = "Email is required", Description = "Email is required", LastModified = "2020/02/25")]
        public string EmailAddressValidation
        {
            get { return this["EmailAddressValidation"]; }
        }

        [ResourceEntry("Line1Validation", Value = "Address line 1 is required", Description = "Address line 1 is required", LastModified = "2020/02/25")]
        public string Line1Validation
        {
            get { return this["Line1Validation"]; }
        }

        [ResourceEntry("PostalCodeValidation", Value = "Postal code is required", Description = "Postal code is required", LastModified = "2020/02/25")]
        public string PostalCodeValidation
        {
            get { return this["PostalCodeValidation"]; }
        }

        [ResourceEntry("CityValidation", Value = "City is required", Description = "City is required", LastModified = "2020/02/25")]
        public string CityValidation
        {
            get { return this["CityValidation"]; }
        }

        [ResourceEntry("BackButton", Value = "Back", Description = "Back Button", LastModified = "2020/02/26")]
        public string BackButton
        {
            get { return this["BackButton"]; }
        }

        [ResourceEntry("ThankYou", Value = "Thank you for the order", Description = "Thank you for the order", LastModified = "2020/02/27")]
        public string ThankYou
        {
            get { return this["ThankYou"]; }
        }

        [ResourceEntry("ContactUs", Value = "Contact Us", Description = "Contact Us", LastModified = "2020/02/27")]
        public string ContactUs
        {
            get { return this["ContactUs"]; }
        }

        [ResourceEntry("Quantum", Value = "Quantum", Description = "Quantum", LastModified = "2020/02/27")]
        public string Quantum
        {
            get { return this["Quantum"]; }
        }

        [ResourceEntry("Copyright", Value = "Copyright © 2002-2019 Quantum. All rights reserved.", Description = "Copyright Message", LastModified = "2020/02/27")]
        public string Copyright
        {
            get { return this["Copyright"]; }
        }

        [ResourceEntry("Shipping", Value = "Shipping", Description = "Shipping", LastModified = "2020/02/27")]
        public string Shipping
        {
            get { return this["Shipping"]; }
        }

        [ResourceEntry("AddToBasket", Value = "Added to basket", Description = "Added to basket", LastModified = "2020/02/27")]
        public string AddToBasket
        {
            get { return this["AddToBasket"]; }
        }

        [ResourceEntry("NotAddToBasket", Value = "Not added to basket", Description = "Not added to basket", LastModified = "2020/02/27")]
        public string NotAddToBasket
        {
            get { return this["NotAddToBasket"]; }
        }

        [ResourceEntry("ShoppingCart", Value = "Shopping cart", Description = "Shopping Cart", LastModified = "2020/02/27")]
        public string ShoppingCart
        {
            get { return this["ShoppingCart"]; }
        }

        [ResourceEntry("ContinueShopping", Value = "Continue shopping", Description = "Continue shopping", LastModified = "2020/02/27")]
        public string ContinueShopping
        {
            get { return this["ContinueShopping"]; }
        }

        [ResourceEntry("ReturnToStore", Value = "return to our store and add some items", Description = "Return to Store", LastModified = "2020/02/26")]
        public string ReturnToStore
        {
            get { return this["ReturnToStore"]; }
        }

        [ResourceEntry("YourCartIsEmpty", Value = "Your cart is empty. Please", Description = "Your Cart is Empty", LastModified = "2020/02/26")]
        public string YourCartIsEmpty
        {
            get { return this["YourCartIsEmpty"]; }
        }

        [ResourceEntry("Review", Value = "Review", Description = "Review", LastModified = "2020/03/02")]
        public string Review
        {
            get { return this["Review"]; }
        }

        [ResourceEntry("Reviews", Value = "Reviews", Description = "Reviews", LastModified = "2020/03/02")]
        public string Reviews
        {
            get { return this["Reviews"]; }
        }

        [ResourceEntry("AverageRating", Value = "Average Rating", Description = "Average Rating", LastModified = "2020/03/02")]
        public string AverageRating
        {
            get { return this["AverageRating"]; }
        }

        [ResourceEntry("Rating", Value = "Rating", Description = "Rating", LastModified = "2020/03/02")]
        public string Rating
        {
            get { return this["Rating"]; }
        }

        [ResourceEntry("YourName", Value = "Your name", Description = "Your name", LastModified = "2020/03/02")]
        public string YourName
        {
            get { return this["YourName"]; }
        }

        [ResourceEntry("EmailOptional", Value = "Email (optional)", Description = "Email (optional)", LastModified = "2020/03/02")]
        public string EmailOptional
        {
            get { return this["EmailOptional"]; }
        }

        [ResourceEntry("Submit", Value = "Submit", Description = "Submit", LastModified = "2020/03/02")]
        public string Submit
        {
            get { return this["Submit"]; }
        }

        [ResourceEntry("ShowAllResults", Value = "Show all results", Description = "Show all results", LastModified = "2020/03/02")]
        public string ShowAllResults
        {
            get { return this["ShowAllResults"]; }
        }

        [ResourceEntry("ClearFilter", Value = "Clear filter", Description = "Clear Filter", LastModified = "2020/03/02")]
        public string ClearFilter
        {
            get { return this["ClearFilter"]; }
        }

        [ResourceEntry("FilterProducts", Value = "Filter products", Description = "Filter Products", LastModified = "2020/03/02")]
        public string FilterProducts
        {
            get { return this["FilterProducts"]; }
        }

        [ResourceEntry("OrderConfirmation", Value = "An order confirmation email has been sent to your email address", Description = "Order Confiramtion Message", LastModified = "2020/03/12")]
        public string OrderConfirmation
        {
            get { return this["OrderConfirmation"]; }
        }

        [ResourceEntry("Items", Value = "Items", Description = "Items", LastModified = "2020/03/12")]
        public string Items
        {
            get { return this["Items"]; }
        }
        
        [ResourceEntry("ViewShoppingCart", Value = "View shopping cart", Description = "View shopping cart", LastModified = "2020/05/04")]
        public string ViewShoppingCart
        {
            get { return this["ViewShoppingCart"]; }
        }

        [ResourceEntry("Checkout", Value = "Checkout", Description = "Checkout", LastModified = "2020/05/04")]
        public string Checkout
        {
            get { return this["Checkout"]; }
        }
    }
}