/**
 * function ReportPerformanceController Controller
 * @namespace Controller
 * @author 
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("ReportPerformanceController", ReportPerformanceController);

	ReportPerformanceController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$pager', '$notification', '$timeout', 'ReportPerformanceModel'];


	/**
	 * @function Controller 'Resultados da prova'
	 * @name ReportPerformanceController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $window
	 * @param {Object} $sce
	 * @param {Object} $util
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @param {Object} ReportPerformanceModel
	 * @returns
	 */
	function ReportPerformanceController($rootScope, $scope, $window, $sce, $util, $pager, $notification, $timeout, ReportPerformanceModel) {
		$scope.paginate = $pager();
		$scope.pageSize = 10;
		/**
		 * @function Método Construtor da pagina
		 * @param {Object} authorize
		 * @returns
		 */
		$scope.init = function __init() {

			configStart();
			$(document).ready(function () {
				var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

				var startDate = new Date();
				startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
				$("#dateStart").datepicker("setDate", startDate);

				var endDate = new Date();
				endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
				$("#dateEnd").datepicker("setDate", endDate);


			});


		};


		function updateFilterCount() {
			$scope.countFilter = 0;
			if ($scope.filters.DRE) $scope.countFilter += 1;
			if ($scope.filters.School) $scope.countFilter += 1;
			if ($scope.filters.Group) $scope.countFilter += 1;
			if ($scope.filters.SubGroup) $scope.countFilter += 1;
			if ($scope.filters.Test_id) $scope.countFilter += 1;
		};

		/**
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @param
		 * @returns
		 */
		function configListFilter() {

			$scope.listFilter = {
				DREs: [],
				Schools: [],
				Groups: [],
				SubGroups: [],
				Tests: [],
				CurriculumGrades: [],
				TestsBySubGroup: [],
				Disciplines: []
			};
		};

		/**
		 * @function Configuração dos filtros
		 * @param
		 * @returns
		 */
		function configFilters() {
			$scope.filters = {
				DRE: undefined,
				School: undefined,
				Group: undefined,
				SubGroup: undefined,
				DateStart: undefined,
				DateEnd: undefined,
				Test_id: undefined,
				CurriculumGrade: undefined,
				Discipline: undefined
			};
		};

		/**
		* @function Mudanças global Local
		* @name clearGlobalFilters
		* @namespace TestListController
		* @memberOf Controller
		* @public
		* @param
		* @returns
		*/
		function clearGroupFilters() {
			$scope.group = false;
			$scope.listFilter.SubGroups = [];
			$scope.listFilter.Groups = [];
			$scope.listFilter.TestsBySubGroup = [];
			$scope.listFilter.Disciplines = [];
			$scope.listFilter.CurriculumGrades = [];
			$scope.filters.SubGroup = undefined;
			$scope.filters.Group = undefined;
			$scope.filters.Discipline = undefined;
			$scope.filters.CurriculumGrade = undefined;
		};
		$scope.clearGroupFilters = clearGroupFilters;

		/**
		 * @function Configuração das listas
		 * @param
		 * @returns
		 */
		function configLists() {

			$scope.list = {
				displayed: []
			};
		};
		/**
		 * @function Obter objeto inicial para configuração da page.
		 * @param {Object} 
		 * @returns
		 */
		function configStart() {
			$scope.group = false;
			$scope.countFilter = 0;
			$scope.blockPage = false;
			$scope.typeFile = false;
			$scope.searchFilter = false;
			$scope.batchWarning = { status: false, message: "" };
			$scope.AllowAnswer = true;
			$scope.ShowResult = true;
			$scope.processingFilter = [];
			$scope.labelTermTest = "Prova individual";
			$scope.labelTermGroupTest = "Grupo de provas";
			configListFilter();
			configLists();
			configFilters();
			$scope.clearFilters();
			getDREs();
			$scope.showTabs = false;

			$scope.chart1 = {
			    visible: false,
			    chart: {}
			};

			//$scope.setPagination();
			$scope.$watchCollection("[filters.DRE, filters.School, filters.Group, filters.SubGroup, filters.Test_id]", function () {
				updateFilterCount();
			}, true);
		};

		/**
		 * @function Obter todas DREs
		 * @param
		 * @returns
		 */
		function getDREs() {

			ReportPerformanceModel.getDRESimple({}, function (result) {

				if (result.success) {
					$scope.listFilter.DREs = result.lista;
					if ($scope.listFilter.DREs.length === 1)
					{
						$scope.filters.DRE = $scope.listFilter.DREs[0];
						getSchools();
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
		function getSchools() {

			if ($scope.filters.DRE === undefined || $scope.filters.DRE === null)
				return;

			var params = { 'dre_id': $scope.filters.DRE.Id };

			ReportPerformanceModel.getSchoolsSimple(params, function (result) {

				if (result.success) {
					$scope.listFilter.Schools = result.lista;
					if ($scope.listFilter.Schools.length === 1)
						$scope.filters.School = $scope.listFilter.Schools[0];
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};
		$scope.getSchools = getSchools;
		/**
		 * @function Obter todos os grupos
		 * @param
		 * @returns
		 */
		function getGroups() {
			$scope.filters.Test_id = undefined;
			$scope.filters.Discipline = undefined;
			$scope.filters.CurriculumGrade = undefined;
			$scope.listFilter.Tests = [];
			$scope.listFilter.Disciplines = [];
			$scope.listFilter.CurriculumGrades = [];
			ReportPerformanceModel.getGroups({ }, function (result) {
				$scope.group = true;
				if (result.success) {
					$scope.listFilter.Groups = result.lista;
					if ($scope.listFilter.Groups.length === 1) {
						$scope.filters.Group = $scope.listFilter.Groups[0];
						getSubGroup();
					}
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};
		$scope.getGroups = getGroups;
		/**
		 * @function Obter todas Escolas
		 * @param
		 * @returns
		 */
		function getSubGroup() {

			if ($scope.filters.Group === undefined || $scope.filters.Group === null)
				return;

			var params = { 'Id': $scope.filters.Group.Id };

			ReportPerformanceModel.getSubGroup(params, function (result) {

				if (result.success) {
					$scope.listFilter.SubGroups = result.modelList.TestSubGroups;
					if ($scope.listFilter.SubGroups.length === 1)
						$scope.filters.SubGroup = $scope.listFilter.SubGroups[0];
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};
		$scope.getSubGroup = getSubGroup;
		/**
		* @function Obter as provas dado um período
		* @param {object} _callback
		* @returns
		*/
		function getTests(_callback) {
			if ($scope.filters.DateStart && $scope.filters.DateEnd) {
				if (!validateDate()) return;
				ReportPerformanceModel.getTestByDate({ DateStart: $scope.filters.DateStart, DateEnd: $scope.filters.DateEnd }, function (result) {
					if (result.success) {
						$scope.listFilter.Tests = result.lista;
						if ($scope.listFilter.Tests.length === 0)
							$notification.alert("Nenhuma prova foi encontrada no período selecionado.")
						else if ($scope.listFilter.Tests.length === 1)
							$scope.filters.Test_id = $scope.listFilter.Tests[0].TestId;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			if (_callback) _callback();
		};
		$scope.getTests = getTests;
		/**
		* @function Obter as provas dado um período
		* @param {object} _callback
		* @returns
		*/
		function getTestsBySubGroup(_callback) {

			ReportPerformanceModel.getTestsBySubGroup({ Id: $scope.filters.SubGroup }, function (result) {
				if (result.success) {
					$scope.listFilter.Tests = result.lista;
					if ($scope.listFilter.Tests.length === 0)
						$notification.alert("Nenhuma prova foi encontrada no período selecionado.")
					else if ($scope.listFilter.Tests.length === 1)
						$scope.filters.Test_id = $scope.listFilter.Tests[0].TestId;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};
		$scope.getTestsBySubGroup = getTestsBySubGroup;

		/**
		* @function Obter os períodos
		* @param {object} _callback
		* @returns
		*/
		function getCurriculumGradeTests(_callback) {
			if ($scope.group === true) {
				ReportPerformanceModel.getDistinctCurricumGradeByTestSubGroup_Id({ TestSubGroup_Id: $scope.filters.SubGroup.Id }, function (result) {
					if (result.success) {
						$scope.listFilter.CurriculumGrades = result.lista;
						if ($scope.listFilter.CurriculumGrades.length === 0)
							$notification.alert("Nenhuma ano de aplicação foi encontrado.")
						else
							$scope.filters.CurriculumGrade = $scope.listFilter.CurriculumGrades[0];
						getAveragesTests();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			else {
				ReportPerformanceModel.getCurriculumGradeByTestId({ Test_id: $scope.filters.Test_id }, function (result) {
					if (result.success) {
						$scope.listFilter.CurriculumGrades = result.lista;
						if ($scope.listFilter.CurriculumGrades.length === 0)
							$notification.alert("Nenhuma ano de aplicação foi encontrado.")
						else
							$scope.filters.CurriculumGrade = $scope.listFilter.CurriculumGrades[0];
						getAveragesTests();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}

		};
		$scope.getCurriculumGradeTests = getCurriculumGradeTests;
		/**
		* @function Obter as disciplinas
		* @param {object} _callback
		* @returns
		*/
		function getDisciplinesTests(_callback) {
			if ($scope.group === true) {
				ReportPerformanceModel.getDisciplinesByTestSubGroup_Id({ TestSubGroup_Id: $scope.filters.SubGroup.Id }, function (result) {
					if (result.success) {
						$scope.listFilter.Disciplines = result.lista;
						if ($scope.listFilter.Disciplines.length === 0)
							$notification.alert("Nenhuma ano de aplicação foi encontrado.")
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			else {
				ReportPerformanceModel.getDisciplineByTestId({ Test_id: $scope.filters.Test_id }, function (result) {
					if (result.success) {
						$scope.listFilter.Disciplines = result.lista;
						if ($scope.listFilter.Disciplines.length === 0)
							$notification.alert("Nenhuma ano de aplicação foi encontrado.")
						else if ($scope.listFilter.Disciplines.length === 1)
							$scope.filters.Discipline = $scope.listFilter.Disciplines[0].Id;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};
		$scope.getDisciplinesTests = getDisciplinesTests;
		/**
		* @function Obter as médias
		* @param {object} _callback
		* @returns
		*/
		function getAveragesTests(_callback) {
			var params = {};
			if ($scope.filters.DRE !== undefined && $scope.filters.DRE !== null)
				params.dre_id = $scope.filters.DRE.Id;
			if ($scope.filters.School !== undefined && $scope.filters.School !== null)
				params.esc_id = $scope.filters.School.Id;
			if ($scope.filters.CurriculumGrade !== undefined && $scope.filters.CurriculumGrade !== null)
				params.tcp_id = $scope.filters.CurriculumGrade.tcp_id;
			if ($scope.filters.Discipline !== undefined && $scope.filters.Discipline !== null)
				params.discipline_id = $scope.filters.Discipline.Id;
			if ($scope.group === true) {
				params.subgroup_id = $scope.filters.SubGroup.Id;
				ReportPerformanceModel.GetAveragesByTestSubGroup_Id(params, function (result) {
					if (result.success) {
						$scope.listFilter.Averages = result.data;
						if ($scope.listFilter.Averages.length === 0)
							$notification.alert("Nenhum resultado encontrado.")
						$scope.carregaGrafico_Desempenho();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			else {
				params.test_id = $scope.filters.Test_id;

				ReportPerformanceModel.GetAveragesByTest(params, function (result) {
					if (result.success) {
						$scope.listFilter.Averages = result.data;
						if ($scope.listFilter.Averages.length === 0)
							$notification.alert("Nenhuma ano de aplicação foi encontrado.")
						$scope.carregaGrafico_Desempenho();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};
		$scope.getAveragesTests = getAveragesTests;
		/**
		 * @function Validar período de datas
		 * @param 
		 * @returns
		 */
		function validateDate() {
			if (!$scope.filters.DateStart || !$scope.filters.DateEnd) {
				$notification.alert("É necessário selecionar um período.");
				return false;
			}
			if ($util.greaterEndDateThanStartDate($scope.filters.DateStart, $scope.filters.DateEnd) === false) {
				$notification.alert("Data inicial não pode ser maior que a data final.");
				$scope.filters.DateEnd = "";
				return false;
			}
			return true;
		};
		function changeTest(_callback) {
			if ($scope.filters.Test_id > 0 && $('#selectTest').val() != "") {
				$('#testCode').val($scope.filters.Test_id)
			}

			if (_callback) _callback();
		};
		$scope.changeTest = changeTest;

		function changeTestCode(_callback) {
			if ($scope.filters.Test_id > 0 && $('#testCode').val() != "") {
				$('#selectTest').val($scope.filters.Test_id)
			}

			if (_callback) _callback();
		};
		$scope.changeTestCode = changeTestCode;
		/**
		* @function obter detalhes de cabeçalho da page
		* @param {object} _callback
		* @returns
		*/
		function getHeaderDetails(_callback) {
			if ($scope.group === true) {
				$scope.header.Description = $scope.filters.Group.Description;
				$scope.header.SubDescription = $scope.filters.SubGroup.Description
			}
			else {
				if (!$scope.filters.Test_id) return;
				ReportPerformanceModel.getInfoTestReport({ Test_id: $scope.filters.Test_id }, function (result) {
					if (result.success) {
						$scope.header = result.lista;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
					if (_callback) _callback();
				});

				ReportPerformanceModel.getInfoTestCurriculumGrade({ Test_id: $scope.filters.Test_id }, function (result) {
					if (result.success) {
						$scope.headerCurriculumGrade = result;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
					if (_callback) _callback();
				});
			}


		};
		$scope.getHeaderDetails = getHeaderDetails;
		/**
		 * @function Limpeza dos filtros de pesquisa.
		 * @param
		 * @returns
		 */
		function clearFilters() {

			$scope.listFilter.Schools = [];
			$scope.School = null;
			configFilters();
			$scope.countFilter = 0
		};
		$scope.clearFilters = clearFilters;

		/**
		 * @function Limpeza dos filtros de pesquisa
		 * @param {string} filter
		 * @returns
		 */
		function clearByFilter(filter) {

			if (filter === 'DRE') {
				$scope.listFilter.Schools = [];
				$scope.filters.School = undefined;
				return;
			}

			if (filter === 'School') {

				return;
			}
			if (filter === 'Group') {
				$scope.listFilter.SubGroups = [];
				$scope.listFilter.TestsBySubGroup = [];
				$scope.filters.SubGroup = undefined;
				return;
			}
			if (filter === 'Subgroup') {
				$scope.listFilter.TestsBySubGroup = [];
				return;
			}
			if (filter === 'Type') {
				$scope.listFilter.Groups = [];
				$scope.listFilter.SubGroups = [];
				$scope.listFilter.TestsBySubGroup = [];
				$scope.filters.Group = undefined;
				$scope.filters.SubGroup = undefined;
				return;
			}
		};
		$scope.clearByFilter = clearByFilter;
		/**
		 * @function Pesquisa.
		 * @param
		 * @returns
		 */
		function search() {
			if (!validateDate() || !validate()) return;

			$scope.header = { Description: "", SubDescription: "" };
			getHeaderDetails();
			getDisciplinesTests();
			getCurriculumGradeTests();
			$scope.showTabs = true;
		};
		$scope.search = search;
		/**
		* @function validar
		* @param
		* @returns
		*/
		function validate() {
		    if (!$scope.filters.DRE) {
		        $notification.alert("É necessário selecionar uma DRE.");
		        return false;
		    }

		    if (!$scope.filters.School) {
		        $notification.alert("É necessário selecionar uma escola.");
		        return false;
		    }

			if ($scope.group === true) {
				if (!$scope.filters.SubGroup) {
					$notification.alert("É necessário selecionar um subgrupo.");
					return false;
				}
			}
			else {
				if (!$scope.filters.Test_id) {
					$notification.alert("É necessário selecionar uma prova ou preencher o seu código.");
					return false;
				}

				if (!Number.isInteger($scope.filters.Test_id)) {
					$notification.alert("Código da prova inválido.");
					return false;
				}
				var position = $scope.listFilter.Tests.indexOfField("TestId", $scope.filters.Test_id);
				if (position < 0) {
					$notification.alert("Prova não encontrada ou fora do período informado.");
					return false;
				}
			}
			return true;
		};

		Array.prototype.indexOfField = function (propertyName, value) {
			for (var i = 0; i < this.length; i++)
				if (this[i][propertyName] === value)
					return i;
			return -1;
		}
		/*
		  @function Montar gráfico de desempenho de alunos, turma, escola, DRE, SME 
		*/
		$scope.carregaGrafico_Desempenho = function () {

		    var turIds = [], testIds = [], turName = [], turPerformance = [], DREAverage = [], SMEAverage = [], schoolAverage = [];

		    for (var x = 0; x < $scope.listFilter.Averages.AvgTeams.length; x++) {
		        turIds.push($scope.listFilter.Averages.AvgTeams[x].Tur_id);
		        testIds.push($scope.listFilter.Averages.AvgTeams[x].Test_id);
				turName.push(["Turma " + $scope.listFilter.Averages.AvgTeams[x].Tur_codigo]);
				turPerformance.push($scope.listFilter.Averages.AvgTeams[x].Media);
				schoolAverage.push($scope.listFilter.Averages.AvgESC);
				DREAverage.push($scope.listFilter.Averages.AvgDRE);
				SMEAverage.push($scope.listFilter.Averages.AvgSME);
			}

		    $scope.listFilter.TurIds = turIds;
		    $scope.listFilter.TestIds = testIds;

			var data = {
			    teams: turIds,
			    tests: testIds,
                //teamsNames: turName,
			    datasets: [
                {
                    type: 'line',
                    label: 'Média escola',
                    data: schoolAverage,
                    fill: false,
                    borderColor: 'red',
                    backgroundColor: 'red',
                    pointBorderColor: 'red',
                    pointBackgroundColor: 'red',
                    pointHoverBackgroundColor: 'red',
                    pointHoverBorderColor: 'red',
                    yAxisID: 'y-axis-1'
                },
                {
                    type: 'line',
                    data: SMEAverage,
                    fill: false,
                    label: 'Média SME',
                    borderColor: 'orange',
                    backgroundColor: 'orange',
                    pointBorderColor: 'orange',
                    pointBackgroundColor: 'orange',
                    pointHoverBackgroundColor: 'orange',
                    pointHoverBorderColor: 'orange',
                    yAxisID: 'y-axis-2',
                    index: 9999
                },
                {
                    type: 'line',
                    data: DREAverage,
                    fill: false,
                    label: 'Média DRE',
                    borderColor: 'green',
                    backgroundColor: 'green',
                    pointBorderColor: 'green',
                    pointBackgroundColor: 'green',
                    pointHoverBackgroundColor: 'green',
                    pointHoverBorderColor: 'green',
                    yAxisID: 'y-axis-3',
                    index: 9999
                },
                {
                    type: 'bar',
                    label: 'Desempenho turma',
                    data: turPerformance,
                    backgroundColor: '#7cb5ec',
                    yAxisID: 'y-axis-0'
                }                
                ],
			    labels: turName
			};

			var canvas = document.getElementById('gfcPerformance');
			var ctx = canvas.getContext("2d");

			if ($scope.chart1.chart && $scope.chart1.chart.canvas)
			    $scope.chart1.chart.destroy();

			$scope.chart1 = {
			    visible: true, chart: new Chart(ctx, {
			        type: 'bar',
			        options: {
			            responsive: true,
			            scales: {
			                yAxes: [{
                                display: true,
			                    id: "y-axis-0",
			                    gridLines:{
			                        display: true
			                    },
			                    ticks: {
			                        min: 0,
			                        stepSize: 10,
			                        max: 100,
			                        callback: function (value) {
			                            return value + "%"
			                        }
			                    }
			                }, {
			                    display: false,
			                    id: "y-axis-1",
			                    gridLines: {
			                        display: false
			                    },
			                    ticks: {
			                        min: 0,
			                        stepSize: 10,
			                        max: 100
			                    }
			                }, {
			                    display: false,
			                    id: "y-axis-2",
			                    gridLines: {
			                        display: false
			                    },
			                    ticks: {
			                        min: 0,
			                        stepSize: 10,
			                        max: 100
			                    }
			                }, {
			                    display: false,
			                    id: "y-axis-3",
			                    gridLines: {
			                        display: false
			                    },
			                    ticks: {
			                        min: 0,
			                        stepSize: 10,
			                        max: 100
			                    }
			                }]
			            }
			        },
			        data: data
			    })
			};

			canvas.onclick = function (evt) {
			    var activePoints = $scope.chart1.chart.getElementsAtEvent(evt);
			    if (activePoints && activePoints.length > 0) {
			        var idx = activePoints[0]['_index'];

			        window.open("/Correction?test_id=" + $scope.listFilter.TestIds[idx] + "&team_id=" + $scope.listFilter.TurIds[idx] + "&esc_id=" + $scope.filters.School.Id + "&dre_id=" + $scope.filters.DRE.Id, "_blank");
			    }
			};

			if (turPerformance.length > 0) {
			    $('#gfcPerformance').show();
			    $('.abre-zoom').show();
			    $('.downloadChartCorrection').show();
			    $('.fecha-zoom').hide();
			}
			else {
			    $('#gfcPerformance').hide();
			    $('.abre-zoom').hide();
			    $('.downloadChartCorrection').hide();
			}

			return;
		};

		$scope.abreZoom = function abreZoom(_callback) {
		    $('.expandir').addClass('fixed-zoom');
		    $('.fecha-zoom').show();
		}


		$scope.fechaZoom = function fechaZoom(_callback) {
		    $('.expandir').removeClass('fixed-zoom');
		    $('.fecha-zoom').hide();
		}

		$scope.downloadChart = (function (graph) {
		    $(graph).get(0).toBlob(function (blob) {
		        saveAs(blob, "grafico.png");
		    });
		});

		/**
		 * @function Acionar date-picker através de btn
		 * @param
		 * @returns
		 */
		$scope.datepicker = function __datepicker(id) { $("#" + id).datepicker('show'); };
		/**
		 * @function Abrir/fechar painel de filtros
		 * @param
		 * @returns
		 */
		function open() {

			$('.side-filters').toggleClass('side-filters-animation').promise().done(function a() {

				if (angular.element(".side-filters").hasClass("side-filters-animation")) {
					angular.element('body').css('overflow', 'hidden');
				}
				else {
					angular.element('body').css('overflow', 'inherit');
				}
			});
		};
		$scope.open = open;
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