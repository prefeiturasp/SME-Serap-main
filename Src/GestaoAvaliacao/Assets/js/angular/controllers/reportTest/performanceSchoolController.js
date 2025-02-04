﻿/**
 * @function Performance School Controller
 * @author Julio Cesar Silva - 18/04/2016
 */
(function (angular, $) {

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
		.module('appMain')
		.controller("PerformanceSchoolController", PerformanceSchoolController);


    PerformanceSchoolController.$inject = ['$scope', 'ReportTestModel', '$notification', '$pager', '$util'];

    /**
     * @function Controller para relatório de desempenho de prova por escola
     * @param {Object} $scope
     * @param {Object} ReportTestModel
     * @param {Object} $notification
     * @param {Object} $pager
     * @param {Object} $util
     * @return
     */
    function PerformanceSchoolController($scope, ReportTestModel, $notification, $pager, $util) {

        //parâmetros enviados via url
        var params = $util.getUrlParams();

        /**
         * @description config. variaveis
         * @param
         * @returns
         */
        function configVariaveis() {
            $scope.countFilter = 1;
            // :paginação
            $scope.paginate = $pager(ReportTestModel.getReportBySchoolPerformance);
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.paginate.indexPage(0);
            // :modelos
            $scope.filters = {};
            $scope.filters.global = true;
            $scope.schools = [];
            $scope.$watch('filters', function () {
                $scope.countFilter = 1;
                if ($scope.filters.dateStart) $scope.countFilter += 1;
                if ($scope.filters.dateEnd) $scope.countFilter += 1;
                if ($scope.filters.DRE) $scope.countFilter += 1;
                if ($scope.filters.School) $scope.countFilter += 1;
                if ($scope.filters.Test) $scope.countFilter += 1;
                if ($scope.filters.Year) $scope.countFilter += 1;
                if ($scope.filters.Class) $scope.countFilter += 1;
            }, true);
        };

        /**
         * @description Exportar planilha (arquivo) de relatório
         * @param
         * @returns
         */
        $scope.exportFile = function _exportFile() {

        };

        /**
         * @description Pesquisa paginada
         * @param
         * @returns
         */
        $scope.search = function _search() {

            var params = {
                TestId: ($scope.filters.Test != undefined && $scope.filters.Test != null) ? $scope.filters.Test.Id : 0,
                Year: ($scope.filters.Year != undefined && $scope.filters.Year != null) ? $scope.filters.Year.Id : '',
                uad_id: ($scope.filters.DRE != undefined && $scope.filters.DRE != null) ? $scope.filters.DRE.Id : null,
                esc_id: ($scope.filters.School != undefined && $scope.filters.School != null) ? $scope.filters.School.Id : 0,
                tur_id: ($scope.filters.Section != undefined && $scope.filters.Section != null) ? $scope.filters.Section.Id : 0
            };

            $scope.paginate.paginate(params).then(function (result) {

                if (result.success) {

                    if (result.lista !== undefined) {

                        $scope.paginate.nextPage();
                        $scope.schools = result.lista.Schools;
                        $scope.AverageDre = result.lista.AverageDre;
                        $scope.AverageGrid = result.lista.AverageGrid;

                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    }
                    else {
                        $scope.schools = [];
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    $scope.schools = [];
                }
            });
        };

        /**
         * @description Limpar filtros de pesquisa
         * @param
         * @returns
         */
        $scope.clearFields = function _clearFields() {
            $scope.filters.dateStart = "";
            $scope.filters.dateEnd = "";
            $scope.filters.DRE = "";
            $scope.filters.School = "";
            $scope.filters.Test = "";
            $scope.filters.Year = "";
            $scope.filters.Class = "";
            $scope.filters.global = true;
        };

        /**
         * @description Abrir/fechar painel de filtros
         * @param
         * @returns
         */
        $scope.open = function __open() {

            $('.side-filters').toggleClass('side-filters-animation').promise().done(function a() {

                // :abrir painel
                if (angular.element(".side-filters").hasClass("side-filters-animation")) {
                    angular.element('body').css('overflow', 'hidden');
                }
                // :fechar painel
                else {
                    angular.element('body').css('overflow', 'inherit');
                }
            });
        };

        /**
         * @description Fechar painel de filtros por click da page
         * @param
         * @returns
         */
        function close(e) {

            var element_in_painel = false;

            // :não realizar escape se (elementos do datepicker presentes no painel) || element possuir a marcação
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
		 * @description Iniciar
		 * @param
		 * @returns
		 */
        (function __init() {
            $notification.clear();
            configVariaveis();
            angular.element('body').click(close);
            $scope.search();
        })();
    };

})(angular, jQuery);