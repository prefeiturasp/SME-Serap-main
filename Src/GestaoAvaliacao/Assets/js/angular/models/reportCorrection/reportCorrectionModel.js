/* 
* ReportCorrection-Model
*/
(function () {
	angular.module('services').factory('ReportCorrectionModel', ['$resource', function ($resource) {

		// Model
		var model = {
		    'getAllTests': {
				method: 'GET',
				url: base_url('ReportCorrection/GetAllTests')
		    },
			'find': {
				method: 'GET',
				url: base_url('ReportCorrection/Find')
			},
			'findLevel': {
				method: 'GET',
				url: base_url('ReportCorrection/FindByLevel')
			},
			'getDres': {
			    method: 'GET',
			    url: base_url('ReportCorrection/GetDres')
			},
			'getSchools': {
			    method: 'GET',
			    url: base_url('ReportCorrection/GetSchools')
			},
			'getSection': {
                method: 'GET',
                url: base_url('ReportCorrection/GetSection')
			},
			'getStudent': {
			    method: 'GET',
			    url: base_url('ReportCorrection/GetStudent')
			},
			'exportDRE': {
			    method: 'GET',
			    url: base_url('ReportCorrection/ExportDRE')
			},
			'exportSchool': {
			    method: 'GET',
			    url: base_url('ReportCorrection/ExportSchool')
			},
			'exportSection': {
			    method: 'GET',
			    url: base_url('ReportCorrection/ExportSection')
			},
			'exportStudent': {
			    method: 'GET',
			    url: base_url('ReportCorrection/ExportStudent')
			}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
