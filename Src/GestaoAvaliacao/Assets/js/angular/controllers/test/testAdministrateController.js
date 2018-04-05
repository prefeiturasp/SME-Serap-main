/**
 * function TestAdministrateController Controller
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
		.controller("TestAdministrateController", TestAdministrateController);

	TestAdministrateController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$pager', '$notification', '$timeout', 'TestAdministrateModel', 'AdherenceModel', 'AnswerSheetModel'];


	/**
	 * @function Controller 'Administrar controle da prova'
	 * @name TestAdministrateController
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
	function TestAdministrateController($rootScope, $scope, $window, $sce, $util, $pager, $notification, $timeout, TestAdministrateModel, AdherenceModel, AnswerSheetModel) {

	    $scope.paginate = $pager(TestAdministrateModel.getSectionAdministrate);
	    $scope.pageSize = 10;
	    $scope.countFilter = 0;

	    /**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
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
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @name configListFilter
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
	    function configListFilter() {

	        $scope.listFilter = {
	            DREs: [],
	            Schools: [],
                Years: [],
	            Turns: []
	        };
	    };


	    /**
		 * @function Configuração dos filtros
		 * @name configFilters
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
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
		 * @name configLists
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
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
		 * @name configCurrentSelectedMasterFilters
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
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
		 * @name setCurrentSelectedMasterFilters
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
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
         * @name getDREs
         * @namespace TestAdministrateController
         * @memberOf Controller
         * @private
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

        
	    /**
		 * @function Obter todas Escolas
		 * @name getSchools
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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
		 * @name getYears
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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
		 * @name getTurns
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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


	    /**
         * @function Limpeza dos filtros de pesquisa.
         * @name clearFilters
         * @namespace TestAdministrateController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
	    $scope.clearFilters = function __clearFilters() {

	        $scope.listFilter.Schools = [];
	        $scope.listFilter.Years = [];
	        $scope.listFilter.Turns = [];
	        $scope.School = null;
	     
	        if ($scope.params.esc_id != null && $scope.executionOne != 0) {
	        	var auxId = $scope.params.test_id;
	        	$scope.params = [];
	        	$scope.params["test_id"] = auxId;
	        }//if

	        configFilters();
	        if ($scope.executionOne != 0) {
	        	setCurrentSelectedMasterFilters();
	        }
	    };


	    /**
		 * @function Limpeza dos filtros de pesquisa
		 * @name clearByFilter
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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
		 * @function Obter objeto inicial para configuração da page.
		 * @name init
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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

	    /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
	    function getAuthorize() {
	        TestAdministrateModel.getAuthorize($scope.params, function (result) {
	            if (result.success) {
	                checkSessionstorage(result.dados);
	            } else {
	                $scope.blockPage = true;
	                $notification[result.type ? result.type : 'error'](result.message);
	                $scope.safeApply();
	                redirectToList();
	                return;
	            }
	        });
	    };


	    function checkSessionstorage(dados) {

	        var leavingIndexAdministrate = JSON.parse(sessionStorage.getItem('leavingIndexAdministrate'));
	        var arrivingIndexAdministrate = JSON.parse(sessionStorage.getItem('arrivingIndexAdministrate'));
	        var searchFilterIndexAdministrate = JSON.parse(sessionStorage.getItem('searchFilterIndexAdministrate'));

	        //DEBUG
	        //if (searchFilterIndexAdministrate) {
	        //    $scope.filters = searchFilterIndexAdministrate;
	        //    loadFilterFromSessionStorage(dados);
	        //    return;
	        //}

	        if (leavingIndexAdministrate && arrivingIndexAdministrate && searchFilterIndexAdministrate) {
	            $scope.filters = searchFilterIndexAdministrate;
	            loadFilterFromSessionStorage(dados);
	        } else {
	            configStart(dados);
	            sessionStorage.removeItem('searchFilterIndexAdministrate')
	        }

	        sessionStorage.removeItem('leavingIndexAdministrate')
	        sessionStorage.removeItem('arrivingIndexAdministrate')
	    }

	    function loadFilterFromSessionStorage(authorize) {

	        $scope.blockPage = false;
	        $scope.typeFile = false;
	        $scope.downloadUrl = null;
	        $scope.searchFilter = false;
	        $scope.answerSheetBlocked = authorize.answerSheetBlocked;
	        if (authorize === undefined || authorize === null) {
	            $scope.blockPage = true;
	            $notification.alert("Não foi possível obter as permissões de acesso a página.");
	            redirectToList();
	            return;
	        }
	        configTestInformation(authorize);
	        if ($scope.params === undefined || $scope.params.test_id === undefined || !parseInt($scope.params.test_id))
	            if (parseInt($scope.params.test_id) !== $scope.testInformation.TestId) {
	                $scope.blockPage = true;
	                $notification.alert("Id da prova inválido ou usuário sem permissão.");
	                $scope.safeApply();
	                redirectToList();
	                return;
	            }
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

	        $scope.setPagination();
	        $scope.$watch("filters", function () {
	            $scope.countFilter = 0;
	            if ($scope.filters.DRE) $scope.countFilter += 1;
	            if ($scope.filters.School) $scope.countFilter += 1;
	            if ($scope.filters.Year) $scope.countFilter += 1;
	            if ($scope.filters.Turn) $scope.countFilter += 1;
	        }, true);
	    }


        /**
		 * @function Configuração inicial para configuração da page.
		 * @name configStart
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
	    function configStart(authorize) {

			$scope.blockPage = false;
			$scope.typeFile = false;
			$scope.downloadUrl = null;
			$scope.searchFilter = false;
			$scope.answerSheetBlocked = authorize.answerSheetBlocked;
			if (authorize === undefined || authorize === null) {
				$scope.blockPage = true;
				$notification.alert("Não foi possível obter as permissões de acesso a página.");
				redirectToList();
				return;
			}
			configTestInformation(authorize);
			if ($scope.params === undefined || $scope.params.test_id === undefined || !parseInt($scope.params.test_id))
                if(parseInt($scope.params.test_id) !== $scope.testInformation.TestId) {
			        $scope.blockPage = true;
			        $notification.alert("Id da prova inválido ou usuário sem permissão.");
			        $scope.safeApply();
			        redirectToList();
			        return;
			    }
			configCurrentSelectedMasterFilters();
			configListFilter();
			configLists();
			$scope.clearFilters();
			getDREs();
			$scope.setPagination();
			$scope.$watch("filters", function () {
			    $scope.countFilter = 0;
			    if ($scope.filters.DRE) $scope.countFilter += 1;
			    if ($scope.filters.School) $scope.countFilter += 1;
			    if ($scope.filters.Year) $scope.countFilter += 1;
			    if ($scope.filters.Turn) $scope.countFilter += 1;
			}, true);
		};

		$scope.searchFilterSchool = function () {
			$scope.searchFilter = true;
			$scope.setPagination();

			sessionStorage.setItem('searchFilterIndexAdministrate', JSON.stringify($scope.filters));
		};


	    /**
		 * @function Limpar Paginação
		 * @name setPagination
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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
		 * @name search
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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
			}//else

		    var params = {
		        'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
		        'dre_id': ($scope.filters.DRE !== undefined && $scope.filters.DRE !== null) ? $scope.filters.DRE.Id : "",
		        'esc_id': schoolId,
		        'crp_ordem': ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0,
		        'ttn_id': ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0
		    };

		    $scope.paginate.paginate(params).then(function (result) {

		        if (result.success) {

		        	setCurrentSelectedMasterFilters();

		        	if (result.lista.length > 0) {

		                $scope.paginate.nextPage();
		                $scope.list.displayed = angular.copy(result.lista);

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
		            $notification[result.type ? result.type : 'error'](result.message);
		            $scope.list.displayed = null;
				}

		    }, function () {
		        $scope.message = true;
		        $scope.list.displayed = null;
		    });
		};


	    /**
         * @function Obtem o number do enum dado o value do enum
         * @name getEnumIntegerByValue
         * @namespace TestAdministrateController
         * @memberOf Controller
         * @private
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
	            return "Gerar folhas de resposta da turma";
	    };


	    /**
		 * @function Agendar geração de folhas de respostas.
		 * @name redirectToAnswerSheet
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
	    $scope.generateAnswerSheet = function __generateAnswerSheet(team, enumerador, lookGenerate) {

	    	if (!lookGenerate) {

	    		var params = {
	    			Id: $scope.testInformation.TestId,
	    			schoolId: team.esc_id,
	    			teamId: team.tur_id,
	    			type: enumerador,
	    		    studentId: 0
	    		};

	    		AnswerSheetModel.generateAnswerSheet(params, function (result) {

	    			if (result.success) {
	    				team.FileAnswerSheet.Id = result.generateTest.FileAnswerSheet.Id;
	    				team.FileAnswerSheet.Name = result.generateTest.FileAnswerSheet.Name;
	    				team.FileAnswerSheet.Path = result.generateTest.FileAnswerSheet.Path;

	    				$scope.downloadUrl = result.generateTest.FileAnswerSheet;
	    				$notification.success(result.message);
	    			}
	    			else {
	    				$notification[result.type ? result.type : 'error'](result.message);
	    			}
	    		});
	    	}//if

	    };

		/**
		* @function Efeua o download da folha de resposta.
		* @name downloadFolhaResposta
		* @namespace TestAdministrateController
		* @memberOf Controller
		* @public
		* @param
		* @return
		*/
	    $scope.downloadFolhaResposta = function _downloadFolhaResposta(team) {
	    	if (team.FileAnswerSheet.Path != null)
	    		window.open("/Test/DownloadFile?Id=" + team.FileAnswerSheet.Id, "_self");
	    };

	    $scope.leavingIndexAdministrate = function __leavingIndexAdministrate() {
	        sessionStorage.setItem('leavingIndexAdministrate', JSON.stringify(true));
	    };

	    /**
		 * @function ComeBack.
		 * @param
		 * @return
		 */
	    $scope.arrivingTest = function __arrivingTest() {
	        sessionStorage.setItem('arrivingTest', JSON.stringify(true));
	        $window.location.href = '/Test';
	    };

	    /**
		 * @function Cancel.
		 * @name cancel
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
	    $scope.cancel = function __cancel() {
	        $window.location.href = '/Test';
	    };


	    /**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace TestAdministrateController
		 * @memberOf Controller
		 * @public
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