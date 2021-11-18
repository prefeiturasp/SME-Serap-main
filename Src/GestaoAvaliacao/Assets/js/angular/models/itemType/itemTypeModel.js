/* 
* ItemType-Model
*/
(function () {
    angular.module('services').factory('ItemTypeModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('ItemType/Load')
            },
            'loadTestType': {
                method: 'GET',
                url: base_url('ItemType/LoadForTestType')
            },
            'find': {
            	method: 'GET',
            	url: base_url('ItemType/Find')
            },
            'search': {
            	method: 'GET',
            	url: base_url('ItemType/Search')
            },
            'save': {
            	method: 'POST',
            	url: base_url('ItemType/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('ItemType/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
