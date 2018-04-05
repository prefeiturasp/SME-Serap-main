/* 
* ReportCorrection-Model
*/
(function () {
    angular.module('services').factory('ReportTestPerformanceModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getAllTests': {
                method: 'GET',
                url: base_url('ReportTestPerformance/GetAllTests')
            },
            'find': {
                method: 'GET',
                url: base_url('ReportTestPerformance/Find')
            },
            'findLevel': {
                method: 'GET',
                url: base_url('ReportTestPerformance/FindByLevel')
            },
            'getDres': {
                method: 'GET',
                url: base_url('ReportTestPerformance/GetDres')
            },
            'getSchools': {
                method: 'GET',
                url: base_url('ReportTestPerformance/GetSchools')
            },
            'exportDRE': {
                method: 'GET',
                url: base_url('ReportTestPerformance/ExportDRE')
            },
            'exportSchool': {
                method: 'GET',
                url: base_url('ReportTestPerformance/ExportSchool')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
