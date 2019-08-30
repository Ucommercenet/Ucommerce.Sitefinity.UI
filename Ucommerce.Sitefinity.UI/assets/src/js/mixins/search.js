export default {
    data() {
        return {
            searchQuery: null,
            suggestions: [],
            searchResult: []
        }
    },
    methods: {
        search: function (rootId) {

            var searchRoutesSelector = '#' + rootId + ' .productSearchUrl';
            var searchUrlContainers = document.querySelectorAll(searchRoutesSelector);
            if (searchUrlContainers && searchUrlContainers.length > 0) {
                var searchUrl = searchUrlContainers[0].value;
                this.$http.post(searchUrl, { SearchQuery: this.searchQuery })
                    .then(function (response) {
                        if (response.data)
                            this.searchResult = response.data;
                    });
            }

            var suggestionRoutesSelector = '#' + rootId + ' .searchSuggestionsUrl';
            var suggestionsUrlContainers = document.querySelectorAll(suggestionRoutesSelector);
            if (suggestionsUrlContainers && suggestionsUrlContainers.length > 0) {
                var searchSuggestionsUrl = suggestionsUrlContainers[0].value;
                this.$http.post(searchSuggestionsUrl, { SearchQuery: this.searchQuery })
                    .then(function (response) {
                        if (response.data)
                            this.suggestions = response.data;
                    });
            }
        },
        searchPageHref: function (searchPageUrl) {

            return searchPageUrl + '?search=' + this.searchQuery;
        }
    }
}