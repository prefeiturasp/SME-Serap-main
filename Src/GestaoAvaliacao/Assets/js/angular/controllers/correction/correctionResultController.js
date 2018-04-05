/**
 * function CorrectionResultController Controller
 * @namespace Controller
 * @author Julio Cesar Silva 26/11/2015
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
		.controller("CorrectionResultController", CorrectionResultController);


    CorrectionResultController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', 'CorrectionModel', 'TestAdministrateModel', 'DisciplineModel', 'ItemModel', 'TestModel'];


    /**
	 * @function Controller 'Resultados da prova'
	 * @name CorrectionResultController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $window
	 * @param {Object} $sce
	 * @param {Object} $util
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @param {Object} CorrectionModel
	 * @param {Object} TestAdministrateModel
	 * @return
	 */
    function CorrectionResultController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, CorrectionModel, TestAdministrateModel, DisciplineModel, ItemModel, TestModel) {


        /**
		  * @function Redirecionar para listagem de provas.
		  * @name redirectToList
		  * @namespace CorrectionResultController
		  * @memberOf Controller
		  * @private
		  * @param
		  * @return
		  */
        function redirectToList(destiny) {
            $timeout(function __invalidTestId() {
                $window.location.href = destiny;
            }, 3000);
        };


        /**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace CorrectionResultController
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
                Team: informations.team,
                SchoolName: informations.schoolName,
                Token: informations.token,
                NumberAnswer: informations.numberAnswer === undefined || informations.numberAnswer === null ? 4 : informations.numberAnswer,
                blockCorrection: informations.blockCorrection,
                blockAccess: informations.blockAccess
            };
        };


        /**
		 * @function Configuração das listas
		 * @name config
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        function config() {

            $scope.list = {};
            $scope.average =
                {
                    AvgDRE: 0,
                    AvgESC: 0,
                    AvgSME: 0,
                    AvgTeam: 0
                }
            $scope.listDiscipline = [];
            $scope.itemsOrderOptions = [{ id: 0, label: "Desempenho" }, { id: 1, label: "Alfabética " }]
            $scope.itemsGraphOrderOptions = [{ id: 0, label: "Número de chamada" }, { id: 1, label: "Alfabética " }]
            $scope.selectedOrderItem = 0;
            $scope.orderItemByDesc = 0;
            $scope.testInformation.selectedOrderGraph1 = 0;
            $scope.testInformation.selectedOrderGraph1ByDesc = 0;
            $scope.testInformation.selectedOrderGraph2 = 0;
            $scope.testInformation.selectedOrderGraph2ByDesc = 0;
            $scope.testInformation.performanceGraph1Init = 0;
            $scope.testInformation.performanceGraph1End = 100;
            $scope.testInformation.performanceGraph2Init = 0;
            $scope.testInformation.performanceGraph2End = 100;
            $scope.testInformation.discipline_id = "";
            $scope.testInformation.discipline_name = "";
            $scope.oneDiscipline = false;
            $scope.chart1 = {
                visible: false,
                chart: {}
            };
            $scope.chart2 = {
                visible: false,
                chart: {}
            };
            $scope.chart3 = {
                visible: false,
                chart: {}
            }
        };


        /**
		 * @function Configuração da slider para exibição das notas dos alunos
		 * @name configSlider
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {array} lista
		 * @return
		 */
        function configSlider(lista) {

            var interval = 30;

            $scope.slider = {
                min: 0,
                max: ((lista.length - interval) < 0) ? 0 : lista.length - interval,
                current: 0,
                interval: interval,
                length: lista.length
            };

            $scope.begin();
        };

        /**
         * @function obter as turmas que compartilham a escola com a turma selecionada
         * @param {object} _callback
         * @returns
         */
        function getTeams() {
            var params = {
                'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'ttn_id': 0,
                'esc_id': $scope.testInformation.Team.esc_id,
                'crp_ordem': 0,
                'dre_id': null,
                'statusCorrection': '2,3'
            };

            TestAdministrateModel.getSectionAdministrate(params, function (result) {

                if (result.success) {
                    $scope.listTeams = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function obter as disciplinas da prova
         * @param {object} _callback
         * @returns
         */
        function getDisciplines(_callback) {
            if (!$scope.testInformation.TestId) return;
            $timeout(function () {
                DisciplineModel.loadComboByTest({ Test_id: $scope.testInformation.TestId }, function (result) {
                    if (result.success) {
                        $scope.listDiscipline = result.lista;
                        $scope.testInformation.discipline_id = "";
                        $scope.testInformation.discipline_name = "";
                        $scope.oneDiscipline = false;

                        if ($scope.listDiscipline.length == 1) {
                            $scope.testInformation.discipline_id = $scope.listDiscipline[0].Id;
                            $scope.testInformation.discipline_name = $scope.listDiscipline[0].Description;
                            $scope.oneDiscipline = true;
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                    if (_callback) _callback();
                });
            });
        };
        $scope.getDisciplines = getDisciplines;

        /**
         * @function gerar o relatório através da disciplina selecionada
         * @param {object} _callback
         * @returns
         */
        function loadByDiscipline(_callback) {
            GetTestAveragesPercentagesByTest(function () {
                getStudents($scope.carregaGrafico_Desempenho);
            });
        }
        $scope.loadByDiscipline = loadByDiscipline;

        /**
         * @function Gera a lista de itens por disciplina
         * @param {object} _callback
         * @returns
         */
        function loadAndGroupItensByDiscipline(_callback) {

            var listItens = [];
            $scope.itemsLoaded = 0;

            ItemModel.getItemSummaryByTestItens({ test_id: $scope.params.test_id }, function (r) {
                if (r.success) {
                    listItens = r.lista;

                    $scope.itemsByDiscipline = [];
                    for (var x = 0; x < $scope.listDiscipline.length; x++) {

                        var discipline = {
                            id: $scope.listDiscipline[x].Id,
                            name: $scope.listDiscipline[x].Description,
                            items: []
                        };

                        $scope.itemsByDiscipline.push(discipline);

                        var y = 0;
                        while (y < $scope.list.Statistics.Averages.length) {
                            if ($scope.list.Statistics.Averages[y].Discipline_Id == $scope.listDiscipline[x].Id) {
                                var itemId = $scope.list.Statistics.Averages[y].Item_Id;
                                var index = x;
                                var item;

                                for (var z = 0; z < listItens.length; z++) {
                                    if (listItens[z].Id == itemId) {
                                        item = listItens[z];
                                    }
                                }

                                item.averages = getAveragesByItemId(itemId);
                                $scope.itemsByDiscipline[index].items.push(item);
                                $scope.itemsLoaded++;

                                if ($scope.disciplineId !== undefined && $scope.itemId !== undefined && $scope.itemId == itemId) {
                                    $scope.findItemById($scope.disciplineId, $scope.itemId);
                                }
                            }
                            y++;
                        }
                    }

                    if (_callback)
                        _callback();
                }
            });
        }
        $scope.LoadAndGroupItensByDiscipline = loadAndGroupItensByDiscipline;

        //Seleciona o item para exibir as informações na modal
        $scope.findItemById = function (disciplineId, itemId) {

            $scope.disciplineId = disciplineId;
            $scope.itemId = itemId;
            $scope.itens = [];

            $scope.selectedItemModal = { item: {}, averages: [], disciplineName: '' }

            $scope.copyDisciplinas = angular.copy($scope.itemsByDiscipline);
            for (var x = 0; x < $scope.copyDisciplinas.length; x++) {
                for (var y = 0; y < $scope.copyDisciplinas[x].items.length; y++) {
                    $scope.itens.push($scope.copyDisciplinas[x].items[y]);
                }
            }

            for (var x = 0; x < $scope.itemsByDiscipline.length; x++) {
                if ($scope.itemsByDiscipline[x].id == disciplineId) {
                    for (var y = 0; y < $scope.itemsByDiscipline[x].items.length; y++) {
                        if ($scope.itemsByDiscipline[x].items[y].Id == itemId) {
                            $scope.selectedItemModal.disciplineId = disciplineId;
                            $scope.selectedItemModal.disciplineName = $scope.itemsByDiscipline[x].name;
                            $scope.selectedItemModal.item = $scope.itemsByDiscipline[x].items[y];

                            if ($scope.videoDestaque = $scope.itemsByDiscipline[x].items[y].Videos.length > 0)
                                $scope.videoDestaque = $scope.itemsByDiscipline[x].items[y].Videos[0];
                            break;
                        }
                    }
                    break;
                }
            }

            if ($scope.selectedItemModal.item.Alternatives !== undefined) {
                for (var x = 0; x < $scope.selectedItemModal.item.Alternatives.length; x++) {
                    var alternativa = $scope.selectedItemModal.item.Alternatives[x];
                    var choices = 0;
                    var didntAnswer = 0;
                    var countStudent = 0;
                    for (var y = 0; y < $scope.list.Students.length; y++) {
                        if ($scope.list.Students[y].Alternatives != null && $scope.list.Students[y].Alternatives.length > 0) {
                            countStudent++;
                            for (var z = 0; z < $scope.list.Students[y].Alternatives.length; z++) {
                                if ($scope.list.Students[y].Alternatives[z].Alternative_Id == alternativa.Id) {
                                    choices++;
                                    break;
                                }
                            }
                        }
                        else
                            didntAnswer++;
                    }
                    $scope.selectedItemModal.item.Alternatives[x].average = parseFloat((choices / countStudent * 100).toFixed(2));
                }
                if (didntAnswer > 0)
                    $scope.selectedItemModal.didntAnswer = didntAnswer;

                $scope.alternativeGraph = undefined;
                $scope.carregaGrafico_DesempenhoPorDisciplina();
                $scope.carregaGrafico_EscolhaItens();
            }
        }

        $scope.proximaQuestao = function (selectedItemModal) {
            $scope.selectedItemModal = { item: {}, averages: [], disciplineName: '' }

            $scope.itens.sort(function (a, b) {
                return a.ItemOrder < b.ItemOrder ? -1 : a.ItemOrder > b.ItemOrder ? 1 : 0;
            });

            var index = -1;
            for (var i = 0, len = $scope.itens.length; i < len; i++) {
                if ($scope.itens[i].Id === selectedItemModal.Id) {
                    index = i;
                    break;
                }
            }

            if (index + 1 != $scope.itens.length) {
                $scope.selectedItemModal.item = $scope.itens[index + 1];
                $scope.selectedItemModal.disciplineName = $scope.itens[index + 1].Discipline_name;
                $scope.selectedItemModal.disciplineId = $scope.itens[index + 1].Discipline_id;

                if ($scope.videoDestaque = $scope.itens[index + 1].Videos.length > 0)
                    $scope.videoDestaque = $scope.itens[index + 1].Videos[0];
            }
            else {
                $scope.selectedItemModal.item = $scope.itens[index];
                $scope.selectedItemModal.disciplineName = $scope.itens[index].Discipline_name;
                $scope.selectedItemModal.disciplineId = $scope.itens[index].Discipline_id;

                if ($scope.videoDestaque = $scope.itens[index].Videos.length > 0)
                    $scope.videoDestaque = $scope.itens[index].Videos[0];

                $notification.alert("Não existem mais itens na prova.");
            }

            if ($scope.selectedItemModal.item.Alternatives !== undefined) {
                for (var x = 0; x < $scope.selectedItemModal.item.Alternatives.length; x++) {
                    var alternativa = $scope.selectedItemModal.item.Alternatives[x];
                    var choices = 0;
                    var didntAnswer = 0;
                    var countStudent = 0;
                    for (var y = 0; y < $scope.list.Students.length; y++) {
                        if ($scope.list.Students[y].Alternatives != null && $scope.list.Students[y].Alternatives.length > 0) {
                            countStudent++;
                            for (var z = 0; z < $scope.list.Students[y].Alternatives.length; z++) {
                                if ($scope.list.Students[y].Alternatives[z].Alternative_Id == alternativa.Id) {
                                    choices++;
                                    break;
                                }
                            }
                        }
                        else
                            didntAnswer++;
                    }
                    $scope.selectedItemModal.item.Alternatives[x].average = parseFloat((choices / countStudent * 100).toFixed(2));
                }
                if (didntAnswer > 0)
                    $scope.selectedItemModal.didntAnswer = didntAnswer;

                $scope.alternativeGraph = undefined;
                $scope.carregaGrafico_DesempenhoPorDisciplina();
                $scope.carregaGrafico_EscolhaItens();
            }
        }

        $scope.voltaQuestao = function (selectedItemModal) {
            $scope.selectedItemModal = { item: {}, averages: [], disciplineName: '' }

            $scope.itens.sort(function (a, b) {
                return a.ItemOrder < b.ItemOrder ? -1 : a.ItemOrder > b.ItemOrder ? 1 : 0;
            });

            var index = -1;
            for (var i = 0, len = $scope.itens.length; i < len; i++) {
                if ($scope.itens[i].Id === selectedItemModal.Id) {
                    index = i;
                    break;
                }
            }

            if (index - 1 >= 0) {
                $scope.selectedItemModal.item = $scope.itens[index - 1];
                $scope.selectedItemModal.disciplineName = $scope.itens[index - 1].Discipline_name;
                $scope.selectedItemModal.disciplineId = $scope.itens[index - 1].Discipline_id;

                if ($scope.videoDestaque = $scope.itens[index - 1].Videos.length > 0)
                    $scope.videoDestaque = $scope.itens[index - 1].Videos[0];
            }
            else {
                $scope.selectedItemModal.item = $scope.itens[index];
                $scope.selectedItemModal.disciplineName = $scope.itens[index].Discipline_name;
                $scope.selectedItemModal.disciplineId = $scope.itens[index].Discipline_id;

                if ($scope.videoDestaque = $scope.itens[index].Videos.length > 0)
                    $scope.videoDestaque = $scope.itens[index].Videos[0];

                $notification.alert("Não existem itens anteriores na prova.");
            }

            if ($scope.selectedItemModal.item.Alternatives !== undefined) {
                for (var x = 0; x < $scope.selectedItemModal.item.Alternatives.length; x++) {
                    var alternativa = $scope.selectedItemModal.item.Alternatives[x];
                    var choices = 0;
                    var didntAnswer = 0;
                    var countStudent = 0;
                    for (var y = 0; y < $scope.list.Students.length; y++) {
                        if ($scope.list.Students[y].Alternatives != null && $scope.list.Students[y].Alternatives.length > 0) {
                            countStudent++;
                            for (var z = 0; z < $scope.list.Students[y].Alternatives.length; z++) {
                                if ($scope.list.Students[y].Alternatives[z].Alternative_Id == alternativa.Id) {
                                    choices++;
                                    break;
                                }
                            }
                        }
                        else
                            didntAnswer++;
                    }
                    $scope.selectedItemModal.item.Alternatives[x].average = parseFloat((choices / countStudent * 100).toFixed(2));
                }
                if (didntAnswer > 0)
                    $scope.selectedItemModal.didntAnswer = didntAnswer;

                $scope.alternativeGraph = undefined;
                $scope.carregaGrafico_DesempenhoPorDisciplina();
                $scope.carregaGrafico_EscolhaItens();
            }
        }

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
        }
        /**
		 * @function Carregar médias por item.
		 * @name getAveragesByItemId **/

        function getAveragesByItemId(itemId) {
            var averages = { rightChoose: 0 };
            for (var x = 0; x < $scope.list.Statistics.Averages.length; x++) {
                if ($scope.list.Statistics.Averages[x].Item_Id == itemId) {
                    averages.rightChoose = $scope.list.Statistics.Averages[x].Average;
                    break;
                }
            }
            return averages;
        }

        /**
		 * @function Carregar estudandes de uma turma.
		 * @name getStudents
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        function getStudents(_callback) {

            var params = {
                'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0,
                'discipline_id': $scope.testInformation.discipline_id
            };

            CorrectionModel.getResultCorrectionGrid(params, function (result) {

                if (result.success) {

                    if (result.entity === null || result.entity === undefined)
                        return;

                    $scope.list = result.entity;

                    $scope.LoadAndGroupItensByDiscipline(_callback);

                    configSlider(result.entity.Answers);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Carregar médias da DRE, Escola e SME
         * @name GetTestAveragesPercentagesByTest
         * @namespace CorrectionResultController
         * @memberOf Controller
         * @private
         * @param
         * @return
         */
        function GetTestAveragesPercentagesByTest(_callback) {

            $scope.params = $util.getUrlParams();

            var params = {
                'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'esc_id': $scope.params.esc_id,
                'dre_id': $scope.params.dre_id,
                'team_id': $scope.testInformation ? $scope.testInformation.Team.Id : ($scope.params.team_id ? $scope.params.team_id : 0),
                'discipline_id': $scope.testInformation.discipline_id
            };

            CorrectionModel.getTestAveragesPercentagesByTest(params, function (result) {

                if (result.success) {

                    if (!result.data)
                        return;

                    $scope.average = result.data;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
		 * @function Obter a lista visível para gabarito (slider interval)
		 * @name setAnswersKey
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function setAnswersKey(index) {
            $scope.answers.push(angular.copy($scope.list.Answers[index]));
            $scope.totalAnswers = $scope.answers.length;
        };


        /**
		 * @function Obter a lista visível para estatísticas (slider interval)
		 * @name setStatistics
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function setStatistics(index) {
            $scope.statistics.push(angular.copy($scope.list.Statistics.Averages[index]));
        };


        /**
		 * @function Obter a lista visível de respostas no slider dos estudantes
		 * @name setStudentsAnswersToSlider
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function setStudentsAnswersToSlider(index) {
            for (var a = 0; a < $scope.list.Students.length; a++)
                if ($scope.list.Students[a].Alternatives !== undefined && $scope.list.Students[a].Alternatives !== null)
                    $scope.list.Students[a].Answers.push(angular.copy($scope.list.Students[a].Alternatives[index]));
                else
                    $scope.list.Students[a].Answers.push(null);
        };


        /**
		 * @function Limpar a lista visível de respostas no slider dos estudantes
		 * @name setStudentsAnswersToSlider
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function resetStudentsAnswersLists() {

            for (var a = 0; a < $scope.list.Students.length; a++)
                $scope.list.Students[a].Answers = [];
        };


        /**
		 * @function Inverter array de answers
		 * @name StudentsAnswersReverse
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function studentsAnswersReverse() {

            for (var a = 0; a < $scope.list.Students.length; a++)
                $scope.list.Students[a].Snswers = $scope.list.Students[a].Answers.reverse();
        };

        /**
		 * @function Buscar itens 
		 * @name getItensByTeam
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param {int} index
		 * @return
		 */
        function getItensByTeam() {

            $scope.listItensByDiscipline = [];
            var params = {
                'Test_Id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'Team_Id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0
            }
            CorrectionModel.getItensByTeam(params, function (result) {
                if (result.success) {
                    if (!result.dados)
                        return;

                    $scope.listItensByDiscipline = result.dados;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    $timeout(function __redirectError() { $window.history.back(); }, 1300);
                }
            });
        }

        function compareStudentsByName(a, b) {
            if (a.alu_nome < b.alu_nome)
                return -1;
            if (a.alu_nome > b.alu_nome)
                return 1;
            return 0;
        }

        function compareStudentsByAttendance(a, b) {
            if (a.mtu_numeroChamada < b.mtu_numeroChamada)
                return -1;
            if (a.mtu_numeroChamada > b.mtu_numeroChamada)
                return 1;
            return 0;
        }

        /*
          @function Montar gráfico de desempenho de alunos, turma, escola, DRE, SME 
        */
        $scope.carregaGrafico_Desempenho = function () {

            if ($scope.testInformation.selectedOrderGraph1 == 1)
                $scope.list.Students.sort(compareStudentsByName);
            else
                $scope.list.Students.sort(compareStudentsByAttendance);

            if ($scope.testInformation.selectedOrderGraph1ByDesc == 1)
                $scope.list.Students.reverse()

            if ($scope.list != null) {

                var studentsName = [], studentsPerformance = [];

                for (var x = 0; x < $scope.list.Students.length; x++) {
                    $scope.list.Students[x].Performance = $scope.list.Students[x].Performance == null ? 0 : $scope.list.Students[x].Performance;
                    if (($scope.list.Students[x].Performance >= $scope.testInformation.performanceGraph1Init)
                        && ($scope.list.Students[x].Performance <= $scope.testInformation.performanceGraph1End)) {

                        studentsName.push($scope.list.Students[x].mtu_numeroChamada + ' - ' + $scope.list.Students[x].alu_nome);
                        studentsPerformance.push($scope.list.Students[x].Performance);
                    }
                }

                var data = {
                    datasets: [
                        {
                            type: 'horizontalBar',
                            label: 'Desempenho alunos',
                            data: studentsPerformance,
                            backgroundColor: '#5c92d8',
                            xAxisID: 'x-axis-0'
                        },
                        {
                            type: 'line',
                            label: 'Média SME (' + $scope.average.AvgSME + '%)',
                            backgroundColor: 'magenta',
                            data: []
                        },
                        {
                            type: 'line',
                            label: 'Média DRE (' + $scope.average.AvgDRE + '%)',
                            backgroundColor: 'green',
                            data: []
                        },
                        {
                            type: 'line',
                            label: 'Média escola (' + $scope.average.AvgESC + '%)',
                            backgroundColor: 'purple',
                            data: []
                        },
                        {
                            type: 'line',
                            label: 'Média turma (' + $scope.average.AvgTeam + '%)',
                            backgroundColor: 'orange',
                            data: []
                        }
                    ],
                    labels: studentsName
                };

                //limpar gráfico antigo
                var divParent = document.getElementById('gfcPerformance').parentElement;
                document.getElementById('gfcPerformance').remove();
                divParent.innerHTML = '<canvas id="gfcPerformance" style="max-width: 700px; width:700px; overflow-x: auto; overflow-y: hidden;"></canvas>';

                var canvas = document.getElementById('gfcPerformance');
                var ctx = canvas.getContext("2d");

                canvas.height = 200 + (studentsName.length) * 30;
                canvas.width = 700;

                $scope.chart1 = {
                    visible: true, chart: new Chart(ctx, {
                        type: 'horizontalBar',
                        options: {
                            responsive: false,
                            maintainAspectRatio: false,
                            legend: {
                                onClick: function (event, legendItem) { }
                            },
                            lineAtIndexes: [
                                { index: $scope.average.AvgESC, color: 'purple' },
                                { index: $scope.average.AvgTeam, color: 'orange' },
                                { index: $scope.average.AvgSME, color: 'magenta' },
                                { index: $scope.average.AvgDRE, color: 'green' }
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

                return;
            }
            else {
                //$scope.reportItem = null;
                $notification['warning']('Nenhum dado foi encontrado.');
            }
        };

        /*
          @function Monta gráfico pizza das alternativas escolhidas
        */
        $scope.carregaGrafico_EscolhaItens = function () {

            if ($scope.list != null) {

                var alternatives = [], alternativesPorcentagem = [];
                var labels = [];
                var ids = [];
                var somaPorcentagem = 0;

                for (var x = 0; x < $scope.selectedItemModal.item.Alternatives.length; x++) {
                    var alt = $scope.selectedItemModal.item.Alternatives[x];
                    somaPorcentagem += alt.average;
                    alternatives.push(alt.average);
                    labels.push(alt.Numeration + (alt.Correct ? ' (Gabarito) ' : ''));
                    ids.push(alt.Id);
                }
                if (somaPorcentagem < 100) {
                    var pNuloRasura = 100 - somaPorcentagem;
                    labels.push("R/N");
                    alternatives.push(pNuloRasura);
                    ids.push(999);
                }

                if ($scope.selectedItemModal.item.didntAnswer && $scope.selectedItemModal.item.didntAnswer > 0) {
                    labels.push('Não responderam');
                    alternatives.push((($scope.selectedItemModal.item.didntAnswer / $scope.list.Students.length) * 100));
                    ids.push(0);
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

                //limpar gráfico antigo
                var divParent = document.getElementById('gfcItem').parentElement;
                document.getElementById('gfcItem').remove();
                divParent.innerHTML = '<canvas id="gfcItem" style="margin: auto;"></canvas>';

                var canvas = document.getElementById('gfcItem');
                var ctx = canvas.getContext("2d");


                canvas.height = 200;
                canvas.width = 400;

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
                        $scope.carregaGrafico_DesempenhoPorDisciplina();
                    }
                };

                return;
            }
            else {
                //$scope.reportItem = null;
                $notification['warning']('Nenhum dado foi encontrado.');
            }
            //  }
        };

        /*
          @function Montar gráfico de desempenho de alunos que escolheram determinado item
        */
        $scope.carregaGrafico_DesempenhoPorDisciplina = function () {

            if ($scope.list != null) {

                var studentsName = [], studentsPerformance = [];

                if ($scope.testInformation.selectedOrderGraph2 == 1)
                    $scope.list.Students.sort(compareStudentsByName);
                else
                    $scope.list.Students.sort(compareStudentsByAttendance);

                if ($scope.testInformation.selectedOrderGraph2ByDesc == 1)
                    $scope.list.Students.reverse()

                for (var x = 0; x < $scope.list.Students.length; x++) {
                    var studentPerformance = 0;
                    var showStudent = false;
                    if ($scope.list.Students[x].Alternatives && $scope.list.Students[x].Alternatives.length > 0)
                        for (var y = 0; y < $scope.list.Students[x].Alternatives.length; y++) {
                            if ($scope.list.Students[x].Alternatives[y].Alternative_Id == $scope.alternativeGraph || !$scope.alternativeGraph) {
                                showStudent = true;
                            }
                            if ($scope.list.Students[x].Alternatives[y].Discipline_Id == $scope.selectedItemModal.disciplineId
                                && $scope.list.Students[x].Alternatives[y].Correct)
                                studentPerformance++;
                        }
                    else if ($scope.alternativeGraph == 0 || !$scope.alternativeGraph)
                        showStudent = true;


                    if (showStudent
                        && ((studentPerformance / $scope.list.Answers.length) * 100) >= $scope.testInformation.performanceGraph2Init
                        && ((studentPerformance / $scope.list.Answers.length) * 100) <= $scope.testInformation.performanceGraph2End) {
                        studentsName.push($scope.list.Students[x].mtu_numeroChamada + ' - ' + $scope.list.Students[x].alu_nome);
                        studentsPerformance.push((studentPerformance / $scope.list.Answers.length) * 100);
                    }
                }

                var data = {
                    datasets: [
                        {
                            type: 'horizontalBar',
                            label: 'Desempenho alunos' + ($scope.selectedAlternativeLabel ? ' (' + $scope.selectedAlternativeLabel + ')' : ''),
                            data: studentsPerformance,
                            backgroundColor: '#5c92d8',
                            xAxisID: 'x-axis-0'
                        }
                    ],
                    labels: studentsName
                };

                //limpar gráfico antigo
                var divParent = document.getElementById('gfcPerformanceByAlternative').parentElement;
                document.getElementById('gfcPerformanceByAlternative').remove();
                divParent.innerHTML = '<canvas id="gfcPerformanceByAlternative" style="margin: auto;"></canvas>';

                var canvas = document.getElementById('gfcPerformanceByAlternative');
                var ctx = canvas.getContext("2d");

                canvas.height = 200 + (studentsName.length) * 30;
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

                return;
            }
            else {
                //$scope.reportItem = null;
                $notification['warning']('Nenhum dado foi encontrado.');
            }
        };

        /*funções para reordenar por ordem crescente ou decrescente*/

        $scope.alterOrderItens = function (selectedOrderItem) {
            $scope.orderItemByDesc = 0
        }

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

            $scope.carregaGrafico_DesempenhoPorDisciplina();

        }

        /*---------------------------------------------------------------------*/

        /**
		 * @function Próximo slider
		 * @name next
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.next = function __next() {

            var nextValue = $scope.slider.current + 1;

            if (nextValue > $scope.slider.max)
                return;

            $scope.answers = [];
            $scope.statistics = [];
            resetStudentsAnswersLists();
            $scope.slider.current = nextValue;

            for (var a = $scope.slider.current; a < $scope.slider.length; a++) {

                if ($scope.answers.length < $scope.slider.interval) {

                    /* Gabarito */
                    setAnswersKey(a);

                    /* Estatísticas */
                    setStatistics(a);

                    /* Respostas Aluno */
                    setStudentsAnswersToSlider(a);
                }
            }

            $scope.totalAnswers = a;
        };


        /**
		 * @function Anterior slider
		 * @name previus
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.previus = function __previus() {

            var previusValue = $scope.slider.current - 1;

            if (previusValue < 0)
                return;

            $scope.answers = [];
            $scope.statistics = [];
            resetStudentsAnswersLists();
            $scope.slider.current = previusValue;

            for (var a = $scope.slider.current; a < $scope.slider.length; a++) {

                if ($scope.answers.length < $scope.slider.interval) {

                    /* Gabarito */
                    setAnswersKey(a);

                    /* Estatísticas */
                    setStatistics(a);

                    /* Respostas Aluno */
                    setStudentsAnswersToSlider(a);
                }
            }

            $scope.totalAnswers = a;
        };


        /**
		 * @function Começo slider
		 * @name begin
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.begin = function __begin() {

            $scope.answers = [];
            $scope.statistics = [];
            resetStudentsAnswersLists();
            $scope.slider.current = 0;

            for (var a = 0; a < $scope.slider.length; a++) {

                if ($scope.answers.length < $scope.slider.interval) {

                    /* Gabarito */
                    setAnswersKey(a);

                    /* Estatísticas */
                    setStatistics(a);

                    /* Respostas Aluno */
                    setStudentsAnswersToSlider(a);
                }
            }

            $scope.totalAnswers = a;
        };


        /**
		 * @function Final slider
		 * @name end
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.end = function __end() {

            $scope.answers = [];
            $scope.statistics = [];
            resetStudentsAnswersLists();
            $scope.slider.current = $scope.slider.max;

            for (var a = $scope.slider.length - 1 ; a >= 0; a--) {

                if (a >= $scope.slider.max) {

                    /* Gabarito */
                    setAnswersKey(a);

                    /* Estatísticas */
                    setStatistics(a);

                    /* Respostas Aluno */
                    setStudentsAnswersToSlider(a);
                }
            }

            $scope.totalAnswers = a;
            $scope.answers = $scope.answers.reverse();
            $scope.statistics = $scope.statistics.reverse();
            studentsAnswersReverse();
        };

        $scope.init = function __init() {
            $scope.params = $util.getUrlParams();

            var params = {
                'test_id': $scope.params.test_id ? $scope.params.test_id : 0,
                //Se esta sendo executado através da mudança do combo, pega o valor selecionado, se não, pega da url
                'team_id': $scope.testInformation ? $scope.testInformation.Team.Id : ($scope.params.team_id ? $scope.params.team_id : 0),
                'result': true
            };

            $scope.params = params;

            CorrectionModel.getAuthorize(params, function (result) {
                if (result.success) {
                    $scope.getAuthorize(result.dados);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    $timeout(function __redirectError() { $window.history.back(); }, 1300);
                }
            });
        };

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getAuthorize
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.getAuthorize = function __getAuthorize(authorize) {
            $scope.blockPage = false;

            if (authorize === undefined || authorize === null) {
                $scope.blockPage = true;
                $notification.alert("Não foi possível obter as permissões de acesso a página.");
                redirectToList('/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId);
                return;
            }

            configTestInformation(authorize);

            if ($scope.params === undefined ||
                $scope.testInformation.TestId === undefined || $scope.testInformation.TestId === null ||
                $scope.testInformation.Team === undefined || $scope.testInformation.Team === null ||
                $scope.params.test_id === undefined || $scope.params.team_id === undefined ||
                !parseInt($scope.params.test_id) || !parseInt($scope.params.team_id)) {

                $scope.blockPage = true;
                $notification.alert("Id da prova inválido ou Id da turma inválido.");
                redirectToList("/Test");
                return;
            }

            if (parseInt($scope.params.test_id) !== $scope.testInformation.TestId || parseInt($scope.params.team_id) !== $scope.testInformation.Team.Id) {

                $scope.blockPage = true;
                $notification.alert("Id da prova não corresponde ou Id da turma não corresponde.");
                redirectToList("/Test");
                return;
            }

            if ($scope.testInformation.blockAccess) {
                $scope.blockPage = true;
                $notification.alert("Usuário não possui permissão para realizar essa ação.");
                redirectToList('/Test/IndexAdministrate?test_id=' + $scope.testInformation.TestId);
                return;
            }

            config();
            GetTestAveragesPercentagesByTest(function (){
                getDisciplines(function () {
                    getStudents($scope.carregaGrafico_Desempenho);
                });
            });
            getTeams();
        };


        /**
		 * @function Remover parenteses da label da alternativa
		 * @name removeBracket
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param {string} Label
		 * @return
		 */
        $scope.removeBracket = function __removeBracket(label) {

            if (label === undefined || label === null)
                return "";

            return label.replace(/["'()]/g, "");
        };


        /**
		 * @function Editar resultados da prova
		 * @name editResults
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.editResults = function __editResults() {

            var params = {
                'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0
            };

            CorrectionModel.unblockCorrection(params, function (result) {

                if (result.success) {

                    $notification.success("Prova reaberta para digitação com sucesso.");

                    $timeout(function () {
                        redirectToList("/Correction/IndexForm?test_id=" + $scope.testInformation.TestId + "&team_id=" + $scope.testInformation.Team.Id);
                    }, 2000);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };


        /**
		 * @function Exportar microdados
		 * @name exportMicrodata
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.exportMicrodata = function __exportMicrodata() {

            var params = {
                'test_id': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'team_id': ($scope.testInformation.Team.Id !== undefined && $scope.testInformation.Id !== null) ? $scope.testInformation.Team.Id : 0,
                'discipline_id': $scope.testInformation.discipline_id
            };

            CorrectionModel.getResultExport(params, function (result) {
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
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.cancel = function __cancel() {
            $window.location.href = '/Test/IndexStudentResponses?test_id=' + $scope.testInformation.TestId; //+ '&esc_id=' + $scope.testInformation.Team.esc_id
        };

        /**
         * @function arrivingStudentResponse.
         * @param
         * @return
         */
        $scope.arrivingStudentResponse = function __arrivingStudentResponse() {
            sessionStorage.setItem('arrivingStudentResponse', JSON.stringify(true));
            $window.location.href = '/Test/IndexStudentResponses?test_id=' + $scope.testInformation.TestId;
        };


        /**
		 * @function Forçar a atualização (diggest) do angular
		 * @name safeApply
		 * @namespace CorrectionResultController
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
		 * @function Chamado após tags html serem lidas pelo browser. 
		 * @name __postHtmlCompile
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        angular.element(document).ready(function __postHtmlCompile() {

        });

        /**
		 * @function Chamado após angular ser 'digerido' (diggest). 
		 * @name __cycleAngular
		 * @namespace CorrectionResultController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        var hasRegistered = false;
        $scope.$watch(function __cycleAngular() {
            if (hasRegistered) return;
            hasRegistered = true;
            $scope.$$postDigest(function __postDisgestAngular() {
                hasRegistered = false;
                //TODO
            });
        });

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

    };

})(angular, jQuery);