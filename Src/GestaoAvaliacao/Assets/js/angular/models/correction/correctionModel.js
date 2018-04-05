/**
 * function Correction Model
 * @namespace Model
 * @author Julio Cesar Silva - 25/11/2015
 */
(function () {

	angular.module('services').factory('CorrectionModel', ['$resource', function ($resource) {

	    var model = {

	        'getStudentBySection': {
		        method: 'GET',
		        url: base_url('Correction/GetStudentBySection')
	        },
	        'getStudentAnswer': {
	            method: 'GET',
	            url: base_url('Correction/GetStudentAnswer')
	        },
	        'finalizeCorrection': {
	            method: 'POST',
	            url: base_url('Correction/FinalizeCorrection')
	        },
	        'getResultCorrectionGrid': {
	            method: 'GET',
	            url: base_url('Correction/GetResultCorrectionGrid')
	        },
	        'unblockCorrection': {
	            method: 'POST',
	            url: base_url('Correction/UnblockCorrection')
	        },
	        'getResultExport': {
	            method: 'GET',
	            url: base_url('Correction/GetResultExport')
	        },
	        'getAuthorize': {
	            method: 'GET',
	            url: base_url('Correction/GetAuthorize')
	        },
	        'getTestAveragesPercentagesByTest': {
	            method: 'GET',
	            url: base_url('Correction/GetTestAveragesPercentagesByTest')
	        },
	        'getItems': {
	            method: 'GET',
	            url: base_url('Block/GetItems')
	        }
		};
    
		return $resource('', {}, model);

	}]);
})();

