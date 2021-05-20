/* 
* ElectronicTestModel.
*/
(function () {
    angular.module('services').factory('ElectronicTestResultModel', ['$resource', function ($resource) {

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
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})()