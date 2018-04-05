/**
 * function AdherenceApiModel Model
 * @namespace Model
 * @author Luís Maron - 25/02/2016
 */
(function () {

	'use strict';

	angular.module('services')
		.factory('AdherenceApiModel', AdherenceApiModel);

	AdherenceApiModel.$inject = ['$http'];

	function AdherenceApiModel($http) {

		return {
			'adherence': 
				function(params) {
					return $http.post(api_url('adherence'), params)
				},
			'adherenceList':
				function (params) {
					return $http.post(api_url('adherence/List'), params)
				}
		};
	};

})();
