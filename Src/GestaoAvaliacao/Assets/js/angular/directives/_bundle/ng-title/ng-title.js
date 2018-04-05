/**
 * @function Implementar attr "title" em selects que utilizam-se de ng-options
 * @author julio.silva@mstech.com
 * @since 18/11/2016
 */
; (function () {

    'use strict';

    angular
       .module('directives')
       .directive('ngTitle', ngTitle);

    ngTitle.$inject = ['$timeout'];

    function ngTitle($timeout) {
        return {
            restrict: 'A',
            scope: {
                ngTitle: '=',
                ngTitlePropertie: '@'
            },
            link: function ($scope, elem, attrs) {
                $scope.ngTitleIndex = 1;
                attrs.$observe('ngTitleIndex', function (value) {
                    if(value)
                        $scope.ngTitleIndex = parseInt(value);
                });
                elem.bind('DOMSubtreeModified', function (e) {
                    if (e.target.innerHTML.length > 0) {
                        $timeout(function () {
                            var options = elem.children().each(function (e) {
                                var listkey = e - $scope.ngTitleIndex; //desconta-se o indice do option default (null)
                                if ($scope.ngTitle instanceof Array) {
                                    if ($scope.ngTitle[listkey]) {
                                        $(this).attr("title", ($scope.ngTitle[listkey][$scope.ngTitlePropertie] || $scope.ngTitle[listkey]));
                                    }
                                }
                                else {
                                    $(this).attr("title", $(this).text());
                                }
                            });
                        }, 0);

                    }
                });
            }
        };
    };


    angular
      .module('directives')
      .directive('ngTitleSelected', ngTitleSelected);

    ngTitleSelected.$inject = ['$timeout'];

    function ngTitleSelected($timeout) {
        return {
            restrict: 'A',
            link: function ($scope, elem, attrs) {
                elem.bind("change", function () {
                    elem.attr("title", elem.find(":selected").text());
                });
            }
        };
    };

})();