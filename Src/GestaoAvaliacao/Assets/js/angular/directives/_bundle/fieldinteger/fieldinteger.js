/**
 * @function Campos do tipo numéricos inteiros
 * @namespace Directive
 * @author julio cesar da silva - 24/02/2016
 */
(function (angular, $) {

    'use strict';

	angular.module('directives')
           .directive('fieldinteger', fieldinteger);

    function fieldinteger() {

        return {
            restrict: 'A',
            scope: false,
            link: function (scope, element, attrs) {

                element.on("input", function () {
                    scope.$apply(function () {
                        element.val(element.val().match(/[0-9]*/));
                    });
                });
            }
        };
    };

})(angular, jQuery);