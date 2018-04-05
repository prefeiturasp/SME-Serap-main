/**
 * @function Cadastro/Edição de Competências Cognitivas Controller
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("FormCognitiveCompetenceController", FormCognitiveCompetenceController);

    FormCognitiveCompetenceController.$inject = ['$scope', '$rootScope', '$window', '$notification', '$util', 'CognitiveCompetenceModel'];

    function FormCognitiveCompetenceController($scope, $rootScope, $window, $notification, $util, CognitiveCompetenceModel) {

        $scope.params = $util.getUrlParams();
        $scope.cognitiveCompetence = { Description: undefined };

        function Init() {

            $notification.clear();
            
            if ($scope.params.Id !== undefined) {

                CognitiveCompetenceModel.findSimple({ Id: $scope.params.Id }, function (result) {

                    if (result.success) {
                            $scope.cognitiveCompetence = result.cognitiveCompetence;
                        }
                        else {
                            $notification[result.type ? result.type : 'error'](result.message);
                        }
                    });
            }   
        };

        $scope.salvar = function __salvar() {

            if ($scope.verifica()) {

                CognitiveCompetenceModel.save($scope.cognitiveCompetence, function (result) {

                    if (result.success) {
                        $notification.success(result.message);
                        $window.location.href = '/CognitiveCompetence/List';
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };
        
        $scope.voltar = function __voltar() {
            $window.location.href = '/CognitiveCompetence/List';
        };

        $scope.verifica = function __verifica() {

            if (!$scope.cognitiveCompetence.Description) {

                $notification.alert('O campo "Descrição" é obrigatório.');
                angular.element('#description').focus();
                return false;
            }
            return true;
        };

        Init();
    };

})(angular, jQuery);