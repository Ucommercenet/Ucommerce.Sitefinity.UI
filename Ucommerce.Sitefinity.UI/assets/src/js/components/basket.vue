<template>
    <div class="mt-2">
        <label for="productQuantityInput" class="font-weight-bold">Quantity:</label>
        <span id="ctl00_contentPlaceholder_C020_productsFrontendDetail_ctl00_ctl00_SingleItemContainer_ctrl0_addToCartWidget_ctl00_ctl00_quantityRequiredFieldValidator" style="display:none;">
            <span class="text-danger">
                When adding product to a cart you must specify the quantity
            </span>
        </span>
        <span id="ctl00_contentPlaceholder_C020_productsFrontendDetail_ctl00_ctl00_SingleItemContainer_ctrl0_addToCartWidget_ctl00_ctl00_quantityValidator" style="display:none;">
            <span class="text-danger">
                The quantity must be greater than 0 and less than 9,999.
            </span>
        </span>
        <input id="productQuantityInput" v-model="quantity" name="ctl00$contentPlaceholder$C020$productsFrontendDetail$ctl00$ctl00$SingleItemContainer$ctrl0$addToCartWidget$ctl00$ctl00$quantity" type="text" class="form-control" />

        <input type="submit" name="ctl00$contentPlaceholder$C020$productsFrontendDetail$ctl00$ctl00$SingleItemContainer$ctrl0$addToCartWidget$ctl00$ctl00$addToCartButton" value="Add to cart" v-on:click="addToBasket()" id="ctl00_contentPlaceholder_C020_productsFrontendDetail_ctl00_ctl00_SingleItemContainer_ctrl0_addToCartWidget_ctl00_ctl00_addToCartButton" class="btn btn-info mt-2" />
        <span v-show="showAddToBasketMessage">{{addToBasketMessage}}</span>
    </div>
</template>

<script>
    const addToBasketSuccessMessage = 'Added to basket';
    const addToBasketFailedMessage = 'Not added to basket';

    export default {
        name: "basket",
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
                    var addToBasketUrl = addToBasketUrlContainers[0].value;

                    var productQuantity = 1;
                    if (this.quantity !== '' && !isNaN(this.quantity)) {
                        productQuantity = parseInt(this.quantity);
                    }

                    var addToBasketModel = { Quantity: productQuantity, Sku: this.productSku, VariantSku: this.variantSku };

                    this.$http.post(addToBasketUrl, addToBasketModel)
                        .then(function (response) {
                            this.addToBasketMessage = addToBasketSuccessMessage;
                            this.showAddToBasketMessage = true;

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