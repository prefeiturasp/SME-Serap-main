/* 
* Skill-Model
*/
(function () {
    angular.module('services').factory('SkillModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('Skill/Load')
            },
            'find': {
                method: 'GET',
                url: base_url('Skill/Find')
            },
            'findParent': {
                method: 'GET',
                url: base_url('Skill/FindParent')
            },            
            'search': {
                method: 'GET',
                url: base_url('Skill/Search')
            },
            'searchByMatrix': {
                method: 'GET',
                url: base_url('Skill/SearchByMatrix')
            },
            'loadByMatrix': {
                method: 'GET',
                url: base_url('Skill/LoadByMatrix')
            },
            'save': {
                method: 'POST',
                url: base_url('Skill/Save')
            },
            'saveRange': {
                method: 'POST',
                url: base_url('Skill/SaveRange')
            },
            'delete': {
                method: 'POST',
                url: base_url('Skill/Delete')
            },
            'deleteByMatrix': {
                method: 'POST',
                url: base_url('Skill/DeleteByMatrix')
            },
            'getByMatriz': {
                method: 'GET',
                url: base_url('Skill/GetByMatrix')
            },
            'getByTestDiscipline': {
                method: 'GET',
                url: base_url('Skill/GetByTestDiscipline')
            },
            'getByParent': {
                method: 'GET',
                url: base_url('Skill/GetByParent')
            },
            'getComboByDiscipline': {
                method: 'GET',
                url: base_url('Skill/GetComboByDiscipline')
            }
            
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

