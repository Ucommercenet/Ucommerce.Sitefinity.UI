import { initializeComponent } from "../functions/init";

initializeComponent("review-list", initReviewList);

function initReviewList(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        data: {
            Reviews: null
        },
        methods: {
            fetchData: function () {
                this.$http.get(location.href + '/review', {}).then((response) => {
                    if (response.data &&
                        response.data.Status &&
                        response.data.Status == 'success' &&
                        response.data.Data &&
                        response.data.Data.data && 
                        response.data.Data.data.Reviews) {

                        this.Reviews = response.data.Data.data.Reviews;
                    }
                    else {
                        this.Reviews = null;
                    }
                });
            }
        },
        created: function () {
            this.fetchData();
        }
    });
}




