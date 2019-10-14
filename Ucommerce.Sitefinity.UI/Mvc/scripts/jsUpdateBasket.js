define('jsUpdateBasket', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with 'var', must be "private"
    var classSelector = '.js-update-basket';

    /** START OF PUBLIC API **/

    var jsUpdateBasket = {};

    jsUpdateBasket.init = function () {
        var removeButtonSelector = $(classSelector).data('line-remove');

        $(removeButtonSelector).each(function (index, element) {
            $(this).click(function () {
                var removeOrderlineUrl = config.$rootSelector.find(classSelector).data('remove-orderline-url');
                var orderlineId = $(this).data('line-id');

                $.ajax({
                    type: 'POST',
                    url: removeOrderlineUrl,
                    data: {
                        orderlineId: orderlineId
                    },
                    dataType: 'json',
                    success: function (data) {
                        $('[data-orderline]').each(function (index, element) {
                            if (element.dataset.orderline == data.orderlineId) {
                                $(element).remove();
                            }
                        });
                        if ($('[data-orderline]').length == 0) {
                            $('body').load(location.href);
                        }

                        config.$triggerEventSelector.trigger("basket-changed", data.MiniBasketRefresh);
                    }
                });

            });
        });

        config.$rootSelector.find(classSelector).click(function () {
            var $updateBasket = $(this);
            var refreshUrl = $updateBasket.data('refresh-url');
            var orderlines = $('[data-orderline-id]');
            var itemTotalSelector = $updateBasket.data('item-total');


            var orderlineArray = [];

            orderlines.each(function (index, element) {
                var orderlineId = element.dataset.orderlineId;
                var orderlineQty = element.value;
                var currentKeyValue = { orderlineId, orderlineQty }

                orderlineArray.push(currentKeyValue);
            });

            $.ajax({
                type: 'POST',
                url: refreshUrl,
                data: {
                    RefreshBasket: orderlineArray
                },
                dataType: 'json',
                success: function (data) {
                    var orderlines = data.OrderLines;
                    var orderlineIds = [];
                    $('[data-orderline]').each(function (index, element) {
                        var currentLine;
                        for (var i = 0; i < orderlines.length; i++) {
                            orderlineIds.push(orderlines[i].OrderlineId.toString());
                            if (element.dataset.orderline == orderlines[i].OrderlineId) {
                                currentLine = orderlines[i];
                            };
                        };
                        if ($.inArray(element.dataset.orderline, orderlineIds) >= 0) {
                            $(element).find(itemTotalSelector)[0].innerHTML = currentLine.Total;
                        } else {
                            $(element).remove();
                        }
                    });

                    if ($('[data-orderline]').length == 0) {
                        $('body').load(location.href);
                    }

                    var orderSubtotal = $updateBasket.data('order-subtotal');
                    $(orderSubtotal).text(data.SubTotal);
                    var taxTotal = $updateBasket.data('order-tax');
                    $(taxTotal).text(data.TaxTotal);
                    var discountTotal = $updateBasket.data('order-discounts');
                    $(discountTotal).text(data.DiscountTotal);
                    var orderTotal = $updateBasket.data('order-total');
                    $(orderTotal).text(data.OrderTotal);

                    config.$triggerEventSelector.trigger("basket-changed", data.MiniBasketRefresh);
                },
                error: function (err) {
                    console.log("Something went wrong...");
                }
            });
        });
    }
    return jsUpdateBasket;
});