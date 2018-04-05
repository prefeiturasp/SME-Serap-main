/* 
* Reporter-item-Model
*/
(function () {
	angular.module('services').factory('ReportTestModel', ['$resource', function ($resource) {

		// Model
	    var model = {

            //filtros
	        'getYearsBySchool': {
	            method: 'GET',
	            url: base_url('ReportTest/GetYearsBySchool')
	        },
	        'getAllYears': {
	            method: 'GET',
	            url: base_url('ReportTest/GetAllYears')
	        },
	        'getTestByYear': {
	            method: 'GET',
	            url: base_url('ReportTest/GetTestByYear')
	        },
	        'getDResByTest': {
	            method: 'GET',
	            url: base_url('ReportTest/GetDResByTest')
	        },
	        'getSchoolsByTest': {
	            method: 'GET',
	            url: base_url('ReportTest/GetSchoolsByTest')
	        },
	        'getSectionsByTest': {
	            method: 'GET',
	            url: base_url('ReportTest/GetSectionsByTest')
	        },
	        'getTestBySchool': {
	            method: 'GET',
	            url: base_url('ReportTest/GetTestBySchool')
	        },

            //pesquisa
			'getReportByItemPerformance': {
				method: 'GET',
				url: base_url('ReportTest/GetReportByItemPerformance')
			},
			'getReportBySkillPerformance': {
			    method: 'GET',
			    url: base_url('ReportTest/GetReportBySkillPerformance')
			},
			'getReportBySchoolPerformance': {
			    method: 'GET',
			    url: base_url('ReportTest/GetReportBySchoolPerformance')
			}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
