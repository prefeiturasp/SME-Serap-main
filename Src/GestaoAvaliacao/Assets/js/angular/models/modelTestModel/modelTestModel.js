/**
 * function ModelTestModel Model
 * @namespace Model
 * @author julio cesar da silva - 14/05/2015
 */
(function () {

	'use strict';

	angular.module('services')
		.factory('ModelTestModel', ModelTestModel);

	ModelTestModel.$inject = ['$resource'];

	function ModelTestModel($resource) {

		var model = {

			'save': {
				method: 'POST',
				url: base_url('ModelTest/Save')
			},
			'find': {
				method: 'GET',
				url: base_url('ModelTest/Find')
			},
			'delete': {
				method: 'POST',
				url: base_url('ModelTest/Delete')
			},
			'search': {
				method: 'GET',
				url: base_url('ModelTest/Search')
			},
			'findSimple': {
				method: 'GET',
				url: base_url('ModelTest/FindSimple')
			}
		};

		return $resource('', {}, model);
	};

})();
