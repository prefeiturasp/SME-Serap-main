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
			}

			//'uploadFile': {
			//	method: 'POST',
			//	url: base_url('File/UploadFile')
			//},
			//'upload': {
			//	method: 'POST',
			//	url: base_url('File/Upload')
			//},
			//'delete': {
			//	method: 'POST',
			//	url: base_url('File/LogicalDelete')
			//},
			//'searchUploadedFiles': {
			//	method: 'GET',
			//	url: base_url('File/SearchUploadedFiles')
			//},
			//'existsLinkedFiles': {
			//	method: 'GET',
			//	url: base_url('File/ExistsLinkedFiles')
			//},
			//'checkFileExists': {
			//	method: 'GET',
			//	url: base_url('File/CheckFileExists')
			//},
			//'checkFilePathExists': {
			//	method: 'GET',
			//	url: base_url('File/CheckFilePathExists')
			//},
			//'checkFileExistsResultadoProva': {
			//	method: 'GET',
			//	url: base_url('File/CheckFileExistsResultadoProva')
			//}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();
