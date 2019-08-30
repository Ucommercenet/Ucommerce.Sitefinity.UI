import { initializeComponent } from "../functions/init";
import basket from '../components/basket';

initializeComponent("products", initProducts);

function initProducts(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        components: {
            basket
        }
    });
}




