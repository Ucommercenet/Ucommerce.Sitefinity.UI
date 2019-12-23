(function ($) {
    angular.module('designer').requires.push('expander', 'sfSelectors');

    angular.module('designer').controller('SimpleCtrl', ['$scope', 'propertyService', function ($scope, propertyService) {

        $scope.feedback.showLoadingIndicator = true;
        var emptyGuid = '00000000-0000-0000-0000-000000000000';
        
        propertyService.get()
            .then(function (data) {
                if (data) {
                    $scope.properties = propertyService.toAssociativeArray(data.Items);

                    $scope.$broadcast('setPreselectedValuesFromList', $scope.properties.ProductIds.PropertyValue, 'product');

                    $scope.$broadcast('setPreselectedValuesFromList', $scope.properties.CategoryIds.PropertyValue, 'productCategory');

                    $scope.$broadcast('setPreselectedValuesFromList', $scope.properties.FallbackCategoryIds.PropertyValue, 'productCategory');

                    $scope.$on('preSelectedValuesChanged',
                        function (event, data, itemsType) {
                            if (data && data[0]) {
                                if (itemsType === 'product') {
                                    $scope.properties.ProductIds.PropertyValue = data.map(function (e) { return e.id }).join(',');
                                }
                                else if (itemsType === 'productCategory') {
                                    $scope.properties.CategoryIds.PropertyValue = data.map(function (e) { return e.id }).join(',');
                                    $scope.properties.FallbackCategoryIds.PropertyValue = data.map(function (e) { return e.id }).join(',');
                                }
                            }
                            else {
                                if (itemsType === 'product') {
                                    $scope.properties.ProductIds.PropertyValue = '';
                                }
                                else if (itemsType === 'productCategory') {
                                    $scope.properties.CategoryIds.PropertyValue = '';
                                    $scope.properties.FallbackCategoryIds.PropertyValue = '';
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
                    if ($scope.properties.OpenInSamePage.PropertyValue && $scope.properties.OpenInSamePage.PropertyValue.toLowerCase() === 'true') {
                        $scope.properties.DetailsPageId.PropertyValue = emptyGuid;
                    }
                    else {
                        if (!$scope.properties.DetailsPageId.PropertyValue ||
                            $scope.properties.DetailsPageId.PropertyValue === emptyGuid) {
                            $scope.properties.OpenInSamePage.PropertyValue = true;
                        }
                    }
                });
            })
            .finally(function () {
                $scope.feedback.showLoadingIndicator = false;
            }); 
    }]);
})(jQuery);