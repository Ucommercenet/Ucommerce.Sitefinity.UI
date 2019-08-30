import { initializeComponent } from "../functions/init";
import dropdownMenu from "../components/dropdown-menu";
import searchQuantum from '../components/search-quantum';
import searchBootstrap from '../components/search-bootstrap';
import searchVanilla from '../components/search-vanilla';

initializeComponent("category-navigation", initCategories);

function initCategories(rootElement) {
    const scriptElement = rootElement.querySelector('script[data-items]');
    const items = scriptElement === null ? [] : JSON.parse(scriptElement.innerHTML).items;
    new Vue({
        el: '#' + rootElement.id,
        props: {
            showSearchBar: {
                type: Boolean,
                default: false
            },
            showFilter: {
                type: Boolean,
                default: false
            }
        },
        data: {
            items: items,
            numberOfItemsInBasket: 5
        },
        components: {
            dropdownMenu,
            searchQuantum,
            searchBootstrap,
            searchVanilla
        },
        methods: {
            toggleSearchBar: function () {

                this.showSearchBar = !this.showSearchBar;
            },
            toggleFilter: function () {

                this.showFilter = !this.showFilter;
            },
            closeSearch: function () {
                setTimeout(() => {
                    if (this.showSearchBar)
                        this.toggleSearchBar();

                    if (this.showFilter)
                        this.toggleFilter();
                });
            }
        }
    });
}