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
        .controller("ListKnowledgeAreaController", ListKnowledgeAreaController);

    ListKnowledgeAreaController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'KnowledgeAreaModel', '$window'];

    function ListKnowledgeAreaController($scope, rootScope, $notification, $pager, KnowledgeAreaModel, $window) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(KnowledgeAreaModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.knowledgeAreaList = null;
            $scope.knowledgeArea = null;
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

            var obj = {};

            if ($scope.fieldSearch) {
                obj.search = $scope.fieldSearch;
            }
            else {
                obj.search = null;
            }

            $scope.paginate.paginate(obj).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.knowledgeAreaList = result.lista;

                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    }
                    else
                    {
                        $scope.message = true;
                        $scope.knowledgeAreaList = null;
                    }
                }
                else
                {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                $scope.message = true;
                $scope.knowledgeAreaList = null;
            });
        };

        $scope.confirmar = function __confirmar(knowledgeArea) {
            $scope.knowledgeArea = knowledgeArea;
            angular.element('#modal').modal('show');
        };

        $scope.delete = function __delete(knowledgeArea) {

            KnowledgeAreaModel.delete({ Id: knowledgeArea.Id }, function (result) {

                if (result.success) {

                    $scope.copySearch();

                    $notification.success(result.message);

                    $scope.knowledgeAreaList.splice($scope.knowledgeAreaList.indexOf(knowledgeArea), 1);

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modal').modal('hide');
        };

        $scope.edit = function (obj) {
            $window.location.href = '/KnowledgeArea/Form?Id=' + obj.Id;
        };

        Init();
    };

})(angular, jQuery);