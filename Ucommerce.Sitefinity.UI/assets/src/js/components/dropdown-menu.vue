<script>
    import dropdownSubmenu from "./dropdown-submenu";
    export default {
        name: "dropdownMenu",
        template: '#dropdown-menu-template',
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