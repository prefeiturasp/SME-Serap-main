/* 
* ResponseChangeLogModel
*/
(function () {
    angular.module('services').factory('ResponseChangeLogModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getResponseChangeLog': {
                method: 'GET',
                url: base_url('ResponseChangeLog/GetResponseChangeLog')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
