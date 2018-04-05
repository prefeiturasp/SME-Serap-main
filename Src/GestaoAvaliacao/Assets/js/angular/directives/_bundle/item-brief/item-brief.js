/*
 * @description Visualizar resumo de item
 * @param {object} angular - instância do angularJS
 * @param {object} $ - instância do Jquery
 * @author Julio Cesar Silva - 08/04/2016 (c) Mstech Educação e Tecnologia
 * @example:
 *     <item-brief can-print="true"></item-brief>
 */
(function (angular, $) {

    'use strict';

    angular
        .module("directives")
        .controller("$ItemBriefController", $ItemBriefController)
        .directive("itemBrief", $itemBrief);

    $itemBrief.$inject = ['$log'];
    $ItemBriefController.$inject = ['$scope', '$http', '$q', '$log'];

    function $itemBrief($log) {

        return {
            restrict: 'E',
            scope: { },
            templateUrl: "/Assets/js/angular/directives/_bundle/item-brief/item-brief.html",
            controller: '$ItemBriefController',
            link: function ($scope, element, attrs) {

                attrs.$observe('itemId', function (itemId) {

                    if (itemId === undefined || itemId === null || itemId.length === 0) {
                        $log.error("Id do item inválido");
                        return;
                    }

                    $scope.setItemId(itemId);
                });

                attrs.$observe('canPrint', function (canPrint) {
                       
                    canPrint = Boolean(canPrint === "true");

                    $scope.setCanPrint(canPrint);
                });

                attrs.$observe('description', function (description) {

                    $scope.setDescription(description);
                });
            }
        };
    };

    function $ItemBriefController($scope, $http, $q, $log) {

        // modelos
        $scope.item = {
            ItemCode: undefined,
            Revoked: undefined,
            Situation: undefined,
            Discipline: undefined,
            Matriz: undefined,
            Skills: undefined,
            CurriculumGrade: undefined,
            Proficiency: undefined,
            Tips: undefined,
            ItemType: undefined,
            Keywords: undefined,
            ItemLevel: undefined,
            Versions: undefined,
            TRI: undefined,
            Sentence: undefined,
            TextBase: undefined,
            Statement: {
                Descritpion: undefined
            },
            Alternatives: undefined
        };

        /**
         * @description Abrir modal de item
         * @param 
         * @return
         */
        $scope.showItem = function __showItem() {
          
            $scope.getItem(function (_success) {

                if (_success) {
                    angular.element('#modalItemBrief_' + $scope.itemId).modal({ backdrop: 'static' });
                }
                else {
                    $log("Erro ao abrir modal --> directiva item-brief.js, método 'showItem'");
                }
            });
        };

        /**
         * @description Obter url base do site e concatenar com path especificado
         * @param {string} path - url parcial
         * @return
         */
        $scope.siteURL = function __siteURL(path) {
            var location = window.location;
            var origin = location.origin ? location.origin + "/" + path : location.protocol + "//" + location.host + "/" + path;
            return origin;
        };

        /**
         * @description Obter o item da o Id.
         * @param {Object} _callback - função
         * @callback - comunica finalização da requisição (GET)
         * @returns 
         */
        $scope.getItem = function __getItem(_callback) {

            $q.all([
                $http.get($scope.siteURL('/Item/GetMatrixByItem?itemId=' + $scope.itemId)),
                $http.get($scope.siteURL('/Item/GetBaseTextItems?itemId=' + $scope.itemId)),
                $http.get($scope.siteURL('/Item/GetItemById?itemId=' + $scope.itemId))
            ]).then(function (results) {
               
                // 1ª
                if (results[0].data.success) {

                    $scope.item.Discipline = {
                        Description: results[0].data.lista.Description,
                        total: 0
                    }
                    $scope.item.Matriz = {
                        Description: results[0].data.lista.EvaluationMatrix.Description
                    }
                }
                else {
                    $log.error("Erro ao obter no método --> 'getMatrixByItem' item-brief.js", results[0].data.message);
                }

                // 2ª
                if (results[1].data.success) {

                    if (results[1].data.lista != null) {

                        $scope.item.TextBase = results[1].data.lista.Description != null ? results[1].data.lista.Description : undefined;
                    }
                    else {
                        $log.error('ItemModel.getBaseTextItems -> retorno Lista vazia');
                    }
                }
                else {
                    $log.error("Erro ao obter no método --> 'getBaseTextItems' item-brief.js", results[1].data.message);
                }
                
                // 3ª
                if (results[2].data.success) {

                    if (results[2].data.lista != null) {

                        $scope.item.Revoked = results[2].data.lista.Revoked;
                        $scope.item.ItemCode = results[2].data.lista.ItemCode.toString();
                        $scope.item.Situation = results[2].data.lista.ItemSituation != null ? results[2].data.lista.ItemSituation : undefined;
                        $scope.item.Skills = results[2].data.lista.ItemSkills;
                        $scope.item.CurriculumGrade = results[2].data.lista.ItemCurriculumGrades[0].Description != null ? results[2].data.lista.ItemCurriculumGrades[0].Description : undefined;
                        $scope.item.Proficiency = results[2].data.lista.proficiency != null ? results[2].data.lista.proficiency.toString() : undefined;
                        $scope.item.Tips = results[2].data.lista.Tips != null ? results[2].data.lista.Tips : undefined;
                        $scope.item.ItemType = results[2].data.lista.ItemType.Description;
                        $scope.item.Keywords = results[2].data.lista.Keywords != null ? $scope.breakKeywords(results[2].data.lista.Keywords) : undefined;
                        $scope.item.ItemLevel = results[2].data.lista.ItemLevel;
                        $scope.item.Versions = results[2].data.lista.Versions;
                        $scope.item.TRI = [{ preDescription: "Proporção de acertos", Value: results[2].data.lista.TRIDifficulty != null ? results[2].data.lista.TRIDifficulty.toString() : undefined },
                                            { preDescription: "Discriminação", Value: results[2].data.lista.TRIDiscrimination != null ? results[2].data.lista.TRIDiscrimination.toString() : undefined },
                                            { preDescription: "Acerto casual", Value: results[2].data.lista.TRICasualSetting != null ? results[2].data.lista.TRICasualSetting.toString() : undefined }];

                        $scope.item.Sentence = results[2].data.lista.descriptorSentence != null ? results[2].data.lista.descriptorSentence : undefined;
                        $scope.item.Statement.Description = results[2].data.lista.Statement.Description != null ? results[2].data.lista.Statement.Description : undefined;
                        $scope.item.Alternatives = results[2].data.lista.Alternatives != null ? results[2].data.lista.Alternatives : undefined;
                        $scope.item.id = $scope.itemId;
                    }
                    else {
                        $log.error("Erro ao obter no método --> 'getItemById' item-brief.js");
                    }
                }

                if (_callback) _callback(true);
            },
            function (result, status, headers, config) {
                $log("Erro ao obter item directive --> 'item-brief' ");
                if (_callback) _callback(false);
            });


            /*
            $http.get($scope.siteURL('Item/GetItemById/')).then(function (result, status, headers, config) {
                $scope.item = result.data;
                if (_callback) _callback();
            }, function (result, status, headers, config) {
                $log("Erro ao obter item directive --> 'item-brief' ");
                if (_callback) _callback();
            });
            */
        };

        /**
         * @description Imprimir um item
         * @param
         * @returns 
         */
        $scope.previewPrint = function __previewPrint() {
            window.open("/Item/PreviewPrintItem?id=" + $scope.itemId);
        };

        /**
         * @description Formatar as palavras-chaves para tags
         * @param
         * @return
         */
        $scope.breakKeywords = function (_tags) {

            var arrTags = [];

            var arrSplited = _tags.split(";")

            for (var key in arrSplited) {
                arrSplited[key] = arrSplited[key].replace(";", "");
                arrTags.push(arrSplited[key]);
            }

            return arrTags;
        };

        /**
         * @description Forçar o ciclo de diggest do angularJS para atualização de tela
         * @param
         * @return
         */
        $scope.safeApply = function safeApply() {
            var $scope, fn, force = false;
            if (arguments.length === 1) {
                var arg = arguments[0];
                if (typeof arg === 'function') {
                    fn = arg;
                } else {
                    $scope = arg;
                }
            } else {
                $scope = arguments[0];
                fn = arguments[1];
                if (arguments.length === 3) {
                    force = !!arguments[2];
                }
            }
            $scope = $scope || this;
            fn = fn || function () { };

            if (force || !$scope.$$phase) {
                $scope.$apply ? $scope.$apply(fn) : $scope.apply(fn);
            } else {
                fn();
            }
        };

        /**
         * @description set Id do item
         * @param
         * @return
         */
        $scope.setItemId = function __setItemId(_itemId) {
            $scope.itemId = _itemId;
        };

        /**
         * @description set permissão pra imprimir
         * @param
         * @return
         */
        $scope.setCanPrint = function __setCanPrint(_canPrint) {
            $scope.canPrint = _canPrint;
        };

        /**
         * @description set uma descrição
         * @param
         * @return
         */
        $scope.setDescription = function __setDescription(_description) {
            $scope.description = _description;
        };
    };

})(angular, jQuery);