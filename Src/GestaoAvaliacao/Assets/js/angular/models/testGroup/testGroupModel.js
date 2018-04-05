/* 
* TestGroup-Model
*/
(function () {
    angular.module('services').factory('TestGroupModel', ['$resource', function ($resource) {

        // Model
        var model = {
            'load': {
                method: 'GET',
                url: base_url('TestGroup/Load')
            },
            'loadPaginate': {
                method: 'GET',
                url: base_url('TestGroup/LoadPaginate')
            },
            'find': {
                method: 'GET',
                url: base_url('TestGroup/Find')
            },
            'save': {
                method: 'POST',
                url: base_url('TestGroup/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('TestGroup/Delete')
            },
            'search': {
                method: 'GET',
                url: base_url('TestGroup/Search')
            },
            'get': {
                method: 'GET',
                url: base_url('TestGroup/GetTestGroup')
            },
            'loadGroupsSubGroups': {
                method: 'GET',
                url: base_url('TestGroup/LoadGroupsSubGroups')
            },
            'verifyDeleteSubGroup': {
                method: 'GET',
                url: base_url('TestGroup/VerifyDeleteSubGroup')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

