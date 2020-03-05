<script>
    const addToBasketSuccessMessage = 'Added to basket';
    const addToBasketFailedMessage = 'Not added to basket';

    export default {
        name: "addToBasket",
        template: '#add-to-basket-template',
        props: {
            productSku: {
                type: String,
                default: null
            },
            selectedVariants: {
                type: Object,
                default: null
            },
            addToBasketMessage: String,
            showAddToBasketMessage: Boolean,
            quantity: {
                type: Number,
                default: 1
            },
            rootId: String
        },
        methods: {
            addToBasket: function () {

                var routesSelector = '#' + this.rootId + ' .addToBasketUrl';
                var addToBasketUrlContainers = document.querySelectorAll(routesSelector);
                if (addToBasketUrlContainers && addToBasketUrlContainers.length > 0) {
                    var addToBasketUrl = '/' + addToBasketUrlContainers[0].value;

                    var productQuantity = 1;
                    if (this.quantity !== '' && !isNaN(this.quantity)) {
                        productQuantity = parseInt(this.quantity);
                    }

                    var variants = [];

                    if (this.selectedVariants) {
                        for (var i = 0; i < Object.keys(this.selectedVariants).length; i++) {
                            variants.push(this.selectedVariants[Object.keys(this.selectedVariants)[i]]);
                        }
                    }

                    var addToBasketModel = { Quantity: productQuantity, Sku: this.productSku, Variants: variants };

                    this.$http.post(addToBasketUrl, addToBasketModel)
                        .then(function (response) {
                            this.addToBasketMessage = addToBasketSuccessMessage;
                            this.showAddToBasketMessage = true;

							var isEmpty = !(response.data.NumberOfItemsInBasket > 0);
                            var miniBasketRefresh = { NumberOfItems: response.data.NumberOfItemsInBasket, Total: response.data.OrderTotal, IsEmpty: isEmpty };

							$(document).find('.js-mini-basket').each(function () {

                                var $miniBasket = $(this);

                                var emptySelector = $miniBasket.data("mini-basket-empty-selector");
                                var notEmptySelector = $miniBasket.data("mini-basket-not-empty-selector");
                                var numberOfItemsSelector = $miniBasket.data("mini-basket-number-of-items-selector");
                                var totalSelector = $miniBasket.data("mini-basket-total-selector");

                                if (miniBasketRefresh) {
                                    if (miniBasketRefresh.IsEmpty) {
                                        $miniBasket.find(notEmptySelector).hide();
                                        $miniBasket.find(emptySelector).show();

                                    } else {
                                        $miniBasket.find(numberOfItemsSelector).text(miniBasketRefresh.NumberOfItems);
                                        $miniBasket.find(totalSelector).text(miniBasketRefresh.Total);

                                        $miniBasket.find(notEmptySelector).show();
                                        $miniBasket.find(emptySelector).hide();
                                    }
                                }
                            });

							setTimeout(() =>
                                this.showAddToBasketMessage = false,
                                5000);
                        }, function (error) {
                            this.addToBasketMessage = addToBasketFailedMessage;
                            this.showAddToBasketMessage = true;

                            setTimeout(() =>
                                this.showAddToBasketMessage = false,
                                5000);
                        });
                }                
            }
        }
    };
</script>