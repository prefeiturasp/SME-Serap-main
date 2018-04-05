/* 
* ItemSituation-Model
*/
(function () {
    angular.module('services').factory('ItemSituationModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('ItemSituation/Load')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

