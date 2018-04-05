/* 
* TestTypeCourse-Model
*/
(function () {
    angular.module('services').factory('TestTypeCourseModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('TestTypeCourse/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('TestTypeCourse/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('TestTypeCourse/Delete')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
