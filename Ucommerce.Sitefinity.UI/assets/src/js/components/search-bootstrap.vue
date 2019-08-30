<template>
    <div class="row">
        <div class="col-md-offset-3 col-md-6">
            <button class="close" aria-label="Close" v-on:click="$emit('close-search')"><i aria-hidden="true" class="glyphicon glyphicon-remove p-3">Close Search</i></button>

            <div class="bg-white shadow-lg p-3 rounded">

                <form v-if="searchPageUrl !== ''" v-bind:action="searchPageUrl" class="input-group" _lpchecked="1">
                    <input type="text" autocomplete="off" v-model="searchQuery" focus v-on:input="search(rootId)" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="form-control form-control-lg py-2 border-right-0 border" placeholder="Search for products">
                    <span class="input-group-append">
                        <button class="btn btn-outline-secondary border-left-0 border" type="button">
                            <i class="glyphicon glyphicon-search"></i>
                        </button>
                    </span>
                </form>

                <form v-if="searchPageUrl === ''" v-on:submit.prevent class="input-group" _lpchecked="1">
                    <input type="text" autocomplete="off" v-model="searchQuery" autofocus v-on:input="search(rootId)" v-on:keyup.escape="$emit('close-search')" required="" name="search" class="form-control form-control-lg py-2 border-right-0 border" placeholder="Search for products">
                    <span class="input-group-append">
                        <button class="btn btn-outline-secondary border-left-0 border" type="button">
                            <i class="glyphicon glyphicon-search"></i>
                        </button>
                    </span>
                </form>

                <div>
                    <ul class="list-group" v-show="searchResult.length !== 0 || suggestions.length !== 0">
                        <li v-for="suggestion in suggestions" class="list-group-item" v-on:click="searchQuery = suggestion;search(rootId)">
                            <span class="img">
                            </span>
                            <span class="stext-info">
                                <span class="h4 p-3 ng-binding">{{suggestion}}</span>
                            </span>
                        </li>
                        <li v-for="product in searchResult" class="list-group-item">
                            <a class="nav-link" v-bind:href="product.Url">
                                <span class="img">
                                    <img height="50" width="50" v-bind:src="product.ThumbnailImageUrl" alt="">
                                </span>
                                <span class="text-info">
                                    <span class="h3 p-3">{{product.Name}}</span>
                                </span>
                            </a>
                        </li>
                    </ul>

                    <a v-show="searchPageUrl !== ''" v-bind:href="searchPageHref(searchPageUrl)" class="btn btn-block">Show all results</a>

                </div>
            </div>

        </div>
        <div class="col-md-offset-3">

        </div>
    </div>
</template>

<script>
    import search from "../mixins/search";

    export default {
        name: "searchBootstrap",
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