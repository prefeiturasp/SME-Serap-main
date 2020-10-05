"use strict";

function abrirPaginaDeUploadDeArquivos() {
    try {
        $(".page").hide();
        $("#uploadArquivos-page").show();
    }
    catch (error) {
        console.log(error);
    }
};