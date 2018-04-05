/* 
* TestTypeItemLevel-Model
*/
(function () {
    angular.module('services').factory('TestTypeItemLevelModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('TestTypeItemLevel/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('TestTypeItemLevel/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('TestTypeItemLevel/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('TestTypeItemLevel/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('TestTypeItemLevel/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

