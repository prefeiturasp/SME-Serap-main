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
            ng.DescricaoDoBotaoDaProvaDoAnoAtual = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            ng.load();
            configAccordion();
        };

        ng.load = function __load() {

            studentResultsListModel.load(function (result) {
                if (result.success) {
                    ng.ListaProvasDoAnoCorrente = [];
                    ng.ListaProvasDoAnoCorrente[0] = {
                        NomeDaProva: "EJA_Final2_Matemática_1º Semestre - Matemática",
                        Periodo: "Semestral",
                        DataDeFinalizacao: "25/02/2021",
                        TempoDeProva: "02 horas e 3 minutos",
                        QuantidadeDeItens: 32
                    };
                    ng.ProvasDoAnoCorrente = true;
                    ng.DescricaoDoBotaoDaProvaDoAnoAtual = "Provas do ano 2021";
                    //ng.ProvasDoAnoCorrente = false;
                    //ng.DescricaoDoBotaoDaProvaDoAnoAtual = "Não existem provas respondidas no ano 2021";
                    ng.ClassBotaoDaProvaDoAnoAtual = ng.ProvasDoAnoCorrente ? "nao-iniciadas" : "sem-registros";

                    ng.ListaProvasDosAnosAnteriores = [];
                    ng.ListaProvasDosAnosAnteriores[0] = {
                        NomeDaProva: "EJA_Final2_Matemática_1º Semestre - Matemática",
                        Periodo: "Semestral",
                        DataDeFinalizacao: "25/02/2021",
                        TempoDeProva: "02 horas e 3 minutos",
                        QuantidadeDeItens: 29
                    };
                    ng.ListaProvasDosAnosAnteriores[1] = {
                        NomeDaProva: "EJA_Final2_Matemática_1º Semestre - Matemática",
                        Periodo: "Semestral",
                        DataDeFinalizacao: "25/02/2021",
                        TempoDeProva: "02 horas e 3 minutos",
                        QuantidadeDeItens: 28
                    };
                    ng.ListaProvasDosAnosAnteriores[2] = {
                        NomeDaProva: "EJA_Final2_Matemática_1º Semestre - Matemática",
                        Periodo: "Semestral",
                        DataDeFinalizacao: "25/02/2021",
                        TempoDeProva: "02 horas e 3 minutos",
                        QuantidadeDeItens: 27
                    };

                    //if (result.ListaProvasDoAnoCorrente.length > 0) {
                    //    ng.ListaProvasDoAnoCorrente = result.ProvasDoAnoCorrente;
                    //}
                    //else {
                    //    ng.ListaProvasDoAnoCorrente = null;
                    //}

                    //if (result.ListaProvasDosAnosAnteriores.length > 0) {
                    //    ng.ListaProvasDosAnosAnteriores = result.ProvasDosAnosAnteriores;
                    //}
                    //else {
                    //    ng.ListaProvasDosAnosAnteriores = null;
                    //}
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
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