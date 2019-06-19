function facetSelectorController($scope, SearchService) {
    $scope.searchService = SearchService;

    $scope.createQueryString = function() {
        var classSelector = ".js-facets";
        var queryStrings = {};
        var baseUrl = window.location.href.split('?')[0] + '?';
        var allChecked = $(classSelector + ':checked');
        allChecked.each(function() {
            var key = $(this).attr('data-facets-name');
            if (queryStrings[key] == null) {
                queryStrings[key] = $(this).attr('data-facets-value').toString() + '|';
            } else {
                queryStrings[key] += $(this).attr('data-facets-value').toString() + '|';
            }
        });

        for (var propertyName in queryStrings) {
            baseUrl += propertyName + '=' + queryStrings[propertyName] + '&';
        }
        var newUrl = baseUrl.substring(0, baseUrl.length - 1);
                
        window.location.href = newUrl;
    };

    $scope.clearFilters = function() {
        window.location.href = window.location.pathname;
    };

    $scope.ensureCheckboxesAreChecked = function() {
        var result = {},
            queryString = location.search.slice(1),
            re = /([^&=]+)=([^&]*)/g,
            m;

        while (m = re.exec(queryString)) {
            result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
        }

        var params = result;

        for (var propertyName in params) {
            var value = params[propertyName].split('|');

            for (var i = 0; i < value.length - 1; i++) {
                var filter = '.js-facets[data-facets-name="' +
                    propertyName +
                    '"][data-facets-value="' +
                    value[i] +
                    '"]';
                var checkbox = $(filter);
                if (checkbox != null) {
                    $scope[propertyName.replace(/ /g, '_') + "ShowFacets"] = true;
                    checkbox.attr("checked", true);
                }
            }
        }
    };

    $scope.ensureCheckboxesAreChecked();
}

angular.module('app').controller("FacetSelectorController", facetSelectorController);