/**
 * function TestAdministrate Model
 * @namespace Factory
 * @author Julio Cesar Silva 25/11/2015
 */
(function () {

	angular.module('services').factory('TestAdministrateModel', ['$resource', function ($resource) {

		var model = {
		    'getSectionAdministrate': {
		        method: 'GET',
		        url: base_url('Test/GetSectionAdministrate')
		    },
		    'getStatusCorrectionList': {
		        method: 'GET',
		        url: base_url('Test/GetStatusCorrectionList')
		    },
		    'getAuthorize': {
		        method: 'GET',
		        url: base_url('Test/GetAuthorize')
	        }
		};
    
		return $resource('', {}, model);

	}]);
})();

