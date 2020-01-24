import { initializeComponent } from "../functions/init";
import checkoutNavigation from "../components/checkout-navigation";
import inputField from "../components/input-field";

initializeComponent("address-widget", initCart);

function initCart(rootElement) {
    const scriptElement = rootElement.querySelector('script[data-items]');
    const model = scriptElement === null ? [] : JSON.parse(scriptElement.innerHTML).model;

    new Vue({
        el: '#' + rootElement.id,
        data: {
            model
        },
        components: {
            checkoutNavigation,
            inputField
        },
        methods: {
            checkForm: function (e) {
                var valid = true;
                var form = this.$refs.form;
                var errors = form.querySelectorAll("span.field-validation-error");

                if (errors.length) {
                    for (var i = 0; i < errors.length; i++) {
                        if (errors[i].innerHTML) {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid) {
                    e.preventDefault();
                }
            }
        }
    });
}

