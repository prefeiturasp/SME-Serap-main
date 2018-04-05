/**
 * function CorrectionController Controller
 * @namespace Controller
 * @author Julio Cesar Silva 25/11/2015
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("CorrectionController", CorrectionController);

	CorrectionController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', '$http', 'CorrectionModel', 'AdherenceModel', 'AbsenceReasonModel', 'CorrectionApiModel', 'AnswerSheetModel'];


	/**
	 * @function Controller 'Administrar controle da prova'
	 * @name CorrectionController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
     * @param {Object} $window
     * @param {Object} $sce
     * @param {Object} $util
	 * @param {Object} $notification
	 * @param {Object} $timeout
     * @param {Object} CorrectionModel
     * @param {Object} AdherenceModel
     * @param {Object} AbsenceReasonModel
	 * @return
	 */
	function CorrectionController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, $http, CorrectionModel, AdherenceModel, AbsenceReasonModel, CorrectionApiModel, AnswerSheetModel) {

		/**
		  * @function Redirecionar para listagem de provas.
		  * @name redirectToList
		  * @namespace CorrectionController
		  * @memberOf Controller
		  * @private
		  * @param {string} destiny
		  * @return
		  */
		function redirectToList(destiny) {
			$timeout(function __invalidTestId() {
				$window.location.href = destiny;
			}, 3000);
		};


		/**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace CorrectionController
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
				Team: informations.team,
				SchoolName: informations.schoolName,
				Token: informations.token,
				NumberAnswer: informations.numberAnswer === undefined || informations.numberAnswer === null ? 4 : informations.numberAnswer,
				blockCorrection: informations.blockCorrection,
				InCorrection: informations.InCorrection,
				answerSheetBlocked: informations.answerSheetBlocked,
				blockAccess: informations.blockAccess
			};
		};


		/**
		 * @function Configuração das lista
		 * @name configLists
		 * @namespace CorrectionController
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
		 * @function Carregar estudandes de uma turma.
		 * @name getStudents
		 * @namespace CorrectionController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function getStudents() {

			var params = {
				'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
				'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0
			};

			AnswerSheetModel.getStudentBySection(params, function (result) {

				if (result.success) {
					$scope.list.displayed = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		$scope.init = function __init() {
		    $scope.params = $util.getUrlParams();

		    var params = {
		        'test_id': $scope.params.test_id ? $scope.params.test_id : 0,
		        'team_id': $scope.params.team_id ? $scope.params.team_id : 0,
		        'result': false
		    };

		    CorrectionModel.getAuthorize(params, function (result) {
		        if (result.success) {
		            $scope.getAuthorize(result.dados);
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		/**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace CorrectionController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.getAuthorize = function __getAuthorize(authorize) {
			$scope.blockPage = false;


			if (authorize === undefined || authorize === null) {
				$scope.blockPage = true;
				$notification.alert("Não foi possível obter as permissões de acesso a página.");
				redirectToList("/Test");
				return;
			}

			configTestInformation(authorize);

			if ($scope.params === undefined ||
                $scope.testInformation.TestId === undefined || $scope.testInformation.TestId === null ||
                $scope.testInformation.Team === undefined || $scope.testInformation.Team === null ||
                $scope.params.test_id === undefined || $scope.params.team_id === undefined ||
                !parseInt($scope.params.test_id) || !parseInt($scope.params.team_id)) {

				$scope.blockPage = true;
				$notification.alert("Id da prova inválido ou Id da turma inválido.");
				redirectToList("/Test");
				return;
			}

			if (parseInt($scope.params.test_id) !== $scope.testInformation.TestId || parseInt($scope.params.team_id) !== $scope.testInformation.Team.Id) {

				$scope.blockPage = true;
				$notification.alert("Id da prova não corresponde ou Id da turma não corresponde.");
				redirectToList("/Test");
				return;
			}

			if ($scope.testInformation.NumberAnswer === 0) {
				$scope.blockPage = true;
				$notification.alert("A prova não possui gabarito.");
				redirectToList('/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId);
				return;
			}

			if ($scope.testInformation.blockAccess) {
			    $scope.blockPage = true;
			    $notification.alert("Usuário não possui permissão para realizar essa ação.");
			    redirectToList('/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId);
			    return;
			}

			configLists();
			getStudents();
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
		$scope.generateAnswerSheet = function __generateAnswerSheet(student, enumerador, lookGenerate) {

			if (!lookGenerate) {
				var params = {
					Id: $scope.testInformation.TestId,
					schoolId: $scope.testInformation.Team.esc_id,
					teamId: $scope.testInformation.Team.Id,
					type: enumerador,
					studentId: student.alu_id
				};

				AnswerSheetModel.generateAnswerSheet(params, function (result) {

					if (result.success) {
						student.FileAnswerSheet = result.generateTest.FileAnswerSheet;
						$notification.success(result.message);
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}

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
		$scope.downloadFolhaResposta = function _downloadFolhaResposta(student) {

			if (student.FileAnswerSheet && student.FileAnswerSheet.Path != null)
				window.open("/File/DownloadFile?Id=" + student.FileAnswerSheet.Id, "_self");
		};

	    /**
         * @function arrivingIndexAdministrate.
         * @param
         * @return
         */
		$scope.arrivingIndexAdministrate = function __arrivingIndexAdministrate() {
		    sessionStorage.setItem('arrivingIndexAdministrate', JSON.stringify(true));
		    $window.location.href = '/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId //+ '&esc_id=' + $scope.testInformation.Team.esc_id;;
		};

		/**
		 * @function Cancel.
		 * @name cancel
		 * @namespace CorrectionController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.cancel = function __cancel() {
		    $window.location.href = '/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId //+ '&esc_id=' + $scope.testInformation.Team.esc_id;;
		};


		/**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace CorrectionController
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
		 * @function Chamado após tags html serem lidas pelo browser. 
		 * @name __postHtmlCompile
		 * @namespace CorrectionController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		angular.element(document).ready(function __postHtmlCompile() {

		});


		/**
		 * @function Chamado após angular ser 'digerido' (diggest). 
		 * @name __cycleAngular
		 * @namespace CorrectionController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		var hasRegistered = false;
		$scope.$watch(function __cycleAngular() {
			if (hasRegistered) return;
			hasRegistered = true;
			$scope.$$postDigest(function __postDisgestAngular() {
				hasRegistered = false;
				//TODO
			});
		});


	};

})(angular, jQuery);