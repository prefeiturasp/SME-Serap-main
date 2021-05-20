/**
 * function FormElectronicTestController Controller
 * @namespace Controller
 * @author Jessica Sartori 12/06/2017
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("IndexElectronicTestResultController", IndexElectronicTestResultController);


    IndexElectronicTestResultController.$inject = ['$scope', '$notification', '$util', '$timeout', 'ElectronicTestResultModel', '$window', '$sce'];


    function IndexElectronicTestResultController(ng, $notification, $util, $timeout, ElectronicTestResultModel, $window, $sce) {


        function Init() {

            $notification.clear();
            ng.message = false;
            ng.test = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            ng.indiceItem = 0;
            ng.gabaritoAberto = false;
            ng.alternativaSelecionada = 0;
            ng.inicioProva = true;
            ng.pularRespondidas;
            angular.element('#modalConfirmacaoEntregaProva').modal('hide');
            ng.provaFinalizada = false;
            ng.showHeaderDetails = false;
            ng.enunciadoFontSize = 14;
            ng.alternativasFontSize = 14;
            load();
        };

        ng.loadHeaderDetais = function __loadHeaderDetais() {
            ng.showHeaderDetails = !ng.showHeaderDetails;
        };

        ng.abrirGabarito = function __abrirGabarito() {
            debugger;
            ng.gabaritoAberto = !ng.gabaritoAberto;
            if (ng.gabaritoAberto) {
                $('.gabarito-novo').addClass('abre');
            }
            else {
                $('.gabarito-novo').removeClass('abre');
            }
        };

        ng.proximaQuestao = function __proximaQuestao(item) {
            if (ng.indiceItem == ng.test.Itens.length - 1) return;
            if (!ng.pularRespondidas) {
                ng.indiceItem = ng.indiceItem + 1;
                ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
                return;
            }

            let nextItem = 0;
            for (nextItem = ng.indiceItem + 1; nextItem < ng.test.Itens.length; nextItem++) {
                if (!ng.tes.Itens[nextItem].Alternatives.some(x => x.Selected))
                    break;
            }

            ng.indiceItem = nextItem;
            ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
        };

        ng.voltaQuestao = function __voltaQuestao(item) {
            if (ng.indiceItem == 0) return;
            if (!ng.pularRespondidas) {
                ng.indiceItem = ng.indiceItem - 1;
                ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
                return;
            }

            let nextItem = 0;
            for (nextItem = ng.indiceItem - 1; nextItem < 0; nextItem--) {
                if (!ng.tes.Itens[nextItem].Alternatives.some(x => x.Selected))
                    break;
            }

            ng.indiceItem = nextItem;
            ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
        };

        Array.prototype.contains = function (obj) {
            var i = this.length;
            while (i--) {
                if (this[i] == obj) {
                    return true;
                }
            }
            return false;
        }

        function loadPagePros() {
            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;
        }

        function load() {
            loadPagePros();

            let testKeyStorage = getTestStorageKey(ng.testId);
            let testFromStorage = JSON.parse(localStorage.getItem(testKeyStorage));

            if (testFromStorage != null) {
                ng.test = testFromStorage;
                loadStudentCorrection();
            }
            else {
                ElectronicTestResultModel.loadByTestId({ test_id: ng.testId }, function (result) {
                    if (result.success) {
                        if (result.test.Id > 0) {
                            loadItens(result.test);
                        }
                        else {
                            ng.message = true;
                            ng.test = null;
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        // Load Itens paginated
        // --------------------
        function loadItens(test) {
            if (!test || test == null) {
                $notification['error']('Não foi possível localizar a prova informada. Por favor tente novamente.');
                return;
            }

            var page = 0;
            var pageItens = 5;
            test.Itens = [];
            loadItensPage(test, page, pageItens);
        };

        function loadItensPage(test, page, pageItens) {
            ElectronicTestResultModel.loadTestItensByTestId({ test_id: test.Id, page, pageItens }, function (result) {
                validateLoadItensPageResult(test, result, page, pageItens);
            });
        };

        function validateLoadItensPageResult(test, result, page, pageItens) {
            if (!result || !result.success) {
                $notification.alert('Não há itens carregados');
                return;
            }

            if (result.itens instanceof Array) {
                if (result.itens <= 0) {
                    finalizeLoadItens(test);
                }
                else {
                    test.Itens = test.Itens.concat(result.itens);
                    page++;
                    loadItensPage(test, page, pageItens);
                }
            }
            else {
                finalizeLoadItens(test);
            }
        };

        function finalizeLoadItens(test) {
            ng.test = test;

            let testKeyStorage = getTestStorageKey(ng.testId);
            localStorage.setItem(testKeyStorage, JSON.stringify(ng.test));
            loadStudentCorrection();
        };

        // --------------------

        function loadStudentCorrection() {
            ng.provaFinalizada = ng.test.QuantDiasRestantes <= 0;
            if (ng.admin) {
                finalizeLoad();
                return;
            }

            let keyStorage = getStudentCorrectionStorageKey(ng.testId, ng.aluId, ng.turId);
            var studentCorrection = JSON.parse(localStorage.getItem(keyStorage));

            if (studentCorrection != null) {
                updateChosenAlternatives(studentCorrection);
                return;
            }

            ElectronicTestResultModel.loadStudentCorrectionAsync({ test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId }, function (result) {
                if (result.success) {

                    if (result.studentCorrection == null) {
                        
                    }
                    else {
                        localStorage.setItem(keyStorage, JSON.stringify(result.studentCorrection));
                        updateChosenAlternatives(result.studentCorrection);
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        function updateChosenAlternatives(studentCorrection) {
            for (let i = 0; i < studentCorrection.Answers.length; i++) {
                var item = ng.test.Itens.find(x => x.Id == studentCorrection.Answers[i].ItemId);
                if (item == null) continue;

                var alternative = item.Alternatives.find(x => x.Id == studentCorrection.Answers[i].AlternativeId);
                if (alternative == null) continue;

                alternative.Selected = true;
            }

            if (studentCorrection.LastAnswer != null && ng.test.Itens.length > studentCorrection.LastAnswer + 1) {
                ng.indiceItem = studentCorrection.LastAnswer + 1;
            }

            ng.provaFinalizada = ng.provaFinalizada || studentCorrection.Done;
            
        };

        function finalizeLoad() {
            ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
            ng.inicioProva = false;
        };

        function getTestStorageKey(testId) {
            return "Test-" + testId;
        }

        function getStudentCorrectionStorageKey(testId, aluId, turId) {
            return "answerTest-" + testId + "-" + aluId + "-" + turId;
        }

        ng.zoomAlternativas = function (up) {
            if (up == true && ng.alternativasFontSize < 22)
                ng.alternativasFontSize = ng.alternativasFontSize + 4;
            
            if (up == false && ng.alternativasFontSize > 14)
                ng.alternativasFontSize = ng.alternativasFontSize - 4;

            $('label.alternativeElementClass p').css("font-size", ng.alternativasFontSize + "px");
        };

        ng.zoomEnunciado = function (up) {
            if (up == true && ng.enunciadoFontSize < 22)
                ng.enunciadoFontSize = ng.enunciadoFontSize + 4;

            if (up == false && ng.enunciadoFontSize > 14)
                ng.enunciadoFontSize = ng.enunciadoFontSize - 4;

            $('div.baseTextElementClass p').css("font-size", ng.enunciadoFontSize + "px");
            $('div.statementElementClass p').css("font-size", ng.enunciadoFontSize + "px");
        };

        ng.sair = sair;

        function sair() {
            location.href = document.referrer;
        }

        ng.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }

        Init();
    };
})(angular, jQuery);