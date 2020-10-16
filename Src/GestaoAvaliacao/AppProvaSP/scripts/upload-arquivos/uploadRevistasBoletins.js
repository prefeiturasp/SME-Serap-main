"use strict";

var formularioParaNovoLoteAberto = false;
var uploadFileBatch = null;
var uploadFileItens = [];
var uploadFileItensCount = 0;
var uploadFileItensEnviados = 0;
var uploadFileItensComErro = [];

const quantidadeMaximaDeArquivosSendoEnviados = 5;

function abrirPaginaDeUploadDeRevistasEBoletins() {
    try {
        $(".page").hide();
        $("#uploadRevistasBoletins-page").show();
        obterUsuario();
        validarLotesAtivos();
    }
    catch (error) {
        console.log(error);
    }
};

function getUrl(endpoint) {
    return urlBackEnd + "api/UploadRevistasEBoletins/" + endpoint;
}

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

// Menu events
function validarLotesAtivos() {
    $.post(getUrl("CancelActiveBatches"), { UsuId: Usuario.usu_id })
        .done(function (result) {
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            voltarParaTiposDeArquivos();
        });
};

function abrirFormularioParaNovoLote() {
    let divNovoUploadRevistasBoletins = document.getElementById("divNovoUploadRevistasBoletins");
    let paragraghIniciarNovoLoteDeArquivos = document.getElementById("iniciarNovoLoteDeArquivosIcon");

    if (formularioParaNovoLoteAberto) {
        divNovoUploadRevistasBoletins.style.display = "none";
        paragraghIniciarNovoLoteDeArquivos.innerHTML = "<span class='mdi mdi-arrow-down-drop-circle-outline'></span>";
    }
    else {
        divNovoUploadRevistasBoletins.style.display = "block";
        paragraghIniciarNovoLoteDeArquivos.innerHTML = "<span class='mdi mdi-arrow-up-drop-circle-outline'></span>";
    }

    formularioParaNovoLoteAberto = !formularioParaNovoLoteAberto;
};

function exibirMenu() {
    $("#uploadRevistasBoletinsMenu-page").show();
    $("#uploadRevistasBoletinsHistorico-page").hide();
    $("#uploadRevistasBoletinsDetalhesDoLoteAtual-page").hide();
    $("#divItensDetalhesDoLote").hide();
};

function voltarParaConfiguracoes() {
    $(".page").hide();
    $("#configuracoes-page").show();
};

function voltarParaTiposDeArquivos() {
    $(".page").hide();
    $("#uploadArquivos-page").show();
};

