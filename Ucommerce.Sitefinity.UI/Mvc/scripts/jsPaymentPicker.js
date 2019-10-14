define('jsPaymentPicker', ["jquery", "jsConfig"], function ($, config) {
    "use strict";

    // declared with `var`, must be "private"
    var classSelector = ".js-payment-picker";

    function tiggerPaymentMethodChanged(paymentMethodId) {
        config.$triggerEventSelector.trigger("payment-method-changed", {
            paymentMethodId: paymentMethodId
        });
    }

    function initCompleted() {
        var paymentMethodId = config.$rootSelector.find(classSelector + ":checked").val();
        tiggerPaymentMethodChanged(paymentMethodId);
    }

    /** START OF PUBLIC API **/

    var jsPaymentPicker = {};

    jsPaymentPicker.init = function () {
        config.$rootSelector.find(classSelector).each(function() {
            config.$triggerEventSelector.on("init-completed", { $element: $(this) }, initCompleted);
        });
        if (config.$rootSelector.find(classSelector + ":checked").length == 0) {
            config.$rootSelector.find(classSelector).first().prop('checked', true);
        };
        config.$rootSelector.find(classSelector).on("change", (function () {
			    var paymentMethodId = $(this).val();
			    tiggerPaymentMethodChanged(paymentMethodId);
			}));
    };

    /** END OF PUBLIC API **/

    return jsPaymentPicker;
});