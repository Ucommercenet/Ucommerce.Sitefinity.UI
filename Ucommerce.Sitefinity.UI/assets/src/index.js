'use strict';

import Vue from 'vue';
window.Vue = Vue;

if (process.env.NODE_ENV === "development") {
    Vue.config.devtools = true;
} else {
    Vue.config.devtools = false;
}

import VueResource from 'vue-resource';
Vue.use(VueResource);

import "./js/widgets/facet-filter";
import "./js/widgets/category-navigation";
import "./js/widgets/products";
import "./js/widgets/cart";
import "./js/widgets/address";
import "./js/widgets/shipping-method";
import "./js/widgets/payment-method";
import "./js/widgets/checkout-overview";
