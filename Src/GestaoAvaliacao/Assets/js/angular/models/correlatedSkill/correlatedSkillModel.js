/* 
* CorrelatedSkill-Model
*/
(function () {
    angular.module('services').factory('CorrelatedSkillModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'loadList': {
                method: 'GET',
                url: base_url('CorrelatedSkill/LoadList')
            },
            'save': {
                method: 'POST',
                url: base_url('CorrelatedSkill/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('CorrelatedSkill/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();

