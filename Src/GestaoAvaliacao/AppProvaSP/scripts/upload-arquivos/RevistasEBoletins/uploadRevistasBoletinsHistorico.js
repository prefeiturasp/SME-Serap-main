"use strict";

var formularioParaBuscarHistoricoUploadRevistasBoletinsAberto = false;
var filtroAtualDoHistorico = {};
var historico = {};

function abrirFormularioParaBuscarHistorico() {
    let divBuscarHistoricoUploadRevistasBoletins = document.getElementById("divBuscarHistoricoUploadRevistasBoletins");
    let BuscarHistoricoUploadRevistasBoletinsIcon = document.getElementById("BuscarHistoricoUploadRevistasBoletinsIcon");

    if (formularioParaBuscarHistoricoUploadRevistasBoletinsAberto) {
        divBuscarHistoricoUploadRevistasBoletins.style.display = "none";
        BuscarHistoricoUploadRevistasBoletinsIcon.innerHTML = "<span class='mdi mdi-arrow-down-drop-circle-outline'></span>";
    }
    else {
        divBuscarHistoricoUploadRevistasBoletins.style.display = "block";
        BuscarHistoricoUploadRevistasBoletinsIcon.innerHTML = "<span class='mdi mdi-arrow-up-drop-circle-outline'></span>";
    }

    formularioParaBuscarHistoricoUploadRevistasBoletinsAberto = !formularioParaBuscarHistoricoUploadRevistasBoletinsAberto;
};

function getHistoricoUrl(endpoint) {
    return urlBackEnd + "api/UploadRevistasEBoletinsHistorico/" + endpoint;
}

function aplicarFiltro() {
    let edicao = $("#historicoEdicao").val();
    let areaDeConhecimento = $("#historicoAreaDeConhecimento").val();
    let cicloDeAprendizagem = $("#historicoCicloDeAprendizagem").val();

    if (edicao == "") {
        ProvaSP_Alerta("Informações faltando", "É necessário preencher ao menos o ano de edição para realizar a consulta.");
        return;
    }

    filtroAtualDoHistorico.Page = 1;
    filtroAtualDoHistorico.Edicao = edicao;
    filtroAtualDoHistorico.AreaDeConhecimento = areaDeConhecimento;
    filtroAtualDoHistorico.CicloDeAprendizagem = cicloDeAprendizagem;
    buscarLotes();
};

function limparDados() {
    var tableHistoricoDeLotes = document.getElementById("tableHistoricoDeLotes");
    for (var i = tableHistoricoDeLotes.rows.length - 1; i > 0; i--) {
        tableHistoricoDeLotes.deleteRow(i);
    }
};

