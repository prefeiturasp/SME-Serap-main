/**
 * @function notificação de filtros selecionados
 * @namespace Controller
 * @author Everton Luis Ferreira - 06/10/2016
 */
(function (angular, $) {

	'use strict';

	angular
        .module('directives')
        .directive('notificationFilters', notificationFilters)

	function getTemplate() {

	    return '<div class="notificationFilter" data-side-filters ng-click="openFilter();" ng-if="notificationFilter > 0">{{notificationFilter}}</div>';
	};

	function notificationFilters() {

		return {
			restrict: 'EA',
			transclude: true,
			scope: {
				method: '&',
				filters: '@',
				countFilters: '@',
				customClass: "@"
			},
			template: getTemplate(),
			replace: true,
			link: function notification($scope, elem, attrs) {

				$scope.notificationFilter = 0;

				$scope.$watchCollection('[filters, countFilters]', function (n, o) {
					$scope.countFiltersSelected();
				});

				$scope.countFiltersSelected = function __countFiltersSelected() {

					if ($scope.filters.length) {
						$scope.filters = JSON.parse($scope.filters);
					}
					$scope.list = Object.keys($scope.filters);

					var i, count = 0, max = Object.keys($scope.filters).length;
					for (i = 0; i < max; i++) {
						if ($scope.filters[$scope.list[i]]) {
							count++;
						}
					}
					$scope.notificationFilter = (count + JSON.parse($scope.countFilters).length);

				};

				$scope.openFilter = function __openFilter() {
					$scope.method();
				};

			}
		};
		
	};

})(angular, jQuery);