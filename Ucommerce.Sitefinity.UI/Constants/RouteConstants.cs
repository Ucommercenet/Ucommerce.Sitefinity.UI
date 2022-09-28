namespace UCommerce.Sitefinity.UI.Constants
{
    /// <summary>
    /// This class contains the route constants used by the UCommerce UI Module. 
    /// </summary>
    internal class RouteConstants
    {
        public const string ADD_TO_BASKET_ROUTE_NAME = "addToBasketUrl";
        public const string ADD_TO_BASKET_ROUTE_VALUE = ROUTE_PREFIX + "/basket/add";
        public const string ADD_VOUCHER_ROUTE_NAME = "addVoucherUrl";
        public const string ADD_VOUCHER_ROUTE_VALUE = ROUTE_PREFIX + "/basket/voucher";
        public const string GET_BASKET_ROUTE_NAME = "getBasketUrl";
        public const string GET_BASKET_ROUTE_VALUE = ROUTE_PREFIX + "/basket/get";
        public const string PRICE_GROUP_ROUTE_NAME = "changePriceGroupUrl";
        public const string PRICE_GROUP_ROUTE_VALUE = ROUTE_PREFIX + "/price-group";
        public const string ROUTE_PREFIX = "storeapi";
        public const string SEARCH_ROUTE_NAME = "productSearchUrl";
        public const string SEARCH_ROUTE_VALUE = ROUTE_PREFIX + "/search/full-text";
        public const string SEARCH_SUGGESTIONS_ROUTE_NAME = "searchSuggestionsUrl";
        public const string SEARCH_SUGGESTIONS_ROUTE_VALUE = ROUTE_PREFIX + "/search/suggestions";
        public const string UPDATE_LINE_ITEM_ROUTE_NAME = "updateLineItemUrl";
        public const string UPDATE_LINE_ITEM_ROUTE_VALUE = ROUTE_PREFIX + "/basket/line-item";
    }
}
