"use strict";

function abrirPaginaDeUploadDeArquivos() {
    try {
        $(".page").hide();
        $("#uploadArquivos-page").show();
    }
    catch (error) {
        voltarParaConfiguracoes();
    }
};

function voltarParaConfiguracoes() {
    $(".page").hide();
    $("#configuracoes-page").show();
};

function obterUsuario() {
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