export default {
    data() {
        return {
            searchQuery: null,
            suggestions: [],
            searchResult: []
        }
    },
    methods: {
        search: function () {

            this.$http.post('/SearchApi/FullText', { SearchQuery: this.searchQuery })
                .then(function (response) {
                    if (response.data)
                        this.searchResult = response.data;
                });

            this.$http.post('/SearchApi/Suggestions', { SearchQuery: this.searchQuery })
                .then(function (response) {
                    if (response.data)
                        this.suggestions = response.data;
                });
        },
        searchPageHref: function (searchPageUrl) {

            return searchPageUrl + '?search=' + this.searchQuery;
        }
    }
}