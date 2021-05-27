/* 
* ElectronicTestModel.
*/
(function () {
    angular.module('services').factory('ElectronicTestResultModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('ElectronicTestResult/Load')
            },
            'loadByTestId': {
                method: 'GET',
                url: base_url('ElectronicTestResult/LoadByTestId')
            },
            'loadTestItensByTestId': {
                method: 'GET',
                url: base_url('ElectronicTestResult/LoadTestItensByTestId')
            },
            'loadStudentCorrectionAsync': {
                method: 'GET',
                url: base_url('ElectronicTestResult/LoadStudentCorrectionAsync')
            },
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()