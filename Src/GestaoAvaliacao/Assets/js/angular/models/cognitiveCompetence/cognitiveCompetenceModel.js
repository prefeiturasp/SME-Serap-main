/* 
* CorrelatedSkill-Model
*/
(function () {
	angular.module('services').factory('CognitiveCompetenceModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'load': {
				method: 'GET',
				url: base_url('CognitiveCompetence/Load')
			},
			'find': {
				method: 'GET',
				url: base_url('CognitiveCompetence/Find')
			},
			'findSimple': {
				method: 'GET',
				url: base_url('CognitiveCompetence/FindSimple')
			},
			'search': {
				method: 'GET',
				url: base_url('CognitiveCompetence/Search')
			},
			'save': {
				method: 'POST',
				url: base_url('CognitiveCompetence/Save')
			},
			'delete': {
				method: 'POST',
				url: base_url('CognitiveCompetence/Delete')
			}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();

