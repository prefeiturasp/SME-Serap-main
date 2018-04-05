/* 
* TypeLevelEducation-Model
*/
(function () {
    angular.module('services').factory('TypeLevelEducationModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('TypeLevelEducation/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('TypeLevelEducation/Find')
            }         
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

