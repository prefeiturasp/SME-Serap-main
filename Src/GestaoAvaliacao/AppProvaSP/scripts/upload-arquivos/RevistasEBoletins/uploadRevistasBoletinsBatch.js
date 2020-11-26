"use strict";

var formularioParaBuscarHistoricoUploadRevistasBoletinsAberto = false;
var loteEnviado = false;
var uploadFileBatch = null;
var uploadFileItens = [];
var uploadFileItensCount = 0;
var uploadFileItensEnviados = 0;
var uploadFileItensComErro = [];
var quantidadeDeDeTentativasDeUpload = 1;

const quantidadeMaximaDeArquivosSendoEnviados = 5;
const quantidadeMaximaDeTentativasDeUploadPorArquivo = 3;
const desktopIniFile = "desktop.ini";

function getLoteUrl(endpoint) {
    return urlBackEnd + "api/UploadRevistasEBoletins/" + endpoint;
}

function abrirFormularioParaNovoLote() {
    let divNovoUploadRevistasBoletins = document.getElementById("divNovoUploadRevistasBoletins");
    let iniciarNovoLoteDeArquivosIcon = document.getElementById("iniciarNovoLoteDeArquivosIcon");

    if (formularioParaBuscarHistoricoUploadRevistasBoletinsAberto) {
        divNovoUploadRevistasBoletins.style.display = "none";
        iniciarNovoLoteDeArquivosIcon.innerHTML = "<span class='mdi mdi-arrow-down-drop-circle-outline'></span>";
    }
    else {
        divNovoUploadRevistasBoletins.style.display = "block";
        iniciarNovoLoteDeArquivosIcon.innerHTML = "<span class='mdi mdi-arrow-up-drop-circle-outline'></span>";
    }

    formularioParaBuscarHistoricoUploadRevistasBoletinsAberto = !formularioParaBuscarHistoricoUploadRevistasBoletinsAberto;
};

