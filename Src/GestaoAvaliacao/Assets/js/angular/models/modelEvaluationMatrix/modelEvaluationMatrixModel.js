/* 
* ModelEvaluationMatrix-Model
*/
(function () {
    angular.module('services').factory('ModelEvaluationMatrixModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('ModelEvaluationMatrix/Load')
            },
            'loadPaginate': {
                method: 'GET',
                url: base_url('ModelEvaluationMatrix/LoadPaginate')
            },
            'find': {
                method: 'GET',
                url: base_url('ModelEvaluationMatrix/Find')
            },
            'save': {
                method: 'POST',
                url: base_url('ModelEvaluationMatrix/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('ModelEvaluationMatrix/Delete')
            },
            'search': {
                method: 'GET',
                url: base_url('ModelEvaluationMatrix/Search')
            },
            'get': {
                method: 'GET',
                url: base_url('ModelEvaluationMatrix/GetModelEvaluation')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

