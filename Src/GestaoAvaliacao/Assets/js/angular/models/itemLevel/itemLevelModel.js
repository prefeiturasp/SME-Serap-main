/* 
* ItemLevel-Model
*/
(function () {
    angular.module('services').factory('ItemLevelModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('ItemLevel/Load')
            },
            'loadLevels': {
                method: 'GET',
                url: base_url('ItemLevel/LoadLevels')
            },
            'find': {
                method: 'GET',
                url: base_url('ItemLevel/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('ItemLevel/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('ItemLevel/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('ItemLevel/Delete')
            }

        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

