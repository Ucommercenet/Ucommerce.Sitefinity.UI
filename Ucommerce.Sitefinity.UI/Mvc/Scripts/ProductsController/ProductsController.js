function productsController($scope, BasketService) {
    $scope.basketService = BasketService;

    $scope.addToBasket = function (sku) {
        $scope.basketService.addToBasket({
            Quantity: 1,
            Sku: sku,
            VariantSku: null
        });
    };

    $scope.addVariantToBasket = function (sku, variantSku) {
        $scope.basketService.addToBasket({
            Quantity: 1,
            Sku: sku,
            VariantSku: variantSku
        });
    };
}

angular.module('app').controller("productsController", productsController);