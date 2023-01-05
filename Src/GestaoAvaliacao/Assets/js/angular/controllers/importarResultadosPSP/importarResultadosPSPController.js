
(function (angular, $) {

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives', 'tooltip']);

	//~GETTER
	angular
		.module('appMain')
		.controller("ImportarResultadosPSPController", ImportarResultadosPSPController);

	ImportarResultadosPSPController.$inject = ['$rootScope', '$scope', 'ImportarResultadosPSPModel', '$notification', '$timeout', '$sce', '$pager', '$compile', '$util'];


	function ImportarResultadosPSPController($rootScope, $scope, ImportarResultadosPSPModel, $notification, $timeout, $sce, $pager, $compile, $util) {

		var self = this;
		var params = $util.getUrlParams();

		$scope.tipoResultado = null;
		$scope.listaTiposResultados = [];
		$scope.listaImportacoes = null;
		$scope.codigoOuNomeArquivo = "";
		$scope.paginate = $pager(ImportarResultadosPSPModel.carregaImportacoes);
		$scope.pageSize = 10;

		/**
		 * @function carrega funções de configuração
		 * @private
		 * @param
		 */
		$scope.load = function _load() {
			$notification.clear();
			carregaTiposResultados();
			$scope.carregaImportacoes();
		};

		$scope.carregaImportacoes = function __Importacoes(paginate, codigoOuNomeArquivo) {
			$scope.listaImportacoes = [];

			$scope.paginate.paginate(codigoOuNomeArquivo).then(
				function (result) {
					if (result.success) {
						if (result.lista.length > 0) {
							$scope.paginate.nextPage();
							$scope.listaImportacoes = result.lista;
							$scope.pageSize = result.pageSize;
							if (!$scope.pages > 0) {
								$scope.pages = $scope.paginate.totalPages();
								$scope.totalItens = $scope.paginate.totalItens();
							}
						} else {
							$scope.listaImportacoes = null;
						}
						if (!codigoOuNomeArquivo && codigoOuNomeArquivo == undefined) $scope.codigoOuNomeArquivo = "";
					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}
				});

        }

		$scope.fileUploadSuccess = function __fileUploadSuccess(data) {

			//$timeout(function () {
			//	angular.element('#' + $scope.modelFile.Guid).css('width', '0%');
			//	$scope.modelFile.Path = "";
			//}, 2000);

			//$scope.LimparCampos();
			//$scope.searchFile();
		};

		function carregaTiposResultados() {
			//TestGroupModel.loadGroupsSubGroups(function (result) {
			//	if (result.success) {
			//		ng.grupoSubgrupoList = result.groupSubGroup;
			//		ng.e1_grupoSubgrupo = setValuesComb(ng.grupoSubgrupoList, result.groupSubGroup);
			//	}
			//	else {
			//		$notification[result.type ? result.type : 'error'](result.message);
			//	}
			//});
			$scope.listaTiposResultados = [{ Id: 1, Description: "Resultado aluno" },
											{ Id: 2, Description: "Resultado turma" },
											{ Id: 3, Description: "Resultado escola" }];
		};


		$scope.load();

	};

})(angular, jQuery);