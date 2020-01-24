import { initializeComponent } from "../functions/init";
import checkoutNavigation from "../components/checkout-navigation";

initializeComponent("shipping-widget", initCart);

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
        }
    });
}

