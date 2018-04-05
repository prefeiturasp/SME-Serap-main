(function (angular, $) {

    angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    angular
		.module('appMain')
		.controller("ReportStudentPerformanceController", ReportStudentPerformanceController);

    ReportStudentPerformanceController.$inject = ['$rootScope', '$scope', 'ReportStudentPerformanceModel', '$notification', '$window', '$util'];


    function ReportStudentPerformanceController($rootScope, $scope, ReportStudentPerformanceModel, $notification, $window, $util) {

        var urlParams = $util.getUrlParams();

        /**
         * @function Responsavél por instanciar tds as variaveis
         * @param 
         * @returns
         */
        function configVariables() {
            $scope.message = false;
            $scope.studendInformation = {};
            $scope.unitsInformation = [];
            getStudentInformation();
            getUnitsInformation();
        };

        /**
         * @function obter informações do aluno
         * @param {object} _callback
         * @returns
         */
        function getStudentInformation(_callback) {

            var params = {
                'test_id': urlParams.test_id,
                'alu_id': urlParams.alu_id,
                'dre_id': urlParams.dre_id
            }

            ReportStudentPerformanceModel.getStudentInformation(params, function (result) {
                if (result.success) {
                    $scope.studendInformation = result.data;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
         * @function obter informações da unidade
         * @param {object} _callback
         * @returns
         */
        function getUnitsInformation(_callback) {

            var params = {
                'test_id': urlParams.test_id,
                'esc_id': urlParams.esc_id,
                'dre_id': urlParams.dre_id,
                'team_id': urlParams.team_id
            };

            ReportStudentPerformanceModel.getUnitsInformation(params, function (result) {
                if (result.success) {
                    $scope.unitsInformation = result.data;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
                if (_callback) _callback();
            });
        };

        /**
        * @function Exporta para csv o relatório
        * @param 
        * @returns
        */
        $scope.generateReport = function __generateReport() {
            var params = {
                test_id: urlParams.test_id,
                alu_id: urlParams.alu_id,
                esc_id: urlParams.esc_id,
                dre_id: urlParams.dre_id,
                team_id: urlParams.team_id
            };

            ReportStudentPerformanceModel.exportReport(params, function (result) {
                if (result.success) {
                    window.open("/File/DownloadFile?Id=" + result.file.Id, "_self");
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        /**
		 * @function Redirecionar para tela de correção
		 * @param
		 * @return
		 */
        $scope.redirectToCorrection = function __redirectToCorrection() {
            $window.location.href = '/Correction?test_id=' + urlParams.test_id + '&team_id=' + urlParams.team_id + '&esc_id=' + urlParams.esc_id + '&dre_id=' + urlParams.dre_id;
        };

        /**
         * @function Iniciar
         * @param
         * @returns
         */
        (function __init() {
            $notification.clear();
            $(document).ready(function () {
                configVariables();
            });
        })();

    }


})(angular, jQuery);