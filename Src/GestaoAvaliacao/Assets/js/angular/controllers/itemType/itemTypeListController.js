(function (angular, $) {

	'use strict';

	// App
	var AppItemType = angular.module('appMain', ['services', 'filters', 'directives']);

	
	// Controller lista
	AppItemType.controller("ListCtrl", ['ItemTypeModel', '$scope', '$notification', '$pager', ListCtrl]);
	function ListCtrl(ItemTypeModel, ng, $notification, $pager) {

		/**
         * @function Load
         * @param {Object} $event Evento gerado pela mudança de rota
         * @param {Route} current RotaParameter
         * @private
         */
		function load($event, current) {
			
			$notification.clear();
			ng.paginate = $pager(ItemTypeModel.search);
			ng.pesquisa = '';
			ng.message = false;
			ng.itemTypeList = null;
			ng.testType = null;
			ng.pages = 0;
			ng.totalItens = 0;
			ng.pageSize = 10;
			ng.load();
		};

		/**
         * @function Copia o valor da pesquisa
         * @param
         * @public
         */
		ng.Search = function () {
			ng.fieldSearch = angular.copy(ng.pesquisa);
			ng.paginate.indexPage(0);
			ng.pageSize = ng.paginate.getPageSize();
			ng.pages = 0;
			ng.totalItens = 0;
			ng.load();
		};

		/**
         * @function Load
         * @param
         * @public
        */
		ng.load = function () {

			ng.paginate.paginate({ search: ng.fieldSearch }).then(function (result) {
				if (result.success) {
					if (result.lista.length > 0) {
						ng.itemTypeList = result.lista;
						if (!ng.pages > 0) {
							ng.pages = ng.paginate.totalPages();
							ng.totalItens = ng.paginate.totalItens();
						}
					}
				} else {
					ng.message = true;
					ng.itemTypeList = null;
				}

			}, function () {
				ng.message = true;
				ng.itemTypeList = null;
			});

		};

		load();

	};

})(angular, jQuery);