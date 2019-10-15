using System;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.DataResolving;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class CartModel : ICartModel
    {
        private Guid productDetailsPageId;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private Guid nextStepId;

        public CartModel(Guid? nextStepId = null, Guid? productDetailsPageId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
        }

        public CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl)
        {
            var basketVM = new CartRenderingViewModel();

            if (!_transactionLibraryInternal.HasBasket())
            {
                return basketVM;
            }

            PurchaseOrder basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new OrderlineViewModel
                {
                    Quantity = orderLine.Quantity,
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString(),
                    Discount = orderLine.Discount,
                    Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString(),
                    Price = new Money(orderLine.Price, basket.BillingCurrency).ToString(),
                    ProductUrl = GetProductUrl(CatalogLibrary.GetProduct(orderLine.Sku), this.productDetailsPageId),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), basket.BillingCurrency).ToString(),
                    OrderLineId = orderLine.OrderLineId
                };
                basketVM.OrderLines.Add(orderLineViewModel);
            }

            basketVM.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.NextStepUrl = GetNextStepUrl(nextStepId);

            basketVM.RefreshUrl = refreshUrl;
            basketVM.RemoveOrderlineUrl = removeOrderLineUrl;

            return basketVM;
        }

        public string GetProductUrl(Product product, Guid detailPageId)
        {
            if (detailPageId == Guid.Empty)
            {
                return CatalogLibrary.GetNiceUrlForProduct(product);
            }

            var baseUrl = Ucommerce.Sitefinity.UI.Pages.UrlResolver.GetPageNodeUrl(detailPageId);
            
            string catUrl;
            var productCategory = product.GetCategories().FirstOrDefault();
            if (productCategory == null)
            {
                catUrl = CategoryModel.DefaultCategoryName;
            }
            else
            {
                catUrl = CategoryModel.GetCategoryPath(productCategory);
            }

            var rawtUrl = string.Format("{0}/{1}", catUrl, product.ProductId);
            string relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", rawtUrl);

            string url;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                url = UrlPath.ResolveUrl(relativeUrl, true);
            }
            else
            {
                url = Ucommerce.Sitefinity.UI.Pages.UrlResolver.GetAbsoluteUrl(relativeUrl);
            }

            return url;
        }

        public CartUpdateBasketViewModel Update(CartUpdateBasket model)
        {
            foreach (var updateOrderline in model.RefreshBasket)
            {
                var newQuantity = updateOrderline.OrderLineQty;
                if (newQuantity <= 0)
                {
                    newQuantity = 0;
                }

                _transactionLibraryInternal.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
            }

            _transactionLibraryInternal.ExecuteBasketPipeline();

            var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            CartUpdateBasketViewModel updatedBasket = new CartUpdateBasketViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new CartUpdateOrderline();
                orderLineViewModel.OrderlineId = orderLine.OrderLineId;
                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();

                updatedBasket.OrderLines.Add(orderLineViewModel);
            }

            string orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string discountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string subTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

            updatedBasket.OrderTotal = orderTotal;
            updatedBasket.DiscountTotal = discountTotal;
            updatedBasket.TaxTotal = taxTotal;
            updatedBasket.SubTotal = subTotal;

            return updatedBasket;
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }
    }
}
