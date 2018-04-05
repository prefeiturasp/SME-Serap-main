/**
 * function 
 * @namespace Factory
 * @author 
 */
(function () {

    angular.module('services').factory('TestRevokeModel', ['$resource', function ($resource) {

        var model = {
            'getItems': {
                method: 'GET',
                url: base_url('Test/GetItems')
            },
            'getTestInfo': {
                method: 'GET',
                url: base_url('Test/GetTestInfo')
            },
            'updateItems': {
                method: 'POST',
                url: base_url('Block/UpdateItemsRevoked')
            }
        };

        return $resource('', {}, model);

    }]);

})();