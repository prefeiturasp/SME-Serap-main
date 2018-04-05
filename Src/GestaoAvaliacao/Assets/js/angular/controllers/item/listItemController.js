/**
 * @function Consulta de item
 * @namespace Controller
 * @author Everton Ferreira - 21/10/2015
 * @author Julio Cesar da Silva - 03/03/2016
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'ngTagsInput']);

	//~GETTER
	angular
		.module('appMain')
		.controller("ListItemController", ListItemController);

	ListItemController.$inject = ['$window', 'ItemModel', 'DisciplineModel', 'EvaluationMatrixModel', 'SkillModel', 'EvaluationMatrixCourseCurriculumGradeModel', 'ItemSituationModel', '$pager', '$scope', '$rootScope', '$notification', '$timeout', '$location', 'ParameterModel'];

	function ListItemController($window, ItemModel, DisciplineModel, EvaluationMatrixModel, SkillModel, EvaluationMatrixCourseCurriculumGradeModel, ItemSituationModel, $pager, $scope, $rootScope, $notification, $timeout, $location, ParameterModel) {

		function getParameters(id) {

			$scope.parameters = {};

			ParameterModel.getParameters({ Id: id }, function (result) {

				if (result.success) {

					try {

						var i = 0,
							len = result.lista.length,
							j,
							leng,
							e_,
							p_;

						for (i; i < len; i++) {

							e_ = result.lista[i];
							leng = e_.Parameters.length;


							for (j = 0; j < leng; j++) {
								p_ = e_.Parameters[j];

								$scope.parameters[e_.Parameters[j].Key] = e_.Parameters[j];
							}
						}

						if (id != 4) {

							//fluxos de carregamento
							if ($scope.editMode) {
								carregarEditarItem();
							}
							else if ($scope.createMode && $scope.comommItemID != undefined) {
								carregarTextoBase();
							}
							else if ($scope.createMode) {
								carregarDisciplinas();
							}
						} else {
							if ($scope.parameters.SHOW_ITEM_NARRATED.Value == "True")
								$scope.showItemNarrated = true;
							else $scope.showItemNarrated = false;
						}

						
					}
					catch (fail) {
						$notification.error("Erro no retorno dos parâmetros da página de cadastro de itens.")
					}
				}
				else {
					$notification.error("Não foi possível carregar os parâmetros.")
				}
				if (id != 4) config();
			});

		};

		function loaded() {
			$notification.clear();
			getParameters(2);
			getParameters(4);
		};

		function config() {

			$scope.searchItemNarrated = false;
			$scope.modelItemAnulado   = false;
			$scope.tabAltTCT = 'alternativa';
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.pageSize = 10;
			$scope.message = false;
			$scope.paginate = $pager(ItemModel.searchItems);
			$scope.paginate.indexPage(0);
			$scope.showMateria = false;
			$scope.showMatriz = false;
			configInternalObjs();
			init();
		};

		function configInternalObjs() {

		    $scope.countFilter = 0;
			$scope.palavrasChave;
			$scope.codigo;
			$scope.ItensList;
			$scope.showAddItemNarrated = false;
			$scope.headerskills;
			$scope.ShowItemNarrated = false;
			$scope.filtroProficiencia = {
				masterDescription: "Proficiência:",
				min: undefined,
				max: undefined
			};
			$scope.filtroStatusItem = {
				masterDescription: "Situação do item:",
				selecionados: [],
				lista: []
			};
			$scope.filtroVersao = {
				masterDescription: "Exibir versionamento:",
				objFiltroVersao: false
			};
			$scope.materia = {
				masterDescription: "Componente curricular",
				objMateria: undefined,
				lista: []
			};
			$scope.matriz = {
				loaded : false,
				masterDescription: "Matriz",
				objMatriz: { Id: undefined, Description: '--Selecione--' },
				lista: [{ Id: undefined, Description: '--Selecione--' }]
			};
			$scope.skills = [];
			$scope.abstract = {
				textbase: "",
				navigation: 0,
				currentID: undefined,
				item: undefined,
				listaItensID: [],
				texbaseId : 0
			};
			$scope.periodoCurso = {
			    masterDescription: Parameters.Item.ITEMCURRICULUMGRADE.Value + "(s) do curso",
				show: false,
				selecionados: [],
				lista: []
			};
			$scope.curriculumGradeLabel = Parameters.Item.ITEMCURRICULUMGRADE.Value;
			$scope.popovermenu = {
				title: "empty",
				content: "empty",
				id: undefined,
				lastVersion: undefined,
				visualizar: visualizarCallback,
				editar: editarCallback,
				excluir: excluirCallback,
				addItem: addItemCallback,
			};
			$scope.tooltip = {
				title: "",
				checked: false
			};
			$scope.skillsCount = 0;
			$scope.$watch("skills", function () {
			    $scope.skillsCount = 0;
			    for (var a = 0; a < $scope.skills.length; a++) 
			        if($scope.skills[a].objSkill)
			            if($scope.skills[a].objSkill.Id)
			                $scope.skillsCount += 1;
			    updateCountFilters();
			}, true);
			$scope.keyworldsCount = 0;
			$scope.$watchCollection("palavrasChave", function () {
			    $scope.keyworldsCount = 0;
			    if($scope.palavrasChave)
			        $scope.keyworldsCount = $scope.palavrasChave.length > 0 ? 1 : 0;
			    updateCountFilters();
			});
			$scope.seriesCount = 0;
			$scope.$watchCollection("periodoCurso.selecionados", function () {
			    $scope.seriesCount = $scope.periodoCurso.selecionados.length;
			    updateCountFilters();
			});
			$scope.$watchCollection("[materia.objMateria, matriz.objMatriz, filtroVersao.objFiltroVersao, modelItemAnulado, "
                + " searchItemNarrated, filtroProficiencia.min, filtroProficiencia.max, "
                + " filtroStatusItem.selecionados[0], filtroStatusItem.selecionados[1]]",
            function () { updateCountFilters(); });
		};

	    /**
         * @function update filter count
         * @param
         * @returns
         */
		function updateCountFilters() {
		    $scope.countFilter = 0;
		    if ($scope.materia.objMateria)
		        if ($scope.materia.objMateria.Id)
		            $scope.countFilter += 1;
		    if ($scope.matriz.objMatriz)
		        if ($scope.matriz.objMatriz.Id && $scope.matriz.loaded)
		            $scope.countFilter += 1;
		    if ($scope.filtroStatusItem.selecionados) $scope.countFilter += $scope.filtroStatusItem.selecionados.length;
		    if ($scope.filtroVersao.objFiltroVersao) $scope.countFilter += 1;
		    if ($scope.modelItemAnulado) $scope.countFilter += 1;
		    if ($scope.searchItemNarrated) $scope.countFilter += 1;
		    if ($scope.filtroProficiencia.min) $scope.countFilter += 1;
		    if ($scope.filtroProficiencia.max) $scope.countFilter += 1;
		    if ($scope.periodoCurso.selecionados) $scope.countFilter += $scope.seriesCount;
		    $scope.countFilter += $scope.skillsCount;
		    $scope.countFilter += $scope.keyworldsCount;
		};

		function init() {
			carregarDisciplinas();
			carregarSituacaoItem();
			$scope.loadPagination();
		};

		function carregarDisciplinas() {

		    DisciplineModel.loadComboHasMatrix({},function (result) {

				if (result.success) {

				    $scope.materia.lista = result.lista;
				    $scope.materia.lista.unshift({ Id: undefined, Description: '--Selecione--' });
				    $scope.materia.objMateria = { Id: undefined, Description: '--Selecione--' };
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		$scope.carregarMatriz = function carregarMatriz() {

		    $scope.matriz.lista = [{ Id: undefined, Description: '--Selecione--' }];
		    $scope.matriz.objMatriz = { Id: undefined, Description: '--Selecione--' };
		    $scope.skills = [];
		    $scope.periodoCurso.lista = [];
		    $scope.periodoCurso.selecionados = [];
		    $scope.matriz.loaded = false;

		    if ($scope.materia.objMateria.Id !== undefined)
		        EvaluationMatrixModel.getComboByDiscipline({ Id: $scope.materia.objMateria.Id }, function (result) {

					    if (result.success) {

					    	$scope.matriz.lista = result.lista;
					    	$scope.matriz.loaded = true;
					        $scope.matriz.lista.unshift({ Id: undefined, Description: '--Selecione--' });
					        $scope.matriz.objMatriz = { Id: undefined, Description: '--Selecione--' };
					        $scope.skills = [];
					        $scope.periodoCurso.lista = [];
					        $scope.periodoCurso.selecionados = [];

					        return;
					    }
					    else {
					        $notification[result.type ? result.type : 'error'](result.message);
					    }
					});
		};

		$scope.carregarSkills = function carregarSkills() {

			if ($scope.matriz.objMatriz.Id !== undefined) {

				carregarSeries();
				$scope.skills = [];

				SkillModel.getByMatriz({ Id: $scope.matriz.objMatriz.Id },
                    function (result) {

						if (result.success) {
							var formatedList = [];
							for (var key in result.lista) {

								formatedList.push({
								    Id: result.lista[key].ModelSkillLevels.Id,
								    Description: result.lista[key].ModelSkillLevels.Description,
									objSkill:{ Id: undefined, Description: '--Selecione--' },
									show: false,
									lista: $scope.getSubstractSkill(result.lista[key].ModelSkillLevels.Skills)
								});
							}
							$scope.skills = formatedList;
						}
						else {
							$notification[result.type ? result.type : 'error'](result.message);
						}
					});
			}
		};

		$scope.carregarCascadeSkill = function carregarCascadeSkill($indexSkill, node) {

		    for (var a = 0; a < $scope.skills.length; a++) {
		        if (a > $indexSkill) {
		            $scope.skills[a].lista = [{ Id: undefined, Description: "--Selecione--" }];
		            $scope.skills[a].objSkill = { Id: undefined, Description: "--Selecione--" };
		        }
		    }

		    var _nextIndex = $indexSkill + 1;
		    
		    if (_nextIndex < $scope.skills.length && node.Id > 0) {

		        SkillModel.getByParent({ Id: node.Id }, function (result) {

                    if (result.success) {

                        var _skills = [];

                        for (var key in result.lista)
                            _skills.push(result.lista[key].Skills);

                        $scope.skills[_nextIndex].lista = $scope.getSubstractSkill(_skills);
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
		    }
		};

		function carregarSituacaoItem() {

		    ItemSituationModel.load({},function (result) {

					if (result.success) {

						$scope.filtroStatusItem.lista = result.lista;

						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};

		function carregarSeries() {

			if ($scope.matriz.objMatriz != undefined) {

			    EvaluationMatrixCourseCurriculumGradeModel
                    .getCurriculumGradesByMatrix({ evaluationMatrixId: $scope.matriz.objMatriz.Id },
                    function (result) {
						if (result.success) {
							var _list = [];
							for (var key in result.lista) {
								_list.push({
									Id: result.lista[key].TypeCurriculumGrade.Id,
									Description: result.lista[key].TypeCurriculumGrade.Description
								});
							}
							$scope.periodoCurso.lista = _list;

							return;
						}
						else {
							$notification[result.type ? result.type : 'error'](result.message);
						}
					});
			}
		};

		function carregarItemByTextoBase(currentID) {

		    ItemModel.getItemSummaryById({ itemId: currentID },
                function (result) {
					if (result.success) {

						try {
							$scope.abstract.item = result.lista;
						}
						catch (err) {
							$notification.error("Erro ao carregar um item de um Texto Base.");
						}
					}
					else {
					}
				});
		};

		function carregarItemPart_01(_id) {

		    ItemModel.getMatrixByItem({ itemId: _id },
                function (result) {
					if (result.success) {
						$scope.view.item.Discipline = {
							Description: result.lista.Description,
							total: 0
						}
						$scope.view.item.Matriz = {
							Description: result.lista.EvaluationMatrix.Description
						}
						carregarItemPart_02(_id);
						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};

		function carregarItemPart_02(_id) {

		    ItemModel.getBaseTextItems({ itemId: _id },
                function (result) {
					if (result.success) {

						if (result.lista != null) {

							$scope.view.item.TextBase = result.lista.Description != null ? result.lista.Description : undefined;								

							carregarItemPart_03(_id);
						}
						else {
							$notification.error('ItemModel.getBaseTextItems -> retorno Lista vazia');
						}

						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};

		function carregarItemPart_03(_id) {

		    ItemModel.getItemById({ itemId: _id },
                function (result) {
					if (result.success) {

						if (result.lista != null) {

                            $scope.view.item.Revoked = result.lista.Revoked;
							$scope.view.item.ItemCode = result.lista.ItemCode.toString();
							$scope.view.item.Situation = result.lista.ItemSituation != null ? result.lista.ItemSituation : undefined;
							$scope.view.item.Skills = result.lista.ItemSkills;
							$scope.view.item.CurriculumGrade = result.lista.ItemCurriculumGrades[0].Description != null ? result.lista.ItemCurriculumGrades[0].Description : undefined;
							$scope.view.item.Proficiency = result.lista.proficiency != null ? result.lista.proficiency.toString() : undefined;
							$scope.view.item.Tips = result.lista.Tips != null ? result.lista.Tips : undefined;
							$scope.view.item.ItemType = result.lista.ItemType.Description;
							$scope.view.item.Keywords = result.lista.Keywords != null ? $scope.breakKeywords(result.lista.Keywords) : undefined;
							$scope.view.item.ItemLevel = result.lista.ItemLevel;
							$scope.view.item.Versions = result.lista.Versions;
							$scope.view.item.TRI = [{ preDescription: "Proporção de acertos", Value: result.lista.TRIDifficulty != null ? result.lista.TRIDifficulty.toString() : undefined },
												{ preDescription: "Discriminação", Value: result.lista.TRIDiscrimination != null ? result.lista.TRIDiscrimination.toString() : undefined },
												{ preDescription: "Acerto casual", Value: result.lista.TRICasualSetting != null ? result.lista.TRICasualSetting.toString() : undefined }];


							$scope.view.item.Sentence = result.lista.descriptorSentence != null ? result.lista.descriptorSentence : undefined;
							$scope.view.item.Statement.Description = result.lista.Statement.Description != null ? result.lista.Statement.Description : undefined;
							$scope.view.item.Alternatives = result.lista.Alternatives != null ? result.lista.Alternatives : undefined;
							$scope.view.item.id = _id;

							angular.element('#modalVisualizarFull').modal({ backdrop: 'static' });
						}
						else {
							$notification.error('ItemModel.getItemById -> retorno Lista vazia');
						}

						return;
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
		};

		$scope.loadPagination = function (searchCode) {

		    $scope.paginate.paginate(searchCode ? { ItemCode: $scope.codigo } : $scope.currentFilters)
                .then( function (result) {
					if (result.lista.length > 0) {
						$scope.paginate.nextPage();
						$scope.ItensList = result.lista;
						if (!$scope.pages > 0) {
							$scope.pages = $scope.paginate.totalPages();
							$scope.totalItens = $scope.paginate.totalItens();
						}
					}
					else {
						$scope.message = true;
						$scope.ItensList = null;
					}
					if (!searchCode && searchCode == undefined ) {
						$scope.codigo = "";
					}
				}, function (result) {
					$scope.message = true;
					$scope.ItensList = null;
				});
		};

		$scope.carregarTextoBase = function (_id, narrated) {

		    $scope.showAddItemNarrated = narrated;

		    ItemModel.getBaseTextItems({ itemId: _id },
				function (result) {
					if (result.success) {
						try {

							$scope.abstract.navigation = 0;
							$scope.abstract.textbase = result.lista.Description;
							$scope.abstract.texbaseId = result.lista.Id;

							$scope.abstract.listaItensID = (result.lista.Items.length > 0) ? result.lista.Items : [result.lista.Items];
							carregarItemByTextoBase($scope.abstract.listaItensID[0]);
							angular.element('#modalVisualizar').modal({ backdrop: 'static' });
						}
						catch (err) { }
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				}
			);
		};

		$scope.getSubstractSkill = function (_lista) {

			var substracts = [];

			for (var i = 0; i < _lista.length; i++) {
				if (_lista[i].Description.length > 20) {
					substracts.push({
						Id: _lista[i].Id,
						Description: _lista[i].Description,
						Substract: _lista[i].Description.substring(0, 20) + "...",
						LastLevel: _lista[i].LastLevel
					});
				}
				else {
					substracts.push({
						Id: _lista[i].Id,
						Description: _lista[i].Description,
						Substract: _lista[i].Description,
						LastLevel: _lista[i].LastLevel
					});
				}
			}

			substracts.unshift({ Id: undefined, Description: '--Selecione--' });

			return substracts;
		};

		$scope.pesquisar = function (searchCode) {
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.paginate.indexPage(0);
			$scope.pageSize = $scope.paginate.getPageSize();
			$scope.setCurrentFilters();
			$scope.loadPagination(searchCode);
		};

		$scope.searchItemNarrado = function _searchItemNarrado() {
		    $scope.searchItemNarrated = !$scope.searchItemNarrated;
		};

		$scope.searchItemAnulado = function _searchItemAnulado() {
		    $scope.modelItemAnulado = !$scope.modelItemAnulado;
		};

		$scope.setCurrentFilters = function () {

			var lastskill = $scope.getLastSkillID();
			$scope.currentFilters = {
			    ItemSituation: $scope.getGroupIDs($scope.filtroStatusItem.selecionados),
			    ShowVersion: $scope.filtroVersao.objFiltroVersao,
			    ProficiencyStart: $scope.filtroProficiencia.min,
			    ProficiencyEnd: $scope.filtroProficiencia.max,
			    Keywords: $scope.joinKeywords($scope.palavrasChave),
			    DisciplineId: ($scope.materia.objMateria !== undefined && $scope.materia.objMateria != null) ? $scope.materia.objMateria.Id : undefined,
			    EvaluationMatrixId: ($scope.matriz.objMatriz !== undefined && $scope.matriz.objMatriz != null) ? $scope.matriz.objMatriz.Id : undefined,
			    SkillId: lastskill.Id,
			    SkillLastLevel: lastskill.LastLevel,
			    ShowItemNarrated: $scope.searchItemNarrated,
			    Revoked: $scope.modelItemAnulado,
			    TypeCurriculumGrades: $scope.getGroupIDs($scope.periodoCurso.selecionados)
			};
		};

		$scope.getGroupIDs = function (_group) {

		    var IDs = "";

		    for (var i = 0; i < _group.length; i++) {
		        if (i < (_group.length - 1))
		            IDs = IDs.concat(_group[i].Id + ",");
		        else
		            IDs = IDs.concat(_group[i].Id + "");
		    }

		    return IDs;
		};

		$scope.getLastSkillID = function () {

			var lastskill = {
				Id: undefined,
				LastLevel: false
			};

			for (var i = 0; i < $scope.skills.length; i++) {
				if ($scope.skills[i].objSkill != undefined) {
					lastskill.Id = $scope.skills[i].objSkill.Id;
					lastskill.LastLevel = $scope.skills[i].objSkill.LastLevel;
				}
				else {
					break;
				}
			}

			return lastskill;
		};

		$scope.novoItem = function () {
			$window.location.href = "/Item/Form";
		};

		$scope.breakKeywords = function (_tags) {

		    var arrTags = [];

		    var arrSplited = _tags.split(";")

		    for (var key in arrSplited) {
		        arrSplited[key] = arrSplited[key].replace(";", "");
		        arrTags.push(arrSplited[key]);
		    }

		    return arrTags;
		};

		$scope.createView = function () {

			$scope.view = {
				item: {
					ItemCode: undefined,
					Situation: undefined,
					Discipline: undefined,
					Matriz: undefined,
					Skills: undefined,
					CurriculumGrade: undefined,
					Proficiency: undefined,
					Tips: undefined,
					ItemType: undefined,
					Keywords: undefined,
					ItemLevel: undefined,
					Versions: undefined,
					TRI: undefined,
					Sentence: undefined,
					TextBase: undefined,
					Statement: {
						Descritpion: undefined
					},
					Alternatives: undefined
				}
			};
		};

		function visualizarCallback(_id) {

			$scope.createView();
			carregarItemPart_01(_id);
		};

		function editarCallback(_id) {
			
			var timer = setTimeout(function () {

				clearInterval(timer);

				$window.location.href = '/Item/Form?id=' + _id;

			}, 100);
		};

		function excluirCallback(_id) {
			$scope.targetId = _id;
			angular.element("#modaldelete").modal({ backdrop: 'static' });
		};

		function addItemCallback(_id) {

			$scope.targetId = _id;
			$scope.addItem(_id);
		};

		$scope.delete = function () {

			ItemModel.delete({ Id: $scope.targetId },

			function (result) {

				if (result.success) {
					angular.element("#modaldelete").modal("hide");
					$notification.success("Item excluído com sucesso!");
					$scope.pesquisar();
					$scope.targetId = undefined;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			},
			function () {
			});
		};

		$scope.canceldelete = function () {
			$scope.targetId = undefined;
			angular.element('#modaldelete').modal('hide');
		};

		$scope.setMenu = function (_id, _lastVersion, BaseTextId) {
			$scope.popovermenu.id = _id;
			$scope.popovermenu.lastVersion = _lastVersion;
			$scope.popovermenu.hasBaseText = (BaseTextId == null) ? false : true;
		};

		$scope.skillAccordion = function (_index) {
			try {
				$scope.skills[_index].show = !$scope.skills[_index].show;
			}
			catch (error) {
			}
		};

		$scope.periodoAccordion = function () {
			try {
				$scope.periodoCurso.show = !$scope.periodoCurso.show;
			}
			catch (error) {
			}
		};

		$scope.next = function () {

			$scope.abstract.navigation += 1;

			if ($scope.abstract.navigation > ($scope.abstract.listaItensID.length - 1))
				$scope.abstract.navigation = 0;

			carregarItemByTextoBase($scope.abstract.listaItensID[$scope.abstract.navigation]);
		};

		$scope.previus = function () {

			$scope.abstract.navigation += -1;

			if ($scope.abstract.navigation < 0)
				$scope.abstract.navigation = ($scope.abstract.listaItensID.length - 1);

			carregarItemByTextoBase($scope.abstract.listaItensID[$scope.abstract.navigation]);
		};

		$scope.joinKeywords = function (_tags) {

			var keywords = "";

			if (_tags !== undefined) {

				for (var i = 0; i < _tags.length; i++) {

					if (i < (_tags.length - 1))
						keywords = keywords.concat(_tags[i].text + ",");
					else
						keywords = keywords.concat(_tags[i].text);
				}
			}
			return keywords;
		};

		$scope.limpar = function () {

			$scope.palavrasChave = undefined;
			//$scope.codigo = undefined;
			$scope.filtroProficiencia.min = undefined;
			$scope.filtroProficiencia.max = undefined;
			$scope.filtroStatusItem.selecionados = [];
			$scope.materia.objMateria = undefined;
			$scope.matriz.objMatriz = undefined;
			$scope.skills = [];
			$scope.periodoCurso.selecionados = [];
			//$scope.pesquisar();
			$scope.ShowItemNarrated = false;
			$scope.modelItemAnulado = false;
			$scope.searchItemNarrated = false;
			$scope.filtroVersao.objFiltroVersao = false;
		};

		$scope.setTabAltTCT = function (tab) {

			$scope.tabAltTCT = tab;
		};

		$scope.showTabAltTCT = function (tab) {

			if ($scope.tabAltTCT == tab)
				return true;
			return false;
		};

		$scope.addItem = function (id) {

			angular.element('#modalVisualizar').modal('hide');

			var timer = $timeout(function () {

				$window.location.href = '/Item/Form?i='+ id;

				$timeout.cancel(timer);

			}, 500);
		};

		$scope.previewPrint = function (_id) {
			window.open("/Item/PreviewPrintItem?id=" + _id);
		}

		$scope.previewPrintBaseText = function (id) {
			window.open("/Item/PreviewPrintBaseText?id=" + id);
		}
		
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

		loaded();
	};

})(angular, jQuery);