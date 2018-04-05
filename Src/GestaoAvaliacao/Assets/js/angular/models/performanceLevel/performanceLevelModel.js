/* 
* ItemLevel-Model
*/
(function () {
    angular.module('services').factory('PerformanceLevelModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'load': {
                method: 'GET',
                url: base_url('PerformanceLevel/Load')
            },
            'loadLevels': {
                method: 'GET',
                url: base_url('PerformanceLevel/LoadLevels')
            },
            'find': {
                method: 'GET',
                url: base_url('PerformanceLevel/Find')
            },
            'getAll': {
                method: 'GET',
                url: base_url('PerformanceLevel/GetAll')
            },
            'search': {
                method: 'GET',
                url: base_url('PerformanceLevel/Search')
            },
            'save': {
                method: 'POST',
                url: base_url('PerformanceLevel/Save')
            },
            'delete': {
                method: 'POST',
                url: base_url('PerformanceLevel/Delete')
            }
        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();