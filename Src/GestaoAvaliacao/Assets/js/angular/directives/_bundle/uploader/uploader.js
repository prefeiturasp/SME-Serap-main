/* @function Diretiva para upload de arquivo muitlpart
 * @namespace Directive
 * @autor: Julio Cesar da Silva: 28/09/2015
 */
(function (angular, $) {

    'use strict';

    angular.module('directives')
        .directive('uploader', uploader);
       
    uploader.$inject = ['$parse'];
    
    function uploader ($parse) {
        return {
            restrict: 'A',
            link: function($scope, element, attrs) {

                var model = $parse(attrs.uploader);
                var modelSetter = model.assign;

                element.bind('change', function(){
                    $scope.$apply(function () {
                        modelSetter($scope, element[0].files[0]);
                    });
                });
            }
        };
    };
        
})(angular, jQuery);