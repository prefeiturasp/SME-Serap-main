/**
 * @function Cadastro/Edição Item Level
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
        .controller("FormItemLevelController", FormItemLevelController);

    FormItemLevelController.$inject = ['$scope', '$rootScope', '$window', '$notification', '$util', 'ItemLevelModel'];

    function FormItemLevelController($scope, $rootScope, $window, $notification, $util, ItemLevelModel) {

        $scope.params = $util.getUrlParams();

        function Init() {

            $notification.clear();

            connfigInternalObjects();

            loadEditItemModel();
        };

        function connfigInternalObjects() {

            angular.element('.number').on('input', function (event) {
                this.value = this.value.replace(/[^0-9]/g, '');           
            });
            $scope.itemLevel = null;
            $scope.description =undefined;
            $scope.valor = undefined;
        };

        function loadEditItemModel() {

            if ($scope.params.Id) {

                ItemLevelModel.find({ Id: $scope.params.Id }, function (result) {

                        $scope.itemLevel = result.itemLevel;
                        $scope.valor = 0 + $scope.itemLevel.Value;
                        $scope.description = '' + $scope.itemLevel.Description;
                });
            }
        };

        $scope.salvar = function __salvar() {

            if ($scope.verifica()) {

                $scope.itemLevel.Value = $scope.valor;
                $scope.itemLevel.Description = $scope.description;

                ItemLevelModel.save($scope.itemLevel, function (result) {
                    if (result.success) {
                        $notification.success(result.message);
                        $window.location.href = '/ItemLevel/List';
                    }
                    else {

                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.verifica = function __verifica() {

            if (!$scope.description) {

                $notification.alert('O campo "Descrição" é obrigatório.');
                angular.element('#description').focus();
                return false;
            }

            if (!$scope.valor) {

                $notification.alert('O campo "Valor" é obrigatório.');
                angular.element('#value').focus();
                return false;
            }

            if ($scope.valor <= 0) {

                $notification.alert('O campo "Valor" deve ser superior a zero.');
                angular.element('#value').focus();
                return false;
            }

            if (!$scope.itemLevel)
                $scope.itemLevel = {};

            return true;
        };

        Init();
    };

})(angular, jQuery);