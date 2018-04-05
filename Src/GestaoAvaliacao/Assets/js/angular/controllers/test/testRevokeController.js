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
		.controller("TestRevokeController", TestRevokeController);

    TestRevokeController.$inject = ['$scope', '$window', '$pager', 'TestRevokeModel', '$notification', '$util', '$sce', '$timeout'];



    function TestRevokeController($scope, $window, $pager, TestRevokeModel, $notification, $util, $sce, $timeout) {

        $scope.formatAsHtml = function _formatAsHtml(html, size) {

            var DEFAULT_SIZE = 20;
            var _len = parseInt(size || DEFAULT_SIZE);

            var aux = $sce.trustAsHtml(html);


            if (html && html.length > _len) {
                return $sce.trustAsHtml(html.substring(0, _len).concat(' ...'));
            }
            else {
                return $sce.trustAsHtml(html);
            }
        };

        $scope.clearTagString = function (_tagstring) {

            if (_tagstring == null)
                return "";
            var _text;

            try { _text = jQuery(_tagstring).text(); } catch (e) { _text = _tagstring; }


            _text = $scope.minimize(_text, 29);

            if (_text == "")
                _text = "<somente imagem>";

            return _text;
        };

        $scope.minimize = function (_text, _length) {

            if (_text == null || _text == undefined)
                return "";

            if (_text.length > _length)
                _text = _text.substring(0, _length) + "...";

            return _text;
        };

        $scope.ItemSituation = {
            NOTREVOKED: 1,
            WAITING: 2,
            REVOKEDTEST: 3,
            REVOKED: 4,
            REFUSED: 5
        }

        $scope.init = function __init() {
            $scope.params = $util.getUrlParams();

            if ($scope.params.test_id && $scope.params.test_id.split(".").length == 1) {
                getAuthorize();
            } else {
                $scope.blockPage = true;
                $notification.alert("Id da prova inválido ou usuário sem permissão.");
                redirectToList();
                return;
            }
        };

        /**
		  * @function Redirecionar para listagem de provas.
		  * @name redirectToList
		  * @namespace TestAdministrateController
		  * @memberOf Controller
		  * @private
		  * @param
		  * @return
		  */
        function redirectToList() {
            $timeout(function __invalidTestId() {
                $window.location.href = "/Test";
            }, 3000);
        };

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
        function getAuthorize() {

            TestRevokeModel.getTestInfo($scope.params, function (result) {
                if (result.success) {
                    $scope.getTestInfo(result.dados);
                } else {
                    $scope.blockPage = true;
                    $notification[result.type ? result.type : 'error'](result.message);
                    redirectToList();
                    return;
                }
            });
        };

        $scope.getTestInfo = function _getTestInfo(dados) {

            $scope.testId = $scope.params.test_id;

            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.paginate = $pager(TestRevokeModel.getItems);
            $notification.clear();

            $scope.selectedItem = null;

            $scope.code = null;
            $scope.order = null;
            $scope.justification = null;
            $scope.testDescription = "";
            $scope.testDiscipline = "";
            $scope.itemList = {};

            $scope.testDescription = dados.testDescription;
            $scope.testDiscipline = dados.testDiscipline;
            $scope.loadPagination();
        }

        $scope.cancel = function __cancel() {
            $window.location.href = '/Test';
        };

        $scope.confirmRevoke = function _confirmRevoke(item) {
            $scope.justification = item.Justification;
            $scope.selectedItem = item;
            angular.element('#modal').modal('show');
        };

        $scope.cancelRevoke = function _cancelRevoke() {
            $scope.selectedItem = null;
        };

        $scope.search = function _search() {
            $scope.resetPages();
            $scope.loadPagination();

        };

        $scope.isItemRevoked = function _isItemRevoked(item) {
            if (item) {
                if (item.ItemSituation == $scope.ItemSituation.WAITING
                    || item.ItemSituation == $scope.ItemSituation.RECEIVED) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        $scope.infoText = function _infoText(selectedItem) {
            if (selectedItem) {
                if ($scope.isItemRevoked(selectedItem)) {
                    return "Deseja cancelar a anulação do item: " + selectedItem.ItemCode + "?"

                } else {
                    return "Deseja realmente anular o item: " + selectedItem.ItemCode + "?"
                }
            }
        }

        $scope.loadPagination = function _loadPagination() {
            $scope.paginate.paginate($scope.getFilters()).then(

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

				}, function (result) {
				    $scope.message = true;
				    $scope.itemList = null;
				});

        };

        $scope.getFilters = function _getFilters() {
            var filter = {
                TestId: $scope.testId,
                ItemCode: $scope.code != null && $scope.code != "" ? $scope.code : undefined,
                ItemOrder: $scope.order != null && $scope.order != "" ? $scope.order - 1: undefined
            }

            return filter;
        };

        $scope.revokeItem = function _revokeItem() {

            TestRevokeModel.updateItems({
                // Item_Id: $scope.selectedItem.Item_Id,
                Test_Id: $scope.testId,
                RequestRevoke_Id: $scope.selectedItem.RequestRevoke_Id,
                BlockItem_Id: $scope.selectedItem.BlockItem_Id,
                Justification: $scope.justification,
                ItemSituation: $scope.selectedItem.ItemSituation != 1 ? 1 :2
            }, function (result) {
                if (result.success) {
                    $notification.success(result.message);

                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

                $scope.resetPages();
                $scope.selectedItem = null;
                $scope.loadPagination();
                angular.element('#modal').modal('hide');

            });
        };

        $scope.resetPages = function _resetPages() {
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
        }        
    };

})(angular, jQuery);