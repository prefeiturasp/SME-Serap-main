/* 
* Matrix-Model.
*/
(function () {
    angular.module('services').factory('PageConfigurationModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'get': {
                method: 'GET',
                url: base_url('PageConfiguration/Get')
            },
            'find': {
                method: 'GET',
                url: base_url('PageConfiguration/Find')
            },
            'loadAll': {
                method: 'GET',
                url: base_url('PageConfiguration/LoadAll')
            },
            'search': {
                method: 'GET',
                url: base_url('PageConfiguration/Search')
            },
            'getCategoryList': {
                method: 'GET',
                url: base_url('PageConfiguration/GetCategoryList')
            },
            'save': {
                method: 'POST',
                url: base_url('PageConfiguration/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('PageConfiguration/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()