/* 
* StudentResults-Model
*/
(function () {
    angular.module('services').factory('studentResultsListModel', ['$resource', function ($resource) {

        // Model
        var model = {

            // CHAMADA ETAPA 1
            'getResultadosDosEstudantes': {
                method: 'GET',
                url: base_url('studentResults/getResultadosDosEstudantes')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
