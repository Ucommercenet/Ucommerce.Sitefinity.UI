<template>
    <ul class="navbar-nav mr-auto" v-bind:class="{'closed': isParentOpen == false && isRoot == false, 'category-parent': isRoot, 'category-child': isRoot == false }">
        <li class="nav-item ml-4 mr-4"
            v-for="node in nodes"
            v-bind:key="node.DisplayName"
            v-bind:class="{'active': node.IsActive}"
            v-on:click.stop="nodeClicked(node)">
            <a :href="node.Url" class="nav-link h4">
                {{node.DisplayName}}
                <i v-if="isRoot && node.Categories.length > 0" class="glyphicon glyphicon-menu-down"></i>
                <i v-if="!isRoot && node.Categories.length > 0" class="glyphicon glyphicon-menu-right"></i>
            </a>

            <dropdown-menu v-if="node.Categories.length > 0"
                           :is-parent-open="node.IsActive"
                           :nodes="node.Categories"></dropdown-menu>
        </li>
    </ul>
</template>

<script>
    export default {
        name: "dropdownMenu",
        data: {
            selectedNode: null
        },
        props: {
            nodes: {
                type: Array,
                default: []
            },
            isParentOpen: {
                type: Boolean,
                default: false
            },
            isRoot: {
                type: Boolean,
                default: false
            }
        },
        methods: {
            nodeClicked: function (node) {
                node.IsActive = !node.IsActive;

                if (typeof this.selectedNode !== "undefined" && this.selectedNode !== node) {
                    this.selectedNode.IsActive = false;
                }

                if (typeof this.selectedNode !== "undefined") {
                    this.selectedNode.Categories = this.selectedNode.Categories.map(c => {
                        if (c.IsActive == true)
                            c.IsActive = false;

                        return c;
                    });
                }

                this.selectedNode = node;
            }
        }
    };
</script>