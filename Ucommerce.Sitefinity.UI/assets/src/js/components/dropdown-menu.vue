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
        },
        components: {
            dropdownSubmenu
        }
    };
</script>