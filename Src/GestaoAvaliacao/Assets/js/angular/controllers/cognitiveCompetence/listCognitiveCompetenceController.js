/**
 * @function Listagem de Competências Cognitivas Controller
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {
	
    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ListCognitiveCompetenceController", ListCognitiveCompetenceController);

    ListCognitiveCompetenceController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'CognitiveCompetenceModel'];

    function ListCognitiveCompetenceController($scope, rootScope, $notification, $pager, CognitiveCompetenceModel) {

        function Init() {

            $notification.clear();
            $scope.paginate = $pager(CognitiveCompetenceModel.search);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.cognitiveCompetenceList = null;
            $scope.cognitiveCompetence = null;
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
                        $scope.cognitiveCompetenceList = result.lista;

                        if (!$scope.pages > 0) {

                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.cognitiveCompetenceList = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                $scope.message = true;
                $scope.cognitiveCompetenceList = null;
            });
        };
       
        $scope.confirmar = function __confirmar(cognitiveCompetence) {
            $scope.cognitiveCompetence = cognitiveCompetence;
            angular.element('#modal').modal('show');
        };
        
        $scope.delete = function __delete(cognitiveCompetence) {

            CognitiveCompetenceModel.delete({ Id: cognitiveCompetence.Id }, function (result) {

                if (result.success) {
                    $notification.success(result.message);
                    $scope.cognitiveCompetenceList.splice($scope.cognitiveCompetenceList.indexOf(cognitiveCompetence), 1);
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