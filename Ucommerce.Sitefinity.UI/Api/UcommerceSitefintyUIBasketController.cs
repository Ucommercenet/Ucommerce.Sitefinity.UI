using System.Linq;
using System.Web.Http;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Slugs;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints for managing the basket.
    /// </summary>
    public class UcommerceSitefintyUiBasketController : ApiController
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
        public IMarketingLibrary MarketingLibrary => ObjectFactory.Instance.Resolve<IMarketingLibrary>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        [Route(RouteConstants.ADD_TO_BASKET_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Add(AddToBasketDTO model)
        {
            if (model.Quantity < 1)
            {
                var responseDTO = new OperationStatusDTO();
                responseDTO.Status = "failed";
                responseDTO.Message = "Quantity must be greater than 0";

                return Json(responseDTO);
            }

            string variantSku = null;
            var product = Product.FirstOrDefault(x => x.Sku == model.Sku && x.VariantSku == null);
            if (product == null)
            {
                var responseDTO = new OperationStatusDTO();
                responseDTO.Status = "failed";
                responseDTO.Message = $"No product with SKU: '{model.Sku}'";

                return Json(responseDTO);
            }

            if (model.Variants != null && model.Variants.Any())
            {
                var productQuery = Product.All()
                    .Where(x => x.Sku == model.Sku);
                foreach (var variant in model.Variants)
                {
                    productQuery = productQuery.Where(x =>
                        x.ProductProperties.Any(y => y.Value == variant.Value && y.ProductDefinitionField.Name == variant.TypeName));
                }

                var result = productQuery.FirstOrDefault();

                if (result != null)
                {
                    variantSku = result.VariantSku;
                }
            }

            TransactionLibrary.AddToBasket((int)model.Quantity, model.Sku, variantSku);

            InsightUcommerce.SendProductInteraction(product, "Add to shopping cart", $"{product.Name} ({product.Sku})");

            return Json(GetBasketModel());
        }

        [Route(RouteConstants.ADD_VOUCHER_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult AddVoucher(AddVoucherDTO model)
        {
            MarketingLibrary.AddVoucher(model.VoucherCode);
            TransactionLibrary.ExecuteBasketPipeline();
            return Json(GetBasketModel());
        }

        [Route(RouteConstants.PRICE_GROUP_ROUTE_VALUE)]
        [HttpPut]
        public IHttpActionResult ChangePriceGroup(ChangePriceGroupDTO model)
        {
            var priceGroupRepository = ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>();
            CatalogLibrary.ChangePriceGroup(priceGroupRepository.Get(model.PriceGroupId)
                .Guid);

            return Ok();
        }

        [Route(RouteConstants.GET_BASKET_ROUTE_VALUE)]
        [HttpGet]
        public IHttpActionResult GetBasket()
        {
            if (!TransactionLibrary.HasBasket())
            {
                return Json(new BasketDTO());
            }

            return Json(GetBasketModel());
        }

        [Route(RouteConstants.UPDATE_LINE_ITEM_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult UpdateLineItem(UpdateLineItemDTO model)
        {
            if (model.NewQuantity == 0)
            {
                var orderLine = OrderLine.Get(model.OrderlineId);
                var product = Product.FirstOrDefault(p => p.Sku == orderLine.Sku && p.VariantSku == orderLine.VariantSku);
                InsightUcommerce.SendProductInteraction(product, "Remove product from cart", $"{product?.Name} ({product?.Sku})");
            }

            TransactionLibrary.UpdateLineItem(model.OrderlineId, model.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();

            return Json(GetBasketModel());
        }

        private BasketDTO GetBasketModel()
        {
            var model = new BasketDTO();
            var basket = TransactionLibrary.GetBasket();
            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            ObjectFactory.Instance.Resolve<IImageService>();
            ObjectFactory.Instance.Resolve<IUrlService>();

            foreach (var orderLine in basket.OrderLines)
            {
                var product = CatalogLibrary.GetProduct(orderLine.Sku);
                var url = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                var thumbnailImageUrl = product.ThumbnailImageUrl;
                new Money(orderLine.Total.Value, currencyIsoCode);

                var orderLineModel = new OrderLineDTO
                {
                    ProductName = orderLine.ProductName,
                    Quantity = orderLine.Quantity,
                    UnitPrice = new Money(orderLine.Price, currencyIsoCode).ToString(),
                    Total = new Money(orderLine.Total.GetValueOrDefault(), currencyIsoCode).ToString(),
                    OrderlineId = orderLine.OrderLineId,
                    HasDiscount = orderLine.Discounts.Any(),
                    ProductUrl = url,
                    ThumbnailImageMediaUrl = thumbnailImageUrl,
                };
                model.OrderLines.Add(orderLineModel);
            }

            foreach (var discount in basket.Discounts)
            {
                model.Discounts.Add(new DiscountDTO
                {
                    Name = discount.CampaignItemName,
                    Value = new Money(discount.AmountOffTotal, currencyIsoCode).ToString(),
                });
            }

            model.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            model.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            model.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            model.HasDiscount = basket.Discount.GetValueOrDefault() > 0;
            model.NumberOfItemsInBasket = basket.OrderLines.Sum(x => x.Quantity);
            model.ShippingTotal =
                new Money(basket.ShippingTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            model.PaymentTotal = new Money(basket.PaymentTotal.GetValueOrDefault(), currencyIsoCode).ToString();

            return model;
        }
    }
}
