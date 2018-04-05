/**
 * @function Performance Item Controller
 * @author Julio Cesar Silva - 13/04/2016
 */
(function (angular, $) {

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
		.module('appMain')
		.controller("PerformanceItemController", PerformanceItemController);


	PerformanceItemController.$inject = ['$scope', 'ReportTestModel', '$notification', '$pager', '$util'];

	/**
     * @function Controller para relatório de desempenho de prova por item
     * @param {Object} $scope
     * @param {Object} ReportFiltersModel
     * @param {Object} $notification
     * @param {Object} $pager
     * @param {Object} $util
     * @return
     */
	function PerformanceItemController($scope, ReportTestModel, $notification, $pager, $util) {

	    // :parâmetros enviados via url
	    var params = $util.getUrlParams();

	    // :modelos
	    $scope.collection = {
	        original: {
	            items: [],
	            dre: [],
	            grid: [],
	        },
	        average: {
	            items: [],
                dre: [],
                grid: [],
            },
            schools: []
	    };

	    /**
         * @description config. variaveis
         * @param
         * @returns
         */
	    function configVariaveis() {
	        $scope.countFilter = 1;
	        // :paginação
	        $scope.paginate = $pager(ReportTestModel.getReportByItemPerformance);
	        $scope.pages = 0;
	        $scope.totalItens = 0;
	        $scope.pageSize = 10;
	        $scope.paginate.indexPage(0);

            // :modelos
	        $scope.filters = {};
	        $scope.filters.global = true;
	        $scope.list = [];

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
         * @description Gerar gráficos
         * @param
         * @returns
         */
	    $scope.generateGraph = function _generateGraph() {

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

	                if (result.lista != undefined && result.lista.Schools.length > 0) {

	                    $scope.paginate.nextPage();
	                    $scope.collection.schools = result.lista.Schools;
	                    $scope.collection.original.items = result.lista.Items;
	                    $scope.collection.original.dre = result.lista.AverageDre;
	                    $scope.collection.original.grid = result.lista.AverageGrid;

	                    configSlider($scope.collection.original.items);

	                    if (!$scope.pages > 0) {
	                        $scope.pages = $scope.paginate.totalPages();
	                        $scope.totalItens = $scope.paginate.totalItens();
	                    }
	                }
	                else {
	                    $scope.collection.schools = [];
	                }
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	                $scope.collection.schools = [];
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
         * @description Obter array dado o próximo índice da lista
         * @param
         * @returns
         */
	    $scope.next = function __next() {

	        var nextValue = $scope.slider.current + 1;

	        if (nextValue > $scope.slider.max)
	            return;

	        $scope.slider.current = nextValue;

	        pickNext();
	    };

	    /**
         * @description Obter array dado o índice anterior da lista
         * @param
         * @returns
         */
	    $scope.previus = function __previus() {

	        var previusValue = $scope.slider.current - 1;

	        if (previusValue < 0)
	            return;

	        $scope.slider.current = previusValue;

	        pickPrevius();
	    };

	    /**
         * @description Obter array a partir do começo da lista
         * @param
         * @returns
         */
	    $scope.begin = function __begin() {

	        $scope.slider.current = 0;

	        pickBegin();
	    };

	    /**
         * @description Obter array a partir do fim da lista
         * @param
         * @returns
         */
	    $scope.end = function __end() {

	        $scope.slider.current = $scope.slider.max;

	        pickEnd();
	    };

	    /**
         * @description Config. slider (paginação fake tabela)
         * @param {Array} lista - config. dado uma lista
         * @returns
         */
	    function configSlider(lista) {

	        var interval = 4;

	        $scope.slider = {
	            min: 0,
	            max: ((lista.length - interval) < 0) ? 0 : lista.length - interval,
	            current: 0,
	            interval: interval,
	            length: lista.length
	        };

	        $scope.begin();
	    };

	    /**
         * @description Obter array dado o próximo índice da lista
         * @param
         * @returns
         */
	    function pickNext() {

	        reset();

	        for (var a = $scope.slider.current; a < $scope.slider.length; a++)
	            if ($scope.collection.average.items.length < $scope.slider.interval)
	                setArr(a);
	    };

	    /**
         * @description Obter array dado o índice anterior da lista
         * @param
         * @returns
         */
	    function pickPrevius() {

	        reset();

	        for (var a = $scope.slider.current; a < $scope.slider.length; a++)
	            if ($scope.collection.average.items.length < $scope.slider.interval)
	                setArr(a);
	    };

	    /**
         * @description Obter array a partir do começo da lista
         * @param
         * @returns
         */
	    function pickBegin() {

	        reset();

	        for (var a = 0; a < $scope.slider.length; a++)
	            if ($scope.collection.average.items.length < $scope.slider.interval)
	                setArr(a);
	    };

	    /**
         * @description Obter array a partir do fim da lista
         * @param
         * @returns
         */
	    function pickEnd() {

	        reset();

	        for (var a = $scope.slider.length - 1 ; a >= 0; a--)
	            if (a >= $scope.slider.max)
	                setArr(a);

	        reverse();
	    };

	    /**
         * @description Obter array a ser mostrado a partir de um índice do array original
         * @param
         * @returns
         */
	    function setArr(index) {

	        $scope.collection.average.items.push(angular.copy($scope.collection.original.items[index]));
	        $scope.collection.average.dre.push(angular.copy($scope.collection.original.dre[index]));
	        $scope.collection.average.grid.push(angular.copy($scope.collection.original.grid[index]));

	        for (var a = 0; a < $scope.collection.schools.length; a++)
	            $scope.collection.schools[a].averageDisplayed.push(angular.copy($scope.collection.schools[a].Average[index]));
	    };

	    /**
         * @description Reset array de médias de todas as escolas
         * @param
         * @returns
         */
	    function reset() {

	        $scope.collection.average.items = [];
	        $scope.collection.average.dre = [];
	        $scope.collection.average.grid = [];

	        for (var a = 0; a < $scope.collection.schools.length; a++)
	            $scope.collection.schools[a].averageDisplayed = [];
	    };

	    /**
         * @description Reset array de médias de todas as escolas
         * @param
         * @returns
         */
	    function reverse() {

	        $scope.collection.average.items = $scope.collection.average.items.reverse();
	        $scope.collection.average.dre = $scope.collection.average.dre.reverse();
	        $scope.collection.average.grid = $scope.collection.average.grid.reverse();

	        for (var a = 0; a < $scope.collection.schools.length; a++)
	            $scope.collection.schools[a].averageDisplayed = $scope.collection.schools[a].averageDisplayed.reverse();
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