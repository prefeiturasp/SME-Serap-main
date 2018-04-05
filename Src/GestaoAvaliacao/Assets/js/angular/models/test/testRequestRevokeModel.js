/**
 * function 
 * @namespace Factory
 * @author 
 */
(function () {

    angular.module('services').factory('TestRequestRevokeModel', ['$resource', function ($resource) {

        var model = {
            'GetPendingRevokeItems': {
                method: 'GET',
                url: base_url('Test/GetPendingRevokeItems')
            },
            'getRequestRevokes': {
                method: 'GET',
                url: base_url('Test/GetRequestRevokes')
            },

            'revokeItem': {
                method: 'POST',
                url: base_url('Block/RevokeItem')
            },
            'updateRequestRevokedByTestBlockItem': {
                method: 'POST',
                url: base_url('Block/UpdateRequestRevokedByTestBlockItem')
            }
        };

        return $resource('', {}, model);

    }]);

})();