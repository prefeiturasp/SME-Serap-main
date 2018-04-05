/**
 * function AdministrativeUnitTypeController Controller
 * @namespace Controller
 */
(function (angular, $) {
    //~SETTER
    angular
		.module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
		.module('appMain')
		.controller("AdministrativeUnitTypeController", AdministrativeUnitTypeController);

    AdministrativeUnitTypeController.$inject = ['$scope', '$notification', '$util', '$timeout', 'AdministrativeUnitTypeModel'];

    function AdministrativeUnitTypeController(ng, $notification, $util, $timeout, AdministrativeUnitTypeModel) {

        //Objects        
        ng.validationErrors = [];

        ng.init = init;
        ng.validate = validate;
        var sucessMessage = "Tipos de unidade salvos com sucesso.";

        function init() {
            getUnitiesAvailables();
        };

        function validate() {

            ng.validationErrors = [];

            if (ng.unitiesSelecteds.length <= 0) {
                ng.validationErrors.push({ errorMessage: "É necessário selecionar pelo menos um tipo de unidade." });
                return;
            }

            var idList = [];

            for (var i = 0; i < ng.unitiesAvailables.length; i++) {
                idList.push(ng.unitiesAvailables[i].id);
            }

            //vm.promise = TransferCalculationModel.GetListById(idList).then(function (response) {
            //    vm.listUadId = response.data;
            //    var item = null;

            //    for (var i = 0; i < vm.listUadId.length; i++) {
            //        item = _.filter(vm.unitiesAvailables, { 'id': vm.listUadId[i] });
            //        vm.unitiesSelecteds.push(item[0]);
            //        var index = vm.unitiesAvailables.indexOf(item[0]);
            //        vm.unitiesAvailables.splice(index, 1);
            //    }

            //    if (item) {
            //        sucessMessage = "Algumas unidades precisaram permanecer selecionadas por estarem em uso no sistema.";
            //    }

            //    save();

            //}).catch(function (e) {
            //    notification.error("Erro ao buscar os valores para cálculo de repasse.");
            //    $log.error(e.data);
            //});
        }

        function getUnitiesAvailables() {
            AdministrativeUnitTypeModel.get(function (result) {
                if (result.success) {

                    if (result.lista.length > 0) {
                        ng.unitiesAvailables = result.lista;
                        getUnitiesSelecteds();
                    } else {
                        ng.message = true;
                        ng.unitiesAvailables = [];
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            });
        }

        function getUnitiesSelecteds() {
            AdministrativeUnitTypeModel.getAdministrativeUnitsTypes(function (result) {
                if (result.success) {

                    if (result.lista.length > 0) {
                        ng.unitiesSelecteds = result.lista;
                    } else {
                        ng.message = true;
                        ng.unitiesSelecteds = [];
                    }
                } else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }

        ng.save = function __save() {
            AdministrativeUnitTypeModel.save(ng.unitiesSelecteds, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        }
    }

})(angular);
