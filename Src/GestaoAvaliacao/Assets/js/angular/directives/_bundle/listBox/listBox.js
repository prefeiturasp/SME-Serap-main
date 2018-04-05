(function () {
    'use strict';

    angular
            .module('directives')
            .directive('multiSelect', multiSelect);

    multiSelect.$inject = ['$q'];

    function multiSelect($q) {
        return {
            restrict: 'E',
            require: 'ngModel',
            scope: {
                infoMessage: "@",
                selectedLabel: "@",
                availableLabel: "@",
                displayAttr: "@",
                orderBy: "@",
                available: "=",
                model: "=ngModel"
            },
            templateUrl: '/Assets/js/angular/directives/_bundle/listBox/listBox.html',
            link: function (scope, elm, attrs) {
                scope.selected = {
                    available: [],
                    current: []
                };

                /* Handles cases where scope data hasn't been initialized yet */
                var dataLoading = function (scopeAttr) {
                    var loading = $q.defer();
                    if (scope[scopeAttr]) {
                        loading.resolve(scope[scopeAttr]);
                    } else {
                        scope.$watch(scopeAttr, function (newValue, oldValue) {
                            if (newValue !== undefined)
                                loading.resolve(newValue);
                        });
                    }
                    return loading.promise;
                };

                /* Filters out items in original that are also in toFilter. Compares by reference. */
                var filterOut = function (original, toFilter) {
                    var filtered = [];
                    angular.forEach(original, function (entity) {
                        var match = false;
                        for (var i = 0; i < toFilter.length; i++) {
                            if (toFilter[i][attrs.displayAttr] == entity[attrs.displayAttr]) {
                                match = true;
                                break;
                            }
                        }
                        if (!match) {
                            filtered.push(entity);
                        }
                    });
                    return filtered;
                };

                scope.refreshAvailable = function () {
                    scope.available = filterOut(scope.available, scope.model);
                    scope.selected.available = [];
                    scope.selected.current = [];
                };

                scope.add = function () {
                    scope.model = scope.model.concat(scope.selected.available);
                    scope.refreshAvailable();
                };

                scope.remove = function () {
                    scope.available = scope.available.concat(scope.selected.current);
                    scope.model = filterOut(scope.model, scope.selected.current);
                    scope.refreshAvailable();
                };

                scope.update = function () {
                    scope.model = scope.model.concat(scope.selected.current);
                    //scope.model = filterOut(scope.model, scope.selected.current);
                    scope.refreshAvailable();
                };

                $q.all([dataLoading("model"), dataLoading("available")]).then(function (results) {
                    scope.refreshAvailable();
                });
            }
        };
    }
})();