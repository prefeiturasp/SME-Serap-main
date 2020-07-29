/* 
* TestType-Model
*/
(function () {
    angular.module('services').factory('TestTypeModel', ['$resource', function ($resource) {

        var model = {

            'loadByUserGroup': {
                method: 'GET',
                url: base_url('TestType/LoadByUserGroup')
            },
            'load': {
                method: 'GET',
                url: base_url('TestType/Load')
            },
            'findTest': {
                method: 'GET',
                url: base_url('TestType/FindTest')
            },
            'find': {
                method: 'GET',
                url: base_url('TestType/Find')
            },
            'search': {
                method: 'GET',
                url: base_url('TestType/Search')
            },
            'existsTestAssociated': {
                method: 'GET',
                url: base_url('TestType/ExistsTestAssociated')
            },
            'save': {
                method: 'POST',
                url: base_url('TestType/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('TestType/Delete')
            },
            'getFrequencyApplicationList': {
                method: 'GET',
                url: base_url('TestType/GetFrequencyApplicationList')
            },
            'getFrequencyApplicationParentList': {
                method: 'GET',
                url: base_url('TestType/GetFrequencyApplicationParentList')
            },
            'getFrequencyApplicationChildList': {
                method: 'GET',
                url: base_url('TestType/GetFrequencyApplicationChildList')
            },
            'getDeficiencies': {
                method: 'GET',
                url: base_url('TestType/GetDeficiencies')
            }

        };
 
        return $resource('', {}, model);

    }]);
})();

