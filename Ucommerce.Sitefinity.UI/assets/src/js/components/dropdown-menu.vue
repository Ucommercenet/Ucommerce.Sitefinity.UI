<template>
    <ul class="navbar-nav mr-auto">
        <li v-for="node in nodes"
            v-bind:key="node.DisplayName"
            class="nav-item"
            v-bind:class="{'dropdown mr-4': node.Categories.length > 0}">
            <a v-if="node.Categories.length > 0"
               :href="node.Url"
               :id="node.DisplayName"
               class="nav-link float-left"
               v-on:click.stop>
                {{node.DisplayName}}
                <span class="sr-only">(current)</span>
            </a>
            <a v-else
               class="nav-link dropdown-item"
               :href="node.Url"
               v-on:click.stop>
                {{node.DisplayName}}
                <span v-if="node.IsOpen" class="sr-only">(current)</span>
            </a>
            <button v-if="node.Categories.length > 0"
                    data-toggle="dropdown" 
                    aria-haspopup="true" 
                    aria-expanded="false" 
                    class="btn btn-outline-dark shadow-none w-50 dropdown-toggle position-absolute p-0 float-left h-100 border-0"
                    v-on:click.stop.prevent="nodeClicked(node)"></button>
            <dropdown-submenu :parent-node="node"></dropdown-submenu>
        </li>
    </ul>
</template>

<script>
    import dropdownSubmenu from "./dropdown-submenu";
    export default {
        name: "dropdownMenu",
        data: {
            selectedNode: null
        },
        props: {
            nodes: {
                type: Array,
                default: []
            }
        },
        created: function () {
            for (var node = 0; node < this.nodes.length; node++) {
                for (var property in this.nodes[node]) {
                    var isOpenProp = "IsOpen";
                    
                    if (!this.nodes[node].hasOwnProperty(isOpenProp)) {
                        this.nodes[node][isOpenProp] = false;
                    }
                }
            }
        },
        methods: {
            nodeClicked: function (node) {

                node.IsOpen = !node.IsOpen;

                if (typeof this.selectedNode !== "undefined" && this.selectedNode !== node) {
                    this.selectedNode.IsOpen = false;
                }

                if (typeof this.selectedNode !== "undefined") {
                    this.selectedNode.Categories = this.selectedNode.Categories.map(c => {
                        if (c.IsOpen == true)
                            c.IsOpen = false;
                        return c;
                    });
                }

                this.selectedNode = node;
            }
        },
        components: {
            dropdownSubmenu
        }
    };
</script>