define('jsMiniBasket', ['jquery', 'jsConfig'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-mini-basket";

    var basketChanged = function (event, data) {
        var $miniBasket = event.data.$element;

        var emptySelector = $miniBasket.data("mini-basket-empty-selector");
        var notEmptySelector = $miniBasket.data("mini-basket-not-empty-selector");
        var numberOfItemsSelector = $miniBasket.data("mini-basket-number-of-items-selector");
        var totalSelector = $miniBasket.data("mini-basket-total-selector");

        if (data) {
            if (data.IsEmpty) {
                $miniBasket.find(notEmptySelector).hide();
                $miniBasket.find(emptySelector).show();

            } else {
                $miniBasket.find(numberOfItemsSelector).text(data.NumberOfItems);
                $miniBasket.find(totalSelector).text(data.Total);

                $miniBasket.find(notEmptySelector).show();
                $miniBasket.find(emptySelector).hide();
            }
        }
    }

    /** START OF PUBLIC API **/

    var jsMiniBasket = {};

    jsMiniBasket.init = function () {
        config.$rootSelector.find(classSelector).each(function() {
            config.$triggerEventSelector.on("basket-changed", { $element: $(this) }, basketChanged);
        });
    };

    /** END OF PUBLIC API **/

    return jsMiniBasket;
});