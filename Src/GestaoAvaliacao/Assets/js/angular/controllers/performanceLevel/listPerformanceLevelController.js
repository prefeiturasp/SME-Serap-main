/**
 * function Cadastro/Edição Performance Level
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('appMain', ['services', 'filters', 'directives']);

    angular
        .module('appMain')
        .controller("ListPerformanceLevelController", ListPerformanceLevelController);

    ListPerformanceLevelController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'PerformanceLevelModel'];

    function ListPerformanceLevelController($scope, rootScope, $notification, $pager, PerformanceLevelModel) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(PerformanceLevelModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.performanceLevelList = null;
            $scope.performanceLevel = null;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.load();
        };

        $scope.copySearch = function __copySearch() {

            $scope.fieldSearch = angular.copy($scope.pesquisa);
            $scope.paginate.indexPage(0);
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.load();
        };

        $scope.load = function __copySearch() {

            $scope.paginate.paginate({ search: $scope.fieldSearch }).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {

                        $scope.paginate.nextPage();

                        $scope.performanceLevelList = result.lista;

                        if (!$scope.pages > 0) {

                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    }
                    else {

                        $scope.message = true;
                        $scope.performanceLevelList = null;
                    }
                }
                else {

                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {

                $scope.message = true;
                $scope.performanceLevelList = null;
            });
        };

        $scope.confirmar = function __confirmar(performanceLevel) {

            $scope.performanceLevel = performanceLevel;
            angular.element('#modal').modal('show');
        };

        $scope.delete = function __delete(performanceLevel) {

            PerformanceLevelModel.delete({ Id: performanceLevel.Id }, function (result) {

                if (result.success) {

                    $scope.copySearch();

                    $notification.success(result.message);

                    $scope.performanceLevelList.splice($scope.performanceLevelList.indexOf(performanceLevel), 1);
                }
                else {

                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });

            angular.element('#modal').modal('hide');
        };

        Init();
    };

})(angular, jQuery);