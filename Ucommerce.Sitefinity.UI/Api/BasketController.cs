using System.Linq;
using System.Web.Http;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Content;
using Ucommerce.Search.Slugs;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints for managing the basket.
    /// </summary>
    public class BasketController : ApiController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public IMarketingLibrary MarketingLibrary => ObjectFactory.Instance.Resolve<IMarketingLibrary>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

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
            var priceGroupRepository = ObjectFactory.Instance.Resolve<Ucommerce.EntitiesV2.IRepository<Ucommerce.EntitiesV2.PriceGroup>>();
            CatalogLibrary.ChangePriceGroup(priceGroupRepository.Get(model.PriceGroupId).Guid);

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
            if (model.Quantity < 1)
            {
                var responseDTO = new OperationStatusDTO();
                responseDTO.Status = "failed";
                responseDTO.Message = "Quantity must be greater than 0";

                return this.Json(responseDTO);
            }

            string variantSku = null;
            var product = CatalogLibrary.GetProduct(model.Sku);
            var variants = CatalogLibrary.GetVariants(product);

            if (model.Variants == null || !model.Variants.Any())
            {
                var variant = variants.FirstOrDefault();

                if (variant == null)
                {
                    var responseDTO = new OperationStatusDTO();
                    responseDTO.Status = "failed";
                    responseDTO.Message = $"No Variant with '{ model.Sku}' exists.";

                    return this.Json(responseDTO);
                }

                variantSku = variant.VariantSku;
            }
            else
            {
                var variant = variants.FirstOrDefault(x => model.Variants.Any(y => y.Value == x.VariantSku));
                if (variant != null)
                {
                    variantSku = variant.VariantSku;
                }
            }

            TransactionLibrary.AddToBasket((int)model.Quantity, model.Sku, variantSku);
            return Json(this.GetBasketModel());
        }

        private BasketDTO GetBasketModel()
        {
            var model = new BasketDTO();
            var basket = TransactionLibrary.GetBasket(false);
            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            var imageService = ObjectFactory.Instance.Resolve<IImageService>();
            var urlService = ObjectFactory.Instance.Resolve<IUrlService>();

            foreach (var orderLine in basket.OrderLines)
            {

                var product = CatalogLibrary.GetProduct(orderLine.Sku);
                var url = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                var thumbnailImageUrl = product.ThumbnailImageUrl;
                var lineTotal = new Money(orderLine.Total.Value, currencyIsoCode);

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
