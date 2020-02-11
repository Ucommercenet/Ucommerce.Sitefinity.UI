import { initializeComponent } from "../functions/init";
import checkoutNavigation from "../components/checkout-navigation";
import store from '../store';

import { mapState } from 'vuex';

initializeComponent("checkout-overview", initCart);

function initCart(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        store,
        data: {
            model: null
        },
        computed: {
            ...mapState([
                'updateIteration',
                'allowNavigate'
            ]),
        },
        watch: {
            updateIteration: function () {
                this.fetchData();
            }
        },
        components: {
            checkoutNavigation
        },
        methods: {
            fetchData: function () {
                this.$http.get('/uc/checkout/preview', {}).then((response) => {
                    if (response.data) {
                        this.model = response.data.Data ? response.data.Data.data : null;
                    }
                });
            },
            submit: function (callback) {
                if (this.$store.state.widgets.length) {
                    this.$store.commit('triggersubmit');
                    callback(false);
                }
                else {
                    callback(true);
                }
            }
        },
        created: function () {
            this.fetchData();
        }
    });
}

