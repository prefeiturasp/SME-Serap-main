/**
 * TestListController Controller
 * @author Alexandre Garcia Simões - Mstech: 29/05/2015
 * @author Julio Cesar Silva - Mstech: 07/11/2016
 */
(function (angular, $) {

	'use strict';

	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	angular
		.module('appMain')
		.controller("TestListController", TestListController)
	    .directive('draggable', function () {
	        return function (scope, element) {
	            var el = element[0];
	            el.draggable = true;

	            el.addEventListener(
                  'dragstart',
                  function (e) {
                      e.dataTransfer.effectAllowed = 'move';
                      e.dataTransfer.setData('Text', this.id);
                      this.classList.add('drag');
                      return false;
                  },
                  false
                );

	            el.addEventListener(
                  'dragend',
                  function (e) {
                      this.classList.remove('drag');
                      return false;
                  },
                  false
                );
	        }
	    })
	    .directive('droppable', function () {
	        return {
	            scope: {
	                drop: '&',
	                target: '='
	            },
	            link: function (scope, element) {
	                var el = element[0];

	                el.addEventListener(
                      'dragover',
                      function (e) {
                          e.dataTransfer.dropEffect = 'move';
                          if (e.preventDefault) e.preventDefault();
                          this.classList.add('over');
                          return false;
                      },
                      false
                    );

	                el.addEventListener(
                      'dragenter',
                      function (e) {
                          this.classList.add('over');
                          return false;
                      },
                      false
                    );

	                el.addEventListener(
                      'dragleave',
                      function (e) {
                          this.classList.remove('over');
                          return false;
                      },
                      false
                    );

	                el.addEventListener(
                      'drop',
                      function (e) {
                          if (e.preventDefault) { e.preventDefault(); }
                          if (e.stopPropagation) e.stopPropagation();

                          this.classList.remove('over');

                          var targetId = this.id;
                          var item = document.getElementById(e.dataTransfer.getData('Text'));
                          scope.$apply(function (scope) {
                              var fn = scope.drop();
                              if ('undefined' !== typeof fn) {
                                  fn(item.id, targetId);
                              }
                          });

                          return false;
                      },
                      false
                    );
	            }
	        }
	    });

	TestListController.$inject = ['$scope', '$notification', '$timeout', '$pager', 'TestListModel', 'FileModel', 'AdherenceModel', 'TestTypeModel'];

	/**
	 * @function Test List Controller 
	 * @param {Object} $scope
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @param {Object} $pager
	 * @param {Object} TestListModel
	 * @param {Object} FileModel
	 * @param {Object} AdherenceModel
	 * @param {Object} TestTypeModel
	 * @returns
	 */
	function TestListController(ng, $notification, $timeout, $pager, TestListModel, FileModel, AdherenceModel, TestTypeModel) {
		ng.countFilter = 0;
		ng.EnumFrequencyApplication = {
			Yearly:1,
			Semiannual :2,
			Bimonthly:3,
			Monthly:4,
		};

		ng.profile = getCurrentVision();
		ng.visAdmin = EnumVisions.ADMINISTRATOR;
		ng.visibleTest = false;
		ng.visibleMultidiscipline = false;

		ng.totalResults;
	    var searchFilter = {}, //filtro da busca
            searchFilterFromCache = {},
			searchResult = {}; //resultado da busca
		ng.labels = null; //labels da view        
		ng.codItem = null; //ng-model[input]:                     Código do item
		ng.selectedTestType = null; //ng-model[select]:                     tipo de prova
		ng.selectedDiscipline = null; //ng-model[select]:                        disciplina         
		ng.dateStart = null; //ng-model[date]:                      data de inicio   
		ng.dateEnd = null; //ng-model[date]:                     data de termino   
		ng.disciplineEnable = true; //lock[selectDiscipline]:  flag Ativa/Desativa select        
		ng.selectedTest = null; //prova selecionada na lista de retorno da busca   
		ng.global = true;
		ng.frequencyApplicationList = [];
		ng.frequencyApplication = null;
		ng.labelTermGlobal = getParameterValue(parameterKeys[0].GLOBAL_TERM);
		ng.labelTermLocal = getParameterValue(parameterKeys[0].LOCAL_TERM);
		ng.pages = 0;
		ng.totalItens = 0;
		ng.pageSize = 10;
		ng.paginate = $pager(TestListModel.searchTests);
		ng.listExpandido = [];
		ng.listExpandidoAcao = [];
		ng.getGroup = true;


		/**
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @name configListFilter
		 * @namespace TestListController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function configListFilter() {
			ng.listFilter = {
				DREs: [],
				Schools: [],
				Years: [],
				Teams: []
			};
		};

		/**
		 * @function Configuração dos filtros
		 * @param
		 * @return
		 */
		function configFilters() {
			ng.filters = {
				DRE: undefined,
				School: undefined,
				Year: undefined,
				Turn: undefined
			};
		};

		/**
		 * @function Obter todas DREs
		 * @name getDREs
		 * @namespace TestListController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
		function getDREs() {
			AdherenceModel.getDRESimple({}, function (result) {

				if (result.success) {
					ng.listFilter.DREs = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function carregar frequências de aplicação
		 * @param {object} _callback
		 * @returns
		 */
		ng.loadFrequencyApplication = function __loadFrequencyApplication(_callback) {
			ng.frequencyApplication = null;
			if (ng.selectedTestType == null || ng.selectedTestType == undefined || ng.selectedTestType.FrequencyApplication == undefined) return;
			if (ng.selectedTestType.FrequencyApplication == ng.EnumFrequencyApplication.Yearly) {
				ng.frequencyApplication = ng.selectedTestType.FrequencyApplication;
				return;
			}
			TestTypeModel.getFrequencyApplicationChildList({ parentId: ng.selectedTestType.FrequencyApplication }, function (result) {
				if (result.success) {
					ng.frequencyApplicationList = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
				if (_callback) _callback();
			});
		};

		/**
		 * @function Configura variaveis iniciais
		 * @name getAuthorize
		 * @namespace TestController
		 * @memberOf Controller
		 * @public
		 * @param {object} dados
		 * @return
		 */
		ng.getAuthorize = function __getAuthorize(dados, groupFilter) {
		    if (!(dados === null) && !(dados === undefined) && !(dados.global === undefined)) {
		        searchFilter = { global: dados.global };
			}

			if (groupFilter === null || groupFilter === undefined || groupFilter.TestGroupId === undefined || groupFilter.TestGroupId === null) {
				ng.getSearchResult();
				return;
			}

			ng.searchByGroup(groupFilter);
		};

		/**
		 * @function Obter todas Escolas
		 * @name getSchools
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.getSchools = function __getSchools() {

			if (ng.filters.DRE === undefined || ng.filters.DRE === null)
				return;

			var params = { 'dre_id': ng.filters.DRE.Id };

			AdherenceModel.getSchoolsSimple(params, function (result) {

				if (result.success) {
					ng.listFilter.Schools = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Obter todos os anos
		 * @name getYears
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.getYears = function __getYears() {


			if (ng.filters.School === undefined || ng.filters.School === null)
				return;

			var params = { 'esc_id': ng.filters.School.Id };

			TestListModel.getCurriculumGradeSimple(params, function (result) {

				if (result.success) {
					ng.listFilter.Years = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Obter todos Turnos
		 * @name getTurns
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.getTurns = function __getTurns() {

			if (ng.filters.School === undefined || ng.filters.School === null)
				return;

			var params = { 'esc_id': ng.filters.School.Id };

			AdherenceModel.getShiftSimple(params, function (result) {

				if (result.success) {
					ng.listFilter.Turns = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Limpeza dos filtros de pesquisa.
		 * @name clearFilters
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.clearFilters = function __clearFilters() {

			ng.listFilter.Schools = [];
			ng.listFilter.Years = [];
			ng.listFilter.Turns = [];
			configFilters();
		};

		/**
		 * @function Limpeza dos filtros de pesquisa
		 * @name clearByFilter
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param {string} filter
		 * @return
		 */
		ng.clearByFilter = function __clearByFilter(filter) {

			if (filter === 'DRE') {
				ng.listFilter.Schools = [];
				ng.listFilter.Years = [];
				ng.listFilter.Turns = [];
				ng.filters.School = undefined;
				ng.filters.Year = undefined;
				ng.filters.Turn = undefined;
				return;
			}

			if (filter === 'School') {
				ng.listFilter.Years = [];
				ng.listFilter.Turns = [];
				ng.filters.Year = undefined;
				ng.filters.Turn = undefined;
				return;
			}

			if (filter === 'Group') {
			    ng.listFilter.SubGroups = [];
			    ng.selectedSubGroup = undefined;
			    return;
			}
		};

		/**
		 * @function Mudanças global Local
		 * @name clearGlobalFilters
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.clearGlobalFilters = function __clearGlobalFilters() {

			if (ng.global === true) {
				ng.clearFilters();
			}
		};

		/**
		 * @function Inicialização do controller
		 * @name clearByFilter
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		function init() {

			ng.labels = {
				show: false,
				header: "Consulta de provas",
				codItem: "Código da prova:",
				testType: 'Tipo de prova:',
				testTypeList: [],
				discipline: 'Componente curricular:',
				disciplineList: [],
				frequencyApplication: 'Frequência de aplicação',
				criacao: "Criação:",
				situacao: "Situação:",
				situacaoList: [
					{ Id: 2, Description: "Cadastrada", Style: "icone-cadastrar material-icons situacao", Checked: false, Icon: 'radio_button_unchecked' },
					{ Id: 4, Description: "Aplicada", Style: "icone-aplicar material-icons situacao", Checked: false, Icon: 'check_circle' },
					{ Id: 3, Description: "Em andamento", Style: "icone-andamento material-icons situacao", Checked: false, Icon: 'timelapse' },
					{ Id: 1, Description: "Pendente", Style: "icone-pendente material-icons situacao", Checked: false, Icon: 'remove_circle_outline' }
				],
				headerBoxTable: "Provas"
			};
			getTestTypeList();
			getGroups();
			configListFilter();
			configFilters();
			getDREs();

			ng.tipoVisualizacao = "1";
			ng.ordenacao = "3";

			ng.$watchCollection('[selectedTestType, selectedDiscipline, selectedBimester, frequencyApplication, dateStart, dateEnd,'
				+ ' labels.situacaoList[0].Checked, labels.situacaoList[1].Checked, labels.situacaoList[2].Checked, labels.situacaoList[3].Checked,'
				+ ' filters.DRE, filters.School, filters.Year, filters.Turn, visibleTest, visibleMultidiscipline]', function (res) {

				ng.countFilter = 0;
				if (ng.selectedTestType) ng.countFilter += 1;
				if (ng.selectedGroup) ng.countFilter += 1;
				if (ng.selectedSubGroup) ng.countFilter += 1;
				if (ng.selectedDiscipline) ng.countFilter += 1;
				if (ng.selectedBimester) ng.countFilter += 1;
				if (ng.frequencyApplication) ng.countFilter += 1;
				if (ng.dateStart) ng.countFilter += 1;
				if (ng.dateEnd) ng.countFilter += 1;
				var situationCount = 0;
				for (var a = 0; a < ng.labels.situacaoList.length; a++) 
					if (ng.labels.situacaoList[a].Checked)
						situationCount += 1;
				ng.countFilter += situationCount;
				if (ng.visibleTest) ng.countFilter += 1;
				if (ng.visibleMultidiscipline) ng.countFilter += 1;
				if (ng.global) return;
				if (ng.filters.DRE) ng.countFilter += 1;
				if (ng.filters.School) ng.countFilter += 1;
				if (ng.filters.Year) ng.countFilter += 1;
				if (ng.filters.Turn) ng.countFilter += 1;

				}, true);

			checkSessionstorage();
		};

		function checkSessionstorage() {

		    var leavingTest = JSON.parse(sessionStorage.getItem('leavingTest'));
		    var arrivingTest = JSON.parse(sessionStorage.getItem('arrivingTest'));
		    var searchFilterTest = sessionStorage.getItem('searchFilterTest')

            //DEBUG
		    //if (searchFilterLocalStogare) {
		    //    searchFilter = JSON.parse(searchFilterLocalStogare)
		    //    loadFilterFromCache();
		    //    return;
		    //}

		    if (leavingTest && arrivingTest && searchFilterTest) {
				searchFilter = JSON.parse(searchFilterTest)
				ng.getGroup = searchFilter.getGroup;
		        loadFilterFromCache();
		    } else {
		        sessionStorage.removeItem('searchFilterTest')
		    }
		    
		    sessionStorage.removeItem('arrivingTest')
		    sessionStorage.removeItem('leavingTest')
		}

		function loadFilterFromCache() {

		    searchFilter.TestTypeCached ? ng.selectedTestType = searchFilter.TestTypeCached : null;
		    if (searchFilter.TestTypeCached) {
		        ng.getDisciplines();
		        ng.loadFrequencyApplication(function () {
		            if (typeof searchFilter.frequencyApplication === 'object') {
		                ng.frequencyApplication = searchFilter.frequencyApplication;
		            } else {
		                ng.frequencyApplication = { Id: searchFilter.frequencyApplication }
                    }
		            
		            searchFilter.DisciplineIdCached ? ng.selectedDiscipline = searchFilter.DisciplineIdCached : null;		            
		        });
		    }

		    searchFilter.TestGroupIdCached ? ng.selectedGroup = searchFilter.TestGroupIdCached : null;
		    if (searchFilter.TestGroupIdCached) {
		        ng.getSubGroup();

		        searchFilter.TestSubGroupIdCached ? ng.selectedSubGroup = searchFilter.TestSubGroupIdCached : null;
		    }

		    searchFilter.CreationDateStart ? ng.dateStart = searchFilter.CreationDateStart : null;
		    searchFilter.CreationDateEnd ? ng.dateEnd = searchFilter.CreationDateEnd: null;
		    searchFilter.Cadastrada ? ng.labels.situacaoList[0].Checked = true : false;
		    searchFilter.Aplicada ? ng.labels.situacaoList[1].Checked = true : false;
		    searchFilter.Andamento ? ng.labels.situacaoList[2].Checked = true : false;
		    searchFilter.Pendente ? ng.labels.situacaoList[3].Checked = true : false;
		    ng.visibleTest = true ? searchFilter.hasOwnProperty("visibleTest"): false;
		    ng.visibleMultidiscipline = searchFilter.visibleMultidiscipline;

		    if (!ng.global) {            
		        ng.filters.DRE = typeof searchFilter.dre_id === 'object' ? searchFilter.dre_id : { Id: searchFilter.dre_id }
		        if (ng.filters.DRE) {
		            ng.getSchools();
		            ng.filters.School = typeof searchFilter.esc_id === 'object' ? searchFilter.esc_id : { Id: searchFilter.esc_id }
		            ng.getYears();
		            ng.getTurns();
		            ng.filters.Year = typeof searchFilter.tne_id_ordem === 'object' ? searchFilter.tne_id_ordem : { Id: searchFilter.tne_id_ordem }
		            ng.filters.Turn = typeof searchFilter.ttn_id === 'object' ? searchFilter.ttn_id : { Id: searchFilter.ttn_id }
		        }
		    }

		}


		/**
		* @name getTestTypeList
		* @namespace TestListController
		* @desc GET: Solicitação da lista de tipos de prova
		* @memberOf Controller.TestListController
		*/
		function getTestTypeList() {

			TestListModel.loadByUserGroupSearchTest(function (result) {

				if (result.success) {
					ng.labels.show = true;
					ng.labels.testTypeList = result.lista.testTypeList;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @name getDisciplines
		 * @namespace TestListController
		 * @desc GET: Solicitação da lista de disciplinas relacionadas ao tipo de prova selecionado
		 * @memberOf Controller.TestListController
		 */
		ng.getDisciplines = function () {
			ng.labels.disciplineList = []; //limpa lista de disciplinas
			ng.selectedDiscipline = null; //limpa model de disciplina selecionada
			ng.disciplineEnable = true; //desativa combobox de disciplina
			if (!ng.selectedTestType) return;
			TestListModel.searchDisciplinesSaves({ typeLevelEducation: ng.selectedTestType.TypeLevelEducationId }, function (result) {
				if (result.success) {
					ng.disciplineEnable = false; //ativa combobox de disciplina
					ng.labels.disciplineList = result.lista; //alimenta lista de disciplina
				} else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Validação dos dados para filtrar e realizar busca: válidos / inválidos
		 */
		ng.limpar = function () {
			ng.labels.disciplineList = [];
			ng.frequencyApplicationList = [];
			ng.selectedTestType = null;
			ng.selectedDiscipline = null;
			ng.frequencyApplication = null;
			ng.dateStart = "";
			ng.dateEnd = "";
			for (var i = 0; i < ng.labels.situacaoList.length; i++) 
				ng.labels.situacaoList[i].Checked = false;
			ng.clearFilters();
			ng.countFilter = 0;
		};

		/**
		* @name searchValidation
		* @namespace TestListController
		* @desc Validação dos dados para filtrar e realizar busca: válidos / inválidos
		* @memberOf Controller.TestListController
		*/
		ng.searchFilterValidation = function (searchCode) {
			if (ng.dateStart && ng.dateEnd && new Date(ng.dateStart) > new Date(ng.dateEnd)) {
				$notification.alert("A data de término não pode ser anterior a data de início.");
			}
			else {
				if (isNaN(ng.codItem) && ng.codItem != null) {
					$notification.alert("Código de item inválido.");
				} else {
				    searchFilterFormat(searchCode);
				}
			}
		};

		/**
		* @name searchFilterFormat
		* @namespace TestListController
		* @desc Formatação dos dados para filtrar e realizar busca
		* @memberOf Controller.TestListController
		*/
		function searchFilterFormat(searchCode) {

		    searchFilter = {};

		    ng.selectedTestType ? searchFilter.TestType = parseInt(ng.selectedTestType.Id) : null;
		    ng.selectedTestType ? searchFilter.TestTypeCached = ng.selectedTestType : null;

		    if (searchCode)
		    {
		        ng.getGroup = false;
		    }
		    else {
		        ng.getGroup = true;
		    }

		    ng.selectedGroup ? searchFilter.TestGroupId = parseInt(ng.selectedGroup.Id) : null;
		    ng.selectedGroup ? searchFilter.TestGroupIdCached = ng.selectedGroup : null;

		    ng.selectedSubGroup ? searchFilter.TestSubGroupId = parseInt(ng.selectedSubGroup.Id) : null;
		    ng.selectedSubGroup ? searchFilter.TestSubGroupIdCached = ng.selectedSubGroup : null;

		    ng.selectedDiscipline ? searchFilter.DisciplineId = parseInt(ng.selectedDiscipline.Id) : null;
		    ng.selectedDiscipline ? searchFilter.DisciplineIdCached = ng.selectedDiscipline: null;
			ng.dateStart ? searchFilter.CreationDateStart = ng.dateStart : null;
			ng.dateEnd ? searchFilter.CreationDateEnd = ng.dateEnd : null;
			ng.labels.situacaoList[0].Checked ? searchFilter.Cadastrada = true : null;
			ng.labels.situacaoList[1].Checked ? searchFilter.Aplicada = true : null;
			ng.labels.situacaoList[2].Checked ? searchFilter.Andamento = true : null;
			ng.labels.situacaoList[3].Checked ? searchFilter.Pendente = true : null;
			searchFilter.frequencyApplication = ng.frequencyApplication;
			searchFilter.dre_id = (ng.filters.DRE !== undefined && ng.filters.DRE !== null) ? ng.filters.DRE.Id : "";
			searchFilter.tne_id_ordem = (ng.filters.Year !== undefined && ng.filters.Year !== null) ? ng.filters.Year.Id : "";
			searchFilter.esc_id = (ng.filters.School !== undefined && ng.filters.School !== null) ? ng.filters.School.Id : 0;
			searchFilter.ttn_id = (ng.filters.Turn !== undefined && ng.filters.Turn !== null) ? ng.filters.Turn.Id : 0;
			searchFilter.global = ng.global;
			searchFilter.visibleMultidiscipline = ng.visibleMultidiscipline ? true : undefined;

			if (ng.profile == EnumVisions.ADMINISTRATOR)
				searchFilter.visibleTest = ng.visibleTest ? !ng.visibleTest : undefined;
			else 
				searchFilter.visibleTest = undefined;
			ng.paginate.indexPage(0);
			ng.pageSize = ng.paginate.getPageSize();
			ng.getSearchResult(searchCode);
		};

		ng.searchByGroup = function __searchByGroup(group) {
		    ng.getGroup = false;

		    searchFilter.TestGroupId = parseInt(group.TestGroupId);
		    searchFilter.TestSubGroupId = parseInt(group.TestSubGroupId);

			ng.getSearchResult();
		}

		/**
		* @name getSearchResult
		* @namespace TestListController
		* @desc GET: Realiza busca de provas de acordo com o filtro definido pelo usuário
		* @memberOf Controller.TestListController
		*/
		ng.getSearchResult = function (searchCode) {
		    searchFilter.getGroup = ng.getGroup;
		    searchFilter.ordenacao = ng.ordenacao;
			ng.paginate.paginate(searchCode ? { TestId: parseInt(ng.codItem) } : searchFilter)
			.then(function (result) {
			    if (searchFilter.getGroup) {
			        if (result.lista.length > 0) {
			            ng.searchResultGroup = result.lista;
			            ng.totalResults = result.lista.length;
			        }
			        else {
			            ng.searchResultGroup = null;
			        }
			    }
			    else {
			        if (result.lista.length > 0) {
			            ng.searchResult = result.lista;
			            ng.totalResults = result.lista.length;
			        }
			        else {
			            ng.searchResult = null;
			        }
			    }
				ng.pages = ng.paginate.totalPages();
				ng.totalItens = ng.paginate.totalItens();
				if (!searchCode && searchCode == undefined) {
					ng.codItem = "";
				}

				sessionStorage.setItem('searchFilterTest', JSON.stringify(searchFilter));
			},
			function (result) {
				ng.searchResult = null;
			});
		};

		/**
		* @name changeTest
		* @namespace TestListController
		* @desc Armazena os dados da prova selecionada pelo usuário na lista de busca 
		* @param {Object} - Dados da prova selecionada
		* @memberOf Controller.TestListController
		*/
		ng.changeTest = function __changeTest(selectedTest) {
			$timeout(function () {
				ng.selectedTest = selectedTest;
				ng.popovermenu.selectedTest = selectedTest;
				ng.popovermenu.profile = ng.profile;
				ng.popovermenu.visAdmin = EnumVisions.ADMINISTRATOR;
			}, 0);
		};
        
		/**
		* @name deleteSelectedTest
		* @namespace TestListController
		* @desc Deleta a prova selecionada pelo usuário 
		* @memberOf Controller.TestListController
		*/
		ng.deleteSelectedTest = function () {

			TestListModel.deleteTest({ Id: ng.selectedTest.Test.TestId }, function (result) {

				if (result.success) {

					angular.element("#modalExcluir").modal("hide");
					$notification.success(result.message);
					ng.getSearchResult();

				} else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @name closedModal
		 * @namespace TestListController
		 * @desc Fechando janela modal: Cancelando exclusão da prova selecionada
		 * @memberOf Controller.TestListController
		 */
		ng.closedModal = function () {
			angular.element("#modalExcluir").modal("hide");
		};

	   /**
		* @name createTest
		* @namespace TestListController
		* @desc Redirecionando para tela de cadastro de nova prova
		* @memberOf Controller.TestListController
		*/
		ng.createTest = function () {
			window.location.href = base_url("Test/IndexForm");
		};

		/**
		 * POPOVER: Possíveis ações para a prova selecionada
		 * na lista de provas encontradas na busca
		 */
		ng.popovermenu = {
			title: "empty",
			content: "empty",
			selectedTest: undefined,
			baixar: function () {
				getFilesTest();
				angular.element("#modalAlert").modal({ backdrop: 'static' });
			},
			vincular: function () {
				window.location.href = base_url("File/Index?Id=" + ng.selectedTest.Test.TestId);
			},
			editar: function () {
				window.location.href = base_url("Test/IndexForm/" + ng.selectedTest.Test.TestId);
			},
			excluir: function () {
				angular.element("#modalExcluir").modal({ backdrop: 'static' });
			},
			application: function () {
			    sessionStorage.setItem('leavingTest', JSON.stringify(true));
				window.location.href = base_url("Test/IndexAdministrate?test_id=" + ng.selectedTest.Test.TestId);
			},
			studentResponses: function () {
			    sessionStorage.setItem('leavingTest', JSON.stringify(true));
				window.location.href = base_url("Test/IndexStudentResponses?test_id=" + ng.selectedTest.Test.TestId);
			},
			adherence: function () {
			    sessionStorage.setItem('leavingTest', JSON.stringify(true));
				window.location.href = base_url("Adherence/Index?test_id=" + ng.selectedTest.Test.TestId);
			},
			exportar: function () {
				window.location.href = base_url("Test/IndexImport?test_id=" + ng.selectedTest.Test.TestId);
			},
			anular: function () {
				window.location.href = base_url("Test/IndexRevoke?test_id=" + ng.selectedTest.Test.TestId);
			},
			permissao: function () {
			    window.location.href = base_url("Test/IndexPermission?test_id=" + ng.selectedTest.Test.TestId);
			},
			hide: function () {
				angular.element("#modalHide").modal({ backdrop: 'static' });
			},
			show: function () {
				angular.element("#modalShow").modal({ backdrop: 'static' });
			}
		};

		/**
		 * @function Ocultar prova
		 * @author julio.silva@mstech.com
		 * @since 02/11/2016
		 */
		ng.hideTest = function __hideTest() {
			angular.element("#modalHide").modal('hide');
			TestListModel.changeTestVisible({ Id: ng.selectedTest.Test.TestId, Visible: false }, function (result) {
				if (result.success) {
					$notification.success(result.message);
					ng.getSearchResult();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Mostrar prova
		 * @author julio.silva@mstech.com
		 * @since 02/11/2016
		 */
		ng.showTest = function __showTest() {
			angular.element("#modalShow").modal('hide');
			TestListModel.changeTestVisible({ Id: ng.selectedTest.Test.TestId, Visible: true }, function (result) {
				if (result.success) {
					$notification.success(result.message);
					ng.getSearchResult();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.selectedVisible = function _selectedVisible() {
			ng.visibleTest = !ng.visibleTest;
		};

		ng.selectedMultidiscipline = function _selectedMultidiscipline() {
			ng.visibleMultidiscipline = !ng.visibleMultidiscipline;
		};

		/**
		 * @function Cancelar ocultar/mostrar prova
		 * @author julio.silva@mstech.com
		 * @since 02/11/2016
		 * @param {string} _target
		 */
		ng.cancelHideShow = function __cancelHideShow(_target) {
			angular.element("#".concat(_target)).modal('hide');
			ng.popovermenu.selectedTest = undefined;
		};

		/**
		 * @name getFilesTest
		 * @namespace TestListController
		 * @desc Busca todos os arquivos vinculados a prova
		 * @memberOf Controller.TestListController
		 */
		function getFilesTest() {
			TestListModel.getTestFiles({ Id: ng.selectedTest.Test.TestId }, function (result) {

				if (result.success) {
					ng.files = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.verMaisTest = function _verMaisTest(test_id) {
		    if ($.inArray(test_id, ng.listExpandido) < 0) {
		        $('#mais_' + test_id).show();
		        ng.listExpandido.push(test_id);

		        $('#acao_' + test_id).hide();
		        var indice = $.inArray(test_id, ng.listExpandidoAcao);
		        ng.listExpandidoAcao.splice(indice, 1);

		    }
		    else {
		        $('#mais_' + test_id).hide();
		        var indice = $.inArray(test_id, ng.listExpandido);
		        ng.listExpandido.splice(indice, 1);
		    }
		}

		ng.acaoTest = function _acaoTest(selectedTest) {
		    var test_id = selectedTest.Test.TestId;
		    if ($.inArray(test_id, ng.listExpandidoAcao) < 0) {
		        $('#acao_' + test_id).show();

                // remove a exibição dos outros itens.
		        $.each(ng.listExpandidoAcao, function (index, value) {
		            $('#acao_' + value).hide();
		        });

		        ng.listExpandidoAcao = [];
		        ng.listExpandidoAcao.push(test_id);

		        ng.changeTest(selectedTest);

		        $('#mais_' + test_id).hide();
		        var indice = $.inArray(test_id, ng.listExpandido);
		        ng.listExpandido.splice(indice, 1);
		    }
		    else {
		        $('#acao_' + test_id).hide();
		        $.each(ng.listExpandidoAcao, function (index, value) {
		            $('#acao_' + value).hide();
		        });

		        ng.listExpandidoAcao = [];
		    }
		}

		/**
	  * @name downloadFile
	  * @namespace TestListController
	  * @desc Faz o download individual do arquivo vinculado a prova
	  * @memberOf Controller.TestListController
	  */
		ng.downloadFile = function _downloadFile(id) {

			FileModel.checkFileExists({ Id: id }, function (result) {

				if (result.success) {
					window.open("/File/DownloadFile?Id=" + id, "_self");
				}
				else {
					$notification.alert("Não foi possível baixar o arquivo selecionado!");
				}
			});
		};

		/**
		 * @name downloadAllFilesZip
		 * @namespace TestListController
		 * @desc Faz o download de todos os arquivos vinculados a prova
		 * @memberOf Controller.TestListController
		 */
		ng.downloadAllFilesZip = function downloadAllFilesZip() {
			TestListModel.checkFilesExists({ Id: ng.selectedTest.Test.TestId }, function (result) {
				if (result.success) {
					window.open("/Test/DownloadZipFiles?Id=" + ng.selectedTest.Test.TestId, "_self");
				} else {
					$notification.alert("Arquivo(s) não encontrado(s).");
				}
			});
		};

		/**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace TestListController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		ng.safeApply = function __safeApply() {
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

		ng.datepicker = function (id) { $("#" + id).datepicker('show'); };

		/**
		 * @function Abrir/fechar painel de filtros
		 * @param
		 * @returns
		 */
		ng.open = function __open() {

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

			if (angular.element(".side-filters").hasClass("side-filters-animation")) ng.open();
		}; angular.element('body').click(close);

	    /**
		* @name moverProvaBaixo
		* @namespace TestListController
		* @param {test} - Dados da prova
		* @memberOf Controller.TestListController
		*/
		ng.moverProvaBaixo = function __moverProvaBaixo(test) {
		    TestListModel.changeOrderTestDown({ Id: test.Test.TestId, order: test.Test.Order }, function (result) {
		        if (result.success) {
		            ng.getSearchResult();
		        } else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

	    /**
		* @name moverProvaCima
		* @namespace TestListController
		* @param {test} - Dados da prova
		* @memberOf Controller.TestListController
		*/
		ng.moverProvaCima = function __moverProvaCima(test) {
		    TestListModel.changeOrderTestUp({ Id: test.Test.TestId, order: test.Test.Order }, function (result) {
		        if (result.success) {
		            ng.getSearchResult();
		        } else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

	    /**
		 * @function Obter todos os grupos
		 * @param
		 * @returns
		 */
		function getGroups() {
		    TestListModel.getGroups({}, function (result) {
				ng.group = true;
		        if (result.success) {
		            ng.listFilter.Groups = result.lista;
		            if (ng.listFilter.Groups.length === 1) {
		                ng.selectedGroup = ng.listFilter.Groups[0];
		                getSubGroup();
		            }
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};
		
	    /**
		 * @function Obter todas Escolas
		 * @param
		 * @returns
		 */
		ng.getSubGroup = function __getSubGroup() {

		    if (ng.selectedGroup === undefined || ng.selectedGroup === null)
		        return;

		    var params = { 'Id': ng.selectedGroup.Id };

		    TestListModel.getSubGroup(params, function (result) {

		        if (result.success) {
		            ng.listFilter.SubGroups = result.modelList.TestSubGroups;
		            if (ng.listFilter.SubGroups.length === 1)
		                ng.selectedSubGroup = ng.listFilter.SubGroups[0];
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		ng.alterarVisualizacao = function __alterarVisualizacao(tipoVisualizacao) {
		    ng.tipoVisualizacao = tipoVisualizacao;
		}

		ng.alterarOrdenacao = function __alterarOrdenacao() {
		    ng.getSearchResult();
		}

		ng.backGroup = function __backGroup() {
		    ng.getGroup = false;
		    searchFilterFormat();
		}

		ng.handleDrop = function (idOrigem, idDestino) {
		    if (ng.profile == EnumVisions.ADMINISTRATOR) {
		        if (ng.getGroup) {
		            if (idOrigem == 0 || idDestino == 0) {
		                $notification['alert']('O grupo de provas antigas não pode ser movido.');
		            }
		            else {
		                TestListModel.changeOrderSubGroup({ idOrigem: idOrigem, idDestino: idDestino }, function (result) {
		                    if (result.success) {
		                        ng.getSearchResult();
		                    } else {
		                        $notification[result.type ? result.type : 'error'](result.message);
		                    }
		                });
		            }
		        }
		        else {
		            TestListModel.changeOrderTest({ idOrigem: idOrigem, idDestino: idDestino }, function (result) {
		                if (result.success) {
		                    ng.getSearchResult();
		                } else {
		                    $notification[result.type ? result.type : 'error'](result.message);
		                }
		            });
		        }
		    }

		    return;
		}

		init();
	};

})(angular, jQuery);