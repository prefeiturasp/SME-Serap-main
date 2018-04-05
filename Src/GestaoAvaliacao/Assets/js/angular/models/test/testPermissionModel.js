/**
 * function TestPermission Model
 * @namespace Factory
 * @author Haila Pelloso 09/03/2017
 */
(function () {

    angular.module('services').factory('TestPermissionModel', ['$resource', function ($resource) {

        var model = {
            'getTestsPermissions': {
                method: 'GET',
                url: base_url('Test/GetTestsPermissions')
            },
            'getAuthorize': {
                method: 'GET',
                url: base_url('Test/GetAuthorize')
            },
            'savePermission': {
                method: 'POST',
                url: base_url('Test/SavePermission')
            },
        };

        return $resource('', {}, model);

    }]);
})();

