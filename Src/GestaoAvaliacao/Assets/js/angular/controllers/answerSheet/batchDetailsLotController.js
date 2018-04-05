/**
 * function BatchDetails Controller
 * @namespace Controller
 * @author Julio Cesar Silva - 10/12/2015
 */
(function (angular, $) {

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
		.controller("BatchDetailsLotController", BatchDetailsLotController);


    BatchDetailsLotController.$inject = ['$rootScope', '$scope', '$notification', '$timeout', '$sce', '$pager', '$compile', '$util', '$window', 'AnswerSheetModel', 'AdherenceModel', 'FileModel', 'TestListModel', 'ParameterModel', '$interval'];

    /**
     * @function Controller para upload de folhas de resposta
     * @name BatchDetailsLotController
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
     * @return
     */
    function BatchDetailsLotController($rootScope, $scope, $notification, $timeout, $sce, $pager, $compile, $util, $window, AnswerSheetModel, AdherenceModel, FileModel, TestListModel, ParameterModel, $interval) {

        $scope.paginate = $pager(AnswerSheetModel.getBatchAnswerSheetDetail);
        $scope.pageSize = 10;
        $scope.WARNING_UPLOAD_BATCH_DETAIL = Boolean(Parameters.General.WARNING_UPLOAD_BATCH_DETAIL == "True");
        $scope.EnumBatchSituationByte = IndexBatchDetailsEnumBatchSituationByte;
        $scope.EnumBatchSituationLabel = IndexBatchDetailsEnumBatchSituationLabel;
        $scope.EnumBatchSituationDescription = IndexBatchDetailsEnumBatchSituationDescription;
        $scope.EnumBatchProcessingByte = IndexBatchDetailsEnumBatchProcessingByte;
        $scope.EnumBatchProcessingLabel = IndexBatchDetailsEnumBatchProcessingLabel;

        $scope.exibirAnswerSheetBatchFiles = false;
        //$scope.searchFieldNomeAluno;
        //$scope.searchFieldTurma;

        $scope.BatchQueueFileComplete = {
            id: 0,
            name: "",
            dre: "",
            school: "",
            dateUpload: "",
            dateProc: "",
            user: ""
        };

        /**
         * @function Config. para lista de arquivos em processo de extração
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        (function configUploadQueueStatus() {
            $scope.EnumUploadQueueStatus = {
                PendingUnzip: 1,
                Success: 2,
                NotUnziped: 3,
                Processing: 4
            };
            $scope.listUploadQueueStatus = [
                { label: "Todos", code: undefined },
                { label: "Aguardando descompactação", code: 1 },
                { label: "Descompactado com sucesso", code: 2 },
                { label: "Erro na descompactação", code: 3 },
                { label: "Descompactação em andamento", code: 4 }];
            $scope.uploadQueueStatus = undefined;
            $scope.pagerUploadQueue = {
                paginate: $pager(AnswerSheetModel.getUploadQueueStatusDreSchool),
                pages: 0,
                totalItens: 0,
                message: false,
                pageSize: 10,
                list: null,
                lastFive: null
            };
            $scope.pagerUploadQueue.paginate.indexPage(0);
            $scope.topUploadQueue = [];
        })();

        /**
         * @function Trocar filtro e pesquisar
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        $scope.changeUploadQueueFilter = function changeUploadQueueFilter(_uploadQueueStatus) {
            $scope.uploadQueueStatus = _uploadQueueStatus;
            clearUploadQueurPagination();
            getUploadQueueStatus();
        };

        $scope.showFiles = function showFiles(_batchQueueFileId, _fileName, SupAdmUnitName, SchoolName, DtUpload, DtProcessamento, UserName) {
            $scope.BatchQueueFileComplete = {
                id: _batchQueueFileId,
                name: _fileName,
                dre: SupAdmUnitName,
                school: SchoolName,
                dateUpload: DtUpload,
                dateProc: DtProcessamento,
                user: UserName
            };

            $scope.ShowAllStudentsLot = true;
            $scope.filters.SituationLot.value = '';
            $scope.filters.SituationLot.state = false;
            $scope.filters.BatchQueueFile.value = { Id: _batchQueueFileId, Name: _fileName }
            angular.element(document.getElementById('uploadQueueModal')).modal('hide');
            $scope.exibirAnswerSheetBatchFiles = true;
            $scope.setPagination();

        }

        $scope.deleteError = function deleteError() {
            angular.element("#modalErro").modal('hide');

            AnswerSheetModel.deleteBatchFilesError({ Id: $scope.itemDeletadoErros.Id }, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                    $scope.search();
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }

        $scope.callModalError = function (i) {
            $scope.itemDeletadoErros = i;
            angular.element("#modalErro").modal({ backdrop: 'static' });
        };

        $scope.callModal = function (i) {
            $scope.itemDeletado = i;
            angular.element("#modal").modal({ backdrop: 'static' });
        };

        $scope.callModalDeleteFileError = function (i) {
            $scope.fileDeletado = i;
            angular.element("#modalDeleteErro").modal({ backdrop: 'static' });
        }

        $scope.deleteFileById = function __deleteFileById() {
            angular.element("#modalDeleteErro").modal('hide');
            AnswerSheetModel.deleteFileById({ file: $scope.fileDeletado }, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                    $scope.paginate.indexPage(0);
                    $scope.filtra($scope.searchFieldNomeAluno, $scope.searchFieldTurma);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }


        $scope.deleteZip = function deleteZip() {
            angular.element("#modal").modal('hide');

            AnswerSheetModel.deleteBatchQueueAndFiles({ Id: $scope.itemDeletado.Id }, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                    $scope.search();
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }

        $scope.voltarListagemLotes = function voltarListagemLotes() {
            $scope.ShowAllStudentsLot = false;
            $scope.exibirAnswerSheetBatchFiles = false;
            $scope.filters.BatchQueueFile.value = '';
            clearUploadQueurPagination();
            $scope.setPagination();
        }

        $scope.printFilter = function printFilter(label) {
            var activeFilterString = "";

            if (label == $scope.filters.StartDate.label || label == $scope.filters.EndDate.label) {
                if ($scope.exibirAnswerSheetBatchFiles) {
                    activeFilterString = $scope.filters.FilterDateUpdate.value ? "Data correção" : "Data upload";
                    activeFilterString += (label == $scope.filters.StartDate.label) ? " início" : " fim";
                }
                else {
                    activeFilterString = "";
                }
            }
            else
                activeFilterString = label;

            if (label == $scope.filters.BatchQueueFile.label && $scope.exibirAnswerSheetBatchFiles)
                activeFilterString += " " + $scope.filters.BatchQueueFile.value.Id + " - " + $scope.filters.BatchQueueFile.value.Name;
            else if (label == $scope.filters.DRE.label)
                activeFilterString += $scope.filters.DRE.value != null ? " " + $scope.filters.DRE.value.Description : "";
            else if (label == $scope.filters.School.label)
                activeFilterString += $scope.filters.School.value != null ? " " + $scope.filters.School.value.Description : "";
            else if (label == $scope.filters.SituationLot.label && !$scope.exibirAnswerSheetBatchFiles)
                activeFilterString += $scope.filters.SituationLot.value != null ? " " + $scope.filters.SituationLot.value.Description : "";
            else if (label == $scope.filters.Processing.label && $scope.exibirAnswerSheetBatchFiles)
                activeFilterString += " " + $scope.filters.Processing;
            else if (label == $scope.filters.StartDate.label && $scope.exibirAnswerSheetBatchFiles)
                activeFilterString += " " + formatDate($scope.filters.StartDate.value);
            else if (label == $scope.filters.EndDate.label && $scope.exibirAnswerSheetBatchFiles)
                activeFilterString += " " + formatDate($scope.filters.EndDate.value);

            return activeFilterString;
        }

        $scope.printProcessingFilter = function printProcessingFilter(value) {

            for (var x = 0; x < $scope.listFilter.Processing.length; x++) {
                if (value == $scope.listFilter.Processing[x].Id) {
                    return value == $scope.EnumBatchSituationByte.Warning ? (!$scope.WARNING_UPLOAD_BATCH_DETAIL ? $scope.listFilter.Processing[x].Description : 'Sucesso') : $scope.listFilter.Processing[x].Description;
                }
            }
        }

        $scope.removeFilter = function removeFilter(label) {
            if (label == $scope.filters.BatchQueueFile.label) {
                $notification.alert("Não é possível remover o filtro do arquivo zip.");
                return;
            }
            else if (label == $scope.filters.DRE.label) {
                $scope.filters.DRE.value = '';
                $scope.filters.DRE.state = false;
            }
            else if (label == $scope.filters.School.label) {
                $scope.filters.School.value = '';
                $scope.filters.School.state = false;
            }
            else if (label == $scope.filters.SituationLot.label) {
                $scope.filters.SituationLot.value = '';
                $scope.filters.SituationLot.state = false;
            }
            else if (label == $scope.filters.StartDate.label) {
                $scope.filters.StartDate.value = '';
                $scope.filters.StartDate.state = false;
                $scope.processingFilter = [];
                for (var i = 0; i < $scope.listFilter.Processing.length; i++) {
                    //procurando as situações que não sejam 1 e 7 pra desmarcar no menu de filtros
                    if ($scope.listFilter.Processing[i].Id != 1 && $scope.listFilter.Processing[i].Id != 7) {
                        $scope.listFilter.Processing[i].state = false;
                    }
                    if ($scope.listFilter.Processing[i].state) {
                        $scope.processingFilter.push({ Id: $scope.listFilter.Processing[i].Id, state: true });
                    }
                }
            }
            else if (label == $scope.filters.EndDate.label) {
                $scope.filters.EndDate.value = '';
                $scope.filters.EndDate.state = false;
                $scope.processingFilter = [];
                for (var i = 0; i < $scope.listFilter.Processing.length; i++) {
                    //procurando as situações que não sejam 1 e 7 pra desmarcar no menu de filtros
                    if ($scope.listFilter.Processing[i].Id != 1 && $scope.listFilter.Processing[i].Id != 7) {
                        $scope.listFilter.Processing[i].state = false;
                    } else if ($scope.listFilter.Processing[i].state) {
                        $scope.processingFilter.push({ Id: $scope.listFilter.Processing[i].Id, state: true });
                    }
                }
            }
            else if (label == $scope.filters.Processing.label) {
                $scope.filters.Processing.value = '';
            }
            $scope.setPagination();

        }

        $scope.removeProcessingFilter = function removeProcessingFilter(index) {

            if ($scope.processingFilter.length > 1) {
                if (index > -1) {
                    for (i = 0; i < $scope.listFilter.Processing.length; i++) {
                        //procurando o id igual no array para desmarcá-lo
                        if ($scope.listFilter.Processing[i].Id === $scope.processingFilter[index].Id) {
                            $scope.listFilter.Processing[i].state = false;
                        }
                    }

                    $scope.processingFilter.splice(index, 1);
                }
                $scope.setPagination();
            }
            else {
                $notification.alert("Selecione uma situação.");
                return;
            }
        }

        /**
         * @function Limpar pesquisa paginada
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        function clearUploadQueurPagination() {
            $scope.pagerUploadQueue.paginate.indexPage(0);
            $scope.pagerUploadQueue.message = false;
            $scope.pagerUploadQueue.pages = 0;
            $scope.pagerUploadQueue.pageSize = $scope.pagerUploadQueue.paginate.getPageSize();
            $scope.pagerUploadQueue.totalItens = 0;
            $scope.pagerUploadQueue.list = null;
        };

        /**
         * @function Criar XHR (Ajax) para obter status de unzip/save de arquivos em lotes
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        function getUploadQueueStatus() {
            $scope.pagerUploadQueue.paginate.paginate({ Situation: ($scope.filters !== undefined && $scope.filters.SituationLot.value != null) ? $scope.filters.SituationLot.value.Id : undefined, SupAdmUnitId: ($scope.filters !== undefined && $scope.filters.DRE.value != null) ? $scope.filters.DRE.value.Id : undefined, SchoolId: ($scope.filters !== undefined && $scope.filters.School.value != null) ? $scope.filters.School.value.Id : undefined, StartDate: $scope.filters !== undefined ? $scope.filters.StartDate.value : undefined, EndDate: $scope.filters !== undefined ? $scope.filters.EndDate.value : undefined, top: $scope.filters !== undefined ? $scope.filters.topFiltro : undefined }).then(function (result) {
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.pagerUploadQueue.paginate.nextPage();
                        $scope.pagerUploadQueue.list = result.lista;
                        $timeout(function () {
                            for (var a = 0; a < $scope.pagerUploadQueue.list.length; a++)
                                angular.element('#batch_' + $scope.pagerUploadQueue.list[a].File_Id).removeClass('in');
                        }, 0);
                        if (!$scope.pagerUploadQueue.pages > 0) {
                            $scope.pagerUploadQueue.pages = $scope.pagerUploadQueue.paginate.totalPages();
                            $scope.pagerUploadQueue.totalItens = $scope.pagerUploadQueue.paginate.totalItens();
                        }
                    } else {
                        $scope.pagerUploadQueue.message = true;
                        $scope.pagerUploadQueue.list = null;
                        $scope.pagerUploadQueue.totalItens = 0;
                        $scope.pagerUploadQueue.pages = 0;

                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                $scope.pagerUploadQueue.message = true;
                $scope.pagerUploadQueue.list = null;
            });
        };

        /**
         * @function Obter últimos arquivos 
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        function getUploadQueueTop() {

            AnswerSheetModel.getUploadQueueTop({ Top: 5 }, function (result) {
                if (result.success) {
                    $scope.topUploadQueue = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function ref. pode ser utilizado no context do $scope angular
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        $scope.getUploadQueueStatus = getUploadQueueStatus;

        /**
         * @function ref. pode ser utilizado no context do $scope angular
         * @author julio.silva@mstech.com.br
         * @since 28/09/2016
         * @returns
         */
        $scope.getUploadQueueTop = getUploadQueueTop; getUploadQueueTop();

        ///**
        // * @function Abrir modal para lista de arquivos .zip na fila de extração
        // * @author julio.silva@mstech.com.br
        // * @since 29/09/2016
        // * @returns
        // */
        //$scope.openUploadQueueModal = function __openUploadQueueModal() {
        //    angular.element("#uploadQueueModal").modal({ backdrop: 'static' });
        //    clearUploadQueurPagination();
        //    getUploadQueueStatus();
        //};

        /**
		 * @function Armazena na variavel do scope os filtros atuais.
		 * @name getFilter
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @private
	     * @param 
		 * @return
		 */
        function getFilter() {
            if ($scope.testInformation) {
                $scope.currentFilter = {
                    BatchId: ($scope.testInformation.batch === undefined || $scope.testInformation.batch === null) ? 0 : $scope.testInformation.batch.Id,
                    TestId: ($scope.testInformation.test.Id === undefined || $scope.testInformation.test.Id === null || !parseInt($scope.testInformation.test.Id)) ? 0 : $scope.testInformation.test.Id,
                    SupAdmUnitId: ($scope.filters.DRE !== undefined && $scope.filters.DRE.value !== null) ? $scope.filters.DRE.value.Id : null,
                    Situation: ($scope.filters.SituationLot !== undefined && $scope.filters.SituationLot.value !== null) ? $scope.filters.SituationLot.value.Id : null,
                    SchoolId: ($scope.filters.School !== undefined && $scope.filters.School.value !== null) ? $scope.filters.School.value.Id : ($scope.testInformation.school.Id === undefined || $scope.testInformation.school.Id === null || !parseInt($scope.testInformation.school.Id)) ? 0 : $scope.testInformation.school.Id,
                    CurriculumTypeId: $scope.filters.Year ? $scope.filters.Year.CurriculumTypeId : null,
                    ShiftTypeId: $scope.filters.Turn ? $scope.filters.Turn.Id : null,
                    SectionId: ($scope.testInformation.team.Id === undefined || $scope.testInformation.team.Id === null || !parseInt($scope.testInformation.team.Id)) ? 0 : $scope.testInformation.team.Id,
                    StartDate: $scope.filters.StartDate.value,
                    EndDate: $scope.filters.EndDate.value,
                    Processing: getValuesProcessingFilter(),
                    FilterDateUpdate: $scope.filters.FilterDateUpdate.value,
                    BatchQueueId: ($scope.filters.BatchQueueFile && $scope.filters.BatchQueueFile.value) ? $scope.filters.BatchQueueFile.value.Id : 0,
                    AluNome: $scope.searchFieldNomeAluno,
                    Turma: $scope.searchFieldTurma,
                    ShowAllStudentsLot: $scope.ShowAllStudentsLot
                };
            } else {

                $scope.currentFilter = {
                    BatchId: $scope.filters.BatchId,
                    BatchQueueId: ($scope.filters.BatchQueueFile && $scope.filters.BatchQueueFile.value) ? $scope.filters.BatchQueueFile.value.Id : 0,
                    SupAdmUnitId: ($scope.filters.DRE !== undefined && $scope.filters.DRE.value !== null) ? $scope.filters.DRE.value.Id : null,
                    SchoolId: (($scope.filters.School && $scope.filters.School.value != null) ? $scope.filters.School.value.Id : null),
                    Situation: ($scope.filters.SituationLot !== undefined && $scope.filters.SituationLot.value !== null) ? $scope.filters.SituationLot.value.Id : null,
                    CurriculumTypeId: $scope.filters.Year ? $scope.filters.Year.CurriculumTypeId : null,
                    ShiftTypeId: $scope.filters.Turn ? $scope.filters.Turn.Id : null,
                    StartDate: $scope.filters.StartDate.value,
                    EndDate: $scope.filters.EndDate.value,
                    Processing: getValuesProcessingFilter(),
                    FilterDateUpdate: $scope.filters.FilterDateUpdate.value,
                    AluNome: $scope.searchFieldNomeAluno,
                    Turma: $scope.searchFieldTurma,
                    ShowAllStudentsLot: $scope.ShowAllStudentsLot
                };
            }
        };

        /**
		 * @function Retorna os Ids de cada estado de processamento selecionado concatenado separado por virgula
		 * @name getValuesProcessingFilter
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @private
	     * @param 
		 * @return {String}
		 */
        function getValuesProcessingFilter() {

            var i, max = $scope.processingFilter.length, result = "";
            for (i = 0; i < max; i++) {
                if (i < max - 1) result += $scope.processingFilter[i].Id + ","
                else result += $scope.processingFilter[i].Id
            }//for i
            return result;
        };

        /**
		 * @function Armazena na variavel do scope os filtros atuais.
		 * @name getFilter
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @private
	     * @param 
		 * @return
		 */
        function createBatch(fileId) {
            getFilter();
            if ($scope.authorize)
                if ($scope.authorize.team.Id != 0)
                    $scope.identificationType = 1;
            var entity = {
                Id: ($scope.batch !== undefined && $scope.batch !== null) ? $scope.batch.Id : 0,
                Test_Id: $scope.currentFilter.TestId,
                SupAdmUnit_Id: getDERId(),
                BatchType: $scope.identificationType,
                School_Id: getSchoolId(),
                Section_Id: ($scope.currentFilter.SectionId > 0 ? $scope.currentFilter.SectionId : null),
                AnswerSheetBatchFiles: [{ File_Id: fileId }]
            };
            AnswerSheetModel.saveBatch(entity, function (result) {
                if (result.success) {
                    $scope.batch = {
                        Id: result.batchId
                    };
                    $notification.success(result.message);
                    $scope.identificationTypeLook = true;
                    angular.element(".loading-box").addClass('ng-hide');
                    angular.element("#preloading").html("Carregando...").css('left', '46%');
                    if ($scope.pagerUploadQueue != null && $scope.pagerUploadQueue.list != null) {
                        $scope.voltarListagemLotes();
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    angular.element(".loading-box").addClass('ng-hide');
                    angular.element("#preloading").html("Carregando...").css('left', '46%');
                }
                getUploadQueueTop();
            });
        };

        function getSchoolId() {

            if ($scope.currentFilter.SchoolId > 0 && $scope.params.test_id) return $scope.currentFilter.SchoolId;
            else if ($scope.filtersUpload.School && !$scope.params.test_id) return $scope.filtersUpload.School.Id;
            else return null;
        };

        function getDERId() {

            if ($scope.currentFilter.SupAdmUnitId > 0 && $scope.params.test_id) return scope.currentFilter.SupAdmUnitId;
            else if ($scope.filtersUpload.DRE && !$scope.params.test_id) return $scope.filtersUpload.DRE.Id;
            else return null;
        };

        /**
		 * @function esconde ou mostra as situações de Id-7 erro na identificação e de Id-8 Na fila para identificação
		 * @name showHideSituation
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @public
	     * @param {int} time
		 * @return
		 */
        $scope.showHideSituation = function (processing, value) {

            if (processing.Id != 7 && processing.Id != 1) {
                return value;
            }
            return !value;
        };

        $scope.checkDateSelected = function __checkDateSelected() {
            return !$scope.filters.StartDate.value || !$scope.filters.EndDate.value
        };

        $scope.countFiltersSelected = function __countFiltersSelected() {

            var i, max = Object.keys($scope.filters).length, list = Object.keys($scope.filters), count = 0;
            for (i = 0; i < max; i++) {
                if ($scope.filters[list[i]] != undefined && $scope.filters[list[i]].value && $scope.filters[list[i]].state) {
                    count++;
                }
            }

            for (i = 0; i < $scope.processingFilter.length; i++) {
                if ($scope.processingFilter[i].state) {
                    count++;
                }
            }

            $scope.notificationFilter = count;

            return count;

        };

        /**
		 * @function Redirecionar.
		 * @name redirectToList
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @private
	     * @param {int} time
		 * @return
		 */
        function redirectToList(time) {
            if (time < 0) time = 0;
            $timeout(function __invalidTestId() {
                if ($scope.testInformation) {
                    if ($scope.testInformation.batch !== undefined && $scope.testInformation.batch !== null) {
                        if ($scope.QRCode !== undefined && $scope.QRCode !== null) {
                            if ($scope.QRCode == true)
                                $window.location.href = "/Test/IndexAdministrate?test_id=" + $scope.batch.Test_Id;
                            else
                                $window.location.href = "/Correction/IndexForm?test_id=" + $scope.batch.Test_Id + '&team_id=' + $scope.batch.Section_Id;
                        }
                        else {
                            $notification.alert("Não foi possível identificar a página de origem.");
                        }
                    } else if ($scope.testInformation.team.Id !== undefined && $scope.testInformation.team.Id !== null && parseInt($scope.testInformation.team.Id)) {
                        $window.location.href = "/Correction/IndexForm?test_id=" + $scope.testInformation.test.Id + '&team_id=' + $scope.testInformation.team.Id;
                    } else if ($scope.testInformation.school.Id !== undefined && $scope.testInformation.school.Id == 0) {
                        $window.location.href = "/Test/IndexStudentResponses?test_id=" + $scope.testInformation.test.Id;
                    } else if ($scope.testInformation.school.Id !== undefined && $scope.testInformation.school.Id !== null && parseInt($scope.testInformation.school.Id)) {
                        $window.location.href = "/Test/IndexAdministrate?test_id=" + $scope.testInformation.test.Id;
                    } else {
                        $notification.alert("Não foi possível identificar a página de origem.");
                    }
                } else {
                    $notification.alert("Não foi possível obter os dados de acesso a página.");
                    setTimeout(function () { $window.history.back(); }, 3000);
                }
            }, time);
        };

        /**
        */
        $scope.redirectTestId = function __redirectTestId(Id) {
            if (Id) {
                $window.location.href = "/AnswerSheet/IndexBatchDetails?test_id=" + Id;
            }
        }

        /**
		 * @function Inicialização das listas de filtros de pesquisa.
		 * @name configListFilter
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        function configListFilter() {

            $scope.listFilter = {
                DREs: [],
                Schools: [],
                Processing: [],
                DRE: [],
                School: [],
                SituationLot: []
            };

            $scope.listFilterUpload = {
                DREs: [],
                Schools: [],
                Processing: [],
                DRE: [],
                School: [],
                SituationLot: []
            };
        };

        /**
         * @function Configuração dos filtros
         * @name configFilters
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
        function configFilters() {

            $scope.filters = {
                BatchQueueFile: { label: "Arquivo zip", value: "", state: false },
                DRE: { label: "DRE", value: "", state: false },
                School: { label: "Escola", value: "", state: false },
                Processing: { label: "Situação", value: "", state: false },
                StartDate: { label: "Data início", value: "", state: false },
                EndDate: { label: "Data fim", value: "", state: false },
                FilterDateUpdate: { label: "DataCorrecao", value: true }, //auxiliar
                SituationLot: { label: "Situação do lote - ", value: "", state: false }
            };
        };

        /**
        * @function Configuração dos filtros da modal de upload
        * @name configFiltersUpload
        * @namespace BatchDetailsController
        * @memberOf Controller
        * @private
        * @param
        * @return
        */
        function configFiltersUpload() {

            $scope.filtersUpload = {
                DRE: undefined,
                School: undefined
            };
        };

        //Formatação para as tags de filtros ativos
        function formatDate(date) {
            var data = new Date(date.replace(/-/g, "/"));
            var dia = data.getDate();
            if (dia.toString().length == 1)
                dia = "0" + dia;
            var mes = data.getMonth() + 1;
            if (mes.toString().length == 1)
                mes = "0" + mes;
            var ano = data.getFullYear();
            return dia + "/" + mes + "/" + ano;
        }

        /**
         * @function Configuração da lista
         * @name configLists
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
        function configLists() {

            $scope.list = {
                displayed: [
                    {
                        "Description": "",
                        "DRE": {
                            "Id": "",
                            "Description": ""
                        },
                        "School": {
                            "Id": 0,
                            "Description": ""
                        },
                        "Team": {
                            "Id": 0,
                            "Description": ""
                        },
                        "Student": {
                            "Id": 0,
                            "Description": ""
                        },
                        "CreateData": "",
                        "Processing": 0
                    }
                ]
            };
        };

        /**
         * @function Inicialização das informações da prova
         * @name configTestInformation
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
        function configTestInformation(informations) {

            $scope.testInformation = {
                test: informations.test,
                team: informations.team,
                school: informations.school,
                batch: informations.batch,
                blockAccess: informations.blockAccess
            };

            if ($scope.testInformation.batch != null) {
                $scope.batch = $scope.testInformation.batch;
                $scope.identificationType = $scope.testInformation.batch.BatchType;
            }

            if ($scope.identificationType > 0)
                $scope.identificationTypeLook = true;
        };

        $scope.setIdentificationTypeValue = function _setIdentificationTypeValue(id) {
            $scope.identificationType = id;
        };

        $scope.alertSelectIdentificationType = function _alertSelectIdentificationType() {

            if (!$scope.blockProcessing)
                $notification.alert("Selecione um tipo de identificação.");
            else
                $notification.alert("Lote em processamento, por favor aguarde a finalização.");
        };

        $scope.activeModalConfirm = function () {
            angular.element("#initModal").modal({ backdrop: 'static' });
        };

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name init
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @public
		 * @param {object} authorize
		 * @return
		 */
        $scope.init = function __init() {
            $scope.params = $util.getUrlParams();
            if ($scope.params.test_id != "") {
                getAuthorize();
            } else {
                $scope.blockPage = true;
                $notification.alert("Id da prova inválido ou usuário sem permissão.");
                $scope.safeApply();
                redirectIdIvalid();
                return;
            }
        };

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
        function getAuthorize(_callback) {

            if ($scope.params.test_id) {
                AnswerSheetModel.getAuthorize($scope.params, function (result) {
                    if (result.success) {
                        checkInfoAturorize(result.dados);
                    } else {
                        $scope.blockPage = true;
                        $notification[result.type ? result.type : 'error'](result.message);
                        $scope.safeApply();
                        redirectIdIvalid();
                        return;
                    }
                    if (_callback) _callback();
                });
            } else {
                checkInfoAturorize();
            }
        };

        function redirectIdIvalid() {
            $timeout(function __invalidTestId() {
                $window.history.back();
            }, 3000);
        };

        /**
		 * @function  Configuração inicial para configuração da page.
		 * @name configStart
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @private
		 * @param {object} authorize
		 * @return
		 */
        function configStart() {

            $scope.batch = null;
            $scope.QRCode = false;
            $scope.batchInfo = null;
            $scope.blockPage = false;
            $scope.modelFile = { "Guid": $util.getGuid() };
            $scope.blockUploadLook = false;
            $scope.identificationType = 0;
            $scope.identificationTypeLook = false;
            $scope.blockProcessing = false;
            $scope.processingFilter = [];
            $scope.dateUpload = true;
            $scope.uploading = false;
            $scope.countProcessing = 0;
            $scope.notificationFilter = 0;
            $scope.STUDENT_NUMBER_ID = Parameters.Test.STUDENT_NUMBER_ID == "True" ? true : false;
            $scope.listextensions = [];
            var parametroGabarito = getParameterValue(parameterKeys[0].DELETE_BATCH_FILES);
            $scope.baixarGabarito = parametroGabarito ? $.parseJSON(parametroGabarito.toLowerCase()) : false;
            $scope.inicializacaoPagina = true;

            if (!$scope.STUDENT_NUMBER_ID) {
                $scope.identificationType = 1;
            }
            configFiltersUpload();

            $scope.$watchCollection("[filters.DRE, filters.School, filters.StartDate, filters.EndDate, notificationFilter, processingFilter]",
                 function () {
                     $scope.countFilter = $scope.countFiltersSelected();
                 }, true);

            clearUploadQueurPagination();
            //getUploadQueueStatus();

        };

        function checkInfoAturorize(authorize) {

            $scope.authorize = authorize;
            //lote geral
            $scope.generalBatch = authorize ? true : false;

            if (authorize || $scope.params.test_id) {
                if (authorize) {
                    $scope.identificationLook = authorize.team.Id
                    if (checkParamtersUrl(authorize)) {

                        configTestInformation(authorize);

                        var test_id = ($scope.testInformation.test.Id === undefined || $scope.testInformation.test.Id === null || !parseInt($scope.testInformation.test.Id) ? 0 : $scope.testInformation.test.Id);

                        if (test_id === 0) {
                            $scope.blockPage = true;
                            $notification.alert("Id da prova inválido ou usuário sem permissão.");
                            $scope.safeApply();
                            redirectToList(3000);
                            return;
                        }

                        if ($scope.testInformation.blockAccess) {
                            $scope.blockPage = true;
                            $notification.alert("Usuário não possui permissão para realizar essa ação.");
                            redirectToList(3000);
                            return;
                        }

                        configStart();
                        startMethodPage();
                        getDREs();
                    }
                    else {
                        $notification.alert("Não foi possível obter os dados de acesso a página.");
                    }
                } else {
                    $notification.alert("Não foi possível obter os dados de acesso a página.");
                    setTimeout(function () { $window.history.back(); }, 3000);
                }

            } else {
                configStart();
                $scope.lookUpload = true;
                startMethodPage();
                getDREsNoId();
                $scope.countFiltersSelected();
            }
        };

        /**
         * @function Faz a chamada dos métodos iniciais da tela
         * @name startMethodPage 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param 
         * @return
         */
        function startMethodPage() {
            configListFilter();
            configFilters();
            configLists();
            $scope.getSituationList();
            getParametersUploadFile();
            getSituacaoLote();
        };

        /**
         * @function 
         * @name periodSelected 
         * @namespace BatchesController
         * @memberOf Controller
         * @private
         * @param 
         * @return
         */
        $scope.periodSelected = function __periodSelected() {

            if (checkValueDate()) {

                if ($scope.filters.StartDate.value && $scope.filters.EndDate.value) {
                    $scope.dateUpload = false;
                } else if (($scope.processingFilter.length - $scope.countProcessing) >= 1) {
                    $scope.dateUpload = true;
                }
            }
        };

        /**
         * @function valida data inicial não podendo do ser maior que a data final
         * @name checkValueDate 
         * @namespace BatchesController
         * @memberOf Controller
         * @private
         * @param 
         * @return
         */
        function checkValueDate() {

            if ($util.greaterEndDateThanStartDate($scope.filters.StartDate.value, $scope.filters.EndDate.value) === false) {
                $notification.alert("Data inicial não pode ser maior que a data final");
                $scope.filters.EndDate.value = "";
                if (($scope.processingFilter.length - $scope.countProcessing) >= 1)
                    $scope.dateUpload = true;
                return false;
            }

            return true;
        };

        /**
         * @function 
         * @name setFilterProcessing 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param 
         * @return
         */
        $scope.setFilterProcessing = function __setFilterProcessing(processing) {

            if (processing.state) {
                $scope.processingFilter.push({ Id: processing.Id, state: false });

                // id das situações que não precisa de períodos selecionados
                if (processing.Id !== 7 && processing.Id !== 1) {

                    var i, max = $scope.processingFilter.length;

                    for (i = 0; i < max; i++) {

                        //bloqueando a pesquisa caso não estiver periodo selecionado e id da situação dor diferente de 7 e 8
                        if ((max - $scope.countProcessing) >= 1 && (!$scope.filters.StartDate.value || !$scope.filters.EndDate.value)) {
                            $scope.dateUpload = true;
                            //desbloqueando a pesquisa
                        } else if (($scope.filters.StartDate.value && $scope.filters.EndDate.value)) {
                            $scope.dateUpload = false;
                        }
                    }//for i
                } else {
                    $scope.countProcessing++;
                    //quando se tem ao menos uma situação selecionada libera a pesquisa
                    if ($scope.processingFilter.length == 1) {
                        $scope.dateUpload = false;
                    }
                }

            } else {

                if (processing.Id == 7 || processing.Id == 1) {
                    $scope.countProcessing--;
                }

                var i, max = $scope.processingFilter.length;

                for (i = 0; i < max; i++) {
                    //procurando o id igual no array para exclui-lo
                    if ($scope.processingFilter[i].Id === processing.Id) {
                        $scope.processingFilter.splice(i, 1);

                        if (($scope.processingFilter.length - $scope.countProcessing) < 1) {
                            $scope.dateUpload = false;
                        }

                        break;
                    }
                }//for i

            }//else
            $scope.countFiltersSelected();
        };

        $scope.openModalngGeneralBatch = function __openModalngGeneralBatch() {
            angular.element("#modalGeneralBatch").modal({ backdrop: 'static' });
        };

        /**
         * @function bloqueia o upload do lote na modal
         * @name blockUpload 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param 
         * @return
         */
        $scope.blockUpload = function __blockUpload() {

            if ($scope.filtersUpload.DRE && $scope.filtersUpload.School) {
                $scope.blockUploadLook = true;
            } else {
                $scope.blockUploadLook = false;
            }

        };

        $scope.alertUpload = function __alertUpload() {
            if (!$scope.filtersUpload.DRE)
                $notification.alert("Selecione uma DRE.");
            else
                $notification.alert("Selecione uma escola.");
        };

        /**
         * @function Busca todos tipos de arquivos permitidos para realizar upload
         * @name getParametersUploadFile 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param {Object} authorize
         * @return
         */
        function getParametersUploadFile() {
            ParameterModel.getParametersUploadFile(function (result) {
                if (result.success) {
                    $scope.listextensions = result.lista;
                } else if (result.message != "") {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function getSituacaoLote() {
            AnswerSheetModel.getSituationLot(function (result) {
                if (result.success) {
                    $scope.listFilter.SituationLot = result.lista;
                    $scope.listFilterUpload.SituationLot = result.lista
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function checkParamtersUrl
         * @name 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param {Object} authorize
         * @return
         */
        function checkParamtersUrl(authorize) {

            if ((authorize.test.Id != 0 && authorize.school.Id != 0) ||
				(authorize.test.Id != 0 && authorize.school.Id == 0) ||
				(authorize.test.Id != 0 && authorize.team.Id != 0) ||
				(authorize.test.Id != 0 && authorize.team.Id == 0) ||
				(authorize.test.Id != 0 && authorize.school.Id != 0 && authorize.team.Id != 0) ||
				(authorize.batch.Id != 0)) {
                $scope.lookUpload = true;
                return true;
            }//if

            $scope.lookUpload = false;
            return false;
        };//checkParamtersUrl


        /**
         * @function Datepicker
         * @name 
         * @namespace BatchesController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.datepicker = function (id) {
            $("#" + id).datepicker('show');
        };

        /**
		 * @function Obter todos as opções de processamento
		 * @name getProcessingList
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.getSituationList = function __getSituationList() {

            AnswerSheetModel.getSituationList(function (result) {
                if (result.success) {
                    $scope.listFilter.Processing = setSituationOrder(result.lista);

                    if (!$scope.generalBatch) {
                        $scope.listFilter.Processing[4].state = true;
                        $scope.setFilterProcessing($scope.listFilter.Processing[4]);
                    } else {
                        $scope.listFilter.Processing[0].state = true;
                        $scope.setFilterProcessing($scope.listFilter.Processing[0]);
                    }
                    $scope.setPagination();
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Ordem de exibição das situações
         * @author julio.silva@mstech.com.br
         * @param
         * returns {Array}
         */
        function setSituationOrder(data) {

            for (var a = 0; a < data.length; a++) {
                if (data[a].Id === 7) data[a].order = 1; // :"Na fila para identificação"
                if (data[a].Id === 1) data[a].order = 2; // :"Aguardando correção"
                if (data[a].Id === 4) data[a].order = 3; // :"Sucesso"
                if (data[a].Id === 6) data[a].order = 4; // :"Conferir"
                if (data[a].Id === 5) data[a].order = 5; // :"Erro na correção"
                if (data[a].Id === 8) data[a].order = 6; // :"Erro na identificação"
            }
            return data;
        };

        /**
		 * @function Obter todos as opções de processamento
		 * @name getProcessingList
		 * @namespace BatchesController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.findBatch = function __findBatch() {

            getFilter();

            var entity = {
                BatchId: ($scope.batch !== undefined && $scope.batch !== null) ? $scope.batch.Id : 0,
                TestId: $scope.currentFilter.TestId,
                SupAdmUnitId: $scope.currentFilter.SupAdmUnitId,
                SchoolId: ($scope.currentFilter.SchoolId > 0 ? $scope.currentFilter.SchoolId : null),
                SectionId: ($scope.currentFilter.SectionId > 0 ? $scope.currentFilter.SectionId : null),
                CurriculumTypeId: $scope.filters.Year ? $scope.filters.Year.CurriculumTypeId : null,
                ShiftTypeId: $scope.filters.Turn ? $scope.filters.Turn.Id : null,
                StartDate: $scope.filters.StartDate.value,
                EndDate: $scope.filters.EndDate.value,
                Processing: getValuesProcessingFilter(),
                FilterDateUpdate: $scope.filters.FilterDateUpdate.value,
                BatchQueueId: ($scope.filters.BatchQueueFile && $scope.filters.BatchQueueFile.value) ? $scope.filters.BatchQueueFile.value.Id : 0,
                AluNome: $scope.searchFieldNomeAluno,
                Turma: $scope.searchFieldTurma
            };


            AnswerSheetModel.findBatch(entity, function (result) {
                setBatchData(result);
            });
        };

        function setBatchData(result) {
            if (result.success) {
                if (result.entity != null) {
                    $scope.batch = result.entity;
                    $scope.batchInfo = result.batch;
                    $scope.QRCode = result.QRCode;
                    $scope.identificationType = result.entity.BatchType;
                    $scope.blockProcessing = result.blockUpload;

                    if ($scope.identificationType > 0)
                        $scope.identificationTypeLook = true;

                } else {

                    if ($scope.authorize.team.id == undefined && $scope.authorize.school.Id == 0)
                        $scope.identificationType = 1;

                    else if ($scope.authorize.school.Id != 0 && $scope.authorize.team.Id == 0) {
                        $scope.identificationTypeLook = true;
                        $scope.identificationType = 3;
                    }
                }
            }
            else {
                $notification[result.type ? result.type : 'error'](result.message);
            }
        };

        /**
        * @function Limpeza dos filtros de pesquisa.
        * @name clearFilters
        * @namespace BatchDetailsController
        * @memberOf Controller
        * @public
        * @param
        * @return
        */
        $scope.clearFilters = function __clearFilters() {
            $scope.listFilter.StartDate = "";
            $scope.listFilter.EndDate = "";
            $scope.listFilter.Turns = [];
            $scope.listFilter.Years = [];
            $scope.listFilter.Schools = []
            $scope.filters.Processing = [];
            $scope.listFilter.SituationLot = []
            $scope.processingFilter = [];
            $scope.countProcessing = 0;
            $scope.dateUpload = false;


            // reseta o status de selecionado 
            var i, max = $scope.listFilter.Processing.length;
            for (i = 0; i < max; i++) {
                if ($scope.listFilter.Processing[i] !== $scope.listFilter.Processing[4])
                    $scope.listFilter.Processing[i].state = false;
                else $scope.listFilter.Processing[i].state = true;
            }//for i 

            $scope.setFilterProcessing($scope.listFilter.Processing[4]);
            configFilters();
        };


        /**
		 * @function Set Paginação
		 * @name setPagination
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.setPagination = function __setPagination(origemPesquisaTela) {

            // :validação - deve selecionar pelo menos 1 situação
            if ($scope.processingFilter.length === 0) {
                $notification.alert("Selecione uma situação.");
                return;
            }

            if (!$scope.dateUpload) {
                activateFilters();
                clearPagination();
                if (origemPesquisaTela) {
                    $scope.filters.topFiltro = undefined;
                    $scope.inicializacaoPagina = false;
                    $scope.ShowAllStudentsLot = false;
                }
                $scope.findBatch();
                $scope.search();
            }
            else {
                $notification.alert("Selecione um período.");
            }
        };

        /**
		 * @function Limpar Paginação
		 * @name clearPagination
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        function clearPagination() {

            $scope.paginate.indexPage(0);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.pages = 0;
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.totalItens = 0;
            getFilter();
            $scope.currentFilter;
        };

        function activateFilters() {
            $scope.filters.BatchQueueFile.state = ($scope.filters.BatchQueueFile.value ? true : false),
            $scope.filters.DRE.state = ($scope.filters.DRE.value ? true : false),
            $scope.filters.School.state = ($scope.filters.School.value ? true : false),
            $scope.filters.SituationLot.state = ($scope.filters.SituationLot.value && !$scope.exibirAnswerSheetBatchFiles ? true : false),
            $scope.filters.StartDate.state = ($scope.filters.StartDate.value ? true : false),
            $scope.filters.EndDate.state = ($scope.filters.EndDate.value ? true : false)

            for (var x = 0; x < $scope.processingFilter.length; x++) {
                $scope.processingFilter[x].state = true;
            }

            $scope.countFiltersSelected();
        }

        $scope.openStudentsResponses = function openStudentsResponses(test_id, team_id) {
            window.open("/Correction/IndexForm?test_id=" + test_id + '&team_id=' + team_id, "_blank");
        }

        $scope.filtra = function filtra(searchFieldNomeAluno, searchFieldTurma) {
            $scope.paginate.indexPage(0);
            $scope.currentFilter.AluNome = searchFieldNomeAluno;
            $scope.currentFilter.Turma = searchFieldTurma;
            $scope.search();
        }

        /**
		 * @function Pesquisa.
		 * @name search
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.search = function __search() {

            if ($scope.exibirAnswerSheetBatchFiles) {
                $scope.paginate.paginate($scope.currentFilter).then(function (result) {

                    if (result.success) {

                        if (result.lista.length > 0) {

                            $scope.paginate.nextPage();

                            $scope.list.displayed = angular.copy(result.lista);
                            $scope.filtredDateUpdate = result.filterDateUpdate;

                            if (!$scope.pages > 0) {
                                $scope.pages = $scope.paginate.totalPages();
                                $scope.totalItens = $scope.paginate.totalItens();
                            }
                        } else {
                            $scope.message = true;
                            $scope.list.displayed = null;
                            $scope.pages = 0;
                            $scope.totalItens = 0;
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }

                }, function () {
                    $scope.message = true;
                    $scope.list.displayed = null;
                });
            }
            else {
                clearUploadQueurPagination();
                if ($scope.inicializacaoPagina) {
                    $scope.filters.topFiltro = 10;
                }
                getUploadQueueStatus();
            }
        };

        /**
         * @function situação do processo de uploading
         * @param {boolean} state
         * @return
         */
        $scope.uploadingState = function __uploadingState(state) {
            $scope.uploading = state;
        };

        /**
         * @function File upload.
         * @param {object} data
         * @return
         */
        $scope.fileUploadSuccess = function __fileUploadSuccess(data) {
            if (data == undefined || data == null) return;
            angular.element(".loading-box").removeClass('ng-hide');
            $timeout(function () {
                angular.element('#' + $scope.modelFile.Guid).css('width', '0%');
                $scope.modelFile.Path = "";
            }, 2000);
            createBatch($scope.modelFile.Id);
        };


        /**
         * @function Processar.
         * @name sendToProcess
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.sendToProcess = function __sendToProcess() {

            angular.element('#initModal').modal('hide');

            getFilter();
            var filter = $scope.currentFilter;

            if ($scope.currentFilter.BatchId > 0)
                filter.BatchId = $scope.currentFilter.BatchId;
            else
                filter.BatchId = ($scope.batch !== undefined && $scope.batch !== null) ? $scope.batch.Id : 0;

            AnswerSheetModel.sendToProcessing(filter, function (result) {

                if (result.success) {

                    $notification.success(result.message);

                    $scope.findBatch();
                    $scope.setPagination();
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };


        /**
         * @function Download de erros.
         * @name exportErros
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.exportErros = function __exportErros() {

            getFilter();
            var filter = $scope.currentFilter;

            AnswerSheetModel.exportAnswerSheetData(filter, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };


        /**
         * @function Cancel.
         * @name cancel
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.cancel = function __cancel() {
            redirectToList(0);
        };

        /**
		 * @function Obter todas DREs
		 * @name getDREs
		 * @namespace BatchDetailsController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        function getDREs() {

            var params = { 'testId': $scope.testInformation.test.Id };

            AdherenceModel.getAdheredDreSimple(params, function (result) {

                if (result.success) {
                    $scope.listFilter.DREs = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		* @function Obter todas DREs
		* @name getDREs
		* @namespace BatchDetailsController
		* @memberOf Controller
		* @private
		* @param
		* @return
		*/
        function getDREsNoId() {

            AdherenceModel.getDRESimple({}, function (result) {

                if (result.success) {
                    $scope.listFilter.DREs = result.lista;
                    $scope.listFilterUpload.DREs = result.lista
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.openModalShool = function () {
            angular.element("#modalLote").modal({ backdrop: 'static' });
        };

        /**
         * @function Obter todas Escolas
         * @name getSchools
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getSchools = function __getSchools() {

            if (!$scope.filters.DRE.value)
                return;

            if ($scope.testInformation) {
                var params = { 'testId': $scope.testInformation.test.Id, 'dre_id': $scope.filters.DRE.value.Id };

                AdherenceModel.getAdheredSchoolSimple(params, function (result) {

                    if (result.success) {
                        $scope.listFilter.Schools = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            } else {

                var params = { 'dre_id': $scope.filters.DRE.value.Id };

                AdherenceModel.getSchoolsSimple(params, function (result) {

                    if (result.success) {
                        $scope.listFilter.Schools = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        /**
         * @function Obter todas Escolas sem a necessidade de passar o testId
         * @name getSchoolsNotTestId
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getSchoolsNotTestId = function __getSchoolsNotTestId() {

            var params = { 'dre_id': $scope.filtersUpload.DRE.Id };

            AdherenceModel.getSchoolsSimple(params, function (result) {

                if (result.success) {
                    $scope.listFilterUpload.Schools = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.getYears = function __getYears() {

            if ($scope.filters.School.value === undefined || $scope.filters.School.value === null)
                return;

            TestListModel.getCurriculumGradeSimple({ 'esc_id': $scope.filters.School.value.Id }, function (result) {

                if (result.success) {
                    $scope.listFilter.Years = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }

        $scope.getTurns = function __getTurns() {

            if ($scope.filters.School.value === undefined || $scope.filters.School.value === null)
                return;

            AdherenceModel.getShiftSimple({ 'esc_id': $scope.filters.School.value.Id }, function (result) {

                if (result.success) {
                    $scope.listFilter.Turns = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Obter todos os lotes da  Escolas
         * @name getSchools
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getBatchSchool = function __getBatchSchool() {

            if ($scope.filters.School.value !== undefined) {
                angular.element('#modalLote').modal('hide');
                $scope.testInformation.school = $scope.filters.School.value;
                $scope.setPagination();
            } else {
                $notification.alert("Selecione uma escola.");
            }
        };

        /**
         * @function Obter todos os lotes da  Escolas
         * @name getSchools
         * @namespace BatchDetailsController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getBatchGeneral = function __getBatchSchool() {
            $scope.filters.DRE.value = undefined;
            $scope.filters.School.value = undefined;
            $scope.testInformation.school.Id = 0;
            $scope.testInformation.school.Description = null;
            $scope.setPagination();
        };

        $scope.downloadFile = function __downloadFile(path) {
            FileModel.checkFilePathExists({ path: path }, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFilePath?path=" + path, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'alert'](result.message);
                }
            });
        };

        $scope.downloadFileById = function __downloadFileById(fileId) {
            FileModel.checkFileExists({ Id: fileId }, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + fileId, "_self");
                }
                else {
                    $notification.alert("Arquivo não encontrado.");
                }
            });
        }

        /**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace BatchDetailsController
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

        $interval(function () {
            getFilter();
            var entity = {
                BatchId: ($scope.batch !== undefined && $scope.batch !== null) ? $scope.batch.Id : 0,
                TestId: $scope.currentFilter.TestId,
                SupAdmUnitId: $scope.currentFilter.SupAdmUnitId,
                SchoolId: ($scope.currentFilter.SchoolId > 0 ? $scope.currentFilter.SchoolId : null),
                SituationLot: ($scope.currentFilter.Situation > 0 ? $scope.currentFilter.Situation : null),
                SectionId: ($scope.currentFilter.SectionId > 0 ? $scope.currentFilter.SectionId : null),
                CurriculumTypeId: $scope.filters.Year ? $scope.filters.Year.CurriculumTypeId : null,
                ShiftTypeId: $scope.filters.Turn ? $scope.filters.Turn.Id : null,
                StartDate: $scope.filters.StartDate.value,
                EndDate: $scope.filters.EndDate.value,
                Processing: getValuesProcessingFilter(),
                FilterDateUpdate: $scope.filters.FilterDateUpdate.value,
                BatchQueueId: ($scope.filters.BatchQueueFile && $scope.filters.BatchQueueFile.value) ? $scope.filters.BatchQueueFile.value.Id : 0
            };
            $.ajax({
                url: base_url('AnswerSheet/FindBatch'),
                data: entity,
                error: function (result) {
                    console.log("erro no interval de atualização de batch details header");
                },
                dataType: 'json',
                success: function (result) {
                    setBatchData(result);
                },
                type: 'GET'
            });

        }, 60000);
    };

})(angular, jQuery);