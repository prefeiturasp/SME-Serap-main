/**
 * @function Realiza limpeza de string com espaços em branco
 * @namespace Filter
 * @author Julio Cesar da Silva - 07/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('filters')
        .filter('changeBlankSpace', changeBlankSpace);

    function changeBlankSpace() {

        return function (value) {

            if (value === null || value === undefined)
                return "";

            try {
                value = value.replace(/\s\s/g, '&nbsp;&nbsp;').replace(/&nbsp;\s/g, '&nbsp;&nbsp;');
            }
            catch (e) {
                console.error("Erro no filtro 'changeBlankSpace'.");
            }

            return value;
        }
    };

})(angular, jQuery);