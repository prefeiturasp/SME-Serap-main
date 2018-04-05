/**
 * @function Realiza minimização de um texto dado length (size)
 * @namespace Filter
 * @author Julio Cesar da Silva - 02/02/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('filters')
        .filter('minimize', filterMinimize);

    function filterMinimize() {
        var DEFAULT_SIZE = 20;
        return function (value, size, pattern) {
            var _pattern = pattern || "...";
            var _len = parseInt(size || DEFAULT_SIZE);
            if (typeof value === 'string') {
                if (value.length > _len)
                    return value.substring(0, _len).concat(_pattern);
                else
                    return value;
            }
            else {
                return '';
            }
        }
    };
})(angular, jQuery);