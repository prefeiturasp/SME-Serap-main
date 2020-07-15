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


    FormElectronicTestController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', 'ElectronicTestModel', '$window', '$sce'];


    function FormElectronicTestController(ng, $notification, $location, $anchorScroll, $util, $timeout, ElectronicTestModel, $window, $sce) {
        Init();

        function Init() {

            $notification.clear();
            ng.message = false;
            ng.test = null;
            ng.itens = null;
            ng.alternatives = null;
            ng.idsItens = null;
            ng.dadosAlternativaSelecionada = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            load();
            ng.gabaritoAberto = false;
            ng.indiceItem = 0;
            ng.alternativaSelecionada = 0;
            ng.inicioProva = true;
            ng.pularRespondidas;
            ng.itensNaoPreenchidos = [];
            ng.itensPreenchidos = [];
            angular.element('#modalConfirmacaoEntregaProva').modal('hide');
            ng.mensagemEntregaProva;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL ? true : false;
            ng.provaFinalizada = false;

            ng.videos = null;
            ng.audios = null;
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
            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            if (ng.itensPreenchidos.length !== 0 && ng.pularRespondidas) {
                var contador = 1;

                while (ng.itens.length > contador && ng.itensPreenchidos.contains(item.ItemOrder + contador)) {
                    contador++;
                }

                if ((ng.indiceItem + contador) >= ng.itens.length) {
                    ng.indiceItem = ng.itens.length - 1;
                }
                else {
                    ng.indiceItem = ng.indiceItem + contador;
                }
            }
            else {
                ng.indiceItem = ng.indiceItem + 1;
            }

            ng.possuiTextoBase = ng.itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";

        };

        ng.voltaQuestao = function __voltaQuestao(item) {
            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            if (ng.itensPreenchidos.length !== 0 && ng.pularRespondidas) {
                var contador = 1;
                var ordem = item.ItemOrder;

                while (ordem > 0 && ng.itensPreenchidos.contains(ordem - 1)) {
                    contador++;
                    ordem--;
                }

                if ((ng.indiceItem - contador) <= 0) {
                    ng.indiceItem = ng.indiceItem - ng.indiceItem;
                }
                else {
                    ng.indiceItem = ng.indiceItem - contador;
                }
            }
            else {
                ng.indiceItem = ng.indiceItem - 1;
            }

            ng.possuiTextoBase = ng.itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";
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

            ElectronicTestModel.loadByTestId({ test_id: ng.testId }, function (result) {
                if (result.success) {

                    if (result.test.Id > 0) {
                        ng.test = result.test;
                        CarregarItens();
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
        };

        function CarregarItens() {

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            ElectronicTestModel.loadItensByTestId({ test_id: ng.testId }, function (result) {
                if (result.success) {
                    if (result.itens.length > 0) {
                        ng.itens = result.itens;
                        ng.idsItens = ng.itens.map(function (v) {
                            return v.Item_Id;
                        });

                        CarregarAlternativas();
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

        function CarregarAlternativas() {

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            ElectronicTestModel.loadAlternativesByItens({ itens: ng.idsItens, test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId }, function (result) {
                if (result.success) {
                    if (result.alternatives.length > 0) {
                        ng.alternatives = result.alternatives;

                        if (!result.existemDados) {
                            ng.indiceItem = 0;
                        }
                        if (result.existemDados && (ng.itens.length > result.ordemUltimaResposta + 1)) {
                            ng.indiceItem = result.ordemUltimaResposta + 1;
                        }

                        ng.possuiTextoBase = ng.itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";

                        ng.provaFinalizada = result.provaFinalizada;
                        ng.inicioProva = false;
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

        ng.handleRadioClick = function (alternativa) {

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            ng.alternativaSelecionada = alternativa;

            ElectronicTestModel.save({ alternativa: ng.alternativaSelecionada, test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId, ordemItem: ng.itens[ng.indiceItem].ItemOrder }, function (result) {
                if (result.success) {
                    var alternativaSelecionadaIndex = ng.alternatives.findIndex(x => x.Id == ng.alternativaSelecionada.Id);
                    ng.alternatives[alternativaSelecionadaIndex].Selected = true;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            });

        };

        ng.sair = function () {
            $window.location.href = '/ElectronicTest/Index';
        }

        ng.openModalEntregaProva = function () {

            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            ng.mensagemEntregaProva = '';
            if (ng.itensPreenchidos.length !== ng.itens.length) {

                ng.mensagemEntregaProva = "O(s) item(ns) ";

                for (var i = 0; i < ng.itens.length; i++) {
                    if (!ng.itensPreenchidos.contains(ng.itens[i].ItemOrder)) {
                        ng.mensagemEntregaProva += ng.itens[i].ItemOrder + 1 + ", ";
                    }
                }
                ng.mensagemEntregaProva = ng.mensagemEntregaProva.substring(0, (ng.mensagemEntregaProva.length - 2));

                ng.mensagemEntregaProva += " não está(ão) preenchido(s). Deseja entregar a prova mesmo assim?";
            }
            else {
                ng.mensagemEntregaProva = "Deseja entregar a prova?";
            }

            angular.element("#modalConfirmacaoEntregaProva").modal({ backdrop: 'static' });
        };

        ng.entregarProva = function __entregarProva() {

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            ElectronicTestModel.finalizeCorrection({ tur_id: ng.turId, test_id: ng.testId, alu_id: ng.aluId }, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                    angular.element("#modalConfirmacaoEntregaProva").modal("hide");
                    $window.location.href = '/ElectronicTest/Index';
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            });


        };

        ng.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }
    };
})(angular, jQuery);