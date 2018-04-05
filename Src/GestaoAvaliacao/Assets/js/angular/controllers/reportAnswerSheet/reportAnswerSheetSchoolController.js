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
		.controller("ReportAnswerSheetSchoolController", ReportAnswerSheetSchoolController);

    ReportAnswerSheetSchoolController.$inject = ['$rootScope', '$scope', 'ReportAnswerSheetModel', 'AdherenceModel', '$notification', '$timeout', '$pager', '$window', '$util'];

    function ReportAnswerSheetSchoolController($rootScope, $scope, ReportAnswerSheetModel, AdherenceModel, $notification, $timeout, $pager, $window, $util) {

        /**
		 * @function init
		 * @param 
		 * @return
		 */
        function Init() {

            $scope.params = $util.getUrlParams();

            if ($scope.params.FilterDateUpdate)
                $scope.params.FilterDateUpdate = Boolean($scope.params.FilterDateUpdate == "true");

            if (!$scope.params.startDate || !$scope.params.endDate) {
                $scope.hasDateFilter = true;
                $scope.params.FilterDateUpdate = false;
                //var startDate = new Date();
                //startDate = new Date().setDate(startDate.getDate() - 7)
                //$scope.params.startDate = moment(startDate).format("YYYY/MM/DD");
                //$scope.params.endDate = moment(new Date()).format("YYYY/MM/DD");
                var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

                var startDate = new Date();
                startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
                $("#dateStart").datepicker("setDate", startDate);

                var endDate = new Date();
                endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
                $("#dateEnd").datepicker("setDate", endDate);
            }

            configVariables();
        };

        /**
		* @function Responsavel por instanciar tds as variaveis
		* @name configVariables
		* @namespace ReportAnswerSheetSchoolController
		* @memberOf Controller
		* @private
		* @param 
		* @return
		*/
        function configVariables() {
            $scope.countFilter = 0;
            // :paginação
            $scope.paginate = $pager(ReportAnswerSheetModel.getFollowUpIdentificationSchool);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = [];
            // :vars
            $scope.filters = {
                School: "",
                DREId: $scope.params.dre_id ? $scope.params.dre_id : ""
            };
            $scope.info = {
                School: "",
                DRE: ""
            };
            $scope.listFilter = {
                DREs: [],
                Schools: []
            };
            getDRES(function () {
                if($scope.filters.DREId)
                    getSchools();
                $scope.search();
            }.bind(this));

            $scope.$watchCollection("[params.FilterDateUpdate, params.startDate, params.endDate, filters.DREId, filters.School]", function () {
                $scope.countFilter = 0;
                if ($scope.hasDateFilter) $scope.countFilter = 1;
                if ($scope.params.startDate && $scope.hasDateFilter) $scope.countFilter += 1;
                if ($scope.params.endDate && $scope.hasDateFilter) $scope.countFilter += 1;
                if ($scope.filters.DREId) $scope.countFilter += 1;
                if ($scope.filters.School) $scope.countFilter += 1;
            }, true);
        };

        /**
		 * @function Obter todas Escolas
		 * @param {object} _callback
		 * @return
		 */
        function getDRES(_callback) {
            AdherenceModel.getDRESimple(function (result) {
                if (result.success) {
                    $scope.listFilter.DREs = result.lista;
                    if ($scope.params.dre_id) {
                        for (var i = 0; i < $scope.listFilter.DREs.length; i++)
                            if ($scope.listFilter.DREs[i].Id == $scope.params.dre_id) {
                                $scope.filters.DREId = $scope.listFilter.DREs[i].Id;
                            }
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
		 * @function Obter todas Escolas dada uma dre
		 * @param {object} _callback
		 * @return
		 */
        function getSchools(_callback) {
            if (!$scope.filters.DREId) return;

            AdherenceModel.getSchoolsSimple({ dre_id: $scope.filters.DREId }, function (result) {
                if (result.success) {
                    $scope.listFilter.Schools = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };
        $scope.getSchools = getSchools;

        /**
         * @function Limpar filtros
         * @param 
         * @returns
         */
        $scope.clearFilters = function __clearFilters() {
            if ($scope.listFilter.DREs && $scope.listFilter.DREs.length > 1) {
                $scope.filters.DREId = "";
            }
            $scope.filters.School = "";
        };

        /**
         * @function Validar período de datas
         * @param 
         * @returns
         */
        function validateDate() {
            if (!$scope.params.startDate || !$scope.params.endDate) {
                $notification.alert("É necessário selecionar um período.");
                return false;
            }
            if ($util.greaterEndDateThanStartDate($scope.params.startDate, $scope.params.endDate) === false) {
                $notification.alert("Data inicial não pode ser maior que a data final.");
                $scope.params.endDate = "";
                return false;
            }
            return true;
        };


        /**
         * @function Limpar paginação
         * @param 
         * @returns
         */
        $scope.search = function __search() {
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            getFilters();
            if ($scope.hasDateFilter)
                if (!validateDate()) return;
            $scope.Paginate();
            getReportInfo();
        };

        /**
        * @function Obter filtros
        * @param
        * @returns
        */
        function getFilters() {
            $scope.searcheableFilter = {
                SupAdmUnitId: !$scope.filters.DREId ? undefined : $scope.filters.DREId,
                SchoolId: !$scope.filters.School ? undefined : $scope.filters.School.Id,
                StartDate: $scope.params.startDate,
                EndDate: $scope.params.endDate,
                FilterDateUpdate: $scope.params.FilterDateUpdate
            };
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
         * @function Voltar para tela anterior
         * @param 
         * @returns
         */
        $scope.prevRedirect = function __prevRedirect() {
            window.history.back();
        };

        /**
		 * @function Gera o relatório de processamento da correção
		 * @param 
		 * @returns
		 */
        $scope.generateReport = function __generateReport() {
            var filter = {
                SupAdmUnitId: !$scope.filters.DREId ? undefined : $scope.filters.DREId,
                SchoolId: !$scope.filters.School ? undefined : $scope.filters.School.Id,
                StartDate: $scope.params.startDate,
                EndDate: $scope.params.endDate,
                View: 3
            };

            ReportAnswerSheetModel.exportFollowUpIdentification(filter, function (result) {
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
                SupAdmUnitId: !$scope.filters.DREId ? undefined : $scope.filters.DREId,
                SchoolId: !$scope.filters.School ? undefined : $scope.filters.School.Id,
            };

            $scope.info.DRE = "";
            $scope.info.School = "";

            ReportAnswerSheetModel.getIdentificationReportInfo(filter, function (result) {
                if (result.success) {
                    if (result.entity && result.entity.SupAdmUnitName) {
                        $scope.info.DRE = {
                            Id: result.entity.SupAdmUnit_Id,
                            Description: result.entity.SupAdmUnitName
                        };
                    }
                    if (result.entity && result.entity.SchoolName) {
                        $scope.info.School = {
                            Id: result.entity.SchoolId,
                            Description: result.entity.SchoolName
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
            Init();
        })();
    };

})(angular, jQuery);