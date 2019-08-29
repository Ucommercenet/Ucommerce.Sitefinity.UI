<template>
    <ul class="category-parent">
        <li v-for="node in nodes">
            <a :href="node.Url">
                {{node.DisplayName}}
                <i v-if="node.Categories.length > 0" class="fa fa-chevron-down"></i>
            </a>

            <dropdown-node v-if="node.Categories.length > 0" v-for="childNode in node.Categories" v-bind:key="childNode.DisplayName" :node="childNode"></dropdown-node>
        </li>
    </ul>
</template>

<script>
    import dropdownNode from "../components/dropdown-node";
    export default {
        name: "dropdownMenu",
        props: {
            nodes: {
                type: Array,
                default: []
            },
            isOpen: {
                type: Boolean,
                default: false
            }
        },
        components: {
            dropdownNode
        }
    };
</script>