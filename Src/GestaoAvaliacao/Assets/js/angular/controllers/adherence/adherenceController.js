/**
 * function Adherence Controller
 * @namespace Controller
 * @author Julio Cesar da Silva - 06/11/2015 - 11/11/2015
 */
(function (angular, $) {

	'use strict';

	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	angular
		.module('appMain')
		.controller("AdherenceController", AdherenceController);

	AdherenceController.$inject = ['$rootScope', '$scope', '$notification', '$pager', '$util', 'AdherenceModel', 'AdherenceApiModel', '$window', '$timeout', '$http'];

	/**
	 * @function Controller 'Aderir Escolas'
	 * @name AdherenceController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $notification
	 * @param {Object} $pager
	 * @param {Object} $util
	 * @param {Object} AdherenceModel
	 * @param {Object} $window
	 */
	function AdherenceController($rootScope, $scope, $notification, $pager, $util, AdherenceModel, AdherenceApiModel, $window, $timeout, $http) {

	    $scope.pageSize = 10;
	    $scope.countFilter = 0;

		/**
		  * @function Textos dos headers das modais de aviso.
		  * @name configHtmlTitleModal
		  * @namespace AdherenceController
		  * @memberOf Controller
		  * @private
		  * @param
		  * @return
		  */
		function configHtmlTitleModal() {

			$scope.selectAllAdherenceModalTitle = "";
		};

		/**
		  * @function Redirecionar para listagem de provas.
		  * @name redirectToList
		  * @namespace AdherenceController
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
		 * @function Configuração das variaveis de mecânica do sistema.
		 * @name configMechanics
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param {boolean} allAdhered
		 * @return
		 */
		function configMechanics(allAdhered) {

			$scope.mechanics = {
				AllAdhered: (allAdhered === undefined || allAdhered === null) ? false : allAdhered,
				AllAdheredOnPagination: false
			};
		};

		/**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace AdherenceController
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
				AllAdhered: (informations.testAllAdhered !== null) ? informations.testAllAdhered : false,
				Token: informations.token,
				Global: informations.global === undefined || informations.global === null ? false : informations.global
			};
		};

		/**
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @name configListFilter
		 * @namespace AdherenceController
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
				Teams: []
			};
		};

		/**
		 * @function Configuração dos filtros
		 * @name configFilters
		 * @namespace AdherenceController
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
				Turn: undefined,
				Adherence: false
			};
		};

		/**
		 * @function Configuração das listas [Pesquisa | Selecionados | Excluidos]
		 * @name configLists
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function configLists() {

			$scope.list = {
				displayed: null
			};
		};

		/**
		 * @function Configuração das propriedades para exibição de estatísticas
		 * @name configStatistics
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function configStatistics() {

			$scope.statistics = {
				SelectedTeams: 0,
				SelectedSchools: 0,
				TotalSchools: 0,
				Open: false
			};
		};

		/**
		 * @function Obter todas DREs
		 * @name getDREs
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function getDREs() {

			AdherenceModel.getDRESimple({}, function (result) {

				if (result.success) {
					$scope.listFilter.DREs = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Obter todas as turmas de uma escola
		 * @name getTeams
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param {object} params
         * @param {int} $index
		 * @return
		 */
		function getTeams(params, $index) {

			AdherenceModel.getSectionGrid(params, function (result) {

				if (result.success) {
					$scope.list.displayed[$index]['Teams'] = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
         * @function Obter todas as turmas selecionadas de uma escola
         * @name getSelectedTeams
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {object} params
         * @param {int} $index
         * @return
         */
		function getSelectedTeams(params, $index) {

			AdherenceModel.getSelectedSection(params, function (result) {

				if (result.success) {
					if (result.lista.length > 0)
						$scope.list.displayed[$index]['Teams'] = result.lista;
					else
						$notification[result.type ? result.type : 'error'](result.message);
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

	    /**
		 * @function Obter todas os alunos da turma de uma escola
		 * @name getStudents
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param {object} params
         * @param {int} $index
		 * @return
		 */
		function getStudents(params, $indexSchool, $indexTeam) {

		    AdherenceModel.getStudentsGrid(params, function (result) {

		        if (result.success) {
		            $scope.list.displayed[$indexSchool]['Teams'][$indexTeam]['Students'] = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

	    /**
         * @function Obter todas os alunos selecionados da turma de uma escola
         * @name getSelectedStudents
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {object} params
         * @param {int} $index
         * @return
         */
		function getSelectedStudents(params, $indexSchool, $indexTeam) {

		    AdherenceModel.getSelectedStudents(params, function (result) {

		        if (result.success) {
		            if (result.lista.length > 0)
		                $scope.list.displayed[$indexSchool]['Teams'][$indexTeam]['Students'] = result.lista;
		            else
		                $notification[result.type ? result.type : 'error'](result.message);
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		/**
		 * @function Realizar busca em uma lista dado um elem
		 * @name buscarElem
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @private
		 * @param {object} elem
		 * @param {array} lista
		 * @return object
		 */
		function buscarElem(elem, lista) {

			if (!lista)
				return null;

			var i = 0, m = lista.length;

			for (i; i < m; i++)
				if (lista[i].Id)
					if (lista[i].Id === elem.Id)
						return { index: i, instance: lista[i] };

			return null;
		};

		/**
         * @function Obtem o number do enum dado o value do enum
         * @name getEnumSelectionTypeStatus
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {string} _key
         * @return
         */
		function getEnumSelectionTypeStatus(_key) {

			for (var key in $scope.enumSelectionType)
				if (_key === $scope.enumSelectionType[key])
					return key;
		};

		/**
         * @function Obtem o number do enum dado o value do enum
         * @name getEnumEntityTypeStatus
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {string} _key
         * @return
         */
		function getEnumEntityTypeStatus(_key) {

			for (var key in $scope.enumEntityType)
				if (_key === $scope.enumEntityType[key])
					return parseInt(key);
		};

		/**
         * @function Verifica a escola tem turmas parcialmente selecionadas, todas selecionadas ou nenhuma selecionada
         * @name schoolStateTeams
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {object} _school
         * @param {boolean} select
         * @return boolean
         */
		function schoolStateTeams(teams, select) {

			if (teams === undefined || teams === null) {

				if (select) {
					return getEnumSelectionTypeStatus('Selecionado');
				}
				else {
					return getEnumSelectionTypeStatus('Não Selecionado');
				}
			}

			var count = 0;

			for (var i = 0; i < teams.length; i++)
				if (teams[i].Selected === true)
					count += 1;

			if (count === 0)
				return getEnumSelectionTypeStatus('Não Selecionado');
			else if (count === teams.length)
				return getEnumSelectionTypeStatus('Selecionado');
			else if (count > 0 && count < teams.length)
				return getEnumSelectionTypeStatus('Parcial');
		};

	    /**
         * @function Verifica a turma tem alunos parcialmente selecionadas, todas selecionados ou nenhum selecionado
         * @name teamStateStudents
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {object} _school
         * @param {boolean} select
         * @return boolean
         */
		function teamStateStudents(students, select) {

		    if (students === undefined || students === null) {

		        if (select) {
		            return getEnumSelectionTypeStatus('Selecionado');
		        }
		        else {
		            return getEnumSelectionTypeStatus('Não Selecionado');
		        }
		    }

		    var count = 0;

		    for (var i = 0; i < students.length; i++)
		        if (students[i].Selected === true)
		            count += 1;

		    if (count === 0)
		        return getEnumSelectionTypeStatus('Não Selecionado');
		    else if (count === students.length)
		        return getEnumSelectionTypeStatus('Selecionado');
		    else if (count > 0 && count < students.length)
		        return getEnumSelectionTypeStatus('Parcial');
		};

		/**
         * @function Retorno do save do save
         * @name proccessResult
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {object} result
         * @param {object} message
         * @return boolean
         */
		function proccessResult(result) {

			$scope.safeApply();

			if (result.success) {
				$scope.statistics.Open = false;
				removeSuccessBatch(result);
			}
			else {
				rollback(result);
			}

			$scope.safeApply();
		};

		function removeSuccessBatch(result) {

			if ($scope.savingEntity.batch === true)
				for (var a = 0; a < $scope.savingEntity.entity.length; a++)
					if (parseInt($scope.savingEntity.entity[a].Id) === result.id)
						$scope.savingEntity.entity.splice(a, 1);
		};

		function rollback(result) {

			if ($scope.savingEntity.batch === true)
				rollbackBatchSchool(result);
			else if ($scope.savingEntity.entityType === getEnumEntityTypeStatus('Escola'))
				rollbackSchool(result);
			else if ($scope.savingEntity.entityType === getEnumEntityTypeStatus('Turma'))
				rollbackTeam(result)
			else if ($scope.savingEntity.entityType === getEnumEntityTypeStatus('Aluno') && $scope.savingEntity.blocked === true)
			    rollbackStudentBlocked(result)
			else if ($scope.savingEntity.entityType === getEnumEntityTypeStatus('Aluno'))
			    rollbackStudent(result)
        };

		function rollbackBatchSchool(result) {

			for (var a = 0; a < $scope.savingEntity.entity.length; a++) {

				if (parseInt($scope.savingEntity.entity[a].Id) === result.id) {

					var _school = buscarElem($scope.savingEntity.entity[a], $scope.list.displayed);

					if (_school !== null) {
						_school.instance.Open = false;
						_school.instance.Selected = $scope.savingEntity.select;
						_school.instance.Status = $scope.savingEntity.entity[a].Status;

						if (_school.instance['Teams'] !== undefined && _school.instance['Teams'] !== null) {

							for (var i = 0; i < _school.instance.Teams.length; i++) {
								_school.instance.Teams[i].Selected = _school.instance.Selected;
								_school.instance.Teams[i].Status = _school.instance.Status;

								if (_school.instance.Teams[i]['Students'] !== undefined && _school.instance.Teams[i]['Students'] !== null) {
								    for (var j = 0; j < _school.instance.Teams[i].Students.length; j++) {
								        _school.instance.Teams[i].Students[j].Selected = _school.instance.Selected;
								    }
								}

							}
						}
					}

					$scope.savingEntity.entity.splice(a, 1);
				}
			}

			$notification[result.type ? result.type : 'error'](result.message + " (Processamento em lote)");
			$scope.mechanics.AllAdheredOnPagination = $scope.savingEntity.select;

		};

		function rollbackSchool(result) {

			var _school = buscarElem($scope.savingEntity.entity, $scope.list.displayed);

			if (_school !== null) {

				_school.instance.Open = false;
				_school.instance.Selected = !_school.instance.Selected;
				_school.instance.Status = $scope.savingEntity.entity.Status;

				if (_school.instance['Teams'] !== undefined && _school.instance['Teams'] !== null) {

					for (var i = 0; i < _school.instance.Teams.length; i++) {
						_school.instance.Teams[i].Selected = _school.instance.Selected;
						_school.instance.Teams[i].Status = _school.instance.Status;
					}
				}
			}

			$notification[result.type ? result.type : 'error'](result.message);
		};

		function rollbackTeam(result) {

			var _school = buscarElem($scope.savingEntity.entityParent, $scope.list.displayed);

			if (_school !== null) {

				var team = buscarElem($scope.savingEntity.entity, _school.instance.Teams);
				_school.instance.Teams[team.index].Selected = !_school.instance.Teams[team.index].Selected;
				_school.instance.Status = $scope.savingEntity.entityParent.Status;
			}

			$notification[result.type ? result.type : 'error'](result.message);
		};

		function rollbackStudentBlocked(result) {
		    var _team = $scope.savingEntity.entityParent;

		    if (_team !== null) {

		        var student = buscarElem($scope.savingEntity.entity, _team.Students);
		        $scope.list.displayed[$scope.savingEntity.indexSchool].Teams[$scope.savingEntity.indexTeam].Students[$scope.savingEntity.indexStudent].Status = $scope.savingEntity.statusOld;
		        $scope.list.displayed[$scope.savingEntity.indexSchool].Teams[$scope.savingEntity.indexTeam].Students[$scope.savingEntity.indexStudent].Selected = $scope.savingEntity.selectedOld;
		        _team.Status = $scope.savingEntity.entityParent.Status;
		    }

		    $notification[result.type ? result.type : 'error'](result.message);
		};

		function rollbackStudent(result) {
		    var _team = $scope.savingEntity.entityParent;

		    if (_team !== null) {

		        var student = buscarElem($scope.savingEntity.entity, _team.Students);
		        _team.Students[student.index].Selected = !_team.Students[student.index].Selected;
		        _team.Status = $scope.savingEntity.entityParent.Status;
		    }

		    $notification[result.type ? result.type : 'error'](result.message);
		};

		/**
         * @function Set numa páginação todas as escolas estão marcadas
         * @name setAllSchoolsOnPagination
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param {array} lista
         * @return boolean
         */
		function setAllSchoolsOnPagination(lista) {

			var count = 0;

			for (var a = 0; a < lista.length; a++)
				if (lista[a].Selected)
					count += 1;

			if (count === lista.length)
				$scope.mechanics.AllAdheredOnPagination = true;
			else
				$scope.mechanics.AllAdheredOnPagination = false;
		};

		/**
         * @function Configura a flag que controla a lista interna de turmas para uma escola
         * @name setPaginationSchoolsOpen
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param
         * @return boolean
         */
		function setPaginationSchoolsOpen() {

			for (var a = 0; a < $scope.list.displayed.length; a++)
				$scope.list.displayed[a].Open = false;
		};

		/**
         * @function Envia dados da escola/turma/aluno selecionado para salvar a informação
         * @name selectEntity
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param 
         */
		function selectEntity(param) {
			$http.defaults.headers.common["Authorization"] = $scope.testInformation.Token;

			AdherenceApiModel.adherence(param).then(function (result) {
				proccessResult(result.data);
			})
		}

		/**
         * @function Envia dados da escola/turma/aluno selecionado para salvar a informação
         * @name selectEntity
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param 
         */
		function selectEntityList(param) {
			$http.defaults.headers.common["Authorization"] = $scope.testInformation.Token;

			AdherenceApiModel.adherenceList(param).then(function (result) {
				for (var ret in result.data) {
					proccessResult(result.data[ret]);
				}
			})
		}

		/**
         * @function Obter enum com os tipos de estados de seleção de uma escola.
         * @name getEnumSelectionType
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
		$scope.getEnumSelectionType = function __getEnumSelectionType(enumSelectionType) {
			$scope.enumSelectionType = enumSelectionType;
		};

		/**
         * @function Obter enum com os tipos das entidades escola/turmas.
         * @name getEnumEntityType
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
		$scope.getEnumEntityType = function __getEnumEntityType(enumEntityType) {
			$scope.enumEntityType = enumEntityType;
		};

		/**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.getAuthorize = function __getAuthorize(authorize) {

			$scope.params = $util.getUrlParams();
			$scope.blockPage = false;
			if (authorize === undefined || authorize === null) {
				$scope.blockPage = true;
				$notification.alert("Não foi possível obter as permissões de acesso a página.");
				redirectToList();
				return;
			}
			configTestInformation(authorize);
			if ($scope.params === undefined || $scope.params.test_id === undefined || !parseInt($scope.params.test_id)) {
				if (parseInt($scope.params.test_id) !== $scope.testInformation.TestId) {
					$scope.blockPage = true;
					$notification.alert("Id da prova inválido, ou usuário sem permissão.");
					redirectToList();
					return;
				}
			}
			configHtmlTitleModal();
			configMechanics($scope.testInformation.AllAdhered);
			configStatistics();
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
			    if ($scope.filters.Adherence) $scope.countFilter += 1;
			}, true);
		};

		/**
		 * @function Obter todas Escolas
		 * @name getSchools
		 * @namespace AdherenceController
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
		 * @namespace AdherenceController
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
        * @namespace AdherenceController
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
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
		$scope.clearFilters = function __clearFilters() {

			$scope.listFilter.Schools = [];
			$scope.listFilter.Years = [];
			$scope.listFilter.Turns = [];
			configFilters();
		};

		/**
		 * @function Limpeza dos filtros de pesquisa
		 * @name clearByFilter
		 * @namespace AdherenceController
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
		 * @function Confirmar a realização de pesquisa.
		 * @name clearByFilter
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {string} filter
		 * @return
		 */
		$scope.confirmSearchPagination = function __confirmSearchPagination() {

			angular.element('#searchModal').modal();
		};

		/**
		 * @function Set Paginação
		 * @name setPagination
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.setPagination = function __setPagination() {

		    if ($scope.paginate)
		        $scope.pageSize = $scope.paginate.getPageSize();

			if ($scope.testInformation.TestOwner === true && $scope.filters.Adherence === false) {
			    $scope.paginate = $pager(AdherenceModel.getSchoolsGrid);
			}
			else {
				$scope.paginate = $pager(AdherenceModel.getSelectedSchool);
			}
			$scope.paginate.pageSize($scope.pageSize);
			$scope.paginate.indexPage(0);
			$scope.pesquisa = '';
			$scope.message = false;
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.search();
		};

		/**
		 * @function Limpar Paginação
		 * @name clearPagination
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.clearPagination = function __clearPagination() {
		    
			$scope.paginate.indexPage(0);
			$scope.pesquisa = '';
			$scope.message = false;
			$scope.pages = 0;
			$scope.totalItens = 0;
		};

		/**
		 * @function Pesquisa.
		 * @name search
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.search = function __search() {

			var params = {
				'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
				'dre_id': ($scope.filters.DRE !== undefined && $scope.filters.DRE !== null) ? $scope.filters.DRE.Id : "",
				'crp_ordem': ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0,
				'esc_id': ($scope.filters.School !== undefined && $scope.filters.School !== null) ? $scope.filters.School.Id : 0,
				'ttn_id': ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0
			};

			$scope.paginate.paginate(params).then(function (result) {

				if (result.success) {

					if (result.lista.length > 0) {

						$scope.paginate.nextPage();

						$scope.list.displayed = angular.copy(result.lista);

						setAllSchoolsOnPagination(result.lista);

						setPaginationSchoolsOpen();

						if (!$scope.pages > 0) {
							$scope.pages = $scope.paginate.totalPages();
							$scope.totalItens = $scope.paginate.totalItens();
						}
					} else {
					    $scope.list.displayed = [];
					}
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}

			}, function () {
				$scope.message = true;
				$scope.list.displayed = null;
			});
		};

		/**
		 * @function Carregar turmas de uma escola
		 * @name loadTeamsBySchool
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {object} school
         * @param {int} $index
         * @param {boolean} status
		 * @return
		 */
		$scope.loadTeamsBySchool = function __loadTeamsBySchool(school, $index, status) {

			if (school === undefined || school === null || status === false)
				return;

			var params = {
				'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
				'ttn_id': ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0,
				'esc_id': school.Id,
				'crp_ordem': ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0
			};

			if ($scope.testInformation.TestOwner) {
			    getTeams(params, $index);
			}
			else {
			    getSelectedTeams(params, $index);
			}
		};

	    /**
		 * @function Carregar alunos das turmas de uma escola
		 * @name loadStudentsByTeam
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {object} school
         * @param {int} $index
         * @param {boolean} status
		 * @return
		 */
		$scope.loadStudentsByTeam = function __loadStudentsByTeam(team, $indexSchool, $indexTeam, status) {

		    if (team === undefined || team === null || status === false)
		        return;

		    var params = {
		        'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'team_id': team.Id
		    };

		    if ($scope.testInformation.TestOwner) {
		        getStudents(params, $indexSchool, $indexTeam);
		    }
		    else {
		        getSelectedStudents(params, $indexSchool, $indexTeam);
		    }
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
		 * @name cancel
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.cancel = function __cancel() {
			$window.location.href = '/Test';
		};

		/**
		 * @function Select a school.
		 * @name selectSchool
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {object} school
		 * @return
		 */
		$scope.selectSchool = function __selectSchool(_school) {
			if (_school === undefined || _school === null)
				return;

			//school
			_school.EntityType = parseInt(getEnumEntityTypeStatus('Escola'));
			var ttn_id = ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0;
			var year = ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0;

			//find teams and set change status
			if (_school.Teams !== undefined && _school.Teams !== null) {
				for (var i = 0; i < _school.Teams.length; i++) {
					_school.Teams[i].Selected = _school.Selected;
					_school.Teams[i].Status = _school.Status;
					_school.Teams[i].EntityType = parseInt(getEnumEntityTypeStatus('Turma'));

				    //find students and set change status
					if (_school.Teams[i].Students !== undefined && _school.Teams[i].Students != null) {
					    for (var j = 0; j < _school.Teams[i].Students.length; j++) {
					        _school.Teams[i].Students[j].Selected = _school.Selected;
					        _school.Teams[i].Students[j].Status = _school.Status;
					        _school.Teams[i].Students[j].EntityType = parseInt(getEnumEntityTypeStatus('Aluno'));
					    }
					}
				}
			}

			//keep backup copy
			$scope.savingEntity = {
				entityType: _school.EntityType,
				entityParent: undefined,
				entity: angular.copy(_school)
			};

			//change school status
			_school.Status = parseInt(schoolStateTeams(_school.Teams, _school.Selected));


			var param = {
				idEntity: parseInt(_school.Id),
				typeEntity: parseInt(_school.EntityType),
				typeSelection: parseInt(_school.Status),
				ttn_id: ttn_id,
				year: year
			}

			selectEntity(param);

		};

		/**
		 * @function Select a team.
		 * @name selectTeam
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {int} $indexSchool
		 * @param {int} $indexTeam
         * @param {boolean} select
		 * @return
		 */
		$scope.selectTeam = function __selectTeam($indexSchool, $indexTeam, select) {
			var _school = $scope.list.displayed[$indexSchool];
			var team = $scope.list.displayed[$indexSchool].Teams[$indexTeam];

			//define type
			team.EntityType = getEnumEntityTypeStatus('Turma');

			//keep backup copy
			$scope.savingEntity = {
				entityType: team.EntityType,
				entityParent: angular.copy(_school),
				entity: angular.copy(team)
			};

			//team changes
			team.Selected = select;
			team.Status = parseInt((select === true) ? getEnumSelectionTypeStatus('Selecionado') : getEnumSelectionTypeStatus('Não Selecionado'));
			team.Open = false;

			//school changes
			_school.Status = parseInt(schoolStateTeams(_school.Teams, select));
			_school.Selected = (_school.Status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || _school.Status === parseInt(getEnumSelectionTypeStatus('Parcial'))) ? true : false;

			var ttn_id = ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0;
			var year = ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0;

			var param = {
				idEntity: parseInt(team.Id),
				typeEntity: parseInt(team.EntityType),
				typeSelection: parseInt(team.Status),
				ttn_id: ttn_id,
				year: year
			}

			selectEntity(param);
		};

	    /**
		 * @function Select a student.
		 * @name selectStudent
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {int} $indexSchool
		 * @param {int} $indexTeam
         * @param {int} $indexStudent
         * @param {boolean} select
		 * @return
		 */
		$scope.selectStudent = function __selectStudent($indexSchool, $indexTeam, $indexStudent, select) {

		    var _school = $scope.list.displayed[$indexSchool];
		    var _team = $scope.list.displayed[$indexSchool].Teams[$indexTeam];
		    var student = $scope.list.displayed[$indexSchool].Teams[$indexTeam].Students[$indexStudent];

		    //define type
		    student.EntityType = getEnumEntityTypeStatus('Aluno');

		    //keep backup copy
		    $scope.savingEntity = {
		        entityType: student.EntityType,
		        entityParent: angular.copy(_team),
		        entity: angular.copy(student),
		        blocked: false
		    };

		    //student changes
		    student.Selected = select;
		    student.Status = parseInt((select === true) ? getEnumSelectionTypeStatus('Selecionado') : getEnumSelectionTypeStatus('Não Selecionado'));

		    //team changes
		    _team.Status = parseInt(teamStateStudents(_team.Students, select));
		    _team.Selected = (_team.Status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || _team.Status === parseInt(getEnumSelectionTypeStatus('Parcial'))) ? true : false;

		    //school changes
		    _school.Status = parseInt(schoolStateTeams(_school.Teams, select));
		    _school.Selected = (_school.Status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || _school.Status === parseInt(getEnumSelectionTypeStatus('Parcial'))) ? true : false;

		    var ttn_id = ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0;
		    var year = ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0;

		    var param = {
		        idEntity: parseInt(student.Id),
		        typeEntity: parseInt(student.EntityType),
		        typeSelection: parseInt(student.Status),
		        ttn_id: ttn_id,
		        year: year,
		        parentId: _team.Id
		    }

		    selectEntity(param);
		};

	    /**
		 * @function Select a student.
		 * @name selectStudent
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {int} $indexSchool
		 * @param {int} $indexTeam
         * @param {int} $indexStudent
         * @param {boolean} select
		 * @return
		 */
		$scope.blockedStudent = function __blockedStudent($indexSchool, $indexTeam, $indexStudent) {

		    var _school = $scope.list.displayed[$indexSchool];
		    var _team = $scope.list.displayed[$indexSchool].Teams[$indexTeam];
		    var student = $scope.list.displayed[$indexSchool].Teams[$indexTeam].Students[$indexStudent];

		    var select = student.Status === parseInt(getEnumSelectionTypeStatus('Bloqueado'));

		    //define type
		    student.EntityType = getEnumEntityTypeStatus('Aluno');

		    //keep backup copy
		    $scope.savingEntity = {
		        entityType: student.EntityType,
		        entityParent: angular.copy(_team),
		        entity: angular.copy(student),
		        blocked: true,
		        selectedOld: student.Selected,
		        statusOld: student.Status,
		        indexSchool: $indexSchool,
		        indexTeam: $indexTeam,
		        indexStudent: $indexStudent,
		    };

		    //student changes
		    student.Selected = false;
		    student.Status = parseInt(select ? getEnumSelectionTypeStatus('Não Selecionado') : getEnumSelectionTypeStatus('Bloqueado'));

		    //team changes
		    _team.Status = parseInt(teamStateStudents(_team.Students, select));
		    _team.Selected = (_team.Status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || _team.Status === parseInt(getEnumSelectionTypeStatus('Parcial'))) ? true : false;

		    //school changes
		    _school.Status = parseInt(schoolStateTeams(_school.Teams, select));
		    _school.Selected = (_school.Status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || _school.Status === parseInt(getEnumSelectionTypeStatus('Parcial'))) ? true : false;

		    var ttn_id = ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0;
		    var year = ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0;

		    var param = {
		        idEntity: parseInt(student.Id),
		        typeEntity: parseInt(student.EntityType),
		        typeSelection: parseInt(student.Status),
		        ttn_id: ttn_id,
		        year: year,
		        parentId: _team.Id
		    }

		    selectEntity(param);
		};

		/**
		 * @function Carregar dados estatísticos 
		 * @name loadStatistics
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.loadStatistics = function __loadStatistics() {

			if ($scope.statistics.Open === false) {

				var params = { 'test_id': $scope.testInformation.TestId };

				AdherenceModel.getTotalSelected(params, function (result) {

					if (result.success) {

						$scope.statistics.SelectedTeams = result.lista.totalSelectedSection;
						$scope.statistics.SelectedSchools = result.lista.totalSelectedSchool;
						$scope.statistics.TotalSchools = result.lista.totalSchool;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}

			$scope.statistics.Open = !$scope.statistics.Open;
		};

		/**
         * @function Selecionar/'Deselecionar' todas as escolas na paginação
         * @name selectAllSchoolOnPagination
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param {select}
         * @return
         */
		$scope.selectAllSchoolOnPagination = function __selectAllSchoolOnPagination(select) {

			//keep backup copy
			$scope.savingEntity = {
				select: !select,
				batch: true,
				entityType: parseInt(getEnumEntityTypeStatus('Escola')),
				entityParent: undefined,
				entity: []
			};

			//collection sender
			var collection = [];

			var schoolIds = [];
			var ttn_id = ($scope.filters.Turn !== undefined && $scope.filters.Turn !== null) ? $scope.filters.Turn.Id : 0;
			var year = ($scope.filters.Year !== undefined && $scope.filters.Year !== null) ? $scope.filters.Year.Id : 0;

			for (var a = 0; a < $scope.list.displayed.length; a++) {

				$scope.list.displayed[a].Open = true;
				$scope.list.displayed[a].Selected = select;
				$scope.list.displayed[a].EntityType = parseInt(getEnumEntityTypeStatus('Escola'));

				//find teams (turmas)
				if ($scope.list.displayed[a].Teams !== undefined && $scope.list.displayed[a].Teams !== null) {
					for (var b = 0; b < $scope.list.displayed[a].Teams.length; b++) {
						$scope.list.displayed[a].Teams[b].Selected = select;
						$scope.list.displayed[a].Teams[b].Status = $scope.list.displayed[a].Status;
						$scope.list.displayed[a].Teams[b].EntityType = parseInt(getEnumEntityTypeStatus('Turma'));

					    //find students and set change status
						if ($scope.list.displayed[a].Teams[b].Students !== undefined && $scope.list.displayed[a].Teams[b].Students != null) {
						    for (var j = 0; j < $scope.list.displayed[a].Teams[b].Students.length; j++) {
						        $scope.list.displayed[a].Teams[b].Students[j].Selected = select;
						        $scope.list.displayed[a].Teams[b].Students[j].Status = $scope.list.displayed[a].Status;
						        $scope.list.displayed[a].Teams[b].Students[j].EntityType = parseInt(getEnumEntityTypeStatus('Aluno'));
						    }
						}

					}
				}

				//keep backup copy
				$scope.savingEntity.entity.push(angular.copy($scope.list.displayed[a]));

				//change school state
				$scope.list.displayed[a].Status = parseInt(schoolStateTeams($scope.list.displayed[a].Teams, $scope.list.displayed[a].Selected));

				schoolIds.push(parseInt($scope.list.displayed[a].Id));
			}

			var param = {
			    entityList: schoolIds,
			    typeSelection: (select === true) ? getEnumSelectionTypeStatus('Selecionado') : getEnumSelectionTypeStatus('Não Selecionado'),
			    ttn_id: ttn_id,
			    year: year
			};
			
			selectEntityList(param);
		};

		/**
		 * @function Confirmar marcar/desmarcar
		 * @name validateAllAdherence
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.validateAllAdherence = function __validateAllAdherence() {

			var forecast = !$scope.mechanics.AllAdhered;

			if (forecast !== $scope.testInformation.AllAdhered) {

				if (forecast === true)
					$scope.selectAllAdherenceModalTitle = "<p><h4><strong>Seleção de todas as escolas da rede</strong></h4></p><p>Ao confirmar todas as escolas e turmas serão marcadas como aderidas, deseja realmente realizar esta operação?</p>";
				else
					$scope.selectAllAdherenceModalTitle = "<p><h4 ><strong>Remover seleção de todas as escolas da rede</strong></h4></p><p>Ao confirmar todas as escolas e turmas serão removidas da adesão, deseja realmente realizar esta operação?</p>";

				angular.element('#selectAllAdherenceModal').modal({ backdrop: 'static', keyboard: false });
			}
			else {
				$scope.allAdherenceControll();
			}
		};

		/**
         * @function Marcar/Desmarcar todas as provas como aderidas
         * @name allAdherenceControll
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
		$scope.allAdherenceControll = function __allAdherenceControll() {

			angular.element('#selectAllAdherenceModal').modal('hide');

			var params = {
				test_id: $scope.testInformation.TestId,
				allAdhered: !$scope.mechanics.AllAdhered
			};

			AdherenceModel.switchAllAdhrered(params, function (result) {

				if (result.success) {
					$scope.mechanics.AllAdhered = !$scope.mechanics.AllAdhered;
					$scope.testInformation.AllAdhered = $scope.mechanics.AllAdhered;
					$scope.statistics.Open = false;
					configLists();
					$scope.clearPagination();
					$notification.success(result.message);
					$scope.search();
				}
				else {
					//console.log('error salvar todas escolas e turmas: ', result);
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
         * @function Seleção Parcial
         * @name isPartialSelection
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param {int} status
         * @return
         */
		$scope.isPartialSelection = function __isPartialSelection(status) {

		    if (status === parseInt(getEnumSelectionTypeStatus('Não Selecionado')) || status === parseInt(getEnumSelectionTypeStatus('Selecionado')) || status === parseInt(getEnumSelectionTypeStatus('Bloqueado')))
				return false;

			return true;
		};

	    /**
         * @function Seleção Parcial
         * @name isPartialSelection
         * @namespace AdherenceController
         * @memberOf Controller
         * @public
         * @param {int} status
         * @return
         */
		$scope.isBlocked = function __isBlocked(status) {

		    if (status === parseInt(getEnumSelectionTypeStatus('Bloqueado')))
		        return true;

		    return false;
		};

		/**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace AdherenceController
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