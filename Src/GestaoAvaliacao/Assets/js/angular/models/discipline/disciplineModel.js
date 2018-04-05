/* 
* Discipline-Model
*/
(function () {
    angular.module('services').factory('DisciplineModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('Discipline/Load')
            },
            'loadCustom': {
                method: 'GET',
                url: base_url('Discipline/LoadCustom')
            },
            'find': {
                method: 'GET',
                url: base_url('Discipline/Find')
            },
            'searchDisciplines': {
                method: 'GET',
                url: base_url('Discipline/SearchDisciplines')
            },
            'search': {
                method: 'GET',
                url: base_url('Discipline/Search')
            },

            'searchDisciplinesSaves': {
                method: 'GET',
                url: base_url('Discipline/SearchDisciplinesSaves')
            },

            'searchAllDisciplines': {
                method: 'GET',
                url: base_url('Discipline/searchAllDisciplines')
            },
            'save': {
                method: 'POST',
                url: base_url('Discipline/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('Discipline/Delete')

            },
            'saveRange': {
                method: 'POST',
                url: base_url('Discipline/SaveRange')
            },
            'loadWithMatrix': {
                method: 'GET',
                url: base_url('Discipline/LoadWithMatrix')
            },
            'loadComboHasMatrix': {
                method: 'GET',
                url: base_url('Discipline/LoadComboHasMatrix')
            },
            'loadComboByTest': {
                method: 'GET',
                url: base_url('Discipline/LoadComboByTest')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();



