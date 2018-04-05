/* 
* EvaluationMatrixCourseCurriculumGrade-Model
*/
(function () {
    angular.module('services').factory('EvaluationMatrixCourseCurriculumGradeModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getCurriculumGradesByMatrix': {
                method: 'GET',
                url: base_url('EvaluationMatrixCourseCurriculumGrade/GetCurriculumGradesByMatrix')
            },
            'save': {
                method: 'POST',
                url: base_url('EvaluationMatrixCourseCurriculumGrade/Save')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();