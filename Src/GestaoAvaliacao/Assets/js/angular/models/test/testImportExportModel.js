(function () {
    angular.module('services').factory('TestImportExportModel', ['$resource', function ($resource) {

		var model = {

			'loadByLevelEducationModality': {
				method: 'GET',
				url: base_url('/CurriculumGrade/LoadByLevelEducationModality')
			},
			'importAnalysisSearch': {
			    method: 'GET',
			    url: base_url('/Test/ImportAnalysisSearch')
			},
			'exportAnalysisSearch': {
				method: 'GET',
				url: base_url('/Test/ExportAnalysisSearch')
			},
			'solicitExport': {
				method: 'POST',
				url: base_url('/Test/SolicitExport')
			},
			'remove': {
			    method: 'POST',
			    url: base_url('/Test/remove')
			}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();

