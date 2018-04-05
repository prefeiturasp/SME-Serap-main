/**
 * function Controller para cadastro de matriz de avaliação
 * @namespace Controller
 * @author Thiago Macedo Silvestre 02/03/2015
 * @author Alexandre Calil Blasizza Paravani 04/03/2015
 * @author Alexandre Garcia Simõe 09/06/2015
 * @author Luis Henrique Pupo Maron 25/09/2015
 * @author Leticia Langeli Garcia De Goes 28/09/2015
 * @author Everton Luis Ferreira 14/10/2015
 * @author Julio cesar da silva - 18/10/2016
 */
(function (angular, $) {

	'use strict';

	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	angular
		.module('appMain')
		.controller("EvaluationMatrixController", EvaluationMatrixController);

	EvaluationMatrixController.$inject = ['EvaluationMatrixModel', 'ModelEvaluationMatrixModel', 'CurriculumGradeModel', 'CourseModel',
					    'LevelEducationModel', 'DisciplineModel', 'ModelSkillLevelModel', 
						'SkillModel', 'EvaluationMatrixCourseModel', 'ModalityModel', 
						'EvaluationMatrixCourseCurriculumGradeModel', 'CognitiveCompetenceModel', '$scope', 
						'$rootScope', '$location', '$notification', '$pager', '$util'];

	/**
	 * @function Controller listagem de Matrizes de avaliação
	 * @name EvaluationMatrixController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} EvaluationMatrixModel
	 * @param {Object} ModelEvaluationMatrixModel
	 * @param {Object} CurriculumGradeModel
	 * @param {Object} CourseModel
	 * @param {Object} LevelEducationModel
	 * @param {Object} DisciplineModel
	 * @param {Object} ModelSkillLevelModel
	 * @param {Object} SkillModel
	 * @param {Object} EvaluationMatrixCourseModel
	 * @param {Object} ModalityModel
	 * @param {Object} EvaluationMatrixCourseCurriculumGradeModel
	 * @param {Object} CognitiveCompetenceModel
	 * @param {Object} ng
	 * @param {Object} $rootScope
	 * @param {Object} $location
	 * @param {Object} $notification
	 * @param {Object} $pager
	 */
	function EvaluationMatrixController(EvaluationMatrixModel, ModelEvaluationMatrixModel, CurriculumGradeModel, CourseModel, LevelEducationModel, DisciplineModel, ModelSkillLevelModel, SkillModel, EvaluationMatrixCourseModel, ModalityModel, EvaluationMatrixCourseCurriculumGradeModel, CognitiveCompetenceModel, ng, $rootScope, $location, $notification, $pager, $util) {

	    ng.params = $util.getUrlParams();
	    ng.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;
		
		/**
		 * @function - Load
		 * @param
		 * @param
		 * @private
		 */
		function loadController( ) {
			$notification.clear();
			configInternalObjects();
			configInternalNotifications();
			ng.init(ng.params.Id);
		};


		/**
		 * @function - Load
		 * @param
		 * @private
		 */
		function configInternalNotifications() {
			ng.notification = {
				objNotification: undefined,
				alert: function (title, message, closeable, time) {
					this.objNotification = {
						title: title,
						message: message,
						type: 'warning',
						closeable: (closeable == null || closeable == undefined) ? true : closeable,
						time: (time == null || time == undefined) ? 8000 : time,
					}
				},
				success: function (title, message, closeable, time) {
					this.objNotification = {
						title: title,
						message: message,
						type: 'success',
						closeable: (closeable == null || closeable == undefined) ? true : closeable,
						time: (time == null || time == undefined) ? 8000 : time,
					}
				},
				error: function (title, message, closeable, time) {
					this.objNotification = {
						title: title,
						message: message,
						type: 'error',
						closeable: (closeable == null || closeable == undefined) ? true : closeable,
						time: (time == null || time == undefined) ? 8000 : time,
					}
				},
				info: function (title, message, closeable, time) {
					this.objNotification = {
						title: title,
						message: message,
						type: 'info',
						closeable: (closeable == null || closeable == undefined) ? true : closeable,
						time: (time == null || time == undefined) ? 8000 : time,
					}
				},
				clear: function () {
					this.objNotification = undefined;
				}
			}
		};


		/**
		 * @function - Carregamento atualização
		 * @param
		 * @private
		 */
		function configParameters() {

			// Defini váriaveis padrão para quando receber a requisição do server
			function padrao(result) {
				ng.editMode = true;
				if (result.success) {
					ng.evaluationMatrix = result.lista;
					//Popula campos do HTML com os dados sem associar com o cache
					ng.objMatrix.Id = result.lista.Id;
					ng.objMatrix.Description = '' + ng.evaluationMatrix.Description;
					ng.objMatrix.Edition = '' + ng.evaluationMatrix.Edition;
					ng.visualizar = result.lista.Visualizar;
					ng.selectedObjModelMatrix = setValuesComb(ng.modelMatrixList, result.lista.ModelEvaluationMatrix);
					ng.selectedObjLevelEducation = setValuesComb(ng.tipoNivelEnsinoList, result.lista.TypeLevelEducation);
					for (var l = 0; l < ng.situationList.length; l++) {
						if (ng.situationList[l].Description == result.lista.Situacao.Description) {
							ng.selectedObjSituation = ng.situationList[l];
							break;
						}
					}
					ng.Discipline = result.lista.Discipline;
					if (result.lista.EvaluationMatrixCourse.length > 0)
						ng.containsCourse = true;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			}
			if (ng.params.Id && ng.params.navigation) {
				// mostra qual é o nível de navegação que deve exibir !
				ng.navigation = 2;
				EvaluationMatrixModel.loadUpdate({ evaluationMatrixId: ng.params.Id }, function (result) {
						padrao(result);
						ng.carregaLevel(result.lista.ModelEvaluationMatrix.Id);
					});
			}
			else if (ng.params.Id) {
				// mostra qual é o nível de navegação que deve exibir !
				ng.navigation = 1;
				EvaluationMatrixModel.loadUpdate({ evaluationMatrixId: ng.params.Id }, function (result) {
						padrao(result);
					});
			}
		};


		/**
		 * @function - Carregamento atualização
		 * @param
		 * @private
		 */
		function configInternalObjects() {
			ng.editMode = false;
			//Objetos Matriz
			ng.listLevelPanel = true;
			ng.evaluationMatrix = null;
			ng.level = null;
			ng.navigation = 1;
			ng.parent = null;
			ng.containsCourse = false;
			//Obj Situação
			ng.situationList = null;
			//Objeto Matriz
			ng.objMatrix = {
				Id: 0,
				Discipline: undefined,
				ModelEvaluationMatrix: undefined,
				Description: undefined,
				Edition: undefined,
				State: undefined
			};
			//Pesquisa Skills
			ng.paginate = $pager(SkillModel.searchByMatrix);
			ng.pesquisaDescription = '';
			ng.pesquisaCode = '';
			ng.message = false;
			ng.levelId = 0;
			ng.skillList = null;
			ng.skill = null;
			ng.pages = 0;
			ng.totalItens = 0;
			ng.pageSize = 10;
			ng.selectedCognitiveCompetence = null;
			//Skills
			ng.proximoNivel = null;
			ng.skillListModal = [];
			ng.action = null;
			ng.parentId = 0;
			ng.LastLevel = null;
			ng.nextLevel = false;
			ng.selectedCognitiveCompetence = 0;
			ng.ModelEvaluationMatrix = null;
			//Objeto Skill
			ng.objSkill = [];
			ng.nivel = undefined;
			//Objs Course
			ng.evaluationMatrixId = null;
			ng.paginateCourse = $pager(EvaluationMatrixCourseModel.load);
			ng.messageCourse = false;
			ng.typeLevel = null;
			ng.typeLevelDescription = null;
			ng.pagesCourse = 0;
			ng.totalItensCourse = 0;
			ng.pageSizeCourse = 10;
			ng.visualizar = undefined;
			ng.course = null;
			ng.disciplinesList = null;
			ng.courseList = null;
			ng.courseListGrid = null;
			//Objeto Course
			ng.objCurriculumGradesEdit = {
				Id: undefined,
				EvaluationMatrixCourse: undefined,
				CurriculumGradeId: undefined,
				TypeCurriculumGradeId: undefined,
				Ordem: undefined
			};
			//Objeto Course
			ng.objCourse = {
				CourseId: undefined,
				EvaluationMatrix: undefined,
				EvaluationMatrixCurriculumGrade: undefined,
				TypeLevelEducationId: undefined,
				Modality: { Id: '', Description: undefined }
			};
			/*Modais Editar/Adicionar*/
			ng.curriculumGradeListEdit = null;
			ng.curriculumGradeListModal = null;
			ng.curriculumGradeEdit = null;
			ng.curriculumGradeListBackup = null;
			//Máscara para os campos com a classe number. 
			angular.element('.number').on('input', function (event) {
				this.value = this.value.replace(/[^0-9]/g, '');
			});
			ng.description = null;
			ng.parentDescription = null;
			ng.modalityList = null;
		};
			
	   
		/**
		 * @function - Init
		 * @param
		 * @public
		 */
		ng.init = function __init() {
			ng.carregaSituation();
		};


		/**
		 * @function - Carregar situação
		 * @param {Object} result
		 * @public
		 */
		ng.carregaSituation = function __carregaSituation(result) {
		    EvaluationMatrixModel.loadComboSituation(function (result) {
				if (result.success) {
					ng.situationList = result.lista;
					if (ng.visualizar == undefined)
						ng.selectedObjSituation = ng.situationList[0];
						ng.carregaModelMatrix();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


		/**
		 * @function - Carregar Modelo de Matriz
		 * @param {Object} result
		 * @public
		 */
		ng.carregaModelMatrix = function __carregaModelMatrix(result) {
			if (ng.editMode == false) {
			    ModelEvaluationMatrixModel.load(function (result) {
						if (result.success) {
							ng.modelMatrixList = result.lista;
							ng.carregaLevelEducation();
						}
						else {
							$notification[result.type ? result.type : 'error'](result.message);
						}
					});
			}
		};


		/**
		 * @function - Carrega o combo Tipo Nivel de Ensino
		 * @param
		 * @public
		 */
		ng.carregaLevelEducation = function __carregaLevelEducation() {
		    LevelEducationModel.load(function (result) {
				if (result.success) {
					ng.tipoNivelEnsinoList = result.lista;
					ng.$watch('selectedObjLevelEducation', ng.carregaDisciplines);
					configParameters();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


		/**
		 * @function - Confirmar Editar Curso
		 * @param {Object} course
		 * @public
		 */
		ng.confirmarEditCourse = function __confirmarEditCourse(course) {
			ng.course = course
			ng.curriculumGradeListBackup = angular.copy(course.EvaluationMatrixCourseCurriculumGrades);
			CurriculumGradeModel.searchCurriculumGrade({ cur_id: course.CourseId }, function (result) {
				if (result.success) {
					ng.curriculumGradeListModal = result.lista;
					ng.curriculumGradeEdit = { curriculumGradeListModal: [] };
					ng.curriculumGradeListEdit = ng.course.EvaluationMatrixCourseCurriculumGrades;
					for (var a in ng.curriculumGradeListModal) {
						for (var b in ng.curriculumGradeListEdit) {
							if (ng.curriculumGradeListModal[a].Id == ng.curriculumGradeListEdit[b].Id) {
								ng.curriculumGradeListModal[a].Status = ng.curriculumGradeListEdit[b].Status;
								ng.curriculumGradeListModal[a].checked = true;
								ng.curriculumGradeListModal[a].IdBD = ng.curriculumGradeListEdit[b].IdBD;
							}
						}
					}
					ng.notification.clear();
					angular.element('#modalCourse').modal({ backdrop: 'static' });
					return;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function - Confirmar
		 * @param {string} skillDescription
		 * @public
		 */
		ng.removeAndAddPeriod = function (list, option, indice ) {
			if (!option.checked) {
				for (var i = 0; i < list.length; i++){
					if (list[i].Id == option.Id) {
						list.splice(i, 1);
						break;
					}//if
				}
			} else {
				list.push(option);
			}//else
		};


		/**
		 * @function - Confirmar
		 * @param {string} skillDescription
		 * @public
		 */
		ng.confirmar = function __confirmar(skillDescription) {
			$('#alertSkill').remove();
			ng.skillDescription = skill;
			ng.objSkill = [];
			ng.notification.clear();
			angular.element('#modalSkill').modal({ backdrop: 'static' });;
		};


		/**
		 * @function - Confirmar
		 * @param {int} level
		 * @param {Object} skill
		 * @param {string} mode
		 * @param {int} proximoNivel
		 * @param {string} description
		 * @public
		 */
		ng.confirmarModal = function __confirmarModal(level, skill, mode, proximoNivel, description) {
		    clearSkillsLevel();
			if (mode == 'edit') {
				ng.level = level;
				SkillModel.find({ Id: skill }, function (result) {
						ng.skill = result.skill;
						ng.skillListModal = null;
						ng.LastLevel = result.skill.LastLevel;
						if(ng.LastLevel){
							CognitiveCompetenceModel.find(function (result) {
								if (result.success) {
									ng.listCognitiveCompetence = result.lista;
									var arr = jQuery.grep(ng.listCognitiveCompetence, function (n, i) {
										if (ng.skill.CognitiveCompetence != null)
											return (n.Id == ng.skill.CognitiveCompetence.Id);
										else return false;
									});
									ng.skill.CognitiveCompetence = arr[0];
								}
								else {
									$notification[result.type ? result.type : 'error'](result.message);
								}
							});
						}
					});
			}
			else if (mode == 'last') {
				ng.parentId = skill.Id;
				ng.level = proximoNivel;
				ng.levelId = proximoNivel.Id;
				ng.LastLevel = proximoNivel.LastLevel;
				ng.nextLevel = true;
				ng.description = description;
				ng.parent = skill;
				CognitiveCompetenceModel.find(function (result) {
					if (result.success) {
						ng.listCognitiveCompetence = result.lista;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			else if (mode == 'novo') {
				ng.level = level;
				ng.description = null;
			}
			ng.skillListModal = [];
			ng.notification.clear();
			angular.element('#modalSkill').modal({ backdrop: 'static' });
		};

		function clearSkillsLevel() {
		    ng.LastLevel = false;
		    ng.skill = null;
		    ng.action = null;
		};


		/**
		 * @function - Modais Deletar
		 * @param {Object} skill
		 * @param {int} level
		 * @public
		 */
		ng.confirmarDeletarSkill = function __confirmarDeletarSkill(skill, level) {
			ng.skill = skill
			ng.levelId = level.Id;
			ng.parentId = skill.Parent;
			angular.element('#modalSkillDelete').modal({ backdrop: 'static' });
		};


		/**
		 * @function - Deletar Curso
		 * @param {Object} course
		 * @public
		 */
		ng.confirmarDeletarCourse = function __confirmarDeletarCourse(course) {
			ng.course = course
			angular.element('#modalCourseDelete').modal({ backdrop: 'static' });
		};


		/**
		 * @function - Salvar CurriculumGrades
		 * @param {Array} curriculumGradeList
		 * @public
		 */
		ng.saveCurriculumGrades = function __saveCurriculumGrades(curriculumGradeList) {
			ng.curriculumGradeEdit = curriculumGradeList;
			if (ng.verificaCurriculumGrades()) {
				ng.objCurriculumGradesEdit = [];
				var EvaluationMatrixCourse = {
					Id: ng.course.Id
				}
				for (var key in curriculumGradeList) {
					ng.objCurriculumGradesEdit.push({
						Id: curriculumGradeList[key].IdBD,
						EvaluationMatrixCourse: EvaluationMatrixCourse,
						CurriculumGradeId: curriculumGradeList[key].Id,
						TypeCurriculumGradeId: 0,
						Ordem: curriculumGradeList[key].Ordem
					});
				}
				EvaluationMatrixCourseCurriculumGradeModel.save({ evaluationMatrixCourseCurriculumGrades: ng.objCurriculumGradesEdit, evaluationMatrixId: ng.objMatrix.Id, courseId: ng.course.CourseId, typeLevelEducationId: ng.course.TypeLevelId, modalityId: ng.course.Modality.Id }, function (result) {
					if (result.success) {
						$notification.success(result.message);
						ng.notification.clear();
						angular.element('#modalCourse').modal('hide');
						ng.clearPagesCourse();
						ng.loadCourse();
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				}, function (error) {
					$notification[error.type ? error.type : 'error'](error.message);
				});
			}
		};


		/**
		 * @function - Verificar CurriculumGrades
		 * @param
		 * @public
		 */
		ng.verificaCurriculumGrades = function __verificaCurriculumGrades() {
			if (ng.curriculumGradeListEdit.length <= 0) {
			    ng.notification.alert('', 'É necessário selecionar ao menos um ' + ng.curriculumGradeLabel + ' do curso.');
				return false;
			}
			return true;
		};


		/**
		 * @function - Carregar Combo Nivel
		 * @param {int} id
		 * @public
		 */
		ng.carregaLevel = function __carregaLevel(id) {
		    ModelSkillLevelModel.load({ modelEvatuationMatrixId: id }, function (result) {
					if (result.success) {
						ng.nivel = result.lista[0];
						ng.carregaView();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};


		/**
		 * @function - Carregar View
		 * @param
		 * @public
		 */
		ng.carregaView = function __carregaView() {
			ng.listLevelPanel = false;
			var level = ng.nivel.Level + 1;
			if (ng.parentId == 0)
				ng.parentDescription = null;
			ng.clearPaginateSkill();
			var acao = 'next';
			ng.loadSkill(ng.parentId, ng.nivel.Id, acao);
			ModelSkillLevelModel.findLevel({
			    level: level,
			    modelEvatuationMatrixId: ng.selectedObjModelMatrix.Id
			}, function (result) {
				ng.proximoNivel = result.modelSkillLevel;
			});
		};


		/**
		 * @function - Carregar próximo nível
		 * @param {Object} parent
		 * @param {string} parentDescription
		 * @param {int} proximoNivel
		 * @public
		 */
		ng.carregarProximoNivel = function __carregarProximoNivel(parent, parentDescription, proximoNivel) {
			ng.clearPaginateSkill();
			ng.nextLevel = true;
			ng.parent = parent;
			ng.parentId = parent.Id;
			ng.nivel = proximoNivel;
			ng.action = 'last';
			ng.parentDescription = parentDescription;
			ng.carregaView();
		};


		/**
		 * @function - Voltar nível anterior
		 * @param {Object} parent
		 * @public
		 */
		ng.voltarNivelAnterior = function __voltarNivelAnterior(parent) {

			ng.clearPaginateSkill();
			if (ng.parent.ModelSkillLevel.Id == 1)
				ng.parentId = 0;
			var acao = 'back';
			ng.loadSkill(parent.Id, ng.parent.ModelSkillLevel.Id, acao);
			ng.nivel = parent.ModelSkillLevel;
			var level = parent.ModelSkillLevel.Level + 1;
			ModelSkillLevelModel.findLevel({ level: level, modelEvatuationMatrixId: ng.selectedObjModelMatrix.Id }, function (result) {
				ng.proximoNivel = result.modelSkillLevel;
				if (ng.parent.Parent > 0) {
				    SkillModel.findParent({ id: ng.parent.Parent }, function (result) {
						ng.parent = result.skill;
						ng.parentDescription = ng.parent.Code + " - " + ng.parent.Description;
					});
				}
			});
			if (ng.parent.ModelSkillLevel.Level == 1) {
				ng.parentDescription = null;
				ng.nivel.Description = ng.parent.ModelSkillLevel.Description;
			}
		};


		/**
		 * @function - Verificar Cursos
		 * @param
		 * @public
		 */
		ng.verificaCourses = function __verificaCourses() {
			if (!ng.selectedObjModality) {
				$notification.alert('É necessário selecionar uma modalidade.');
				return false;
			}
			if (ng.courseList.length == 0) {
				$notification.alert("", "Não existem cursos para a modalidade selecionada.");
				return false;
			}
			if (!ng.selectedObjCourse) {
				$notification.alert('É necessário selecionar um curso.');
				return false;
			}
			if (ng.curriculumGradeList.length == 0) {
			    $notification.alert("Não existem " + ng.curriculumGradeLabel + "(s) para o curso selecionado.");
				return false;
			}
			if (ng.curriculumGrade.curriculumGradeList) {
				if (ng.curriculumGrade.curriculumGradeList.length <= 0) {
				    $notification.alert('É necessário selecionar ao menos um ' + ng.curriculumGradeLabel + ' do curso.');
					return false;
				}
			}
			return true;
		};


		/**
		 * @function - Salvar Cursos
		 * @param {Object} course
		 * @param {Array} curriculumGradeList
		 * @public
		 */
		ng.salvarCourses = function __salvarCourses(course, curriculumGradeList) {
			if (ng.verificaCourses()) {
				var objCurriculumGrade = [];
				var EvaluationMatrix = {
					Id: ng.objMatrix.Id
				};
				for (var key in curriculumGradeList) {
					objCurriculumGrade.push({
						CurriculumGradeId: curriculumGradeList[key].Id,
						Ordem: curriculumGradeList[key].Ordem,
						TypeCurriculumGradeId: curriculumGradeList[key].TypeCurriculumGradeId
					});
				}
				ng.objCourse.CourseId = course.Id;
				ng.objCourse.EvaluationMatrixCourseCurriculumGrades = objCurriculumGrade;
				ng.objCourse.EvaluationMatrix = EvaluationMatrix;
				ng.objCourse.TypeLevelEducationId = ng.selectedObjLevelEducation.Id;
				ng.objCourse.Modality.Id = ng.selectedObjModality.Id;
				EvaluationMatrixCourseModel.save(ng.objCourse, function (result) {
					if (result.success) {
						$notification.success(result.message);
						ng.objMatrix.Id = EvaluationMatrix.Id;
						ng.objMatrix.EvaluationMatrix = EvaluationMatrix;
						ng.selectedObjCourse = null;
						ng.curriculumGrade.curriculumGradeList = [];
						ng.clearPagesCourse();
						ng.loadCourse();
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};


		/**
		 * @function - Carregar Courses
		 * @param
		 * @public
		 */
		ng.carregarCourses = function __carregarCourses() {
		    ng.navigation = 3;
		    ng.clearPagesCourse();
			ng.loadCourse();
			ng.carregaModality();
			ng.courseList = null;
			ng.selectedObjModality = null;
		};


		/**
		 * @function - Carregar Modality
		 * @param
		 * @public
		 */
		ng.carregaModality = function __carregaModality(result) {
		    ModalityModel.load({},function (result) {
				if (result.success) {
					ng.modalityList = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


		/**
		 * @function - Carregar Courses
		 * @param
		 * @public
		 */
		ng.carregaCourse = function __carregaCourse(result) {
			ng.selectedObjCourse = null;
			ng.curriculumGradeList = null;
			if (ng.selectedObjModality != undefined && ng.selectedObjLevelEducation != undefined) {
			    CourseModel.searchCoursesByLevelModality({
					typeLevelEducation: ng.selectedObjLevelEducation.Id,
					modality: ng.selectedObjModality.Id
			    },
                function (result) {
					if (result.success) {
						ng.courseList = result.lista;
						if (ng.courseList.length == 0)
							$notification.alert("Não existem cursos para a modalidade selecionada.");
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};


		/**
		 * @function - Carregar Curriculum Grade
		 * @param
		 * @public
		 */
		ng.loadCurriculumGrade = function __loadCurriculumGrade() {
			if (ng.selectedObjCourse != undefined) {
			    CurriculumGradeModel.searchCurriculumGrade({ cur_id: ng.selectedObjCourse.Id }, function (result) {
						if (result.success) {
							ng.curriculumGradeList = result.lista;
							if (ng.curriculumGradeList.length == 0)
							    $notification.alert("Não existem " + ng.curriculumGradeLabel + "(s) para o curso selecionado.");
						}
						else {
							$notification[result.type ? result.type : 'error'](result.message);
							ng.curriculumGradeList = [];
						}
					});
				ng.curriculumGrade = { curriculumGradeList: [] };
			}
		};


		/**
		 * @function - Carregar Courses
		 * @param
		 * @public
		 */
		ng.loadCourse = function __loadCourse() {
			ng.paginateCourse.paginate({
			    tipoNivelEnsino: ng.selectedObjLevelEducation.Id,
			    evaluationMatrixId: ng.objMatrix.Id
			}).then(function (result) {
				if (result.success) {
					ng.paginateCourse.nextPage();
					ng.courseListGrid = result.lista;
					if (!ng.pagesCourse > 0) {
						ng.pagesCourse = ng.paginateCourse.totalPages();
						ng.totalItensCourse = ng.paginateCourse.totalItens();
					}
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			}, function () {
				ng.messageCourse = true;
				ng.courseListGrid = null;
			});
		};


		/**
		 * @function - Carregar Courses
		 * @param
		 * @public
		 */
		ng.clearPagesCourse = function __clearPagesCourse() {
		    ng.paginateCourse.indexPage(0);
		    ng.pageSizeCourse = ng.paginateCourse.getPageSize();
			ng.pagesCourse = 0;
			ng.totalItensCourse = 0;
		};


		/**
		 * @function - Deletar Courses
		 * @param {Object} course
		 * @public
		 */
		ng.deleteCourse = function __deleteCourse(course) {
			EvaluationMatrixCourseModel.delete({ Id: course.Id, evaluationMatrixId: ng.objMatrix.Id }, function (result) {
				if (result.success) {
					$notification.success(result.message);
					ng.clearPagesCourse();
					ng.loadCourse();
					ng.courseListGrid.splice(ng.courseListGrid.indexOf(course), 1);
				} else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
			angular.element('#modalCourseDelete').modal('hide');
		};


		/**
		 * @function - Carregar Disciplinas
		 * @param
		 * @public
		 */
		ng.carregaDisciplines = function __carregaDisciplines() {
			if (ng.selectedObjLevelEducation != undefined) {
			    DisciplineModel.searchDisciplinesSaves({ typeLevelEducation: ng.selectedObjLevelEducation.Id }, function (result) {
					if (result.success) {
						ng.disciplinesList = result.lista;
						if (ng.Discipline != undefined)
							ng.selectedObjLevelDisciplines = setValuesComb(ng.disciplinesList, ng.Discipline);
						if (ng.disciplinesList.length == 0)
							$notification.alert("Não existem componente(s) curricular(es) cadastrado(s) para esse nível de ensino.");
					}
					else {   
							$notification[result.type ? result.type : 'error'](result.message);
						}
				});
			}
		};


		/**
		 * @function - Salvar
		 * @param {Object} list - lista de valores que preencherá o combo
		 * @param {Object} opcao - opcao a ser procurada dentro da lista
		 * @public
		 */
		function setValuesComb(list, opcao) {
			for (var k = 0; k < list.length; k++) {
				if (list[k].Description == opcao.Description) {
					return list[k];
				};
			};
		};


		/**
		 * @function - Salvar
		 * @param {Object} evaluationMatrix
		 * @param {Object} curriculumGradeList
		 * @public
		 */
		ng.salvar = function __salvar() {
			if (ng.verifica()) {
				var Discipline = { Id: ng.selectedObjLevelDisciplines.Id };
				var ModelEvaluationMatrix = { Id: ng.selectedObjModelMatrix.Id };
				ng.objMatrix.Discipline = Discipline;
				ng.objMatrix.ModelEvaluationMatrix = ModelEvaluationMatrix;
				if (!ng.evaluationMatrix)
					ng.evaluationMatrix = {};
				ng.evaluationMatrix.Description = ng.objMatrix.Description;
				ng.evaluationMatrix.Edition = ng.objMatrix.Edition;
				ng.objMatrix.State = ng.selectedObjSituation.Id;
				EvaluationMatrixModel.save(ng.objMatrix, function (result) {
					if (result.success) {
						ng.navigation = 2;
						ng.objMatrix.Id = result.Id;
						ng.parentId = 0;
						ng.carregaLevel(ng.selectedObjModelMatrix.Id);
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};


		/**
		 * @function - Verificar
		 * @param
		 * @public
		 */
		ng.verifica = function __verifica() {
			if (ng.objMatrix) {
				if (!ng.objMatrix.Description) {
					$notification.alert('O campo "Nome da Matriz" é obrigatório.');
					angular.element('#description').focus();
					return false;
				}
				if (!ng.objMatrix.Edition) {
					$notification.alert('O campo "Edição da Matriz" é obrigatório.');
					angular.element('#edition').focus();
					return false;
				}
			}
			else {
				$notification.alert('O campo "Nome da Matriz" é obrigatório.');
				angular.element('#description').focus();
				return false;
			}
			if (ng.selectedObjModelMatrix == undefined) {
				$notification.alert('É necessário selecionar o modelo da matriz.');
				return false;
			}
			if (ng.selectedObjLevelEducation == undefined) {
				$notification.alert('É necessário selecionar um nível de ensino.');
				return false;
			}
			if (ng.selectedObjLevelDisciplines == undefined) {
				$notification.alert('É necessário selecionar um componente curricular.');
				return false;
			}
			return true;
		};


		/**
		 * @function - Adicionar Skill Modal
		 * @param {Object} skill
		 * @public
		 */
		ng.adicionarSkillModal = function __adicionarSkillModal(skill) {
			if (ng.verificaSkill()) {
				ng.skillListModal.push({ Description: skill.Description, Code: skill.Code.toUpperCase(), ItemAdicionado: (skill.Code.toUpperCase() + " - " + skill.Description), CognitiveCompetence : skill.CognitiveCompetence })
				skill.Description = null;
				skill.Code = null;
				skill.CognitiveCompetence = null;
			}
		};


		/**
		 * @function - Remover Skill Modal
		 * @param {int} index
		 * @public
		 */
		ng.removerSkillModal = function __removerSkillModal(index) {
			ng.skillListModal.splice(index, 1);
		};


		/**
		 * @function - Carregar Skill
		 * @param {int} parentId
		 * @param {int} levelId
		 * @param {string} acao
		 * @public
		 */
		ng.loadSkill = function __loadSkill(parentId, levelId, acao) {
			ng.paginate.paginate({ evaluationMatrixId: ng.objMatrix.Id, modelSkillLevelId: levelId, parentId: parentId, acao: acao }).then(function (result) {
				if (result.lista.length > 0) {
					ng.paginate.nextPage();
					ng.skillList = result.lista;
					if (!ng.pages > 0) {
						ng.pages = ng.paginate.totalPages();
						ng.totalItens = ng.paginate.totalItens();
					}
					if (ng.nextLevel) {
						ng.nextLevel = false;
					}
				} else {
					ng.parentId = 0;
					ng.message = true;
					ng.skillList = null;
				}
			}, function () {
				ng.message = true;
				ng.skillList = null;
			});
		};


		/**
		 * @function - Remover Skill Modal
		 * @param {Object} skill
		 * @public
		 */
		ng.deleteSkill = function __deleteSkill(skill) {
			SkillModel.deleteByMatrix({ Id: skill.Id, evaluationMatrixId: ng.objMatrix.Id },
				function (result) {
					if (result.success) {
						ng.clearPaginateSkill();
						var acao = 'next';
						ng.loadSkill(ng.parentId, ng.levelId, acao);
						$notification.success(result.message);
						ng.skillList.splice(ng.skillList.indexOf(skill), 1);
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			angular.element('#modalSkillDelete').modal('hide');
		};


		/**
		 * @function - Remover Skill Modal
		 * @param
		 * @public
		 */
		ng.copySearch = function __copySearch() {
		    ng.paginate.indexPage(0);
		    ng.pageSize = ng.paginate.getPageSize();
			ng.pages = 0;
			ng.totalItens = 0;
		};


		/**
		 * @function - Salvar Skill na Edição
		 * @param {Object} skill
		 * @public
		 */
		ng.salvarSkill = function __salvarSkill(skill) {
			if (ng.verificaSkill()) {
				var EvaluationMatrix = {
					Id: ng.objMatrix.Id
				};
				var ModelSkillLevel = {
					Id: ng.nivel.Id
				}
				var Parent = {
					Id: ng.parentId
				};
				var CognitiveCompetence = {
					Id: ng.skill.CognitiveCompetence != null ? ng.skill.CognitiveCompetence.Id : 0
				};
				ng.objSkill = {
					Id: skill.Id,
					Description: skill.Description,
					Code: skill.Code.toUpperCase(),
					Parent: Parent,
					LastLevel: ng.LastLevel,
					EvaluationMatrix: EvaluationMatrix,
					ModelSkillLevel: ModelSkillLevel,
					CognitiveCompetence: CognitiveCompetence
				};
				SkillModel.save(ng.objSkill, function (result) {
					if (result.success) {
						if (result.type == "Update")
							$notification.success(ng.level.Description + " alterado(a) com sucesso.");
						ng.notification.clear();
						angular.element('#modalSkill').modal('hide');
						ng.clearPaginateSkill();
						var acao = 'next';
						ng.loadSkill(ng.parentId, ng.level.Id, acao);
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};


		/**
		 * @function - Salvar Lista de Skills
		 * @param {Array} listSkill
		 * @public
		 */
		ng.salvarSkills = function __salvarSkills(listSkill) {

		    ng.objSkill = [];
			var EvaluationMatrix = {
				Id: ng.objMatrix.Id
			};
			var ModelSkillLevel = {
				Id: ng.level.Id
			};
			var Parent = {
				Id: ng.parentId
			};
			for (var i in listSkill) {
				ng.objSkill.push({
					Description: listSkill[i].Description,
					Code: listSkill[i].Code,
					Parent: Parent,
					LastLevel: ng.level.LastLevel,
					EvaluationMatrix: EvaluationMatrix,
					ModelSkillLevel: ModelSkillLevel,
					CognitiveCompetence: listSkill[i].CognitiveCompetence
				});
			}
			SkillModel.saveRange(ng.objSkill, function (result) {
				if (result.success) {
					if (result.type == "Save") {
						$notification.success(ng.level.Description + "(s) salvo(s) com sucesso.");
						ng.nivel = ng.proximoNivel;
						ng.parentDescription = ng.description;
						ng.notification.clear();
						angular.element('#modalSkill').modal('hide');
					}
					ng.clearPaginateSkill();
					ng.nivel = ng.level;
					ng.carregaView();
					ng.skillListModal = null;
				}
				else {

					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};


		/**
		 * @function - Clear Paginate Skills
		 * @param
		 * @public
		 */
		ng.clearPaginateSkill = function __clearPaginateSkill() {
			ng.paginate.indexPage(0);
			ng.pages = 0;
			ng.totalItens = 0;
			ng.pesquisaDescription = '';
			ng.pageSize = ng.paginate.getPageSize();
			ng.pesquisaCode = '';
		};


		/**
		 * @function - Verificar Skills
		 * @param
		 * @public
		 */
		ng.verificaSkill = function __verificaSkill() {
			if (ng.skill) {
				if (!ng.skill.Code) {
					ng.notification.alert('', 'O campo "Código do(a) ' + ng.level.Description + '" é obrigatório.');
					return false;
				}
				if (!ng.skill.Description) {
					ng.notification.alert('', 'O campo "Nome do(a) ' + ng.level.Description + '" é obrigatório.');
					return false;
				} else {
					for (var a in ng.skillListModal) {
						if (angular.equals(ng.skillListModal[a].Code.toUpperCase(), ng.skill.Code.toUpperCase())) {
							ng.notification.alert('', 'Já existe um código com essa descrição.');
							return false;
						}
					}
				}
			}
			else {
				ng.notification.alert('', 'O campo "Nome do(a) ' + ng.level.Description + '" é obrigatório.');
				return false;
			}
			return true;
		};


		/**
		 * @function - Minimizador de textos
		 * @param
		 * @public
		 */
		ng.minimize = function __minimize(_text) {

		    if (_text === null || _text === undefined)
		        return "";

			if (_text.length > 100)
				_text = _text.substring(0, 100) + "...";
			return _text;
		};


		/**
		 * @function - Cancelar
		 * @param
		 * @public
		 */
		ng.cancelar = function __cancelar() {
			ng.course.EvaluationMatrixCourseCurriculumGrades = ng.curriculumGradeListBackup;
			ng.notification.clear();
			angular.element('#modalCourse').modal('hide');
		};


		/**
		 * @function - previus
		 * @param {int} _navigation
		 * @public
		 */
		ng.previus = function __previus(_navigation) {
			ng.navigation = _navigation;
			if (_navigation == 2) ng.curriculumGradeList = [];
		};


		/**
		 * @function - next
		 * @param {int} _navigation
		 * @public
		 */
		ng.next = function __next(_navigation) {
			ng.navigation = _navigation;
		};

		ng.activeModal = function __activeModal(label, text) {
		    if (!text) return;
		    ng.textSelected = {
		        Description: label,
		        TextDescription: text
		    };
		    angular.element("#modalDescription").modal({ backdrop: 'static' });
		};

	    //inicialização
		loadController();

	};

})(angular, jQuery);