"use strict";

var baseUrl = urlBackEnd + "api/UploadRevistasEBoletins/";

function abrirPaginaDeUploadDeRevistasEBoletins() {
    try {
        $(".page").hide();
        $("#uploadRevistasBoletins-page").show();
        load();
    }
    catch (error) {
        console.log(error);
    }
};

function exibirMenu() {

};

function exibirDetalhesDoLoteAutal() {

};

function exibirHistorico() {

};

function load() {
    if(Usuario == null) Usuario = ObterUsuario();

    $.get(baseUrl + "Load", { usuId: Usuario.usu_id })
        .done(function (result) {
            finalizaLoad(result);
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

function finalizaLoad(result) {
    debugger;
    if (result.BatchInProgressExists && result.BatchInProgressOwner) {
        $("#detalhesDoLoteAtual").show();
        $("#menuUploadRevistasBoletins").hide();
    }
    else {
        $("#menuUploadRevistasBoletins").show();
        $("#detalhesDoLoteAtual").hide();

        if (result.BatchInProgressExists)
            $("#iniciarNovoLoteDeArquivos").addClass("headerTitleDisabled");
    }
};

function ObterUsuario() {
    if (!mobile) {
        if (window.location.href.indexOf("file:///") == 0) {
            return JSON.parse(localStorage.getItem("Usuario"));
        }
        else {
            return jsonUsuario;
        }
    }
    else {
        return JSON.parse(localStorage.getItem("Usuario"));
    }
}