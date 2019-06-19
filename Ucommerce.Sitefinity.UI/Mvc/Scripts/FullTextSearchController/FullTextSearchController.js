function fullTextSearchController($scope, SearchService) {
    $scope.searchService = SearchService;
}

angular.module('app').controller("fullTextSearchController", fullTextSearchController);