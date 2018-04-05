/**
 * function ListTestGroupController Controller
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
        .controller("ListTestGroupController", ListTestGroupController)

    ListTestGroupController.$inject = ['$scope', 'TestGroupModel', '$notification', '$location', '$pager', '$window'];


    function ListTestGroupController(ng, TestGroupModel, $notification, $location, $pager, $window) {

        function configInternalObjects() {
            ng.situacaoOptions = [{ Id: 1, Description: 'Ativo' }, { Id: 2, Description: 'Inativo' }];
            ng.searchField;
            //ng.levelField;
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
            ng.paginate = $pager(TestGroupModel.search);
            ng.pageSize = 10;
        };

        function Grupo() {
            this.Description = "";
            this.status = undefined;
            this.TestSubGroups = [];
            this.length = this.TestSubGroups.length;
        };

        //ng.numberOnly = function () {
        //    ng.levelField = ng.levelField.replace(/[^0-9]/g, "");
        //};

        ng.create = function () {
            $window.location.href = "/TestGroup/Form";
        };

        ng.edit = function (obj) {
            $window.location.href = '/TestGroup/Form?Id=' + obj.Id;
        };

        ng.delete = function () {

            angular.element("#modal").modal('hide');

            TestGroupModel.delete(
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
                           ng.listaGrupos = result.lista
                       }
                       else {
                           ng.listaGrupos = null;
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
                   ng.listaGrupos = null;
                   ng.pages = 0;
                   ng.total = 0;
                   $notification[result.type ? result.type : 'error'](result.message);
               }
            );
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

        $notification.clear();
        configInternalObjects();
        ng.loadPage();
    };


})(angular, jQuery);