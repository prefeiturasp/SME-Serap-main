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
        .controller("FormElectronicTestController", FormElectronicTestController);


    FormElectronicTestController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', 'ElectronicTestModel', '$window', '$sce', '$http'];


    function FormElectronicTestController(ng, $notification, $location, $anchorScroll, $util, $timeout, ElectronicTestModel, $window, $sce, $http) {
        Init();

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
            ng.mensagemEntregaProva;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL ? true : false;
            ng.provaFinalizada = false;
            ng.showHeaderDetails = false;
            ng.enunciadoFontSize = 14;
            ng.alternativasFontSize = 14;
            ng.savingAnswers = false;

            load();
            initializeJobToSaveAnswers();
        };

        ng.loadHeaderDetais = function __loadHeaderDetais() {
            ng.showHeaderDetails = !ng.showHeaderDetails;
        };

        ng.abrirGabarito = function __abrirGabarito() {
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

        function load() {
            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            let testKeyStorage = getTestStorageKey(ng.testId);
            let testFromStorage = JSON.parse(localStorage.getItem(testKeyStorage));

            if (testFromStorage != null) {
                ng.test = testFromStorage;
                loadStudentCorrection();
            }
            else {
                ElectronicTestModel.loadByTestId({ test_id: ng.testId }, function (result) {
                    if (result.success) {

                        if (result.test.Id > 0) {
                            ng.test = result.test;
                            localStorage.setItem(testKeyStorage, JSON.stringify(ng.test));
                            loadStudentCorrection();
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

        function loadStudentCorrection() {
            let keyStorage = getStudentCorrectionStorageKey(ng.testId, ng.aluId, ng.turId);
            var studentCorrection = JSON.parse(localStorage.getItem(keyStorage));

            if (studentCorrection != null) {
                updateChosenAlternatives(studentCorrection);
                return;
            }

            ElectronicTestModel.loadStudentCorrectionAsync({ test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId }, function (result) {
                if (result.success) {

                    if (result.studentCorrection == null) {
                        finalizeLoad();
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

            ng.provaFinalizada = studentCorrection.Done;
            finalizeLoad();
        };

        function finalizeLoad() {
            ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.test.Itens[ng.indiceItem].BaseTextDescription != null && ng.test.Itens[ng.indiceItem].BaseTextDescription != "";
            ng.inicioProva = false;
        };

        ng.handleRadioClick = function (chosenAlternative) {
            if (chosenAlternative.Selected) return;

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            var answer = {
                ItemId: chosenAlternative.Item_Id,
                AlternativeId: chosenAlternative.Id,
                Changed: true
            };

            let keyStorage = getStudentCorrectionStorageKey(ng.testId, ng.aluId, ng.turId);
            var studentCorrection = JSON.parse(localStorage.getItem(keyStorage));

            if (studentCorrection == null) {
                studentCorrection = {
                    Done: false,
                    Answers: []
                };
                studentCorrection.Answers.push(answer);
            }
            else {
                studentCorrection.Answers = addOrReplaceAnswer(studentCorrection.Answers, answer);
            }

            studentCorrection.LastAnswer = ng.indiceItem;
            localStorage.setItem(keyStorage, JSON.stringify(studentCorrection));

            var item = ng.test.Itens.find(x => x.Id == chosenAlternative.Item_Id);
            for (let i = 0; i < item.Alternatives.length; i++) {
                item.Alternatives[i].Selected = item.Alternatives[i].Id == chosenAlternative.Id;
            }
        };

        function addOrReplaceAnswer(answers, newAnswer) {
            if (answers.some(x => x.ItemId === newAnswer.ItemId)) {
                answers = answers.map(a => newAnswer.ItemId === a.ItemId ? newAnswer : a);
            }
            else {
                answers.push(newAnswer);
            }
            return answers;
        };

        function getTestStorageKey(testId) {
            return "Test-" + testId;
        }

        function getStudentCorrectionStorageKey(testId, aluId, turId) {
            return "answerTest-" + testId + "-" + aluId + "-" + turId;
        }

        function saveAnswers() {
            if (ng.savingAnswers) return;

            let keyStorage = getStudentCorrectionStorageKey(ng.testId, ng.aluId, ng.turId);
            var studentCorrection = JSON.parse(localStorage.getItem(keyStorage));
            if (studentCorrection == null || studentCorrection.Answers.length <= 0) return;

            var changedAnswers = studentCorrection.Answers.filter(x => x.Changed);
            if (changedAnswers == null || changedAnswers.length <= 0) return;
            for (var i = 0; i < changedAnswers.length; i++)
                changedAnswers[i].Changed = false;

            ng.savingAnswers = true;
            ng.$digest();

            $.ajax({
                url: base_url('ElectronicTest/SaveAnswersAsync'),
                data: { test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId, ordemItem: ng.test.Itens[ng.indiceItem].ItemOrder, answers: changedAnswers },
                type: "POST",
                dataType: "JSON",
                success: function (result) {
                    if (result.success) {
                        var recentStudentCorrection = JSON.parse(localStorage.getItem(keyStorage));
                        recentStudentCorrection.Answers = recentStudentCorrection.Answers.map(a => changedAnswers.find(x => x.ItemId == a.ItemId && x.AlternativeId == a.AlternativeId) ?? a);
                        localStorage.setItem(keyStorage, JSON.stringify(recentStudentCorrection));
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                    ng.savingAnswers = false;
                    ng.$digest();
                },
                error: function (result) {
                    $notification[result.type ? result.type : 'error'](result.message);
                    ng.savingAnswers = false;
                    ng.$digest();
                }
            });
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

        ng.sair = function () {
            stopJobToSaveAnswers();
            $window.location.href = '/ElectronicTest/Index';
        }

        function initializeJobToSaveAnswers() {
            ng.intervalToSaveAnswers = setInterval(saveAnswers, 60000);
        };

        function stopJobToSaveAnswers() {
            clearInterval(ng.intervalToSaveAnswers);
        };

        ng.openModalEntregaProva = function () {
            let itensNaoPreenchidos = [];

            for (var i = 0; i < ng.test.Itens.length; i++) {
                if (ng.test.Itens[i].Alternatives.some(x => x.Selected))
                    continue;

                itensNaoPreenchidos.push(ng.test.Itens[i].ItemOrder + 1);
            }

            if (itensNaoPreenchidos.length > 0) {
                ng.mensagemEntregaProva = "O(s) item(ns) " + itensNaoPreenchidos.join() + " não está(ão) preenchido(s). Deseja entregar a prova mesmo assim?";
            }
            else {
                ng.mensagemEntregaProva = "Deseja entregar a prova?";
            }
            angular.element("#modalConfirmacaoEntregaProva").modal({ backdrop: 'static' });
        };

        ng.entregarProva = function __entregarProva() {

            stopJobToSaveAnswers();
            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;
            
            saveAnswers();

            ElectronicTestModel.finalizeCorrection({ tur_id: ng.turId, test_id: ng.testId, alu_id: ng.aluId }, function (result) {
                if (result.success) {
                    $notification.success(result.message);

                    let keyStorage = getStudentCorrectionStorageKey(ng.testId, ng.aluId, ng.turId);
                    var studentCorrection = JSON.parse(localStorage.getItem(keyStorage));
                    studentCorrection.Done = true;
                    studentCorrection.LastAnswer = 0;
                    localStorage.setItem(keyStorage, JSON.stringify(studentCorrection));

                    angular.element("#modalConfirmacaoEntregaProva").modal("hide");
                    $window.location.href = '/ElectronicTest/Index';
                }
                else {
                    initializeJobToSaveAnswers();
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            });
        };

        ng.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }
    };
})(angular, jQuery);