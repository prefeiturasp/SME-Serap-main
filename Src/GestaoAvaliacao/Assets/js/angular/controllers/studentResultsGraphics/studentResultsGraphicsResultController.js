(function (angular, $) {

    'use strict';

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
        .controller("StudentResultsGraphicsResultController", StudentResultsGraphicsResultController);


    StudentResultsGraphicsResultController.$inject = ['$rootScope', '$scope', '$window', '$sce', '$util', '$notification', '$timeout', 'StudentResultsGraphicsModel', 'TestAdministrateModel', 'DisciplineModel', 'ItemModel', 'TestModel'];


    /**
	 * @function Controller 'Resultados da prova'
	 * @name StudentResultsGraphicsResultController
	 * @namespace Controller
	 * @memberOf appMain
	 * @param {Object} $rootScope
	 * @param {Object} $scope
	 * @param {Object} $window
	 * @param {Object} $sce
	 * @param {Object} $util
	 * @param {Object} $notification
	 * @param {Object} $timeout
	 * @param {Object} StudentResultsGraphicsModel
	 * @param {Object} TestAdministrateModel
	 * @return
	 */
    function StudentResultsGraphicsResultController($rootScope, $scope, $window, $sce, $util, $notification, $timeout, StudentResultsGraphicsModel, TestAdministrateModel, DisciplineModel, ItemModel, TestModel) {


        /**
		  * @function Redirecionar para listagem de provas.
		  * @name redirectToList
		  * @namespace StudentResultsGraphicsResultController
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
         * @function arrivingStudentResponse.
         * @param
         * @return
         */
        $scope.arrivingStudentResponse = function __arrivingStudentResponse() {
            $window.location.href = '/StudentResults/Index';
        };

        /**
		 * @function Inicialização das informações da prova
		 * @name configTestInformation
		 * @namespace StudentResultsGraphicsResultController
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
                TurId: informations.TurId,
                EscId: informations.EscId,
                DreId: informations.DreId,
                Turma: informations.Turma,
                Ano: informations.Ano,
                SchoolName: informations.schoolName,
                Tests: informations.Tests,
                Anos: informations.AnosDeAplicacaoDaProva
            };
        };


        function config() {

            $scope.percentualDeAcerto = {};
            $scope.chart1 = {
                visible: false,
                chart: {}
            };
        };


        /**
		 * @function Carregar percentual de acerto
		 * @name getPercentualDeAcerto
		 * @namespace StudentResultsGraphicsResultController
		 * @memberOf Controller
		 * @private
		 * @param
		 * @return
		 */
        function getPercentualDeAcerto(_callback) {
            var params = {
                'TestId': ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0,
                'TurId': $scope.testInformation.TurId,
                'EscId': $scope.testInformation.EscId,
                'DreId': $scope.testInformation.DreId
            };
            
            StudentResultsGraphicsModel.getPercentualDeAcerto(params, function (result) {

                if (result.success) {
                    if (result.percentualDeAcerto === null || result.percentualDeAcerto === undefined)
                        return;

                    $scope.percentualDeAcerto = result.percentualDeAcerto;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

                if (_callback)
                    _callback();
            });
        };

        function loadByTests() {
            var canvas = document.getElementById('gfcPerformance');
            var ctx = canvas.getContext("2d");
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            config();

            var params = {
                'Ano': $scope.testInformation ? $scope.testInformation.Ano : ($scope.params.Ano ? $scope.params.Ano : 0),
            };
            StudentResultsGraphicsModel.getTests(params, function (result) {

                if (result.success) {
                    if (result.dados === null || result.dados === undefined)
                        return;

                    $scope.testInformation.Tests = result.dados;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }
        $scope.loadByTests = loadByTests;

        function loadGraficoDeDesempenho(test) {
            debugger;
            var testId = ($scope.testInformation.TestId !== undefined || $scope.testInformation.TestId !== null) ? $scope.testInformation.TestId : 0;
            //if (testId && testId > 0)
            //    getPercentualDeAcerto($scope.carregaGrafico_Desempenho);
        }
        $scope.loadGraficoDeDesempenho = loadGraficoDeDesempenho;

        /*
          @function Montar gráfico com percentual de acerto
        */
        $scope.carregaGrafico_Desempenho = function () {
            if ($scope.percentualDeAcerto != null) {

                var labelsPercentualDeAcerto = ["Aluno", "Turma", "DRE", "SME"];
                var perncetualDeAcerto = [
                    $scope.percentualDeAcerto.PercentualDeAcertoAluno,
                    $scope.percentualDeAcerto.PercentualDeAcertoTurma,
                    $scope.percentualDeAcerto.PercentualDeAcertoDRE,
                    $scope.percentualDeAcerto.PercentualDeAcertoSME
                ];
                var data = {
                    datasets: [
                        {
                            type: 'horizontalBar',
                            data: perncetualDeAcerto,
                            backgroundColor: [
                                "#5c92d8",
                                "orange",
                                "green",
                                "magenta"
                            ],
                            xAxisID: 'x-axis-0'
                        },
                    ],
                    labels: labelsPercentualDeAcerto
                };

                //limpar gráfico antigo
                var divParent = document.getElementById('gfcPerformance').parentElement;
                document.getElementById('gfcPerformance').remove();
                divParent.innerHTML = '<canvas id="gfcPerformance" style="max-width: 700px; width:700px; overflow-x: auto; overflow-y: hidden;"></canvas>';

                var canvas = document.getElementById('gfcPerformance');
                var ctx = canvas.getContext("2d");

                canvas.height = 200 + (labelsPercentualDeAcerto.length) * 30;
                canvas.width = 700;

                $scope.chart1 = {
                    visible: true, chart: new Chart(ctx, {
                        type: 'horizontalBar',
                        options: {
                            responsive: false,
                            maintainAspectRatio: false,
                            legend: {
                                display: false
                            },
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
                                            return value + ""
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

        /*---------------------------------------------------------------------*/

        $scope.init = function __init() {
            $scope.params = $util.getUrlParams();

            var params = {
                'TestId': $scope.params.TestId ? $scope.params.TestId : 0,
                'Ano': $scope.testInformation ? $scope.testInformation.Ano : ($scope.params.Ano ? $scope.params.Ano : 0),
            };

            $scope.params = params;

            StudentResultsGraphicsModel.getDataTest(params, function (result) {
                if (result.success) {
                    $scope.getDataTest(result.dados);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    $timeout(function __redirectError() { $window.history.back(); }, 1300);
                }
            });
        };

        /**
		 * @function Obter objeto inicial para configuração da page.
		 * @name getDataTest
		 * @namespace StudentResultsGraphicsResultController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.getDataTest = function __getDataTest(dadosDaProva) {
            if (dadosDaProva === undefined || dadosDaProva === null) {
                $notification.alert("Não foi possível obter as permissões de acesso a página.");
                return;
            }

            configTestInformation(dadosDaProva);

            if ($scope.params === undefined ||
                $scope.testInformation.TestId === undefined || $scope.testInformation.TestId === null ||
                $scope.params.TestId === undefined) {
                $notification.alert("Id da prova inválido.");
                return;
            }

            config();
            getPercentualDeAcerto($scope.carregaGrafico_Desempenho);
        };

        /**
		 * @function Chamado após tags html serem lidas pelo browser. 
		 * @name __postHtmlCompile
		 * @namespace StudentResultsGraphicsResultController
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
		 * @namespace StudentResultsGraphicsResultController
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
    };

})(angular, jQuery);