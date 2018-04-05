/**
 * function ReportDREController Controller
 * @namespace Controller
 * @author julio.silva@mstech.com.br: since 10/10/2016
 */
(function (angular, $) {

	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	angular
		.module('appMain')
		.controller("ReportDREController", ReportDREController);

	ReportDREController.$inject = ['$rootScope', '$scope', 'ReportCorrectionModel', 'AdherenceModel', 'TestModel', '$notification', '$timeout', '$pager', '$window', '$util'];

	function ReportDREController($rootScope, $scope, ReportCorrectionModel, AdherenceModel, TestModel, $notification, $timeout, $pager, $window, $util) {

		/**
		 * @function Responsavél por instanciar tds as variaveis
		 * @param 
		 * @returns
		 */
	    function configVariables() {
	        $scope.WARNING_UPLOAD_BATCH_DETAIL = Boolean(Parameters.General.WARNING_UPLOAD_BATCH_DETAIL == "True");
	        $scope.countFilter = 0;
            // :paginação
		    $scope.paginate = $pager(ReportCorrectionModel.getDres);
			$scope.totalItens = 0;
			$scope.pages = 0;
			$scope.pageSize = 10;
			$scope.message = false;
			$scope.listResult = null;
            // :vars
			$scope.listDREs = [];
			$scope.listTests = [];
			$scope.totalNetwork = {
			    Aderidos: 0,
			    Identificados: 0,
			    Sucesso: 0,
			    Conferir: 0,
			    Ausente: 0,
			    Erro: 0,
			    Pendente: 0
			};
			getDREs();
			$scope.$watch("filters", function () {
			    $scope.countFilter = 0;
			    if ($scope.filters.DateStart) $scope.countFilter += 1;
			    if ($scope.filters.DateEnd) $scope.countFilter += 1;
			    if ($scope.filters.test_id) $scope.countFilter += 1;
			    if ($scope.filters.uad_id) $scope.countFilter += 1;
			}, true);
		};

	    /**
         * @function obter detalhes de cabeçalho da page
         * @param {object} _callback
         * @returns
         */
		function getHeaderDetails(_callback) {
		    if (!$scope.filters.test_id) return;
		    TestModel.getInfoTestReport({ Test_id: $scope.filters.test_id }, function (result) {
		        if (result.success) {
		            $scope.header = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		        if (_callback) _callback();
		    });
		};
		$scope.getHeaderDetails = getHeaderDetails;

	    /**
		 * @function Obter todas as DREs
		 * @param {object} _callback
		 * @returns
		 */
		function getDREs(_callback) {
		    AdherenceModel.getDRESimple(function (result) {
		        if (result.success) {
		            $scope.listDREs = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		        if (_callback) _callback();
		    });
		};

	    /**
		 * @function Obter as provas dado um período
		 * @param {object} _callback
		 * @returns
		 */
		function getTests(_callback) {
		    if ($scope.filters.DateStart && $scope.filters.DateEnd) {
		        if (!validateDate()) return;
		        TestModel.getTestByDate({ DateStart: $scope.filters.DateStart, DateEnd: $scope.filters.DateEnd }, function (result) {
		            if (result.success) {
		                $scope.listTests = result.lista;
		                if($scope.listTests.length === 0)
                            $notification.alert("Nenhuma prova foi encontrada no período selecionado.")
		            }
		            else {
		                $notification[result.type ? result.type : 'error'](result.message);
		            }
		        });
		    }
		    if (_callback) _callback();
		};
		$scope.getTests = getTests;
        	   
		function changeTest(_callback) {
		    if ($scope.filters.test_id > 0 && $('#selectTest').val() != "") {
		        $('#testCode').val($scope.filters.test_id)
		    }
		   
		    if (_callback) _callback();
		};
		$scope.changeTest = changeTest;
        	   
		function changeTestCode(_callback) {
		    if ($scope.filters.test_id > 0 && $('#testCode').val() != "") {
		        $('#selectTest').val($scope.filters.test_id)
		    }

		    if (_callback) _callback();
		};
		$scope.changeTestCode = changeTestCode;

	    /**
         * @function Validar período de datas
         * @param 
         * @returns
         */
		function validateDate() {
		    if (!$scope.filters.DateStart || !$scope.filters.DateEnd) {
		        $notification.alert("É necessário selecionar um período.");
		        return false;
		    }
		    if ($util.greaterEndDateThanStartDate($scope.filters.DateStart, $scope.filters.DateEnd) === false) {
		        $notification.alert("Data inicial não pode ser maior que a data final.");
		        $scope.filters.DateEnd = "";
		        return false;
		    }
		    return true;
		};

	    /**
		 * @function Limpar filtros
		 * @param 
		 * @returns
		 */
		$scope.clearFilters = function __clearFilters() {
		    $scope.filters = {};
		    $scope.filters.uad_id = "";
		    $scope.filters.test_id = "";
		    $scope.filters.DateStart = "";
		    $scope.filters.DateEnd = "";
		};

	    /**
		 * @function Zera a paginação e pesquisa novamente
		 * @param 
		 * @returns
		 */
		$scope.search = function __search() {
		    if (!validateDate() || !validate()) return;
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.paginate.indexPage(0);
			$scope.pageSize = $scope.paginate.getPageSize();
			$scope.searcheableFilter = angular.copy($scope.filters);
			getHeaderDetails();
			Paginate();
		};

	    /**
         * @function validar
         * @param
         * @returns
         */
		function validate() {
		    if (!$scope.filters.test_id) {
		        $notification.alert("É necessário selecionar uma prova ou preencher o seu código.");
		        return false;
		    }

		    if (!Number.isInteger($scope.filters.test_id)) {
		        $notification.alert("Código da prova inválido.");
		        return false;
		    }

		    var position = $scope.listTests.indexOfField("TestId", $scope.filters.test_id);
		    if (position < 0) {
		        $notification.alert("Prova não encontrada ou fora do período informado.");
		        return false;
		    }

		    return true;
		};

		Array.prototype.indexOfField = function (propertyName, value) {
		    for (var i = 0; i < this.length; i++)
		        if (this[i][propertyName] === value)
		            return i;
		    return -1;
		}

		/**
		 * @function Responsavél pela paginação
		 * @param 
		 * @returns
		 */
		function Paginate() { 
		    $scope.paginate.paginate($scope.searcheableFilter).then(function (result) {
		        $scope.message = false;
		        if (result.success) {
		            if (result.lista.length > 0) {
		                $scope.paginate.nextPage();
		                $scope.listResult = result.lista;
		                $scope.QuantidadeTotal = result.QuantidadeTotal;
		                if (!$scope.pages > 0) {
		                    $scope.pages = $scope.paginate.totalPages();
		                    $scope.totalItens = $scope.paginate.totalItens();
		                }
		            } else {
		                $scope.message = true;
		                $scope.listResult = null;
		            }
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    }, function () {
		        $scope.message = true;
		        $scope.listResult = null;
		    });
		};
		$scope.Paginate = Paginate;

		/**
		 * @function Gera o relatório de processamento da correção
		 * @param 
		 * @returns
		 */
		$scope.generateReport = function __generateReport() {
		    ReportCorrectionModel.exportDRE($scope.searcheableFilter, function (result) {
		        if (result.success) {
		            window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

	    /**
		 * @function Acionar date-picker através de btn
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
		    $scope.clearFilters();
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