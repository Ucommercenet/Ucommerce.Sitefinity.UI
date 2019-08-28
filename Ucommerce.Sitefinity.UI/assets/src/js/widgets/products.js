import { initializeComponent } from "../functions/init";
import { setTimeout } from "timers";

initializeComponent("products", initProducts);

function initProducts(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        data: {
            addToBasketUrl: '/Basket/AddToBasketMock',
            addToBasketMessage: '',
            showAddToBasketMessage: false
        },
        methods: {
            addProductToBasket: function (sku) {

                var productQuantity = 1;
                var quantityInputValue = this.$refs[sku].value;
                if (quantityInputValue !== '' && !isNaN(quantityInputValue)) {
                    productQuantity = parseInt(quantityInputValue);
                }

                var addToBasketModel = { Quantity: productQuantity, Sku: sku, VariantSku: null };

                this.addToBasket(addToBasketModel);
            },
            addVariantToBasket: function (sku, variantSku) {

                var quantityInputValue = this.$refs[variantSku].value;
                if (quantityInputValue !== '' && !isNaN(quantityInputValue)) {
                    productQuantity = parseInt(quantityInputValue);
                }

                var addToBasketModel = { Quantity: productQuantity, Sku: sku, VariantSku: variantSku };

                this.addToBasket(addToBasketModel);
            },
            addToBasket: function (addToBasketModel) {

                this.$http.post(this.addToBasketUrl, addToBasketModel)
                    .then(function (response) {
                        this.addToBasketMessage = "Added to basket";
                        this.showAddToBasketMessage = true;

                        setTimeout(() =>
                            this.showAddToBasketMessage = false,
                            5000);
                    }, function (error) {
                        this.addToBasketMessage = "Not added to basket";
                        this.showAddToBasketMessage = true;

                        setTimeout(() =>
                            this.showAddToBasketMessage = false,
                            5000);
                    });
            }
        }
    });
}




