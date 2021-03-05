(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("StudentResultsController", StudentResultsController);


    StudentResultsController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', '$window', 'studentResultsListModel'];

    function StudentResultsController(ng, $notification, $location, $anchorScroll, $util, $timeout, $window, studentResultsListModel) {

        function Init() {
            $notification.clear();
            ng.message = false;
            ng.ListaProvasDoAnoCorrente = null;
            ng.ListaProvasDosAnosAnteriores = null;
            ng.ProvasDoAnoCorrente = true;
            ng.ClassBotaoDaProvaDoAnoAtual = "nao-iniciadas";
            ng.DescricaoDoBotaoDaProvaDoAnoAtual = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            ng.Ano = null;
            ng.load();
            configAccordion();
        };

        ng.load = function __load() {

            studentResultsListModel.getResultadosDosEstudantes(function (result) {
                if (result.success) {
                    ng.Ano = result.dados.Ano;

                    if (result.dados.ListaProvasDoAnoCorrente.length > 0) {
                        ng.ListaProvasDoAnoCorrente = result.dados.ListaProvasDoAnoCorrente;
                        ng.DescricaoDoBotaoDaProvaDoAnoAtual = "Provas do ano " + result.dados.Ano;
                        ng.ProvasDoAnoCorrente = true;
                        ng.ClassBotaoDaProvaDoAnoAtual = "nao-iniciadas";
                    }
                    else {
                        ng.ListaProvasDoAnoCorrente = null;
                        ng.DescricaoDoBotaoDaProvaDoAnoAtual = "Não existem provas respondidas no ano " + result.dados.Ano;
                        ng.ProvasDoAnoCorrente = false;
                        ng.ClassBotaoDaProvaDoAnoAtual = "sem-registros";
                    }

                    if (result.dados.ListaProvasDosAnosAnteriores.length > 0) {
                        ng.ListaProvasDosAnosAnteriores = result.dados.ListaProvasDosAnosAnteriores;
                    }
                    else {
                        ng.ListaProvasDosAnosAnteriores = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        ng.abrirGraficoDaProva = function(ano, testeId) {
            redirectToList("/StudentResultsGraphics/Index?Ano="+ano+"&TestId="+testeId);
        }

        function redirectToList(destiny) {
            $timeout(function __invalidTestId() {
                $window.location.href = destiny;
            }, 1000);
        };

        ng.getListLength = function (list) {
            if (list == null) return 0;
            if (list === undefined) return 0;
            return list.length;
        }

        function configAccordion() {
            var acc = document.getElementsByClassName("accordion-test");

            for (var i = 0; i < acc.length; i++) {
                acc[i].addEventListener("click", function () {
                    this.classList.toggle("active");
                    var panel = this.nextElementSibling;
                    if (panel.style.maxHeight) {
                        panel.style.maxHeight = null;
                    } else {
                        panel.style.maxHeight = panel.scrollHeight + "px";
                    }
                });
            }
        };

        Init();
    };
})(angular, jQuery);