<template>
    <ul class="category-child closed">
        <li class="">
            <a style="" :href="node.Url">
                {{node.DisplayName}}
            </a>
            <dropdown-node v-if="node.Categories.length > 0" v-for="childNode in node.Categories" v-bind:key="childNode.DisplayName" :node="childNode"></dropdown-node>
        </li>
    </ul>
</template>

<script>
    export default {
        name: "dropdownNode",
        props: {
            node: Object,
            isOpen: {
                type: Boolean,
                default: false
            }
        },
        methods: {

        }
    };
</script>