<template>
    <ul class="dropdown-menu"
        v-bind:class="{'show': isParentOpen}"
        :aria-labelledby="parentNode.DisplayName">
        <li v-for="node in parentNode.Categories"
            v-bind:key="node.DisplayName"
            v-bind:class="{'nav-item dropdown': node.Categories.length > 0, 'dropdown-item': node.Categories === 0 }"
            v-on:click.stop.prevent="nodeClicked(node)">
            <a v-if="node.Categories.length > 0"
               class="nav-link dropdown-toggle ml-4"
               :href="node.Url"
               :id="node.DisplayName"
               data-toggle="dropdown"
               aria-haspopup="true"
               aria-expanded="false">
                {{node.DisplayName}}
                <span class="sr-only">(current)</span>
                <dropdown-submenu :parent-node="node"></dropdown-submenu>
            </a>
            <a v-else
               class="dropdown-item nav-link"
               :href="node.Url">
                {{node.DisplayName}}
                <span class="sr-only">(current)</span>
            </a>
        </li>
    </ul>
</template>

<script>
    export default {
        name: "dropdownSubmenu",
        data: {
            selectedNode: null
        },
        props: {
            parentNode: {
                type: Object,
                default: []
            }
        },
        computed: {
            isParentOpen: function () {
                return this.parentNode.IsActive;
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