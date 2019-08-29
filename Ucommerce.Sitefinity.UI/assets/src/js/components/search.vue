<template>
    <div class="search">
        <h1>JOJOJ</h1>
        <button class="search__close" v-on:click="$emit('close-search')"><i class="fa fa-times"></i></button>

        <div class="search__stage">

            <form v-if="searchPageUrl !== null" v-bind:action="searchPageUrl" class="search__form" _lpchecked="1">
                <i class="fa fa-search"></i>
                <input type="text" autocomplete="off" v-model="searchQuery" focus v-on:change="search" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="search__input" placeholder="Search for products">
            </form>

            <form v-if="searchPageUrl === null" class="search__form" _lpchecked="1">
                <i class="fa fa-search"></i>
                <input type="text" autocomplete="off" v-model="searchQuery" autofocus v-on:change="search" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="search__input" placeholder="Search for products">
            </form>

            <div class="search__autocomplete" v-show="searchResult.length !== 0 || suggestions.length !== 0">
                <ul class="search__suggest">
                    <li v-for="suggestion in suggestions" v-on:click="searchQuery = suggestion;search()">
                        <span class="search__suggest-image">
                        </span>
                        <span class="search__suggest-info">
                            <span class="search__suggest-title">{{suggestion}}</span>
                        </span>
                    </li>
                    <li v-for="product in searchResult">
                        <a v-bind:href="product.Url">
                            <span class="search__suggest-image">
                                <img v-bind:src="product.ThumbnailImageUrl" alt="">
                            </span>
                            <span class="search__suggest-info">
                                <span class="search__suggest-title">{{product.Name}}</span>
                            </span>
                        </a>
                    </li>
                </ul>

                <a v-if="searchPageUrl !== null" href="searchPageHref" class="button button--block">Show all results</a>

            </div>
        </div>

    </div>
</template>

<script>
    export default {
        name: "Search",
        props: {
            searchPageUrl: {
                type: String,
                default: null
            },
            searchQuery: String,
            suggestions: {
                type: Array,
                default: []
            },
            searchResult: {
                type: Array,
                default: []
            }
        },
        computed: {
            searchPageHref: function () {

                return this.searchPageUrl + '?search=' + this.searchQuery;
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
            }
        }
    };
</script>