/// <reference path="rating.tpl.html" />
/**
 * @function - criar um sistema simples de rating.
 * @autor julio.silva@mstech.com
 * @since 01/10/2014
 */
(function __ratingDirective() {

    'use strict';

    angular
        .module('directives')
        .directive('ratingDirective', ratingDirective);

    ratingDirective.$inject = [];

    function ratingDirective() {

        return {
            restrict: 'A',
            templateUrl: "../Assets/js/angular/directives/_bundle/rating/rating.tpl.html",
            scope: {
                difficulty: '=',
                list: '=',
                internalselected: '&',
                index: '&',
                lock: '='
            },
            link: {
                pre: function (scope) {
                    scope.$watch('difficulty', function () {
                        if (scope.difficulty != undefined && scope.list != undefined ) {
                            for (var i = 0; i < scope.list.length; i++) {
                                if (scope.difficulty.Id == scope.list[i].Id) {
                                    scope.index = i;
                                    scope.internalselected = scope.list[i].Description;
                                    break;
                                }
                            }
                        }
                        if (scope.difficulty == undefined) {
                            scope.index = -1;
                            scope.internalselected = undefined;
                        }
                    });
                },
                post: function (scope) {
                    scope.select = function (_value) {
                        if (!scope.lock) {
                            scope.internalselected = _value.Description;
                            scope.difficulty = _value;
                            scope.index = scope.list.indexOf(_value);
                        }         
                    }
                }
            }
        };
    };

})();