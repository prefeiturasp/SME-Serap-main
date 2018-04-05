/*
 * @description Diretiva que contém os elementos do fitro das páginas de relatórios de provas
 * @autor: Luís Maron
 * @autor: Julio Cesar Silva - update: 13/04/2016
 * Exemplo: 
 *      <report-filters filters="filters" global="global"></report-filters>
 */
(function (angular, $) {

    'use strict';

    angular.module('directives')
        .directive('reportFilters', reportFilters);

    reportFilters.$inject = ['$notification', 'ReportTestModel', 'AdherenceModel'];

    function reportFilters($notification, ReportTestModel, AdherenceModel) {

        function __link($scope, element, attrs) {

            $scope.listFilter = {
                DREs: [],
                Schools: [],
                Years: [],
                Tests: [],
                Classes: []
            };

            /**
             * @description Selciona todos os anos disponíveis
             * @param 
             * @returns
             */
            $scope.getYears = function __getYears() {

                ReportTestModel.getAllYears(function (result) {

                    if (result.success) {
                        $scope.listFilter.Years = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }

                }, function (result) {
                    $notification[result.type ? result.type : 'error'](result.message);
                });
            };

            /**
             * @description Localizar provas aplicadas para determinado ano
             * @param
             * @returns
             */
            $scope.getTestByYear = function __getTestByYear() {

                if (($scope.filters.Year === undefined || $scope.filters.Year === null)) {
                    $notification.alert('O filtro "Ano*" é obrigatório para realizar a pesquisa.');
                    return;
                }
                if (($scope.filters.dateStart === undefined || $scope.filters.dateStart === null)) {
                    $notification.alert('O filtro "Data de início*" é obrigatório para realizar a pesquisa.');
                    $scope.filters.Year = undefined;
                    return;
                }
                if (($scope.filters.dateEnd === undefined || $scope.filters.dateEnd === null)) {
                    $notification.alert('O filtro "Data de início*" é obrigatório para realizar a pesquisa.');
                    $scope.filters.Year = undefined;
                    return;
                }

                var param = {
                    year: $scope.filters.Year.Id,
                    ApplicationStartDate: $scope.filters.dateStart,
                    ApplicationEndDate: $scope.filters.dateEnd
                };

                ReportTestModel.getTestByYear(param, function (result) {

                    if (result.success) {
                        $scope.listFilter.Tests = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }

                }, function (result) {
                    $notification[result.type ? result.type : 'error'](result.message);
                });
            };

            /**
             * @description Seleciona DREs aderidas em determinada prova
             * @param
             * @returns
             */
            $scope.getDResByTest = function __getDResByTest() {

                if ($scope.filters.Test === undefined || $scope.filters.Test === null) {
                    $notification.alert('O filtro "Prova*" é obrigatório para realizar a pesquisa.');
                    return;
                }

                var param = {
                    TestId: $scope.filters.Test.Id,
                    year: $scope.filters.Year.Id,
                };

                ReportTestModel.getDResByTest(param, function (result) {
                    if (result.success) {
                        $scope.listFilter.DREs = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            };

            /**
             * @description Seleciona escolas aderidas em determinada prova
             * @param
             * @returns
             */
            $scope.getSchoolsByTest = function __getSchoolsByTest() {

                if ($scope.filters.DRE === undefined || $scope.filters.DRE === null) {
                    $notification.alert('O filtro "DRE*" é obrigatório para realizar a pesquisa.');
                    return;
                }

                var param = {
                    TestId: $scope.filters.Test.Id,
                    year: $scope.filters.Year.Id,
                    uad_id: $scope.filters.DRE.Id
                };

                ReportTestModel.getSchoolsByTest(param, function (result) {
                    if (result.success) {
                        $scope.listFilter.Schools = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            };

            /**
             * @description Seleciona escolas aderidas em determinada prova
             * @param
             * @returns
             */
            $scope.getSectionsByTest = function __getSectionsByTest() {

                if ($scope.filters.School === undefined || $scope.filters.School === null) {
                    $notification.alert('O filtro "Escolas*" é obrigatório para realizar a pesquisa.');
                    return;
                }

                var param = {
                    TestId: $scope.filters.Test.Id,
                    year: $scope.filters.Year.Id,
                    esc_id: $scope.filters.School.Id
                };

                ReportTestModel.getSectionsByTest(param, function (result) {
                    if (result.success) {
                        $scope.listFilter.Classes = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            };

            /**
             * @description Seleciona DREs cadastradas
             * @param
             * @returns
             */
            $scope.getDREs = function __getDREs() {
                AdherenceModel.getDRESimple(function (result) {
                    if (result.success) {
                        $scope.listFilter.DREs = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            };

            /**
             * @description Seleciona escolas cadastradas
             * @param
             * @returns
             */
            $scope.getSchools = function __getSchools() {

                if ($scope.filters.DRE === undefined || $scope.filters.DRE === null) {
                    $notification.alert('O filtro "DRE*" é obrigatório para realizar a pesquisa.');
                    return;
                }

                var params = {
                    dre_id: $scope.filters.DRE.Id
                };

                AdherenceModel.getSchoolsSimple(params, function (result) {

                    if (result.success) {
                        $scope.listFilter.Schools = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                }, function (result) {
                    $notification[result.type ? result.type : 'error'](result.message);
                });
            };

            /**
             * @description Seleciona escolas cadastradas
             * @param {Object} 
             * @returns
             */
            $scope.getTests = function __getTests() {

                if (($scope.filters.School === undefined || $scope.filters.School === null)) {
                    $notification.alert('O filtro "Escolas*" é obrigatório para realizar a pesquisa.');
                    $scope.filters.Year = undefined;
                    return;
                }
                if (($scope.filters.dateStart === undefined || $scope.filters.dateStart === null)) {
                    $notification.alert('O filtro "Data de início*" é obrigatório para realizar a pesquisa.');
                    $scope.filters.Year = undefined;
                    return;
                }
                if (($scope.filters.dateEnd === undefined || $scope.filters.dateEnd === null)) {
                    $notification.alert('O filtro "Data de início*" é obrigatório para realizar a pesquisa.');
                    $scope.filters.Year = undefined;
                    return;
                }

                var params = {
                    esc_id: $scope.filters.School.Id,
                    ApplicationStartDate: $scope.filters.dateStart,
                    ApplicationEndDate: $scope.filters.dateEnd
                };

                ReportTestModel.getTestBySchool(params, function (result) {
                    if (result.success) {
                        $scope.listFilter.Tests = result.lista;
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            };

            /**
             * @description Limpa os filtros de acordo com o filtro alterado
             * @param {Object} filter
             * @returns
             */
            $scope.clearByGlobalFilter = function __clearByGlobalFilter(filter) {

                if (filter === 'Year' || filter === 'DateStart' || filter === 'DateEnd') {
                    $scope.listFilter.Schools = [];
                    $scope.listFilter.DREs = [];
                    $scope.listFilter.Tests = [];
                    $scope.listFilter.Classes = [];
                    $scope.filters.Test = undefined;
                    $scope.filters.DRE = undefined;
                    $scope.filters.School = undefined;
                    $scope.filters.Classes = undefined;
                    return;
                }

                if (filter === 'Test') {
                    $scope.listFilter.Schools = [];
                    $scope.listFilter.DREs = [];
                    $scope.listFilter.Classes = [];
                    $scope.filters.DRE = undefined;
                    $scope.filters.School = undefined;
                    $scope.filters.Classes = undefined;
                    return;
                }

                if (filter === 'DRE') {
                    $scope.listFilter.Schools = [];
                    $scope.listFilter.Classes = [];
                    $scope.filters.School = undefined;
                    $scope.filters.Classes = undefined;
                    return;
                }

                if (filter === 'School') {
                    $scope.listFilter.Classes = [];
                    $scope.filters.Class = undefined;
                    return;
                }
            };

            /**
             * @description Limpa os filtros de acordo com o filtro alterado
             * @param {Object} filter
             * @returns
             */
            $scope.clearByLocalFilter = function __clearByGlobalFilter(filter) {

                if (filter === 'Year' || filter === 'DateStart' || filter === 'DateEnd') {
                    $scope.filters.Test = null;
                    $scope.listFilter.Tests = [];
                    $scope.filters.Class = null;
                    $scope.listFilter.Classes = [];
                    return;
                }

                if (filter === 'Test') {
                    $scope.listFilter.Schools = [];
                    $scope.listFilter.DREs = [];
                    $scope.listFilter.Classes = [];
                    $scope.filters.DRE = undefined;
                    $scope.filters.School = undefined;
                    $scope.filters.Classes = undefined;
                    return;
                }

                if (filter === 'DRE') {
                    $scope.filters.School = null;
                    $scope.listFilter.Schools = [];
                    $scope.filters.Year = null;
                    $scope.listFilter.Years = [];
                    $scope.filters.Test = null;
                    $scope.listFilter.Tests = [];
                    $scope.filters.Class = null;
                    $scope.listFilter.Classes = [];
                    return;
                }

                if (filter === 'School') {
                    $scope.filters.Year = null;
                    $scope.listFilter.Years = [];
                    $scope.filters.Test = null;
                    $scope.listFilter.Tests = [];
                    $scope.filters.Class = null;
                    $scope.listFilter.Classes = [];
                    return;
                }
            };

            /**
             * @description Limpa os filtros de acordo com o filtro alterado
             * @param {Object} filter
             * @returns
             */
            $scope.refreshFilters = function __clearByGlobalFilter(filter) {

                $scope.listFilter = {
                    DREs: [],
                    Schools: [],
                    Years: [],
                    Tests: [],
                    Classes: []
                };
                $scope.filters.Test = undefined;
                $scope.filters.DRE = undefined;
                $scope.filters.School = undefined;
                $scope.filters.Classes = undefined;
                $scope.filters.dateStart = undefined;
                $scope.filters.dateEnd = undefined;
                $scope.filters.Year = undefined;
            }

            /**
		     * @description Forçar datepicker por trigger em botão
		     * @param
		     * @returns
		     */
            $scope.datepicker = function __datepicker(id) {
                angular.element("#" + id).datepicker('show');
            };

            /**
		     * @description Forçar a atualização (diggest) do angular
		     * @param
		     * @returns
		     */
            $scope.safeApply = function __safeApply() {
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

            $scope.getYears();
        };

        return {
            restrict: 'E',
            templateUrl: base_url('Assets/js/angular/directives/_bundle/reportFilters/filtersTpl.html'),
            scope: {
                filters: '=',
                global: '='
            },
            link: __link
        };
    };

})(angular, jQuery);