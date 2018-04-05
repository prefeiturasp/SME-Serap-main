/* 
* Matrix-Model.
*/
(function () {
    angular.module('services').factory('EvaluationMatrixModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('EvaluationMatrix/GetByDiscipline')
            },
            'loadByMatriz': {
                method: 'GET',
                url: base_url('EvaluationMatrix/GetByMatriz')
            },
            'getComboByDiscipline': {
                method: 'GET',
                url: base_url('EvaluationMatrix/GetComboByDiscipline')
            },
            'loadCombo': {
                method: 'GET',
                url: base_url('EvaluationMatrix/LoadCombo')
            },
            'loadComboSimple': {
                method: 'GET',
                url: base_url('EvaluationMatrix/LoadComboSimple')
            },
            'loadComboSituation': {
                method: 'GET',
                url: base_url('EvaluationMatrix/LoadComboSituation')
            },
            'loadMatrix': {
                method: 'GET',
                url: base_url('EvaluationMatrix/Load')
            },
            'loadUpdate': {
                method: 'GET',
                url: base_url('EvaluationMatrix/LoadUpdate')
            },
            'find': {
                method: 'GET',
                url: base_url('EvaluationMatrix/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('EvaluationMatrix/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('EvaluationMatrix/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('EvaluationMatrix/Delete')

            },
            'saveRange': {
                method: 'POST',
                url: base_url('EvaluationMatrix/SaveRange')
            },
            'GetByDiscipline': {
                method: 'GET',
                url: base_url('EvaluationMatrix/GetByDiscipline')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()