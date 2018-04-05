/* 
* CurriculumGrade-Model.
*/
(function () {
    angular.module('services').factory('CurriculumGradeModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'searchCurriculumGrade': {
                method: 'GET',
                url: base_url('CurriculumGrade/SearchCurriculumGrade')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()