/**
 * function ReportAnswerSheetFilesController Controller
 * @namespace Controller
 * @author Everton Ferreira - since: 08/09/2016
 * @author julio.silva@mstech.com.br - since: 11/10/2016
 */
(function (angular, $) {

    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
		.module('appMain')
		.controller("ReportAnswerSheetFilesController", ReportAnswerSheetFilesController);

    ReportAnswerSheetFilesController.$inject = ['$rootScope', '$scope', 'ReportAnswerSheetModel', 'AdherenceModel', 'FileModel', 'TestModel', '$notification', '$timeout', '$pager', '$window', '$util'];

    function ReportAnswerSheetFilesController($rootScope, $scope, ReportAnswerSheetModel, AdherenceModel, FileModel, TestModel, $notification, $timeout, $pager, $window, $util) {

        /**
		 * @function init
		 * @param 
		 * @returns
		 */
        function Init() {

            $scope.params = $util.getUrlParams();
            
            if ($scope.params.FilterDateUpdate)
                $scope.params.FilterDateUpdate = Boolean($scope.params.FilterDateUpdate == "true");

            if (!$scope.params.school_id || !$scope.params.startDate || !$scope.params.endDate || $scope.params.FilterDateUpdate == undefined) {
                $notification.alert("URL inválida.");
                $timeout(function () {
                    window.history.back();
                }, 3000);
                return;
            }

            configVariables();
        };

        /**
		 * @function Responsavel por instanciar tds as variaveis
		 * @param 
		 * @returns
		 */
        function configVariables() {
            // :paginação
            $scope.paginate = $pager(ReportAnswerSheetModel.getFollowUpIdentificationFiles);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = [];
            // :vars
            $scope.listModal = [
               { Description: "Baixar todos os arquivos enviados", state: 0 },
               { Description: "Todos os arquivos com  a situação", state: 2 },
               { Description: "Todos os arquivos com  a situação", state: 3 },
               { Description: "Todos os arquivos com  a situação", state: 4 }
            ];
            $scope.filters = {
                DRE: "",
                School: ""
            };
            $scope.info = {
                School: "",
                DRE: ""
            };
            $scope.listFilter = {
                DREs: "",
                School: "",
                Processing: []
            };
            getSituations();
            var parameterBatchFile = getParameterValue(parameterKeys[0].DELETE_BATCH_FILES);
            $scope.batchFile = parameterBatchFile ? $.parseJSON(parameterBatchFile.toLowerCase()) : false;
            $scope.search();

            $scope.$watch("listFilter.Processing", function () {
                $scope.countFilter = 0;
                for (var a = 0; a < $scope.listFilter.Processing.length; a++)
                    if ($scope.listFilter.Processing[a].state)
                        $scope.countFilter += 1;
            }, true);
        };

        /**
		 * @function Obter todas Situações
		 * @param {object} _callback
		 * @returns
		 */
        function getSituations(_callback) {
            ReportAnswerSheetModel.getSituationList(function (result) {
                if (result.success) {
                    $scope.listFilter.Processing = result.lista;
                    $scope.clearFilters();
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
         * @function Limpar filtros
         * @param 
         * @returns
         */
        $scope.clearFilters = function __clearFilters() {
            for (var a = 0; a < $scope.listFilter.Processing.length; a++)
                $scope.listFilter.Processing[a].state = false;
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
            $scope.Paginate();
            getReportInfo();
        };

        /**
         * @function Obter filtros
         * @param
         * @returns
         */
        function getFilters() {
            var processing = [];
            for (var a = 0; a < $scope.listFilter.Processing.length; a++)
                if ($scope.listFilter.Processing[a].state)
                    processing.push($scope.listFilter.Processing[a].Id);
            $scope.searcheableFilter = {
                SupAdmUnitId: $scope.params.dre_id,
                SchoolId: $scope.params.school_id,
                StartDate: $scope.params.startDate,
                EndDate: $scope.params.endDate,
                Processing: processing.toString(),
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
                        $scope.listResult = angular.copy(result.lista);
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
         * @function Download de arquivo
         * @param 
         * @returns
         */
        $scope.downloadFile = function __downloadFile(id) {
            FileModel.checkFileExists({ Id: id }, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'alert'](result.message);
                }
            });
        };

        /**
         * @function Download de arquivo
         * @param 
         * @returns
         */
        $scope.downloadAllFiles = function __downloadAllFiles(type) {
            var filter = {
                SupAdmUnitId: $scope.params.dre_id,
                SchoolId: $scope.params.school_id,
                StartDate: $scope.params.startDate,
                EndDate: $scope.params.endDate,
                Type: type,
                FilterDateUpdate: $scope.params.FilterDateUpdate
            };

            ReportAnswerSheetModel.downloadZipFiles(filter, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Obter detalhes do cabeçalho
         * @author leticia langeli 
         * @since 13/10/2016
         * @param 
         * @returns
         */
        function getReportInfo() {
            var filter = {
                SupAdmUnitId: !$scope.params.dre_id ? undefined : $scope.params.dre_id,
                SchoolId: !$scope.params.school_id ? undefined : $scope.params.school_id,
            };
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
                    $timeout(function () {
                        window.location.href = "/ReportAnswerSheet/FollowUpIdentification";
                    }, 3000);
                }
            });
        };

        /**
         * @function Voltar para tela anterior
         * @param 
         * @return
         */
        $scope.prevRedirect = function __prevRedirect() {
            window.history.back();
        };

        /**
		 * @function Gera o relatório de processamento da correção
		 * @param 
		 * @return
		 */
        $scope.generateReport = function __generateReport() {

            var processing = [];
            for (var a = 0; a < $scope.listFilter.Processing.length; a++)
                if ($scope.listFilter.Processing[a].state)
                    processing.push($scope.listFilter.Processing[a].Id);

            var filter = {
                SupAdmUnitId: $scope.params.dre_id,
                SchoolId: $scope.params.school_id,
                StartDate: $scope.params.startDate,
                EndDate: $scope.params.endDate,
                View: 4,
                Processing: processing.toString(),
                FilterDateUpdate: $scope.params.FilterDateUpdate
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

        /**
		 * @function Acionar datepicker por btn
		 * @param 
		 * @return
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