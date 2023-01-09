
(function (angular, $) {

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ImportarResultadosPSPController", ImportarResultadosPSPController);

    ImportarResultadosPSPController.$inject = ['$rootScope', '$scope', 'ImportarResultadosPSPModel', '$notification', '$timeout', '$sce', '$pager', '$compile', '$util', '$http',];


    function ImportarResultadosPSPController($rootScope, $scope, ImportarResultadosPSPModel, $notification, $timeout, $sce, $pager, $compile, $util, $http) {

        var self = this;
        var params = $util.getUrlParams();

        $scope.tipoResultado = null;
        $scope.listaTiposResultados = [];
        $scope.listaImportacoes = null;
        $scope.codigoOuNomeArquivo = "";
        $scope.arquivoSelecionado = null;
        $scope.paginate = $pager(ImportarResultadosPSPModel.carregaImportacoes);
        $scope.pageSize = 10;

        $scope.load = function _load() {
            $notification.clear();
            $scope.carregaTiposResultados();
            $scope.carregaImportacoes();
        };

        $scope.pesquisarArquivo = function _pesquisarArquivo() {
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            $scope.carregaImportacoes('paginate', $scope.codigoOuNomeArquivo);
        };

        $scope.carregaImportacoes = function __Importacoes(paginate, codigoOuNomeArquivo) {
            $scope.listaImportacoes = [];

            $scope.paginate.paginate(codigoOuNomeArquivo).then(
                function (result) {
                    if (result.success) {
                        if (result.lista.length > 0) {
                            $scope.paginate.nextPage();
                            $scope.listaImportacoes = result.lista;
                            $scope.pageSize = result.pageSize;
                            if (!$scope.pages > 0) {
                                $scope.pages = $scope.paginate.totalPages();
                                $scope.totalItens = $scope.paginate.totalItens();
                            }
                        } else {
                            $scope.listaImportacoes = null;
                        }
                        if (!codigoOuNomeArquivo && codigoOuNomeArquivo == undefined) $scope.codigoOuNomeArquivo = "";
                    } else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });

        }

        $scope.selecionarArquivo = function __selecionarArquivo(element) {
            $scope.arquivoSelecionado = element.files[0];
            console.log($scope.arquivoSelecionado);
        }

        $scope.salvarImportacao = function __salvarImportacao() {
            var form = new FormData();
            form.append('file', $scope.arquivoSelecionado);
            form.append('codigoTipoResultado', $scope.tipoResultado.Codigo);
            $http.post($util.getWindowLocation('/ImportarResultadosPSP/ImportarArquivoResultado'), form, {
                transformRequest: angular.identity,
                headers: {
                    'Content-Type': undefined,
                    //'__XHR__': function () {
                    //    return function (xhr) {
                    //        xhr.upload.addEventListener("progress", function (event) {
                    //            $scope.progressup = parseInt(((event.loaded / event.total) * 100));
                    //            if ($scope.progressup < 99)
                    //                angular.element('#' + $scope.component.Guid).css('width', $scope.progressup + '%');
                    //        });
                    //    };
                    //}
                }
            })
                .success(function (data, status) {
                    if (data.success) {
                        $scope.tipoResultado = null;
                        $scope.arquivoSelecionado = null;
                        $scope.carregaImportacoes();
                        $notification.success("Arquivo importado com sucesso!");
                    }
                    else {
                        $notification[data.type ? data.type : 'error'](data.message);
                    }
                })
                .error(function (data, status) {
                    $notification.error(data);
                });

        }

        $scope.carregaTiposResultados = function __carregaTiposResultados() {
            ImportarResultadosPSPModel.carregaTiposResultadoPspAtivos(function (result) {
                if (result.success) {
                    if (result.lista.length > 0) {
                        $scope.listaTiposResultados = result.lista;
                    } else {
                        $scope.listaTiposResultados = null;
                    }
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.load();

    };

})(angular, jQuery);