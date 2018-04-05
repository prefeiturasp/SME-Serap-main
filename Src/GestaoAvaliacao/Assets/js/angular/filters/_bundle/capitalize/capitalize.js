/**
 * @function Capitalize
 * @namespace Filter
 * @author julio.silva@mstech.com.br
 * @since 08/11/2016
 */
(function (angular, $) {

    'use strict';

    angular
        .module('filters')
        .filter('capitalize', filterMinimize);

    function filterMinimize() {

        return function (value) {

            if (typeof value === 'string') {

                if (value.length > 0)
                    return value.charAt(0).toUpperCase() + value.slice(1);
                else
                    return value;
            }
            else {
                return '';
            }
        }
    };
})(angular, jQuery);