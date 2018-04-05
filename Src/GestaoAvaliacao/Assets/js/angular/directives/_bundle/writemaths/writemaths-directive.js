/**
 * @function Aplica a tooltip em real-time para mostrar fórmulas matemáticas no redactor
 * @namespace Directive
 * @autor: Julio Cesar da Silva: 23/07/2015
 */
(function (angular, $) {

	'use strict';

	angular
		.module('directives')
		.directive('writemaths', writemaths);

	writemaths.$inject = ['$notification', '$window'];

	function writemaths($notification, $window) {

		return {
			restrict: 'A',
			scope: {
				position: '@',
				previewposition: '@'
			},
			link: function (scope, element, attrs) {

				if (scope.position != undefined && scope.previewposition != undefined)
					element.writemaths({ position: scope.position, previewPosition: scope.previewposition, of: 'this' });
				else
					element.writemaths({ position: 'center top', previewPosition: 'center bottom', of: 'this' });
			}
		}
	};

})(angular, jQuery);