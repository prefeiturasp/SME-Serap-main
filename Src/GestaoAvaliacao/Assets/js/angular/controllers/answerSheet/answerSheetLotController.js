/**
 * function AnswerSheetLot Controller
 * @namespace Controller
 * @author Rafael Odassi - 15/08/2016
 */
(function (angular, $) {
	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("AnswerSheetLotController", AnswerSheetLotController);

	AnswerSheetLotController.$inject = ['$rootScope', '$scope', '$notification', '$pager', '$util', 'AnswerSheetModel', 'AdherenceModel', 'FileModel', '$timeout'];

	/**
     * @function Controller para upload de folhas de resposta
     * @name BatchDetailsController
     * @namespace Controller
     * @memberOf appMain
     * @memberOf appMain
     * @param {Object} $rootScope
     * @param {Object} $scope
     * @param {Object} $notification
     * @param {Object} $timeout
     * @param {Object} $sce
     * @param {Object} $pager
     * @param {Object} $compile
     * @param {Object} $util
     * @param {Object} $window
     * @param {Object} AnswerSheetModel
     * @param {Object} TestModel
     * @return
     */
	function AnswerSheetLotController($rootScope, $scope, $notification, $pager, $util, AnswerSheetModel, AdherenceModel, FileModel, $timeout) {
	   
	    $scope.pageSize = 10;
	    $scope.pageSizeNewLot = 10;
	    $scope.pageSizeLotFiles = 10;
		$scope.listTests = [];
		$scope.pages = 0;
		$scope.totalItens = 0;
		$scope.answerSheet = {};
		$scope.listFilters = {};
		$scope.StateExecution = {
		    NotRequest: 1,
		    Pending: 2,
		    Canceled: 6
		};
		$scope.tab = 1;		
		$scope.listaDRE = [];
		$scope.filters = {
		    Lot_Id: undefined,
		    Test_Id: undefined,
		    StartDate: undefined,
		    EndDate: undefined,
		    StateExecution: undefined,
		    Type: undefined
		};
		$scope.pagesListaEscola = 0;
		$scope.totalItensListaEscola = 0;
		$scope.lotsSelecteds = [];
		$scope.pagesModalNewLot = 0;
		$scope.totalItensModalNewLot = 0;
		$scope.countFilter = 0;
		$scope.$watchCollection('filters', function () {
		    $scope.countFilter = 0;
		    if ($scope.filters.StartDate && $scope.tab == 1) $scope.countFilter += 1;
		    if ($scope.filters.EndDate && $scope.tab == 1) $scope.countFilter += 1;
		    if ($scope.filters.StateExecution) $scope.countFilter += 1;
		}, true);

		/**
		 * @function Set Paginação
		 * @name setPagination
		 * @namespace AnswerSheetLotController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.setPagination = function __setPagination() {
		    var type = $scope.getEnumTabs();
		    if ($scope.paginate)
		        $scope.pageSize = $scope.paginate.getPageSize();
		    $scope.paginate = type == 1 ? $pager(AnswerSheetModel.searchTestLot) : $pager(AnswerSheetModel.searchLotList);
		    $scope.paginate.pageSize($scope.pageSize);
			$scope.paginate.indexPage(0);			
			$scope.message = false;
			$scope.pages = 0;
			$scope.totalItens = 0;
			
			$scope.search();			
		};

		$scope.searchFilter = function __searchFilter(typeSearch) {
		    var type = $scope.getEnumTabs();
		    if ($scope.paginate)
		        $scope.pageSize = $scope.paginate.getPageSize();
		    $scope.paginate = type == 1 ? $pager(AnswerSheetModel.searchTestLot) : $pager(AnswerSheetModel.searchLotList);		    
		    $scope.paginate.pageSize($scope.pageSize);
			$scope.paginate.indexPage(0);
			$scope.message = false;
			$scope.pages = 0;
			$scope.totalItens = 0;
			
			$scope.search(typeSearch);
		}		

		/**
		 * @function Pesquisa.
		 * @name search
		 * @namespace AnswerSheetLotController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.search = function __search(typeSearch) {

		    var type = $scope.getEnumTabs();

		    if (type == 2 && $scope.filters.Lot_Id && $scope.filters.Lot_Id <= 0) {
		        $scope.listLots = null;
		        return;
		    } else if (type == 1 && $scope.filters.Test_Id && $scope.filters.Test_Id <= 0) {
		        $scope.listTests = null;
		        return;
		    }

		    var filters = {		        
		        Lot_Id: $scope.filters.Lot_Id,
		        Test_Id: $scope.filters.Test_Id,
		        StartDate: $scope.filters.StartDate,
		        EndDate: $scope.filters.EndDate,
		        StateExecution: $scope.filters.StateExecution,
		        Type: type
		    };
		    
		    var startD = $scope.filters.StartDate ? $scope.filters.StartDate.split("/")[0] + "-" + $scope.filters.StartDate.split("/")[1] + "-" + $scope.filters.StartDate.split("/")[2] : "";
		    var endD = $scope.filters.EndDate ? $scope.filters.EndDate.split("/")[0] + "-" + $scope.filters.EndDate.split("/")[1] + "-" + $scope.filters.EndDate.split("/")[2] : "";

		    if (typeSearch == 'filter' && moment(startD).isAfter(endD)) {		        		        
		        $notification['alert']('A data de início não pode ser maior que a de fim.');
		    }
		    else {
		        $scope.paginate.paginate(filters).then(function (result) {
		            if (result.success) {
		                type == 2 ? $scope.listLots = angular.copy(result.lista) : $scope.listTests = angular.copy(result.lista);

		                if (result.lista.length > 0) {
		                    $scope.paginate.nextPage();

		                    if (!$scope.pages > 0) {
		                        $scope.pages = $scope.paginate.totalPages();
		                        $scope.totalItens = $scope.paginate.totalItens();
		                    }
		                } else {
		                    $scope.message = true;
		                }
		            }
		            else {
		                $notification[result.type ? result.type : 'error'](result.message);
		            }

		        }, function () {
		            $scope.message = true;
		        });
		    }
		};

		$scope.save = function __save(action) {
		    var type = $scope.answerSheet.Type ? $scope.answerSheet.Type : $scope.getEnumTabs();

			var entity = {
				Id : ($scope.answerSheet.Id === undefined || $scope.answerSheet.Id === null || $scope.answerSheet.Id === 0) ? 0 : $scope.answerSheet.Id,
				Test_Id: $scope.answerSheet.Test_Id,
				uad_id: ($scope.answerSheet.DRE === undefined || $scope.answerSheet.DRE === null || $scope.answerSheet.DRE.Id === '') ? null : $scope.answerSheet.DRE.Id,
				esc_id: ($scope.answerSheet.School === undefined || $scope.answerSheet.School === null || $scope.answerSheet.School.Id === '') ? null : $scope.answerSheet.School.Id,
				StateExecution: $scope.answerSheet.StateExecution,
				Type: type
			};
			
			var list = $.map($scope.lotsSelecteds, function (val, i) {			    
			    return {
			        Test_Id: val.Code,
			        Type: type
			    };
			});

			list = $scope.answerSheet.Type == 1 ? undefined : list;

			if ($scope.answerSheet.generateAgain === false) {
				AnswerSheetModel.saveLot(entity, list, function (result) {
					if (result.success) {
						$notification.success(result.message);
						angular.element("#modalLote").modal("hide");
						angular.element("#modalNovoLote").modal("hide");
						$scope.setPagination();

						if (action == 'updateList') {
						    $scope.clear();
						    $scope.setPagination();
						}
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
			else {
				AnswerSheetModel.generateLotAgain(entity, function (result) {
					if (result.success) {
						$notification.success(result.message);
						angular.element("#modalLote").modal("hide");
						$scope.setPagination();
					}
					else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});
			}
		};		

		/**
         * @function showModal
         * @name getDREs
         * @namespace AnswerSheetLotController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
		$scope.showModal = function __showModal(entity) {
		    $scope.clearCombos();

		    $scope.answerSheet = {
		        Id: entity.Type == 2 ? entity.Lot_Id : undefined,
		        Test_Id: entity.Type == 1 ? entity.TestCode : undefined,
		        generateAgain : false,
		        Type: entity.Type,
                StateExecution: $scope.StateExecution.Pending
		    }
			
		    if (entity.Type == 1) {
		        getDREs();
		        angular.element("#modalLote").modal({ backdrop: 'static' });
		    } else {
		        $scope.save();
		    }
		}

		$scope.showModalCancel = function __showModalCancel(entity) {
			$scope.clearCombos();
			$scope.answerSheet = {
                Id: entity.Type == 2 ? entity.Lot_Id : undefined,
			    Test_Id: entity.Type == 1 ? entity.TestCode : undefined,
                Type: entity.Type
			}

			angular.element("#modalCancelar").modal({ backdrop: 'static' });
		}

		$scope.modalCancelNao = function __modalCancelSim() {
			angular.element("#modalCancelar").modal("hide");
		}

		$scope.modalCancelSim = function __modalCancelSim() {
			var entity = {
				Id: ($scope.answerSheet.Id === undefined || $scope.answerSheet.Id === null || $scope.answerSheet.Id === 0) ? 0 : $scope.answerSheet.Id,
				Test_Id: $scope.answerSheet.Test_Id,
				StateExecution: $scope.StateExecution.Canceled,
				Type: $scope.answerSheet.Type
			};

			AnswerSheetModel.saveLot(entity, function (result) {

				if (result.success) {
					$notification.success(result.message);
				}
				else {
				    $notification[result.type ? result.type : 'error'](result.message);
				}

				angular.element("#modalCancelar").modal("hide");
				$scope.setPagination();
			});
		}

		$scope.saveLot = function __saveLot(action) {
		    $scope.answerSheet = {
		        generateAgain: false,
		        StateExecution: $scope.StateExecution.NotRequest
		    }

		    $scope.save(action);
		}

		$scope.deleteLot = function __deleteLot(lot) {
		    var entity = {
		        Id: lot.Type == 1 ? lot.Id : lot.Lot_Id,
		        Type: lot.Type
		    };

		    AnswerSheetModel.deleteLot(entity, function (result) {

		        if (result.success) {
		            $notification.success(result.message);
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }

		        $scope.setPagination();
		        $scope.clear();
		    });
		}

		/**
		 * @function Limpeza dos filtros de pesquisa
		 * @name clearByFilter
		 * @namespace AnswerSheetLotController
		 * @memberOf Controller
		 * @public
		 * @param {string} filter
		 * @return
		 */
		$scope.clearFilterByDRE = function __clearFilterByDRE(filter) {
		    angular.element("#modalLote").modal("hide");
			$scope.listFilters.Schools = [];
			return;
		};

		/**
		 * @function Obter todas Escolas
		 * @name getSchools
		 * @namespace AnswerSheetLotController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
		$scope.getSchools = function __getSchools() {
			if ($scope.answerSheet.DRE === undefined || $scope.answerSheet.DRE === null)
				return;

			var params = { 'testId': $scope.answerSheet.Test_Id, 'dre_id': $scope.answerSheet.DRE.Id };

			AdherenceModel.getAdheredSchoolSimple(params, function (result) {
				if (result.success) {
					$scope.listFilters.Schools = result.lista;
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		$scope.downloadFile = function __downloadFile(fileId) {
			FileModel.checkFileExists({ Id: fileId }, function (result) {
				if (result.success) {
					window.open("/File/DownloadFile?Id=" + fileId, "_self");
				}
				else {
					$notification.alert("Arquivo não encontrado.");
				}
			});
		}

		$scope.generateAgain = function __generateAgain(entity) {
		    $scope.clearCombos();
			$scope.answerSheet = {
			    Id: entity.Type == 1 ? entity.Id : entity.Lot_Id,
				Test_Id: entity.Type == 1 ? entity.TestCode : undefined,
				generateAgain: true,
				Type: entity.Type,
				StateExecution: $scope.StateExecution.Pending
			}
			
			if (entity.Type == 1) {
			    getDREs();
			    angular.element("#modalLote").modal({ backdrop: 'static' });
			} else {
			    $scope.save();
			}
		}

		$scope.clearCombos = function __clearCombos() {
		    $scope.answerSheet.DRE = null;
		    $scope.answerSheet.School = null;
		    $scope.listFilters.DREs = [];
		    $scope.listFilters.Schools = [];
		    angular.element("#modalLote").modal("hide");
		}
	    
	    /**
         * @function Obter todas DREs
         * @name getDREs
         * @namespace AnswerSheetLotController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
		function getDREs() {
		    var params = { 'testId': $scope.answerSheet.Test_Id === undefined || $scope.answerSheet.Test_Id === null ? 0 : $scope.answerSheet.Test_Id };

		    AdherenceModel.getAdheredDreSimple(params, function (result) {
		        if (result.success) {
		            $scope.listFilters.DREs = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		$scope.open = function $open() {
		    $scope.getAnswerSheetLotSituationList();

		    $('.filters').toggleClass('filters-animation').promise().done(function a() {
		        if (angular.element(".filters").hasClass("filters-animation"))
		            angular.element('body').css('overflow', 'hidden');
		        else
		            angular.element('body').css('overflow', 'inherit');
		    });
		};
	    
		$scope.close = function $close(e) {
		    if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('next') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('day') && e.target.tagName === "TD") ||
	            $(e.target).parent().is("[data-filters]") ||
                e.target.hasAttribute('data-filters'))
		        return;

		    if (angular.element(".filters").hasClass("filters-animation")) $scope.open();
		};
	    
		$scope.clear = function $clear() {
		    $scope.filters = {
		        Lot_Id: undefined,
		        Test_Id: undefined,
		        StartDate: undefined,
		        EndDate: undefined,
		        StateExecution: undefined,
		        Type: undefined
		    };
		    $scope.countFilter = 0;
		};
	    
		$scope.datepicker = function $datepicker(id) {
		    $("#" + id).datepicker('show');
		};	    
	    
		$scope.popovermenu = {
		    title: "empty",
		    content: "empty",
		    selectedTest: undefined,
		    baixar: function (lotId) {
		        angular.element("#modalBaixar").modal({ backdrop: 'static' });
		        $scope.searchFilterBaixarLote(lotId);
		        $scope.getDREs();		        
		    }
		};
	    
		$scope.openNovoLote = function $openNovoLote() {
		    angular.element("#modalNovoLote").modal({ backdrop: 'static' });
		    $scope.loadByUserGroupSearchTest();
		    $scope.searchModalNewLot();
		}

		$scope.getEnumTabs = function $getEnumTabs() {
		    return $scope.tab == 1 ? 2 : 1;
		};

		$scope.getAnswerSheetLotSituationList = function $getAnswerSheetLotSituationList() {
		    if (!$scope.statesFilter) {
		        AnswerSheetModel.getAnswerSheetLotSituationList(function (result) {
		            if (result.success) {
		                $scope.statesFilter = result.lista;
		            }
		            else {
		                $notification[result.type ? result.type : 'error'](result.message);
		            }
		        });
		    }		    
		};

		$scope.searchFilterBaixarLote = function $searchFilterBaixarLote(lotId) {
		    if ($scope.paginateLotFiles)
		        $scope.pageSizeLotFiles = $scope.paginateLotFiles.getPageSize();
		    $scope.paginateLotFiles = $pager(AnswerSheetModel.searchLotFiles);
		    $scope.paginateLotFiles.pageSize($scope.pageSizeLotFiles);
		    $scope.paginateLotFiles.indexPage(0);
		    $scope.messageBaixarLote = false;
		    $scope.pagesBaixarLote = 0;
		    $scope.totalItensBaixarLote = 0;

		    $scope.searchLotFiles(lotId);
		};

		$scope.searchLotFiles = function $searchLotFiles(lotId) {
		    if (lotId)
		        $scope.lotId = lotId;

		    var data = {
		        Lot_Id: $scope.lotId,
		        SupAdmUnitId: $scope.dre || undefined,
		        SchoolId: $scope.escola || undefined
		    };

		    $scope.paginateLotFiles.paginate(data).then(function (result) {
		        if (result.success) {
		            $scope.lotFiles = result.lista;

		            if (result.lista.length > 0) {
		                $scope.paginateLotFiles.nextPage();

		                if (!$scope.pagesBaixarLote > 0) {
		                    $scope.pagesBaixarLote = $scope.paginateLotFiles.totalPages();
		                    $scope.totalItensBaixarLote = $scope.paginateLotFiles.totalItens();
		                }
		            } else {
		                $scope.messageBaixarLote = true;
		            }
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }

		    }, function () {
		        $scope.messageBaixarLote = true;
		    });		    
		};

		$scope.getDREs = function $getDREs() {
		    AdherenceModel.getDRESimple(function (result) {
		        if (result.success) {
		            $scope.listDres = result.lista;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};

		$scope.changeSchool = function $changeSchool(escola) {		    
		    $scope.escola = escola ? escola.Id : undefined;
		};

		$scope.getSchoolsTabEscola = function $getSchoolsTabEscola(dre) {		    
		    if (dre != null && dre != undefined && dre != "") {
		        $scope.dre = dre;

		        var data = {
		            dre_id: dre
		        };

		        AdherenceModel.getSchoolsSimple(data, function (result) {
		            if (result.success) {
		                $scope.listSchools = result.lista;
		            }
		            else {
		                $notification[result.type ? result.type : 'error'](result.message);
		            }
		        });
		    }
		    else {
		        $scope.dre = undefined;
		        $scope.escola = undefined;
		        $scope.listSchools = "";
		    }
		};

		$scope.setTab = function $setTab (currentTab) {
		    $scope.tab = currentTab;
		    $scope.clear();
		    $scope.setPagination();
		}

		$scope.loadByUserGroupSearchTest = function $loadByUserGroupSearchTest() {
		    AnswerSheetModel.loadByUserGroupSearchTest(function (result) {
		        if (result.success) {		            
		            $scope.tipoProvaCombo = result.lista.testTypeList;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });
		};		

		$scope.searchModalNewLot = function $searchModalNewLot() {
		    if ($scope.pageSearchNewLot)
		        $scope.pageSizeNewLot = $scope.pageSearchNewLot.getPageSize();
		    $scope.pageSearchNewLot = $pager(AnswerSheetModel.searchAdheredTests);
		    $scope.pageSearchNewLot.pageSize($scope.pageSizeNewLot);
		    $scope.pageSearchNewLot.indexPage(0);
		    $scope.messageModalNewLot = false;
		    $scope.pagesModalNewLot = 0;
		    $scope.totalItensModalNewLot = 0;

		    $scope.searchNewLot();
		}
	    
		$scope.searchNewLot = function $searchNewLot() {

		    if ($scope.codNovoLote && $scope.codNovoLote <= 0) {
		        $scope.listModalNewLot = $scope.lotsSelecteds = null;
		        return;
		    }

		    var filter = {
		        Test_Id: $scope.codNovoLote,
		        StartDate: $scope.dataStartNovoLote,
		        EndDate: $scope.dataEndNovoLote,
		        TestType_Id: $scope.tipoprova
		    };

		    var startD = filter.StartDate ? filter.StartDate.split("/")[0] + "-" + filter.StartDate.split("/")[1] + "-" + filter.StartDate.split("/")[2] : "";
		    var endD = filter.EndDate ? filter.EndDate.split("/")[0] + "-" + filter.EndDate.split("/")[1] + "-" + filter.EndDate.split("/")[2] : "";

		    if (moment(startD).isAfter(endD)) {
		        $notification['alert']('A data de início não pode ser maior que a data de fim.');
		    }
		    else {
		        $scope.pageSearchNewLot.paginate(filter).then(function (result) {
		            if (result.success) {
		                $scope.listModalNewLot = result.lista;		                

		                angular.forEach($scope.listModalNewLot, function (value, key) {
		                    angular.forEach($scope.lotsSelecteds, function (valueAux, keyAux) {		                        
		                        if (value.Code == valueAux.Code) {
		                            value.check = true;		                            
		                        }		                            		                        
		                    });
		                });		                

		                if (result.lista.length > 0) {
		                    $scope.pageSearchNewLot.nextPage();

		                    if (!$scope.pagesModalNewLot > 0) {
		                        $scope.pagesModalNewLot = $scope.pageSearchNewLot.totalPages();
		                        $scope.totalItensModalNewLot = $scope.pageSearchNewLot.totalItens();		                        
		                    }
		                } else {
		                    $scope.messageModalNewLot = true;
		                }
		            }
		            else {
		                $notification[result.type ? result.type : 'error'](result.message);
		            }
		        }, function () {
		            $scope.messageModalNewLot = true;
		        });
		    }
		};

		$scope.e2_CheckedItem = function $e2_CheckedItem(newLot, $event) {
		    $event.preventDefault();

		    newLot.check = !newLot.check;		    

		    if (newLot.check) {
		        $scope.lotsSelecteds.push(newLot);
		    }
		    else {		        
		        var lotsAux = $.map($scope.lotsSelecteds, function (val, i) {
		            if (newLot.Code != val.Code) {
		                return val;
		            }
		            else {
		                return null;
		            }
		        });

		        $scope.lotsSelecteds = lotsAux;
		    }
		};

		$scope.removeLotSelected = function $removeLotSelected(lot, $index) {
		    angular.forEach($scope.listModalNewLot, function (value, key) {		        
		        if (value.Code == lot.Code) {
		            value.check = false;
		        }		        
		    });
		    
		    $scope.lotsSelecteds.splice($index, 1);		    
		}

		$scope.clearModalNovoLote = function $clearModalNovoLote() {		    
		    $scope.codNovoLote = undefined;
		    $scope.dataStartNovoLote = undefined;
		    $scope.dataEndNovoLote = undefined;
		    $scope.tipoprova = undefined;

		    $scope.lotsSelecteds = [];

		    $scope.listModalNewLot = $.map($scope.listModalNewLot, function (val, i) {
		        val.check = false;
		        return val;
		    });
		};

		$scope.modalTotalProvas = function $modalTotalProvas(lot) {
		    $scope.lotIdTotalProvas = lot.Lot_Id;
		    $scope.totalProvas = lot.TestList;
		    angular.element("#modalTotalProvas").modal({ backdrop: 'static' });
		};

		$scope.modalAcompanhamentoLote = function $modalAcompanhamentoLote(entity) {
		    $scope.acompanhamentoLoteId = entity.Type == 1 ? entity.TestCode : entity.Lot_Id;

		    var data = {
		        Id: entity.Type == 1 ? entity.Id : entity.Lot_Id
		    };

		    AnswerSheetModel.searchLotHistory(data, function (result) {
		        if (result.success) {
                    console.log(result)
		            $scope.acompanhamentoLote = result.lista;
		            $scope.tamanhoPastaLote = result.entity.tempFolderSize;
		            $scope.tamanhoPastaPrincipal = result.entity.mainFolderSize;
		            $scope.ownerLote = result.entity.Owner;
		        }
		        else {
		            $notification[result.type ? result.type : 'error'](result.message);
		        }
		    });

		    angular.element("#modalAcompanhamentoLote").modal({ backdrop: 'static' });
		};

		$scope.closeModalBaixarLote = function $closeModalBaixarLote() {		    
		    $scope.dre = undefined;
		    $scope.escola = undefined;
		    $scope.listSchools = "";		    
		};
		
		function init() {
		    $scope.setPagination();
		    angular.element('body').click($scope.close);
		};

		init();		
	};
})(angular, jQuery);