import { initializeComponent } from "../functions/init";
import checkoutNavigation from "../components/checkout-navigation";

initializeComponent("checkout-overview", initCart);

function initCart(rootElement) {
    const scriptElement = rootElement.querySelector('script[data-items]');
    const data = scriptElement === null ? [] : JSON.parse(scriptElement.innerHTML).model;

    new Vue({
        el: '#' + rootElement.id,
        data: {
            model: null
        },
        components: {
            checkoutNavigation
        },
        created: function () {
            this.fetchModel();
        },
        methods: {
            fetchModel: function () {
                var me = this;

                //dummy request
                setTimeout(function () {
                    me.model = data;
                }, 500);
            }
        }
    });
}

