using System.Linq;
using System.Web.Http;
using UCommerce.Api;
using UCommerce.Catalog;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints for managing the basket.
    /// </summary>
    public class BasketController : ApiController
    {
        [Route(RouteConstants.GET_BASKET_ROUTE_VALUE)]
        [HttpGet]
        public IHttpActionResult GetBasket()
        {
            if (!TransactionLibrary.HasBasket())
            {
                return Json(new BasketDTO());
            }

            return Json(this.GetBasketModel());
        }

        [Route(RouteConstants.ADD_VOUCHER_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult AddVoucher(AddVoucherDTO model)
        {
            var voucherAdded = MarketingLibrary.AddVoucher(model.VoucherCode);
            TransactionLibrary.ExecuteBasketPipeline();
            return Json(this.GetBasketModel());
        }

        [Route(RouteConstants.PRICE_GROUP_ROUTE_VALUE)]
        [HttpPut]
        public IHttpActionResult ChangePriceGroup(ChangePriceGroupDTO model)
        {
            var priceGroupRepository = ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>();
            CatalogLibrary.ChangePriceGroup(priceGroupRepository.Get(model.PriceGroupId));

            return Ok();
        }

        [Route(RouteConstants.UPDATE_LINE_ITEM_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult UpdateLineItem(UpdateLineItemDTO model)
        {
            TransactionLibrary.UpdateLineItem(model.OrderlineId, model.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();

            return Json(this.GetBasketModel());
        }

        [Route(RouteConstants.ADD_TO_BASKET_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Add(AddToBasketDTO model)
        {
            string variantSku = null;
            var product = CatalogLibrary.GetProduct(model.Sku);

            if (model.Variants == null || !model.Variants.Any())
            {
                var variant = product.Variants.FirstOrDefault();

                if (variant != null)
                {
                    variantSku = variant.VariantSku;
                }
            }
            else
            {
                var variants = product.Variants.AsEnumerable();

                foreach (var v in model.Variants)
                {
                    variants = variants.Where(pv => pv.ProductProperties.Any(pp => pp.Value == v.Value));
                }

                var variant = variants.FirstOrDefault();
                if (variant != null)
                {
                    variantSku = variant.VariantSku;
                }
            }

            TransactionLibrary.AddToBasket(model.Quantity, model.Sku, variantSku);
            return Json(this.GetBasketModel());
        }

        private BasketDTO GetBasketModel()
        {
            var model = new BasketDTO();
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
                model.Discounts.Add(new DiscountDTO
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

        private OrderLineDTO MapOrderLine(OrderLine orderLine)
        {
            var orderLineViewModel = new OrderLineDTO
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
