(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("StudentTestSessionController", StudentTestSessionController);

	StudentTestSessionController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$pager', '$notification', '$timeout', '$location', 'TestAdministrateModel','StudentTestSessionModel'];


	/**
		 * @function Controller 'Administrar controle da prova'
		 * @name StudentTestSessionController
		 * @namespace Controller
		 * @memberOf appMain
		 * @param {Object} $rootScope
		 * @param {Object} $scope
		 * @param {Object} $notification
		 * @param {Object} $timeout
		 * @param {Object} TestAdministrateModel
		 * @return
		 */
	function StudentTestSessionController($rootScope, $scope, $window, $sce, $util, $pager, $notification, $timeout, $location, TestAdministrateModel, StudentTestSessionModel) {

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

			if (authorize.testId != 0 && authorize.esc_id != 0 && authorize.team_id != 0) {
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
							redirectToList();
							return;
						}

						getStudentsSession();
					}
					else {
						$scope.blockPage = true;
						$notification.alert("Não foi possível obter os dados de acesso a página.");
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
				redirectToList();
				return;
			}
		};

		$(function () {
			$('[data-toggle="popover"]').popover()
		})

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
					redirectToList();
					return;
				}
			});
		};

	    /**
		 * @function Pesquisa.
		 * @param
		 * @return
		 */
		function getStudentsSession() {

			var params = {
				'test_id': $scope.params.test_id,
				'team_id': $scope.params.team_id,
				'esc_id': $scope.params.esc_id,
			};

			StudentTestSessionModel.getStudentsSession(params, function (result) {
				if (result.success) {
					if (result.dados.length > 0) {
						$scope.list = angular.copy(result.dados);
					} else {
						$scope.message = true;
						$scope.list = null;
					}
				}
				else {
					$scope.list = null;
				}

			}, function () {
				$scope.message = true;
				$scope.list = null;
			});
		};

		$scope.openModalTempoDeCadaSessao = function openModal(sessoes) {
			$scope.sessoes = sessoes;
			angular.element("#tempoDeCadaSessao").modal({ backdrop: 'static' });
		}

		$scope.arrivingTest = function __arrivingTest() {
			$window.location.href = '/Test/IndexStudentResponses?test_id=' + $scope.params.test_id;
		};
	};

})(angular, jQuery);