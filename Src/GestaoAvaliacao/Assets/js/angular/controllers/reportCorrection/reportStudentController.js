/**
 * function ReportDREController Controller
 * @namespace Controller
 * @author julio.silva@mstech.com.br: since 11/10/2016
 */
(function (angular, $) {

	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	angular
		.module('appMain')
		.controller("ReportStudentController", ReportStudentController);

	ReportStudentController.$inject = ['$rootScope', '$scope', 'ReportCorrectionModel', 'TestModel', '$notification', '$timeout', '$pager', '$window', '$util'];

	function ReportStudentController($rootScope, $scope, ReportCorrectionModel, TestModel, $notification, $timeout, $pager, $window, $util) {

	    $scope.WARNING_UPLOAD_BATCH_DETAIL = Boolean(Parameters.General.WARNING_UPLOAD_BATCH_DETAIL == "True");
	    $scope.EnumBatchSituationByte = IndexBatchDetailsEnumBatchSituationByte;
	    $scope.EnumBatchSituationLabel = IndexBatchDetailsEnumBatchSituationLabel;
	    $scope.EnumBatchSituationDescription = IndexBatchDetailsEnumBatchSituationDescription;
	    $scope.profile = getCurrentVision();
	    $scope.params = $util.getUrlParams();

	    /**
         * @function Recuperar query string da url
         * @param
         * @returns
         */
	    function Init() {
	        if (!$scope.params.tur_id || !$scope.params.test_id) {
	            $notification.alert("URL inválida.");
	            $timeout(function () {
	                window.history.back();
	            }, 3000);
	            return;
	        }
	        if ($scope.profile == EnumVisions.ADMINISTRATOR || $scope.profile == EnumVisions.MANAGER) getHeaderDetails();
	        if ($scope.profile != EnumVisions.ADMINISTRATOR && $scope.profile != EnumVisions.MANAGER) getHeaderDetailsForVision();
	        configVariables();
	        $scope.search();
	    };

	    /**
		 * @function Responsavél por instanciar tds as variaveis
		 * @param 
		 * @returns
		 */
	    function configVariables() {
	        // :paginação
	        $scope.paginate = $pager(ReportCorrectionModel.getStudent);
	        $scope.totalItens = 0;
	        $scope.pages = 0;
	        $scope.pageSize = 10;
	        $scope.message = false;
	        // :vars
	        $scope.listResult = [];
	        $scope.header = {
	            DRE: "",
	            Test: "",
	            School: "",
                Section: ""
	        };
	        $scope.totalNetwork = {
	            Aderidos: 0,
	            Identificados: 0,
	            Sucesso: 0,
	            Conferir: 0,
	            Ausente: 0,
	            Erro: 0,
	            Pendente: 0
	        };
	    };

	    /**
         * @function obter detalhes de cabeçalho da page
         * @param {object} _callback
         * @returns
         */
	    function getHeaderDetails(_callback) {
	        TestModel.getInfoTurReport({
	            Test_id: $scope.params.test_id,
	            uad_id: $scope.params.uad_id,
	            esc_id: $scope.params.esc_id,
	            tur_id: $scope.params.tur_id
	        }, function (result) {
	            if (result.success) {
	                $scope.header = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	                $timeout(function () {
	                    //window.location.href = "/ReportCorrection/"
	                }, 3000);
	            }
	            if (_callback) _callback();
	        });
	    };
	    $scope.getHeaderDetails = getHeaderDetails;

	    function getHeaderDetailsForVision(_callback) {
	        if (!$scope.params.test_id) return;
	        TestModel.getInfoTestReport({ Test_id: $scope.params.test_id }, function (result) {
	            if (result.success) {
	                $scope.header = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	            if (_callback) _callback();
	        });
	    };

	    /**
		 * @function Zera a paginação e pesquisa novamente
		 * @param 
		 * @returns
		 */
	    $scope.search = function __search() {
	        $scope.pages = 0;
	        $scope.totalItens = 0;
	        $scope.paginate.indexPage(0);
	        $scope.pageSize = $scope.paginate.getPageSize();
	        getFilters();
	        Paginate();
	    };

	    /**
         * @function validar
         * @param
         * @returns
         */
	    function getFilters() {
	        $scope.searcheableFilter = {
	            uad_id: $scope.params.uad_id,
	            esc_id: $scope.params.esc_id,
	            test_id: $scope.params.test_id,
	            tur_id: $scope.params.tur_id,
	            DateStart: $scope.params.dateStart,
	            DateEnd: $scope.params.dateEnd
	        };
	    };

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
	                    $scope.listResult = angular.copy(result.lista);
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
		 * @return
		 */
		$scope.generateReport = function __generateReport() {
		    ReportCorrectionModel.exportStudent($scope.searcheableFilter, function (result) {
		        if (result.success) {
		            window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		/**
		 * @function Redireciona para tela anterior
		 * @param 
		 * @return
		 */
		$scope.prevRedirect = function __prevRedirect() {
		    window.history.back();
		};

	    /**
         * @function Iniciar
         * @param
         * @returns
         */
		(function __init() {
		    $notification.clear();
		    Init();
		})();
	};

})(angular, jQuery);