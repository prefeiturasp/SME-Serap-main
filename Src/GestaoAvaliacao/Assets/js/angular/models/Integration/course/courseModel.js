/* 
* Course-Model.
*/
(function () {
    angular.module('services').factory('CourseModel', ['$resource', function ($resource) {
        // Model
        var model = {
            'searchCourses': {
                method: 'GET',
                url: base_url('Course/SearchCourses')
            },
            'searchCoursesByLevel': {
                method: 'GET',
                url: base_url('Course/SearchCoursesByLevelEducation')
            },
            'searchCoursesByLevelModality': {
                method: 'GET',
                url: base_url('Course/SearchCoursesByLevelModality')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()