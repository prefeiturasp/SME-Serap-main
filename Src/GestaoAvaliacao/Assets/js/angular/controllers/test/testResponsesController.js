/**
 * function TestStudentResponsesController Controller
 * @namespace Controller
 * @author Julio Cesar Silva 24/11/2015
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("TestStudentResponsesController", TestStudentResponsesController);

	TestStudentResponsesController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$pager', '$notification', '$timeout', '$location', 'TestAdministrateModel', 'AdherenceModel'];


	/**
	 * @function Controller 'Administrar controle da prova'
	 * @name TestStudentResponsesController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $notification
	 * @param {Object} $timeout
     * @param {Object} TestAdministrateModel
     * @param {Object} AdherenceModel
	 * @return
	 */
	function TestStudentResponsesController($rootScope, $scope, $window, $sce, $util, $pager, $notification, $timeout, $location, TestAdministrateModel, AdherenceModel) {

	    $scope.paginate = $pager(TestAdministrateModel.getSectionAdministrate);
	    $scope.pageSize = 10;

	    /**
		 * @function Inicialização das informações da prova
		 * @param
		 * @return
		 */
	    function configTestInformation(informations) {

	        $scope.testInformation = {
	            Description: informations.testName !== undefined && informations.testName !== null ? informations.testName : "Prova sem nome",
	            FrequencyApplication: informations.frequencyApplication,
	            Discipline: informations.testDiscipline !== undefined && informations.testDiscipline !== null ? informations.testDiscipline : "Sem disciplina",
	            TestId: informations.testId,
	            TestOwner: informations.testOwner,
	            Global: informations.global === undefined || informations.global === null ? false : informations.global
	        };

	        $scope.answerSheetBlocked = informations.answerSheetBlocked;
	    };

	    function checkParamtersUrl(authorize) {

	        if ((authorize.testId != 0 && authorize.esc_id != 0) ||
				(authorize.testId != 0 && authorize.esc_id == 0)) {
	            return true;
	        }

	        return false;
	    };

	    function checkInfoAturorize(authorize) {

	        if (authorize || $scope.params.test_id) {
	            if (authorize) {

	                if (checkParamtersUrl(authorize)) {

	                    configTestInformation(authorize);

	                    var test_id = ($scope.testInformation.TestId === undefined || $scope.testInformation.TestId === null || !parseInt($scope.testInformation.TestId) ? 0 : $scope.testInformation.TestId);

	                    if (test_id === 0) {
	                        $scope.blockPage = true;
	                        $notification.alert("Id da prova inválido ou usuário sem permissão.");
	                        $scope.safeApply();
	                        redirectToList();
	                        return;
	                    }

	                    checkSessionstorage();
	                    	                    
	                }
	                else {
                        $scope.blockPage = true;
                        $notification.alert("Não foi possível obter os dados de acesso a página.");
	                    $scope.safeApply();
	                    redirectToList();
	                    return;
	                }
	            } else {
	                $notification.alert("Não foi possível obter os dados de acesso a página.");
	                setTimeout(function () { $window.history.back(); }, 3000);
	            }
	        }
	    };

	    /**
		  * @function Redirecionar para listagem de provas.
		  * @param
		  * @return
		  */
	    function redirectToList() {
	        $timeout(function __invalidTestId() {
	            $window.location.href = "/Test";
	        }, 3000);
	    };

	    /**
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @param
		 * @return
		 */
	    function configListFilter() {

	        $scope.listFilter = {
	            DREs: [],
	            Schools: [],
                Years: [],
                Turns: [],
	            Processing: []
	        };
	    };

	    /**
		 * @function Configuração dos filtros
		 * @param
		 * @return
		 */
	    function configFilters() {

	        $scope.filters = {
	            DRE: undefined,
	            School: undefined,
                Year: undefined,
	            Turn: undefined
	        };
	    };

	    /**
		 * @function Configuração das listas
		 * @param
		 * @return
		 */
	    function configLists() {

	        $scope.list = {
	            displayed: []
	        };
	    };

	    /**
		 * @function Current selected master filters
		 * @param 
		 * @return
		 */
	    function configCurrentSelectedMasterFilters() {

	        $scope.currentSelectedMasterFilters = {
	            DRE_Description: undefined,
	            School_Description: undefined
	        };
	    };

	    /**
		 * @function Set current selected master filters
		 * @param 
		 * @return
		 */
	    function setCurrentSelectedMasterFilters() {

	    	if ($scope.filters.DRE !== undefined && $scope.filters.DRE !== null)
	    		$scope.currentSelectedMasterFilters.DRE_Description = $scope.filters.DRE.Description;
	    	else $scope.currentSelectedMasterFilters.DRE_Description = undefined;
	    	if ($scope.filters.School !== undefined && $scope.filters.School !== null)
	    		$scope.currentSelectedMasterFilters.School_Description = $scope.filters.School.Description;
	    	else $scope.currentSelectedMasterFilters.School_Description = undefined;	    	
	    };

	    /**
         * @function Obter todas DREs
         * @param
         * @return
         */
	    function getDREs() {

	        AdherenceModel.getDRESimple({}, function (result) {

	            if (result.success) {
	            	$scope.listFilter.DREs = result.lista;

	            	if ($scope.executionOne == 0)
	            		$scope.setPagination();
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	        });
	    };

	    $scope.setSessionStorage = function(){
	        sessionStorage.setItem("leavingStudentResponse", JSON.stringify(true));
	    }

	    /**
		 * @function Obter todas Escolas
		 * @param
		 * @return
		 */
	    $scope.getSchools = function __getSchools() {

	        if ($scope.filters.DRE === undefined || $scope.filters.DRE === null)
	            return;

	        var params = { 'dre_id': $scope.filters.DRE.Id };

	        AdherenceModel.getSchoolsSimple(params, function (result) {

	            if (result.success) {
	            	$scope.listFilter.Schools = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	        });
	    };

	    /**
		 * @function Obter todos os anos
		 * @param
		 * @return
		 */
	    $scope.getYears = function __getYears() {

            if ($scope.filters.School === undefined || $scope.filters.School === null)
	            return;

            var params = { 'esc_id': $scope.filters.School.Id, 'test_id': $scope.testInformation.TestId };

	        AdherenceModel.getCurriculumGradeSimple(params, function (result) {

	            if (result.success) {
	                $scope.listFilter.Years = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	        });
	    };

	    /**
		 * @function Obter todos Turnos
		 * @param
		 * @return
		 */
	    $scope.getTurns = function __getTurns() {

            if ($scope.filters.School === undefined || $scope.filters.School === null)
	            return;

	        var params = { 'esc_id': $scope.filters.School.Id };

	        AdherenceModel.getShiftSimple(params, function (result) {

	            if (result.success) {
	                $scope.listFilter.Turns = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	        });
	    };

	    function getStatusCorrectionList() {

	        TestAdministrateModel.getStatusCorrectionList(function (result) {

	            if (result.success) {
	                $scope.listFilter.Processing = result.lista;
	            }
	            else {
	                $notification[result.type ? result.type : 'error'](result.message);
	            }
	        });
	    };

	    /**
         * @function salva ou remove todas as situações selecionadas no filtro
         * @param 
         * @return
         */
	    $scope.setFilterProcessing = function __setFilterProcessing(processing) {

	        if (processing.state) {
	            $scope.processingFilter.push(processing.Id);
	        }
	        else {
	            var i, max = $scope.processingFilter.length;
	            // limpando a lista de selecionado e bloqueando a pesquisa quando não tem situação selecionada
	            if (max == 1) {
	                $scope.processingFilter = [];
	                return;
	            }
	            for (i = 0; i < max; i++) {
	                //procurando o id igual no array para exclui-lo
	                if ($scope.processingFilter[i] === processing.Id) {
	                    $scope.processingFilter.splice(i, 1);
	                    break;
	                }
	            }
	        }

	    };

	    /**
         * @function Limpeza dos filtros de pesquisa.
         * @param
         * @return
         */
	    $scope.clearFilters = function __clearFilters() {

	        $scope.listFilter.Schools = [];
	        $scope.listFilter.Years = [];
	        $scope.listFilter.Turns = [];
	        $scope.processingFilter = [];
	        $scope.School = null;
	     
	        if ($scope.params.esc_id != null && $scope.executionOne != 0) {
	        	var auxId = $scope.params.test_id;
	        	$scope.params = [];
	        	$scope.params["test_id"] = auxId;
	        }
	        configFilters();
	        if ($scope.executionOne != 0) setCurrentSelectedMasterFilters();
	        for (var a = 0; a < $scope.listFilter.Processing.length; a++)
	            $scope.listFilter.Processing[a].state = false;
	        $scope.countProcessing = 0;
	        $scope.countFilter = 0
	    };

	    /**
		 * @function Limpeza dos filtros de pesquisa
		 * @param {string} filter
		 * @return
		 */
	    $scope.clearByFilter = function __clearByFilter(filter) {

	        if (filter === 'DRE') {
	            $scope.listFilter.Schools = [];
	            $scope.listFilter.Years = [];
	            $scope.listFilter.Turns = [];
	            $scope.filters.School = undefined;
	            $scope.filters.Year = undefined;
	            $scope.filters.Turn = undefined;
	            return;
	        }

	        if (filter === 'School') {
	            $scope.listFilter.Years = [];
	            $scope.listFilter.Turns = [];
	            $scope.filters.Year = undefined;
	            $scope.filters.Turn = undefined;
	            return;
	        }
	    };

	    /**
		 * @function Método Construtor da pagina
		 * @param {object} authorize
		 * @return
		 */
	    $scope.init = function __init() {

	        $scope.params = $util.getUrlParams();
	        if ($scope.params.test_id && $scope.params.test_id.split(".").length == 1) {
	            getAuthorize();
	            
	        } else {
	            $scope.blockPage = true;
	            $notification.alert("Id da prova inválido ou usuário sem permissão.");
	            $scope.safeApply();
	            redirectToList();
	            return;
	        }
	    };

	    function checkSessionstorage() {
            
	        var leavingStudentResponse = JSON.parse(sessionStorage.getItem('leavingStudentResponse'));
	        var arrivingStudentResponse = JSON.parse(sessionStorage.getItem('arrivingStudentResponse'));
	        var searchFilterStudentResponse = JSON.parse(sessionStorage.getItem('searchFilterStudentResponse'));

	        //DEBUG
	        //if (searchFilterStudentResponse) {
	        //    $scope.filters = searchFilterStudentResponse;
	        //    console.log($scope.filters);
	        //    loadFilterFromSessionStorage();
	        //    return;
	        //}

	        if (leavingStudentResponse && arrivingStudentResponse && searchFilterStudentResponse) {
	            $scope.filters = searchFilterStudentResponse;	            
	            loadFilterFromSessionStorage();
	        } else {
	            configStart();
	            sessionStorage.removeItem('searchFilterStudentResponse')
	        }

	        sessionStorage.removeItem('leavingStudentResponse')
	        sessionStorage.removeItem('arrivingStudentResponse')
	    }

	    function loadFilterFromSessionStorage() {

	        $scope.countFilter = 0;
	        $scope.blockPage = false;
	        $scope.typeFile = false;
	        $scope.searchFilter = false;
	        $scope.batchWarning = { status: false, message: "" };
	        $scope.AllowAnswer = true;
	        $scope.ShowResult = true;
	        $scope.processingFilter = [];
	        $scope.statusCorrection = null;
	        
	        configCurrentSelectedMasterFilters();	        
	        configListFilter();	        
	        configLists();
	        
	        getDREs();
	        if ($scope.filters.DRE) {
	            $scope.getSchools();
	            if ($scope.filters.School) {
	                $scope.getTurns();
	                $scope.getYears();
	            }
	        }

	        $scope.listFilter.Processing = $scope.filters.listFilter;

	        var listProcessing = $scope.filters.listFilter.filter(function (value) {
	            return value.state;
	        });

	        listProcessing.forEach(function (processing) {
	            $scope.setFilterProcessing(processing);  
	        });

	        getStatusCorrectionSlected();
	        $scope.setPagination();

	        $scope.countProcessing = 0;
	        $scope.$watch("processingFilter", function () {
	            $scope.countProcessing = $scope.processingFilter.length;
	            updateFilterCount();
	        }, true);
	        $scope.$watchCollection("[filters.DRE, filters.School, filters.Year, filters.Turn]", function () {
	            updateFilterCount();
	        }, true);

	    }

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @param {object} authorize
		 * @return
		 */
	    function configStart() {
	        $scope.countFilter = 0;
			$scope.blockPage = false;
			$scope.typeFile = false;
			$scope.searchFilter = false;
			$scope.batchWarning = { status: false, message: "" };
			$scope.AllowAnswer = true;
			$scope.ShowResult = true;
			$scope.processingFilter = [];
			$scope.statusCorrection = null;
			configCurrentSelectedMasterFilters();
			configListFilter();
			configLists();
			$scope.clearFilters();
			getDREs();
			getStatusCorrectionList();
			$scope.setPagination();
			$scope.countProcessing = 0;
			$scope.$watch("processingFilter", function () {
			    $scope.countProcessing = 0;
			    $scope.countProcessing = $scope.processingFilter.length;
			    updateFilterCount();
			}, true);
			$scope.$watchCollection("[filters.DRE, filters.School, filters.Year, filters.Turn]", function () {
			    updateFilterCount();
			}, true);
		};

	    function updateFilterCount() {
	        $scope.countFilter = 0;
	        if ($scope.filters.DRE) $scope.countFilter += 1;
	        if ($scope.filters.School) $scope.countFilter += 1;
	        if ($scope.filters.Year) $scope.countFilter += 1;
	        if ($scope.filters.Turn) $scope.countFilter += 1;
	        $scope.countFilter += $scope.countProcessing;
	    };

	    /**
		 * @function Obter objeto para configuração da page.
		 * @param {object} authorize
		 * @return
		 */
	    function getAuthorize() {

	        TestAdministrateModel.getAuthorize($scope.params, function (result) {
	            if (result.success) {
	                checkInfoAturorize(result.dados);
	            } else {
	                $scope.blockPage = true;
	                $notification[result.type ? result.type : 'error'](result.message);
	                $scope.safeApply();
	                redirectToList();
	                return;
	            }
	        });
	    };

	    $scope.searchFilterSchool = function () {

		    $scope.searchFilter = true;
		    getStatusCorrectionSlected();
		    $scope.setPagination();

	        $scope.filters.listFilter = $scope.listFilter.Processing;
	        sessionStorage.setItem('searchFilterStudentResponse', JSON.stringify($scope.filters));
		};

		function getStatusCorrectionSlected() {
		    $scope.statusCorrection = "";
		    var i, max = $scope.processingFilter.length;
		    if (max != 0) {
		        for (i = 0; i < max; i++) {
		            if (i < (max - 1)) {
		                $scope.statusCorrection += $scope.processingFilter[i] + ","
		            } else {
		                $scope.statusCorrection += $scope.processingFilter[i];
		            }
		        }
		    } else {
                $scope.statusCorrection = null;
		    }
		};

	    /**
		 * @function Limpar Paginação
		 * @param
		 * @return
		 */
		$scope.setPagination = function __setPagination() {

		    $scope.paginate.indexPage(0);
		    $scope.pesquisa = '';
		    $scope.message = false;
		    $scope.pages = 0;
		    $scope.totalItens = 0;
		    $scope.pageSize = $scope.paginate.getPageSize();
		    $scope.search();
		};

	    /**
		 * @function Pesquisa.
		 * @param
		 * @return
		 */
		$scope.search = function __search() {

		    var schoolId = null;

			if ($scope.filters.School !== undefined && $scope.filters.School !== null) {
			    schoolId = $scope.filters.School.Id;
			} else if ($scope.params === undefined && $scope.params.esc_id != undefined || $scope.params.esc_id != null) {
			    schoolId = $scope.params.esc_id;
			} else {
			    schoolId = 0;
			}

		    var params = {
		        'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
		        'dre_id': ($scope.filters.DRE !== undefined && $scope.filters.DRE !== null) ? $scope.filters.DRE.Id : "",
		        'esc_id': schoolId,
		        'crp_ordem': ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0,
		        'ttn_id': ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0,
		        'statusCorrection': $scope.statusCorrection
		    };

		    $scope.paginate.paginate(params).then(function (result) {

		        if (result.success) {

		        	setCurrentSelectedMasterFilters();

		        	if (result.lista.length > 0) {

		                $scope.paginate.nextPage();
		                $scope.list.displayed = angular.copy(result.lista);
		                $scope.batchWarning = result.batchWarning;
		                $scope.AllowAnswer = result.AllowAnswer;
		                $scope.ShowResult = result.ShowResult;

		                if (!$scope.pages > 0) {

		                    $scope.pages = $scope.paginate.totalPages();
		                    $scope.totalItens = $scope.paginate.totalItens();    
		                }
		            } else {
		                $scope.message = true;
		                $scope.list.displayed = null;
		            }
		        }
		        else {
		            $scope.list.displayed = null;
				}

		    }, function () {
		        $scope.message = true;
		        $scope.list.displayed = null;
		    });


		};

	    /**
         * @function Obtem o number do enum dado o value do enum
         * @param {object} enumerator
         * @param {string} _key
         * @return
         */
	    $scope.getEnumIntegerByValue = function getEnumIntegerByValue(enumerator, value) {

	        for (var key in enumerator)
	            if (value === enumerator[key])
	                return parseInt(key);
	    };

	    $scope.answerSheetToolTip = function answerSheetToolTip() {

	        if ($scope.answerSheetBlocked)
	            return "A folha de resposta não pode ser gerada pois o tipo da prova não tem um tipo de item definido.";
	        else
	            return "Gerar folha de resposta";
	    };

	    /**
		 * @function Redirecionar para tela de upload de folhas de resposta.
		 * @param
		 * @return
		 */
	    $scope.redirectToAnswerSheet = function __redirectToAnswerSheet() {

	    	var schoolId = null;

	    	if ($scope.filters.School !== undefined && $scope.filters.School !== null) {
	    		schoolId = $scope.filters.School.Id;
	    	} else if ($scope.params === undefined && $scope.params.esc_id != undefined || $scope.params.esc_id != null) {
	    		schoolId = $scope.params.esc_id;
	    	} else {
	    		schoolId = 0;
	    	}

	    	$window.location.href = '/AnswerSheet/IndexBatchDetails?test_id=' + $scope.testInformation.TestId;
	    };

	    /**
		 * @function arrivingTest.
		 * @param
		 * @return
		 */
	    $scope.arrivingTest = function __arrivingTest() {
	        sessionStorage.setItem('arrivingTest', JSON.stringify(true));
            $window.location.href = '/Test';
	    };

	    /**
		 * @function Cancel.
		 * @param
		 * @return
		 */
	    $scope.cancel = function __cancel() {
	        $window.location.href = '/Test';
	    };

	    /**
		 * @function Forçar a atualização (diggest) do angular
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

	};

})(angular, jQuery);