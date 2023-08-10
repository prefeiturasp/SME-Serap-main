/* 
* ReportStudies-Model
*/
(function () {
	angular.module('services').factory('ReportStudiesModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'carregaImportacoes': {
				method: 'GET',
				url: base_url('ReportStudies/ObterImportacoes')
			},			
			'importarArquivoResultado': {
				method: 'POST',
				url: base_url('ReportStudies/Importar')
			},
			'delete': {
				method: 'POST',
				url: base_url('ReportStudies/Delete')
			},
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
