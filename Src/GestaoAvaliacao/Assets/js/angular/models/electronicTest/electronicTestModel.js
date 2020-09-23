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
            'loadAnswersAsync': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadAnswersAsync')
            },
            'save': {
                method: 'POST',
                url: base_url('ElectronicTest/Save')
            },
            'saveAnswersAsync': {
                method: 'POST',
                url: base_url('ElectronicTest/SaveAnswersAsync')
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