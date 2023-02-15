/* 
* NumberItemsAplicationTai-Model
*/
(function () {
    angular.module('services').factory('NumberItemsAplicationTaiModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'loadAll': {
                method: 'GET',
                url: base_url('NumberItemsAplicationTai/LoadAll')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

