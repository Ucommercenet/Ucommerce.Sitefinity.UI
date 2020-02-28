import { initializeComponent } from "../functions/init";
import showRating from "../components/show-rating";

initializeComponent("review-list", initReviewList);

function initReviewList(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        data: {
            Reviews: null,

        },
        computed: {
            count: function () {
                return this.Reviews.length;
            },
            displayRating: function () {
                var reviewSum = 0;
                var count = this.Reviews.length;

                if (!count) {
                    return;
                }

                for (var review of this.Reviews) {
                    reviewSum += this.getRating(review.Rating);
                }

                return (reviewSum / count).toFixed(2);
            },
            averageRating: function () {
                var reviewSum = 0;
                var count = this.Reviews.length;

                if (!count) {
                    return;
                }

                for (var review of this.Reviews) {
                    reviewSum += this.getRating(review.Rating);
                }

                return ((reviewSum / count) * 20);
            },
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
            },
            getRating: function (value) {
                if (!value) {
                    return null;
                }

                return Math.round(Math.abs(value) / 20);
            }
        },
        created: function () {
            console.log(this);
            this.fetchData();
        }
    });
}

