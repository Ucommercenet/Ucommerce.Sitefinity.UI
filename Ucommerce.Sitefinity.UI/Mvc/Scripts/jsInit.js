requirejs.config({
    baseUrl: '/Frontend-Assembly/Ucommerce.Sitefinity.UI/Mvc/Scripts',
    paths: {
        /* Load jquery from google cdn. On fail, load local file. */
        jquery: ['//ajax.googleapis.com/ajax/libs/jquery/3.0.0/jquery.min', 'jquery-3.0.0.min'],
        popper: ['popper.min'],
        bootstrap: ['bootstrap.min'],
        jqueryValidate: ['jquery.validate.min'],
        initBootstrap: ['initBootstrap']
    },
    shim: {
        bootstrap: {
            deps: ['jquery']
        },
        jqueryValidate: {
            deps: ['jquery']
        }
    }
});

define("initBootstrap", ["popper"], function (popper) {
    // set popper as required by Bootstrap
    window.Popper = popper;
    require(["bootstrap"], function (bootstrap) {
        // do nothing - just let Bootstrap initialise itself
    });
});

define('jsConfig', ['jquery'], function ($) {
    'use strict';

    /** START OF PUBLIC API **/

    var jsConfig =
    {
        $rootSelector: function () { return $(document) }(),
        $triggerEventSelector: $({})
    };

    /** END OF PUBLIC API **/

    return jsConfig;
});

require(["jsAddress"], function (component) {
    component.init();
});

require(["jsAddToBasketButton"], function (component) {
    component.init();
});

require(["jsMiniBasket"], function (component) {
    component.init();
});

require(["jsPaymentPicker"], function (component) {
    component.init();
});

require(["jsQuantityPicker"], function (component) {
    component.init();
});

require(["jsShippingPicker"], function (component) {
    component.init();
});

//require(["jsUpdateBasket"], function (component) {
//    component.init();
//});

require(["jsVariantPicker"], function (component) {
    component.init();
});

require(["jsVoucher"], function (component) {
    component.init();
});

require(['jsConfig'], function (config) {
    config.$triggerEventSelector.trigger("init-completed", {});
});