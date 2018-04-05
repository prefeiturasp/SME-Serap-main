/* 
* TestPerformanceLevel-Model
*/
(function () {
    angular.module('services').factory('TestPerformanceLevelModel', ['$resource', function ($resource) {

        // Model
        var model = {

            'getPerformanceLevelByTest': {
                method: 'GET',
                url: base_url('TestPerformanceLevel/GetPerformanceLevelByTest')
            }

        };

        // Retorna o serviço       
        return $resource('', {}, model);

    }]);
})();



