/**
 * function AdherenceModel Model
 * @namespace Model
 * @author Luís Maron - 25/02/2016
 */
(function () {

	'use strict';

	angular.module('services')
		.factory('StudentResultsGraphicsApiModel', StudentResultsGraphicsApiModel);

	StudentResultsGraphicsApiModel.$inject = ['$http'];

	function StudentResultsGraphicsApiModel($http) {

		return {
			'correction':
				function (params) {
					return $http.post(api_url('Correction'), params)
				},
			'absence':
				function (params) {
					return $http.post(api_url('Correction/Absence'), params)
				}
		};
	};

})();
