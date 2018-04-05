/* 
* FormatType-Model
*/
(function () {
    angular.module('services').factory('FormatTypeModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('FormatType/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('FormatType/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('FormatType/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('FormatType/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('FormatType/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

