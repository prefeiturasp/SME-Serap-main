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
        .controller("FormDisciplineController", FormDisciplineController);

    FormDisciplineController.$inject = ['$scope', '$rootScope', '$window', '$notification', 'DisciplineModel', 'LevelEducationModel'];

    function FormDisciplineController($scope, $rootScope, $window, $notification, DisciplineModel, LevelEducationModel) {

        function Init() {
            $notification.clear();
        };

        $scope.salvar = function __salvar(typeDisciplines) {

            if ($scope.verifica()) {

                var obj = [];

                for (var key in typeDisciplines) {

                    obj.push({
                        TypeLevelEducationId: $scope.selectedObjTipoNivelEnsino.Id,
                        Description: typeDisciplines[key].Description,
                        DisciplineTypeId: typeDisciplines[key].Id
                    });
                }

                DisciplineModel.saveRange(obj, function (result) {

                    if (result.success) {

                        $scope.disciplines = undefined;

                        $notification.success(result.message);
                        $window.location.href = '/Discipline/List';
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.carregaLevelEducation = function __carregaLevelEducation(result) {

            LevelEducationModel.load({}, function (result) {

                if (result.success) {

                    $scope.tipoNivelEnsinoList = result.lista;

                    $scope.$watch('selectedObjTipoNivelEnsino', $scope.loadDisciplines);
                }
                else {

                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.loadDisciplines = function __loadDisciplines() {

            $scope.disciplines = undefined;

            if ($scope.selectedObjTipoNivelEnsino != undefined) {

                DisciplineModel.searchDisciplines({ typeLevelEducation: $scope.selectedObjTipoNivelEnsino.Id },
                function (result) {

                    if (result.success) {

                        $scope.disciplines = result.lista;

                        if ($scope.disciplines.length == 0)
                            $notification.alert("Não existem componente(s) curricular(es) cadastrado(s) para esse nível de ensino.");

                        return;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });

                $scope.typeDiscipline = { disciplines: [] };
            }
        };

        $scope.verifica = function __verifica() {

            if ($scope.selectedObjTipoNivelEnsino) {

                if ($scope.selectedObjTipoNivelEnsino.id <= 0) {

                    $notification.alert('É necessário selecionar um nível de ensino.');

                    return false;
                }
            }
            else {

                $notification.alert('É necessário selecionar um nível de ensino.');
                
                return false;
            }

            if ($scope.typeDiscipline != undefined) {

                if ($scope.typeDiscipline.disciplines.length <= 0) {

                    $notification.alert('É necessário selecionar ao menos um componente curricular.');

                    return false;
                }
            }
            return true;
        };

        Init();
    };

})(angular, jQuery);