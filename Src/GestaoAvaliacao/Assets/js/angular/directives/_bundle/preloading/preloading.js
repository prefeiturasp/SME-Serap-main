/*
 * Autor: Jonas Antonelli
 * Name: preloading
 *
 * Descrição: 
 * Interceptor das requisições ajax do projeto
 *
 * Exemplo: 
 * <preloading message="Carregando..."></preloading>
*/

'use strict';

angular.module('directives').directive('preloading', [function () {

    //Caso seja necessário realizar a edição do template deve atualizá-lo a partir da variável(fonte) 'template'.
    var template = '<div id="preloading" ng-show-loading class="ng-cloak loading-box"><label>{{message}}</label></div>';

    return {
        restrict: 'EA',
        transclude: true,
        scope: true,
        template: template,
        replace: true,
        link: function ($scope, elem, attrs) {
            $scope.message = attrs.message || 'Carregando...';
        }
    };
}]);