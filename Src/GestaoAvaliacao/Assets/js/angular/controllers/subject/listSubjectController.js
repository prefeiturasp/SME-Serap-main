/**
 * function ListSubjectController Controller
 * @namespace Controller
 * @author Jessica Sartori 04/04/2017
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ListSubjectController", ListSubjectController)

    ListSubjectController.$inject = ['$scope', 'SubjectModel', '$notification', '$location', '$pager', '$window'];


    function ListSubjectController(ng, SubjectModel, $notification, $location, $pager, $window) {

        var searchResult = {}; //resultado da busca
        ng.paginate = $pager(SubjectModel.searchSubjects);

        ////ng.getSearchResult();
        

        ///**
		//* @name getSearchResult
		//* @namespace TestListController
		//* @desc GET: Realiza busca de provas de acordo com o filtro definido pelo usuário
		//* @memberOf Controller.TestListController
		//*/
        //ng.getSearchResult = function () {
        //    ng.paginate.paginate(ng.searchFieldAssunto, ng.searchFieldSubassunto)
		//	.then(function (result) {
		//	    if (result.lista.length > 0) {
		//	        ng.searchResult = result.lista;
		//	    }
		//	    else {
		//	        ng.searchResult = null;
		//	    }
		//	    ng.pages = ng.paginate.totalPages();
		//	    ng.totalItens = ng.paginate.totalItens();
		//	    if (!searchCode && searchCode == undefined) {
		//	        ng.codItem = "";
		//	    }
		//	},
		//	function (result) {
		//	    ng.searchResult = null;
		//	});
        //};

        function configInternalObjects() {
            ng.situacaoOptions = [{ Id: 1, Description: 'Ativo' }, { Id: 2, Description: 'Inativo' }];
            ng.searchFieldAssunto;
            ng.searchFieldSubassunto;
            ng.listaGrupos = null;
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
            ng.paginate = $pager(SubjectModel.searchSubjects);
            ng.pageSize = 10;
        };

        function Grupo() {
            this.Description = "";
            this.status = undefined;
            this.TestSubGroups = [];
            this.length = this.TestSubGroups.length;
        };

        ng.create = function () {
            $window.location.href = "/Subject/Form";
        };

        ng.edit = function (obj) {
            $window.location.href = '/Subject/Form?Id=' + obj.Id;
        };

        ng.delete = function () {

            angular.element("#modal").modal('hide');

            SubjectModel.delete(
                { Id: ng.itemDeletado.Id },
                function (result) {

                    if (result.success) {
                        ng.loadPage(0);
                        $notification.success(result.message);

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

            if (ng.searchFieldAssunto) {
                obj.assunto = ng.searchFieldAssunto;
            }
            else {
                obj.assunto = null;
            }

            if (ng.searchFieldSubassunto) {
                obj.subassunto = ng.searchFieldSubassunto;
            }
            else {
                obj.subassunto = null;
            }          

            ng.expandir = false;
            if (obj.assunto != null || obj.subassunto != null)
            {
                ng.expandir = true;
            }

            ng.paginate.paginate(obj).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {
                        ng.paginate.nextPage();
                        ng.searchResult = result.lista;

                        if (!ng.pages > 0) {
                            ng.pages = ng.paginate.totalPages();
                            ng.totalItens = ng.paginate.totalItens();
                        }
                    }
                    else {
                        ng.message = true;
                        ng.searchResult = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                ng.message = true;
                ng.searchResult = null;
            });

        };

        function loadList(result) {
            if (result.success) {
                ng.listaGrupos = result.lista;
            } else {
                $notification[result.type ? result.type : 'error'](result.message);
            }

            if (!ng.listaGrupos)
                ng.listaGrupos = [];
        };

        ng.edit = function (subject) {
            $window.location.href = '/Subject/Form?Id=' + subject.Id;
        };

        $notification.clear();
        configInternalObjects();
        ng.loadPage();
    };


})(angular, jQuery);