using System.Linq;
using System.Web.Http;
using Ucommerce.Sitefinity.UI.Api.Model;
using UCommerce;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Runtime;

namespace Ucommerce.Sitefinity.UI.Api
{
    [RoutePrefix("Basket")]
    public class BasketController : ApiController
    {
        [Route("GetBasket")]
        public IHttpActionResult GetBasket()
        {
            if (!TransactionLibrary.HasBasket())
            {
                return Json(new BasketModel());
            }

            return Json(this.GetBasketModel());
        }

        [Route("AddVoucher")]
        public IHttpActionResult AddVoucher(AddVoucherModel model)
        {
            var voucherAdded = MarketingLibrary.AddVoucher(model.VoucherCode);
            TransactionLibrary.ExecuteBasketPipeline();
            return Json(this.GetBasketModel());
        }

        [Route("ChangePriceGroup")]
        public IHttpActionResult ChangePriceGroup(ChangePriceGroupModel model)
        {
            var priceGroupRepository = ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>();
            CatalogLibrary.ChangePriceGroup(priceGroupRepository.Get(model.PriceGroupId));

            return Ok();
        }

        [Route("UpdateLineItem")]
        [HttpPost]
        public IHttpActionResult UpdateLineItem(UpdateLineItemModel model)
        {
            TransactionLibrary.UpdateLineItem(model.OrderlineId, model.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();

            return Json(this.GetBasketModel());
        }

        [Route("AddToBasket")]
        [HttpPost]
        public IHttpActionResult AddToBasket(AddToBasketModel model)
        {
            TransactionLibrary.AddToBasket(model.Quantity, model.Sku, model.VariantSku);
            return Json(this.GetBasketModel());
        }

        [Route("AddToBasketMock")]
        [HttpPost]
        public IHttpActionResult AddToBasketMock(AddToBasketModel model)
        {
            return Ok();
        }

        private BasketModel GetBasketModel()
        {
            var model = new BasketModel();
            var basket = TransactionLibrary.GetBasket(false).PurchaseOrder;
            var imageService = ObjectFactory.Instance.Resolve<IImageService>();
            var urlService = ObjectFactory.Instance.Resolve<IUrlService>();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineModel = this.MapOrderLine(orderLine);
                var currentCatalog = SiteContext.Current.CatalogContext.CurrentCatalog;
                orderLineModel.ProductUrl = urlService.GetUrl(currentCatalog, Product.FirstOrDefault(x => x.Sku == orderLine.Sku));

                orderLineModel.ThumbnailImageMediaUrl = imageService
                    .GetImage(Product.FirstOrDefault(x => x.Sku == orderLine.Sku).ThumbnailImageMediaId).Url;

                model.OrderLines.Add(orderLineModel);
            }

            foreach (var discount in basket.Discounts)
            {
                model.Discounts.Add(new DiscountModel
                {
                    Name = discount.CampaignItemName,
                    Value = new Money(discount.AmountOffTotal, basket.BillingCurrency).ToString(),
                });
            }

            model.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            model.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            model.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            model.HasDiscount = basket.Discount.GetValueOrDefault() > 0;
            model.NumberOfItemsInBasket = basket.OrderLines.Sum(x => x.Quantity);
            model.ShippingTotal =
                new Money(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            model.PaymentTotal = new Money(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

            return model;
        }

        private OrderLineModel MapOrderLine(OrderLine orderLine)
        {
            var orderLineViewModel = new OrderLineModel
            {
                ProductName = orderLine.ProductName,
                Quantity = orderLine.Quantity,
                UnitPrice = new Money(orderLine.Price, orderLine.PurchaseOrder.BillingCurrency).ToString(),
                Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString(),
                OrderlineId = orderLine.OrderLineId,
                HasDiscount = orderLine.Discounts.Any(),
            };

            return orderLineViewModel;
        }
    }
}
