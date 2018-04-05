(function (angular) {

    'use strict'

    angular.module('services').provider('$pager', function Pager() {

        var defaults = {
            currentPage: 0, // Página atual
            pageSize: 10, // Número de itens por página
            totalPages: 0, // Quantidade calculada total de páginas
            totalItens: 0, // Quantidade total de itens, recebido no servidor
            sortDir: '', // Direção da ordenação
            sortField: '' // Campo de ordenação
        };

        "currentPage pageSize totalPages sortDir sortField".split(" ").forEach($.proxy(function (method) {

            this[methodName(method)] = function (value) {
                setConfig(method, value);
            };
        }, this));

        this.setDefaults = function (options) {
            defaults = angular.extend(defaults, options);
        };

        function setConfig(name, value) {
            defaults[name] = value;
        };

        function methodName(text) {
            var result = text.replace(/([A-Z])/g, "$1");
            return 'set' + result.charAt(0).toUpperCase() + result.slice(1);
        };

        this.$get = ['$q', '$timeout', function ($q, $timeout) {
            return function (request, customOptions) {

                var pager = {};
                var options = angular.extend(angular.copy(defaults), customOptions);

                "currentPage pageSize sortDir sortField totalItens".split(" ").forEach(function (method) {
                    pager[method] = function (value) {
                        return setOption(method, value);
                    };
                });

                pager.totalItens = function __totalItens() {
                    return options.totalItens;
                };

                pager.totalPages = function () {
                    return options.totalPages;
                };

                pager.nextPage = function __nextPage() {

                    if (options.currentPage > (options.totalItens / options.pageSize)) {
                        return options.totalPages;
                    }
                    else {
                        return ++options.currentPage;
                    }
                };

                pager.prevPage = function __prevPage() {
                    if (options.currentPage <= 0) {
                        return 0;
                    }
                    else {
                        return --options.currentPage;
                    }
                };

                pager.hasMorePages = function __hasMorePages() {
                    return options.totalPages > 0 && options.currentPage !== options.totalPages;
                };

                pager.indexPage = function __indexPage(index) {
                    options.currentPage = index;
                };

                pager.getIndexPage = function __getIndexPage() {
                    return options.currentPage;
                };

                pager.pageSize = function __pageSize(index) {
                    options.pageSize = index;
                };

                pager.getPageSize = function __getPageSize() {
                    return options.pageSize;
                };

                function setOption(name, value) {
                    if (value || value == 0) {
                        options[name] = value;
                        return value;
                    }
                    else {
                        return options[name];
                    }
                }

                pager.paginate = function (params) {

                    var defer = $q.defer();
                    var result;

                    // Transforma a request para passar os dados da paginação
                    params = angular.extend(params || {}, {
                        $pager: options
                    });

                    // Executa a requisição para buscar os dados
                    if (angular.isFunction(request)) {
                        result = request.call({}, params);
                    }

                    // Nesse ponto, verifico se o método passado é 
                    // à partir de uma instância do Model ou do próprio model.
                    // Caso seja o próprio model, result conterá um objeto $promise.
                    // Caso seja uma instância, result será a própria promise.
                    if (angular.isUndefined(result.$promise) && !angular.isFunction(result.then)) {
                        throw new Error("Uma promise não foi encontrada no resultado dos dados.");
                    }

                    // Quando os dados forem carregados, resolve a promise
                    $q.when(result.$promise || result).then(function (data) {

                        // Pega a resposta da paginação que vai vir na última posição da array
                        var totalItens = data.lista[data.lista.length - 1];
                        if (totalItens && typeof (totalItens) === 'object') {

                            if (totalItens.total && typeof (totalItens.total) === 'string') {
                                options.totalItens = parseInt(totalItens.total);
                                options.totalPages = Math.ceil(options.totalItens / options.pageSize);
                                data.lista.pop();
                            }
                        }

                        defer.resolve(data);

                    }, function (error) {

                        defer.reject(error);
                    });

                    return defer.promise;
                };

                return pager;
            };
        }];
    });

})(angular);