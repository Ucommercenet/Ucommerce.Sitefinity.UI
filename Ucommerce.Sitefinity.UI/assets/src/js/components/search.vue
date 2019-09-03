<script>
    export default {
        name: "search",
        template: '#search-template',
        props: {
            searchPageUrl: {
                type: String,
                default: null
            },
            rootId: String,
            showSearchBar: {
                type: Boolean,
                default: false
            },
            pageId: {
                type: String,
                default: null
            },
            categoryId: Number
        },
        data() {
            return {
                searchQuery: null,
                suggestions: [],
                searchResult: []
            }
        },
        methods: {
            search: function () {

                var searchRoutesSelector = '#' + this.rootId + ' .productSearchUrl';
                var searchUrlContainers = document.querySelectorAll(searchRoutesSelector);
                if (searchUrlContainers && searchUrlContainers.length > 0) {
                    var searchUrl = '/' + searchUrlContainers[0].value;

                    this.$http.post(searchUrl, { SearchQuery: this.searchQuery, PageId: this.pageId, CategoryId: this.categoryId })
                        .then(function (response) {
                            if (response.data)
                                this.searchResult = response.data;
                        });
                }

                var suggestionRoutesSelector = '#' + this.rootId + ' .searchSuggestionsUrl';
                var suggestionsUrlContainers = document.querySelectorAll(suggestionRoutesSelector);
                if (suggestionsUrlContainers && suggestionsUrlContainers.length > 0) {
                    var searchSuggestionsUrl = '/' + suggestionsUrlContainers[0].value;

                    this.$http.post(searchSuggestionsUrl, { SearchQuery: this.searchQuery, PageId: this.pageId, CategoryId: this.categoryId })
                        .then(function (response) {
                            if (response.data)
                                this.suggestions = response.data;
                        });
                }
            },
            searchPageHref: function () {

                return this.searchPageUrl + '?search=' + this.searchQuery;
            },
            toggleSearchBar: function () {
                this.showSearchBar = !this.showSearchBar;
            },
            closeSearch: function () {
                setTimeout(() => {
                    if (this.showSearchBar)
                        this.toggleSearchBar();

                    if (this.showFilter)
                        this.toggleFilter();
                });
            }
        }
    };
</script>