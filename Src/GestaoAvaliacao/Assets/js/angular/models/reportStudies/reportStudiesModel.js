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
			'update': {
				method: 'POST',
				url: base_url('ReportStudies/Update')
			},
			'listarDestinatariosEditarInicial': {
				method: 'GET',
				url: base_url('ReportStudies/ListarDestinatariosEditarInicial')
			},
			'listarDestinatarios': {
				method: 'GET',
				url: base_url('ReportStudies/ListarDestinatarios')
			},
			'listarGrupos': {
				method: 'GET',
				url: base_url('ReportStudies/ListarGrupos')
			},
			'checkReportStudiesExists': {
				method: 'GET',
				url: base_url('ReportStudies/CheckReportStudiesExists')
			},
			'getReportStudies': {
				method: 'GET',
				url: base_url('ReportStudies/GetReportStudies')
			},			
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
