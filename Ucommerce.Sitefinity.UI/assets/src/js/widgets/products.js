import { initializeComponent } from "../functions/init";
import addToBasket from '../components/add-to-basket';

initializeComponent("products", initProducts);

function initProducts(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        components: {
            addToBasket
        },
        props: {
            variantSku: {
                type: String,
                default: null
            },
            productGuid: '',
            variantGuid: '',
            price: 0,
            listPrice: 0
        },
        methods: {
            variantUpdated: function () {
                var variantSelector = document.getElementById('variantSku');
                this.variantGuid = variantSelector.options[variantSelector.selectedIndex].dataset.variantGuid;

                this.getPrices();
            },
            getPrices: function () {
                this.$http.post('/ProductApi/productPrices',
                    {
                        ProductGuid: this.productGuid,
                        VariantGuid: this.variantGuid
                    }).then(function (response) {
                        if (response.data) {
                            for (var i = 0; i < response.data.length; i++) {
                                var pricePoint = response.data[i];

                                if (this.variantGuid && this.variantGuid === pricePoint.ProductGuid) {
                                    this.listPrice = pricePoint.ListPrice;
                                    this.price = pricePoint.Price;
                                    break;
                                }

                                if (this.variantGuid && this.productGuid === pricePoint.ProductGuid) {
                                    this.listPrice = pricePoint.ListPrice;
                                    this.price = pricePoint.Price;
                                }
                            }

                            var variantSelector = document.getElementById('variantSku');
                            if (variantSelector.options[0].text === "")
                                variantSelector.remove(0);
                        }
                    });
            }
        }
    });
}




