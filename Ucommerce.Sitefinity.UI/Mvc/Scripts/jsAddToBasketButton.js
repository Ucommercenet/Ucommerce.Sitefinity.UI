define('jsAddToBasketButton', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-add-to-basket-button";
    var confirmationMessageTimer;

    var showConfirmationMessage = function ($button) {

        var confirmationMessageId = $button.data("confirmation-message-id");
        var $message = $("#" + confirmationMessageId);
        var confirmationMessageTimeoutInMillisecs = $message.data("confirmation-message-timeout-in-millisecs");
        
        $message.slideDown();

        clearTimeout(confirmationMessageTimer);

        confirmationMessageTimer = setTimeout(function () {
            $message.slideUp();
        }, confirmationMessageTimeoutInMillisecs);
    };

    var toogleButton = function ($button) {
        var productVariantSku = $button.data("product-variant-sku");
        var productQuantity = $button.data("product-quantity");
        var isProductFamily = $button.data("is-product-family");
        var disableButton = (isProductFamily === 'True' && !productVariantSku) || productQuantity <= 0;
        $button.prop("disabled", disableButton);
    };

    var productVariantChanged = function (event, data) {
        var $button = event.data.$element;

        var productSku = $button.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        $button.data("product-variant-sku", data.productVariantSku);

        toogleButton($button);
    };

    var productQuantityChanged = function (event, data) {
        var $button = event.data.$element;

        var productSku = $button.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        $button.data("product-quantity", data.productQuantity);

        toogleButton($button);
    };

    /** START OF PUBLIC API **/

    var jsAddToBasketButton = {};

    jsAddToBasketButton.init = function () {
        config.$rootSelector.find(classSelector).each(function() {
            config.$triggerEventSelector.on("product-variant-changed", { $element: $(this) }, productVariantChanged);
            config.$triggerEventSelector.on("product-quantity-changed", { $element: $(this) }, productQuantityChanged);
        });
        config.$rootSelector.find(classSelector)
                .click(function () {
                    var $button = $(this);

                    var productSku = $button.data("product-sku");
                    var addToBasketUrl = $button.data("add-to-basket-url");
                    var productVariantSku = $button.data("product-variant-sku");
                    var productQuantity = $button.data("product-quantity");

                    $.ajax({
                        type: "POST",
                        url: addToBasketUrl,
                        data:
                        {
                            Quantity: productQuantity,
                            ProductSku: productSku,
                            VariantSku: productVariantSku
                        },
                        dataType: "json",
                        success: function (data) {
                            config.$triggerEventSelector.trigger("basket-changed", data);

                            showConfirmationMessage($button);
                        }
                    });
                });
    };

    /** END OF PUBLIC API **/

    return jsAddToBasketButton;
});