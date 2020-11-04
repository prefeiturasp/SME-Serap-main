/**
 * function IndexElectronicTestController Controller
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
        .controller("IndexElectronicTestController", IndexElectronicTestController);


    IndexElectronicTestController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', 'ElectronicTestModel', '$window'];


    function IndexElectronicTestController(ng, $notification, $location, $anchorScroll, $util, $timeout, ElectronicTestModel, $window) {

        function Init() {

            $notification.clear();
            ng.message = false;
            ng.eletronicTestListNaoIniciadas = null;
            ng.eletronicTestListEmAndamento = null;
            ng.eletronicTestFinalizadas = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            ng.load();
            configAccordion();
        };

        ng.load = function __load() {
            
            ElectronicTestModel.load(function (result) {
                if (result.success) {
                    if (result.listaNaoIniciada.length > 0) {
                        ng.eletronicTestListNaoIniciadas = result.listaNaoIniciada;
                    }
                    else {
                        ng.eletronicTestListNaoIniciadas = null;
                    }

                    if (result.listaEmAndamento.length > 0) {
                        ng.eletronicTestListEmAndamento = result.listaEmAndamento;
                    }
                    else {
                        ng.eletronicTestListEmAndamento = null;
                    }

                    if (result.listaFinalizadas.length > 0) {
                        ng.eletronicTestFinalizadas = result.listaFinalizadas;
                    }
                    else {
                        ng.eletronicTestFinalizadas = null;
                    }

                    if (ng.eletronicTestListEmAndamento == null && ng.eletronicTestFinalizadas == null) {
                        ng.message = true;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        ng.abrirProvaNaoIniciada = function (obj) {
            $window.location.href = '/ElectronicTest/Form?TestId=' + obj.Id + '&AluId=' + obj.alu_id + '&TurId=' + obj.tur_id;
        };

        ng.abrirProvaEmProgresso = function (obj) {
            $window.location.href = '/ElectronicTest/Form?TestId=' + obj.Id + '&AluId=' + obj.alu_id + '&TurId=' + obj.tur_id;
        };

        ng.abrirProvaFinalizada = function (obj) {
            $window.location.href = '/ElectronicTest/Form?TestId=' + obj.Id + '&AluId=' + obj.alu_id + '&TurId=' + obj.tur_id;
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