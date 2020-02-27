import { initializeComponent } from "../functions/init";

initializeComponent("review-form", initReviewForm);

function initReviewForm(rootElement) {
    new Vue({
        el: '#' + rootElement.id,
        data: {
            rating: null,
            comments: '',
            userName: '',
            userEmail: ''
        },
        props: [
            'star1',
            'star2',
            'star3',
            'star4',
            'star5',
        ],
        methods: {
            setRating: function (rating) {
                this.rating = rating;
            },
            submit: function () {
                var requestData = {
                    Rating: this.rating,
                    Name: this.userName,
                    Email: this.userEmail,
                    Comments: this.comments
                };

                this.$http.post(location.href + '/submit-review', requestData).then((response) => {
                    if (response.data) {
                        var data = response.data;
                        console.log(data);
                    }
                });
            }
        },
        watch: {
            rating: function (value) {
                for (var i = 1; i <= 5; i++) {
                    if (i <= value) {
                        this['star' + i] = 'star on';
                    }
                    else {
                        this['star' + i] = 'star';
                    }
                }
            }
        },
        created: function () {
            this.setRating(0);
            console.log(this);
        }
    });
}




