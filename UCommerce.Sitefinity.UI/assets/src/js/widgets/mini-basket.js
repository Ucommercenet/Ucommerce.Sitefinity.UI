import { initializeComponent } from "../functions/init";
import store from '../store';

import { mapState } from 'vuex';

initializeComponent("mini-basket", initCart);;

function initCart(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        store,
        data: {
            model: null
        },
        computed: {
            ...mapState([
                'updateIteration'
            ]),
        },
        watch: {
            updateIteration: function () {
                this.fetchData();
            }
        },
        methods: {
            fetchData: function () {
                this.$http.get(location.href + '/uc/checkout/mini-basket', {}).then((response) => {
                    if (response.data &&
                        response.data.status &&
                        response.data.status == 'success' &&
                        response.data.data && response.data.data.data) {

                        this.model = response.data.data.data;
                    }
                    else {
                        this.model = null;
                    }
                });
            }
        },
        created: function () {
            this.fetchData();
        }
    });
}

