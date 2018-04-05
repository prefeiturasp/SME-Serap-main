/**
 * function Listagem Absence Reason Controller
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
        .controller("ListAbsenceReasonController", ListAbsenceReasonController);
    
    ListAbsenceReasonController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'AbsenceReasonModel'];

    function ListAbsenceReasonController($scope, $rootScope, $notification, $pager, AbsenceReasonModel) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(AbsenceReasonModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.absenceReasonList = null;
            $scope.absenceReason = null;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.totalItens = 0;
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
                        $scope.absenceReasonList = result.lista;

                        if (!$scope.pages > 0) {

                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.absenceReasonList = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                $scope.message = true;
                $scope.absenceReasonList = null;
            });
        };

        $scope.confirmar = function __confirmar(absenceReason) {
            $scope.absenceReason = absenceReason;
            angular.element('#modal').modal('show');
        };

        $scope.delete = function __delete(absenceReason) {

            AbsenceReasonModel.delete({ Id: absenceReason.Id }, function (result) {

                if (result.success) {
                    $notification.success(result.message);
                    $scope.absenceReasonList.splice($scope.absenceReasonList.indexOf(absenceReason), 1);
                    $scope.copySearch();
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modal').modal('hide');
        };

        Init();
    };

})(angular, jQuery);