/**
 * Moment Service
 * @namespace Moment
 * @desc Formatação de data e hora Exemplo: <li>{{ banner.PostDate | moment}} </li> || var sPostDate = moment(postDate).format('DD/MM/YYYY');
 * @author Alexandre Garcia Simões - Mstech: 28/04/2015
 * @author Julio Cesar Silva - since: 18/10/2016
 *
 * https://github.com/moment/moment/issues/1407 - caso a data já esteja com o pattern ISO strings like '2013-05-10' não é necessário utilizar new Date(value)
 */
(function () {
    'use strict';

    angular
        .module('filters')
        .filter('moment', filterMoment);        

    function filterMoment() {
        return function (value, fmt, iso) {
            if (value) {
                if(iso)
                    return moment(new Date(value)).format(fmt || 'DD/MM/YYYY');
                else
                    return moment(value).format(fmt || 'DD/MM/YYYY');
            }
            else {
                return '';
            }
        }
    }
})();



