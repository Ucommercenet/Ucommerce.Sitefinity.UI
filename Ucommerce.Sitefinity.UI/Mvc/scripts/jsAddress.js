define('jsAddress', ['jquery', 'jsConfig', 'jquery.validate'], function ($, config) {
    'use strict';

    // declared with `var`, must be "private"
    var classSelector = ".js-address";
    var billingClassSelector = ".js-address-billing";
    var shippingClassSelector = ".js-address-shipping";
    var checkboxClassSelector = ".js-address-checkbox";
    var $form = $(classSelector);
    var submitClassSelector = $form.data('submit-address');
    var saveAddressUrl = $form.data('save-address-url');

    var toggleShippingAddress = function () {
        var value = $(this).is(":checked");
        var shippingAddress = $(classSelector).find(shippingClassSelector);

        if (value) {
            shippingAddress.show();
        }
        else {
            shippingAddress.hide();
        }
    };

    /** START OF PUBLIC API **/

    var jsAddress = {};

    jsAddress.init = function () {
        $(submitClassSelector).off().click(function (e) {
            e.preventDefault();
            $form.find('.error-custom').text('');

            $form.validate({
                errorElement: "span",
                errorClass: "error-custom",
                highlight: function (tag) {
                    $(tag).addClass('error-custom');
                },
                success: function (tag) {
                    $('tag').addClass('success');
                }
            });

            if ($form.valid()) {
                $.ajax({
                    type: 'POST',
                    url: saveAddressUrl,
                    data: $form.serialize(),
                    success: function (data) {
                        if (data.ShippingUrl) {
                            window.location.href = data.ShippingUrl;
                        }
                        if (data.modelStateErrors) {
                            var errors = data.modelStateErrors;
                            for (var i = 0; i < errors.length; i++) {
                                var currentError = errors[i];

                                $form.find('.' + currentError.Key.replace(/\./g, '')).text(currentError.Value[0]);
                            }
                        }
                    }

                });
            }

        });

        config.$rootSelector.find(checkboxClassSelector).on("change", toggleShippingAddress);
    };

    /** END OF PUBLIC API **/

    return jsAddress;
});