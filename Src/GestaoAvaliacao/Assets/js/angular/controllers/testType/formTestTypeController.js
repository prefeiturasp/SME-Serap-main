/**
 * @function Cadastro/Edição tipo de prova
 * @namespace Controller
 * @author Alexandre Calil Blasizza Paravani - 01/03/2015
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

	'use strict';

	angular
        .module('appMain', ['services', 'filters', 'directives']);

	angular
        .module('appMain')
        .controller("FormTestTypeController", FormTestTypeController);

	FormTestTypeController.$inject = ['TestTypeModel', 'FormatTypeModel', 'AnswerSheetModel', 'ItemLevelModel', 'TestTypeItemLevelModel', 'CurriculumGradeModel', 'CourseModel', 'LevelEducationModel', 'ModalityModel', 'TestTypeCourseModel', 'TestTypeCourseCurriculumGradeModel', 'ItemTypeModel', 'ModelTestModel', '$pager', '$scope', '$rootScope', '$window', '$notification', '$util'];

	function FormTestTypeController(TestTypeModel, FormatTypeModel, AnswerSheetModel, ItemLevelModel, TestTypeItemLevelModel, CurriculumGradeModel, CourseModel, LevelEducationModel, ModalityModel, TestTypeCourseModel, TestTypeCourseCurriculumGradeModel, ItemTypeModel, ModelTestModel, $pager, ng, $rootScope, $window, $notification, $util) {

	    ng.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;

	    ng.params = $util.getUrlParams();

	    var flagEdit = false;
		var	auxEdit = {};

		function Init() {
			
			$notification.clear();
			configInternalObjects();

			if (ng.params.Id) {
			    ng.testTypeForEdit = ng.params.Id;
				getExistsTestAssociated();
			}

			ng.carregaLevelEducation();       
			ng.carregaItemType();
		};

		function getExistsTestAssociated() {

			TestTypeModel.existsTestAssociated({ Id: ng.testTypeForEdit },

				function (result) {
					if (result.success) {
						
						ng.typItemBlock = result.block;
						ng.typItemBlockAplication = result.block;

					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};
		
		function configInternalObjects() {

		    ng.frequencyApplication = null;
		    ng.frequencyApplicationList = [];
			ng.testType = undefined;
			ng.testType = { TestTypeItemLevel: undefined };
			ng.TestTypeCourses = { CourseId: '', TestTypeCourseCurriculumGrades: [] };
			ng.TestTypeCoursesList = [];
			ng.TestTypeCoursesListUpdate = [];
			ng.tab = 1;
			ng.AnswerSheet = { id: '' };
			ng.FormatType = { id: '' };
			ng.ItemType = { id: '' };
			ng.navigation = 1;
			ng.testTypeId = 0;
			ng.itensLevel = null;
			ng.Modality = { id: '', Description: undefined };
			ng.TypeLevelEducation = { id: '', Description: undefined };
			ng.avancarCourseDisabled = ng.testTypeId <= 0;
			ng.typItemBlock = false;
			ng.typItemBlockAplication = false;
			ng.globalList = [
				{
					Id: 1,
					Description: getParameterValue(parameterKeys[0].GLOBAL_TERM)
				},
				{
					Id: 2,
					Description: getParameterValue(parameterKeys[0].LOCAL_TERM)
				}]
			ng.Global = ng.globalList[0];
			angular.element('.number').on('input', function (event) {
				this.value = this.value.replace(/[^0-9]/g, '');
			});
			ng.testTypeId = null;
			ng.paginateCourse = $pager(TestTypeCourseModel.load);
			ng.pageSizeCourse = 10;
			ng.messageCourse = false;
			ng.typeLevel = null;
			ng.typeLevelDescription = null;
			ng.pagesCourse = 0;
			ng.totalItensCourse = 0;
			ng.visualizar = undefined;
			ng.course = null;
			ng.curriculumGradeListBackup = [];
			ng.courseList = null;
			ng.courseListGrid = null;
			ng.modelTestList = [];
			ng.Deficiencies = [];
			ng.DeficienciesSelected = [];
			ng.objCurriculumGradesEdit = {
				Id: undefined,
				TestTypeCourse: undefined,
				CurriculumGradeId: undefined,
				TypeCurriculumGradeId: undefined,
				Ordem: undefined
			};
			ng.objCourse = {
				CourseId: undefined,
				TestType: undefined,
				TestTypeCurriculumGrade: undefined,
                Modality: undefined
			};

			ng.direcionadoParaAlunosDeficientesOptions = [
				{
					Id: true,
					Description: 'Sim'
				},
				{
					Id: false,
					Description: 'Não'
				}
			];
			ng.direcionadoParaAlunosDeficientesOptionSelected = ng.direcionadoParaAlunosDeficientesOptions[1];

			ng.loadFrenquencyApplication();
			ng.loadDeficiencies();
		};

		ng.carregaUpdate = function carregaUpdate() {
			if (ng.testTypeForEdit != undefined)
				TestTypeModel.find({ Id: ng.testTypeForEdit }, TestTypeModel_Find);
			else if (ng.testTypeId <= 0) 
				for (var a in ng.itensLevel) 
					ng.itensLevel[a].Value = undefined;
			function TestTypeModel_Find(result) {
				if (!result.success) return;
				ng.testType = result.testType;
				ng.testTypeId = ng.testType.Id;
				ng.avancarCourseDisabled = ng.testTypeId <= 0;
				ng.frequencyApplication = result.testType.FrequencyApplication;
				ng.selectedObjLevelEducation = setSelectedOption(ng.testType.TypeLevelEducation , ng.tipoNivelEnsinoList);
				ng.selectedObjFormatType = setSelectedOption(ng.testType.FormatType, ng.formatTypeList);
				ng.selectedObjItemType = setSelectedOption(ng.testType.ItemType, ng.itemTypeList);
				ng.selectedModelTest = setSelectedOptionById(ng.testType.ModelTest_Id, ng.modelTestList);
				ng.selectedObjGlobal = ng.globalList[ng.testType.Global ? 0 : 1];
				if (ng.testType.TestTypeItemLevel.length > 0) {
					var a, b, l = ng.itensLevel.length, m = ng.testType.TestTypeItemLevel ? ng.testType.TestTypeItemLevel.length : 0;
					for (b = 0; b < m; b++) {
						for (a = 0; a < l; a++) {
							if (ng.itensLevel[a].Id === ng.testType.TestTypeItemLevel[b].IdItem) {
								ng.itensLevel[a].Value = ng.testType.TestTypeItemLevel[b].Value;
								if (ng.testType.Id > 0)
									ng.itensLevel[a].IdBD = ng.testType.TestTypeItemLevel[b].Id;
								break;
							}
						}
					}
				}

				ng.DeficienciesSelected = result.testType.DeficienciesSelected;
				ng.direcionadoParaAlunosDeficientesOptionSelected = ng.direcionadoParaAlunosDeficientesOptions.find(x => x.Id == result.testType.TargetToStudentsWithDeficiencies);

				flagEdit = true;
				cloneModels();
			};
		};

		ng.direcionadoParaAlunosDeficientesOptionChange = direcionadoParaAlunosDeficientesOptionChange;
		function direcionadoParaAlunosDeficientesOptionChange(optionSelected) {
			if (optionSelected.Id == true) {
				angular.element("#divDeficiencies").show();
			}
			else {
				angular.element("#divDeficiencies").hide();
            }
		};

		ng.modalActive = function _modalActive() {
			angular.element("#modaTypeItem").modal({ backdrop: 'static' });
		};

		ng.ablilityElement = function _ablilityElement() {
			angular.element('#modaTypeItem').modal('hide');
			ng.typItemBlock = false;
		};

		function setSelectedOption(option, list) {
			if (list) {
				for (var i = 0; i < list.length; i++)
					if (option)
						if (list)
							if (list[i].Id == option.Id) return list[i];
			} else {
				return [];
			}
		};

		function setSelectedOptionById(id, list) {

		    for (var i = 0; i < list.length; i++)
		        if (list[i].Id == id) return list[i];
		};

		ng.loadDeficiencies = function _loadDeficiencies() {
			debugger;
			TestTypeModel.getDeficiencies({}, function (result) {
				if (result.success) {
					ng.Deficiencies = result.Deficiencies;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.loadFrenquencyApplication = function __loadFrenquencyApplication() {
		    TestTypeModel.getFrequencyApplicationParentList({}, function(result) {
		        if (result.success) {
		            ng.frequencyApplicationList = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		ng.carregaFormatType = function carregaFormatType(result) {
			FormatTypeModel.load(function (result) {
				if (result.success) {
					ng.formatTypeList = result.lista;
					ng.carregaModelTest();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.carregaItemLevel = function carregaItemLevel(result) {
		    ItemLevelModel.loadLevels({}, function(result) {
				if (result.success) {
					ng.itensLevel = result.lista;
					ng.carregaFormatType();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.carregaModelTest = function () {
		    ModelTestModel.findSimple({}, function (result) {
		        if (result.success) {
		            ng.modelTestList = result.lista;
		            ng.carregaUpdate();
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		ng.adicionar = function adicionar() {

		    var auxTestList = { CourseId: '', Modality: '', Description: '', TestTypeCourseCurriculumGrades: [] };

		    for (var key in ng.curriculumGradeList) {

		        if (ng.curriculumGradeList[key].Checked) {

		            auxTestList.TestTypeCourseCurriculumGrades.push({
		                CurriculumGradeId: ng.curriculumGradeList[key].Id,
		                Description: ng.curriculumGradeList[key].Description
		            });
		        }
		    }

		    auxTestList.CourseId = ng.selectedObjCourse.Id;
		    auxTestList.Modality = ng.selectedObjCourse.Modality.Description;
		    auxTestList.Description = ng.selectedObjCourse.Description;

		    ng.TestTypeCoursesList.push(auxTestList);
		};

		ng.salvar = function salvar() {
			if (!ng.verifica()) return;
			flagEdit = true;
			cloneModels();
			ng.testType.AnswerSheet = ng.AnswerSheet;
			ng.testType.frequencyApplication = ng.frequencyApplication;
			ng.Global = ng.selectedObjGlobal.Id == 1;
			ng.testType.Global = ng.Global;
			ng.testType.TypeLevelEducationId = ng.selectedObjLevelEducation.Id;
			var b, listTestTypeItemLevel = [];
			for (var key in ng.itensLevel) {
				if (ng.itensLevel[key].Value) {
					listTestTypeItemLevel.push({
						Id: ng.itensLevel[key].IdBD,
						TestType: { Id: ng.testType.Id },
						ItemLevel: { Id: ng.itensLevel[key].Id },
						Value: ng.itensLevel[key].Value
					});
				}
			}

			var obj = {
				Id: ng.testType.Id,
				Description: ng.testType.Description,
				CourseId: ng.testType.CourseId,
				FormatType: ng.selectedObjFormatType ? { Id: ng.selectedObjFormatType.Id } : undefined,
				AnswerSheet: { Id: ng.testType.AnswerSheet.Id },
				ItemType: ng.selectedObjItemType ? { Id: ng.selectedObjItemType.Id } : undefined,
				FrequencyApplication: ng.testType.frequencyApplication || ng.frequencyApplication,
				Global: ng.testType.Global,
				TestTypeItemLevel: listTestTypeItemLevel,
				TypeLevelEducationId: ng.testType.TypeLevelEducationId,
				ModelTest_Id: ng.selectedModelTest.Id,
				TargetToStudentsWithDeficiencies: ng.direcionadoParaAlunosDeficientesOptionSelected.Id,
				TestTypeDeficiencies: GetSelectedDeficienciesToSave(),
			};
			TestTypeModel.save(obj, function (result) {
			    if (!result.success) {
			        $notification[result.type ? result.type : 'error'](result.message);
			        return;
			    }
			    ng.testTypeId = result.testTypeID;
			    ng.testType.Id = ng.testTypeId;
			    var q = 0, w = 0, e = 0, r = 0;
			    for (q ; q < ng.itensLevel.length; q++) {
			        w = ng.itensLevel[q];

			        for (e = 0 ; e < result.testTypeItemLevel.length; e++) {
			            r = result.testTypeItemLevel[e];
			            if (parseInt(w.Id) === r.IdItem) {
			                w.IdBD = r.Id;
			            }
			        }
			    }
			    auxEdit.itensLevel = angular.copy(ng.itensLevel);
			    ng.avancarCourseDisabled = false;
			    if (result.type !== "Save")
			        $notification.success("Tipo de prova alterado com sucesso.");
			    else 
			        $notification.success("Tipo de prova salvo com sucesso.");
			});
			
		};

		function GetSelectedDeficienciesToSave() {
			if (ng.direcionadoParaAlunosDeficientesOptionSelected.Id == false) return null;
			var result = [];
			var index = 0;

			debugger;
			for (index; index < ng.DeficienciesSelected.length; index++) {
				result.push({
					DeficiencyId: ng.DeficienciesSelected[index].Id,
					TestType: { Id: ng.testType.Id }
				});
			}

			return result;
        }

		ng.verifica = function verifica() {

			if (ng.testType) {

				if (!ng.testType.Description) {

					$notification.alert('O campo "Descrição" é obrigatório.');

					angular.element('#description').focus();

					return false;
				}
				if (!ng.selectedObjLevelEducation) {

					$notification.alert('O campo "Nível de ensino" é obrigatório.');

					return false;
				}
				
				if (ng.selectedModelTest == undefined) {

				    $notification.alert('O campo "Modelo de prova" é obrigatório.');

				    angular.element('#bib').focus();

				    return false;
				}

				if (ng.selectedObjGlobal == undefined) {

					$notification.alert('O campo "Aplicação" é obrigatório.');

					angular.element('#global').focus();

					return false;
				}
				var total = 0;

				for (var a in ng.itensLevel) {

					if (ng.itensLevel[a].Value != undefined) {
						total += parseInt(ng.itensLevel[a].Value == '' ? 0 : ng.itensLevel[a].Value);
					}
				}

				if ((total > 0) && (ng.selectedObjFormatType == undefined)) {

					$notification.alert('Selecione o "Formato".');
					angular.element('#formatyType').focus();

					return false;
				}

				if (ng.selectedObjFormatType != undefined) {
					if (total == 0) {
						$notification.alert('É necessário preencher ao menos um "Item por dificuldade".');
						return false;
					}

					if (ng.selectedObjFormatType.Description === "Porcentagem") {
						if (total == 100) {
							return true;
						}
						else {
							$notification.alert('A somatória dos valores deve atingir 100%.');
							return false;
						}
					}

					return true;
				}

			} else {

				$notification.alert('O campo "Descrição" é obrigatório.');
				angular.element('#description').focus();
			}

			return true;
		};

		ng.limpar = function limpar() {

		    if (!ng.itensLevel)
		        if (!ng.testType)
		            return;

		    var a, b, l = ng.itensLevel.length, m = ng.testType.TestTypeItemLevel ? ng.testType.TestTypeItemLevel.length : 0;

		    for (a = 0; a < l; a++) {
		        ng.itensLevel[a].Value = undefined;
		    }

		    for (a = 0; a < l; a++) {
		        for (b = 0; b < m; b++) {
		            if (ng.itensLevel[a].Id == ng.testType.TestTypeItemLevel[b].IdItem) {

		                ng.itensLevel[a].Value = undefined;
		                ng.testType.TestTypeItemLevel[b].Value = undefined;;

		                if (ng.testType.Id > 0) {
		                    ng.itensLevel[a].IdBD = undefined;
		                    ng.testType.TestTypeItemLevel[b].Id = undefined;
		                }
		            }
		        }
		    }

		    ng.itensLevel = undefined;
		    ng.testType = undefined;
		};

		ng.finalizar = function finalizar() {
		    ng.limpar();
		    $window.location.href = '/TestType/List';
		};

		ng.voltar = function voltar() {
		    ng.limpar();
		    $window.location.href = '/TestType/List';
		};

		ng.replaceValue = function replaceValue(_item) {

			var index = ng.itensLevel.indexOf(_item);
			ng.itensLevel[index].Value = ng.itensLevel[index].Value.replace(/[^0-9]/g, '');
		};

		ng.previus = function previus(_navigation) {
			ng.carregaUpdate();
			ng.navigation = _navigation;
		};

		ng.next = function next(_navigation) {
			ng.navigation = _navigation;
		};

		ng.nivelEnsinoModalidadeMudou = function nivelEnsinoModalidadeMudou() {

		    if ((ng.testType.Disabled != undefined) && (!ng.testType.Disabled) && (ng.testTypeForEdit != undefined)) {

		        ng.avancarCourseDisabled = false;

		        if ((ng.selectedObjLevelEducation.Id != ng.testType.TypeLevelEducation.Id)) {
		            ng.avancarCourseDisabled = true;
		        }
		    }
		};

		ng.carregaLevelEducation = function carregaLevelEducation() {
			ng.courseList = null;
			ng.selectedObjModality = null;
			LevelEducationModel.load({}, function(result) {
				if (result.success) {
					ng.tipoNivelEnsinoList = result.lista;
					ng.carregaModality();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.carregaModality = function carregaModality(result) {
			ng.courseList = null;
			ng.selectedObjModality = null;
			ModalityModel.load(function(result) {
				if (result.success) {
					ng.modalityList = result.lista;
					ng.carregaItemLevel();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.carregaItemType = function carregaItemType(result) {
		    ItemTypeModel.loadTestType({}, function (result) {
				if (result.success) {
					debugger;
					ng.itemTypeList = result.lista;
					try {
						for (var i = 0; i < ng.itemTypeList.length; i++) {
							if (ng.itemTypeList[i].IsDefault) {
								ng.selectedObjItemType = ng.itemTypeList[i];
								break;
							}
						}
					} catch (error) { }
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		ng.carregaCourse = function carregaCourse(result) {
			ng.selectedObjCourse = null;
			ng.curriculumGradeList = null;
			if (!ng.selectedObjModality || !ng.selectedObjLevelEducation) return;
			var selectedString = 'CourseModel.searchCoursesByLevelModality:id=' + ng.selectedObjLevelEducation.Id + 'modality:id=' + ng.selectedObjModality.Id
			var selectedIDs = {
				typeLevelEducation: ng.selectedObjLevelEducation.Id,
				modality: ng.selectedObjModality.Id
			};
			CourseModel.searchCoursesByLevelModality(selectedIDs, function(result) {
				if (!result.success) {
					$notification[result.type ? result.type : 'error'](result.message);
					return;
				}
				ng.courseList = result.lista;
				if (ng.courseList.length == 0)
					$notification.alert("", "Não existem cursos para a modalidade e nível de ensino selecionados.");
			});
		};

		ng.loadCurriculumGrade = function () {
			if (ng.selectedObjCourse != undefined) {
				CurriculumGradeModel.searchCurriculumGrade({ cur_id: ng.selectedObjCourse.Id }, function (result) {
						if (result.success) {
							ng.curriculumGradeList = result.lista;
							if (ng.curriculumGradeList.length == 0)
							    $notification.alert("Não existem " + ng.curriculumGradeLabel + "(s) para o curso selecionado.");

							for (var a in ng.curriculumGradeListModal) {
							    for (var b in ng.curriculumGradeListEdit) {
							        if (ng.curriculumGradeListModal[a].Id == ng.curriculumGradeListEdit[b].Id) {
							            ng.curriculumGradeListModal[a].Status = ng.curriculumGradeListEdit[b].Status;
							            ng.curriculumGradeListModal[a].checked = true;
							            ng.curriculumGradeListModal[a].IdBD = ng.curriculumGradeListEdit[b].IdBD;
							        }
							    }
							}
						}
						else {
							$notification[result.type ? result.type : 'error'](result.message);
						}
					});

				ng.curriculumGrade = { curriculumGradeList: [] };
			}
		};

		ng.loadCourse = function () {
            ng.pageSizeCourse = ng.paginateCourse.getPageSize();
		    ng.paginateCourse.paginate({
		        testTypeId: ng.testTypeId,
		        TypeLevelEducationId: ng.selectedObjLevelEducation.Id
		    }).then(function (result) {
		        ng.testType.Disabled = false;
		        if (result.success) {
		            if (result.lista.length > 0) {
		                ng.courseListGrid = result.lista;
		                if (!ng.pagesCourse > 0) {
		                    ng.pagesCourse = ng.paginateCourse.totalPages();
		                    ng.totalItensCourse = ng.paginateCourse.totalItens();
		                }
		                ng.testType.Disabled = true;
		            }
		            else {
		                ng.messageCourse = true;
		                ng.courseListGrid = null;
		            }
		        } else {
		            ng.messageCourse = true;
		            ng.courseListGrid = null;
		        }

		    }, function () {
		        ng.messageCourse = true;
		        ng.courseListGrid = null;
		    });

		};

		ng.clearPagesCourse = function () {
			ng.paginateCourse.indexPage(0);
			ng.pagesCourse = 0;
			ng.totalItensCourse = 0;
			ng.pageSizeCourse = ng.paginateCourse.getPageSize();
		};

		ng.deleteCourse = function (course) {
			TestTypeCourseModel.delete({ Id: course.Id, testTypeId: ng.testTypeId }, function (result) {
				if (result.success) {
					$notification.success(result.message);
					ng.clearPagesCourse();
					ng.loadCourse();
					ng.courseListGrid.splice(ng.courseListGrid.indexOf(course), 1);
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});

			angular.element('#modalCourseDelete').modal('hide');
		};

		ng.verificaCourses = function () {

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

		ng.avancarCourse = function () {
			if (cloneModelsValidation()) {
			    ng.navigation = 2;
			    ng.carregaModality();
				ng.loadCourse();
			}
		};

		function cloneModels() {
			auxEdit.Description = angular.copy(ng.testType.Description);
			auxEdit.selectedObjLevelEducation = angular.copy(ng.selectedObjLevelEducation);
			auxEdit.frequencyApplication = angular.copy(ng.frequencyApplication);
			auxEdit.selectedObjGlobal = angular.copy(ng.selectedObjGlobal);
			auxEdit.selectedObjFormatType = angular.copy(ng.selectedObjFormatType);
			auxEdit.selectedObjItemType = angular.copy(ng.selectedObjItemType);
			auxEdit.itensLevel = angular.copy(ng.itensLevel);
		};

		function cloneModelsValidation() {
			if (JSON.stringify(auxEdit.Description) != JSON.stringify(angular.copy(ng.testType.Description)) ||
				JSON.stringify(auxEdit.selectedObjLevelEducation) != JSON.stringify(angular.copy(ng.selectedObjLevelEducation)) ||
				JSON.stringify(auxEdit.frequencyApplication) != JSON.stringify(angular.copy(ng.frequencyApplication)) ||
				JSON.stringify(auxEdit.selectedObjGlobal) != JSON.stringify(angular.copy(ng.selectedObjGlobal)) ||
				JSON.stringify(auxEdit.selectedObjFormatType) != JSON.stringify(angular.copy(ng.selectedObjFormatType)) ||
				JSON.stringify(auxEdit.selectedObjItemType) != JSON.stringify(angular.copy(ng.selectedObjItemType)) ||
				JSON.stringify(auxEdit.itensLevel) != JSON.stringify(angular.copy(ng.itensLevel)))
				$notification.alert('Alguns dados foram alterados. Antes de avançar é necessário salvar as modificações.');
			else return true;
		};

		ng.salvarCourses = function (course, curriculumGradeList) {

			if (ng.verificaCourses()) {

				var objCurriculumGrade = [];

				var TestType = { Id: ng.testTypeId };

				for (var key in curriculumGradeList) {

					objCurriculumGrade.push({
						CurriculumGradeId: curriculumGradeList[key].Id,
						Ordem: curriculumGradeList[key].Ordem,
						TypeCurriculumGradeId: curriculumGradeList[key].TypeCurriculumGradeId
					});
				}

				ng.objCourse.CourseId = course.Id;
				ng.objCourse.TestTypeCourseCurriculumGrades = objCurriculumGrade;
				ng.objCourse.TestType = TestType;
				ng.objCourse.ModalityId = course.Modality.Id;

				TestTypeCourseModel.save({ entity: ng.objCourse, TypeLevelEducationId: ng.selectedObjLevelEducation.Id, ModalityId: course.Modality.Id }, function (result) {

					if (result.success) {
						$notification.success("Curso salvo com sucesso.");
						ng.selectedObjCourse = undefined;
						ng.testTypeId = TestType.Id;
						ng.clearPagesCourse();
						ng.loadCourse();
						angular.element('#modalCourse').modal('hide');

					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};

		ng.confirmarEditCourse = function __confirmarEditCourse(course) {
			ng.course = course
			ng.curriculumGradeListBackup = angular.copy(course.TestTypeCourseCurriculumGrades);
		    CurriculumGradeModel.searchCurriculumGrade({ cur_id: course.CourseId }, function (result) {
				if (!result.success) {
					$notification[result.type ? result.type : 'error'](result.message);
					return;
				}
				ng.curriculumGradeListModal = result.lista;
				ng.curriculumGradeEdit = { curriculumGradeListModal: [] };
				ng.curriculumGradeListEdit = ng.course.TestTypeCourseCurriculumGrades;
				var ListModal, ListEdit, a, b;
				for (b in ng.curriculumGradeListEdit) {
				    ListEdit = ng.curriculumGradeListEdit[b];
				    ListEdit.checked = true;
					for (a in ng.curriculumGradeListModal) {
						ListModal = ng.curriculumGradeListModal[a];
						if (ListModal.Id == ListEdit.Id) {
							ListModal.IdBD = ListEdit.IdBD;
							ListModal.Status = ListEdit.Status;
							ListModal.checked = true;
							break;
						}
					}
				}
				angular.element('#modalCourse').modal({ backdrop: 'static' });
				return;
		    });
		};

		ng.confirmarDeletarCourse = function (course) {
		    ng.course = course;
			angular.element('#modalCourseDelete').modal({ backdrop: 'static' });
		};

		ng.saveCurriculumGrades = function (curriculumGradeList) {

			ng.curriculumGradeEdit = curriculumGradeList;

			if (ng.verificaCurriculumGrades()) {

				ng.objCurriculumGradesEdit = [];

				var TestTypeCourse = {
					Id: ng.course.Id
				}

				for (var key in curriculumGradeList) {

					ng.objCurriculumGradesEdit.push({
						Id: curriculumGradeList[key].IdBD,
						TestTypeCourse: TestTypeCourse,
						CurriculumGradeId: curriculumGradeList[key].Id,
						TypeCurriculumGradeId: 0,
						Ordem: curriculumGradeList[key].Ordem
					});
				}

				TestTypeCourseCurriculumGradeModel.save({ testTypeCourseCurriculumGrades: ng.objCurriculumGradesEdit, testTypeId: ng.testTypeId, courseId: ng.course.CourseId, typeLevelEducationId: ng.selectedObjLevelEducation.Id, modalityId: ng.course.Modality.Id }, function (result) {
					if (result.success) {
						$notification.success("Curso salvo com sucesso.");
						angular.element('#modalCourse').modal('hide');
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};

		ng.verificaCurriculumGrades = function () {

		    if (!ng.course.Modality || (ng.course.Modality && !ng.course.Modality.Id)) {

		        $notification.alert('A modalidade é obrigatória.');

		        return false;
		    }

			if (ng.curriculumGradeListEdit.length <= 0) {
			    $notification.alert('É necessário selecionar ao menos um ' + ng.curriculumGradeLabel + ' do curso.');
				return false;
			}
			return true;
		};

		ng.cancelar = function () {
			ng.course.TestTypeCourseCurriculumGrades = ng.curriculumGradeListBackup;
			angular.element('#modalCourse').modal('hide');
		};

	    /**
		 * @function - Confirmar
		 * @param {string} skillDescription
		 * @public
		 */
		ng.removeAndAddPeriod = function (list, option, indice) {
		    if (!option.checked) {
		        for (var i = 0; i < list.length; i++) {
		            if (list[i].Id == option.Id) {
		                list.splice(i, 1);
		                break;
		            }//if
		        }
		    } else {
		        list.push(option);
		    }//else
		};

		ng.activeModal = function __activeModal(label, text, frequency) {
		    if (frequency) {
		        for (var a = 0; a < ng.frequencyApplicationList.length; a++) {
		            if (ng.frequencyApplicationList[a].Id == text) {
		                text = ng.frequencyApplicationList[a].Description;
		            }
		        }
		    }
		    if (!text) return;
		    ng.textSelected = {
		        Description: label,
		        TextDescription: text
		    };
		    angular.element("#modalDescription").modal({ backdrop: 'static' });
		};

		Init();
	};

})(angular, jQuery);