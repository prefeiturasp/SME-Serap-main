/**
 * function Cadastro/Edição Disciplina
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("FormKnowledgeAreaController", FormKnowledgeAreaController);

    FormKnowledgeAreaController.$inject = ['$scope', '$rootScope', '$window', '$notification', 'DisciplineModel', 'KnowledgeAreaModel', '$util'];

    function FormKnowledgeAreaController($scope, $rootScope, $window, $notification, DisciplineModel, KnowledgeAreaModel, $util) {

        function Init() {
            $notification.clear();
            $scope.disciplines = undefined;
            $scope.loadDisciplines();
            
        };

        $scope.salvar = function __salvar(typeDisciplines) {

            if ($scope.verifica()) {
                var m = {};

                var obj = [];

                for (var key in typeDisciplines) {
                    var newItem = { Discipline_Id: typeDisciplines[key].Id };
                    if (obj.indexOf(newItem) === -1) {
                        obj.push(newItem);
                    }
                }

                m.Id = $scope.params.Id ? $scope.params.Id : 0;
                m.Description = $scope.Description;
                m.KnowledgeAreaDisciplines = obj;

                KnowledgeAreaModel.save(m, function (result) {

                    if (result.success) {

                        $scope.disciplines = undefined;

                        $notification.success(result.message);
                        $window.location.href = '/KnowledgeArea/List';
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.loadDisciplines = function __loadDisciplines() {

            DisciplineModel.searchAllDisciplines(function (result) {
                if (result.success) {

                    $scope.disciplines = result.lista;
                    if ($scope.params.Id != undefined) {
                        loadKnowledgeArea($scope.params.Id);
                    }

                    if ($scope.disciplines.length == 0)
                        $notification.alert("Não existem componente(s) curricular(es) cadastrado(s).");

                    return;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });

            $scope.typeDiscipline = { disciplines: [] };
        };

        $scope.verifica = function __verifica() {

            if ($scope.typeDiscipline != undefined) {

                if ($scope.typeDiscipline.disciplines.length <= 0) {

                    $notification.alert('É necessário selecionar ao menos um componente curricular.');

                    return false;
                }
            }
            return true;
        };

        /**
        * @function Carrega parametros quando acessar pagina
        * @private
        * @param {Object} current
        */
        function loadKnowledgeArea(id) {

            if (id != undefined && id > 0) {
                var bd = { Id: id };

                //Pega elemento caso exista, senão cria um novo
                KnowledgeAreaModel.find(bd, function (result) {

                    if (result.success) {
                        $scope.KnowledgeArea = result.knowledgeArea;

                        var obj = [];

                        for (var key in $scope.KnowledgeArea.KnowledgeAreaDisciplines) {

                            obj.push({
                                Id: $scope.KnowledgeArea.KnowledgeAreaDisciplines[key].Discipline_Id
                            });
                        }

                        var disciplinesBD = $scope.disciplines;
                        var disciplinasSelecionadas = procurarElementoEm(obj, disciplinesBD);

                        $scope.typeDiscipline = { disciplines: disciplinasSelecionadas };
                        $scope.Description = $scope.KnowledgeArea.Description;
                    }
                    else {
                        $notification[result.type ? result.type: 'error'](result.message);
                    }
                });
            }
            else {
                $notification[result.type ? result.type : 'error'](result.message);
            }
        };

        /**
		* @function Procura elementos
		* @private
		* @param elm = objeto a ser procurado 
		* @param arr = lista de objetos 
		*/
        function procurarElementoEm(elm, arr) {
            var i;

            if (elm['length'] && elm['length'] > 0) {

                var a = [], q = 0;

                //Varre itens e array comparando Id, se for igual guarda num array a referencia do elemento do array
                for (i = 0; i < elm.length; i++) {
                    for (q = 0; q < arr.length; q++) {
                        if (elm[i] != null) {
                            if (elm[i].Id === arr[q].Id) {
                                a.push(arr[q]);
                                break;
                            }
                        }
                    }
                }
                if (a.length < 1)
                    return elm;
                return a;
            }
            else {

                if (elm['length'] && elm['length'] === 1)
                    elm = elm[0];
                i = arr.indexOf(elm);

                if (i < 0 || !i || i !== 0)
                    return null;

                return arr[i];
            }
        };

        $scope.params = $util.getUrlParams();
        Init();
    };

})(angular, jQuery);