/**
 * function TestRevokeController Controller
 * @namespace Controller
 * @author Lucas Rodrigues
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
		.controller("TestRequestRevokeController", TestRequestRevokeController);

    TestRequestRevokeController.$inject = ['$scope', '$window', '$pager','TestRequestRevokeModel', 'ItemModel', '$notification', '$util', '$timeout'];

    function TestRequestRevokeController($scope, $window, $pager, TestRequestRevokeModel, ItemModel, $notification, $util, $timeout) {

        $scope.countFilter = 0;
        $scope.itemList = [];
        $scope.Situation = { value: undefined };
        $scope.ListSituation = [
            { Description: "Aguardando",           value: 2 },
            { Description: "Anulado Prova",        value: 3 },
            { Description: "Anulado Prova e Item", value: 4 },
            { Description: "Recusado",             value: 5 },
            { Description: "Tudo", value: undefined }
        ];
        $scope.selectedItem = {};
        $scope.paginate = $pager(TestRequestRevokeModel.GetPendingRevokeItems);
        $scope.pages = 0;
        $scope.totalItens = 0;
        $scope.pageSize = 10;
        $notification.clear();   
        $scope.$watchCollection("[StartDate, EndDate, Situation.value]", function () {
            $scope.countFilter = 0;
            if ($scope.StartDate) $scope.countFilter += 1;
            if ($scope.EndDate) $scope.countFilter += 1;
            if ($scope.Situation.value) $scope.countFilter += 1;
        });
        $timeout(function () {
            $scope.countFilter = 1;
        }, 0);

        $scope.search = function _search(searchCode) {
            $scope.resetPages();
            $scope.setFilters();
            $scope.loadPagination(searchCode);
        };         

        $scope.resetPages = function _resetPages() {
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
        };

        $scope.loadPagination = function _loadPagination(searchCode) {            
            if ($scope.dateValidation()) {
                $scope.paginate.paginate(searchCode ? { ItemCode: $scope.ItemCode } : $scope.currentFilters).then(
                    function (result) {
                        if (result.lista.length > 0) {
                            $scope.paginate.nextPage();
                            $scope.itemList = result.lista;
                            if (!$scope.pages > 0) {
                                $scope.pages = $scope.paginate.totalPages();
                                $scope.totalItens = $scope.paginate.totalItens();
                            }
                        }
                        else {
                            $scope.message = true;
                            $scope.itemList = null;
                        }
                        if (!searchCode) $scope.ItemCode = "";
                    }, function (result) {
                        $scope.message = true;
                        $scope.itemList = null;
                    });
            }
            else {
                $notification.alert("A data de término não pode ser maior que a data de início.")
            }
        };

        /**
         * Validação das datas: Início / Fim
         **/
        $scope.dateValidation = function _dateValidation() {
            var test = new Date($scope.StartDate) > new Date($scope.EndDate) ? false : true;
            return test;
        };

        /**
         * Validação dos parametros/filtros de pesquisa
         **/
        $scope.setFilters = function _setFilters() {
            $scope.currentFilters = {
                StartDate: $scope.StartDate != null && $scope.StartDate != "" ? $scope.StartDate : undefined,
                EndDate: $scope.EndDate     != null && $scope.EndDate   != "" ? $scope.EndDate   : undefined,
                Situation: $scope.Situation != null && $scope.Situation != "" ? $scope.Situation.value : undefined
            }
        };

        /**
         * Validação dos parametros/filtros de pesquisa
         **/
        $scope.clearFilters = function _clearFilters() {
            $scope.StartDate = null;
            $scope.EndDate = null;
            $scope.ModelSituation = null;           
            $scope.countFilter = 0;
        };

        /**
         * Exibição da janela modal
         **/
        $scope.openModal = function _openModal(bodyModal, item) {

            $scope.modalBody = bodyModal;
            $scope.selectedItem = item;

            angular.element("#modalRequestRevoke").modal("show");

            if (bodyModal == 'solicitacoesItem') {

                getRequestRevokes();
            }
        };

        /**
         * @function Consulta lista de solicitantes da anulação
         **/
        function getRequestRevokes() {

            TestRequestRevokeModel.getRequestRevokes({ blockItem_Id: $scope.selectedItem.BlockItem_Id }, function (result) {

                if (result.success) {

                    $scope.requestRevokeList = result.lista;

                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

       /**
       * @function Anula/Recusa item
       **/
        $scope.updateRequestRevokedByTestBlockItem = function _updateRequestRevokedByTestBlockItem(item_recusado) {

            var params = {
                Item_Id: $scope.selectedItem.Item_Id,
                BlockItem_Id: $scope.selectedItem.BlockItem_Id,
                Test_Id: $scope.selectedItem.Test_Id,
                Situation: item_recusado ? 5 : 3
            };

            TestRequestRevokeModel.updateRequestRevokedByTestBlockItem(params, function (result) {

                if (result.success) {
                    $scope.selectedItem.Situation = item_recusado ? 5 : 3;
                    $scope.selectedItem.LabelSituation = result.message;

                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		* @function Anula prova e item
		**/
        $scope.UpdateRevokeItem = function _UpdateRevokeItem(desfazer_anulacao) {

            var params = {
                Item_Id: $scope.selectedItem.Item_Id,
                BlockItem_Id: $scope.selectedItem.BlockItem_Id,
                Test_Id: $scope.selectedItem.Test_Id,
                Revoke: desfazer_anulacao ? false : true,
                ItemSituation: desfazer_anulacao ? 2 : 4

            };

            TestRequestRevokeModel.revokeItem(params, function (result) {

                if (result.success) {

                    $scope.selectedItem.Situation = desfazer_anulacao ? 2 : 4;
                    $scope.selectedItem.LabelSituation = result.message;

                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace FileController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.safeApply = function __safeApply() {
            var $scope, fn, force = false;
            if (arguments.length === 1) {
                var arg = arguments[0];
                if (typeof arg === 'function') {
                    fn = arg;
                } else {
                    $scope = arg;
                }
            } else {
                $scope = arguments[0];
                fn = arguments[1];
                if (arguments.length === 3) {
                    force = !!arguments[2];
                }
            }
            $scope = $scope || this;
            fn = fn || function () { };

            if (force || !$scope.$$phase) {
                $scope.$apply ? $scope.$apply(fn) : $scope.apply(fn);
            } else {
                fn();
            }
        };

        /**
		 * @description Forçar datepicker por trigger em botão
		 * @param
		 * @returns
		 */
        $scope.datepicker = function __datepicker(id) {

            angular.element("#" + id).datepicker('show');
        };

        /**
         * @description Agrupamento de ações referentes a um item (menu padrão para várias ações)
         * @author Julio Cesar Silva 11/04/2016
         */
        $scope.popover = {
            item: undefined,
            set: function $set(_item) {
                $scope.popover.item = _item;
            },
            openModal: function $openModal(bodyModal, item) {
                $scope.openModal(bodyModal, item);
            }
        };

        /**
         * @function Abrir/fechar painel de filtros
         * @param
         * @returns
         */
        $scope.open = function __open() {

            $('.side-filters').toggleClass('side-filters-animation').promise().done(function a() {

                if (angular.element(".side-filters").hasClass("side-filters-animation")) {
                    angular.element('body').css('overflow', 'hidden');
                }
                else {
                    angular.element('body').css('overflow', 'inherit');
                }
            });
        };

        /**
         * @function Fechar painel de filtros por click da page
		 * @param
         * @returns
		 */
        function close(e) {

            if ($(e.target).parent().hasClass('.side-filters') || $(e.target).parent().hasClass('tag-item') || $(e.target).parent().hasClass('tags'))
                return;

            var element_in_painel = false;
            if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('next') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('day') && e.target.tagName === "TD") ||
                $(e.target).hasClass('tag-item') || $(e.target).hasClass('tags') ||
	            $(e.target).parent().is("[data-side-filters]") ||
                e.target.hasAttribute('data-side-filters'))
                return;

            if (angular.element(".side-filters").hasClass("side-filters-animation")) $scope.open();
        }; angular.element('body').click(close);

        $scope.loadPagination();
    };
    

})(angular, jQuery);