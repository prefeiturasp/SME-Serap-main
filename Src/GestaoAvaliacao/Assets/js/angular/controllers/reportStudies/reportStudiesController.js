
(function (angular, $) {

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives', 'tooltip']);

    //~GETTER
    angular
        .module('appMain')
        .controller("ReportStudiesController", ReportStudiesController);

    ReportStudiesController.$inject = ['$scope', 'ReportStudiesModel', '$notification', '$pager', '$util', '$http', '$q', '$window', '$rootScope'];


    function ReportStudiesController($scope, ReportStudiesModel, $notification, $pager, $util, $http, $q, $window, $rootScope) {

        var self = this;
        var params = $util.getUrlParams();

        $scope.editMode = false;
        $scope.processando = true;
        $scope.tipoResultado = null;
        $scope.listaGrupos = [];
        $scope.listaDestinatarios = [];
        $scope.listaImportacoes = null;
        $scope.campoPesquisa = "";
        $scope.arquivoSelecionado = null;
        $scope.arquivoSelecionadoCsv = null;
        $scope.resultImportarCsv = null;
        $scope.paginate = $pager(ReportStudiesModel.carregaImportacoes);
        $scope.pageSize = 10;
        $scope.grupo = {};
        $scope.destinatario = { Id: undefined, text: undefined };
        $scope.arquivoEditar = {};

        $scope.load = function _load() {
            self.chamadasBack = {};
            $notification.clear();
            $scope.carregaImportacoesPaginado(null);

            $(".comboListagrupo").select2({
                multiple: false,
                placeholder: "Selecione um grupo",
                width: '100%',
                ajax: {
                    url: "reportstudies/listargrupos",
                    dataType: 'json',
                    data: function (params, page) {
                        return {
                            description: params.term
                        };
                    },
                    processResults: function (data, page) {
                        return { results: data.lista };
                    }
                }
            });

            $(".comboListagrupoEditar").select2({
                multiple: false,
                placeholder: "Selecione um grupo",
                width: '100%',
            });

            $(".comboListaDestinatarioEditar").select2({
                multiple: false,
                placeholder: "selecione um destinatario",
                width: '100%',
            });

            $(".comboListaDestinatario").select2({
                placeholder: "Selecione a lista destinatário"
            });

        };

        $scope.pesquisarArquivo = function _pesquisarArquivo() {
            $scope.pages = 0;
            $scope.totalItens = 0;
            $scope.paginate.indexPage(0);
            $scope.pageSize = $scope.paginate.getPageSize();
            if ($scope.campoPesquisa === '' || $scope.campoPesquisa === null || $scope.campoPesquisa === undefined)
                $scope.carregaImportacoesPaginado(null);
            else
                $scope.carregaImportacoes($scope.campoPesquisa);
        };

        $scope.carregaImportacoes = function __Importacoes(campoPesquisa) {
            $scope.listaImportacoes = [];
            ReportStudiesModel.carregaImportacoes({ searchFilter: campoPesquisa },
                function (result) {
                    if (result.success) {
                        if (result.lista.length > 0) {
                            $scope.listaImportacoes = result.lista;
                            $scope.pageSize = result.pageSize;
                            $scope.totalItens = result.lista.length;
                        } else {
                            $scope.listaImportacoes = null;
                        }
                        $scope.campoPesquisa = "";
                    } else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
        };

        $scope.carregaGrupos = function __carregaGrupos() {
            ReportStudiesModel.listarGrupos({}, function (result) {
                if (result.success) {
                    $scope.listaGrupos = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $scope.carregadestinatarios = function __carregadestinatarios() {
            $(".comboListaDestinatario").select2(
                {
                    placeholder: "selecione um destinatario",
                    width: '100%',
                    ajax: {
                        url: "reportstudies/listardestinatarios",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                filtroDesc: params.term,
                                tipoGrupo: ($('.comboListagrupo').val())
                            };
                        },
                        processResults: function (data, page) {
                            $scope.listaDestinatarios = data.lista;
                            return { results: data.lista };
                        }
                    }
                });
        };

        $scope.destinatarioMudou = function __destinatarioMudou() {
            console.log('UadCodigoDestinatario', $scope.arquivoEditar.UadCodigoDestinatario);
        };

        $scope.carregadestinatariosEditar = function __carregadestinatariosEditar() {
            $(".comboListaDestinatarioEditar").select2(
                {
                    placeholder: "selecione um destinatario",
                    width: '100%',
                    ajax: {
                        url: "reportstudies/listardestinatarios",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                filtroDesc: params.term,
                                tipoGrupo: $scope.arquivoEditar.STipoGrupo
                            };
                        },
                        processResults: function (data, page) {
                            return { results: data.lista };
                        }
                    }
                });
        };

        $scope.carregadestinatariosEditarInicial = function __carregadestinatariosEditarInicial(arquivo) {
            ReportStudiesModel.listarDestinatariosEditarInicial({ tipoGrupo: arquivo.STipoGrupo, filtroDesc: arquivo.UadCodigoDestinatario }, function (result) {
                if (result.success) {
                    $scope.listaDestinatarios = result.lista;
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

        $('.comboListagrupo').on("select2:select", function (e) {
            $(".comboListaDestinatario").empty().trigger('change');
        });

        $scope.carregaImportacoesPaginado = function __ImportacoesPaginado(paginate) {
            $scope.listaImportacoes = [];

            $scope.paginate.paginate().then(
                function (result) {
                    if (result.success) {
                        if (result.lista.length > 0) {
                            $scope.listaImportacoes = result.lista;
                            $scope.pageSize = result.pageSize;
                            $scope.totalItens = $scope.paginate.totalItens();

                            if (!$scope.pages > 0) {
                                $scope.pages = $scope.paginate.totalPages();
                            }
                        } else {
                            $scope.listaImportacoes = null;
                        }
                    } else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
        };

        $scope.callModalNovaImportacao = function __callModalNovaImportacao() {
            $scope.limparDados();
            angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
        };

        $scope.callModalEditarImportacao = function __callModalEditarImportacao(arquivo) {
            $scope.limparDados();
            $scope.editMode = true;

            $scope.carregaGrupos();
            $scope.arquivoEditar = angular.copy(arquivo);
            $scope.carregadestinatariosEditarInicial($scope.arquivoEditar);
            $scope.carregadestinatariosEditar();

            console.log('arquivo', $scope.arquivoEditar);

            angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
        };

        $scope.callModalImportarCsvEdicaoLote = function __callModalImportarCsvEdicaoLote() {
            angular.element("#modalImportarCsvEdicaoLote").modal({ backdrop: 'static' });
        };

        $scope.selecionarArquivo = function __selecionarArquivo(element) {
            $scope.arquivoSelecionado = element.files[0];
            $scope.validacoesArquivo();
        };


        $scope.selecionarArquivoCsv = function __selecionarArquivoCsv(element) {
            $scope.arquivoSelecionadoCsv = element.files[0];
            $scope.validacoesArquivoCsv();
        };

        $scope.validacoesArquivo = function __validacoesArquivo() {
            var tamanhoArquivo = parseInt($scope.arquivoSelecionado.size);
            var fileSize = kmgtbytes(tamanhoArquivo);
            if ($scope.arquivoSelecionado.type !== 'text/html') {
                $scope.arquivoSelecionado = null;
                angular.element("input[type='file']").val(null);
                $notification['error']("Selecione um arquivo HTML");
                return false;
            }
        };


        $scope.validacoesArquivoCsv = function __validacoesArquivoCsv() {
            var tamanhoArquivo = parseInt($scope.arquivoSelecionadoCsv.size);
            var fileSize = kmgtbytes(tamanhoArquivo);
            if ($scope.arquivoSelecionadoCsv.type !== 'text/csv') {
                $scope.arquivoSelecionadoCsv = null;
                angular.element("input[type='file']").val(null);
                $notification['error']("Selecione um arquivo CSV.");
                return false;
            }
        };

        $scope.exibirLoading = function __exibirLoading(exibir) {
            if (exibir)
                angular.element('#div_loading').css('display', 'block');
            else
                angular.element('#div_loading').css('display', 'none');
        };

        $scope.limparDados = function __limpar() {
            $scope.editMode = false;
            $scope.arquivoEditar = {};
            $scope.grupo = null;
            $scope.destinatario = null;
            $scope.arquivoSelecionado = null;
            angular.element("input[type='file']").val(null);
        };

        $scope.limparDadosCsv = function __limparCsv() {
            $scope.arquivoSelecionadoCsv = null;
            angular.element("input[type='file']").val(null);
        };

        $scope.confirmarDeletar = function
            (item) {
            $scope.itemParaDeletar = item;
            angular.element('#modalDelete').modal({ backdrop: 'static' });
        };

        $scope.abrirLink = function __abrirLink(link) {
            $window.open(link, '_blank', 'noreferrer');
        };

        $scope.deletar = function __deletar() {
            ReportStudiesModel.delete({ id: $scope.itemParaDeletar.Codigo }, function (result) {
                if (result.success) {
                    $notification.success('Registro excluido com sucesso!');
                    $scope.load();
                } else {
                    if (result.type && result.message)
                        $notification[result.type ? result.type : 'error'](result.message);
                }
            });
            angular.element('#modalDelete').modal('hide');
        };

        $scope.salvarImportacao = function __salvarImportacao() {

            if ($scope.arquivoSelecionado === null || $scope.arquivoSelecionado === undefined) {
                $scope.callModalNovaImportacao();
                $notification['error']("Selecione um arquivo!");
                return false;
            }

            $scope.validacoesArquivo();
            $scope.exibirLoading(true);
            $scope.UploadFile().then(function (data) {
                if (data.success) {
                    $scope.limparDados();
                    $scope.exibirLoading(false);
                    $scope.carregaImportacoesPaginado();
                    $notification.success("Arquivo importado com sucesso!");
                }
                else {
                    $scope.limparDados();
                    $scope.exibirLoading(false);
                    $notification[data.type ? data.type : 'error'](data.message);
                }
            }, function (e) {
                $scope.limparDados();
                $scope.exibirLoading(false);
                $notification.error(e);
            });
        };

        $scope.UploadFile = function () {
            $scope.grupo.Codigo = $('.comboListagrupo').val();
            $scope.destinatario.Nome = $(".comboListaDestinatario").text();

            var form = new FormData();
            if ($scope.grupo !== null && $scope.grupo !== undefined) {
                form.append('TypeGroup', $('.comboListagrupo').val());
            }

            if ($scope.destinatario !== null && $scope.destinatario !== undefined) {
                form.append('Addressee', $(".comboListaDestinatario").text());
                form.append('uadCodigoDestinatario', $(".comboListaDestinatario").val());
            }

            form.append('Codigo', 0);

            form.append('file', $scope.arquivoSelecionado);
            form.append('Name', $scope.arquivoSelecionado.FileName);
            form.append('Link', '');

            var defer = $q.defer();
            $http.post("/ReportStudies/Save", form,
                {
                    headers: { 'Content-Type': undefined },
                    transformRequest: angular.identity
                })
                .success(function (d) {
                    defer.resolve(d);
                })
                .error(function (e) {
                    $notification.error(e);
                });

            return defer.promise;
        };

        function kmgtbytes(num) {
            if (num > 0) {
                if (num < 1024) { return [num, 'Byte'] }
                if (num < 1048576) { return [parseInt(num / 1024), 'KB'] }
                if (num < 1073741824) { return [parseInt(num / 1024 / 1024), 'MB'] }
                if (num < 1099511600000) { return [parseInt(num / 1024 / 1024 / 1024), 'GB'] }
                return [num / 1024 / 1024 / 1024 / 1024, "TB"]
            }
            return num
        };

        $scope.load();


        $scope.salvarImportacaoCsv = function __salvarImportacaoCsv() {

            if ($scope.arquivoSelecionadoCsv === null || $scope.arquivoSelecionadoCsv === undefined) {
                $scope.callModalImportarCsvEdicaoLote();
                $notification['error']("Selecione um arquivo!");
                return false;
            }
            $scope.validacoesArquivoCsv();
            $scope.exibirLoading(true);
            $scope.UploadFileCsv().then(function (data) {
                if (data.success) {
                    $scope.limparDadosCsv();
                    $scope.carregaImportacoesPaginado();
                    $scope.exibirLoading(false);
                    $scope.resultImportarCsv = data.retorno;

                    if ($scope.resultImportarCsv.QtdeErros > 0)
                        angular.element("#modalResultadoImportarCsv").modal({ backdrop: 'static' });
                    else
                        $notification.success("Importação realizada com sucesso.");

                }
                else {
                    $scope.limparDadosCsv();
                    $scope.exibirLoading(false);
                    $notification[data.type ? data.type : 'error'](data.message);
                }
            }, function (e) {
                $scope.limparDadosCsv();
                $scope.exibirLoading(false);
                $notification.error(e);
            });

        };

        $scope.UploadFileCsv = function () {
            var form = new FormData();
            form.append('file', $scope.arquivoSelecionadoCsv);

            var defer = $q.defer();
            $http.post("/ReportStudies/ImportCsv", form,
                {
                    headers: { 'Content-Type': undefined },
                    transformRequest: angular.identity
                })
                .success(function (d) {
                    defer.resolve(d);
                })
                .error(function (e) {
                    $notification.error(e);
                });

            return defer.promise;
        };

    };

})
    (angular, jQuery);