/* 
* TestGroup-Model
*/
(function () {
    angular.module('services').factory('SubjectModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('Subject/Load')
            },
            'loadPaginate': {
                method: 'GET',
                url: base_url('Subject/LoadPaginate')
            },
            'find': {
                method: 'GET',
                url: base_url('Subject/Find')
            },
            'save': {
                method: 'POST',
                url: base_url('Subject/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('Subject/Delete')
            },
            'search': {
                method: 'GET',
                url: base_url('Subject/Search')
            },
            'get': {
                method: 'GET',
                url: base_url('Subject/GetSubject')
            },
            'loadGroupsSubGroups': {
                method: 'GET',
                url: base_url('Subject/LoadGroupsSubSubject')
            },
            'verifyDeleteSubGroup': {
                method: 'GET',
                url: base_url('Subject/VerifyDeleteSubSubject')
            },
            'searchSubjects': {
                method: 'GET',
                url: base_url('Subject/SearchSubjects')
            }
            ,
            'loadSubjectBySubsubject': {
                method: 'GET',
                url: base_url('Subject/LoadSubjectBySubsubject')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

