(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("StudentResultsGraphicsController", StudentResultsListController)
        //.config($config);

	StudentResultsListController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', '$http', 'studentResultsGraphicsModel', 'AdherenceModel', 'AbsenceReasonModel', 'StudentResultsGraphicsApiModel', 'AnswerSheetModel'];
	$config.$inject = ['$apiSettingConfigProvider'];

    /**
	 * @function Config para provider de ApiSetting
	 * @param {Object} $rootScope
	 * @returns
	 */
	function $config($apiSettingConfigProvider) {
	    var params = $apiSettingConfigProvider.getUrlParams();
	    $apiSettingConfigProvider.setUrlMethodExcluded("GetAuthorize");
	    $apiSettingConfigProvider.setUrlAuthentication(base_url("/Correction/GetAuthorize") + "?result=false&team_id=" + params.team_id + "&test_id=" + params.test_id + "&r=" + new Date().toISOString());
	};

	/**
	 * @function Controller 'Administrar controle da prova'
	 * @param {Object} $rootScope
	 * @param {Object} $scope
     * @param {Object} $window
     * @param {Object} $sce
     * @param {Object} $util
	 * @param {Object} $notification
	 * @param {Object} $timeout
     * @param {Object} studentResultsGraphicsModel
     * @param {Object} AdherenceModel
     * @param {Object} AbsenceReasonModel
	 * @returns
	 */
	function StudentResultsListController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, $http, studentResultsGraphicsModel, AdherenceModel, AbsenceReasonModel, StudentResultsGraphicsApiModel, AnswerSheetModel) {

	    $scope.backupItems = [];
		$scope.backupStudents = [];

		/**
		  * @function Redirecionar para listagem de provas.
		  * @param {string} destiny
		  * @return
		  */
		function redirectToList(destiny) {
			$timeout(function __invalidTestId() {
				$window.location.href = destiny;
			}, 3000);
		};

		/**
		 * @function Configuração das lista
		 * @param
		 * @return
		 */
		function configLists() {

			$scope.list = {
				displayed: []
			};
		};



		$scope.init = function __init() {
		    $scope.params = $util.getUrlParams();
		    $scope.getAuthorize(function () {
		        configLists();
		    });
		};

		/**
		 * @function Obter objeto inicial para configuração da page.
		 * @param {object} _callback
		 * @return
		 */
		$scope.getAuthorize = function __getAuthorize(_callback) {
		    var params = {
		        'test_id': $scope.params.test_id ? $scope.params.test_id : 0,
		        'team_id': $scope.params.team_id ? $scope.params.team_id : 0,
		        'result': false
		    };
		    studentResultsGraphicsModel.getAuthorize(params, function (result) {
		        if (result.success) {
		            $scope.blockPage = false;
		            if (result.dados === undefined || result.dados === null) {
		                $scope.blockPage = true;
		                $notification.alert("Não foi possível obter as permissões de acesso a página.");
		                redirectToList("/Test");
		                return;
		            }
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
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		            redirectToList("/");
		        }
		        if (_callback) _callback();
		    });
		};
	};

})(angular, jQuery);