import { initializeComponent } from "../functions/init";
import showRating from "../components/show-rating";

initializeComponent("review-list", initReviewList);

function initReviewList(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        data: {
            Reviews: null
        },
        components: {
            showRating
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
            },
            formatDate: function (dateField) {
                if (!dateField) {
                    return;
                }

                var dateLabel = '';
                var match = dateField.match(/Date\((.*)\)/);

                if (match && match.length) {
                    dateLabel = moment(match[1], 'x').format('MMM D, YYYY');
                }

                return dateLabel;
            }
        },
        created: function () {
            console.log(this);
            this.fetchData();
        }
    });
}

