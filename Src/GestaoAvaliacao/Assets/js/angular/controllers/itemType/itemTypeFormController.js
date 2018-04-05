(function (angular, $) {

	'use strict';

	// App
	var AppItemTypeForm = angular.module('appMain', ['services', 'filters', 'directives']);

	// Controller Editar
	AppItemTypeForm.controller("FormCtrl", ['ItemTypeModel', '$scope', '$rootScope', '$location', '$notification', FormCtrl]);
	function FormCtrl(ItemTypeModel, ng, $rootScope, $location, $notification) {
		
		//ng.$on('$routeChangeSuccess', load);

		/**
		 * @function load
		 * @private
		 */
		function load() {
			$notification.clear();
			var id = $location.absUrl().split('IndexForm/')[1];
			if (id) {
				ng.itemTypeForEdit = id;
				getItemType();
			}//if
		};

		/**
		 * @function setNewDefault
		 * Seta novo tipo de prova padrão 
		 * @private
		 */
		ng.setNewDefault = setNewDefault
		function setNewDefault() {
			if (!ng.itemType.IsDefault) {
				angular.element('#modal').modal('show');
			}//if
			ng.itemType.IsDefault = true;
		}//setNewDefault

		/**
		 * @function Salvar
		 * Salva a alteração da descrição e o tipo de prova padrão
		 * @private
		 */
		ng.Salvar = Salvar;
		function Salvar() {
			
			ItemTypeModel.save(ng.itemType, function (result) {
				if (result.success) {
					$notification.success(result.message);
					window.location.href = base_url("ItemType");
				}else {
					$notification[result.type ? result.type : 'error'](result.message);
				}//else
			});
			
		}//Salvar

		/**
		 * @function Cancel
		 * Cancela a alteração do tipo de prova padrão
		 * @private
		 */
		ng.Cancel = Cancel
		function Cancel() {
			ng.itemType.IsDefault = false;
			angular.element('#modal').modal('hide');
		}//Cancel

		/**
		 * @function Cancelar
		 * Cancela a edção do tipo de prova e volta pra tela tipo do item "/ItemType"
		 * @private
		 */
		ng.Cancelar = Cancelar;
		function Cancelar() {
			window.location.href = base_url("ItemType");
		}//Cancelar

		/**
		 * @function getItemType
		 * @private
		 */
		function getItemType() {
			
			var bd = { id: ng.itemTypeForEdit };
			//Pega elemento caso exista, senão cria um novo
			ItemTypeModel.find(bd, function (result) {

					if (result.success) {
						ng.itemType = result.itemType;
						
						if (ng.itemType.IsDefault) {
							angular.element('#itemPadrao').attr('disabled', 'disabled');
						}//if

					} else {
						$notification[result.type ? result.type : 'error'](result.message);
					}//else
				});

		}//getItemType
		load();
	};

})(angular, jQuery);