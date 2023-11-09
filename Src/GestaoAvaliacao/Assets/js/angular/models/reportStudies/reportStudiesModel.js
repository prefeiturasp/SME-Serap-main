/* 
* ReportStudies-Model
*/
(function () {
	angular.module('services').factory('ReportStudiesModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'carregaImportacoes': {
				method: 'GET',
				url: base_url('ReportStudies/ListReportStudies')
			},			
			'importarArquivoResultado': {
				method: 'POST',
				url: base_url('ReportStudies/Importar')
			},
			'delete': {
				method: 'POST',
				url: base_url('ReportStudies/Delete')
			},
			'listarDestinatarios': {
				method: 'GET',
				url: base_url('ReportStudies/ListarDestinatarios')
			},
			'listarGrupos': {
				method: 'GET',
				url: base_url('ReportStudies/ListarGrupos')
			},
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
