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
		.controller("CorrectionController", CorrectionController)
        //.config($config);

	CorrectionController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', '$http', 'CorrectionModel', 'AdherenceModel', 'AbsenceReasonModel', 'CorrectionApiModel', 'AnswerSheetModel'];
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
     * @param {Object} CorrectionModel
     * @param {Object} AdherenceModel
     * @param {Object} AbsenceReasonModel
	 * @returns
	 */
	function CorrectionController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, $http, CorrectionModel, AdherenceModel, AbsenceReasonModel, CorrectionApiModel, AnswerSheetModel) {

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
				Team: informations.team,
				SchoolName: informations.schoolName,
				Token: informations.token,
				NumberAnswer: informations.numberAnswer === undefined || informations.numberAnswer === null ? 4 : informations.numberAnswer,
				blockCorrection: informations.blockCorrection,
				InCorrection: informations.InCorrection,
				answerSheetBlocked: informations.answerSheetBlocked,
				blockAccess: informations.blockAccess,
				electronicTest: informations.electronicTest
			};
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

		/**
		 * @function Realizar busca por um estudante em uma lista de estudantes
		 * @param {object} alu_id
		 * @param {array} lista
		 * @return object
		 */
		function findStudentById(alu_id, lista) {

			if (!lista)
				return null;

			var i = 0, m = lista.length;

			for (i; i < m; i++)
				if (lista[i].alu_id)
					if (lista[i].alu_id === alu_id)
						return { index: i, instance: lista[i] };

			return null;
		};

		/**
		 * @function Realizar busca por um item
		 * @param {object} item_id
		 * @param {array} lista
		 * @return object
		 */
		function findItemById(item_id, lista) {

			if (!lista)
				return null;

			var i = 0, m = lista.length;

			for (i; i < m; i++)
				if (lista[i].Item_Id)
					if (lista[i].Item_Id === item_id)
						return { index: i, instance: lista[i] };

			return null;
		};

		/**
         * @function Retorno do save das respostas do gabarito do aluno
         * @param {object} result
         * @return boolean
         */
		function endCorrectionSave(result) {

			$scope.safeApply();

			var student = findStudentById(result.alu_id, $scope.list.displayed);
			var item = findItemById(result.item_id, student.instance.Items);
			var itemBackup = findItemById(result.item_id, $scope.backupItems);

			if (result.success) {
				clearBackupCorrectionSave(itemBackup.index);
			}
			else {
				rollbackCorrectionSave(student, itemBackup, item);
				$notification[result.type ? result.type : 'error'](result.message);
			}

			student.instance.TotalAnswered = verifiqueAnswers(student.instance);
			$scope.safeApply();
		};

		/**
         * @function Retorno do save do motivo de ausência do aluno
         * @param {object} result
         * @return boolean
         */
		function endAbsenceSave(result) {

			$scope.safeApply();

			var student = findStudentById(result.alu_id, $scope.list.displayed);
			var backupStudent = findStudentById(result.alu_id, $scope.backupStudents);

			if (result.success) {

				clearBackupAbsenceReason(backupStudent.index);

				for (var i = 0; i < student.instance.Items.length; i++) {
					clearAlternativeSelection(student.index, i);
				}
				$scope.currentStudent = undefined;
			}
			else {

				rollbackAbsenceSave(student, backupStudent);
				$notification[result.type ? result.type : 'error'](result.message);
			}

			student.instance.TotalAnswered = verifiqueAnswers(student.instance);

			$scope.safeApply();
		};

		/**
         * @function Reverter alterações ao salvar a resposta de um aluno
         * @param {object} student
         * @param {object} itemBackup
         * @return boolean
         */
		function rollbackCorrectionSave(student, itemBackup, item) {

			if (student !== null && itemBackup !== null, item !== null) {
				$scope.list.displayed[student.index].Items[item.index] = angular.copy(itemBackup.instance);
				clearBackupCorrectionSave(itemBackup.index);
			}
		};

		/**
         * @function Reverter alterações ao salvar o motivo de ausência do aluno
         * @param {object} student
         * @param {object} backupStudent
         * @return
         */
		function rollbackAbsenceSave(student, backupStudent) {

			if (student !== null && backupStudent !== null) {
				$scope.list.displayed[student.index] = angular.copy(backupStudent.instance);
				clearBackupAbsenceReason(backupStudent.index);
			}
		};

		/**
         * @function Limpar lista de backups pra transações de absenceReason
         * @param {int} index
         * @return
         */
		function clearBackupAbsenceReason(index) {
			$scope.backupStudents.splice(index, 1);
		};

		/**
         * @function Limpar lista de backups pra transações de CorrectionSave
         * @param {int} index
         * @return
         */
		function clearBackupCorrectionSave(index) {
			$scope.backupItems.splice(index, 1);
		};

		/**
         * @function Obter lista com motivos de ausência
         * @param
         * @return
         */
		function getAbsenceReason() {

			AbsenceReasonModel.loadCombo({}, function (result) {

				if (result.success) {
					$scope.absenceReasonList = result.lista;
					getStudents();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Carregar estudandes de uma turma.
		 * @param {object} _callback
		 * @return
		 */
		function getStudents(_callback) {

			var params = {
				'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
				'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0
			};

			CorrectionModel.getStudentBySection(params, function (result) {

				if (result.success) {
					$scope.list.displayed = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
				if (_callback) _callback();
			});
		};

		/**
		 * @function Carrega o gabarito de um aluno
         * @param {object} student
		 * @param {int} $indexStudent
		 * @return
		 */
		function loadStudentAlternatives(student, $indexStudent) {

			if (student === undefined || student === null || student.Items !== undefined || student.AbsenceReason_id == 1 || student.AbsenceReason_id == 2)
				return;

			var params = {
				'test_id': $scope.testInformation.TestId,
				'team_id': $scope.testInformation.Team.Id,
				'alu_id': student.alu_id
			};

			CorrectionModel.getStudentAnswer(params, function (result) {

				if (result.success) {
					$scope.list.displayed[$indexStudent].Items = result.lista;
					$scope.list.displayed[$indexStudent].TotalAnswers = result.lista.length;
					$scope.list.displayed[$indexStudent].TotalAnswered = verifiqueAnswers($scope.list.displayed[$indexStudent]);
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Reset da questão antes de inserir nova resposta
         * @param {object} student
		 * @param {int} $indexStudent
		 * @return
		 */
		function clearAlternativeSelection($indexStudent, $indexItem) {

			var student = $scope.list.displayed[$indexStudent];

			for (var a = 0; a < student.Items[$indexItem].Alternatives.length; a++)
				student.Items[$indexItem].Alternatives[a].Selected = false;

			student.Items[$indexItem].Null = false;
			student.Items[$indexItem].StrikeThrough = false;
		};

		/**
         * @function Verificar respostas preenchidas pelo usuário
         * @param {object} student
         * @return
         */
		function verifiqueAnswers(student) {

			var total = 0;

			if (student === null || student === undefined || (student.AbsenceReason_id !== 0 && student.AbsenceReason_id !== null) || student.Items === undefined || student.Items === null)
				return total;

			for (var a = 0; a < student.Items.length; a++) {

				//respostas [nula | rasurada]
				if (student.Items[a].Null || student.Items[a].StrikeThrough) {
					total += 1;
				}
				else {

					//respostas [A | B | C ...]
					for (var b = 0; b < student.Items[a].Alternatives.length; b++) {

						if (student.Items[a].Alternatives[b].Selected) {
							total += 1;
							break;
						}
					}
				}
			}

			return total;
		};

		/**
         * @function Obter as labels dos parametros ['Null' | 'Rasurada']
         * @param
         * @return
         */
		function setNullAndErased() {

			$scope.Erased = settingsTest[1].Value;
			$scope.Null = settingsTest[2].Value;
		};

		/**
         * @function Envia a alternativa selecionada para o servidor
         * @param
         * @return
         */
		function correctionSave(param) {
		    $http.defaults.headers.common["Authorization"] = $scope.testInformation.Token;
		    CorrectionApiModel.correction(param)
            .then(function (result) {
                endCorrectionSave(result.data);
            })
            .catch(function (result) {
                //$notification[result.type ? result.type : 'error'](result.message);
                //$scope.getAuthorize();
            });
		};

		/**
         * @function Envia a alternativa selecionada para o servidor
         * @param
         * @return
         */
		function invokeAbsence(param) {
		    $http.defaults.headers.common["Authorization"] = $scope.testInformation.Token;
		    CorrectionApiModel.absence(param).then(function (result) {
		        endAbsenceSave(result.data);
		    })
            .catch(function (result) {
                //$notification[result.type ? result.type : 'error'](result.message);
                //$scope.getAuthorize();
            });
		};

		$scope.init = function __init() {
		    $scope.params = $util.getUrlParams();
		    $scope.getAuthorize(function () {
		        configLists();
		        getAbsenceReason();
		        setNullAndErased();
		        var parametroGabarito = getParameterValue(parameterKeys[0].DELETE_BATCH_FILES);
		        $scope.baixarGabarito = parametroGabarito ? $.parseJSON(parametroGabarito.toLowerCase()) : false;

		        var parametroOcultarBotaoFinalizarEnviar = getParameterValue(parameterKeys[0].OCULTAR_BOTAO_FINALIZAR_ENVIAR);
		        $scope.ocultarBotaoFinalizarEnviar = parametroOcultarBotaoFinalizarEnviar ? $.parseJSON(parametroOcultarBotaoFinalizarEnviar.toLowerCase()) : false;
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
		    CorrectionModel.getAuthorize(params, function (result) {
		        if (result.success) {
		            $scope.blockPage = false;
		            if (result.dados === undefined || result.dados === null) {
		                $scope.blockPage = true;
		                $notification.alert("Não foi possível obter as permissões de acesso a página.");
		                redirectToList("/Test");
		                return;
		            }
		            configTestInformation(result.dados);
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

		/**
         * @function Finalizar e enviar correção
         * @param
         * @return
         */
		$scope.finishAndSend = function __finishAndSend() {
			var params = {
				'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
				'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0
			};
			CorrectionModel.finalizeCorrection(params, function (result) {
				if (result.success) {
					$notification.success("Resultados da prova enviados com sucesso.");
					redirectToList('/Test/IndexStudentResponses?test_id=' + $scope.testInformation.TestId);
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};



		/**
         * @function Enviar para correção automática
         * @param
         * @return
         */
		$scope.sendToAutomaticCorrection = function __sendToAutomaticCorrection() {

			$window.location.href = '/AnswerSheet/IndexBatchDetails?test_id=' + $scope.testInformation.TestId + '&team_id=' + $scope.testInformation.Team.Id;
		};

		/**
		 * @function Salva um motivo de ausência dado um aluno
		 * @param {object} student
		 * @return
		 */
		$scope.absenceSave = function __absenceSave(student) {

			$scope.backupStudents.push(angular.copy($scope.currentAbsenceReasonStudent));
			var AbsenceReasonId = student.AbsenceReason_id === undefined || student.AbsenceReason_id === null ? 0 : student.AbsenceReason_id;
			var AlunoId = student.alu_id === undefined || student.alu_id === null ? 0 : student.alu_id;

			var param = {
				alu_id: AlunoId,
				absenceReasonId: AbsenceReasonId
			};
			invokeAbsence(param);
		};

		/**
		 * @function Seleciona uma nota para Questão
         * @param {int} $indexStudent
         * @param {int} $indexItem
         * @param {int} $indexAlternative
		 * @return
		 */
		$scope.selectAlternative = function __selectAlternative($indexStudent, $indexItem, $indexAlternative, Null, StrikeThrough) {

			var student = $scope.list.displayed[$indexStudent];

			$scope.backupItems.push(angular.copy(student.Items[$indexItem]));

			clearAlternativeSelection($indexStudent, $indexItem);

			if (Null === null && StrikeThrough === null) {
				student.Items[$indexItem].Alternatives[$indexAlternative].Selected = true;
			}
			else if (Null === true) {
				student.Items[$indexItem].Null = true;
			}
			else if (StrikeThrough === true) {
				student.Items[$indexItem].StrikeThrough = true;
			}

			var param = {
				alu_id: student.alu_id,
				alternative_id: $indexAlternative !== null && $indexAlternative !== undefined ? student.Items[$indexItem].Alternatives[$indexAlternative].Id : 0,
				item_id: student.Items[$indexItem].Item_Id,
				n: student.Items[$indexItem].Null,
				r: student.Items[$indexItem].StrikeThrough,
				manual : true
			}

			correctionSave(param);
		};

		/**
		 * @function Selecionar o motivo de ausencia antes do ng-change
		 * @param {object} student
		 * @return
		 */
		$scope.setCurrentAbsenceReasonStudent = function __setCurrentAbsenceReasonStudent(student) {
			$scope.currentAbsenceReasonStudent = angular.copy(student);
		};

		/**
		 * @function Remover parenteses da label da alternativa
		 * @param {string} Label
		 * @return
		 */
		$scope.removeBracket = function __removeBracket(label) {

			if (label === undefined || label === null)
				return "";

			return label.replace(/["'()]/g, "");
		};

		/**
		 * @function Abrir as notas de um aluno somente quando este não possuir um motivo de ausencia
		 * @param {object} student
		 * @param {int} $indexStudent
		 * @return
		 */
		$scope.openCurrentStudent = function __openCurrentStudent(student, $indexStudent) {

			if ($scope.currentStudent === student) {
				$scope.currentStudent = undefined;
				return;
			}

			if (student.AbsenceReason_id != null && student.AbsenceReason_id != undefined && student.AbsenceReason_id > 0) {
			    return;
			} else if ($scope.AbsenceReason_id != null && $scope.AbsenceReason_id != undefined && $scope.AbsenceReason_id > 0) {
			    return;
			}
			else {
			    $scope.currentStudent = student;
			}
		};

		/**
		 * @function Efeua o download da folha de resposta.
		 * @param
		 * @return
		 */
		$scope.downloadFolhaResposta = function _downloadFolhaResposta(student) {
			if (student.FileAnswerSheet && student.FileAnswerSheet.Path != null)
				window.open("/File/DownloadFile?Id=" + student.FileAnswerSheet.Id, "_self");
		};

		$scope.downloadStudentFeedback = function __downloadStudentFeedback(student) {
		    window.open("/AnswerSheet/GetStudentFile?"
                + "testId=" + $scope.testInformation.TestId
                + "&studentId=" + student.alu_id
                + "&sectionId=" + $scope.testInformation.Team.Id, "_self");
		};

	    /**
         * @function arrivingStudentResponse.
         * @param
         * @return
         */
		$scope.arrivingStudentResponse = function __arrivingStudentResponse() {
		    sessionStorage.setItem('arrivingStudentResponse', JSON.stringify(true));
		    $window.location.href = '/Test/IndexStudentResponses?test_id=' + $scope.testInformation.TestId //+ '&esc_id=' + $scope.testInformation.Team.esc_id;;
		};

		/**
		 * @function Cancel.
		 * @param
		 * @returns
		 */
		$scope.cancel = function __cancel() {
			$window.location.href = '/Test/IndexStudentResponses?test_id=' + $scope.testInformation.TestId //+ '&esc_id=' + $scope.testInformation.Team.esc_id;;
		};

		/**
		 * @function Forçar a atualização (diggest) do angular
		 * @param
		 * @returns
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
		 * @param
		 * @returns
		 */
		angular.element(document).ready(function __postHtmlCompile() {

		});

		/**
		 * @function Chamado após angular ser 'digerido' (diggest). 
		 * @param
		 * @returns
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