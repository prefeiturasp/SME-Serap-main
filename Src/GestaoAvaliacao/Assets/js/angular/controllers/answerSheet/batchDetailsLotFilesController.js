/**
 * function BatchDetails Controller
 * @namespace Controller
 * @author Julio Cesar Silva - 10/12/2015
 */
(function (angular, $) {

    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
		.controller("BatchDetailsLotFilesController", BatchDetailsLotFilesController);


    BatchDetailsLotFilesController.$inject = ['$rootScope', '$scope', '$notification', '$timeout', '$sce', '$pager', '$compile', '$util', '$window', 'AnswerSheetModel', 'AdherenceModel', 'FileModel', 'TestListModel', 'ParameterModel', '$interval'];

    /**
     * @function Controller para upload de folhas de resposta
     * @name BatchDetailsLotFilesController
     * @namespace Controller
     * @memberOf appMain
     * @memberOf appMain
     * @param {Object} $rootScope
     * @param {Object} $scope
     * @param {Object} $notification
     * @param {Object} $timeout
     * @param {Object} $sce
     * @param {Object} $pager
     * @param {Object} $compile
     * @param {Object} $util
     * @param {Object} $window
     * @param {Object} AnswerSheetModel
     * @return
     */
    function BatchDetailsLotFilesController($rootScope, $scope, $notification, $timeout, $sce, $pager, $compile, $util, $window, AnswerSheetModel, AdherenceModel, FileModel, TestListModel, ParameterModel, $interval) {

        function Init() {
            $notification.clear();

            $scope.paginate = $pager(AnswerSheetModel.getBatchAnswerSheetDetail);
            $scope.loadDisciplines();

        };

        $scope.loadDisciplines = function __loadDisciplines() {

            $scope.paginate.paginate($scope.currentFilter).then(function (result) {

                if (result.success) {

                    if (result.lista.length > 0) {

                        $scope.paginate.nextPage();

                        $scope.list.displayed = angular.copy(result.lista);
                        $scope.filtredDateUpdate = result.filterDateUpdate;

                        if (!$scope.pages > 0) {
                            $scope.pages = $scope.paginate.totalPages();
                            $scope.totalItens = $scope.paginate.totalItens();
                        }
                    } else {
                        $scope.message = true;
                        $scope.list.displayed = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            }, function () {
                $scope.message = true;
                $scope.list.displayed = null;
            });

            //$scope.filters.BatchQueueFile.value = { Id: _batchQueueFileId, Name: _fileName }
            //angular.element(document.getElementById('uploadQueueModal')).modal('hide');
            //$scope.setPagination();
        };


        $scope.params = $util.getUrlParams();
        Init();
    };

})(angular, jQuery);