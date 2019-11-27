(function ($) {
    angular.module('designer').controller('SimpleCtrl', ['$scope', 'propertyService', '$timeout', function ($scope, propertyService, $timeout) {

        $scope.feedback.showLoadingIndicator = true;
        
        propertyService.get()
            .then(function (data) {
                if (data) {
                    $scope.properties = propertyService.toAssociativeArray(data.Items);

                    $scope.$broadcast('setPreselectedValuesFromList', $scope.properties.ProductIds.PropertyValue, 'product');

                    $scope.$broadcast('setPreselectedValuesFromList', $scope.properties.CategoryIds.PropertyValue, 'productCategory');

                    $scope.$on('preSelectedValuesChanged',
                        function (event, data, itemsType) {
                            if (data && data[0]) {
                                if (itemsType === 'product') {
                                    $scope.properties.ProductIds.PropertyValue = data.map(function (e) { return e.id }).join(',');
                                }
                                else if (itemsType === 'productCategory') {
                                    $scope.properties.CategoryIds.PropertyValue = data.map(function (e) { return e.id }).join(',');
                                }
                            }
                            else {
                                if (itemsType === 'product') {
                                    $scope.properties.ProductIds.PropertyValue = '';
                                }
                                else if (itemsType === 'productCategory') {
                                    $scope.properties.CategoryIds.PropertyValue = '';
                                }
                            }
                        });
                }
            },
                function (data) {
                    $scope.feedback.showError = true;
                    if (data)
                        $scope.feedback.errorMessage = data.Detail;
                })
            .then(function () {
                $scope.feedback.savingHandlers.push(function () {
             
                });
            })
            .finally(function () {
                $scope.feedback.showLoadingIndicator = false;
            }); 
    }]);
})(jQuery);