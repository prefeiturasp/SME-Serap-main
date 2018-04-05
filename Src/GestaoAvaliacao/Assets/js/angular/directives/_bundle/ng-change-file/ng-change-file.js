/**
 * function Detectar mudanças em um input[type=file]
 * @namespace Controller
 * @author Julio Cesar da Silva - 23/05/2016
 */
(function (angular, $) {

    'use strict';

    //~GETTER
    angular
		.module('directives')
        .directive('ngChangeFile', ngChangeFile)
        .config([
                function () {
                    XMLHttpRequest.prototype.setRequestHeader = (function (sup) {
                        return function (header, value) {
                            if ((header === "__XHR__") && angular.isFunction(value))
                                value(this);
                            else
                                sup.apply(this, arguments);
                        };
                    })(XMLHttpRequest.prototype.setRequestHeader);
                }
        ]);

    function ngChangeFile() {
        return {
            restrict: 'A',
            scope: { callback: "=ngChangeFile" },
            link: function ($scope, element, attrs) {
                element.bind('change', function () {
                    $scope.$apply(function () {
                        if ($scope.callback) $scope.callback(element[0].files[0]);
                    });
                });
            }
        };
    };

})(angular, jQuery);