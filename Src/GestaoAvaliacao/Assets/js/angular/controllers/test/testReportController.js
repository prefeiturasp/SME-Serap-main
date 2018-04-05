/**
 * function TestReportController Controller
 * @namespace Controller
 * @author Julio Cesar Silva 24/11/2015
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("TestReportController", TestReportController);

	TestReportController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$timeout'];



	function TestReportController($rootScope, $scope, $window, $sce, $timeout) {

	};

})(angular, jQuery);