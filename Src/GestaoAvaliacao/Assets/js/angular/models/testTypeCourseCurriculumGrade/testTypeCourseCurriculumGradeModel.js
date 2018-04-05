/* 
* TestTypeCourseCurriculumGrade-Model
*/
(function () {
    angular.module('services').factory('TestTypeCourseCurriculumGradeModel', ['$resource', function ($resource) {

        // Model
        var model = {            
            'save': {
                method: 'POST',
                url: base_url('TestTypeCourseCurriculumGrade/Save')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();