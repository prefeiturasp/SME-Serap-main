"use strict";

function abrirPaginaDeUploadDeRevistasEBoletins() {
    try {
        $(".page").hide();
        $("#uploadRevistasBoletins-page").show();
        obterUsuario();
        validarLotesAtivos();
    }
    catch (error) {
        voltarParaTiposDeArquivos();
    }
};

function exibirMenu() {
    $(".page").hide();
    $("#uploadRevistasBoletinsMenu-page").show();
    $("#divItensDetalhesDoLote").hide();
};


function voltarParaTiposDeArquivos() {
    $(".page").hide();
    $("#uploadArquivos-page").show();
};

function validarLotesAtivos() {
    $.post(getLoteUrl("CancelActiveBatches"), { UsuId: Usuario.usu_id })
        .done(function (result) {
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            voltarParaTiposDeArquivos();
        });
};