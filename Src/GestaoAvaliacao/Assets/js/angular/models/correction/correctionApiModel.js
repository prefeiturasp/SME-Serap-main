/**
 * function AdherenceModel Model
 * @namespace Model
 * @author Luís Maron - 25/02/2016
 */
(function () {

	'use strict';

	angular.module('services')
		.factory('CorrectionApiModel', CorrectionApiModel);

	CorrectionApiModel.$inject = ['$http'];

	function CorrectionApiModel($http) {

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
