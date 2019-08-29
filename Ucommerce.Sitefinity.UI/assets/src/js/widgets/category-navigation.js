import { initializeComponent } from "../functions/init";
import dropdownMenu from "../components/dropdown-menu";

initializeComponent("category-navigation", initCategories);

function initCategories(rootElement) {
    const scriptElement = rootElement.querySelector('script[data-items]');
    const items = scriptElement === null ? [] : JSON.parse(scriptElement.innerHTML).items;
    new Vue({
        el: '#' + rootElement.id,
        data: {
            items: items,
            numberOfItemsInBasket: 5
        },
        components: {
            dropdownMenu
        }
    });
}