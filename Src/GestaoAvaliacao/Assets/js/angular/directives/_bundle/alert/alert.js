;(function () {

    'use strict';

    angular
        .module('services')
        .directive('alert', $alert);

    $alert.$inject = ['$notification'];

    function $alert($notification) {
        return {
            restrict: 'E',
            scope: {},
            controller: function () { },
            templateUrl: '/Assets/js/angular/directives/_bundle/alert/alert.html',
            replace: true,
            link: function ($scope, elem, attrs) {
                $scope.notifications = $notification.get();
                $scope.$watch('notifications', function () {
                    $scope.notifications = $notification.get();
                });
                $scope.remove = function (id) {
                    $notification.remove(id);
                };
            }
        };
    };
    
    angular
        .module('services')
        .directive('autoClose', $autoClose);

    $autoClose.$inject =  ['$timeout'];

    function $autoClose($timeout) {
        var increase = 100;
        var delay = 8000;
        return {
            restrict: 'A',
            require: '^alert',
            link: {
                pre: function ($scope, elem, attrs, alert) {
                },
                post: function ($scope, elem, attrs, alert) {
                    delay += increase;
                    alert = $scope.$parent;
                    attrs.$observe("autoClose", function (autoClose) {
                        if (autoClose) {
                            $timeout(function () {
                                $(elem).fadeTo(500, 0).slideUp(500, function () {
                                    delay += -increase;
                                    var id = $(elem).data('index');
                                    alert.remove(id);
                                });
                            }, delay);
                        }
                    });
                }
            }
        };
    };

})();