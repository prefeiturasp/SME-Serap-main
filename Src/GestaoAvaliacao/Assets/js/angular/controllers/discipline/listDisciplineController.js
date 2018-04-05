/**
 * function Consulta de disciplinas
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
        .controller("ListDisciplineController", ListDisciplineController);

    ListDisciplineController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'DisciplineModel'];

    function ListDisciplineController($scope, rootScope, $notification, $pager, DisciplineModel) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(DisciplineModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.disciplineList = null;
            $scope.discipline = null;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.load();
        };

        $scope.copySearch = function __copySearch() {

            $scope.fieldSearch = JSON.parse(JSON.stringify($scope.pesquisa));
            $scope.paginate.indexPage(0);
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.load();
        };

        $scope.load = function __load() {

            $scope.paginate.paginate({ search: $scope.fieldSearch }).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();

                        $scope.disciplineList = result.lista;

                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.disciplineList = null;
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                $scope.message = true;
                $scope.disciplineList = null;
            });
        };

        $scope.confirmar = function __confirmar(discipline) {
            $scope.discipline = discipline;
            angular.element('#modal').modal('show');
        };

        $scope.delete = function __delete(discipline) {

            DisciplineModel.delete({ Id: discipline.Id }, function (result) {

                if (result.success) {

                    $scope.copySearch();

                    $notification.success(result.message);

                    $scope.disciplineList.splice($scope.disciplineList.indexOf(discipline), 1);

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modal').modal('hide');
        };

        Init();
    };

})(angular, jQuery);