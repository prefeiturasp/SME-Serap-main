/* 
* AbsenceReason-Model
*/
(function () {
	angular.module('services').factory('AbsenceReasonModel', ['$resource', function ($resource) {

		// Model
		var model = {

			'load': {
				method: 'GET',
				url: base_url('AbsenceReason/Load')
			},
			'find': {
				method: 'GET',
				url: base_url('AbsenceReason/Find')
			},
			'findSimple': {
				method: 'GET',
				url: base_url('AbsenceReason/FindSimple')
			},
			'search': {
				method: 'GET',
				url: base_url('AbsenceReason/Search')
			},
			'save': {
				method: 'POST',
				url: base_url('AbsenceReason/Save')
			},
			'delete': {
				method: 'POST',
				url: base_url('AbsenceReason/Delete')
			},
			'loadCombo': {
				method: 'GET',
				url: base_url('AbsenceReason/LoadCombo')
			}
		};

		// Retorna o serviço       
		return $resource('', {}, model);

	}]);
})();

