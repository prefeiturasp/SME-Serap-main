/**
 * function ModelTestListController Controller
 * @namespace Controller
 * @author julio cesar da silva - 20/05/2015
 */
(function (angular, $) {

	'use strict';
	
	//~SETTER
	angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
        .module('appMain')
        .controller("ModelTestListController", ModelTestListController);

	ModelTestListController.$inject = ['$rootScope', '$scope', '$notification', '$pager', '$util', 'ModelTestModel'];
	
	function ModelTestListController($rootScope, $scope, $notification, $pager, $util, ModelTestModel) {

		var params = $util.getUrlParams();

		var hasRegistered = false;
		$scope.$watch(function __cycleAngular() {
			if (hasRegistered) return;
			hasRegistered = true;
			$scope.$$postDigest(function __postDisgestAngular() {
				hasRegistered = false;
			});
		});

		$scope.setFilters = function () {
			$scope.fieldSearch = JSON.parse(JSON.stringify($scope.pesquisa));
			$scope.paginate.indexPage(0);
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.pageSize = $scope.paginate.getPageSize();
			$scope.search();
		};

		$scope.search = function () {
			$scope.paginate.paginate({ search: $scope.fieldSearch }).then(function (result) {
				if (result.success) {
					if (result.lista.length > 0) {
						$scope.paginate.nextPage();
						$scope.modelTestList = result.lista;
						if (!$scope.pages > 0) {
							$scope.pages = $scope.paginate.totalPages();
							$scope.totalItens = $scope.paginate.totalItens();
						}
					} else {
						$scope.message = true;
						$scope.modelTestList = null;
					}
				}
				else {
					$notification[result.type ? result.type : 'error'](result.message);
				}

			}, function () {
				$scope.message = true;
				$scope.modelTestList = null;
			});
		};

		$scope.confirmar = function (modelTest) {
			$scope.modelTest = modelTest;
			angular.element('#deleteModal').modal('show');
		};

		$scope.delete = function (modelTest) {
			ModelTestModel.delete({ Id: modelTest.Id }, function (result) {
				if (result.success) {
					$notification.success(result.message);
					$scope.setFilters();
				} else {
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
			angular.element('#deleteModal').modal('hide');
		};

		(function initialize() {
			$notification.clear();
			$scope.paginate = $pager(ModelTestModel.search);
			$scope.pesquisa = '';
			$scope.message = false;
			$scope.modelTestList = null;
			$scope.modelTest = null;
			$scope.pages = 0;
			$scope.totalItens = 0;
			$scope.pageSize = 10;
			$scope.search();
		}).call(this);
	};

})(angular, jQuery);