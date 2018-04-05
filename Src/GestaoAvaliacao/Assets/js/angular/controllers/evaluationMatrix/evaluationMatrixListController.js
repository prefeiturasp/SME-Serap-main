/**
 * function Controller para listagem de matriz de avaliação
 * @namespace Controller
 * @author Thiago Macedo Silvestre 02/03/2015
 * @author Alexandre Calil Blasizza Paravani 04/03/2015
 * @author Alexandre Garcia Simõe 09/06/2015
 * @author Luis Henrique Pupo Maron 25/09/2015
 * @author Leticia Langeli Garcia De Goes 28/09/2015
 * @author Everton Luis Ferreira 14/10/2015
 * @author Julio cesar da silva - 28/10/2015
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
		.controller("EvaluationMatrixListController", EvaluationMatrixListController);
		

    EvaluationMatrixListController.$inject = ['EvaluationMatrixModel', '$scope', '$rootScope', '$notification', '$pager', '$location', '$window'];
    

    /**
	 * @function Controller listagem de Matrizes de avaliação
	 * @name EvaluationMatrixListController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} EvaluationMatrixModel
	 * @param {Object} ng
	 * @param {Object} rootScope
	 * @param {Object} $notification
	 * @param {Object} $pager
	 * @param {Object} $location
	 */
    function EvaluationMatrixListController(EvaluationMatrixModel, ng, rootScope, $notification, $pager, $location, $window) {

        /**
		 * @function - Load
		 * @param
		 * @private
		 */
        function load( ) {

            $notification.clear();
            ng.ordernarPor = 'Description';
            ng.inverterOrdem = false;
            ng.paginate = $pager(EvaluationMatrixModel.search);
            ng.pesquisa = '';
            ng.pesquisaEdition = '';
            ng.message = false;
            ng.evaluationMatrixList = null;
            ng.evaluationMatrix = null;
            ng.pages = 0;
            ng.totalItens = 0;
            ng.pageSize = 10;
            ng.listCognitiveCompetence = null;
            ng.load();
        };

        /**
		 * @function - Copy Search
		 * @param
		 * @public
		 */
        ng.copySearch = function () {

            ng.fieldSearch = JSON.parse(JSON.stringify(ng.pesquisa));
            ng.fieldSearch1 = JSON.parse(JSON.stringify(ng.pesquisaEdition));
            ng.paginate.indexPage(0);
            ng.pages = 0;
            ng.totalItens = 0;
            ng.pageSize = ng.paginate.getPageSize();
            ng.load();
        };

        /**
		 * @function - Load
		 * @param
		 * @public
		 */
        ng.load = function () {

            ng.paginate.paginate({ search: ng.fieldSearch, search1: ng.fieldSearch1 }).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {

                        ng.paginate.nextPage();

                        ng.evaluationMatrixList = result.lista;

                        if (!ng.pages > 0) {
                            ng.pages = ng.paginate.totalPages();
                            ng.totalItens = ng.paginate.totalItens();
                        }
                    } else {
                        ng.message = true;
                        ng.evaluationMatrixList = null;
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                ng.message = true;
                ng.evaluationMatrixList = null;
            });

        }

        /**
		 * @function - Confirmar exclusão
		 * @param
		 * @public
		 */
        ng.confirmar = function (evaluationMatrix) {
            ng.evaluationMatrix = evaluationMatrix;
            angular.element('#modal').modal({ backdrop: 'static' });;
        };

        /**
		 * @function -Exclusão
		 * @param
		 * @public
		 */
        ng.delete = function (evaluationMatrix) {

            EvaluationMatrixModel.delete({ Id: evaluationMatrix.Id }, function (result) {
                if (result.success) {

                    ng.copySearch();

                    $notification.success(result.message);

                    ng.evaluationMatrixList.splice(ng.evaluationMatrixList.indexOf(evaluationMatrix), 1);

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modal').modal('hide');
        }

        /**
		 * @function Redireciona para pagina 2
		 * @param
		 * @public
		 */
        ng.go = function (id) {
            $window.location.href = '/EvaluationMatrix/IndexForm?Id=' + id + "&navigation=2"
        };

        //inicialização
        load();
    };


})(angular, jQuery);