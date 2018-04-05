/* 
* ReportAnswerSheet-Model
*/
(function () {
    angular.module('services').factory('ReportAnswerSheetModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'getFollowUpIdentificationDRE': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/GetFollowUpIdentificationDRE')
            },
            'getFollowUpIdentificationSchool': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/GetFollowUpIdentificationSchool')
            },
            'getFollowUpIdentificationFiles': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/GetFollowUpIdentificationFiles')
            },
            'exportFollowUpIdentification': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/ExportFollowUpIdentification')
            },
            'downloadZipFiles': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/DownloadZipFiles')
            },
            'getIdentificationReportInfo': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/GetIdentificationReportInfo')
            },
            'getSituationList': {
                method: 'GET',
                url: base_url('ReportAnswerSheet/GetSituationList')
            }

        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();