function buscarLotes() {
    $.get(getHistoricoUrl("Get"), filtroAtualDoHistorico)
        .done(function (result) {
            historico = result;
            carregarHistorico(result);
            exibirHistorico();
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
};

// Histórico
function exibirHistorico() {
    $(".page").hide();
    $("#uploadRevistasBoletinsHistorico-page").show();
};

function carregarHistorico(result) {
    limparDados();
    carregarTitulo();
    carregarItens(result.Itens);
    carregarPaginaAtual(result);
};

function carregarTitulo() {
    let divhistoricoDeLotesTitulo = document.getElementById("divhistoricoDeLotesTitulo");
    var titulo = '<span>Busca por: </span><span class="filterItem">Edição: ' + filtroAtualDoHistorico.Edicao + '</span>';

    if (filtroAtualDoHistorico.AreaDeConhecimento != "") {
        let historicoAreaDeConhecimento = document.getElementById("historicoAreaDeConhecimento");
        titulo += '<span class="filterItem">Área: ' + historicoAreaDeConhecimento.options[historicoAreaDeConhecimento.selectedIndex].text + '</span>';
    }

    if (filtroAtualDoHistorico.CicloDeAprendizagem != "") {
        let historicoCicloDeAprendizagem = document.getElementById("historicoCicloDeAprendizagem");
        titulo += '<span class="filterItem">Ciclo: ' + historicoCicloDeAprendizagem.options[historicoCicloDeAprendizagem.selectedIndex].text + '</span>';
    }

    divhistoricoDeLotesTitulo.innerHTML = titulo;
};

function carregarItens(historicoItens) {
    if (historicoItens == null || historicoItens.length <= 0) {
        $("#divResultadosHistoricoDeLotes").hide();
        $("#divSemResultadosHistoricoDeLotes").show();
        return;
    }

    $("#divResultadosHistoricoDeLotes").show();
    $("#divSemResultadosHistoricoDeLotes").hide();
    var rowPosition = 0;
    for (var index = 0; index < historicoItens.length; index++) {
        rowPosition++;
        carregarCabecalho(historicoItens[index], rowPosition);
        rowPosition++;
        carregarDetalhes(historicoItens[index], rowPosition);
    }

    adicionarEventoDeClickDaLinhaDaTabela();
};

function carregarDetalhes(historicoItem, index) {
    var tableHistoricoDeLotes = document.getElementById("tableHistoricoDeLotes");
    var rowDetalhes = tableHistoricoDeLotes.insertRow(index);
    rowDetalhes.style.display = "none";

    var cellDetalhes = rowDetalhes.insertCell(0);
    cellDetalhes.colSpan = 4;

    var detalhesLinha1 = '<div class="detalhesHistoricoItem"><div style="width: 30%;"><span>Edição: </span>' + historicoItem.Edicao + '</div>';
    detalhesLinha1 += '<div style="width: 35%;"><span>Arquivos enviados: </span>' + historicoItem.FileCount + '</div>';
    detalhesLinha1 += '<div style="width: 35%;"><span>Arquivos com erro: </span>' + historicoItem.FileErrorCount + '</div></div>';

    var detalhesLinha2 = '<div class="detalhesHistoricoItem"><div style="width: 50%;"><span>Área de conhecimento: </span>' + historicoItem.AreaDeConhecimento + '</div>';
    detalhesLinha2 += '<div style="width: 50%;"><span>Ciclo de aprendizagem: </span>' + historicoItem.CicloDeAprendizagem + '</div></div>';

    cellDetalhes.innerHTML = detalhesLinha1 + detalhesLinha2;
};

function adicionarEventoDeClickDaLinhaDaTabela() {
    var rows = document.getElementsByClassName("rowCabecalho");
    for (var i = 0; i < rows.length; i++) {
        rows[i].onclick = function () {
            return function () {
                var rowDetalhes = this.nextElementSibling;
                if (rowDetalhes.style.display === "table-row") {
                    rowDetalhes.style.display = "none";
                } else {
                    rowDetalhes.style.display = "table-row";
                }
            };
        }(rows[i]);
    }
}

function carregarCabecalho(historicoItem, index) {
    var tableHistoricoDeLotes = document.getElementById("tableHistoricoDeLotes");
    var rowCabecalho = tableHistoricoDeLotes.insertRow(index);
    rowCabecalho.className = "rowCabecalho";
    var cellLote = rowCabecalho.insertCell(0);
    var cellCriadoPor = rowCabecalho.insertCell(1);
    var cellData = rowCabecalho.insertCell(2);
    var cellSituacao = rowCabecalho.insertCell(3);

    cellLote.innerHTML = historicoItem.UploadFileBatchId;
    cellCriadoPor.innerHTML = historicoItem.UserName;
    cellData.innerHTML = historicoItem.CreatedDate;
    cellSituacao.innerHTML = historicoItem.Situation; 
};

function carregarPaginaAtual(historico) {
    var spanPaginaAtual = document.getElementById("spanPaginaAtual");
    spanPaginaAtual.innerHTML = historico.Page;

    var spanPrimeiraPagina = document.getElementById("spanPrimeiraPagina");
    var spanPaginaAnterior = document.getElementById("spanPaginaAnterior");
    if (historico.Page <= 1) {
        spanPrimeiraPagina.style.display = "none";
        spanPaginaAnterior.style.display = "none";
    }
    else {
        spanPrimeiraPagina.style.display = "block";
        spanPaginaAnterior.style.display = "block";
    }

    var spanUltimaPagina = document.getElementById("spanUltimaPagina");
    var spanProximaPagina = document.getElementById("spanProximaPagina");
    if (historico.Page == historico.MaxPage) {
        spanUltimaPagina.style.display = "none";
        spanProximaPagina.style.display = "none";
    }
    else {
        spanUltimaPagina.style.display = "block";
        spanProximaPagina.style.display = "block";
    }
};

function primeiraPagina() {
    filtroAtualDoHistorico.Page = 1;
    buscarLotes();
};

function paginaAnterior() {
    filtroAtualDoHistorico.Page--;
    buscarLotes();
};

function proximaPagina() {
    filtroAtualDoHistorico.Page++;
    buscarLotes();
};

function ultimaPagina() {
    filtroAtualDoHistorico.Page = historico.MaxPage;
    buscarLotes();
};