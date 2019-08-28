import { initializeComponent } from "../functions/init";
import Basket from '../components/Basket';

initializeComponent("products", initProducts);

function initProducts(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        components: {
            Basket
        }
    });
}




