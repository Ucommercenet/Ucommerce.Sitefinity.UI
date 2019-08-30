<template>
    <div>
        <button v-on:click="$emit('close-search')"><i>Close Search</i></button>

        <div>

            <form v-if="searchPageUrl !== ''" v-bind:action="searchPageUrl" _lpchecked="1">
                <i></i>
                <input type="text" autocomplete="off" v-model="searchQuery" focus v-on:input="search" v-on:keyup.escape="$emit('close-search')" required="" name="search" placeholder="Search for products">
            </form>

            <form v-if="searchPageUrl === ''" v-on:submit.prevent _lpchecked="1">
                <i></i>
                <input type="text" autocomplete="off" v-model="searchQuery" autofocus v-on:input="search" v-on:keyup.escape="$emit('close-search')" required="" name="search" placeholder="Search for products">
            </form>

            <div>
                <ul v-show="searchResult.length !== 0 || suggestions.length !== 0">
                    <li v-for="suggestion in suggestions" v-on:click="searchQuery = suggestion;search()">
                        <span>
                        </span>
                        <span>
                            <span>{{suggestion}}</span>
                        </span>
                    </li>
                    <li v-for="product in searchResult">
                        <a v-bind:href="product.Url">
                            <span>
                                <img v-bind:src="product.ThumbnailImageUrl" alt="">
                            </span>
                            <span>
                                <span>{{product.Name}}</span>
                            </span>
                        </a>
                    </li>
                </ul>

                <a v-show="searchPageUrl !== ''" v-bind:href="searchPageHref(searchPageUrl)">Show all results</a>

            </div>
        </div>

    </div>
</template>

<script>
    import search from "../mixins/search";

    export default {
        name: "searchVanilla",
        mixins: [search],
        props: {
            searchPageUrl: {
                type: String,
                default: null
            }
        }
    };
</script>