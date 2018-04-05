/* 
* Modality-Model.
*/
(function () {
    angular.module('services').factory('ModalityModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('Modality/Load')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()