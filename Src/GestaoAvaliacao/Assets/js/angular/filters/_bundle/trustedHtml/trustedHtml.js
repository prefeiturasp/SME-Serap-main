/**
 * @function Realiza permissão de string html para bind
 * @namespace Filter
 * @author Julio Cesar da Silva - 07/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('filters')
        .filter('trustedHtml', trustedHtml);

    trustedHtml.$inject = ['$sce'];

    function trustedHtml($sce) {

        return function (value) {

            if (value === null || value === undefined)
                return "";

            try {
                value = $sce.trustAsHtml(value);
            }
            catch (e) {
                console.error("Erro no filtro 'trustedHtml'.");
            };
            
            return value;
        }
    };

})(angular, jQuery);