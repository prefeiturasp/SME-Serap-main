/* 
* ModelSkillLevel-Model
*/
(function () {
    angular.module('services').factory('StudentTestSessionModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getStudentsSession': {
                method: 'GET',
                url: base_url('/StudentTestSession/GetStudentsSession')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
