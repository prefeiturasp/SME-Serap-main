(function (angular, $) {

    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
        .module('appMain')
        .controller("ReportItemPerformanceDREController", ReportItemPerformanceDREController);

    ReportItemPerformanceDREController.$inject = ['$rootScope', '$scope', 'ReportItemPerformanceModel', 'ReportPerformanceModel', 'AdherenceModel', 'TestModel', 'DisciplineModel', 'EvaluationMatrixModel', 'SkillModel', '$notification', '$timeout', '$pager', '$window', '$util', '$sce'];


    function ReportItemPerformanceDREController($rootScope, $scope, ReportItemPerformanceModel, ReportPerformanceModel, AdherenceModel, TestModel, DisciplineModel, EvaluationMatrixModel, SkillModel, $notification, $timeout, $pager, $window, $util, $sce) {

        /**
        * @function Responsavél por instanciar tds as variaveis
        * @param 
        * @returns
        */
        function configVariables() {
            $scope.WARNING_UPLOAD_BATCH_DETAIL = Boolean(Parameters.General.WARNING_UPLOAD_BATCH_DETAIL == "True");
            $scope.countFilter = 0;
            // :paginação
            //$scope.paginate = $pager(ReportItemPerformanceModel.getDres);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = null;
            $scope.listResultSME = null;
            $scope.listDREs = [];
            $scope.listTests = [];
            $scope.listGroups = [];
            $scope.listSubGroups = [];
            $scope.listCurriculumGrades = [];
            $scope.listTestsByGroup = [];
            $scope.listDiscipline = [];
            $scope.listSkills = [];
            $scope.filters.discipline_id = "";
            $scope.filters.discipline_name = "";
            $scope.filters.modelEixoHabilidade = {};
            $scope.filters.skill_id = 0;
            $scope.filters.matrix_id = "";
            $scope.filters.level = 1;
            $scope.filters.dre = {};
            $scope.filters.escola = {};
            $scope.filters.turma = {};
            $scope.filters.tests = [];
            $scope.filters.CurriculumGrade = {};
            $scope.oneDiscipline = false;
            $scope.chart1 = {
                visible: false,
                chart: {
                }
            };
            $scope.chart2 = {
                visible: false,
                chart: {
                }
            };
            $scope.chart3 = {
                visible: false,
                chart: {
                }
            };
            $scope.group = false;
            $scope.itemsOrderOptions = [{ id: 0, label: "Desempenho" }, { id: 1, label: "Alfabética " }]
            $scope.itemsGraphOrderOptions = [{ id: 0, label: "Desempenho" }, { id: 1, label: "Alfabética " }]
            $scope.itemsGraph2OrderOptions = [{ id: 0, label: "% de escolha" }, { id: 1, label: "Alfabética " }]
            $scope.performanceByList = [{ id: 1, label: "DRE" }, { id: 2, label: "Escola" }]
            $scope.selectedOrderItem = 0;
            $scope.orderItemByDesc = 0;
            $scope.testInformation = {
                selectedOrderGraph1: 0,
                selectedOrderGraph1ByDesc: 0,
                selectedOrderGraph2: 0,
                selectedOrderGraph2ByDesc: 0,
                performanceGraph1Init: 50,
                performanceGraph1End: 75,
                performanceGraph2Init: 50,
                performanceGraph2End: 75,
                performanceBy: 1
            }

            getGroups();
            getTests();
            getListDREs();
            $scope.$watch("filters", function () {
                $scope.countFilter = 0;
                if ($scope.filters.DateStart) $scope.countFilter += 1;
                if ($scope.filters.DateEnd) $scope.countFilter += 1;
                if ($scope.filters.test_id && !$scope.group) $scope.countFilter += 1;
                if ($scope.filters.Group && $scope.group) $scope.countFilter += 1;
                if ($scope.filters.SubGroup && $scope.group) $scope.countFilter += 1;
                if ($scope.filters.uad_id) $scope.countFilter += 1;
                if ($scope.filters.discipline_id) $scope.countFilter += 1;
                if ($scope.filters.discipline_name) $scope.countFilter += 1;
            }, true);
        };

        /**
         * @function obter detalhes de cabeçalho da page
         * @param {object} _callback
         * @returns
         */
        function getHeaderDetails(_callback) {
            if ($scope.group === true) {
                $scope.header = {};
                $scope.header.Description = $scope.filters.Group.Description;
                $scope.header.SubDescription = $scope.filters.SubGroup.Description
            }
            else {
                if (!$scope.filters.test_id) return;
                TestModel.getInfoTestReport({ Test_id: $scope.filters.test_id }, function (result) {
                    if (result.success) {
                        $scope.header = result.lista;
                    }
                    else {
                        $notification['error'](result.message);
                    }
                    if (_callback) _callback();
                });
            }
        };
        $scope.getHeaderDetails = getHeaderDetails;

        $("#testCode").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btFiltrar").click();
            }
        });

        function filterByDiscipline() {
            var arrItems = [];
            for (var x = 0; x < $scope.tree.disciplinas.length; x++) {
                if ($scope.filters.discipline_id === null || $scope.filters.discipline_id === undefined || $scope.filters.discipline_id === $scope.tree.disciplinas[x].id) {
                    arrItems = arrItems.concat($scope.tree.disciplinas[x].itens);
                }
            }

            configSliderDRE(arrItems);
        }
        $scope.filterByDiscipline = filterByDiscipline;


        function getItemsSlice() {
            $scope.tableDres = [];
            $scope.generalItems = {};
            var inicio = $scope.listaItens.length > 15 ? $scope.sliderDRE.current : 0;
            var fim = ($scope.sliderDRE.current + 14) > $scope.listaItens.length ? $scope.listaItens.length : ($scope.sliderDRE.current + 14);

            var arrItems = [];
            for (var x = 0; x < $scope.tree.disciplinas.length; x++) {
                if ($scope.filters.discipline_id === null || $scope.filters.discipline_id === undefined || $scope.filters.discipline_id === "" || $scope.filters.discipline_id === 0 || $scope.filters.discipline_id === $scope.tree.disciplinas[x].id) {
                    arrItems = arrItems.concat($scope.tree.disciplinas[x].itens);
                }
            }
            arrItems = bubbleSort(arrItems, 'order');

            $scope.generalItems = { name: $scope.tree.name, itens: arrItems.slice(inicio, fim) };

            for (var x = 0; x < $scope.tree.dres.length; x++) {
                var itens = [];
                for (var y = 0; y < $scope.tree.dres[x].disciplinas.length; y++) {
                    if ($scope.filters.discipline_id === null || $scope.filters.discipline_id === undefined || $scope.filters.discipline_id === "" || $scope.filters.discipline_id === 0 || $scope.filters.discipline_id === $scope.tree.dres[x].disciplinas[y].id) {
                        itens = itens.concat($scope.tree.dres[x].disciplinas[y].itens);
                    }
                }

                itens = bubbleSort(itens, 'order');
                $scope.tableDres.push({ id: $scope.tree.dres[x].id, name: $scope.tree.dres[x].name, itens: itens.slice(inicio, fim) });
            }
        };
        $scope.getItemsSlice = getItemsSlice;

        function bubbleSort(a, par) {
            var swapped;
            do {
                swapped = false;
                for (var i = 0; i < a.length - 1; i++) {
                    if (a[i][par] > a[i + 1][par]) {
                        var temp = a[i];
                        a[i] = a[i + 1];
                        a[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);

            return a;
        }

        function curriculumGradeChange() {
            $scope.filters.dre = null;
            $scope.filters.escola = null;
            $scope.filters.test_id = undefined;
            getDres(getDisciplines);
        }
        $scope.CurriculumGradeChange = curriculumGradeChange;

        function testChange() {
            $scope.filters.dre = null;
            $scope.filters.escola = null;
            getDres(getDisciplines);
        }
        $scope.TestChange = testChange;

        function clearTest() {
            $scope.filters.test_id = undefined;
        }
        $scope.ClearTest = clearTest;

        function clearGroup() {
            $scope.filters.SubGroup = undefined;
        }
        $scope.ClearGroup = clearGroup;

        /**
         * @function Obter todas as DREs
         * @param {object} _callback
         * @returns
         */
        function getDres(_callback) {
            var params = {
                test_id: $scope.filters.test_id ? $scope.filters.test_id : 0,  //,
                subGroup_id: $scope.group === true ? $scope.filters.SubGroup.Id : 0,
                tcp_id: ($scope.filters.CurriculumGrade && $scope.filters.CurriculumGrade.tcp_id > 0) ? $scope.filters.CurriculumGrade.tcp_id : 0,
                dre_id: $scope.useFilter ? ($scope.filters.uad_id ? $scope.filters.uad_id : null) : ($scope.filters.dre ? $scope.filters.dre.id : null),
                esc_id: ($scope.filters.escola && !$scope.useFilter) ? $scope.filters.escola.id : null,
                uad_id: $scope.filters.uad_id
            }
            $scope.useFilter = false;

            ReportItemPerformanceModel.getDres(params, function (result) {
                if (result.success && !result.data.erro) {
                    var popularTree = true;
                    if ($scope.tree != null && result.data.level === 3 && result.data.escolaSelecionada > 0) {
                        for (var d = 0; d < $scope.tree.dres.length; d++) {
                            for (var i = 0; i < $scope.tree.dres[d].escolas.length; i++) {
                                if ($scope.tree.dres[d].escolas[i].id == result.data.escolaSelecionada) {
                                    popularTree = false;
                                    $scope.tree.dres[d].escolas[i].turmas = result.data.dres[0].escolas[0].turmas;
                                    break;
                                }
                            }
                        }
                    }

                    if (popularTree) {
                        $scope.tree = result.data;

                        $scope.filters.level = result.data.level;
                        $scope.selectedItem = { 'discipline_id': 0, 'item_id': 0 };
                        $scope.listaItens = [];
                        //if (result.data.tests != null && result.data.tests.length > 0) {
                        $scope.filters.test_id = result.data.test_id;
                        if (result.data.tests && result.data.tests.length > 0)
                            $scope.listTestsByGroup = result.data.tests;
                        //}

                        if (result.data.dreSelecionada) {
                            for (var x = 0; x < result.data.dres.length; x++) {
                                if (result.data.dres[x].id == result.data.dreSelecionada) {
                                    $scope.filters.dre = result.data.dres[x];
                                    if (result.data.escolaSelecionada) {
                                        for (var y = 0; y < result.data.dres[x].escolas.length; y++) {
                                            if (result.data.escolaSelecionada == result.data.dres[x].escolas[y].id) {
                                                $scope.filters.escola = result.data.dres[x].escolas[y];
                                                for (var d = 0; d < $scope.filters.escola.disciplinas.length; d++) {
                                                    for (var i = 0; i < $scope.filters.escola.disciplinas[d].itens.length; i++) {
                                                        $scope.listaItens.push($scope.filters.escola.disciplinas[d].itens[i]);
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    } else {
                                        for (var d = 0; d < $scope.filters.dre.disciplinas.length; d++) {
                                            for (var i = 0; i < $scope.filters.dre.disciplinas[d].itens.length; i++) {
                                                $scope.listaItens.push($scope.filters.dre.disciplinas[d].itens[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            for (var x = 0; x < $scope.tree.disciplinas.length; x++) {
                                for (var y = 0; y < $scope.tree.disciplinas[x].itens.length; y++) {
                                    $scope.listaItens.push($scope.tree.disciplinas[x].itens[y]);
                                }
                            }
                        }
                    }

                    $scope.carregaGrafico_Desempenho();

                    configSliderDRE($scope.listaItens);
                    if (_callback) _callback();

                }
                else if (result.success && result.data.erro) {
                    $notification[result.type ? result.type : 'alert'](result.data.erro);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };
        $scope.GetDres = getDres;

        function getGroups() {
            $scope.filters.test_id = undefined;
            $scope.filters.discipline_id = undefined;
            ReportPerformanceModel.getGroups({}, function (result) {
                $scope.group = true;
                if (result.success) {
                    $scope.listGroups = result.lista;
                    if ($scope.Groups.length === 1) {
                        $scope.filters.Group = $scope.Groups[0];
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
                    $scope.listSubGroups = result.modelList.TestSubGroups;
                    if ($scope.listSubGroups.length === 1)
                        $scope.filters.SubGroup = $scope.listSubGroups[0];
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });

        };
        $scope.getSubGroup = getSubGroup;

        /**
        * @function Obter os períodos
        * @param {object} _callback
        * @returns
        */
        function getCurriculumGradeTests(_callback) {
            if ($scope.group === true) {
                ReportPerformanceModel.getDistinctCurricumGradeByTestSubGroup_Id({ TestSubGroup_Id: $scope.filters.SubGroup.Id }, function (result) {
                    if (result.success) {
                        $scope.listCurriculumGrades = result.lista;
                        if ($scope.listCurriculumGrades.length === 0)
                            $notification.alert("Nenhuma ano de aplicação foi encontrado.")
                        else {
                            $scope.filters.CurriculumGrade = $scope.listCurriculumGrades[0];
                            if (_callback) _callback();
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            else {
                ReportPerformanceModel.getCurriculumGradeByTestId({ Test_id: $scope.filters.test_id }, function (result) {
                    if (result.success) {
                        $scope.listCurriculumGrades = result.lista;

                        if ($scope.listCurriculumGrades.length === 0)
                            $notification.alert("Nenhuma ano de aplicação foi encontrado.")
                        else {
                            $scope.filters.CurriculumGrade = $scope.listCurriculumGrades[0];
                            if (_callback) _callback();
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }

        };
        $scope.getCurriculumGradeTests = getCurriculumGradeTests;

        /**
         * @function Obter todas DREs
         * @name getDREs
         * @namespace AdherenceController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
        function getListDREs() {

            AdherenceModel.getDRESimple({}, function (result) {

                if (result.success) {
                    $scope.listDREs = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
        * @function Obter as provas dado um período
        * @param {object} _callback
        * @returns
        */
        function getTests(_callback) {
            if ($scope.filters.DateStart && $scope.filters.DateEnd) {
                if (!validateDate()) return;
                TestModel.getTestByDate({ DateStart: $scope.filters.DateStart, DateEnd: $scope.filters.DateEnd }, function (result) {
                    if (result.success) {
                        $scope.listTests = result.lista;
                        if ($scope.listTests.length === 0)
                            $notification.alert("Nenhuma prova foi encontrada no período selecionado.")
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
                    $scope.filters.Tests = result.lista;
                    if ($scope.filters.Tests.length === 0)
                        $notification.alert("Nenhuma prova foi encontrada no período selecionado.")
                    else if ($scope.filters.Tests.length === 1)
                        $scope.filters.test_id = $scope.filters.Tests[0].TestId;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };
        $scope.getTestsBySubGroup = getTestsBySubGroup;

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

        /**
         * @function obter as disciplinas da prova
         * @param {object} _callback
         * @returns
         */
        function getDisciplines(_callback) {
            if (!$scope.filters.test_id) return;
            $timeout(function () {
                //DisciplineModel.loadComboByTest({ Test_id: $scope.filters.test_id }, function (result) {
                //    if (result.success) {
                $scope.listDiscipline = $scope.tree.disciplinas;
                $scope.filters.discipline_id = "";
                $scope.filters.discipline_name = "";
                $scope.oneDiscipline = false;

                if ($scope.listDiscipline.length == 1) {
                    $scope.filters.discipline_id = $scope.listDiscipline[0].id;
                    $scope.filters.discipline_name = $scope.listDiscipline[0].name;
                    $scope.oneDiscipline = true;
                }

                //if (_callback) _callback();
                //    }
                //    else {
                //        $notification[result.type ? result.type : 'error'](result.message);
                //    }
                //});
            });
        };
        $scope.getDisciplines = getDisciplines;

        /**
         * @function Carregar médias por item.
         * @name getAveragesByItemId **/

        function getAveragesByItemId(itemId) {
            var averages = { rightChoose: 0 };
            for (var x = 0; x < $scope.sme.length; x++) {
                if ($scope.sme[x].Item_id == itemId) {
                    averages.rightChoose = $scope.sme[x].Media;
                    break;
                }
            }
            return averages;
        }

        /**
         * @function - Carregar Controle de carregamento de skills
         * @param {string} s
         * @public
         */
        $scope.loaderSkillsController = function (s) {

            for (var i = 0; i < $scope.skills.length; i++) {

                if ($scope.skills[i].Id == s.Id) {
                    $scope.skills[i].visible = !$scope.skills[i].visible
                }
                else {
                    $scope.skills[i].visible = false;
                }
            }

            if (s.visible)
                $scope.carregaLastLevel(s)
        };

        /**
    * @function - Carrega ultimo nível da matriz
    * @param {Object} r
    * @public
    */
        $scope.carregaLastLevel = function (r) {

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

        $scope.filterBySkill = function () {
            var result = {};
            switch ($scope.filters.level) {
                case 1:
                    result = $scope.tree;
                    break;
                case 2:
                    result = $scope.filters.dre;
                    break;
                case 3:
                    result = $scope.filters.escola;
                    break;
            }

            for (var x = 0; x < result.disciplinas.length; x++) {
                for (var y = 0; y < result.disciplinas[x].itens.length; y++) {
                    for (var z = 0; z < result.disciplinas[x].itens[y].habilidades.length; z++) {
                        if (result.disciplinas[x].itens[y].habilidades[z].id == $scope.filters.skill_id || !$scope.filters.skill_id)
                            result.disciplinas[x].itens[y].hasSkill = true;
                        else
                            result.disciplinas[x].itens[y].hasSkill = false;
                    }
                }
            }
        }

        $scope.sorterByInt = function (item) {
            return parseInt(item.order);
        };

        $scope.abreZoom = function abreZoom() {
            $('.expandir').addClass('fixed-zoom');
            $('.expandir')[0].style.width = '1200px';
            $('.expandir')[0].style.left = '-150px';
            $('.divRelatorio')[0].style.width = '1165px';

            $scope.carregaGrafico_DesempenhoPorItem();
            $('.fecha-zoom').show();
        }

        $scope.fechaZoom = function fechaZoom() {
            $('.divRelatorio')[0].style.width = '850px';
            $('.expandir').removeClass('fixed-zoom');
            $('.expandir')[0].removeAttribute('style');
            $scope.carregaGrafico_DesempenhoPorItem();
            $('.fecha-zoom').hide();
        }

        //Seleciona o item para exibir as informações na modal
        $scope.findItemById = function (disciplineId, itemId) {
            $scope.itens = [];

            $scope.selectedItemModal = { item: {}, discipline_name: "", discipline_id: 0 };
            switch ($scope.filters.level) {
                case 1:
                    $scope.copyDisciplinas = angular.copy($scope.tree.disciplinas);
                    for (var x = 0; x < $scope.copyDisciplinas.length; x++) {
                        for (var y = 0; y < $scope.copyDisciplinas[x].itens.length; y++) {
                            $scope.itens.push(angular.copy($scope.copyDisciplinas[x].itens[y]));
                        }
                    }

                    for (var x = 0; x < $scope.tree.disciplinas.length; x++) {
                        if ($scope.tree.disciplinas[x].id == disciplineId) {
                            $scope.selectedItemModal.discipline_name = $scope.tree.disciplinas[x].name;
                            $scope.selectedItemModal.discipline_id = $scope.tree.disciplinas[x].id;
                            for (var y = 0; y < $scope.tree.disciplinas[x].itens.length; y++) {
                                if ($scope.tree.disciplinas[x].itens[y].id == itemId) {
                                    $scope.selectedItemModal.item = $scope.tree.disciplinas[x].itens[y];
                                    $scope.videoDestaque = $scope.tree.disciplinas[x].itens[y].videos[0];
                                    break;
                                }
                            }

                            break;
                        }
                    }
                    break;
                case 2:
                    for (var x = 0; x < $scope.filters.dre.disciplinas.length; x++) {
                        $scope.copyDisciplinas = angular.copy($scope.filters.dre.disciplinas);
                        $scope.itens.push(angular.copy($scope.copyDisciplinas[x].itens));
                    }

                    for (var x = 0; x < $scope.filters.dre.disciplinas.length; x++) {
                        if ($scope.filters.dre.disciplinas[x].id == disciplineId) {
                            $scope.selectedItemModal.discipline_name = $scope.filters.dre.disciplinas[x].name;
                            $scope.selectedItemModal.discipline_id = $scope.filters.dre.disciplinas[x].id;
                            for (var y = 0; y < $scope.filters.dre.disciplinas[x].itens.length; y++) {
                                if ($scope.filters.dre.disciplinas[x].itens[y].id == itemId) {
                                    $scope.selectedItemModal.item = $scope.filters.dre.disciplinas[x].itens[y];
                                    $scope.videoDestaque = $scope.filters.dre.disciplinas[x].itens[y].videos[0];
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                case 3:
                    for (var x = 0; x < $scope.filters.escola.disciplinas.length; x++) {
                        $scope.copyDisciplinas = angular.copy($scope.filters.escola.disciplinas);
                        $scope.itens.push(angular.copy($scope.copyDisciplinas[x].itens));
                    }

                    for (var x = 0; x < $scope.filters.escola.disciplinas.length; x++) {
                        if ($scope.filters.escola.disciplinas[x].id == disciplineId) {
                            $scope.selectedItemModal.discipline_name = $scope.filters.escola.disciplinas[x].name;
                            $scope.selectedItemModal.discipline_id = $scope.tree.disciplinas[x].id;
                            for (var y = 0; y < $scope.filters.escola.disciplinas[x].itens.length; y++) {
                                if ($scope.filters.escola.disciplinas[x].itens[y].id == itemId) {
                                    $scope.selectedItemModal.item = $scope.filters.escola.disciplinas[x].itens[y];
                                    $scope.videoDestaque = $scope.filters.escola.disciplinas[x].itens[y].videos[0];
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                case 4:
                    for (var x = 0; x < $scope.filters.turma.disciplinas.length; x++) {
                        $scope.copyDisciplinas = angular.copy($scope.filters.turma.disciplinas);
                        $scope.itens.push(angular.copy($scope.copyDisciplinas[x].itens));
                    }

                    for (var x = 0; x < $scope.filters.turma.disciplinas.length; x++) {
                        if ($scope.filters.turma.disciplinas[x].id == disciplineId) {
                            $scope.selectedItemModal.discipline_name = $scope.filters.turma.disciplinas[x].name;
                            $scope.selectedItemModal.discipline_id = $scope.tree.disciplinas[x].id;
                            for (var y = 0; y < $scope.filters.turma.disciplinas[x].itens.length; y++) {
                                if ($scope.filters.turma.disciplinas[x].itens[y].id == itemId) {
                                    $scope.selectedItemModal.item = $scope.filters.turma.disciplinas[x].itens[y];
                                    $scope.videoDestaque = $scope.filters.turma.disciplinas[x].itens[y].videos[0];
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
            }

            $scope.alternativeGraph = undefined;
            $scope.carregaGrafico_DesempenhoPorItem();
            $scope.carregaGrafico_EscolhaItens();
            getBaseText(itemId);
        }

        $scope.proximaQuestao = function (selectedItemModal) {
            $scope.selectedItemModal = { item: {}, discipline_name: "", discipline_id: 0 };

            $scope.itens.sort(function (a, b) {
                return a.order < b.order ? -1 : a.order > b.order ? 1 : 0;
            });

            var index = -1;
            for (var i = 0, len = $scope.itens.length; i < len; i++) {
                if ($scope.itens[i].id === selectedItemModal.id) {
                    index = i;
                    break;
                }
            }

            if (index + 1 != $scope.itens.length) {
                $scope.selectedItemModal.item = $scope.itens[index + 1];
                $scope.selectedItemModal.discipline_name = $scope.itens[index + 1].discipline_name;
                $scope.selectedItemModal.discipline_id = $scope.itens[index + 1].discipline_id;
                $scope.videoDestaque = $scope.itens[index + 1].videos[0];
            }
            else {
                $scope.selectedItemModal.item = $scope.itens[index];
                $scope.selectedItemModal.discipline_name = $scope.itens[index].discipline_name;
                $scope.selectedItemModal.discipline_id = $scope.itens[index].discipline_id;
                $scope.videoDestaque = $scope.itens[index].videos[0];
                $notification.alert("Não existem mais itens na prova.");
            }

            $scope.alternativeGraph = undefined;
            $scope.carregaGrafico_DesempenhoPorItem();
            $scope.carregaGrafico_EscolhaItens();
            getBaseText(selectedItemModal.id);
        }

        function getBaseText (id) {
            var params = {
                itemId: id
            }
            if (!$scope.selectedItemModal.item.baseText) {
                ReportItemPerformanceModel.getBaseTextByItemId(params, function (result) {
                    if (result.success && !result.data.erro) {
                        $scope.selectedItemModal.item.baseText = result.data;
                    }
                });
            }
        }

        $scope.voltaQuestao = function (selectedItemModal) {
            $scope.selectedItemModal = { item: {}, discipline_name: "", discipline_id: 0 };

            $scope.itens.sort(function (a, b) {
                return a.order < b.order ? -1 : a.order > b.order ? 1 : 0;
            });

            var index = -1;
            for (var i = 0, len = $scope.itens.length; i < len; i++) {
                if ($scope.itens[i].id === selectedItemModal.id) {
                    index = i;
                    break;
                }
            }

            if (index - 1 >= 0) {
                $scope.selectedItemModal.item = $scope.itens[index - 1];
                $scope.selectedItemModal.discipline_name = $scope.itens[index - 1].discipline_name;
                $scope.selectedItemModal.discipline_id = $scope.itens[index - 1].discipline_id;
                $scope.videoDestaque = $scope.itens[index - 1].videos[0];
            }
            else {
                $scope.selectedItemModal.item = $scope.itens[index];
                $scope.selectedItemModal.discipline_name = $scope.itens[index].discipline_name;
                $scope.selectedItemModal.discipline_id = $scope.itens[index].discipline_id;
                $scope.videoDestaque = $scope.itens[index].videos[0];
                $notification.alert("Não existem itens anteriores na prova.");
            }

            $scope.alternativeGraph = undefined;
            $scope.carregaGrafico_DesempenhoPorItem();
            $scope.carregaGrafico_EscolhaItens();
            getBaseText(selectedItemModal.id);
        }

        function changeTest(_callback) {
            if ($scope.filters.test_id > 0 && $('#selectTest').val() != "") {
                $('#testCode').val($scope.filters.test_id)
            }

            if (_callback) _callback();
        };
        $scope.changeTest = changeTest;

        function changeTestCode(_callback) {
            if ($scope.filters.test_id > 0 && $('#testCode').val() != "") {
                $('#selectTest').val($scope.filters.test_id)
            }

            if (_callback) _callback();
        };
        $scope.changeTestCode = changeTestCode;

        //Seleciona a cor da barrinha conforme o progresso
        $scope.getProgressColor = function (value) {

            if (value < 25)
                return 'cor1';
            if (value < 50)
                return 'cor2';
            if (value < 75)
                return 'cor3';
            else
                return 'cor4';

            //$scope.selectedItemModal.averages = 
        }

        $scope.carregaHabilidades = function __carregaHabilidades() {

            $scope.skills = [];
            if ($scope.filters.discipline_id > 0) {
                SkillModel.getComboByDiscipline({ Id: $scope.filters.discipline_id }, function (result) {

                    if (result.success) {
                        $scope.listSkills = result.lista;
                        $scope.selectItem($scope.filters.discipline_id, 0);
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);

                    }
                });
            }
            else
                $scope.listSkills = [];
        };

        /*
          @function Selecionar item para o gráfico 
        */
        $scope.selectItem = function selectItem(discipline_id, item_id) {

            $scope.selectedItem = { 'discipline_id': discipline_id, 'item_id': item_id };
            $scope.carregaGrafico_Desempenho();
        };

        function compareByProperty(a, b) {
            if (a[a.propertyName] < b[b.propertyName])
                return -1;
            if (a[a.propertyName] > b[b.propertyName])
                return 1;
            return 0;
        }

        /*
          @function Selecionar item para o gráfico 
        */
        $scope.getDisciplinesByLevel = function () {
            if ($scope.tree)
                switch ($scope.filters.level) {
                    case 1:
                        return $scope.tree.disciplinas;
                    case 2:
                        return $scope.filters.dre.disciplinas;
                    case 3:
                        return $scope.filters.escola.disciplinas;
                }
        };

        /*
          @function Selecionar item para o gráfico 
        */
        $scope.getMediaByLevel = function () {
            if ($scope.tree)
                switch ($scope.filters.level) {
                    case 1:
                        return $scope.tree.media;
                    case 2:
                        return $scope.filters.dre.media;
                    case 3:
                        return $scope.filters.escola.media;
                }
        };

        /*
          @function Montar gráfico de desempenho de alunos, turma, escola, DRE, SME 
        */
        $scope.carregaGrafico_Desempenho = function () {

            var result = [];
            var subtitle = "";
            switch ($scope.filters.level) {
                case 1:
                    if ($scope.testInformation.performanceBy == 1) {
                        result = $scope.tree.dres;
                        result.name = 'DREs';
                    }
                    else {
                        var lista = [];
                        for (var x = 0; x < $scope.tree.dres.length; x++) {
                            for (var y = 0; y < $scope.tree.dres[x].escolas.length; y++) {
                                $scope.tree.dres[x].escolas[y].dre_id = $scope.tree.dres[x].id;
                                lista.push($scope.tree.dres[x].escolas[y]);
                            }
                        }
                        result = lista;
                        result.name = 'escolas';
                    }

                    subtitle = 'Clique sobre a barra de desempenho uma DRE ou escola para visualizar o próximo nível.'
                    break;
                case 2:
                    result = $scope.filters.dre.escolas;
                    result.name = $scope.filters.dre.name;
                    subtitle = 'Clique sobre a barra de desempenho de uma das escolas para visualizar o nível de turmas.'
                    break;
                case 3:
                    result = $scope.filters.escola.turmas;
                    result.name = $scope.filters.escola.name;
                    if ($scope.filters.test_id > 0)
                        subtitle = 'Clique sobre a barra de desempenho de uma das turmas para acessar a página de relatório de alunos.'
                    break;
            }

            for (var x = 0; x < result.length; x++) {
                result[x].propertyName = ($scope.testInformation.selectedOrderGraph1 == 1 ? "name" : "media");
            }

            result.sort(compareByProperty);

            if ($scope.testInformation.selectedOrderGraph1ByDesc == 1)
                result.reverse();

            if (result != null) {

                var serieNames = [], seriePerformances = [], serieIds = [],
                    DREAverage = 0.00, SMEAverage = 0.00, teamAverage = 0.00, schoolAverage = 0.00;

                for (var z = 0; z < result.length; z++) {
                    if ($scope.selectedItem.discipline_id == 0 && $scope.selectedItem.item_id == 0) {
                        if ((result[z].media >= $scope.testInformation.performanceGraph1Init)
                            && (result[z].media <= $scope.testInformation.performanceGraph1End)) {

                            var serieName = result[z].name;
                            if (serieName.length > 45) {
                                serieName = serieName.substring(0, 45) + "..."
                            }
                            serieNames.push(serieName);
                            seriePerformances.push(result[z].media);
                            serieIds.push(result[z].id);
                            if (SMEAverage == 0.00)
                                SMEAverage = $scope.tree.media;
                            if ($scope.filters.level > 1 && DREAverage == 0.00)
                                DREAverage = $scope.filters.dre.media;
                            if ($scope.filters.level > 2 && schoolAverage == 0.00)
                                schoolAverage = $scope.filters.escola.media;
                            if ($scope.filters.level > 3 && teamAverage == 0.00)
                                teamAverage = $scope.filters.turma.media;
                        }
                    }
                    else if ($scope.selectedItem.item_id == 0) {
                        for (var d = 0; d < result[z].disciplinas.length; d++) {
                            if (result[z].disciplinas[d].id == $scope.selectedItem.discipline_id
                                && (result[z].disciplinas[d].media >= $scope.testInformation.performanceGraph1Init)
                                && (result[z].disciplinas[d].media <= $scope.testInformation.performanceGraph1End)) {

                                var serieName = result[z].name;
                                if (serieName.length > 45) {
                                    serieName = serieName.substring(0, 45) + "..."
                                }
                                serieNames.push(serieName);
                                serieIds.push(result[z].id);
                                seriePerformances.push(result[z].disciplinas[d].media);
                                if (SMEAverage == 0.00)
                                    SMEAverage.push = $scope.tree.disciplinas[d].media;
                                if ($scope.filters.level > 1 && DREAverage == 0.00)
                                    DREAverage = $scope.filters.dre.disciplinas[d].media;
                                if ($scope.filters.level > 2 && schoolAverage == 0.00)
                                    schoolAverage = $scope.filters.escola.disciplinas[d].media;
                                if ($scope.filters.level > 3 && teamAverage == 0.00)
                                    teamAverage = $scope.filters.turma.disciplinas[d].media;
                                break;
                            }
                        }
                    }
                    else {
                        for (var d = 0; d < result[z].disciplinas.length; d++) {
                            if (result[z].disciplinas[d].id == $scope.selectedItem.discipline_id) {
                                for (var i = 0; i < result[z].disciplinas[d].itens.length; i++) {
                                    if (result[z].disciplinas[d].itens[i].id == $scope.selectedItem.item_id
                                        && (result[z].disciplinas[d].itens[i].media >= $scope.testInformation.performanceGraph1Init)
                                        && (result[z].disciplinas[d].itens[i].media <= $scope.testInformation.performanceGraph1End)) {

                                        var serieName = result[z].name;
                                        if (serieName.length > 45) {
                                            serieName = serieName.substring(0, 45) + "..."
                                        }
                                        serieNames.push(serieName);
                                        serieIds.push(result[z].id);
                                        seriePerformances.push(result[z].disciplinas[d].itens[i].media);
                                        if (SMEAverage == 0.00)
                                            SMEAverage = $scope.tree.disciplinas[d].itens[i].media;
                                        if ($scope.filters.level > 1 && DREAverage == 0.00)
                                            DREAverage = $scope.filters.dre.disciplinas[d].itens[i].media;
                                        if ($scope.filters.level > 2 && schoolAverage == 0.00)
                                            schoolAverage = $scope.filters.escola.disciplinas[d].itens[i].media;
                                        if ($scope.filters.level > 3 && teamAverage == 0.00)
                                            teamAverage = $scope.filters.turma.disciplinas[d].itens[i].media;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                var data = {
                    datasets: [
                        {
                            type: 'horizontalBar',
                            label: 'Desempenho alunos',
                            data: seriePerformances,
                            backgroundColor: '#5c92d8',
                            xAxisID: 'x-axis-0'
                        },
                        {
                            type: 'line',
                            label: 'Média SME (' + SMEAverage + '%)',
                            backgroundColor: 'magenta',
                            data: []
                        }
                    ],
                    labels: serieNames
                };

                if (DREAverage > 0)
                    data.datasets.push({
                        type: 'line',
                        label: 'Média DRE (' + DREAverage + '%)',
                        backgroundColor: 'green',
                        data: []
                    })
                if (schoolAverage > 0)
                    data.datasets.push({
                        type: 'line',
                        label: 'Média escola (' + schoolAverage + '%)',
                        backgroundColor: 'purple',
                        data: []
                    })
                if (teamAverage > 0)
                    data.datasets.push({
                        type: 'line',
                        label: 'Média turma (' + teamAverage + '%)',
                        backgroundColor: 'orange',
                        data: []
                    })

                var canvas = document.getElementById('gfcPerformance');
                var ctx = canvas.getContext("2d");

                if ($scope.chart1.chart && $scope.chart1.chart.canvas)
                    $scope.chart1.chart.destroy();

                canvas.height = 200 + (serieNames.length) * 30;
                canvas.width = 700;

                $scope.chart1 = {
                    visible: true, chart: new Chart(ctx, {
                        type: 'horizontalBar',
                        options: {
                            title: {
                                text: subtitle
                            },
                            legend: {
                                onClick: function (event, legendItem) { }
                            },
                            responsive: false,
                            maintainAspectRatio: false,
                            lineAtIndexes: [
                                { index: SMEAverage, color: 'magenta' },
                                { index: DREAverage, color: 'green' },
                                { index: schoolAverage, color: 'purple' },
                                { index: teamAverage, color: 'orange' }
                            ],
                            scales: {
                                xAxes: [{
                                    display: true,
                                    id: "x-axis-0",
                                    gridLines: {
                                        display: true
                                    },
                                    ticks: {
                                        min: 0,
                                        stepSize: 20,
                                        max: 100,
                                        callback: function (value) {
                                            return value + "%"
                                        }
                                    }
                                }],
                                yAxes: [{
                                    id: 'y-axis-0',
                                    gridLines: {
                                        display: false
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

                        var objSel = function (id) {
                            for (var k = 0; k < result.length; k++) {
                                if (result[k].id == id)
                                    return result[k];
                            }
                        }(serieIds[idx]);
                        if ($scope.filters.level == 1) {
                            if ($scope.testInformation.performanceBy == 1)
                                $scope.filters.dre = objSel;
                            else {
                                $scope.filters.escola = objSel;
                                $scope.filters.level++;
                                for (var x = 0; x < $scope.tree.dres.length; x++) {
                                    if ($scope.tree.dres[x].id == objSel.dre_id) {
                                        $scope.filters.dre = $scope.tree.dres[x]
                                        break;
                                    }
                                }
                                getDres(getDisciplines);
                            }
                        }
                        else if ($scope.filters.level == 2) {
                            $scope.filters.escola = objSel;
                            getDres(getDisciplines);
                        }
                        else if ($scope.filters.level == 3) {
                            $scope.filters.turma = objSel;
                        }

                        if ($scope.filters.level < 3) {

                            $scope.filters.level++;
                            $scope.$apply();

                            $scope.carregaGrafico_Desempenho();
                            if ($scope.selectedItem.discipline_id > 0 && $scope.selectedItem.item_id > 0) {
                                $scope.carregaGrafico_EscolhaItens();
                                $scope.carregaGrafico_DesempenhoPorItem();
                            }
                        }
                        else if ($scope.filters.level == 3) {
                            $window.open('/Correction?test_id=' + $scope.filters.turma.test_id + '&team_id=' + serieIds[idx] + '&esc_id=' + $scope.filters.escola.id + '&dre_id=' + $scope.filters.dre.id, "_blank");
                        }
                    }
                };

                return;
            }
            else {
                $notification['warning']('Nenhum dado foi encontrado.');
            }
        };

        /*
          @function Monta gráfico pizza das alternativas escolhidas
        */
        $scope.carregaGrafico_EscolhaItens = function () {

            var alternatives = [], labels = [], ids = [];

            var result = [];
            switch ($scope.filters.level) {
                case 1:
                    result = $scope.tree;
                    result.name = 'DREs';
                    break;
                case 2:
                    result = $scope.filters.dre;
                    result.name = $scope.filters.dre.name;
                    break;
                case 3:
                    result = $scope.filters.escola;
                    result.name = $scope.filters.escola.name;
                    break;
            }

            for (var d = 0; d < result.disciplinas.length; d++) {
                if (result.disciplinas[d].id == $scope.selectedItemModal.discipline_id) {
                    for (var i = 0; i < result.disciplinas[d].itens.length; i++) {
                        if (result.disciplinas[d].itens[i].id == $scope.selectedItemModal.item.id) {
                            var somaPorcentagem = 0;
                            for (var x = 0; x < result.disciplinas[d].itens[i].alternativas.length; x++) {
                                var alt = result.disciplinas[d].itens[i].alternativas[x];
                                somaPorcentagem += alt.media;
                                labels.push(alt.numeration + (alt.correct ? ' (Gabarito) ' : ''));
                                alternatives.push(alt.media);
                                ids.push(alt.id);
                            }
                            if (somaPorcentagem < 100) {
                                var pNuloRasura = 100 - somaPorcentagem;
                                labels.push("R/N");
                                alternatives.push(pNuloRasura);
                                ids.push(999);
                            }
                        }
                    }
                    break;
                }
            }

            var fadeColors = ['#630000', '#800000', '#AF0101', '#FF8282', '#FFCECE'];
            var data = {
                datasets: [{
                    data: alternatives,
                    backgroundColor: fadeColors.slice(0, alternatives.length)
                }],
                labels: labels,
                ids: ids
            };

            var canvas = document.getElementById('gfcItem');
            var ctx = canvas.getContext("2d");
            canvas.height = 200;
            canvas.width = 400;

            if ($scope.chart3.chart && $scope.chart3.chart.canvas)
                $scope.chart3.chart.destroy();

            $scope.chart3 = {
                visible: true, chart: new Chart(ctx, {
                    type: 'pie',
                    data: data,
                    options: {
                        title: {
                            text: '% de escolhas por alternativa',
                            fontFamily: 'Roboto',
                            fontSize: 18,
                            display: true,
                            fontColor: '#333333',
                            fontStyle: 'normal'
                        },
                        legend: {
                            display: true,
                            position: "right"
                        },
                        hover: {
                            onHover: function (e, el) {
                                $("#gfcItem").css("cursor", el[0] ? "pointer" : "default");
                            }
                        }
                    }
                })
            };

            canvas.onclick = function (evt) {
                var activePoints = $scope.chart3.chart.getElementsAtEvent(evt);
                if (activePoints && activePoints.length > 0) {
                    var idx = activePoints[0]['_index'];
                    $scope.alternativeGraph = ids[idx];
                    $scope.selectedAlternativeLabel = 'Alternativa selecionada : ' + labels[idx];
                    $scope.carregaGrafico_DesempenhoPorItem();
                }
            };

            return;
        };

        /*
          @function Montar gráfico de desempenho de alunos que escolheram determinado item
        */
        $scope.carregaGrafico_DesempenhoPorItem = function () {

            var result = [];
            var subtitle = "";
            switch ($scope.filters.level) {
                case 1:
                    if ($scope.testInformation.performanceBy == 1) {
                        result = $scope.tree.dres;
                        result.name = 'DREs';
                    }
                    else {
                        var lista = [];
                        for (var x = 0; x < $scope.tree.dres.length; x++) {
                            for (var y = 0; y < $scope.tree.dres[x].escolas.length; y++) {
                                $scope.tree.dres[x].escolas[y].dre_id = $scope.tree.dres[x].id;
                                lista.push($scope.tree.dres[x].escolas[y]);
                            }
                        }
                        result = lista;
                        result.name = 'escolas';
                    }
                    subtitle = 'Clique sobre a barra de uma DRE ou escola para visualizar as escolhas dos próximos níveis.'
                    break;
                case 2:
                    result = $scope.filters.dre.escolas;
                    result.name = $scope.filters.dre.name;
                    subtitle = 'Clique sobre a barra de uma das escolas para visualizar as escolhas das turmas.'
                    break;
                case 3:
                    result = $scope.filters.escola.turmas;
                    result.name = $scope.filters.escola.name;
                    if ($scope.filters.test_id > 0)
                        subtitle = 'Clique sobre a barra de uma das turmas para acessar a página de relatório de alunos.'
                    break;
            }

            for (var x = 0; x < result.length; x++) {
                result[x].propertyName = ($scope.testInformation.selectedOrderGraph2 == 1 ? "name" : "media");
            }

            result.sort(compareByProperty);

            if ($scope.testInformation.selectedOrderGraph2ByDesc == 1)
                result.reverse();

            if (result != null) {

                var serieNames = [], serieAvgs = [], serieIds = [];

                for (var x = 0; x < result.length; x++) {
                    var altHits = 0;
                    var showSerie = false;

                    for (var y = 0; y < result[x].disciplinas.length; y++) {
                        if ($scope.selectedItemModal.discipline_id == result[x].disciplinas[y].id) {
                            for (var z = 0; z < result[x].disciplinas[y].itens.length; z++) {
                                if ($scope.selectedItemModal.item.id == result[x].disciplinas[y].itens[z].id) {
                                    for (var a = 0; a < $scope.selectedItemModal.item.alternativas.length; a++) {
                                        if ($scope.alternativeGraph == $scope.selectedItemModal.item.alternativas[a].id) {
                                            if (result[x].disciplinas[y].itens[z].alternativas[a].media >= $scope.testInformation.performanceGraph2Init
                                                && result[x].disciplinas[y].itens[z].alternativas[a].media <= $scope.testInformation.performanceGraph2End) {
                                                var serieName = result[x].name;
                                                if (serieName.length > 38) {
                                                    serieName = serieName.substring(0, 38) + "..."
                                                }
                                                serieNames.push(serieName);
                                                serieIds.push(result[x].id);
                                                serieAvgs.push(result[x].disciplinas[y].itens[z].alternativas[a].media);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                var data = {
                    datasets: [
                        {
                            type: 'horizontalBar',
                            label: '% de escolhas ' + ($scope.selectedAlternativeLabel ? ' (' + $scope.selectedAlternativeLabel + ')' : ''),
                            data: serieAvgs,
                            backgroundColor: '#5c92d8',
                            xAxisID: 'x-axis-0'
                        }
                    ],
                    labels: serieNames
                };

                var canvas = document.getElementById('gfcPerformanceByAlternative');
                var ctx = canvas.getContext("2d");

                if ($scope.chart2.chart && $scope.chart2.chart.canvas)
                    $scope.chart2.chart.destroy();

                canvas.height = 200 + (serieNames.length) * 30;
                canvas.width = 600;

                $scope.chart2 = {
                    visible: true, chart: new Chart(ctx, {
                        type: 'horizontalBar',
                        options: {
                            responsive: false,
                            maintainAspectRatio: false,
                            scales: {
                                xAxes: [{
                                    display: true,
                                    id: "x-axis-0",
                                    gridLines: {
                                        display: true
                                    },
                                    ticks: {
                                        min: 0,
                                        stepSize: 20,
                                        max: 100,
                                        callback: function (value) {
                                            return value + "%"
                                        }
                                    }
                                }],
                                yAxes: [{
                                    id: 'y-axis-0',
                                    gridLines: {
                                        display: false
                                    }
                                }]
                            }
                        },
                        data: data
                    })
                };

                canvas.onclick = function (evt) {
                    var activePoints = $scope.chart2.chart.getElementsAtEvent(evt);
                    if (activePoints && activePoints.length > 0) {
                        var idx = activePoints[0]['_index'];

                        var objSel = function (id) {
                            for (var k = 0; k < result.length; k++) {
                                if (result[k].id == id)
                                    return result[k];
                            }
                        }(serieIds[idx]);

                        if ($scope.filters.level < 3) {
                            if ($scope.filters.level == 1) {
                                $scope.filters.dre = objSel;
                            }
                            else if ($scope.filters.level == 2) {
                                $scope.filters.escola = objSel;
                                getDres(getDisciplines);
                            }

                            $scope.filters.level++;
                            $scope.$apply();

                            $scope.carregaGrafico_Desempenho();
                            $scope.carregaGrafico_EscolhaItens();
                            $scope.carregaGrafico_DesempenhoPorItem();
                        }
                        else if ($scope.filters.level == 3) {
                            $scope.filters.turma = objSel;
                            $window.open('/Correction?test_id=' + $scope.filters.turma.test_id + '&team_id=' + serieIds[idx] + '&esc_id=' + $scope.filters.escola.id + '&dre_id=' + $scope.filters.dre.id, "_blank");
                        }
                    }
                };

                return;
            }
            else {
                //$scope.reportItem = null;
                $notification['warning']('Nenhum dado foi encontrado.');
            }
        };

        /*funções para reordenar por ordem crescente ou decrescente*/

        $scope.resortItems = function () {

            if ($scope.orderItemByDesc == 1)
                $scope.orderItemByDesc = 0
            else
                $scope.orderItemByDesc = 1;
        }

        $scope.resortPerformanceGraph = function () {

            if ($scope.testInformation.selectedOrderGraph1ByDesc == 1)
                $scope.testInformation.selectedOrderGraph1ByDesc = 0
            else
                $scope.testInformation.selectedOrderGraph1ByDesc = 1;

            $scope.carregaGrafico_Desempenho();

        }

        $scope.resortPerformanceByDisciplineGraph = function () {

            if ($scope.testInformation.selectedOrderGraph2ByDesc == 1)
                $scope.testInformation.selectedOrderGraph2ByDesc = 0
            else
                $scope.testInformation.selectedOrderGraph2ByDesc = 1;

            $scope.carregaGrafico_DesempenhoPorItem();

        }

        $scope.previousLevel = function () {
            $scope.filters.level--;
            if ($scope.filters.level == 1) {
                $scope.filters.dre = {};
            }
            else if ($scope.filters.level == 2)
                $scope.filters.escola = {};

            $scope.carregaGrafico_Desempenho();
            if ($scope.selectedItemModal && $scope.selectedItemModal.item) {
                $scope.carregaGrafico_DesempenhoPorItem();
                $scope.carregaGrafico_EscolhaItens();
            }
        }


        /**
         * @function Limpar filtros
         * @param 
         * @returns
         */
        $scope.clearFilters = function __clearFilters() {
            $scope.filters = {};
            $scope.filters.uad_id = "";
            $scope.filters.test_id = "";
            //$scope.filters.DateStart = "";
            //$scope.filters.DateEnd = "";
            $scope.filters.discipline_id = "";
            $scope.filters.discipline_name = "";
            $scope.filters.skill_id = 0;
            $scope.filters.Group = undefined;
            $scope.filters.SubGroup = undefined;
            $scope.Groups = [];
            $scope.SubGroups = [];
            $scope.TestsBySubGroup = [];
        };

        $scope.clearGraphs = function __clearGraphs() {
            $scope.highcharObj1 = {};
            $scope.highcharObj2 = {};
            $scope.highcharObj3 = {};
            $scope.itemsOrderOptions = [{ id: 0, label: "Desempenho" }, { id: 1, label: "Alfabética " }]
            $scope.itemsGraphOrderOptions = [{ id: 0, label: "Desempenho" }, { id: 1, label: "Alfabética " }]
            $scope.itemsGraph2OrderOptions = [{ id: 0, label: "% de escolha" }, { id: 1, label: "Alfabética " }]
            $scope.selectedOrderItem = 0;
            $scope.orderItemByDesc = 0;
            $scope.testInformation = {
                selectedOrderGraph1: 0,
                selectedOrderGraph1ByDesc: 0,
                selectedOrderGraph2: 0,
                selectedOrderGraph2ByDesc: 0,
                performanceGraph1Init: 50,
                performanceGraph1End: 75,
                performanceGraph2Init: 50,
                performanceGraph2End: 75
            }
        }

        /**
         * @function Zera a paginação e pesquisa novamente
         * @param 
         * @returns
         */
        $scope.search = function __search() {
            if (!validateDate() || !validate()) return;
            /* $scope.pages = 0;
             $scope.totalItens = 0;
             $scope.paginate.indexPage(0);
             $scope.pageSize = $scope.paginate.getPageSize();*/
            $scope.useFilter = true;
            $scope.searcheableFilter = angular.copy($scope.filters);
            getHeaderDetails();
            if ($scope.group === true)
                $scope.filters.test_id = undefined;

            getCurriculumGradeTests(function () {
                getDres(getDisciplines);
            });

        };

        /**
         * @function validar
         * @param
         * @returns
         */
        function validate() {
            if (!$scope.group) {
                if (!$scope.filters.test_id) {
                    $notification.alert("É necessário selecionar uma prova ou preencher o seu código.");
                    return false;
                }

                if (!Number.isInteger($scope.filters.test_id)) {
                    $notification.alert("Código da prova inválido.");
                    return false;
                }

                var position = $scope.listTests.indexOfField("TestId", $scope.filters.test_id);
                if (position < 0) {
                    $notification.alert("Prova não encontrada ou fora do período informado.");
                    return false;
                }
            }
            else {
                if (!$scope.filters.SubGroup) {
                    $notification.alert("É necessário selecionar um subgrupo.");
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


        /**
         * @function Responsavél pela paginação
         * @param 
         * @returns
         */
        function Paginate(_callback) {
            $scope.paginate.paginate($scope.searcheableFilter).then(function (result) {
                $scope.message = false;
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.listResult = result.lista;
                        $scope.listResultSME = result.MediasSME;
                        configSliderDRE(result.lista);
                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.listResult = null;
                        $scope.listResultSME = null;
                    }
                    if (_callback) _callback();
                }
                else {
                    $notification['error'](result.message);
                }
            }, function () {
                $scope.message = true;
                $scope.listResult = null;
                $scope.listResultSME = null;
            });
        };
        $scope.Paginate = Paginate;

        /**
        * @function Gera o relatório de desempenho do Item
        * @param 
        * @returns
        */
        $scope.generateReport = function __generateReport() {
            var params = {
                test_id: $scope.filters.test_id,  //,
                subGroup_id: $scope.group === true ? $scope.filters.SubGroup.Id : 0,
                tcp_id: ($scope.filters.CurriculumGrade && $scope.filters.CurriculumGrade.tcp_id > 0) ? $scope.filters.CurriculumGrade.tcp_id : 0,
                dre_id: $scope.filters.dre ? $scope.filters.dre.id : null,
                esc_id: $scope.filters.escola ? $scope.filters.escola.id : null,
                uad_id: $scope.filters.uad_id
            }

            ReportItemPerformanceModel.exportDRE(params, function (result) {
                if (result.success) {
                    window.open("/ReportItemPerformance/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification['error'](result.message);
                }
            });
        };

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

            if (angular.element(".side-filters").hasClass("side-filters-animation")) $scope.open();
        };

        /**
         * @function Iniciar
         * @param
         * @returns
         */
        (function __init() {
            $notification.clear();
            $scope.clearFilters();
            angular.element('body').click(close);
            $(document).ready(function () {
                var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

                var startDate = new Date();
                startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
                $("#dateStart").datepicker("setDate", startDate);

                var endDate = new Date();
                endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
                $("#dateEnd").datepicker("setDate", endDate);

                configVariables();
            });
        })();

        /**
         * @function Configuração da slider para exibição das médias
         * @name configSliderDRE
         * @namespace ReportItemPerformanceDREController
         * @memberOf Controller
         * @private
         * @param {array} lista
         * @return
         */
        function configSliderDRE(lista) {

            var interval = 15;

            $scope.sliderDRE = {
                min: 0,
                max: (((lista.length + 1) - interval) < 0) ? 0 : (lista.length + 1) - interval,
                current: 0,
                interval: interval,
                length: lista.length
            };

            $scope.beginDRE();
        };

        //    /**
        //	 * @function Próximo slider
        //	 * @name nextDRE
        //	 * @namespace ReportItemPerformanceDREController
        //	 * @memberOf Controller
        //	 * @public
        //	 * @param
        //	 * @return
        //	 */
        $scope.nextDRE = function __nextDRE() {

            var nextValue = $scope.sliderDRE.current + 1;

            if (nextValue > $scope.sliderDRE.max)
                return;

            $scope.sliderDRE.current = nextValue;
            getItemsSlice();
        };

        /**
         * @function Anterior slider
         * @name previusDRE
         * @namespace ReportItemPerformanceDREController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.previousDRE = function __previousDRE() {

            var previousValue = $scope.sliderDRE.current - 1;

            if (previousValue < 0)
                return;

            $scope.sliderDRE.current = previousValue;
            getItemsSlice();
        };

        /**
         * @function Começo slider
         * @name beginDRE
         * @namespace ReportItemPerformanceDREController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.beginDRE = function __beginDRE() {

            setItensKeyBeginDRE();
            getItemsSlice();
        };

        /**
         * @function Final slider
         * @name endDRE
         * @namespace ReportItemPerformanceDREController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.endDRE = function __endDRE() {

            $scope.sliderDRE.current = $scope.sliderDRE.max;
            getItemsSlice();
        };

        /**
          * @function Obter a lista visível para os dados (slider interval)
          * @name setItensKeyBeginDRE
          * @namespace ReportItemPerformanceDREController
          * @memberOf Controller
          * @private
          * @param {int} index
          * @return
          */
        function setItensKeyBeginDRE() {
            $scope.sliderDRE.current = 0;

            $scope.totalItensHorizontalDRE = $scope.listaItens.length;
        };

        $scope.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }

        $scope.loadVideo = function (video) {
            $scope.videoDestaque = video;
        }

        $scope.downloadChart = (function (graph) {
            $(graph).get(0).toBlob(function (blob) {
                saveAs(blob, "grafico.png");
            });
        });

    }
})(angular, jQuery);