/**
 * function Cadastro/Edição Parâmetros
 * @namespace Controller
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'collapse']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ParameterController", ParameterController);
    
    ParameterController.$inject = ['$scope', '$notification', '$timeout', '$util', 'ParameterModel'];

    function ParameterController($scope, $notification, $timeout, $util, ParameterModel) {
        
        $scope.params = $util.getUrlParams();
        $scope.EnumState = EnumState; // -> razor
        $scope.EnumParameterType = {
            INPUT: 1,
            CHECKBOX: 2,
            DROPDOWN: 3
        };

        function Init() {
            
            $notification.clear();
            configObjects();
            getParameters($scope.params.Id);
        };

        function configObjects() {

            $scope.categories = [];

            $scope.requiredmenu = {
                title: "empty",
                content: "empty",
                index: undefined,
                selectAllRequired: selectAllRequired,
            };

            $scope.versionablemenu = {
                title: "empty",
                content: "empty",
                index: undefined,
                selectAllVersionable: selectAllVersionable,
            };
        };

        function getParameters(_Id) {

            if (_Id == undefined)
                _Id = '1';

            if (typeof _Id === 'string') {

                $scope.pageID = _Id;

                ParameterModel.getParameters({ Id: _Id }, function (result) {

                    if (!result.hasOwnProperty('lista')) return;

                    for (var a = 0; a < result.lista.length; a++) {

                        $scope.categories.push({
                            Id: result.lista[a].Id,
                            Description: result.lista[a].Description,
                            PageRequired: result.lista[a].pageObligatory,
                            PageVersioning: result.lista[a].pageVersioning,
                            Parameters: angular.copy(result.lista[a].Parameters),
                            HistoryParameters: angular.copy(result.lista[a].Parameters)
                        });
                    }

                    for (var b = 0; b < $scope.categories.length; b++) {

                        var param = $scope.categories[b].Parameters;

                        for (var index = 0; index < param.length; index++) {

                            if (param[index].ParameterType === $scope.EnumParameterType.INPUT) {
                                // :TODO
                            }
                            else if (param[index].ParameterType === $scope.EnumParameterType.CHECKBOX) {
                                if (param[index].Value === 'true' || param[index].Value === 'True')
                                    param[index].Value = true;
                                else
                                    param[index].Value = false;
                            }
                            else if (param[index].ParameterType === $scope.EnumParameterType.DROPDOWN) {

                                if (param[index].Key === "UF")
                                    $scope.getStates(b, index);
                                else if (param[index].Key === "CITY_STANDARD")
                                    $scope.getCitys(null, true);
                            }
                        }
                    }

                }, function (result) {
                    $notification[result.type ? result.type : 'error'](result.message);
                });
            }
        };

        function selectAllRequired(index, state) {

            for (var i = 0, len = $scope.categories[index].Parameters.length; i < len; i++) {

                if ($scope.categories[index].Parameters[i].Obligatory != null )
                    $scope.categories[index].Parameters[i].Obligatory = state;
            }
        };

        function selectAllVersionable(index, state) {

            for (var i = 0, len = $scope.categories[index].Parameters.length; i < len; i++) {

                if ($scope.categories[index].Parameters[i].Versioning != null)
                    $scope.categories[index].Parameters[i].Versioning = state;
            }
        };
   
        function wrapper(index) {

            var pack = [];

            for (var i = 0, len = $scope.categories[index].Parameters.length; i < len ; i++) {
                
                if ( $scope.categories[index].Parameters[i].Value != $scope.categories[index].HistoryParameters[i].Value ||
                     $scope.categories[index].Parameters[i].Obligatory != $scope.categories[index].HistoryParameters[i].Obligatory ||
                     $scope.categories[index].Parameters[i].Versioning != $scope.categories[index].HistoryParameters[i].Versioning) {

                    pack.push($scope.categories[index].Parameters[i]);
                }
            }

            return pack;
        };
 
        function updateHistory(index, pack) {

            for (var i = 0, len = $scope.categories[index].Parameters.length; i < len ; i++) {

                for (var j = 0, leng = pack.length; j < leng ; j++) {
                    
                    $scope.finishEditField(index, i);

                    if ($scope.categories[index].Parameters[i].Id == pack[j].Id) {

                        $scope.categories[index].HistoryParameters[i].Value = pack[j].Value;
                        $scope.categories[index].HistoryParameters[i].Obligatory = pack[j].Obligatory;
                        $scope.categories[index].HistoryParameters[i].Versioning = pack[j].Versioning;
                    }
                }    
            }
        };
 
        $scope.setEditField = function __setEditField(catIndex, paramIndex) {

            $scope.categories[catIndex].Parameters[paramIndex].editState = true;
        };
 
        $scope.finishEditField = function __finishEditField(catIndex, paramIndex) {

            $scope.categories[catIndex].Parameters[paramIndex].editState = false;
        };
 
        $scope.initRequiredMenu = function __initRequiredMenu(index) {
           
            $scope.requiredmenu.index = index;
        };
 
        $scope.initVersionableMenu = function __initVersionableMenu(index) {

            $scope.versionablemenu.index = index;
        };
   
        $scope.save = function __save(index) {

            var wrap = wrapper(index);

            if (wrap.length > 0) {
                ParameterModel.save(wrap, function (result) {

                    if (result.success) {

                        updateHistory(index, wrap);
                        $notification.success(result.message);
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        Init();
    };

})(angular, jQuery);