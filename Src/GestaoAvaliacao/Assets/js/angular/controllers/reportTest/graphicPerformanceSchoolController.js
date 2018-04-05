/**
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
		.controller("GraphicPerformanceSchoolController", GraphicPerformanceSchoolController);


    GraphicPerformanceSchoolController.$inject = ['$scope', 'ReportTestModel', '$notification', '$pager', '$util'];

    /**
     * @function Controller para relatório de desempenho de prova por escola
     * @param {Object} $scope
     * @param {Object} GraphicPerformanceSchoolController
     * @param {Object} $notification
     * @param {Object} $pager
     * @param {Object} $util
     * @return
     */
    function GraphicPerformanceSchoolController($scope, ReportTestModel, $notification, $pager, $util) {

        //parâmetros enviados via url
        var params = $util.getUrlParams();

        /**
         * @description config. variaveis
         * @param
         * @returns
         */
        function configVariaveis() {

            // :paginação
            $scope.paginate = $pager(ReportTestModel.getReportBySchoolPerformance);
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            // :modelos
            $scope.filters = {};
            $scope.filters.global = true;
            $scope.schools = [];
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

                        //formatar gráfico
                        transformToGraphics($scope.schools);

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
            $scope.filters = {};
            $scope.dateStart = "";
            $scope.dateEnd = "";
            $scope.global = true;
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
         * @description Transformar dados back-end pra formato gráfico
         * @param
         * @returns {Array} objetos do tipo highcharts
         */
        function transformToGraphics(lista) {

            $scope.charts = [];

            for (var a = 0; a < lista.length; a++) {

                var chart = getNewChart();

                chart.title.text = lista[a].Name;

                chart.series[0].data.push({
                    name: 'Media acertos',
                    y: parseInt(lista[a].AveragePositive),
                });

                chart.series[0].data.push({
                    name: 'Media erros',
                    y: parseInt(lista[a].AverageNegative),
                });

                $scope.charts.push(chart);
            }
        };

        /**
         * @description Criar um novo objeto do tipo highcharts
         * @param
         * @returns
         */
        function getNewChart() {

            return angular.copy({
                options: {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie'
                    },
                    plotOptions: {
                        series: {
                            dataLabels: {
                                enabled: false,
                                format: '{point.name}: {point.y:.1f}%'
                            }
                        },
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            },
                            showInLegend: true
                        }
                    },
                    tooltip: {
                        headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                        pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> do total<br/>'
                    }
                },
                title: {
                    text: undefined
                },
                series: [{
                    name: 'Alternativa',
                    colorByPoint: true,
                    data: []
                }],
                size: {
                    width: 350,
                    height: 250
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