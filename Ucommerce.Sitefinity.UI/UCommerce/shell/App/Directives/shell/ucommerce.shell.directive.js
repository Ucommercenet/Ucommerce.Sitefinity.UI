// Simple navigation directive, binding to a navigation view.
var uc_shell = function($compile) {
	return {
		restrict: "E",
		replace: true,
		scope: {
			'startPage': '@',
			'contentPickerType': '@',
			'disableResize': '@',
			'fixedLeftSize': '@',
			'treeIndetionSize': '@',
			'stylesheet': '@',
			'script' : '@'
		},
		templateUrl: 'App/Directives/shell/shellView.html',
		controller: shellController,
		link: function ($scope, $elem, $attr) {
		}
	}
}

