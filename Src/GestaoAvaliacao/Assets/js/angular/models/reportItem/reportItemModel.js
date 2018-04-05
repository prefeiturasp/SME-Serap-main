/* 
* ReportItem-Model
*/
(function () {
    angular.module('services').factory('ReportItemModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load_reportItemType': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemType')
            },
            'load_reportItemLevel': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemLevel')
            },
            'load_reportItem': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItem')
            },
            'load_reportItemCurriculumGrade': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemCurriculumGrade')
            },
            'load_reportItemSituation': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemSituation')
            },
            'load_reportItemSkill': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemSkill')
            },
            'GetByMatrix': {
                method: 'GET',
                url: base_url('ReportItem/GetByMatrix')
            },
            'load_reportItemSkillOneLevel': {
                method: 'GET',
                url: base_url('ReportItem/load_reportItemSkillOneLevel')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

