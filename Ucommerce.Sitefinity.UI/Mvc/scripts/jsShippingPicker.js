define("jsShippingPicker", ["jquery", "jsConfig"], function ($, config) {
    "use strict";

    // declared with `var`, must be "private"
    var classSelector = ".js-mini-basket";

    function tiggerShippingMethodChanged(shippingMethodId) {
        config.$triggerEventSelector.trigger("shipping-method-changed", {
            shippingMethodId: shippingMethodId
        });
    }

    function initCompleted() {
        var shippingMethodId = config.$rootSelector.find(classSelector + ":checked").val();

        tiggerShippingMethodChanged(shippingMethodId);
    }

    /** START OF PUBLIC API **/

    var jsShippingPicker = {};

    jsShippingPicker.init = function () {
        config.$rootSelector.find(classSelector).each(function() {
            config.$triggerEventSelector.on("init-completed", { $element: $(this) }, initCompleted());
        });
        config.$rootSelector.find(classSelector).on("change", (function () {
			    var shippingMethodId = $(this).val();
			    tiggerShippingMethodChanged(shippingMethodId);
            }));
    };

    //Make a global event init complete, when all components have been loaded
    jsShippingPicker.initCompleted = function () {
       
    };

    /** END OF PUBLIC API **/

    return jsShippingPicker;
});