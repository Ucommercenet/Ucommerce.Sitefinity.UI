<template>
    <div class="search">
        <button class="search__close" v-on:click="$emit('close-search')"><i class="fa fa-times"></i></button>

        <div class="search__stage">

            <form v-if="searchPageUrl !== ''" v-bind:action="searchPageUrl" class="search__form" _lpchecked="1">
                <i class="fa fa-search"></i>
                <input type="text" autocomplete="off" v-model="searchQuery" focus v-on:input="search(rootId)" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="search__input" placeholder="Search for products">
            </form>

            <form v-if="searchPageUrl === ''" v-on:submit.prevent class="search__form" _lpchecked="1">
                <i class="fa fa-search"></i>
                <input type="text" autocomplete="off" v-model="searchQuery" autofocus v-on:input="search(rootId)" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="search__input" placeholder="Search for products">
            </form>

            <div class="search__autocomplete">
                <ul class="search__suggest" v-show="searchResult.length !== 0 || suggestions.length !== 0">
                    <li v-for="suggestion in suggestions" v-on:click="searchQuery = suggestion;search(rootId)">
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

                <a v-show="searchPageUrl !== ''" v-bind:href="searchPageHref(searchPageUrl)" class="button button--block">Show all results</a>

            </div>
        </div>

    </div>
</template>

<script>
    import search from "../mixins/search";

    export default {
        name: "searchQuantum",
        mixins: [search],
        props: {
            searchPageUrl: {
                type: String,
                default: null
            },
            rootId: String
        }
    };
</script>