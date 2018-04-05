'use strict';

angular.module('services').directive('modalAlert', ['$notification', '$timeout', function ($notification, $timeout) {

    //Caso seja necessário realizar a edição do template deve atualizá-lo a partir da variável(fonte) 'template'.
    var template = '<div id="modal-alert" style="margin-top: 10px" ng-show="visible">' +
        '<div ng-class="[\'alert\', \'alert-\'+notification.type]">' + 
            '<button class="close" ng-click="remove()" ng-show="notification.closeable">&times;</button>' + 
            '<div>' +
                '<span ng-show="notification.type == \'warning\'"><i class="material-icons">warning</i></span>' +
                '<span ng-show="notification.type == \'info\'"><i class="material-icons">check_circle</i></span>' +
                '<span ng-show="notification.type == \'error\'"><i class="material-icons">check_circle</i></span>' +
                '<span ng-show="notification.type == \'success\'"><i class="material-icons">check_circle</i></span>' +
                '<strong>{{notification.title}}</strong> {{notification.message}}' +
            '</div>' +
        '</div>' +
    '</div>';

    return {
        restrict: 'E',
        scope: {
            notification: '=',
            visible: '&',
            timecancel: '&'
        },
        template: template,
        link: {

            pre: function (scope, elem, attrs) {

                // trigger para ocultar/mostrar
                scope.$watch('notification', function () {
                    
                    // Fechar uma notificação
                    if (scope.notification == undefined) {
                        scope.visible = false;
                    }
                    // Iniciar uma notificação
                    else if (scope.notification != undefined) {

                        // cancelar timer de animação caso exista uma msg anterior sendo animada.
                        if (scope.timecancel != undefined)
                            $timeout.cancel(scope.timecancel);

                        // tornar a notificação visível
                        scope.visible = true;
                        angular.element(elem).css('display', 'block');
                        angular.element(elem).css('opacity', '1');

                        // tempo de duração de uma notificação
                        scope.timecancel = $timeout(function () {
                            $(elem).fadeTo(500, 0).slideUp(500, function () {
                                scope.notification = undefined;
                                $timeout.cancel(scope.timecancel);
                                scope.timecancel = undefined;
                            });
                        }, scope.notification.time);
                    }
                });
            },

            post: function (scope, elem, attrs) {
                // oculta a notificação automaticamente ao clique.
                scope.remove = function () {
                    scope.visible = false;
                };
            }
        }   
    };
}]);