/* 
* Parameter-Model
*/
(function () {
    angular.module('services').factory('ParameterModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'save': {
                method: 'POST',
                url: base_url('Parameter/Save')
            },
            'getParameters': {
                method: 'GET',
                url: base_url('Parameter/GetParameters')
            },
            'getParametersImage': {
                method: 'GET',
                url: base_url('Parameter/GetParametersImage')
            },
            'getParametersUploadFile': {
                method: 'GET',
                url: base_url('Parameter/GetParametersUploadFile')
            }

        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
