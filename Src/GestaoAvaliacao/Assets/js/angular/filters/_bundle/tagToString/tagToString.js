/**
 * @function Realiza limpeza de strings em formato html
 * @namespace Filter
 * @author Julio Cesar da Silva - 07/03/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('filters')
        .filter('tagToString', tagToString);

    function tagToString() {

        return function (value, returnImgString) {

            if (value === null || value === undefined)
                return "";
            
            /*
            var regex = new RegExp(/\<[^\>]*\>/);
            if (regex.test(value)) {
                value = jQuery(value).text();
            } else {
                console.error("Erro no filtro 'tagToString'");
            }*/

            try { value = jQuery(value).text(); } catch (e) { return ""; }; //console.error("Erro no filtro 'tagToString'"); 

            if (returnImgString === true && value == "")
                value = "<somente imagem>";

            return value;
        }
    };

})(angular, jQuery);