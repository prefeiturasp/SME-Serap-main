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
            'loadTestItensByTestId': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadTestItensByTestId')
            },
            'loadStudentCorrectionAsync': {
                method: 'GET',
                url: base_url('ElectronicTest/LoadStudentCorrectionAsync')
            },
            'save': {
                method: 'POST',
                url: base_url('ElectronicTest/Save')
            },
            'finalizeCorrection': {
                method: 'POST',
                url: base_url('ElectronicTest/FinalizeCorrection')
            },
            'getTestTime': {
                method: 'GET',
                url: base_url('ElectronicTest/GetTestTime')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()