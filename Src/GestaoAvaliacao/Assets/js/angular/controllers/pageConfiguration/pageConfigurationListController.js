/**
 * function Controller para listagem de configuração de página
 * @namespace Controller
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
		.controller("PageConfigurationListController", PageConfigurationListController);


    PageConfigurationListController.$inject = ['PageConfigurationModel', '$scope', '$rootScope', '$notification', '$pager', '$location', '$window'];


    /**
	 * @function Controller listagem de configuração da página
	 * @name PageConfigurationListController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} PageConfigurationModel
	 * @param {Object} ng
	 * @param {Object} rootScope
	 * @param {Object} $notification
	 * @param {Object} $pager
	 * @param {Object} $location
	 */
    function PageConfigurationListController(PageConfigurationModel, ng, rootScope, $notification, $pager, $location, $window) {

        /**
		 * @function - Load
		 * @param
		 * @private
		 */
        function load() {

            $notification.clear();
            ng.ordernarPor = 'Description';
            ng.inverterOrdem = false;
            ng.paginate = $pager(PageConfigurationModel.search);
            ng.pesquisa = '';
            ng.pesquisaCat = '';
            ng.pesquisaEdition = '';
            ng.message = false;
            ng.pageConfigurationList = null;
            ng.pageConfiguration = null;
            ng.pages = 0;
            ng.totalItens = 0;
            ng.pageSize = 10;
            ng.load();
        };

        /**
		 * @function - Copy Search
		 * @param
		 * @public
		 */
        ng.copySearch = function () {

            ng.fieldSearch = JSON.parse(JSON.stringify(ng.pesquisa));
            ng.fieldSearch1 = JSON.parse(JSON.stringify(ng.pesquisaCat));
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
            ng.paginate.paginate({ titulo: ng.fieldSearch, categoria: ng.fieldSearch1 }).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {

                        ng.paginate.nextPage();

                        ng.pageConfigurationList = result.lista;

                        if (!ng.pages > 0) {
                            ng.pages = ng.paginate.totalPages();
                            ng.totalItens = ng.paginate.totalItens();
                        }
                    } else {
                        ng.message = true;
                        ng.pageConfigurationList = null;
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                ng.message = true;
                ng.pageConfigurationList = null;
            });

        }

        /**
		 * @function - Confirmar exclusão
		 * @param
		 * @public
		 */
        ng.confirmar = function (pageConfiguration) {
            ng.pageConfiguration = pageConfiguration;
            angular.element('#modal').modal({ backdrop: 'static' });;
        };

        ng.edit = function (obj) {
            $window.location.href = '/PageConfiguration/Form?Id=' + obj.Id;
        };

        /**
		 * @function -Exclusão
		 * @param
		 * @public
		 */
        ng.delete = function (pageConfiguration) {

            PageConfigurationModel.delete({ Id: pageConfiguration.Id }, function (result) {
                if (result.success) {

                    ng.copySearch();

                    $notification.success(result.message);

                    ng.pageConfigurationList.splice(ng.pageConfigurationList.indexOf(pageConfiguration), 1);

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modal').modal('hide');
        }

        //inicialização
        load();
    };


})(angular, jQuery);