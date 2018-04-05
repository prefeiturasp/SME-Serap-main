/* 
* Course-Model
*/
(function () {
    angular.module('services').factory('CourseModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('Course/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('Course/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('Course/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('Course/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('Course/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

