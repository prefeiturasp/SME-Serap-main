/**
 * @function Consulta Item Level
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
       .module('appMain')
        .controller("ListItemLevelController", ListItemLevelController);

    ListItemLevelController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'ItemLevelModel'];

    function ListItemLevelController($scope, rootScope, $notification, $pager, ItemLevelModel) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(ItemLevelModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.itemLevelList = null;
            $scope.itemLevel = null;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.load();
        };

        $scope.copySearch = function __copySearch() {

            $scope.fieldSearch = angular.copy($scope.pesquisa);
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.load();
        };

        $scope.load = function __load() {

            $scope.paginate.paginate({ search: $scope.fieldSearch }).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();

                        $scope.itemLevelList = result.lista;

                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.itemLevelList = null;
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                $scope.message = true;
                $scope.itemLevelList = null;             
            });
        }

        $scope.confirmar = function __confirmar(itemLevel) {

            $scope.itemLevel = itemLevel;
            angular.element('#modal').modal('show');
        };

        $scope.delete = function __delete(itemLevel) {

            ItemLevelModel.delete({ Id: itemLevel.Id }, function (result) {

                if (result.success) {

                    $scope.copySearch();

                    $notification.success(result.message);

                    $scope.itemLevelList.splice($scope.itemLevelList.indexOf(itemLevel), 1);

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