function criarNovoLote() {
    uploadFileBatch = null;
    uploadFileItens = [];
    let edicao = $("#novoLoteEdicao").val();
    let areaDeConhecimento = $("#novoLoteAreaDeConhecimento").val();
    let cicloDeAprendizagem = $("#novoLoteCicloDeAprendizagem").val();

    if (edicao == "" || areaDeConhecimento == "" || cicloDeAprendizagem == "") {
        ProvaSP_Alerta("Informações faltando", "É necessário preencher todos os campos do formulário para iniciar o lote.");
        return;
    }

    $.post(getLoteUrl("AddBatch"), { Edicao: edicao, AreaDeConhecimento: areaDeConhecimento, CicloDeAprendizagem: cicloDeAprendizagem, UsuId: Usuario.usu_id, UsuName: Usuario.usu_login })
        .done(function (result) {
            uploadFileBatch = result;
            exibirDetalhesDoLoteAtual();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

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
    $("#divItensDetalhesDoLote").hide();

    habilitarAcoes(true);
};

function carregarArquivos(e) {
    var fileselector = document.getElementById('fileselector');
    if (fileselector.files.length <= 0) {
        ProvaSP_Erro("Nenhum arquivo selecionado", "Não existem arquivos na pasta selecionada.");
        return;
    }

    filtrarArquivos();
    uploadFileItensCount = uploadFileItens.length;

    let quantidadeDeArquivos = document.getElementById("quantidadeDeArquivos");
    quantidadeDeArquivos.innerHTML = uploadFileItens.length + " arquivos";

    document.getElementById("detalhesDoLoteBarraDeProgresso").style.width = "1%";
    $("#divItensDetalhesDoLote").show();
}

function filtrarArquivos() {
    for (var i = 0; i < fileselector.files.length; i++) {
        if (!isIniFile(fileselector.files[i])) {
            uploadFileItens.push(fileselector.files[i]);
        }
    }
};

function isIniFile(file) {
    return file.name == desktopIniFile;
}

function habilitarAcoes(habilitar) {
    var btnAddArquivos = document.getElementById("btnAddArquivos");
    var btnIniciar = document.getElementById("btnIniciar");
    btnAddArquivos.disabled = !habilitar;
    btnIniciar.disabled = !habilitar;
};

function inicializarVariaveis() {
    uploadFileBatch = null;
    uploadFileItens = [];
    uploadFileItensComErro = [];
    document.getElementById("fileselector").value = "";
};

function iniciarLote() {
    habilitarAcoes(false);
    $.post(getLoteUrl("StartBatch"), { Id: uploadFileBatch.Id, FileCount: uploadFileItensCount })
        .done(function (result) {
            iniciarEnvio();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

function iniciarEnvio() {
    var queue = new RequestManager(quantidadeMaximaDeArquivosSendoEnviados, finalizarEnvio);

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
        url: getLoteUrl("UploadFile"),
        enctype: 'multipart/form-data',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            atualizarBarraDeTarefas();
        },
        error: (erro) => {
            reportarErroAoEnviarArquivo(file, erro);
        }
    };
};

function atualizarBarraDeTarefas() {
    var barraDeProgresso = document.getElementById("detalhesDoLoteBarraDeProgresso");
    var width = ++uploadFileItensEnviados * 100 / uploadFileItensCount;
    if (width > 100) width = 100;
    barraDeProgresso.style.width = width + "%";
};

function reportarErroAoEnviarArquivo(file, erro) {
    if (uploadFileItensComErro.length <= 0) {
        $("#divArquivosComErro").show();
    }

    uploadFileItensComErro.push(file);
    var tableArquivosComErro = document.getElementById("tableArquivosComErro");
    var row = tableArquivosComErro.insertRow(1);
    var cellTentativa = row.insertCell(0);
    var cellNomeDoArquivo = row.insertCell(1);
    var cellDescricaoDoErro = row.insertCell(2);
    cellTentativa.innerHTML = quantidadeDeDeTentativasDeUpload;
    cellNomeDoArquivo.innerHTML = file.name;
    cellDescricaoDoErro.innerHTML = erro.statusText; 
};

function finalizarEnvio() {
    if (uploadFileItensComErro.length > 0 && quantidadeDeDeTentativasDeUpload < quantidadeMaximaDeTentativasDeUploadPorArquivo) {
        reenviarItensComErro();
        return;
    }

    $("#btnUploadArquivosHistoricoVoltar").hide();
    $("#btnFinalizarLote").show();

    if (uploadFileItensComErro.length > 0) {
        ProvaSP_Alerta("Lote finalizado", "Alguns arquivos não puderam ser enviados ao servidor. Verifique na lista abaixo.");
    }
    else {
        ProvaSP_Alerta("Sucesso", "Lote enviado com sucesso.");
    }
}

function finalizarLote() {
    $.post(getLoteUrl("FinalizeBatch"), { Id: uploadFileBatch.Id, FileErrorCount: uploadFileItensComErro.length })
        .done(function (result) {
            inicializarVariaveis();
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

function reenviarItensComErro() {
    if (quantidadeDeDeTentativasDeUpload > quantidadeMaximaDeTentativasDeUploadPorArquivo) {
        finalizarLote();
        return;
    }

    quantidadeDeDeTentativasDeUpload++;
    let descricaoDaAcao = document.getElementById("descricaoDaAcao");
    descricaoDaAcao.innerHTML = "Reenviando arquivos com erro";

    uploadFileItens = uploadFileItensComErro;
    uploadFileItensComErro = [];
    iniciarEnvio();
}

function confirmarCancelamento() {
    $("#cancelarLoteLoad").show();
    cancelarUpload();
}

function cancelarUpload() {
    $.post(getLoteUrl("CancelBatch"), { Id: uploadFileBatch.Id })
        .done(function (result) {
            inicializarVariaveis();
            jQuery("#modalCancelarLoteDeUpload").modal("hide");
            $("#cancelarLoteLoad").hide();
            exibirMenu();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

window.onbeforeunload = validarFechamentoDaTela;

function validarFechamentoDaTela() {
    if (uploadFileBatch != null) {
        return "Existe um lote em andamento. O upload será cancelado.";
    }
}