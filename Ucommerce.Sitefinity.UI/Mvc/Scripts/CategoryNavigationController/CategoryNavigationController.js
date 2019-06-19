function categoryNavigationController($scope, BasketService) {
    $scope.basketService = BasketService;

}

angular.module('app').controller("categoryNavigationController", categoryNavigationController);