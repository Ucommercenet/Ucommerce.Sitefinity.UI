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
            variantSku: {
                type: String,
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

                    var addToBasketModel = { Quantity: productQuantity, Sku: this.productSku, VariantSku: this.variantSku };

                    this.$http.post(addToBasketUrl, addToBasketModel)
                        .then(function (response) {
                            this.addToBasketMessage = addToBasketSuccessMessage;
                            this.showAddToBasketMessage = true;

                            $({}).trigger("basket-changed", data.MiniBasketRefresh);

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