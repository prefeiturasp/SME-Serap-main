/* 
* ReportCorrection-Model
*/
(function () {
    angular.module('services').factory('ReportItemChoiceModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getAllTests': {
                method: 'GET',
                url: base_url('ReportItemChoice/GetAllTests')
            },
            'find': {
                method: 'GET',
                url: base_url('ReportItemChoice/Find')
            },
            'findLevel': {
                method: 'GET',
                url: base_url('ReportItemChoice/FindByLevel')
            },
            'getDres': {
                method: 'GET',
                url: base_url('ReportItemChoice/GetDres')
            },
            'getSchools': {
                method: 'GET',
                url: base_url('ReportItemChoice/GetSchools')
            },
            'exportItemDRE': {
                method: 'GET',
                url: base_url('ReportItemChoice/ExportItemDRE')
            },
            'exportItemSchool': {
                method: 'GET',
                url: base_url('ReportItemChoice/ExportItemSchool')
            },
            'getItemPercentageChoicePerAlternative': {
                method: 'GET',
                url: base_url('ReportItemChoice/GetItemPercentageChoicePerAlternative')
            },
            'exportReport': {
                method: 'GET',
                url: base_url('ReportItemChoice/ExportReport')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
