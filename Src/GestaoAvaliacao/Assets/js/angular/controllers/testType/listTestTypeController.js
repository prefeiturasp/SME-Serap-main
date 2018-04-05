/**
 * @function Consultar tipo de prova
 * @namespace Controller
 * @author Alexandre Calil Blasizza Paravani - 01/03/2015
 * @author Julio Cesar da Silva - 02/03/2016
 */
(function (angular, $) {

	'use strict';

	angular
        .module('appMain', ['services', 'filters', 'directives']);

	angular
        .module('appMain')
        .controller("ListTestTypeController", ListTestTypeController);

	ListTestTypeController.$inject = ['$scope', '$rootScope', '$notification', '$pager', 'TestTypeModel'];
	
	function ListTestTypeController($scope, rootScope, $notification, $pager, TestTypeModel) {

		function Init() {

			$notification.clear();
			$scope.paginate = $pager(TestTypeModel.search);
			$scope.pesquisa = '';
			$scope.message = false;
			$scope.testTypeList = null;
			$scope.testType = null;
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.pageSize = 10;
			$scope.load();
		};

		$scope.copySearch = function () {
			$scope.fieldSearch = angular.copy($scope.pesquisa);
			$scope.paginate.indexPage(0);
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.pageSize = $scope.paginate.getPageSize();
			$scope.load();
		};

		$scope.load = function () {
			
			$scope.paginate.paginate({ search: $scope.fieldSearch }).then(function (result) {
				if (result.success) {
					if (result.lista.length > 0) {
						
						$scope.testTypeList = result.lista;
						if (!$scope.pages > 0) {
							$scope.pages = $scope.paginate.totalPages();
							$scope.totalItens = $scope.paginate.totalItens();
						}
					}
				} else {
					$scope.message = true;
					$scope.testTypeList = null;
				}

			}, function () {
				$scope.message = true;
				$scope.testTypeList = null;
			});
		};

		$scope.confirmar = function (testType) {

			$scope.testType = testType;
			angular.element('#modal').modal('show');
		};

		$scope.delete = function (testType) {

			TestTypeModel.delete({ Id: testType.Id }, function (result) {

				if (result.success) {

					$notification.success(result.message);

					$scope.testTypeList.splice($scope.testTypeList.indexOf(testType), 1);

					$scope.copySearch();
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});

			angular.element('#modal').modal('hide');
		};

		Init();
	};

})(angular, jQuery);