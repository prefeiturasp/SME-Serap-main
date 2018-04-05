/**
 * function ReportAnswerSheet Controller
 * @namespace Controller
 * @author julio.silva@mstech.com.br - since: 11/10/2016
 */
(function (angular, $) {

    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
		.module('appMain')
		.controller("ReportAnswerSheetDREController", ReportAnswerSheetDREController);

    ReportAnswerSheetDREController.$inject = ['$rootScope', '$scope', 'ReportAnswerSheetModel', 'AdherenceModel', '$notification', '$timeout', '$pager', '$window', '$util'];

    function ReportAnswerSheetDREController($rootScope, $scope, ReportAnswerSheetModel, AdherenceModel, $notification, $timeout, $pager, $window, $util, $filter) {

        /**
		* @function Responsavel por instanciar tds as variaveis
		* @param 
		 * @returns
		*/
        function configVariables() {
            $scope.countFilter = 1;
            // :paginação
            $scope.paginate = $pager(ReportAnswerSheetModel.getFollowUpIdentificationDRE);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = [];
            // :vars
            $scope.filters = angular.extend({
                SupAdmUnitId: "",
                FilterDateUpdate: false
            },
            $scope.filters);

            $scope.searcheableFilter = angular.copy($scope.filters);
            $scope.listFilter = {
                DREs: ""
            };
            $scope.info = {
                DRE: ""
            };
            getDRES();
            $scope.search();


            $scope.$watch("filters", function () {
                $scope.countFilter = 1;
                if ($scope.filters.StartDate) $scope.countFilter += 1;
                if ($scope.filters.EndDate) $scope.countFilter += 1;
                if ($scope.filters.SupAdmUnitId) $scope.countFilter += 1;
            }, true);
        };

        /**
         * @function Obter DREs para carregamento de combo
         * @param 
         * @returns
         */
        function getDRES() {
            AdherenceModel.getDRESimple(function (result) {
                if (result.success) {
                    $scope.listFilter.DREs = result.lista;
                }else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Validar período de datas
         * @param 
         * @returns
         */
        function validateDate() {
            if (!$scope.filters.StartDate || !$scope.filters.EndDate) {
                $notification.alert("É necessário selecionar um período.");
                return false;
            }
            if ($util.greaterEndDateThanStartDate($scope.filters.StartDate, $scope.filters.EndDate) === false) {
                $notification.alert("Data inicial não pode ser maior que a data final.");
                $scope.filters.EndDate = "";
                return false;
            }
            return true;
        };

        /**
         * @function Limpar os filtros
         * @param 
         * @returns
         */
        $scope.clearFilters = function __clearFilters() {
            $scope.filters.SupAdmUnitId = "";
            $scope.filters.StartDate = "";
            $scope.filters.EndDate = "";
            $scope.filters.FilterDateUpdate = false;
        };

        /**
         * @function Aplicar os filtros
         * @param 
         * @returns
         */
        $scope.search = function __search() {
            if (!validateDate()) return;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.searcheableFilter = angular.copy($scope.filters);
            $scope.Paginate();
            getReportInfo();
        };

        /**
		* @function Responsavel pela paginação
		* @param 
		 * @returns
		*/
        $scope.Paginate = function Paginate() {
            $scope.paginate.paginate($scope.searcheableFilter).then(function (result) {
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.listResult = result.lista;
                        $scope.totalFiles = result.totalFiles;
                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.listResult = null;
                    }
                    $scope.FilterDateUpdate = result.FilterDateUpdate;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                $scope.message = true;
                $scope.listResult = null;
            });
        };

        /**
		* @function Gera o relatório de processamento da correção
		* @param 
		 * @returns
		*/
        $scope.generateReport = function __generateReport() {
            $scope.filters.View = 2;

            ReportAnswerSheetModel.exportFollowUpIdentification($scope.filters, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function getReportInfo() {
            var filter = {
                SupAdmUnitId: !$scope.filters.SupAdmUnitId ? undefined : $scope.filters.SupAdmUnitId,
            };

            ReportAnswerSheetModel.getIdentificationReportInfo(filter, function (result) {
                if (result.success) {
                    if (result.entity && result.entity.SupAdmUnitName) {
                        $scope.info.DRE = {
                            Id: result.entity.SupAdmUnit_Id,
                            Description: result.entity.SupAdmUnitName
                        };
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		 * @function Acionar datapicker por btn
		 * @param 
		 * @returns
		*/
        $scope.datepicker = function __datepicker(id) { $("#" + id).datepicker('show'); };

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
            var element_in_painel = false;
            if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('next') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('day') && e.target.tagName === "TD") ||
	            $(e.target).parent().is("[data-side-filters]") ||
                e.target.hasAttribute('data-side-filters'))
                return;

            if (angular.element(".side-filters").hasClass("side-filters-animation")) $scope.open();
        };

        /**
         * @function Iniciar
		 * @param
         * @returns
		 */
        (function __init() {
            $notification.clear();
            angular.element('body').click(close);
            $(document).ready(function () {  
                var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

                var startDate = new Date();
                startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
                $("#dateStart").datepicker("setDate", startDate);

                var endDate = new Date();
                endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
                $("#dateEnd").datepicker("setDate", endDate);

                configVariables();
            });
        })();
    };

})(angular, jQuery);