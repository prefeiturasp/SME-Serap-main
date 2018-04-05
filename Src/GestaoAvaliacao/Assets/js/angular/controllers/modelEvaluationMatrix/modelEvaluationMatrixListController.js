/**
 * function EvaluationMatrix Controller
 * @namespace Controller
 * @author Alexandre Calil Blasizza Paravani 16/04/2015
 * @author Alexandre Garcia Simões 10/06/2015
 * @author Thiago Macedo Silvestre 16/09/2015
 * @author Leticia Langeli Garcia De Goes 30/10/2015
 * @author Julio Cesar da Silva 10/11/2015
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ModelEvaluationMatrixListController", ModelEvaluationMatrixListController)

    ModelEvaluationMatrixListController.$inject = ['$scope', 'ModelEvaluationMatrixModel', '$notification', '$location', '$pager', '$window'];


    function ModelEvaluationMatrixListController(ng, ModelEvaluationMatrixModel, $notification, $location, $pager, $window) {

        function configInternalObjects() {
            ng.situacaoOptions = [{Id: 1, Description: 'Ativo' }, { Id: 2, Description: 'Inativo' }];
            ng.searchField;
            ng.levelField;
            ng.listaModeloMatriz = null;
            ng.itemDeletado;
            ng.numItens = [
               { Id: 0, Description: '10' },
               { Id: 1, Description: '20' },
               { Id: 2, Description: '30' },
               { Id: 3, Description: '40' },
               { Id: 4, Description: '50' }
            ];
            ng.qntdItens;
            ng.pages = 0; 
            ng.total = 0;
            ng.paginate = $pager(ModelEvaluationMatrixModel.search);
            ng.pageSize = 10;
        };

        function ModeloMatriz() {
            this.Description = "";
            this.status = undefined;
            this.ModelSkillLevels = [];
            this.length = this.ModelSkillLevels.length;
        };

        ng.numberOnly = function () {
            ng.levelField = ng.levelField.replace(/[^0-9]/g, "");
        };

        ng.create = function () {
            $window.location.href = "/ModelEvaluationMatrix/IndexForm";
        };

        ng.edit = function (obj) {
            $window.location.href = '/ModelEvaluationMatrix/IndexForm?Id=' + obj.Id;
        };

        ng.delete = function () {

            angular.element("#modal").modal('hide');

            ModelEvaluationMatrixModel.delete(
                { Id: ng.itemDeletado.Id },
                function (result) {

                    if (result.success) {
                        ng.loadPage();
                        $notification.success(result.message);
                        ng.loadPage();

                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                }
            );
        };

        ng.callModal = function (i) {
            ng.itemDeletado = i;
            angular.element("#modal").modal({ backdrop: 'static' });
        };

        ng.loadPage = function (i) {

            if (i !== undefined) {
                ng.pages = 0;
                ng.total = 0;
                ng.paginate.indexPage(0);
                ng.pageSize = ng.paginate.getPageSize();
            }

            var obj = {};

            if (ng.searchField) {
                obj.search = ng.searchField;

            }
            else {
                obj.search = null;
            }

            if (ng.levelField)
                obj.levelQntd = ng.levelField;
            else
                obj.levelQntd = 0;

            ng.paginate.paginate(obj).then(
                
               function (result) {

                   if (result.success) {

                       if (result.lista.length > 0) {
                           ng.listaModeloMatriz = result.lista
                       }
                       else {
                           ng.listaModeloMatriz = null;
                       }

                       if (!ng.pages > 0) {
                           ng.pages = ng.paginate.totalPages();
                       }

                       ng.total = ng.paginate.totalItens();

                   } else {
                       $notification[result.type ? result.type : 'error'](result.message);
                   }
               },
               function (result) {
                   ng.listaModeloMatriz = null;
                   ng.pages = 0;
                   ng.total = 0;
                   $notification[result.type ? result.type : 'error'](result.message);
               }
            );
        };

        function loadList(result) {
            if (result.success) {
                ng.listaModeloMatriz = result.lista;
            } else {
                $notification[result.type ? result.type : 'error'](result.message);
            }

            if (!ng.listaModeloMatriz)
                ng.listaModeloMatriz = [];
        };

        $notification.clear();
        configInternalObjects();
        ng.loadPage();
    };


})(angular, jQuery);