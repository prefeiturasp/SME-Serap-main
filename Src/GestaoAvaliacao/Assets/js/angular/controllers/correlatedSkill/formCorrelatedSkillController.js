/**
 * function Consulta/Cadastro de correlação de habilidade
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
        .module('appMain')
        .controller("FormCorrelatedSkillController", FormCorrelatedSkillController);

    FormCorrelatedSkillController.$inject = ['$scope', '$rootScope', '$notification', '$pager', '$util', 'CorrelatedSkillModel', 'EvaluationMatrixModel', 'SkillModel'];

    function FormCorrelatedSkillController($scope, rootScope, $notification, $pager, $util, CorrelatedSkillModel, EvaluationMatrixModel, SkillModel) {

        $scope.params = $util.getUrlParams();

        function Init() {

            $notification.clear();
            $scope.init();
        };

        function setEvents() {

            $scope.$watch('skills', $scope.carregarTemp, true);
            $scope.$watch('skills2', $scope.carregarTemp2, true);
            $scope.$watch('selectedObjEvaluationMatrix1', $scope.carregaEvaluationMatrix2);
            $scope.$watch('selectedObjEvaluationMatrix2', $scope.carregaSkill2);
        };

        $scope.init = function __init() {

            $scope.paginate = $pager(CorrelatedSkillModel.loadList);
            $scope.pesquisa = '';
            $scope.message = false;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.pageSize = 10;
            $scope.skillsCopy = [];
            $scope.skillsCopy2 = [];
            $scope.text = "";

            $scope.correlatedSkillList = null;
            $scope.correlatedSkill = null;

            $scope.Skill1 = { id: '' };
            $scope.Skill2 = { id: '' };
            $scope.skills = [];
            $scope.skills2 = [];

            setEvents();

            $scope.carregaEvaluationMatrix1();
        }

        $scope.pager = function __pager() {
            $scope.paginate.indexPage(0);
            $scope.pages = 0;
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.totalItens = 0;
        };

        $scope.load = function __load() {
            $scope.paginate.paginate({ MatrizId: $scope.selectedObjEvaluationMatrix1.Id }).then(function (result) {
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.correlatedSkillList = result.lista;
                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    }
                    else {
                        $scope.message = true;
                        $scope.correlatedSkillList = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                $scope.message = true;
                $scope.correlatedSkillList = null;
            });
        };

        $scope.confirmar = function __confirmar(correlatedSkill) {
            $scope.correlatedSkill = correlatedSkill;
            angular.element('#modal').modal('show');
        };

        $scope.carregaEvaluationMatrix1 = function __carregaEvaluationMatrix1(result) {

            EvaluationMatrixModel.loadComboSimple(function (result) {

                if (result.success) {
                    $scope.evaluationMatrixList1 = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.carregaSkill = function __carregaSkill(result) {

            if ($scope.selectedObjEvaluationMatrix1 != undefined) {

                SkillModel.getByMatriz({ Id: $scope.selectedObjEvaluationMatrix1.Id }, function (result) {

                        if (result.success) {

                            var _skills = result.lista;
                            var formatedList = [];
                            for (var key in _skills) {
                                formatedList.push({
                                    Id: _skills[key].ModelSkillLevels.Id,
                                    Description: _skills[key].ModelSkillLevels.Description,
                                    objSkill: undefined,
                                    indexSkill: key,
                                    lista: _skills[key].ModelSkillLevels.Skills
                                });
                            }
                            $scope.skills = formatedList;
                            $scope.skillsCopy = angular.copy(formatedList);
                            return;
                        }
                        else {
                            $scope.skills = [];
                            $notification[result.type ? result.type : 'error'](result.message);
                        }
                    });
            }
        };

        $scope.carregarTemp = function __carregarTemp(newValue, oldValue) {

            for (var i = 0; i < oldValue.length; i++) {

                if (newValue[i] != undefined) {

                    if (!angular.equals(oldValue[i].objSkill, newValue[i].objSkill)) {

                        for (var j = ($scope.skills.indexOf(newValue[i]) + 1) ; j < newValue.length; j++) {
                            $scope.skills[j].objSkill = null;
                            $scope.skills[j].lista = [];
                        }

                        if ((i + 1) < newValue.length) {
                            if (newValue[i].objSkill != undefined)
                                $scope.carregarCascadeSkill(newValue[i].objSkill.Id, ($scope.skills.indexOf(newValue[i]) + 1));
                        }

                        return;
                    }
                }
            }
        };

        $scope.carregarCascadeSkill = function __carregarCascadeSkill(_parentID, childrenIndex) {

            if (_parentID != undefined) {

                SkillModel.getByParent({ Id: _parentID }, function (result) {

                    if (result.success) {
                        var _skills = [];
                        for (var key in result.lista) {
                            $scope.skills[childrenIndex].lista.push(result.lista[key].Skills);
                            $scope.skillsCopy[childrenIndex].lista.push(angular.copy(result.lista[key].Skills));
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };
 
        $scope.carregaEvaluationMatrix2 = function __carregaEvaluationMatrix2() {

            $scope.carregaSkill2();
            $scope.carregarCascadeSkill2();

            if ($scope.selectedObjEvaluationMatrix1 != undefined) {

                var list = $scope.evaluationMatrixList1;
                list = $.grep(list, function (val, index) {
                    return val.Id != $scope.selectedObjEvaluationMatrix1.Id;

                })

                if ($scope.selectedObjEvaluationMatrix2 != undefined) {

                    $scope.selectedObjEvaluationMatrix2 = undefined;
                }

                $scope.evaluationMatrixList2 = list;

                $scope.load();
                $scope.pager();
                $scope.carregaSkill();
            }
            else {
                $scope.selectedObjEvaluationMatrix2 = undefined;
            }
        };
 
        $scope.carregaSkill2 = function __carregaSkill2(result) {

            if ($scope.selectedObjEvaluationMatrix2 != undefined) {

                SkillModel.getByMatriz({ Id: $scope.selectedObjEvaluationMatrix2.Id }, function (result) {

                    if (result.success) {

                        var _skills2 = result.lista;
                        var formatedList2 = [];
                        for (var key in _skills2) {
                            formatedList2.push({
                                Id: _skills2[key].ModelSkillLevels.Id,
                                Description: _skills2[key].ModelSkillLevels.Description,
                                objSkill: undefined,
                                indexSkill: key,
                                lista: _skills2[key].ModelSkillLevels.Skills
                            });
                        }
                        $scope.skills2 = formatedList2;
                        $scope.skillsCopy2 = angular.copy(formatedList2);

                        return;
                    }
                    else {
                        $scope.skills2 = [];
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            else {

                $scope.skills2 = [];
            }
        };

        $scope.carregarTemp2 = function __carregarTemp2(newValue, oldValue) {

            if ($scope.selectedObjEvaluationMatrix2 != undefined) {

                for (var i = 0; i < oldValue.length; i++) {

                    if (newValue[i] != undefined) {

                        if (!angular.equals(oldValue[i].objSkill, newValue[i].objSkill)) {

                            for (var j = ($scope.skills2.indexOf(newValue[i]) + 1) ; j < newValue.length; j++) {
                                $scope.skills2[j].objSkill = null;
                                $scope.skills2[j].lista = [];
                            }

                            if ((i + 1) < newValue.length) {
                                if (newValue[i].objSkill != undefined)
                                    $scope.carregarCascadeSkill2(newValue[i].objSkill.Id, ($scope.skills2.indexOf(newValue[i]) + 1));
                            }
                            return;
                        }
                    }
                }
            }
            else {

                $scope.skills2 = [];
            }
        };
 
        $scope.carregarCascadeSkill2 = function __carregarCascadeSkill2(_parentID, childrenIndex) {

            if ($scope.selectedObjEvaluationMatrix2 != undefined && _parentID != undefined) {

                SkillModel.getByParent({ Id: _parentID }, function (result) {

                    if (result.success) {
                        var _skills2 = [];
                        for (var key in result.lista) {
                            $scope.skills2[childrenIndex].lista.push(result.lista[key].Skills);
                            $scope.skillsCopy2[childrenIndex].lista.push(angular.copy(result.lista[key].Skills));

                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            else {

                $scope.skills2 = [];
            }
        };

        $scope.delete = function __delete(correlatedSkill) {

            CorrelatedSkillModel.delete({ Id: correlatedSkill.Id }, function (result) {

                if (result.success) {
                    $notification.success(result.message);
                    $scope.correlatedSkillList.splice($scope.correlatedSkillList.indexOf(correlatedSkill), 1);
                    $scope.pager();
                    $scope.load();
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);

                }
            });
            angular.element('#modal').modal('hide');
        };

        $scope.salvar = function __salvar() {

            var objSkill1 = {};
            for (var skill in $scope.skills) {

                if ($scope.skills[skill].objSkill) {
                    if ($scope.skills[skill].objSkill.LastLevel) {
                        $scope.Skill1.id = $scope.skills[skill].objSkill.Id;
                    }
                } else {
                    $scope.Skill1.id = null
                }
            }

            var objSkill2 = {};
            for (var skill in $scope.skills2) {

                if ($scope.skills2[skill].objSkill) {
                    if ($scope.skills2[skill].objSkill.LastLevel) {
                        $scope.Skill2.id = $scope.skills2[skill].objSkill.Id;
                    }
                } else {
                    $scope.Skill2.id = null
                }
            }

            if ($scope.verifica($scope.Skill1, $scope.Skill2)) {

                var correlatedSkill = {
                    Skill1: $scope.Skill1,
                    Skill2: $scope.Skill2
                };

                CorrelatedSkillModel.save(correlatedSkill, function (result) {

                    if (result.success) {
                        $notification.success(result.message);
                        $scope.pager();
                        $scope.load();
                    } else {
                        $notification[result.type ? result.type : 'error'](result.message);

                    }
                });
            };
        };

        $scope.verifica = function __verifica(Skill1, Skill2) {

            if (!$scope.selectedObjEvaluationMatrix1 || !$scope.selectedObjEvaluationMatrix2) {

                $notification.alert('A matriz deve ser selecionada para correlação.');

                return false;
            }

            if (!$scope.Skill1.id || !$scope.Skill2.id) {

                $notification.alert('O último nível da matriz deve ser selecionado.');

                return false;
            }
            return true;
        };

        $scope.activeModal = function __activeModal(label, text) {
            if (!text) return;
            $scope.textSelected = {
                Description: label,
                TextDescription: text
            };
            angular.element("#modalTextMatriz").modal({ backdrop: 'static' });
        };

        Init();
    };

})(angular, jQuery);