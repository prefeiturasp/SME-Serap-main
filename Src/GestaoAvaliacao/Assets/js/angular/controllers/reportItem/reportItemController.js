/**
 * @function Report Controller
 * @namespace Controller
 * @author Alexandre Calil Blasizza Paravani 23/04/2015
 * @author Alexandre Garcia Simões 09/06/2015
 * @author Leticia Langeli Garcia De Goes 01/10/2015
 * @author Thiago Macedo Silvestre 30/10/2015
 * @author Everton Luis Ferreira 04/11/2015 14:30:46
 * @author Julio Cesar da Silva  06/11/2015
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
		.module('appMain')
		.controller("ReportController", ReportController);


	ReportController.$inject = ['ReportItemModel', 'DisciplineModel', 'EvaluationMatrixModel', 'ItemSituationModel', 'SkillModel', '$scope', '$rootScope', '$notification', '$timeout', 'LevelEducationModel'];


	/**
	 * @function Controller 'Relatórios'
	 * @name ReportController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} ReportItemModel
	 * @param {Object} DisciplineModel
	 * @param {Object} EvaluationMatrixModel
	 * @param {Object} ItemSituationModel
	 * @param {Object} SkillModel
	 * @param {Object} ng
	 * @param {Object} rootScope
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @param {Object} LevelEducationModel
	 * @return
	 */
	function ReportController(ReportItemModel, DisciplineModel, EvaluationMatrixModel, ItemSituationModel, SkillModel, ng, rootScope, $notification, $timeout, LevelEducationModel) {

	    ng.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;

		angular.element(document).ready(function () {
			$notification.clear();
			ng.init();
		});


		/**
		 * @function Inicialização.
		 * @name init
		 * @namespace ReportController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		ng.init = function () {
		    ng.graphColors = ['#7cb5ec', '#434348', '#90ed7d', '#f7a35c', '#8085e9', '#f15c80', '#e4d354', '#2b908f', '#f45b5b', '#91e8e1', '#96e9e1', '#2f998f', '#8f80e9', '#494949'];
			ng.materia = {
				masterLabel: "Componente curricular*:",
				lock: false,
				objMateria: undefined,
				indexMateria: undefined,
				lista: []
			};
			ng.situacao = {
				inicio: null,
				fim: null
			};
			ng.highchartObj3 = {
				visible: false,
				highchart: {}
			};
			ng.chart1 = {
				visible: false,
				chart: {}
			};
			ng.chart2 = {
			    visible: false,
			    chart: {}
			};
			ng.chart3 = {
			    visible: false,
			    chart: {}
			};
			ng.chart4 = {
			    visible: false,
			    chart: {}
			};
			ng.chart5 = {
			    visible: false,
			    chart: {}
			};
			ng.chart6 = {
			    visible: false,
			    chart: {}
			};
			ng.carregaLevelEducation();
			//carrega matriz
			ng.matrizshow = true;
		};


		/**
		* @function - Carregar Level Education
		* @param
		* @private
		*/
		ng.carregaLevelEducation = function (result) {
		    LevelEducationModel.load(function (result) {
				if (result.success) {
					ng.tipoNivelEnsinoList = result.lista;
					ng.$watch('selectedObjTipoNivelEnsino', ng.loadDisciplines);
				}
				else {

					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


		/**
		* @functioncarregar Disciplinas
		* @param 
		* @public
		*/
		ng.carregaDisciplines = function () {		
			if (ng.selectedObjTipoNivelEnsino != undefined) {
				ng.materia.lista = [];
				DisciplineModel.searchDisciplinesSaves({ typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id }, function (result) {
					if (result.success) {
								
						var flag = true;
						for (var i = 0; i < result.lista.length; i++)
							if (result.lista[i].Description == "Geral")
								flag = false;

						if( flag )
							result.lista.push({ Description: "Geral", Id: -1, LevelEducationDescription: "", Url: base_url('/Assets/images/Geral.png') });
						ng.materia.lista = result.lista;
						ng.materia.objMateria = result.lista[(result.lista.length - 1)];
						ng.carregasituacao();
								
						try {
							if (ng.disciplinesList.length == 0)
								$notification.alert("Não existem componente(s) curricular(es) cadastrado(s) para esse nível de ensino.");
						} catch (e) { }

					}
					else {
								
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};


		/**
		 * @function - Carrega situação dos itens com base no campo checado
		 * @param 
		 * @public
		 */
		ng.carregasituacao = function () {
		    ItemSituationModel.load(function (result) {
				if (result.success) {
					if (ng.materia.objMateria != undefined) {
						ng.carregasituacaoList = result.lista;
						ng.s1 = result.lista[0];
						ng.s2 = result.lista[0];
						ng.s3 = result.lista[0];
						ng.carregaGrafico_Item();
						ng.carregaEvaluationMatrix();
						ng.carregaGrafico_ItemSituation();
						ng.carregas1(ng.s1);
						ng.carregas2(ng.s2);
						ng.carregas3(ng.s3);
					}
				}
				else {

					$notification[result.type ? result.type : 'error'](result.message);
				}
			})
		};


		/**
		 * @function Recarrega o grafico conforme a materia selecionada
		 * @param 
		 * @public
		 */
		ng.carregaMaterias = function ( _cb ) {
			if (_cb)
				ng.materia.objMateria = _cb;
			ng.skills = null;
			ng.lastLevel = null;
			ng.highchartObj3.visible = false;
			ng.chart6.visible = false;
			if (ng.materia.objMateria != null) {
				if (ng.materia.objMateria.Id == -1) {
					ng.carregaGrafico_Item();
				}
			}
			ng.carregaGrafico_ItemSituation();
			if (ng.materia.objMateria) {
				ng.carregaEvaluationMatrix();
				ng.carregas1(ng.s1);
				ng.carregas2(ng.s2);
				ng.carregas3(ng.s3);
			}
		};


		/**
		 * @function Exibe relatório de acordo com o combo selecionado de situação dos itens 
		 * (Relatório Distribuição por tipo de questão)
		 * @param 
		 * @public
		 */
		ng.carregas1 = function __carregas1(itemType) {

			if (ng.s1 != undefined) {
				ng.carregaGrafico_ItemType(itemType);
			}
		};


		/**
		 * @function Exibe relatório de acordo com o combo selecionado de situação dos itens 
		 * (Relatório Distribuição por ano)
		 * @param 
		 * @public
		 */
		ng.carregas2 = function __carregas2(itemCurriculumGrade) {

			if (ng.s2 != undefined) {
				ng.carregaGrafico_ItemCurriculumGrade(itemCurriculumGrade);
			}
		};


		/**
		 * @function Exibe relatório de acordo com o combo selecionado de situação dos itens 
		 * (Relatório Distribuição por grau de dificuldade)
		 * @param 
		 * @public
		 */
		ng.carregas3 = function __carregas3(itemLevel) {

			if (ng.s2 != undefined) {
				ng.carregaGrafico_ItemLevel(itemLevel);
			}
		};


		/**
		 * @functioncarregar Carrega penultimo nível da matriz
		 * @param {Object} result
		 * @public
		 */
		ng.carregaSkill = function (result) {

		    ReportItemModel.GetByMatrix({ Id: result.Id }, function (result) {

				if (result.success) {

					ng.lastLevel = null;
					ng.chart6.visible = false;

					//retorno com penúltimo e último nível
					if (result.lista.length >= 2) {
						ng.lastButOne = result.lista[(result.lista.length-2)].ModelSkillLevels.Description;
						ng.last = result.lista[(result.lista.length-1)].ModelSkillLevels.Description;
						ng.skills = formatSkillReturn(result.lista[(result.lista.length-2)].ModelSkillLevels.Skills);
						return;
					}
					//retorno com apenas último nível
					else {
						ng.lastButOne = undefined;
						ng.last = result.lista[0].ModelSkillLevels.Description;
						var skills = formatSkillReturn(result.lista[0].ModelSkillLevels.Skills);
						ng.lastLevel = skills;
						loadLastLevelByMatrix();
						return;
					}
				}
				else {
					ng.skills = [];
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


	    /**
          * @function Carregar gráfico por matriz quando a mesma tiver apenas 1 nível.
          * @name loadLastLevelByMatrix
          * @namespace RepostItemController
          * @memberOf Controller
          * @author Julio Silva
          * @param
          * @return
          */
		function loadLastLevelByMatrix( ) {

		    var params = {
		        Id: ng.materia.objMateria.Id,
		        matrizId: ng.modelMatrizAval.Id,
		        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
		    };
				
		    ReportItemModel.load_reportItemSkillOneLevel(params, function (result) {

				if (result.success) {

				    if (result.lista.length > 0)
				        ng.lastTittle = result.lista[0].ModelDescription;

				    ng.lastLevel = result.lista;

				    ng.carregaGrafico_ItemSkill(ng.lastLevel);
				}
				else {
				    $notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};
		

		/**
		 * @function Formatar retorno skills
		 * @name formatSkillReturn
		 * @namespace repostItemController
		 * @memberOf Controller
		 * @param
		 * @return
		 */
		function formatSkillReturn(_tempSkill) {

			var _list = [];
			for (var i = 0; i < _tempSkill.length; i++) {
				_list.push({
					Id: _tempSkill[i].Id,
					Description: _tempSkill[i].Description,
					ParentId: _tempSkill[i].ParentId,
					visible: false
				})
			}

			return _list;
		};


		/**
		 * @function - Carregar Controle de carregamento de skills
		 * @param {string} s
		 * @public
		 */
		ng.loaderSkillsController = function (s) {

			for (var i = 0; i < ng.skills.length; i++) {

				if (ng.skills[i].Id == s.Id) {
					ng.skills[i].visible = !ng.skills[i].visible
				}
				else {
					ng.skills[i].visible = false;
				}
			}

			if (s.visible)
				ng.carregaLastLevel(s)
		};

		/**
		 * @function - Carrega ultimo nível da matriz
		 * @param {Object} r
		 * @public
		 */
		ng.carregaLastLevel = function (r) {

		    ReportItemModel.load_reportItemSkill({
		        Id: ng.materia.objMateria.Id,
		        skill: r.Id,
		        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
		    }, function (result) {

				if (result.success) {

					if (result.lista.length > 0)
						ng.lastTittle = result.lista[0].ModelDescription;

					ng.lastLevel = result.lista;

					ng.carregaGrafico_ItemSkill(ng.lastLevel);
				}
				else {
					//ng.skills = [];
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function - Carrega Matrix
		 * @param {Object} result
		 * @public
		 */
		ng.carregaEvaluationMatrix = function (result) {

			if (ng.materia.objMateria.Id != null && ng.materia.objMateria.Id != -1) {
				ng.evaluationMatrixList = [];
				ng.skills = [];
				EvaluationMatrixModel.getComboByDiscipline({ Id: ng.materia.objMateria.Id }, function (result) {

				    if (result.success) {

				        ng.evaluationMatrixList = result.lista;
				    }
				    else {
				        $notification[result.type ? result.type : 'error'](result.message);

				    }
				});
			};

			if (ng.materia.objMateria.Id == -1) {

			    EvaluationMatrixModel.loadComboSimple({ typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id }, function (result) {

			        if (result.success) {

			            ng.evaluationMatrixList = result.lista;
			        }
			        else {

			            $notification[result.type ? result.type : 'error'](result.message);
			        }
			    });
			};
		};

		/**
		 * @function - Relatório Geral
		 * @param 
		 * @public
		 */
		ng.carregaGrafico_Item = function () {

			if (ng.materia.objMateria !== undefined) {

			    ReportItemModel.load_reportItem({
			        Id: ng.materia.objMateria.Id,
			        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
			    }, function (result) {

					if (result.success) {

						var reportItem = [];
						reportItem = result.lista;
						ng.reportItem = reportItem;

						var list = ng.reportItem;
						list = $.grep(list, function (val, index) {
						    return val.Value !== 0;
						});

						var labels = [];
						var values = [];
						var colors = [];
						if (list != 0) {
						    for (var key in reportItem) {
						        labels.push(reportItem[key].Description);
						        values.push(reportItem[key].Value);
						        colors.push(ng.graphColors[key]);
						    }
						}
						var data = {
						    datasets: [{
						        data: values,
						        backgroundColor: colors
						    }],
						    labels: labels,
						};
						var canvas = document.getElementById('gfcQuestao');
						var ctx = canvas.getContext("2d");

						if (ng.chart3.chart && ng.chart3.chart.canvas)
						    ng.chart3.chart.destroy();

						ng.chart3 = {
						    visible: true, chart: new Chart(ctx, {
						        type: 'doughnut',
						        data: data,
						        options: {
						            title: {
						                text: 'Total de itens : ' + reportItem[0].Total,
						                display: true,
						                position: 'bottom',
						                padding: -100,
						                fontFamily: 'Roboto',
						                fontSize: 24,
                                        fontColor: '#333333',
                                        fontStyle: 'normal'
						            },
						            rotation: 1 * Math.PI,
						            circumference: 1 * Math.PI
						        }
						    })
						};

						return;
					}
					else {
						ng.reportItem = null;
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};

		/**
		 * @function - Relatório Distribuição por tipo de questão
		 * @param 
		 * @public
		 */
		ng.carregaGrafico_ItemType = function (itemType) {

			if (ng.materia.objMateria !== undefined && itemType !== undefined) {

			    ReportItemModel.load_reportItemType({
			        Id: ng.materia.objMateria.Id,
			        situacao: itemType.Id,
			        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
			    }, function (result) {

					if (result.success) {
						if (ng.materia.objMateria != undefined) {
							var reportItemType = [];
							reportItemType = result.lista;
							ng.reportItemType = reportItemType

							var list = ng.reportItemType;
							list = $.grep(list, function (val, index) {
							    return val.Value !== 0;
							});

							var labels = [];
							var values = [];
							var colors = [];
							if (list != 0) {
							    for (var key in reportItemType) {
							        labels.push([reportItemType[key].Description]);
							        values.push(reportItemType[key].Value);
							        colors.push(ng.graphColors[key]);
							    }
							}

							var data = {
							    datasets: [{
							        data: values,
							        backgroundColor: colors
							    }],
							    labels: labels,
							};
							var canvas = document.getElementById('gfcTipoQuestao');
							var ctx = canvas.getContext("2d");

							if (ng.chart1.chart && ng.chart1.chart.canvas)
							    ng.chart1.chart.destroy();

							ng.chart1 = {
							    visible: true, chart: new Chart(ctx, {
							        type: 'pie',
							        data: data,
							        options: {
							            title: {
							                text: 'Distribuição por tipo de questão',
							                fontFamily: 'Roboto',
							                fontSize: 18,
							                display: true,
							                fontColor: '#333333',
							                fontStyle: 'normal'
							            },
							        }
							    })
							};
						}
						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			};
		};

		/**
		 * @function - Relatório Distribuição por ano
		 * @param 
		 * @public
		 */
		ng.carregaGrafico_ItemCurriculumGrade = function (itemCurriculumGrade) {

			if (ng.materia.objMateria !== undefined && itemCurriculumGrade !== undefined) {

			    ReportItemModel.load_reportItemCurriculumGrade({
			        Id: ng.materia.objMateria.Id,
			        situacao: itemCurriculumGrade.Id,
			        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
			    }, function (result) {

					if (result.success) {
						if (ng.materia.objMateria != undefined) {
							var reportItemCurriculumGrade = [];
							reportItemCurriculumGrade = result.lista;
							ng.reportItemCurriculumGrade = reportItemCurriculumGrade;

							var list = ng.reportItemCurriculumGrade;
							list = $.grep(list, function (val, index) {
								return val.Value !== 0;
							});

							var labels = [];
							var values = [];
							var colors = [];
							if (list != 0) {
							    for (var key in reportItemCurriculumGrade) {
							        labels.push([reportItemCurriculumGrade[key].Description]);
							        values.push(reportItemCurriculumGrade[key].Value);
							        colors.push(ng.graphColors[key]);
							    }
							}

							var data = {
							    datasets: [{
							        data: values,
							        backgroundColor: colors
							    }],
							    labels: labels,
							};
							var canvas = document.getElementById('gfcAno');
							var ctx = canvas.getContext("2d");

							if (ng.chart4.chart && ng.chart4.chart.canvas)
							    ng.chart4.chart.destroy();

							ng.chart4 = {
							    visible: true, chart: new Chart(ctx, {
							        type: 'pie',
							        data: data,
							        options: {
							            title: {
							                text: 'Distribuição por ano',
							                fontFamily: 'Roboto',
                                            fontSize: 18,
                                            display: true,
                                            fontColor: '#333333',
                                            fontStyle: 'normal'
							            },
							        }
							    })
							};
						}
						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			};
		};

		/**
		 * @function - Relatório Distribuição por grau de dificuldade
		 * @param 
		 * @public
		 */
		ng.carregaGrafico_ItemLevel = function (itemLevel) {
			
			if (ng.materia.objMateria !== undefined && itemLevel !== undefined) {

			    ReportItemModel.load_reportItemLevel({
			        Id: ng.materia.objMateria.Id,
			        situacao: itemLevel.Id,
			        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
			    }, function (result) {

					if (result.success) {
						if (ng.materia.objMateria != undefined) {
							var reportItemLevel = [];
							reportItemLevel = result.lista;
							ng.reportItemLevel = reportItemLevel;

							var list = ng.reportItemLevel;
							list = $.grep(list, function (val, index) {
							    return val.Value !== 0;
							});

							var labels = [];
							var values = [];
							var colors = [];
							if (list != 0) {
							    for (var key in reportItemLevel) {
							        labels.push([reportItemLevel[key].Description]);
							        values.push(reportItemLevel[key].Value);
							        colors.push('#66CDAA');
							    }
							}

							var data = {
							    datasets: [{
							        label: 'Total Itens',
							        data: values,
							        backgroundColor: colors
							    }],
							    labels: labels,
							};
							var canvas = document.getElementById('gfcGrauDificuldade');
							var ctx = canvas.getContext("2d");

							if (ng.chart2.chart && ng.chart2.chart.canvas)
							    ng.chart2.chart.destroy();

							canvas.height = 300;

							ng.chart2 = {
							    visible: true, chart: new Chart(ctx, {
							        type: 'horizontalBar',
							        options: {
							            title: {
							                text: 'Distribuição por grau de dificuldade',
							                fontFamily: 'Roboto',
							                fontSize: 18,
							                display: true,
							                fontColor: '#333333',
							                fontStyle: 'normal'
							            },
							            legend: {
							                onClick: function (event, legendItem) { }
							            },
							            scales: {
							                xAxes: [{
							                    ticks: {
							                        min: 0 
							                    }
							                }],
							                yAxes: [{
							                    gridLines: {
							                        display: false
							                    }
							                }]
							            }
							        },
							        data: data
							    })
							};
						}
						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			};
		};


		/**
		 * @function - Relatório Quantitativo consolidado do Banco de itens
		 * @param 
		 * @public
		 */
		ng.carregaGrafico_ItemSituation = function () {
			
			if (ng.materia.objMateria != undefined) {

			    ReportItemModel.load_reportItemSituation({
			        Id: ng.materia.objMateria.Id,
			        inicio: ng.situacao.inicio,
			        fim: ng.situacao.fim,
			        typeLevelEducation: ng.selectedObjTipoNivelEnsino.Id
			    }, function (result) {

			        if (result.success) {
			            if (ng.materia.objMateria != undefined) {
			                var reportItemSituation = [];
			                reportItemSituation = result.lista;
			                ng.reportItemSituation = reportItemSituation;

			                var list = ng.reportItemSituation;
			                list = $.grep(list, function (val, index) {
			                    return val.Value !== 0;
			                });

			                var labels = [];
			                var values = [];
			                var colors = [];
			                if (list != 0) {
			                    for (var key in reportItemSituation){
			                        labels.push([reportItemSituation[key].Description]);
			                        values.push(reportItemSituation[key].Value);
			                        colors.push('#F4A460');
			                    }
			                }
	
			                var data = {
			                    datasets: [{
			                        label: 'Total Itens',
			                        data: values,
			                        backgroundColor: colors
			                    }],
			                    labels: labels
			                };
			                var canvas = document.getElementById('gfcSituacao');
			                var ctx = canvas.getContext("2d");

			                if (ng.chart5.chart && ng.chart5.chart.canvas)
			                    ng.chart5.chart.destroy();

			                canvas.height = 350;

			                ng.chart5 = {
			                    visible: true, chart: new Chart(ctx, {
			                        type: 'bar',
			                        options: {
			                            title: {
			                                text: 'Quantitativo consolidado do Banco de Itens',
			                                fontFamily: 'Roboto',
			                                fontSize: 18,
			                                display: true,
			                                fontColor: '#333333',
			                                fontStyle: 'normal'
			                            },
			                            legend: {
			                                onClick: function (event, legendItem) { }
			                            },
			                            scales: {
			                                yAxes: [{
			                                    ticks: {
			                                        min: 0,
                                                    stepSize: 250
			                                    }
			                                }],
			                                xAxes: [{
			                                    gridLines: {
			                                        display: false
			                                    }
			                                }]
			                            }
			                        },
			                        data: data
			                    })
			                };

			                return;
			            }
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			};
		};

		/**
		 * @function - Relatório de Visão por matriz
		 * @param {Object} result
		 * @public
		 */
		ng.carregaGrafico_ItemSkill = function (result) {
			
			if (ng.materia.objMateria != undefined) {

     			var reportItemSkill = [];
				reportItemSkill = result;
				ng.reportItemSkill = reportItemSkill;

				var list = ng.reportItemSkill;
				list = $.grep(list, function (val, index) {
				    return val.Value !== 0;
				});
				var labels = [];
				var values = [];
				var colors = [];
				if (list != 0) {
				    for (var key in reportItemSkill) {
				        labels.push([reportItemSkill[key].Code]);
				        values.push(reportItemSkill[key].Value);
				        colors.push('#CD5555');
				    }
				}

				var data = {
				    datasets: [{
                        label: 'Total Itens',
				        data: values,
				        backgroundColor: colors
				    }],
				    labels: labels
				};
				var canvas = document.getElementById('gfcMatrizAvaliativa');
				var ctx = canvas.getContext("2d");

				if (ng.chart6.chart && ng.chart6.chart.canvas)
				    ng.chart6.chart.destroy();

				ng.chart6 = {
				    visible: true, chart: new Chart(ctx, {
				        type: 'horizontalBar',
				        options: {
				            title: { 
				                text: 'Visão por matriz avaliativa de disciplina',
				                fontFamily: 'Roboto',
				                fontSize: 18,
				                display: true,
				                fontColor: '#333333',
				                fontStyle: 'normal'
				            },
				            legend: {
				                onClick: function (event, legendItem) { }
				            },
				            scales: {
				                xAxes: [{
				                    ticks: {
				                        min: 0
				                    }
				                }],
				                yAxes: [{
				                    gridLines: {
				                        display: false
				                    }
				                }]
				            }
				        },
				        data: data
				})};
			}
			return;
		};
		
		/**
		 * @function - Minimizador de textos
		 * @param {string} _text
		 * @public
		 */
		ng.minimize = function (_text) {

			if (_text == undefined || _text == null)
				return '';

			if (_text.length > 18)
				_text = _text.substring(0, 35) + "...";

			return _text;
		};

		/**
		 * @function - Controle de accordion para skills
		 * @param {string} _text
		 * @public
		 */
		ng.skillAccordion = function (_index) {

			try {
				ng.skills[_index].show = !ng.skills[_index].show;
			}
			catch (error) {
			}
		};

		/**
		 * @function - Inicio
		 * @param
		 * @public
		 */
		ng.inicio = (function () {
			$("#inicio").datepicker('show');
		});

		/**
		 * @function - Fim
		 * @param
		 * @public
		 */
		ng.fim = (function () {
			$("#fim").datepicker('show');
		});

		ng.downloadChart = (function (graph) {
		    $(graph).get(0).toBlob(function (blob) {
		        saveAs(blob, "grafico.png");
		    });
		});

		ng.activeModal = function __activeModal(label, text) {
		    if (!text) return;
		    ng.textSelected = {
		        Description: label,
		        TextDescription: text
		    };
		    angular.element("#modalDescription").modal({ backdrop: 'static' });
		};
	};


})(angular, jQuery);