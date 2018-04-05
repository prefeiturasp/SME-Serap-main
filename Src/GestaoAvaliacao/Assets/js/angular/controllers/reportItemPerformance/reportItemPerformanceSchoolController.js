(function (angular, $) {

    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
		.module('appMain')
		.controller("ReportItemPerformanceSchoolController", ReportItemPerformanceSchoolController);

    ReportItemPerformanceSchoolController.$inject = ['$scope', 'ReportItemPerformanceModel', 'TestModel', 'DisciplineModel', 'AdherenceModel', '$notification', '$timeout', '$pager', '$util'];


    function ReportItemPerformanceSchoolController($scope, ReportItemPerformanceModel, TestModel, DisciplineModel, AdherenceModel, $notification, $timeout, $pager, $util) {

        $scope.profile = getCurrentVision();
        $scope.params = $util.getUrlParams();
        $scope.hasFilters = false;
        //if ($scope.profile == EnumVisions.UNIT_ADMINISTRATIVE) $scope.hasFilters = true;

        $scope.WARNING_UPLOAD_BATCH_DETAIL = Boolean(Parameters.General.WARNING_UPLOAD_BATCH_DETAIL == "True");

        /**
        * @function Iniciar
        * @param
        * @returns
        */
        function Init() {
            if ($scope.hasFilters) {
                $scope.filters = {}
                $scope.$watch("filters", function () {
                    $scope.countFilter = 0;
                    if ($scope.filters.DateStart) $scope.countFilter += 1;
                    if ($scope.filters.DateEnd) $scope.countFilter += 1;
                    if ($scope.filters.test_id) $scope.countFilter += 1;
                }, true);
            }
            else if (!$scope.params.uad_id || !$scope.params.test_id) {
                $notification.alert("URL inválida.");
                $timeout(function () {
                    window.history.back();
                }, 3000);
                return;
            }
            configVariables();
            if (!$scope.hasFilters) $scope.search();
            if ($scope.profile == EnumVisions.ADMINISTRATOR || $scope.profile == EnumVisions.MANAGER) getHeaderDetails();
        };


        /**
        * @function Responsavél por instanciar tds as variaveis
        * @param 
        * @returns
        */
        function configVariables() {
            // :paginação
            $scope.paginate = $pager(ReportItemPerformanceModel.getSchools);
            $scope.totalItens = 0;
            $scope.pages = 0;
            $scope.pageSize = 10;
            $scope.message = false;
            $scope.listResult = null;
            $scope.listDiscipline = [];
            $scope.oneDiscipline = false;

            // :vars            
            $scope.header = {
                DRE: undefined,
                Test: undefined
            };
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
         * @function obter detalhes de cabeçalho da page
         * @param {object} _callback
         * @returns
         */
        function getHeaderDetails(_callback) {
            TestModel.getInfoUadReport({
                Test_id: $scope.params.test_id,
                uad_id: $scope.params.uad_id
            }, function (result) {
                if (result.success) {
                    $scope.header = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                    $timeout(function () {
                        window.location.href = "/ReportItemPerformance/"
                    }, 3000);
                }
                if (_callback) _callback();
            });
        };
        $scope.getHeaderDetails = getHeaderDetails;

        function getHeaderDetailsForVision(_callback) {
            var test_id = $scope.params.test_id || $scope.filters.test_id;
            if (!test_id) return;
            TestModel.getInfoTestReport({ Test_id: test_id }, function (result) {
                if (result.success) {
                    $scope.header = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
        * @function Limpar filtros
        * @param 
        * @returns
        */
        $scope.clearFilters = function __clearFilters() {
            $scope.filters = {};
            $scope.filters.test_id = "";
            $scope.filters.DateStart = "";
            $scope.filters.DateEnd = "";
        };

        /**
       * @function Validar período de datas
       * @param 
       * @returns
       */
        function validateDate() {
            if (!$scope.filters.DateStart || !$scope.filters.DateEnd) {
                $notification.alert("É necessário selecionar um período.");
                return false;
            }
            if ($util.greaterEndDateThanStartDate($scope.filters.DateStart, $scope.filters.DateEnd) === false) {
                $notification.alert("Data inicial não pode ser maior que a data final.");
                $scope.filters.DateEnd = "";
                return false;
            }
            return true;
        };

        /**
        * @function obter as disciplinas da prova
        * @param {object} _callback
        * @returns
        */
        function getDisciplines(_callback) {
            if (!$scope.searcheableFilter.test_id) return;
            DisciplineModel.loadComboByTest({ Test_id: $scope.searcheableFilter.test_id }, function (result) {
                if (result.success) {                   
                    $scope.listDiscipline = result.lista;
                    $scope.searcheableFilter.discipline_id = "";
                    $scope.searcheableFilter.discipline_name = "";
                    $scope.oneDiscipline = false;

                    if ($scope.listDiscipline.length == 1) {
                        $scope.searcheableFilter.discipline_id = $scope.listDiscipline[0].Id;
                        $scope.searcheableFilter.discipline_name = $scope.listDiscipline[0].Description;
                        $scope.oneDiscipline = true;
                    }

                    if ($scope.params.discipline_id != null) {
                        $scope.searcheableFilter.discipline_id = $scope.params.discipline_id;
                        var disciplineId = parseInt($scope.params.discipline_id);
                        $scope.searcheableFilter.index = $scope.listDiscipline.map(o => o.Id).indexOf(disciplineId);
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };
        $scope.getDisciplines = getDisciplines;

        /**
         * @function gerar o relatório através da disciplina selecionada
         * @param {object} _callback
         * @returns
         */
        function loadByDiscipline(_callback) {
            if ($scope.hasFilters)
                if (!validateDate() || !validate()) return;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            var disciplineId = parseInt($('#discipline').val().replace("number:", ""));
            $scope.searcheableFilter.discipline_id = disciplineId;
            $scope.params.discipline_id = disciplineId;
            setFilters();
            Paginate();
        }
        $scope.loadByDiscipline = loadByDiscipline;

        function changeTest(_callback) {
            if ($scope.filters.test_id > 0 && $('#selectTest').val() != "") {
                $('#testCode').val($scope.filters.test_id)
            }

            if (_callback) _callback();
        };
        $scope.changeTest = changeTest;

        function changeTestCode(_callback) {
            if ($scope.filters.test_id > 0 && $('#testCode').val() != "") {
                $('#selectTest').val($scope.filters.test_id)
            }

            if (_callback) _callback();
        };
        $scope.changeTestCode = changeTestCode;

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
            * @function validar
            * @param
            * @returns
            */
        function setFilters() {
            if ($scope.hasFilters) {
                $scope.searcheableFilter = angular.copy($scope.filters);
            }
            else {
                $scope.searcheableFilter = {
                    test_id: $scope.params.test_id,
                    uad_id: $scope.params.uad_id,
                    DateStart: $scope.params.dateStart,
                    DateEnd: $scope.params.dateEnd,
                    discipline_id: $scope.params.discipline_id                    
                };

            }
        };

        /**
		 * @function Zera a paginação e pesquisa novamente
		 * @param 
		 * @returns
		 */
        $scope.search = function __search() {
            if ($scope.hasFilters)
                if (!validateDate() || !validate()) return;
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            setFilters();
            getDisciplines();
            Paginate();
        };

        /**
		 * @function Responsavél pela paginação
		 * @param 
		 * @returns
		 */
        function Paginate() {
            $scope.paginate.paginate($scope.searcheableFilter).then(function (result) {
                $scope.message = false;
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.paginate.nextPage();
                        $scope.listResult = result.lista;
                        configSlider(result.lista);
                        $scope.QuantidadeTotal = result.QuantidadeTotal;
                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.listResult = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if ($scope.profile != EnumVisions.ADMINISTRATOR && $scope.profile != EnumVisions.MANAGER) getHeaderDetailsForVision();
            }, function () {
                $scope.message = true;
                $scope.listResult = null;
            });
        };
        $scope.Paginate = Paginate;

        /**
		 * @function Gera o relatório de desempenho da escola
		 * @param 
		 * @returns
		 */
        $scope.generateReport = function __generateReport() {
            ReportItemPerformanceModel.exportSchool($scope.searcheableFilter, function (result) {
                if (result.success) {
                    window.open("/ReportItemPerformance/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		 * @function Redireciona para próxima tela
		 * @param 
		 * @returns
		 */
        $scope.nextRedirect = function __nextRedirect(school) {
            if (!$scope.hasFilters)
                window.location.href = "/ReportItemPerformance/IndexSchool" +
                    "?uad_id=" + $scope.params.uad_id +
                    "&esc_id=" + school.EscolaId +
                    "&test_id=" + $scope.params.test_id +
                    "&dateStart=" + $scope.params.dateStart +
		            "&dateEnd=" + $scope.params.dateEnd +
                    "&discipline_id=" + $scope.params.discipline_id
            else
                window.location.href = "/ReportItemPerformance/IndexSchool" +
                   "?esc_id=" + school.EscolaId +
                   "&test_id=" + $scope.filters.test_id +
                   "&dateStart=" + $scope.filters.DateStart +
                   "&dateEnd=" + $scope.filters.DateEnd +
                   "&discipline_id=" + $scope.params.discipline_id

            console.log($scope.params.discipline_id);
        };

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
         * @function Iniciar
         * @param
         * @returns
         */
        (function __init() {
            $notification.clear();
            if ($scope.hasFilters) $scope.clearFilters();
            angular.element('body').click(close);
            $(document).ready(function () {
                if ($scope.hasFilters) {
                    $timeout(function () {
                        var parametroQuantidadeMeses = parseInt(getParameterValue(parameterKeys[0].QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA));

                        var startDate = new Date();
                        startDate.setMonth(startDate.getMonth() - parametroQuantidadeMeses);
                        $("#dateStart").datepicker("setDate", startDate);

                        var endDate = new Date();
                        endDate.setMonth(endDate.getMonth() + parametroQuantidadeMeses);
                        $("#dateEnd").datepicker("setDate", endDate);
                    }, 0);
                }
                Init();
            });
        })();

        /**
		 * @function Configuração da slider para exibição das médias
		 * @name configSlider
		 * @namespace ReportItemPerformanceSchoolController
		 * @memberOf Controller
		 * @private
		 * @param {array} lista
		 * @return
		 */
        function configSlider(lista) {

            var interval = 15;

            $scope.slider = {
                min: 0,
                max: ((lista[0].Items.length - interval) < 0) ? 0 : lista[0].Items.length - interval,
                current: 0,
                interval: interval,
                length: lista[0].Items.length
            };

            $scope.begin();
        };

        //    /**
        //	 * @function Próximo slider
        //	 * @name next
        //	 * @namespace ReportItemPerformanceSchoolController
        //	 * @memberOf Controller
        //	 * @public
        //	 * @param
        //	 * @return
        //	 */
        $scope.next = function __next() {

            var nextValue = $scope.slider.current + 1;

            if (nextValue > $scope.slider.max)
                return;

            $scope.escolas = [];
            $scope.slider.current = nextValue;

            var quantidade = 0;
            var first = true;

            for (var a = $scope.slider.current; a < $scope.slider.length; a++) {
                if (quantidade < $scope.slider.interval) {

                    for (var d = 0; d < $scope.listResult.length; d++) {
                        if (first) {
                            $scope.escolas.push(angular.copy($scope.listResult[d]));
                            $scope.escolas[d].Items = [];
                        }

                        $scope.escolas[d].Items.push(angular.copy($scope.listResult[d].Items[a]));
                    }

                    first = false;
                    quantidade++;
                }
            }

            $scope.totalItensHorizontal = $scope.escolas[0].Items.length;
        };

        /**
		 * @function Anterior slider
		 * @name previus
		 * @namespace ReportItemPerformanceSchoolController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.previus = function __previus() {

            var previusValue = $scope.slider.current - 1;

            if (previusValue < 0)
                return;

            $scope.escolas = [];
            $scope.slider.current = previusValue;

            var quantidade = 0;
            var first = true;

            for (var a = $scope.slider.current; a < $scope.slider.length; a++) {
                if (quantidade < $scope.slider.interval) {

                    for (var d = 0; d < $scope.listResult.length; d++) {
                        if (first) {
                            $scope.escolas.push(angular.copy($scope.listResult[d]));
                            $scope.escolas[d].Items = [];
                        }

                        $scope.escolas[d].Items.push(angular.copy($scope.listResult[d].Items[a]));
                    }

                    first = false;
                    quantidade++;
                }
            }

            $scope.totalItensHorizontal = $scope.escolas[0].Items.length;
        };

        /**
		 * @function Começo slider
		 * @name begin
		 * @namespace ReportItemPerformanceSchoolController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.begin = function __begin() {

            $scope.escolas = [];

            for (var a = 0; a < $scope.listResult.length; a++) {

                $scope.escolas.push(angular.copy($scope.listResult[a]));
                setItensKeyBegin(a);
            }

        };

        /**
		 * @function Final slider
		 * @name end
		 * @namespace ReportItemPerformanceSchoolController
		 * @memberOf Controller
		 * @public
		 * @param
		 * @return
		 */
        $scope.end = function __end() {

            $scope.slider.current = $scope.slider.max;
            $scope.escolas = [];

            var first = true;

            for (var a = $scope.slider.length - 1 ; a >= 0; a--) {

                if (a >= $scope.slider.max) {
                    for (var d = 0; d < $scope.listResult.length; d++) {
                        if (first) {
                            $scope.escolas.push(angular.copy($scope.listResult[d]));
                            $scope.escolas[d].Items = [];
                        }

                        $scope.escolas[d].Items.push(angular.copy($scope.listResult[d].Items[a]));
                    }

                    first = false;
                }
            }

            for (var d = 0; d < $scope.escolas.length; d++) {
                $scope.escolas[d].Items = $scope.escolas[d].Items.reverse();
            }

            $scope.totalItensHorizontal = $scope.escolas[0].Items.length;
        };


        /**
          * @function Obter a lista visível para os dados (slider interval)
          * @name setItensKeyBegin
          * @namespace ReportItemPerformanceSchoolController
          * @memberOf Controller
          * @private
          * @param {int} index
          * @return
          */
        function setItensKeyBegin(index) {
            $scope.escolas[index].Items = []

            var quantidade = 0;

            $scope.slider.current = 0;

            for (var a = 0; a < $scope.slider.length; a++) {

                if (quantidade < $scope.slider.interval) {
                    $scope.escolas[index].Items.push(angular.copy($scope.listResult[index].Items[a]));
                }
                quantidade++;
            }

            $scope.totalItensHorizontal = $scope.listResult[index].Items.length;
        };
    };

})(angular, jQuery);