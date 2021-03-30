/**
 * function Correction Model
 * @namespace Model
 * @author Julio Cesar Silva - 25/11/2015
 */
(function () {

	angular.module('services').factory('StudentResultsGraphicsModel', ['$resource', function ($resource) {

	    var model = {

	        'getPercentualDeAcerto': {
	            method: 'GET',
				url: base_url('StudentResultsGraphics/GetPercentualDeAcerto')
	        },
	        'getDataTest': {
	            method: 'GET',
				url: base_url('StudentResultsGraphics/GetDataTest')
			},
			'getTests':{
				method: 'GET',
				url: base_url('StudentResultsGraphics/GetTests')
			}
		};
    
		return $resource('', {}, model);

	}]);
})();

