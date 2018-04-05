/* 
* Discipline-Model
*/
(function () {
    angular.module('services').factory('KnowledgeAreaModel', ['$resource', function ($resource) {

        // Model
        var model = {
            
            'load': {
                method: 'GET',
                url: base_url('KnowledgeArea/Load')
            },
            'loadCustom': {
                method: 'GET',
                url: base_url('KnowledgeArea/LoadCustom')
            },
            'find': {
                method: 'GET',
                url: base_url('KnowledgeArea/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('KnowledgeArea/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('KnowledgeArea/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('KnowledgeArea/Delete')
            },
            'loadAllActive': {
                method: 'GET',
                url: base_url('KnowledgeArea/LoadAllActive')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();



