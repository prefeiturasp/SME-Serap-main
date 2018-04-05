/* 
* ElectronicTestModel.
*/
(function () {
    angular.module('services').factory('ElectronicTestModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('ElectronicTest/Load')
            },
            'loadByTestId': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadByTestId')
            },
            'loadItensByTestId': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadItensByTestId')
            },
            'loadAlternativesByItens': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadAlternativesByItens')
            },            
            'save': {
                method: 'POST',
                url: base_url('ElectronicTest/Save')
            },
            'finalizeCorrection': {
                method: 'POST',
                url: base_url('ElectronicTest/FinalizeCorrection')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()