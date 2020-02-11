import { initializeComponent } from "../functions/init";
import checkoutNavigation from "../components/checkout-navigation";

initializeComponent("cart", initCart);

function initCart(rootElement) {
    const scriptElement = rootElement.querySelector('script[data-items]');
    const model = scriptElement === null ? [] : JSON.parse(scriptElement.innerHTML).model;

    new Vue({
        el: '#' + rootElement.id,
        data: {
            model
        },
        components: {
            checkoutNavigation
        },
        methods: {
            updateCartItems: function () {
                var model = this.model;

                var orderlineArray = [];
                var overallQty = 0;

                for (var item of model.OrderLines) {
                    var orderlineId = item.OrderLineId;
                    var orderlineQty = item.Quantity;

                    overallQty += orderlineQty;

                    if (orderlineQty == 0) {
                        this.removeCartItem(orderlineId);
                    }
                    else {
                        var currentKeyValue = { orderlineId, orderlineQty };
                        orderlineArray.push(currentKeyValue);
                    }
                }

                if (overallQty == 0) {
                    location.reload();
                }
                else {
                    this.$http.post(model.RefreshUrl,
                        {
                            RefreshBasket: orderlineArray
                        }).then(function (response) {
                            if (response.data) {
                                var data = response.data;
                                // TODO: Set mapping fields in ViewModel
                                var updatedFields = ['SubTotal', 'TaxTotal', 'DiscountTotal', 'OrderTotal']

                                for (var field of updatedFields) {
                                    model[field] = data[field];
                                }

                                var updatedListFields = ['Total'];

                                for (var currentItem of model.OrderLines) {
                                    for (var updatedItem of data.OrderLines) {
                                        if (currentItem.OrderLineId == updatedItem.OrderlineId) {
                                            for (var field of updatedListFields) {
                                                currentItem[field] = updatedItem[field];
                                            }
                                        }
                                    }
                                }
                            }
                        });
                }

                // TODO: Investigate
                // config.$triggerEventSelector.trigger("basket-changed", data.MiniBasketRefresh);
            },
            removeCartItem: function (itemId) {
                var model = this.model;

                this.$http.post(model.RemoveOrderlineUrl,
                    {
                        orderlineId: itemId
                    }).then(function (response) {
                        if (response.data) {
                            var data = response.data;

                            if (data.OrderLines.length == 0) {
                                location.reload();
                            }
                            else {
                                // TODO: Set mapping fields in ViewModel
                                var updatedFields = ['SubTotal', 'TaxTotal', 'DiscountTotal', 'OrderTotal']

                                for (var field of updatedFields) {
                                    model[field] = data[field];
                                }

                                var updatedItems = [];

                                // generate new set without the removed item
                                for (var item of model.OrderLines) {
                                    if (item.OrderLineId != itemId) {
                                        updatedItems.push(item);
                                    }
                                }

                                model.OrderLines = updatedItems;
                            }
                        }
                    });
            }
        }
    });
}




