<script>
    export default {
        name: "dropdownSubmenu",
        template: '#dropdown-submenu-template',
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
                
                return this.parentNode.IsOpen;
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
        }
    };
</script>