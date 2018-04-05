(function (angular, $) {

    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
		.module('appMain')
		.controller("ResponseChangeLogController", ResponseChangeLogController);

    ResponseChangeLogController.$inject = ['$rootScope', '$scope', 'ResponseChangeLogModel', 'AdherenceModel', 'TestModel', '$notification', '$timeout', '$pager', '$window', '$util'];


    function ResponseChangeLogController($rootScope, $scope, ResponseChangeLogModel, AdherenceModel, TestModel, $notification, $timeout, $pager, $window, $util) {

        /**
        * @function Responsavél por instanciar tds as variaveis
        * @param 
        * @returns
        */
        function configVariables() {
            $scope.countFilter = 0;
            // :paginação
            $scope.paginate = $pager(ResponseChangeLogModel.getResponseChangeLog);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = null;
            $scope.listResultSME = null;
            $scope.TestDescription = null;
            $scope.DreDescription = null;
            $scope.SchoolDescription = null;
            $scope.TeamDescription = null;

            $scope.listDREs = [];
            $scope.listTests = [];
            $scope.listSchools = [];
            $scope.listTeams = [];
            $scope.responseChangeLogList = null;
            
            getDREs();
            $scope.$watch("filters", function () {
                $scope.countFilter = 0;
                if ($scope.filters.DateStart) $scope.countFilter += 1;
                if ($scope.filters.DateEnd) $scope.countFilter += 1;
                if ($scope.filters.test_id) $scope.countFilter += 1;
                if ($scope.filters.uad_id) $scope.countFilter += 1;
                if ($scope.filters.esc_id) $scope.countFilter += 1;
                if ($scope.filters.tur_id) $scope.countFilter += 1;
                if ($scope.filters.DateStartChange) $scope.countFilter += 1;
                if ($scope.filters.DateEndChange) $scope.countFilter += 1;
            }, true);
        };

        $("#testCode").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btFiltrar").click();
            }
        });

        /**
         * @function Obter todas as DREs
         * @param {object} _callback
         * @returns
         */
        function getDREs(_callback) {
            AdherenceModel.getDRESimple(function (result) {
                if (result.success) {
                    $scope.listDREs = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
        * @function Obter as provas dado um período
        * @param {object} _callback
        * @returns
        */
        function getTests(_callback) {
            if ($scope.filters.DateStart && $scope.filters.DateEnd) {
                if (!validateDate()) return;
                TestModel.getTestByDate({ DateStart: $scope.filters.DateStart, DateEnd: $scope.filters.DateEnd }, function (result) {
                    if (result.success) {
                        $scope.listTests = result.lista;
                        if ($scope.listTests.length === 0)
                            $notification.alert("Nenhuma prova foi encontrada no período selecionado.")
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
            if (_callback) _callback();
        };
        $scope.getTests = getTests;

        /**
         * @function Validar período de datas
         * @param 
         * @returns
         */
        function validateDate() {
            if (!$scope.filters.DateStart || !$scope.filters.DateEnd) {
                $notification.alert("É necessário selecionar um período de aplicação da prova.");
                return false;
            }
            if ($util.greaterEndDateThanStartDate($scope.filters.DateStart, $scope.filters.DateEnd) === false) {
                $notification.alert("Data inicial de aplicação não pode ser maior que a data final.");
                $scope.filters.DateEnd = "";
                return false;
            }

            if ($util.greaterEndDateThanStartDate($scope.filters.DateStartChange, $scope.filters.DateEndChange) === false) {
                $notification.alert("Data inicial de alteração não pode ser maior que a data final.");
                $scope.filters.DateEndChange = "";
                return false;
            }

            return true;
        };

        function changeTest(_callback) {
            if ($scope.filters.test_id > 0 && $('#selectTest').val() != "") {
                $('#testCode').val($scope.filters.test_id)
                $scope.TestDescription = $('#selectTest option:selected').text();
            }

            if (_callback) _callback();
        };
        $scope.changeTest = changeTest;

        function changeTestCode(_callback) {
            if ($scope.filters.test_id > 0 && $('#testCode').val() != "") {
                $('#selectTest').val($scope.filters.test_id)
                $scope.TestDescription = $('#selectTest option:selected').text();
            }

            if (_callback) _callback();
        };
        $scope.changeTestCode = changeTestCode;

        /**
         * @function Limpar filtros
         * @param 
         * @returns
         */
        $scope.clearFilters = function __clearFilters() {
            $scope.filters = {};
            $scope.filters.uad_id = "";
            $scope.filters.test_id = "";
            $scope.filters.DateStart = "";
            $scope.filters.DateEnd = "";
            $scope.filters.esc_id = "";
            $scope.filters.tur_id = "";
            $scope.filters.DateStartChange = "";
            $scope.filters.DateEndChange = "";
        };

        /**
		 * @function Limpeza dos filtros de pesquisa
		 * @name clearByFilter
		 * @namespace AdherenceController
		 * @memberOf Controller
		 * @public
		 * @param {string} filter
		 * @return
		 */
        $scope.clearByFilter = function __clearByFilter(filter) {

            if (filter === 'Test') {
                $scope.listSchools = [];
                $scope.listTeams = [];
                $scope.filters.esc_id = undefined;
                $scope.filters.tur_id = undefined;
                return;
            }

            if (filter === 'DRE') {
                $scope.listSchools = [];
                $scope.listTeams = [];
                $scope.filters.esc_id = undefined;
                $scope.filters.tur_id = undefined;
                return;
            }

            if (filter === 'School') {
                $scope.listTeams = [];
                $scope.filters.tur_id = undefined;
                return;
            }
        };

        /**
         * @function Zera a paginação e pesquisa novamente
         * @param 
         * @returns
         */
        $scope.search = function __search() {
            if (!validateDate() || !validate()) return;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.searcheableFilter = angular.copy($scope.filters);
            Paginate();
        };

        /**
         * @function validar
         * @param
         * @returns
         */
        function validate() {
            if (!$scope.filters.test_id) {
                $notification.alert("É necessário selecionar uma prova ou preencher o seu código.");
                return false;
            }

            if (!Number.isInteger($scope.filters.test_id)) {
                $notification.alert("Código da prova inválido.");
                return false;
            }

            var position = $scope.listTests.indexOfField("TestId", $scope.filters.test_id);
            if (position < 0) {
                $notification.alert("Prova não encontrada ou fora do período informado.");
                return false;
            }
            return true;
        };

        Array.prototype.indexOfField = function (propertyName, value) {
            for (var i = 0; i < this.length; i++)
                if (this[i][propertyName] === value)
                    return i;
            return -1;
        }


        /**
         * @function Responsavél pela paginação
         * @param 
         * @returns
         */
        function Paginate() {
            var params = {
                'test_id': $scope.filters.test_id,
                'uad_id': $scope.filters.uad_id,
                'esc_id': $scope.filters.esc_id !== undefined ? $scope.filters.esc_id.Id : null,
                'tur_id': $scope.filters.tur_id !== undefined ? $scope.filters.tur_id.Id : null,
                'DateStartChange': $scope.filters.DateStartChange,
                'DateEndChange': $scope.filters.DateEndChange
            };

            $scope.paginate.paginate(params).then(function (result) {
                $scope.message = false;
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.responseChangeLogList = result.lista;
                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.responseChangeLogList = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            }, function () {
                $scope.message = true;
                $scope.listResult = null;
                $scope.listResultSME = null;
            });
        };
        $scope.Paginate = Paginate;

        /**
        * @function Acionar date-picker através de btn
        * @param
        * @returns
        */
        $scope.datepicker = function __datepicker(id) { $("#" + id).datepicker('show'); };

        /**
         * @function Abrir/fechar painel de filtros
         * @param
         * @returns
         */
        $scope.open = function __open() {

            $('.side-filters').toggleClass('side-filters-animation').promise().done(function a() {

                if (angular.element(".side-filters").hasClass("side-filters-animation")) {
                    angular.element('body').css('overflow', 'hidden');
                }
                else {
                    angular.element('body').css('overflow', 'inherit');
                }
            });
        };

        /**
         * @function Fechar painel de filtros por click da page
         * @param
         * @returns
         */
        function close(e) {
            var element_in_painel = false;
            if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('next') && e.target.tagName === "TH") ||
                ($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
	            ($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
	            ($(e.target).hasClass('day') && e.target.tagName === "TD") ||
	            $(e.target).parent().is("[data-side-filters]") ||
                e.target.hasAttribute('data-side-filters'))
                return;

            if (angular.element(".side-filters").hasClass("side-filters-animation")) $scope.open();
        };

        /**
         * @function Obter todas Escolas
         * @name getSchools
         * @namespace ResponseChangeController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getSchools = function __getSchools() {

            if (!$scope.filters.uad_id)
                return;

            if (!$scope.filters.test_id)
                return;

            $scope.DreDescription = $('#filterDre option:selected').text();

            var params = { 'testId': $scope.filters.test_id, 'dre_id': $scope.filters.uad_id };

            AdherenceModel.getAdheredSchoolSimple(params, function (result) {

                if (result.success) {
                    $scope.listSchools = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
         * @function Obter todas as turmas por escola
         * @name getTeams
         * @namespace ResponseChangeController
         * @memberOf Controller
         * @public
         * @param
         * @return
         */
        $scope.getTeams = function __getTeams() {

            if (!$scope.filters.esc_id)
                return;
            if (!$scope.filters.test_id)
                return;

            $scope.SchoolDescription = $('#filterSchool option:selected').text();

            var params = {
                'test_id': $scope.filters.test_id,
                'ttn_id': 0,
                'esc_id': $scope.filters.esc_id.Id,
                'crp_ordem': 0
            };
           
            AdherenceModel.getSectionGrid(params, function (result) {

                if (result.success) {
                    $scope.listTeams = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };


        $scope.getNameTeam = function __getNameTeam() {
            $scope.TeamDescription = $('#filterTeam option:selected').text();
        };

        /**
         * @function Iniciar
         * @param
         * @returns
         */
        (function __init() {
            $notification.clear();
            $scope.clearFilters();
            angular.element('body').click(close);
            $(document).ready(function () {
                var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

                var startDate = new Date();
                startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
                $("#dateStart").datepicker("setDate", startDate);

                var endDate = new Date();
                endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
                $("#dateEnd").datepicker("setDate", endDate);

                configVariables();
            });
        })();
    }


})(angular, jQuery);