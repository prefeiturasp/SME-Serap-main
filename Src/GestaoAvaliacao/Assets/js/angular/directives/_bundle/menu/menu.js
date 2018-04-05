/**
 * AngularJS Diretive - Cria menu do sistema
 * Autor: Julio Silva
 * Data: 11/04/2016
 */
(function (angular, $) {

    'use strict';

    angular.module('directives')    
           .directive('menu', Menu);
    
    Menu.$inject = ['$http', '$notification', '$httpTransform'];

    function Menu($http, $notification, $httpTransform) {

        var __directive = {
            transclude: true,
            restrict: 'AE',
            templateUrl: '/Assets/js/angular/directives/_bundle/menu/menu.html',
            scope: {
                user: '@',
                system: '@'
            },
            link: {
                pre: __pre,
                post: __post
            }
        };

        function __pre($scope, element, attr) {

            $scope.loading = true;
            $scope.config = { menuState: false, menuClass: '' };

            $scope.sistemas = {};
            var __event = $scope.$watch(function () {
                $scope.loading = $httpTransform.loading();
            });

            $scope.safeApply = function __safeApply() {
                var $scope, fn, force = false;
                if (arguments.length === 1) {
                    var arg = arguments[0];
                    if (typeof arg === 'function') {
                        fn = arg;
                    } else {
                        $scope = arg;
                    }
                } else {
                    $scope = arguments[0];
                    fn = arguments[1];
                    if (arguments.length === 3) {
                        force = !!arguments[2];
                    }
                }
                $scope = $scope || this;
                fn = fn || function () { };

                if (force || !$scope.$$phase) {
                    $scope.$apply ? $scope.$apply(fn) : $scope.apply(fn);
                } else {
                    fn();
                }
            };

            function recursiveMenu(submenu) {

                if (submenu == null || submenu == undefined || submenu.length == 0)
                    return;

                if (submenu.Itens != undefined) {
                    for (var i = 0, len = submenu.Itens.length; i < len; i++) {
                        submenu['$open'] = false;
                        recursiveMenu(submenu.Itens[i]);
                    }
                }
            }; $scope.recursiveMenu = recursiveMenu;

            function collapse() {
                for (var i = 0, len = $scope.menus.length; i < len; i++) {
                    if ($scope.menus[i].Itens != undefined) {
                        if ($scope.menus[i].Itens.length > 0) {
                            $scope.menus[i]['$open'] = false;
                            $scope.recursiveMenu($scope.menus[i]);
                        }
                    }
                }
            }; $scope.collapse = collapse;

            function loadLinks() {
                $http.get('/Layout/MenuAngular').then(function (result) {
                    if (result.data.success) {
                        $scope.menus = result.data.lista;
                        $scope.collapse();
                    }
                    else {
                        $notification.error("Erro ao carregar Menu do Sistema");
                    }
                });
            };

            loadLinks();
        };
 
        function __post($scope, element, attr) {

            $scope.showManual = Boolean(getParameterValue(parameterKeys[0].SHOW_MANUAL) === "True");

            /**
             * @function Abrir/fechar
             * @param
             * @returns
             */
            $scope.open = function __open() {

                $scope.collapse();
                $(".system-menu").find(".collapse").each(function () {
                    $(this).removeClass('in');
                    $(this).blur();
                });

                $('.system-menu').toggleClass('system-menu-animation').promise().done(function a() {

                    if (angular.element(".system-menu").hasClass("system-menu-animation")) {
                        angular.element('body').css('overflow', 'hidden');
                    }
                    else {
                        angular.element('body').css('overflow', 'inherit');
                    }
                });
            };

            /**
             * @function Fechar painel de filtros por click da page
             * @param
             * @returns
             */
            function close(e) {

                if ($(e.target).parent().is("[data-menu-element]") || e.target.hasAttribute('data-menu-element'))
                    return;

                if ($(".system-menu").hasClass("system-menu-animation"))
                    $scope.open();

            }; angular.element('body').click(close);

            /**
             * @function Carregar Sistemas
             * @param
             * @returns
             */
            function loadSistemas() {
                $http.get('/Layout/GetSistemas').then(function (result) {
                    if (result.data.success)
                        $scope.sistemas = result.data.lista || ["Não há sistemas disponiveis."];
                    else
                        $notification.error("Erro ao carregar Menu do Sistema");
                });
            }; loadSistemas();

        };

        return __directive;
    };

})(angular, jQuery);