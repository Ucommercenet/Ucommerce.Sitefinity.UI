define('jsQuantityPicker', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-quantity-picker";

    /** START OF PUBLIC API **/

    var jsQuantityPicker = {};

    jsQuantityPicker.init = function () {
        config.$rootSelector.find(classSelector)
            .change(function () {
                var $picker = $(this);
                var productSku = $picker.data("product-sku");
                var productQuantity = $picker.val();

                config.$triggerEventSelector.trigger("product-quantity-changed",
                {
                    productSku: productSku,
                    productQuantity: productQuantity
                });
            });
    };

    /** END OF PUBLIC API **/

    return jsQuantityPicker;
});