/**
 * function Cadastro/Edição Absence Reason Controller
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
        .controller("FormAbsenceReasonController", FormAbsenceReasonController);
    
    FormAbsenceReasonController.$inject = ['$scope', '$rootScope', '$window', '$notification', '$util', 'AbsenceReasonModel'];

    function FormAbsenceReasonController($scope, $rootScope, $window, $notification, $util, AbsenceReasonModel) {

        $scope.params = $util.getUrlParams();
        $scope.absenceReason = { Description: undefined };

        function Init() {

            $notification.clear();
        
            if ($scope.params.Id !== undefined) {

                AbsenceReasonModel.findSimple({ Id: $scope.params.Id }, function (result) {

                    if (result.success) {
                        $scope.absenceReason = result.absenceReason;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }  
        };


        $scope.setNewDefault = function setNewDefault() {
            if (!$scope.absenceReason.IsDefault) {
                angular.element('#modal').modal('show');
            }
            $scope.absenceReason.IsDefault = true;
        }
        
        $scope.salvar = function __salvar() {

            if ($scope.verifica()) {

                AbsenceReasonModel.save($scope.absenceReason, function (result) {

                    if (result.success) {
                        $notification.success(result.message);
                        $window.location.href = '/AbsenceReason/List';
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }                  
                }); 
            }
        };
        
        $scope.voltar = function __voltar() {
            $window.location.href = '/AbsenceReason/List';
        };
            
        $scope.verifica = function __verifica() {

            if (!$scope.absenceReason.Description) {
                $notification.alert('O campo "Descrição" é obrigatório.');    
                angular.element('#description').focus();
                return false;
            }
            return true;
        };

        Init();
    };

})(angular, jQuery);