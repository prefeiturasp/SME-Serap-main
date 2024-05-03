
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
        $scope.grupo = { "id": "0" };
        $scope.destinatario = { "id": "0", "text": "" };
        $scope.arquivoEditar = null;
        $scope.opcaoPadrao = { "id": "0", "text": "--Selecione uma opção--" };

        $scope.load = function _load() {
            self.chamadasBack = {};
            $notification.clear();
            $scope.carregaImportacoesPaginado(null);
            $scope.carregaGrupos();

            $(".comboListaDestinatarioEditar, .comboListaDestinatario").select2({
                multiple: false,
                placeholder: "--Selecione uma opção--",
                width: '100%',
            });

            $(".comboListagrupo, .comboListagrupoEditar").css({ "width": "100%" });

        };

        $('.comboListagrupoEditar').on('change', function () {
            $(".comboListaDestinatarioEditar").val("").change();
            $scope.destinatario = { "id": "0" };
        });

        $('.comboListagrupo').on('change', function () {
            $(".comboListaDestinatario").val("").change();
            $scope.destinatario = { "id": "0" };
        });

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
                    placeholder: "--Selecione uma opção--",
                    width: '100%',
                    ajax: {
                        url: "reportstudies/listardestinatarios",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                filtroDesc: params.term,
                                tipoGrupo: ($('.comboListagrupo').val().replace("string:", ""))
                            };
                        },
                        processResults: function (data, page) {
                            data.lista.unshift($scope.opcaoPadrao);
                            return { results: data.lista };
                        }
                    }
                });
        };

        $scope.carregadestinatariosEditar = function __carregadestinatariosEditar(arquivo) {
            $scope.setGrupoDestinatarioEditar(arquivo);
            $(".comboListaDestinatarioEditar").select2(
                {
                    placeholder: "--Selecione uma opção--",
                    width: '100%',
                    ajax: {
                        url: "reportstudies/listardestinatarios",
                        dataType: 'json',
                        data: function (params, page) {
                            return {
                                filtroDesc: params.term,
                                tipoGrupo: ($(".comboListagrupoEditar").val().replace("string:", ""))
                            };
                        },
                        processResults: function (data, page) {
                            data.lista.unshift($scope.opcaoPadrao);
                            $scope.setGrupoDestinatarioEditar(arquivo);
                            return { results: data.lista };
                        }
                    }
                });
        };

        $scope.carregadestinatariosEditarInicial = function __carregadestinatariosEditarInicial(arquivo) {
            $scope.setGrupoDestinatarioEditar(arquivo);
            var filtro = arquivo.STipoGrupo == "1" ? arquivo.UadCodigoDestinatario : "";
            ReportStudiesModel.listarDestinatariosEditarInicial({ tipoGrupo: arquivo.STipoGrupo, filtroDesc: filtro }, function (result) {
                if (result.success) {
                    $scope.setGrupoDestinatarioEditar(arquivo);
                    $scope.listaDestinatarios = angular.copy(result.lista);
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }
            });
        };

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
            $scope.carregadestinatarios();
            angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
        };

        $scope.callModalEditarImportacao = function __callModalEditarImportacao(arquivo) {
            $scope.limparDados();
            $scope.listaDestinatarios = null;
            $scope.editMode = true;
            $scope.carregaGrupos();
            $scope.grupo = { "id": arquivo.STipoGrupo };
            $scope.destinatario = { "id": arquivo.ObjDestinatario.id, "text": "" };
            $scope.arquivoEditar = angular.copy(arquivo);
            $scope.carregadestinatariosEditarInicial(arquivo);
            $scope.carregadestinatariosEditar(arquivo);
            angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
        };

        $scope.setGrupoDestinatarioEditar = function __setGrupoDestinatarioEditar(arquivo) {
            if (arquivo != null && arquivo != undefined) {
                if (arquivo.STipoGrupo != null && arquivo.STipoGrupo != undefined)
                    $scope.grupo = { "id": arquivo.STipoGrupo };
                else
                    $scope.grupo = { "id": "0" };

                if (arquivo.ObjDestinatario.id != null && arquivo.ObjDestinatario.id != undefined)
                    $scope.destinatario = { "id": arquivo.ObjDestinatario.id, "text": "" };
                else
                    $scope.destinatario = { "id": "0", "text": "" };
            }
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
            $scope.arquivoEditar = null;
            $scope.grupo = { "id": "0" };
            $scope.destinatario = { "id": "0", "text": "" };
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

        $scope.abrirLink = function __abrirLink(codigo) {
            ReportStudiesModel.checkReportStudiesExists({ Id: codigo }, function (result) {
                if (result.success) {
                    window.open("/ReportStudies/GetReportStudies?Id=" + codigo, "_blank");
                }
                else {
                    $notification.alert(result.message);
                }
            });
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

        $scope.salvarAlteracaoImportacao = function __salvarAlteracaoImportacao() {

            var id = $scope.arquivoEditar.Codigo;
            var tipoGrupo = ($(".comboListagrupoEditar").val());
            var uadCodigoDestinatario = ($(".comboListaDestinatarioEditar").val());

            if (tipoGrupo != null && tipoGrupo != undefined)
                tipoGrupo = tipoGrupo.replace("string:", "");

            if (uadCodigoDestinatario != null && uadCodigoDestinatario != undefined)
                uadCodigoDestinatario = uadCodigoDestinatario.replace("string:", "");

            var objValidacao = $scope.validaGrupoDestinatario(tipoGrupo, uadCodigoDestinatario);
            if (!objValidacao.sucesso) {
                $notification.alert(objValidacao.msg);
                angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
                return false;
            } else {
                ReportStudiesModel.update({
                    id: id,
                    tipoGrupo: tipoGrupo,
                    uadCodigoDestinatario: uadCodigoDestinatario
                }, function (result) {
                    if (result.success) {
                        $scope.limparDados();
                        $scope.exibirLoading(false);
                        $scope.carregaImportacoesPaginado();
                        $notification.success("Arquivo alterado com sucesso!");
                    }
                    else {
                        $scope.limparDados();
                        $scope.exibirLoading(false);
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        $scope.validaGrupoDestinatario = function __validaGrupoDestinatario(tipoGrupo, uadCodigoDestinatario) {
            var valid = true;
            var msg = "";

            if (tipoGrupo == "0" || tipoGrupo == "" || tipoGrupo == null || tipoGrupo == undefined) {
                valid = false;
                msg = "Selecione um grupo!";
            } else if ((tipoGrupo == "1" || tipoGrupo == "2") && (uadCodigoDestinatario == "0" || uadCodigoDestinatario == "" || uadCodigoDestinatario == null || uadCodigoDestinatario == undefined)) {
                valid = false;
                msg = "Selecione um destinatário!";
            }

            return {
                sucesso: valid,
                msg: msg
            };
        };

        $scope.salvarImportacao = function __salvarImportacao() {

            if ($scope.arquivoSelecionado === null || $scope.arquivoSelecionado === undefined) {
                $scope.callModalNovaImportacao();
                $notification['error']("Selecione um arquivo!");
                return false;
            }

            $scope.validacoesArquivo();
            $scope.exibirLoading(true);

            var tipoGrupo = ($(".comboListagrupo").val());
            var uadCodigoDestinatario = ($(".comboListaDestinatario").val());
            if (tipoGrupo !== null && tipoGrupo !== undefined) {
                tipoGrupo = tipoGrupo.replace("string:", "");
            }

            if (uadCodigoDestinatario !== null && uadCodigoDestinatario !== undefined) {
                uadCodigoDestinatario = uadCodigoDestinatario.replace("string:", "");
            }

            var objValidacao = $scope.validaGrupoDestinatario(tipoGrupo, uadCodigoDestinatario);
            if (!objValidacao.sucesso) {
                $scope.exibirLoading(false);
                $notification.alert(objValidacao.msg);
                angular.element("#modalNovaImportacao").modal({ backdrop: 'static' });
                return false;
            }

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
            $scope.grupo.Codigo = $('.comboListagrupo').val().replace("string:", "");

            var tipoGrupo = ($(".comboListagrupo").val());
            var uadCodigoDestinatario = ($(".comboListaDestinatario").val());

            var form = new FormData();
            if (tipoGrupo !== null && tipoGrupo !== undefined) {
                form.append('TypeGroup', tipoGrupo.replace("string:", ""));
            }

            if (uadCodigoDestinatario !== null && uadCodigoDestinatario !== undefined) {
                form.append('uadCodigoDestinatario', uadCodigoDestinatario.replace("string:", ""));
            }

            form.append('file', $scope.arquivoSelecionado);

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