/**
 * function TestPermissionController Controller
 * @namespace Controller
 * @author Haila Pelloso 09/03/2017
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("TestPermissionController", TestPermissionController);

	TestPermissionController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$pager', '$notification', '$timeout', 'TestPermissionModel'];


	/**
	 * @function Controller 'Administrar permissões da prova'
	 * @name TestPermissionController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $notification
	 * @param {Object} $timeout
     * @param {Object} TestPermissionModel
	 * @return
	 */
	function TestPermissionController($rootScope, $scope, $window, $sce, $util, $pager, $notification, $timeout, TestPermissionModel) {

	    $scope.paginate = $pager(TestPermissionModel.getTestsPermissions);
	    $scope.pageSize = 100;

	    /**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace TestPermissionController
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
		  * @namespace TestPermissionController
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
		 * @function Configuração das listas
		 * @name configLists
		 * @namespace TestPermissionController
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
		 * @function Obter objeto inicial para configuração da page.
		 * @name init
		 * @namespace TestPermissionController
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
		 * @namespace TestPermissionController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
	    function getAuthorize() {
	        TestPermissionModel.getAuthorize($scope.params, function (result) {
	            if (result.success) {
	                configStart(result.dados);
	            } else {
	                $scope.blockPage = true;
	                $notification[result.type ? result.type : 'error'](result.message);
	                $scope.safeApply();
	                redirectToList();
	                return;
	            }
	        });
	    };


        /**
		 * @function Configuração inicial para configuração da page.
		 * @name configStart
		 * @namespace TestPermissionController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
	    function configStart(authorize) {

			$scope.blockPage = false;
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
			configLists();
			$scope.setPagination();
		};

	    /**
		 * @function Limpar Paginação
		 * @name setPagination
		 * @namespace TestPermissionController
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
		 * @namespace TestPermissionController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.search = function __search() {

		    var params = {
		        'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0
		    };

		    $scope.paginate.paginate(params).then(function (result) {

		        if (result.success) {

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
		 * @function Save
		 * Salva a alteração da permissão
		 * @private
		 */
		$scope.save = function __save(entity) {
		    var params = {
		        'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
		        'permissions': $scope.list.displayed
		    };

		    TestPermissionModel.savePermission(params, function (result) {
		        if (result.success) {
		            $notification.success(result.message);
		        } else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		        $scope.safeApply();
		    });
		};

	    /**
		 * @function Cancel.
		 * @name cancel
		 * @namespace TestPermissionController
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
		 * @namespace TestPermissionController
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
	};

})(angular, jQuery);