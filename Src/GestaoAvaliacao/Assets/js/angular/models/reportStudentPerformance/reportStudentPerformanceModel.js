/* 
* ReportCorrection-Model
*/
(function () {
    angular.module('services').factory('ReportStudentPerformanceModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getStudentInformation': {
                method: 'GET',
                url: base_url('ReportStudentPerformance/GetStudentInformation')
            },
            'getUnitsInformation': {
                method: 'GET',
                url: base_url('ReportStudentPerformance/GetUnitsInformation')
            },
            'exportReport': {
                method: 'GET',
                url: base_url('ReportStudentPerformance/ExportReport')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
