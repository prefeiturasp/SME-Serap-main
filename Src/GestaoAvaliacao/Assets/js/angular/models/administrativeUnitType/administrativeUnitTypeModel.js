/* 
* AdministrativeUnitType-Model
*/
(function () {
    angular.module('services').factory('AdministrativeUnitTypeModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'get': {
                method: 'GET',
                url: base_url('AdministrativeUnitType/Get')
            },
            'getAdministrativeUnitsTypes': {
                method: 'GET',
                url: base_url('AdministrativeUnitType/GetAdministrativeUnitsTypes')
            },
            'save': {
                method: 'POST',
                url: base_url('AdministrativeUnitType/Save')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();



