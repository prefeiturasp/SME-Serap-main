/*
 * Autor: Alex Figueiredo
 * Name: ng-show-loading, ng-hide-loading
 *
 * Descrição: 
 * Oculta/Mostra um elemento quando requisições estiverem em progresso
 *
 * Exemplo: 
 * <div ng-show-loading || ng-hide-loading ></div>
*/

'use strict';

// ng-show-loading
angular.module('directives').directive('ngShowLoading', ['$animate', '$httpTransform', function ($animate, $httpTransform) {
    return function (scope, elem) {
        scope.$watch(function () {
            return $httpTransform.loading();
        }, function (value) {
            $animate[!!value ? 'removeClass' : 'addClass'](elem, 'ng-hide');
        });
    };
}]);

// ng-hide-loading
angular.module('directives').directive('ngHideLoading', ['$animate', '$httpTransform', function ($animate, $httpTransform) {
    return function (scope, elem) {
        scope.$watch(function () {
            return $httpTransform.loading();
        }, function (value) {
            $animate[!!value ? 'addClass' : 'removeClass'](elem, 'ng-hide');
        });
    };
}]);