function criarNovoLote() {
    uploadFileBatch = null;
    let edicao = $("#novoLoteEdicao").val();
    let areaDeConhecimento = $("#novoLoteAreaDeConhecimento").val();
    let cicloDeAprendizagem = $("#novoLoteCicloDeAprendizagem").val();

    if (edicao == "" || areaDeConhecimento == "" || cicloDeAprendizagem == "") {
        ProvaSP_Alerta("Informações faltando", "É necessário preencher todos os campos do formulário para iniciar o lote.");
        return;
    }

    $.post(getUrl("AddBatch"), { Edicao: edicao, AreaDeConhecimento: areaDeConhecimento, CicloDeAprendizagem: cicloDeAprendizagem, UsuId: Usuario.usu_id })
        .done(function (result) {
            uploadFileBatch = result;
            habilitarAcoes(true);
            exibirDetalhesDoLoteAtual();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
}

// Detalhes do lote

function exibirDetalhesDoLoteAtual() {
    let spanTituloCabecalho = document.getElementById("spanTituloCabecalho");
    spanTituloCabecalho.innerHTML = "Lote " + uploadFileBatch.Id;

    let edicao = document.getElementById("detalhesDoLote-edicao");
    let areaDeConhecimento = document.getElementById("detalhesDoLote-areaDeConhecimento");
    let cicloDeAprendizagem = document.getElementById("detalhesDoLote-cicloDeAprendizagem");
    
    edicao.innerHTML = "Edição: " + uploadFileBatch.Edicao;
    areaDeConhecimento.innerHTML = "Área de conhecimento: " + uploadFileBatch.AreaDeConhecimento;
    cicloDeAprendizagem.innerHTML = "Ciclo de aprendizagem: " + uploadFileBatch.CicloDeAprendizagem;

    $("#uploadRevistasBoletinsDetalhesDoLoteAtual-page").show();
    $("#uploadRevistasBoletinsHistorico-page").hide();
    $("#uploadRevistasBoletinsMenu-page").hide();
};

function carregarItens(e) {
    var fileselector = document.getElementById('fileselector');
    if (fileselector.files.length <= 0) {
        ProvaSP_Erro("Nenhum arquivo selecionado", "Não existem arquivos na pasta selecionada.");
        return;
    }

    uploadFileItens = fileselector.files;
    uploadFileItensCount = uploadFileItens.length;

    let quantidadeDeArquivos = document.getElementById("quantidadeDeArquivos");
    quantidadeDeArquivos.innerHTML = uploadFileItens.length + " arquivos";

    $("#divItensDetalhesDoLote").show();
}

function habilitarAcoes(habilitar) {
    var btnAddArquivos = document.getElementById("btnAddArquivos");
    var btnIniciar = document.getElementById("btnIniciar");
    btnAddArquivos.disabled = !habilitar;
    btnIniciar.disabled = !habilitar;
};

function iniciarLote() {
    habilitarAcoes(false);
    $.post(getUrl("StartBatch"), { Id: uploadFileBatch.Id, FileCount: uploadFileItensCount })
        .done(function (result) {
            iniciarEnvio();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

function iniciarEnvio() {
    var queue = new RequestManager(quantidadeMaximaDeArquivosSendoEnviados, finalizarLote);

    for (var i = 0; i < uploadFileItens.length; i++) {
        var request = montarRequest(uploadFileItens[i]);
        queue.addRequest(request);
    }
}

function montarRequest(file) {
    var formData = new FormData();
    formData.append('file', file);
    formData.append('dto', JSON.stringify({ UploadFileBatchId: uploadFileBatch.Id, DirectoryPath: uploadFileBatch.DirectoryPath }) );  

    return {
        type: "POST",
        url: getUrl("UploadFile"),
        enctype: 'multipart/form-data',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            atualizarBarraDeTarefas();
        },
        error: (erro) => {
            reportarErroAoEnviarArquivo(file);
        }
    };
};

function atualizarBarraDeTarefas() {
    var barraDeProgresso = document.getElementById("detalhesDoLoteBarraDeProgresso");
    var width = ++uploadFileItensEnviados * 100 / uploadFileItensCount;
    if (width > 100) width = 100;
    barraDeProgresso.style.width = width + "%";
};

function reportarErroAoEnviarArquivo(file) {
    uploadFileItensComErro.push(file);
};

function finalizarLote() {
    if (uploadFileItensComErro.length > 0) {
        reenviarItensComErro();
        return;
    }

    $.post(getUrl("FinalizeBatch"), { Id: uploadFileBatch.Id })
        .done(function (result) {
            ProvaSP_Alerta("Sucesso", "Lote enviado com sucesso.");
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

function reenviarItensComErro() {

}

function confirmarCancelamento() {
    $("#cancelarLoteLoad").show();
    cancelarUpload();
}

function cancelarUpload() {
    $.post(getUrl("CancelBatch"), { Id: uploadFileBatch.Id })
        .done(function (result) {
            uploadFileBatch = null;
            jQuery("#modalCancelarLoteDeUpload").modal("hide");
            $("#cancelarLoteLoad").hide();
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

// Histórico
function exibirHistorico() {
    $("#uploadRevistasBoletinsHistorico-page").show();
    $("#uploadRevistasBoletinsDetalhesDoLoteAtual-page").hide();
    $("#uploadRevistasBoletinsMenu-page").hide();
};

function carregarHistorico() {

};

window.onbeforeunload = validarFechamentoDaTela;

function validarFechamentoDaTela() {
    if (uploadFileBatch != null) {
        return "Existe um lote em andamento. O upload será cancelado.";
    }
}