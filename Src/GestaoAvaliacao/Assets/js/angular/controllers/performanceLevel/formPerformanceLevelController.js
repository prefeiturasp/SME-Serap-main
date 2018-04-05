/**
 * function Cadastro/Edição Performance Level
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
        .controller("FormPerformanceLevelController", FormPerformanceLevelController);

    FormPerformanceLevelController.$inject = ['$scope', '$rootScope', '$window', '$notification', '$util', 'PerformanceLevelModel']

    function FormPerformanceLevelController($scope, $rootScope, $window, $notification, $util, PerformanceLevelModel) {

        $scope.params = $util.getUrlParams();

        function Init() {

            $notification.clear();
            configInternalObjects();
            loadPerfomanceModel();
        };
        
        function configInternalObjects() {

            angular.element('.number').on('input', function (event) {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

           $scope.performanceLevel = null;
           $scope.Code = undefined;
           $scope.Description = undefined;
        };

        function loadPerfomanceModel() {

            if ($scope.params.Id) {

                PerformanceLevelModel.find({ Id: $scope.params.Id }, function (result) {

                   $scope.performanceLevel = result.performanceLevel;
                   $scope.Code = '' + $scope.performanceLevel.Code;
                   $scope.Description = '' +$scope.performanceLevel.Description;
                });
            }
        };
 
        $scope.salvar = function () {

            if ($scope.verifica()) {

               $scope.performanceLevel.Code =$scope.Code;
               $scope.performanceLevel.Description =$scope.Description;

                PerformanceLevelModel.save($scope.performanceLevel, function (result) {

                    if (result.success) {
                        $notification.success(result.message);
                        $window.location.href = '/PerformanceLevel/List';
                    }
                    else {

                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.verifica = function () {
            
            if (!$scope.Description) {

                $notification.alert('O campo "Descrição" é obrigatório.');

                angular.element('#description').focus();

                return false;
            }

            if (!$scope.Code) {

                $notification.alert('O campo "Código" é obrigatório.');

                angular.element('#value').focus();

                return false;
            }

            if (!$scope.performanceLevel)
               $scope.performanceLevel = {};

            return true;
        };

        Init();
    };

})(angular, jQuery);