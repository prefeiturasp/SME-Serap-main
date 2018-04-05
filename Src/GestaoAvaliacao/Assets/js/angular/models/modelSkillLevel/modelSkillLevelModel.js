/* 
* ModelSkillLevel-Model
*/
(function () {
    angular.module('services').factory('ModelSkillLevelModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('ModelSkillLevel/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('ModelSkillLevel/Find')
            },
            'findLevel': {
                method: 'GET',
                url: base_url('ModelSkillLevel/FindByLevel')
            }

        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

