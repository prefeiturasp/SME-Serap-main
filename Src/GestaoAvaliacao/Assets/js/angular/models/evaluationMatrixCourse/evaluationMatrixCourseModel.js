/* 
* EvaluationMatrixCourse-Model
*/
(function () {
    angular.module('services').factory('EvaluationMatrixCourseModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('EvaluationMatrixCourse/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('EvaluationMatrixCourse/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('EvaluationMatrixCourse/Delete')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
