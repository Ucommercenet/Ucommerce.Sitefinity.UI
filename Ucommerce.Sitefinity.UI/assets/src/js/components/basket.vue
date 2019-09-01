<script>
    export default {
        name: "basket",
        props: {
            basket: {},
            showSidebarBasket: Boolean,
            rootId: String,
            basketLoaded: Boolean
        },
        created: function () {
            this.loadBasket(this.rootId);
        },
        methods: {
            setCurrency: function (rootId, priceGroupId) {

                var routesSelector = '#' + rootId + ' .changePriceGroupUrl';
                var routeUrlContainers = document.querySelectorAll(routesSelector);
                if (routeUrlContainers && routeUrlContainers.length > 0) {
                    var changePriceGroupUrl = '/' + routeUrlContainers[0].value;
                    this.$http.post(changePriceGroupUrl, { PriceGroupId: priceGroupId })
                        .then(function (response) {
                            console.log("I'm here");
                            location.reload();
                        });
                }
            },
            updateLineItem: function (rootId, orderLineId, newQuantity) {

                var routesSelector = '#' + rootId + ' .updateLineItemUrl';
                var routeUrlContainers = document.querySelectorAll(routesSelector);
                if (routeUrlContainers && routeUrlContainers.length > 0) {
                    var updateLineItemUrl = '/' + routeUrlContainers[0].value;
                    this.$http.post(updateLineItemUrl, { OrderlineId: orderLineId, NewQuantity: newQuantity })
                        .then(function (response) {
                            this.basket = response.data;
                        });
                }
            },
            loadBasket: function (rootId) {

                var routesSelector = '#' + rootId + ' .getBasketUrl';
                var routeUrlContainers = document.querySelectorAll(routesSelector);
                if (routeUrlContainers && routeUrlContainers.length > 0) {
                    var getBasketUrl = '/' + routeUrlContainers[0].value;

                    return this.$http.get(getBasketUrl)
                        .then(function success(response) {

                            this.basket = response.data;
                            this.basketLoaded = true;
                        }), function error(err) {

                            console.log(err);
                        };
                }
            },
            addToBasket: function (rootId, addToBasketModel) {

                var routesSelector = '#' + rootId + ' .addToBasketUrl';
                var routeUrlContainers = document.querySelectorAll(routesSelector);
                if (routeUrlContainers && routeUrlContainers.length > 0) {
                    var addToBasketUrl = '/' + routeUrlContainers[0].value;
                    this.$http.post(addToBasketUrl, addToBasketModel)
                        .then(function (response) {
                            this.basket = response.data;
                            this.toggleSideBarBasket();
                        });
                }
            },
            applyVoucher: function (rootId, voucherCode) {

                var routesSelector = '#' + rootId + ' .addToBasketUrl';
                var routeUrlContainers = document.querySelectorAll(routesSelector);
                if (routeUrlContainers && routeUrlContainers.length > 0) {
                    var addVoucherUrl = '/' + routeUrlContainers[0].value;
                    this.$http.post(addVoucherUrl, { VoucherCode: voucherCode })
                        .then(function (response) {
                            this.basket = response.data;
                        });
                }
            },
            toggleSideBarBasket: function () {
                this.showSidebarBasket = !this.showSidebarBasket;
            }
        }
    };
</script>