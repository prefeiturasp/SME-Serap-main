/* 
* NumberItemsAplicationTai-Model
*/
(function () {
    angular.module('services').factory('NumberItemsAplicationTaiModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('NumberItemsAplicationTai/Load')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

