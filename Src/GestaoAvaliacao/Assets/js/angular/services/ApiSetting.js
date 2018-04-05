/**
 * @function Factory para centralização de configuração de API
 * @author Julio Silva - 24/10/2016.
 */
(function () {

    'use strict';

    angular
        .module('services')
        .provider("$apiSettingConfig", $apiSettingConfig)
        .factory('$apiSetting', $apiSetting)
        .constant("$apiSettingOptions", {
            URL_METHOD_EXCLUDED: null,
            URL_AUTHENTICATION: null
        });

    $apiSettingConfig.$inject = ['$apiSettingOptions'];
    $apiSetting.$inject = ['$apiSettingConfig'];

    function $apiSettingConfig($apiSettingOptions) {

        // define qual método do back-end deve ser excluído do ciclo de validação (método chamado para obter token)
        var url_method_excluded = $apiSettingOptions.URL_METHOD_EXCLUDED;

        // define url de chamada para obter token 
        var url_authentication = $apiSettingOptions.URL_AUTHENTICATION;

        /**
         * @description Set
         * @param {String} _value
         * @returns
         */
        this.setUrlMethodExcluded = function __setUrlMethodExcluded(_value) {
            url_method_excluded = _value;
        };

        /**
         * @description Get
         * @param
         * @returns {String}
         */
        this.getUrlMethodExcluded = function __getUrlMethodExcluded() {
            return url_method_excluded;
        };

        /**
         * @description Set
         * @param {String} _value
         * @returns
         */
        this.setUrlAuthentication = function __setUrlAuthentication(_value) {
            url_authentication = _value;
        };

        /**
         * @description Get
         * @param
         * @returns {String}
         */
        this.getUrlAuthentication = function __getUrlAuthentication() {
            return url_authentication;
        };

        /**
         * @description Get url params
         * @param
         * @returns {String}
         */
        this.getUrlParams = function __getUrlParams() {

            var query_string = {};
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (typeof query_string[pair[0]] === "undefined") {
                    query_string[pair[0]] = decodeURI(pair[1]);
                } else if (typeof query_string[pair[0]] === "string") {
                    var arr = [query_string[pair[0]], pair[1]];
                    query_string[pair[0]] = arr;
                } else {
                    query_string[pair[0]].push(pair[1]);
                }
            }
            return query_string;
        };

        // return provider
        this.$get = function () {
            return this;
        };
    };

    /**
     * @function Construtor
     * @param
     * @returns
     */
    function $apiSetting($apiSettingConfig) {

        return {
            url_method_excluded: $apiSettingConfig.getUrlMethodExcluded(),
            url_authetication: $apiSettingConfig.getUrlAuthentication()
        };
    };
})();