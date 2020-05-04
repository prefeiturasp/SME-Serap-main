"use strict";

function parametrosDaRequisicaoDeExportacaoDeDadosEGraficos() {
    var objEnvio = {};
    var nivel = $("#ddlResultadoNivel").val();
    var edicao = $("#ddlResultadoEdicao").val();
    var ciclo = $("#ddlResultadoCiclo").val();
    var areaConhecimentoId = $("#ddlResultadoAreaConhecimento").val();
    var anoEscolar = $("#ddlResultadoAno").val();
    var lista_uad_sigla = "";
    var lista_esc_codigo = "";
    var lista_turmas = "";
    var lista_alu_matricula = "";

    if (nivel == "DRE") {
        lista_uad_sigla = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().toString();
    }
    else if (nivel == "ESCOLA") {
        lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
    }
    else if (nivel == "TURMA") {
        lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
        lista_turmas = $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get().toString();
    }
    else if (nivel == "ALUNO") {
        lista_alu_matricula = $(".resultado-aluno-item-chk:checked").map(function () { return this.value; }).get().toString();
    }

    objEnvio = {
        Nivel: nivel,
        Edicao: edicao,
        Ciclo: ciclo,
        AreaConhecimentoID: areaConhecimentoId,
        AnoEscolar: anoEscolar,
        lista_uad_sigla: lista_uad_sigla,
        lista_esc_codigo: lista_esc_codigo,
        lista_turmas: lista_turmas,
        lista_alu_matricula: lista_alu_matricula
    }

    return objEnvio;
}

/***** Ínicio do download do CSV Dos Alunos*/

$('.link-exportar-dados-csv-alunos').click(function () {
    gerarRelatorioEmCsvDosAlunos();
});

function gerarRelatorioEmCsvDosAlunos() {
    var objEnvio = parametrosDaRequisicaoDeExportacaoDeDadosEGraficos();

    $.mobile.loading("show", {
        text: "Aguarde a geração do csv...",
        textVisible: true,
        theme: "a",
        html: ""
    });


    $.post(urlBackEnd + "api/ResultadoPorNivel/download-csv-dre-detalhando-escolas-alunos?guid=" + newGuid(), objEnvio)
        .success(function (data) {
            var universalBOM = "\uFEFF";
            var blob = new Blob([universalBOM + data], { type: "text/csv;charset=UTF-8" });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ProficienciaMicrodados.csv";
            link.click();

            $.mobile.loading("hide");
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.responseText);
        });
}

/***** Fim do download do CSV */

/***** Ínicio do download do CSV dos gráficos de radar e barras*/

$('.link-exportar-dados-csv-graficos').click(function () {
    gerarRelatorioEmCsvDosGraficos();
});

function gerarRelatorioEmCsvDosGraficos() {
    var objEnvio = parametrosDaRequisicaoDeExportacaoDeDadosEGraficos();

    $.mobile.loading("show", {
        text: "Aguarde a geração do csv...",
        textVisible: true,
        theme: "a",
        html: ""
    });


    $.post(urlBackEnd + "api/ResultadoPorNivel/download-csv-dre-detalhando-escolas-consolidado?guid=" + newGuid(), objEnvio)
        .success(function (data) {
            var universalBOM = "\uFEFF";
            var blob = new Blob([universalBOM + data], { type: "text/csv;charset=UTF-8" });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ProficienciaDre.csv";
            link.click();

            $.mobile.loading("hide");
        })
        .fail(function (erro) {
            debugger;
            ProvaSP_Erro("Erro " + erro.status, erro.responseText);
        });
}

/***** Fim do download do CSV dos gráficos de radar e barras */