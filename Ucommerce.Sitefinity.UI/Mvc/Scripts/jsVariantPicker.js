define('jsVariantPicker', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';
    // declared with `var`, must be "private"
    var classSelector = ".js-variant-picker";

    var getVariantNameValueDictionary = function (currentProductSku) {
        var data = {};

        $(classSelector)
            .each(function () {
                var $picker = $(this);
                var productSku = $picker.data("product-sku");

                if (currentProductSku != productSku) {
                    return;
                }

                var dataVariantName = $picker.data("variant-name");
                var variantValue = $picker.val();

                data[dataVariantName] = variantValue;
            });

        return data;
    };

    var productVariantChanged = function (event, data) {
        var $picker = $(classSelector);

        var productSku = $picker.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        //TODO: disable options that are no longer possible -- server side get possible options
    };

    var updateProperties = function (event, data) {
        var $picker = $(classSelector);

        // var currentPropertyName = $picker.data("variant-name");
        var currentPropertyName = data.sourceProperty;
        var currentPropertyAvailableValues = [];

        for (var i = 0; i < data.properties.length; i++) {
            if (data.properties[i].PropertyName != currentPropertyName) {
                // currentPropertyAvailableValues = data.properties[i].Values;
                currentPropertyAvailableValues = currentPropertyAvailableValues.concat(data.properties[i].Values);
            }
        }

        $picker.find('option.dropdown-item').each(function (index, element) {
            $(element).prop('disabled', false);
            $(element).removeClass('disabled');
            // for(var i = 0; i<currentPropertyAvailableValues.length; i++){
            var found = $.inArray($(element).val(), currentPropertyAvailableValues) > -1;
            if (!found && element.dataset.variantName != currentPropertyName) {
                $(element).prop('disabled', true);
                $(element).addClass('disabled');
                // }
            }
        });
    }

    var jsVariantPicker = {
        init: function () {
            config.$triggerEventSelector.on("product-variant-changed", { $element: $(this) }, productVariantChanged);
            config.$triggerEventSelector.on("update-properties", { $element: $(this) }, updateProperties);
            // config.$triggerEventSelector.find(classSelector).each(function () {
            //     config.$triggerEventSelector.on("product-variant-changed", { $element: $(this) }, productVariantChanged);
            //     config.$triggerEventSelector.on("update-properties", { $element: $(this) }, updateProperties);
            // });
            config.$rootSelector.find(classSelector)
                .change(function () {
                    var $picker = $(this);
                    var productSku = $picker.data("product-sku");
                    var variantExistsUrl = $picker.data("variant-exists-url");

                    var variantNameValueDictionary = getVariantNameValueDictionary(productSku);

                    $.ajax({
                        type: "POST",
                        url: variantExistsUrl,
                        data:
                        {
                            ProductSku: productSku,
                            VariantNameValueDictionary: variantNameValueDictionary
                        },
                        dataType: "json",
                        success: function (data) {
                            var productVariantSku = data.ProductVariantSku;

                            config.$triggerEventSelector.trigger("product-variant-changed",
                            {
                                productSku: productSku,
                                productVariantSku: productVariantSku
                            });
                        }
                    });

                    $.ajax({
                        type: "POST",
                        url: $picker.data("get-available-combinations-url"),
                        data: {
                            ProductSku: productSku,
                            VariantNameValueDictionary: variantNameValueDictionary
                        },
                        dataType: "json",
                        success: function (data) {
                            config.$triggerEventSelector.trigger("update-properties", {
                                properties: data.properties,
                                sourceProperty: $picker.data("variant-name")
                            });
                        }
                    });
                });
        }
    };

    return jsVariantPicker;

});