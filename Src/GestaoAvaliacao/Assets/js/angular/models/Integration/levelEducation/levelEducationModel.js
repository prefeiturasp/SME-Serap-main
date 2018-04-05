/* 
* LevelEducation-Model.
*/
(function () {
    angular.module('services').factory('LevelEducationModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('LevelEducation/Load')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()