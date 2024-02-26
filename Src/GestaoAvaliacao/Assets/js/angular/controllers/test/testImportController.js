/**
 * @function Test Import Controller
 * @params {Object} angular
 * @params {Object} $
 * @author Julio Cesar Silva 23/5/2016
 */
(function (angular, $) {

	'use strict';

	//~SETTER
	angular
		.module('appMain', ['services', 'filters', 'directives']);

	//~GETTER
	angular
		.module('appMain')
		.controller("TestImportController", TestImportController);

	TestImportController.$inject = ['$scope', '$timeout', '$util', 'TestImportExportModel', '$pager', '$notification', '$http','FileModel'];

	/**
	 * @function Importar arquivos
	 * @param {Object} $scope
	 * @param {Object} $timeout
	 * @param {Object} $util
	 * @param {Object} TestImportExportModel
	 * @param {Object} $pager
	 * @param {Object} $notification
	 * @returns
	 */
	function TestImportController($scope, $timeout, $util, TestImportExportModel, $pager, $notification, $http, FileModel) {
		/**
		 * @function Pesquisa de importação
		 * @param
		 * @returns
		 */
		$scope.getFileImported = function __getFileImported() {
			$scope.import.pagination.paginate()
			.then(function (result) {
				if (result.success) {
					if (result.lista.length > 0) {
						$scope.import.pagination.nextPage();
						$scope.filesImported = angular.copy(result.lista);
						if (!$scope.import.pages > 0) {
							$scope.import.pages = $scope.import.pagination.totalPages();
							$scope.import.totalItens = $scope.import.pagination.totalItens();
						}
					}
				}
				else {
					$scope.filesImported = [];
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		/**
		 * @function Pesquisa de exportação
		 * @param
		 * @returns
		 */
		$scope.getFileExport = function __getFileExport(_param_) {
			$scope.export.pagination.paginate((typeof _param_ === "string" || _param_ === undefined) ? undefined : _param_)
			.then(function (result) {
				if (result.success) {
					$scope.filesExported = angular.copy(result.lista);
					if (!$scope.export.pages > 0) {
						$scope.export.pages = $scope.export.pagination.totalPages();
						$scope.export.totalItens = $scope.export.pagination.totalItens();
					}
				}
				else {
					$scope.filesExported = [];
					$notification[result.type ? result.type : 'error'](result.message);
				}
			});
		};

		$scope.modalExportFile = function __modalExportFile(file) {
			$scope.setCurrent(file);
			angular.element("#modalExportar").modal({ backdrop: 'static' });
		},

		/**
		 * @function Realizar pedido de exportação
		 * @param
		 * @returns
		 */
		$scope.exportFile = function __exportFile(file) {
			angular.element('#modalExportar').modal('hide');

			TestImportExportModel.solicitExport({ TestId: file.Test_Id },
			function (result) {
				if (result.success) {
					$notification.success(result.message)
				}
				else {
					$notification.alert(result.message);
				}

				$scope.getFileExport($scope.filters);
			});
		};

		/**
		 * @function Filtros para pesquisa de exportação
		 * @param
		 * @returns
		 */
		$scope.filterExport = function __filterExport() {
			$scope.export.pages = 0;
			$scope.export.totalItens = 0;
			$scope.export.pagination.indexPage(0);
			$scope.export.pageSize = $scope.import.pagination.getPageSize();
			$scope.getFileExport($scope.filters);
		};

		/**
		 * @function Abrir datepicker por botão
		 * @param {int} id
		 * @returns
		 */
		$scope.datepicker = function (id) {
			$("#" + id).datepicker('show');
		};

		/**
		 * @function Selecionar arquivo para delete
		 * @param {Object} file
		 * @returns
		 */
		$scope.setCurrent = function __setCurrent(file) {
			$scope.currentfile = file;
		};

		/**
		* @function deletar arquivo
		* @param
		* @returns
		*/
		$scope.delete = function __delete() {
			angular.element('#modalExcluir').modal('hide');
			TestImportExportModel.remove(function (result) {
			   if (result.success) {
				   $scope.getFileImported();
			   }
			   else {
				   $notification[result.type ? result.type : 'error'](result.message);
			   }
		   });
		};

		/**
		 * @function Realizar download
		 * @param {Object} element
		 * @returns
		 */
		$scope.downloadFile = function __downloadFile(fileId) {
			FileModel.checkFileExists({ Id: fileId }, function (result) {
				if (result.success) {
					window.open("/File/DownloadFile?Id=" + fileId, "_self");
				}
				else {
					$notification.alert("Arquivo não encontrado");
				}
			});
		};

		/**
		 * @function Realizar download
		 * @param {Object} element
		 * @returns
		 */
		$scope.downloadFileResultadoProva = function __downloadFileResultadoProva(testId, fileId) {
			FileModel.checkFileExistsResultadoProva({ TestId: testId, Id: fileId }, function (result) {
				if (result.success) {
					window.open("/File/DownloadFileResultadoProva?TestId=" + testId + "&Id=" + fileId, "_self");
				}
				else {
					$notification.alert("Arquivo não encontrado");
				}
			});
		};

		/**
		 * @function Limpar filtros de pesquisa para exportação (consulta de provas)
		 * @param {Object} element
		 * @returns
		 */
		$scope.clear = function __clear() {
			$scope.filters = {
				Code: undefined,
				StartDate: undefined,
				EndDate: undefined,
				Sistema: 1
			};
			$scope.countFilter = 1;
		};

		/**
		 * @function Realizar o upload
		 * @param {Object} element
		 * @returns
		 */
		$scope.upload = function __upload(element) {

			if ($scope.uploading) return;
			$scope.uploading = true;
			$scope.progress = undefined;
			var form = new FormData();
			form.append('file', element);
			form.append('fileType', "File");
			$http.post($util.getWindowLocation('/File/UploadFile'), form, {
				transformRequest: angular.identity,
				headers: {
					'Content-Type': undefined,
					'__XHR__': function () {
						return function (xhr) {
							xhr.upload.addEventListener("progress", function (event) {
								$scope.progress = parseInt(((event.loaded / event.total) * 100));
								$scope.safeApply();
							});
						};
					}
				}
			})
			.success(function (data, status) {

				if (data.success) {
					$scope.uploading = false;
					$scope.progress = undefined;
					$scope.getFileImported();
				} else {
					$scope.uploading = false;
					$scope.progress = undefined;
					$notification[data.type ? data.type : 'error'](data.message);
				}
				document.getElementById("fileInport").value = "";
			})
			.error(function (data, status) {
				document.getElementById("fileInport").value = "";
				$scope.uploading = false;
				$scope.progress = undefined;
				$notification[data.type ? data.type : 'error'](data.message);
			});
		};

		/**
		 * @function Forçar ciclo $diggest angularJS
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

		/**
		 * @function Abrir painel de filtros
		 * @param
		 * @returns
		 */
		$scope.open = function __open() {

			$('.filters').toggleClass('filters-animation').promise().done(function a() {
				if (angular.element(".filters").hasClass("filters-animation"))
					angular.element('body').css('overflow', 'hidden');
				else
					angular.element('body').css('overflow', 'inherit');
			});
		};

		/**
		 * @function Fechar painel de filtros
		 * @param {Object} e = event
		 * @returns
		 */
		$scope.close = function __close(e) {

			if (($(e.target).hasClass('datepicker-switch') && e.target.tagName === "TH") ||
				($(e.target).hasClass('prev') && e.target.tagName === "TH") ||
				($(e.target).hasClass('next') && e.target.tagName === "TH") ||
				($(e.target).hasClass('dow') && e.target.tagName === "TH") ||
				($(e.target).hasClass('year') && e.target.tagName === "SPAN") ||
				($(e.target).hasClass('month') && e.target.tagName === "SPAN") ||
				($(e.target).hasClass('day') && e.target.tagName === "TD") ||
				$(e.target).parent().is("[data-filters]") ||
				e.target.hasAttribute('data-filters'))
				return;

			if (angular.element(".filters").hasClass("filters-animation")) $scope.open();
		};

		/**
		 * @function Inicializar
		 * @param
		 * @returns
		 */
		(function initialize() {

			$notification.clear();
			var params = $util.getUrlParams();
			angular.element('body').click($scope.close);
			$scope.filesImported = null;
			$scope.filesExported = null;
			$scope.itemsSystemOptions = [{ id: 0, label: "SERAp" }, { id: 1, label: "SERAp Estudantes" }]
			$scope.export = {
				pagination: $pager(TestImportExportModel.exportAnalysisSearch),
				pageSize: 10,
				pages: 0,
				totalItens: 0
			};
			$scope.import = {
				pagination: $pager(TestImportExportModel.importAnalysisSearch),
				pageSize: 10,
				pages: 0,
				totalItens: 0
			};
			$scope.popover = {
				remove: function __remove() {
					angular.element("#modalExcluir").modal({ backdrop: 'static' });
				},
				donwloadFile: function __donwloadFile() {
					$scope.downloadFile();
				}
			};
			$scope.import.pagination.indexPage(0);
			$scope.export.pagination.indexPage(0);
			$scope.import.pageSize = $scope.import.pagination.getPageSize();
			$scope.export.pageSize = $scope.export.pagination.getPageSize();
			$scope.global = true;
			$scope.aplicada = false;
			$scope.tab = 1;
			$scope.clear();
			$scope.getFileExport($scope.filters);
			$scope.countFilter = 0;

			$scope.$watchCollection('filters', function () {
				$scope.countFilter = 0;
				if ($scope.filters.StartDate) $scope.countFilter += 1;
				if ($scope.filters.EndDate) $scope.countFilter += 1;
				if ($scope.filters.Code) $scope.countFilter += 1;
				if ($scope.filters.Sistema) $scope.countFilter += 1;
			}, true);
		})($scope);

	};

})(angular, jQuery);