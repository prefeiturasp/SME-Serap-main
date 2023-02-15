/* 
* ImportarResultadosPSP-Model
*/
(function () {
	angular.module('services').factory('ImportarResultadosPSPModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'carregaImportacoes': {
				method: 'GET',
				url: base_url('ImportarResultadosPSP/ObterImportacoes')
			},
			'carregaTiposResultadoPspAtivos': {
				method: 'GET',
				url: base_url('ImportarResultadosPSP/ObterTiposResultadoPspAtivos')
			},
			'importarArquivoResultado': {
				method: 'POST',
				url: base_url('ImportarResultadosPSP/ImportarArquivoResultado')
			},
			'baixarModelo': {
				method: 'GET',
				url: base_url('ImportarResultadosPSP/BaixarModelo')
			},
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
