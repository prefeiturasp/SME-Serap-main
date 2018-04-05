/* 
* ReportCorrection-Model
*/
(function () {
    angular.module('services').factory('ReportItemPerformanceModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getAllTests': {
                method: 'GET',
                url: base_url('ReportItemPerformance/GetAllTests')
            },
            'find': {
                method: 'GET',
                url: base_url('ReportItemPerformance/Find')
            },
            'findLevel': {
                method: 'GET',
                url: base_url('ReportItemPerformance/FindByLevel')
            },
            'getDres': {
                method: 'GET',
                url: base_url('ReportItemPerformance/GetDres')
            },
            'getSchools': {
                method: 'GET',
                url: base_url('ReportItemPerformance/GetSchools')
            },
            'getSkillsByDiscipline': {
                method: 'GET',
                url: base_url('ReportItemPerformance/GetSkillsByDiscipline')
            },
            'exportDRE': {
                method: 'GET',
                url: base_url('ReportItemPerformance/ExportDRE')
            },
            'exportSchool': {
                method: 'GET',
                url: base_url('ReportItemPerformance/ExportSchool')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
