﻿"use strict";

var paginaDoPdf = 1;

function recuperarAnoEnturmacao(dataResultado) {
    try {
        var ano = parseInt($("#ddlResultadoAno").val()) - 1;
        return ano;
    }
    catch (error) {
        console.log(error);
    }
}

$('.link-exportar-pdf-detalhes').click(function () {
    paginaDoPdf = 1;
    gerarRelatorioEmPdf();
});

function gerarRelatorioEmPdf() {
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

    $.mobile.loading("show", {
        text: "Aguarde a geração do PDF...",
        textVisible: true,
        theme: "a",
        html: ""
    });

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

    $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), objEnvio)
        .done(function (dataResultado) {
            $.mobile.loading("hide");
            montarRelatorioEmPdf(
                ciclo,
                edicao,
                areaConhecimentoId,
                anoEscolar,
                nivel,
                "divResultadoApresentacao",
                dataResultado,
                objEnvio);
        })
        .fail(function (erro) {
            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
        });
}

function montarRelatorioEmPdf(ciclo, edicao, areaConhecimentoId, ano, nivel, divResultadoContainer, dataResultado, objetoEnviado) {
    var pdf = new jsPDF('p', 'pt', 'a4');
    montarRadarNoPdf(pdf);

    var labelsCiclos = { ciclo1: "Alfabetização", ciclo2: "Interdisciplinar", ciclo3: "Autoral" };

    for (var i = 0; i < dataResultado.Itens.length; i++) {
        if (dataResultado.Itens[i].Valor == -1) {
            dataResultado.Itens[i].Valor = "Profic. não calculada";
        }
    }

    var quantidadeDeBarrasPorPagina = (nivel == "TURMA" && edicao == "ENTURMACAO_ATUAL") ? 7 : 8;
    var quantidadeDeCanvas = Math.ceil(dataResultado.Itens.length / quantidadeDeBarrasPorPagina);

    var proficienciaMaxima = 500;
    var reguaProficiencia = getReguaProficiencia(areaConhecimentoId, proficienciaMaxima);

    if (edicao == "ENTURMACAO_ATUAL" && ciclo == "") {
        var anoAplicacaoProva = recuperarAnoEnturmacao(dataResultado);
        if (anoAplicacaoProva > 9)
            anoAplicacaoProva = 9;
    }


    var cores = getCores();
    var coresProfiencia = getCoresProficiencia(cores);
    var coresEnturmacao = getCoresEsturmacao(cores);

    var hashtableProficienciaId_cor = getHashtableProficienciaIdCor(coresProfiencia);
    var hashtableProficienciaId_enturmacao_cor = getHashtableProficienciaIdEnturmacaoCor(coresEnturmacao);
    var maiorTitulo = "";
    var filtroProficiencia = $(".resultado-filtro-proficiencia:checked").map(function () { return this.value; }).get();

    for (var contadorCanvas = 0; contadorCanvas < quantidadeDeCanvas; contadorCanvas++) {
        var escolas = dataResultado.Itens.slice(contadorCanvas * quantidadeDeBarrasPorPagina, quantidadeDeBarrasPorPagina * (contadorCanvas + 1));

        var dataChartPdf = getNovoObjetoDataChart();

        if (edicao == "ENTURMACAO_ATUAL") {
            dataChartPdf.datasets.push({ data: [], backgroundColor: [] });
        }

        debugger;
        for (var contadorEscola = 0; contadorEscola < escolas.length; contadorEscola++) {
            var item = escolas[contadorEscola];

            if (item.NivelProficienciaID == 0 || filtroProficiencia.indexOf(item.NivelProficienciaID.toString()) >= 0) {
                var tituloAtual = item.Titulo + ": " + item.Valor;
                if (tituloAtual.length > maiorTitulo.length) {
                    maiorTitulo = tituloAtual;
                }
                dataChartPdf.labels.push(item.Titulo + ": " + item.Valor);
                dataChartPdf.datasets[0].data.push(item.Valor);
                dataChartPdf.datasets[0].backgroundColor.push(hashtableProficienciaId_cor[item.NivelProficienciaID]);

                if (edicao == "ENTURMACAO_ATUAL")
                    dataChartPdf.datasets[1].data.push(item.Valor);

                var NivelProficienciaID_ENTURMACAO;
                if (ciclo == "") {
                    if (item.Valor < reguaProficiencia[ano][0])
                        NivelProficienciaID_ENTURMACAO = 1;
                    else if (item.Valor >= reguaProficiencia[ano][0] && item.Valor < reguaProficiencia[ano][1])
                        NivelProficienciaID_ENTURMACAO = 2;
                    else if (item.Valor >= reguaProficiencia[ano][1] && item.Valor < reguaProficiencia[ano][2])
                        NivelProficienciaID_ENTURMACAO = 3;
                    else if (item.Valor >= reguaProficiencia[ano][2])
                        NivelProficienciaID_ENTURMACAO = 4;
                    dataChartPdf.datasets[0].label = "Régua do " + anoAplicacaoProva + "º ano";

                }
                else {
                    if (item.Valor < reguaProficiencia["c" + ciclo][0])
                        NivelProficienciaID_ENTURMACAO = 1;
                    else if (item.Valor >= reguaProficiencia["c" + ciclo][0] && item.Valor < reguaProficiencia["c" + ciclo][1])
                        NivelProficienciaID_ENTURMACAO = 2;
                    else if (item.Valor >= reguaProficiencia["c" + ciclo][1] && item.Valor < reguaProficiencia["c" + ciclo][2])
                        NivelProficienciaID_ENTURMACAO = 3;
                    else if (item.Valor >= reguaProficiencia["c" + ciclo][2])
                        NivelProficienciaID_ENTURMACAO = 4;
                    dataChartPdf.datasets[0].label =
                        "Régua do ciclo de " + labelsCiclos["ciclo" + ciclo];
                }

                if (edicao == "ENTURMACAO_ATUAL") {
                    dataChartPdf.datasets[1].backgroundColor.push(hashtableProficienciaId_enturmacao_cor[NivelProficienciaID_ENTURMACAO]);
                    dataChartPdf.datasets[1].label = "Régua do " + ano + "º ano";
                }
            }
        }

        var canvasId = "chartExportarPdf" + contadorCanvas;
        var alturaDoCanvas = definirAlturaDoCanvas(escolas.length, edicao);
        var primeiraPagina = contadorCanvas == 0;
        criarChartEAdicionarNoPdf(pdf, canvasId, alturaDoCanvas, proficienciaMaxima, ciclo, ano, nivel, edicao, reguaProficiencia, primeiraPagina, labelsCiclos, dataChartPdf, anoAplicacaoProva);

        limparChartDoPdf();

    }

    pdf.save('Proficiencia.pdf');
}

function definirAlturaDoCanvas(quantidade, edicao) {
    if (quantidade == 1) {
        return quantidade * 85; //300;
    }
    else {
        if (edicao == "ENTURMACAO_ATUAL") {
            return quantidade * 60; //300;
        }
        else {
            return quantidade * 57; //300;
        }

    }
}

function getProficienciasAtuais() {
    return [
        { Nome: "Indefinido" },
        { Nome: "Abaixo do básico" },
        { Nome: "Básico" },
        { Nome: "Adequado" },
        { Nome: "Avançado" }
    ];
}

function getReguaProficiencia(areaConhecimentoId, proficienciaMaxima) {
    var reguaProficiencia = [];

    if (areaConhecimentoId == "1") { //Ciências
        reguaProficiencia["c1"] = [125, 175, 225]; //Básico
        reguaProficiencia["c2"] = [175, 225, 275]; //Interdisciplinas
        reguaProficiencia["c3"] = [210, 275, 325]; //Autoral

        reguaProficiencia["2"] = [100, 150, 200]; //2° Ano
        reguaProficiencia["3"] = [125, 175, 225]; //3° Ano
        reguaProficiencia["4"] = [150, 200, 250]; //4° Ano
        reguaProficiencia["5"] = [175, 225, 275]; //5° Ano
        reguaProficiencia["6"] = [190, 240, 290]; //6° Ano
        reguaProficiencia["7"] = [200, 250, 300]; //7° Ano
        reguaProficiencia["8"] = [210, 275, 325]; //8° Ano
        reguaProficiencia["9"] = [225, 300, 350]; //9° Ano
    }
    else if (areaConhecimentoId == "2") {//Língua Portuguesa
        reguaProficiencia["c1"] = [125, 175, 225]; //Básico
        reguaProficiencia["c2"] = [150, 200, 250]; //Interdisciplinas
        reguaProficiencia["c3"] = [185, 250, 300]; //Autoral

        reguaProficiencia["2"] = [100, 125, 175]; //2° Ano
        reguaProficiencia["3"] = [125, 175, 225]; //3° Ano
        reguaProficiencia["4"] = [135, 185, 235]; //4° Ano
        reguaProficiencia["5"] = [150, 200, 250]; //5° Ano
        reguaProficiencia["6"] = [165, 215, 265]; //6° Ano
        reguaProficiencia["7"] = [175, 225, 275]; //7° Ano
        reguaProficiencia["8"] = [185, 250, 300]; //8° Ano
        reguaProficiencia["9"] = [200, 275, 325]; //9° Ano
    }
    else if (areaConhecimentoId == "3") {//Matemática
        reguaProficiencia["c1"] = [150, 200, 250]; //Básico
        reguaProficiencia["c2"] = [175, 225, 275]; //Interdisciplinas
        reguaProficiencia["c3"] = [210, 275, 325]; //Autoral

        reguaProficiencia["2"] = [125, 175, 200]; //3° Ano
        reguaProficiencia["3"] = [150, 200, 250]; //3° Ano
        reguaProficiencia["4"] = [165, 210, 265]; //4° Ano
        reguaProficiencia["5"] = [175, 225, 275]; //5° Ano
        reguaProficiencia["6"] = [190, 240, 290]; //6° Ano
        reguaProficiencia["7"] = [200, 250, 300]; //7° Ano
        reguaProficiencia["8"] = [210, 275, 325]; //8° Ano
        reguaProficiencia["9"] = [225, 300, 350]; //9° Ano
    }
    else if (areaConhecimentoId == "4") {//Redação
        proficienciaMaxima = 100;
        reguaProficiencia["c1"] = [50, 65, 90]; //Básico
        reguaProficiencia["c2"] = [50, 65, 90]; //Interdisciplinas
        reguaProficiencia["c3"] = [50, 65, 90]; //Autoral

        reguaProficiencia["2"] = [50, 65, 90]; //2° Ano
        reguaProficiencia["3"] = [50, 65, 90]; //3° Ano
        reguaProficiencia["4"] = [50, 65, 90]; //4° Ano
        reguaProficiencia["5"] = [50, 65, 90]; //5° Ano
        reguaProficiencia["6"] = [50, 65, 90]; //6° Ano
        reguaProficiencia["7"] = [50, 65, 90]; //7° Ano
        reguaProficiencia["8"] = [50, 65, 90]; //8° Ano
        reguaProficiencia["9"] = [50, 65, 90]; //9° Ano
    }

    return reguaProficiencia;
}

function getCores() {
    return {
        corNivelAbaixoDoBasico_ref: "rgba(255,0,0,alpha)",
        corNivelBasico_ref: "rgba(253,173,0,alpha)",
        corNivelAdequado_ref: "rgba(0,0,255,alpha)",
        corNivelAvancado_ref: "rgba(0,255,0,alpha)"
    };
}

function getCoresProficiencia(cores) {
    return {
        corNivelAbaixoDoBasico_ref: cores.corNivelAbaixoDoBasico_ref.replace("alpha", "0.55"),
        corNivelBasico_ref: cores.corNivelBasico_ref.replace("alpha", "0.3"),
        corNivelAdequado_ref: cores.corNivelAdequado_ref.replace("alpha", "0.3"),
        corNivelAvancado_ref: cores.corNivelAvancado_ref.replace("alpha", "0.3")
    };
}

function getCoresEsturmacao(cores) {
    return {
        corNivelAbaixoDoBasico_enturmacao: cores.corNivelAbaixoDoBasico_ref.replace("alpha", "0.55"),
        corNivelBasico_enturmacao: cores.corNivelBasico_ref.replace("alpha", "0.3"),
        corNivelAdequado_enturmacao: cores.corNivelAdequado_ref.replace("alpha", "0.3"),
        corNivelAvancado_enturmacao: cores.corNivelAvancado_ref.replace("alpha", "0.3")
    };
}

function getHashtableProficienciaIdCor(coresProfiencia) {
    return { 1: coresProfiencia.corNivelAbaixoDoBasico_ref, 2: coresProfiencia.corNivelBasico_ref, 3: coresProfiencia.corNivelAdequado_ref, 4: coresProfiencia.corNivelAvancado_ref };
}

function getHashtableProficienciaIdEnturmacaoCor(coresEnturmacao) {
    return { 1: coresEnturmacao.corNivelAbaixoDoBasico_enturmacao, 2: coresEnturmacao.corNivelBasico_enturmacao, 3: coresEnturmacao.corNivelAdequado_enturmacao, 4: coresEnturmacao.corNivelAvancado_enturmacao };
}

function getTitulosDeNivel(proficienciasAtuais) {
    return [
        "",
        proficienciasAtuais[1].Nome,
        proficienciasAtuais[2].Nome,
        proficienciasAtuais[3].Nome,
        proficienciasAtuais[4].Nome
    ];
}

function getNovoObjetoDataChart() {
    return {
        labels: [],
        datasets: [
            {
                data: [],
                backgroundColor: []
            }
        ]
    };
}

function adicionarGraficoDeBarrasAoPdf(pdf, canvasElement, primeiraPagina, edicao, nivel) {
    paginaDoPdf++;
    pdf.addPage();
    var canvasPosition = 15;
    if (primeiraPagina) {
        
        if(nivel == "TURMA") {
            pdf.setFontType("bold");
            pdf.setFontSize(12);
            pdf.setTextColor(66, 142, 218);
            pdf.text(150, edicao == "ENTURMACAO_ATUAL" ? 70 : 50, "Abaixo segue o detalhamento de proficiência de cada Aluno.");
            pdf.setTextColor(0, 0, 0);
            pdf.setFontType("normal");
            pdf.setFontSize(16);
        }else
        {
            pdf.fromHTML($('#divResultadoTituloDetalhe')[0], 125, canvasPosition);
        }
    }

    var canvasProficiencia = document.getElementById('chartResultadoEscalaSaeb_1');
    var imgData = canvasProficiencia.toDataURL("image/png", 1.0);
    canvasPosition = 60;
    pdf.addImage(imgData, 'PNG', 92, canvasPosition, canvasProficiencia.width * 0.75, canvasProficiencia.height * 0.75, '', 'FAST');

    canvasPosition = 140;
    pdf.addImage(canvasElement.toDataURL("image/png", 1.0), 'PNG', 92, canvasPosition, canvasElement.width * 0.75, canvasElement.height * 0.75, '', 'FAST');

    pdf.setFontSize(9);
    pdf.setFontType("normal")
    pdf.text(500, 800, "Página " + paginaDoPdf);
}

function adicionarGraficoDeBarrasAoPdfDaTurmaDetalhandoAlunosComEnturmacao(pdf, canvasElement, primeiraPagina, edicao, nivel) {
    
    var canvasPosition = 0;
    if(!primeiraPagina)
    {
        paginaDoPdf++;
        pdf.addPage();
    }

    if (primeiraPagina && nivel == "TURMA" && edicao == "ENTURMACAO_ATUAL") {
        pdf.setFontType("bold");
        pdf.setFontSize(12);
        pdf.setTextColor(66, 142, 218);
        pdf.text(150, 70, "Abaixo segue o detalhamento de proficiência de cada Aluno.");
        pdf.setTextColor(0, 0, 0);
        pdf.setFontType("normal");
        pdf.setFontSize(16);
    }

    var canvasProficiencia = document.getElementById('chartResultadoEscalaSaeb_1');
    var imgData = canvasProficiencia.toDataURL("image/png", 1.0);
    canvasPosition = primeiraPagina ? 80 : 60;
    
    pdf.addImage(imgData, 'PNG', 92, canvasPosition, canvasProficiencia.width * 0.75, canvasProficiencia.height * 0.75, '', 'FAST');

    canvasPosition = primeiraPagina ? 160 : 140;
    pdf.addImage(canvasElement.toDataURL("image/png", 1.0), 'PNG', 92, canvasPosition, canvasElement.width * 0.75, canvasElement.height * 0.75, '', 'FAST');

    pdf.setFontSize(9);
    pdf.setFontType("normal")
    pdf.text(500, 800, "Página " + paginaDoPdf);
}

function montarRadarNoPdf(pdf) {   
    pdf.setFontType("bold");
    pdf.setFontSize(20);
    pdf.text(190, 30, $('#lblResultadoTitulo').text());
    pdf.setFontSize(15); 
    pdf.text(150, 48, $('#lblResultadoEdicao').text() + " - " + $('#lblResultadoAreaConhecimento').text() + " - " +  $('#lblResultadoAno').text());
    pdf.setFontType("normal");
    pdf.setFontSize(16);

    var listaDeAgregacao = [];

    $("#divChartResultadoAgregacao").find("canvas").each(function () {
        listaDeAgregacao.push($(this).attr('id'));
    });

    let quantidade = 0;
    let canvasPosition = 65;
    let quantidadeDeRadares = listaDeAgregacao.length;
    listaDeAgregacao.forEach(agregacaoId => {
        let alinhamentoRadar = 80;
        let tamanhoDoCanvas = 0.75;

        if(quantidadeDeRadares == 1)
        {
            alinhamentoRadar = 30;
            tamanhoDoCanvas = 0.85;
        }

        if (quantidade > 0) {
            canvasPosition += 250;

            if ((quantidade) % 3 == 0) {
                paginaDoPdf++;
                pdf.addPage();
                canvasPosition = 25;
            }
        }

        var canvas = document.getElementById(agregacaoId);
        const canvasChartRadar = document.createElement("canvas");
        canvasChartRadar.width = 680;
        canvasChartRadar.height = 300;
        const ctx = canvasChartRadar.getContext("2d");
        ctx.drawImage(canvas, alinhamentoRadar, 0);

        pdf.addImage(canvasChartRadar.toDataURL("image/png", 1.0), 'PNG', 0, canvasPosition, canvasChartRadar.width * tamanhoDoCanvas, canvasChartRadar.height * tamanhoDoCanvas, '', 'FAST');

        pdf.setFontSize(9);
        pdf.setFontType("normal")
        pdf.text(500, 800, "Página " + paginaDoPdf);

        quantidade++;
    });
}

function criarCanvasDoChart(canvasId, alturaDoCanvas) {
    var elementDom = document.getElementById("divChartExportarPdf");
    var canvasElement = document.createElement('canvas');
    elementDom.appendChild(canvasElement);
    canvasElement.id = canvasId;
    canvasElement.height = alturaDoCanvas;
    return canvasElement;
}

function criarChartEAdicionarNoPdf(pdf, canvasId, alturaDoCanvas, proficienciaMaxima, ciclo, ano, nivel, edicao, reguaProficiencia, primeiraPagina, labelsCiclos, dataChartPdf, anoAplicacaoProva) {

    var canvasElement = criarCanvasDoChart(canvasId, alturaDoCanvas);
    var chartResultadoDetalhePdf_ctx = canvasElement.getContext("2d");

    var proficienciasAtuais = getProficienciasAtuais();
    var tituloNivel = getTitulosDeNivel(proficienciasAtuais);

    new Chart(chartResultadoDetalhePdf_ctx, {
        type: 'horizontalBar',
        data: dataChartPdf,
        options: {
            onClick: function (event, chartItem) {
                try {
                    if (chartItem.length > 0) {
                        var labelAtual = chartItem[0]._view.label;
                        var labelSplitArray = labelAtual.split("(");
                        var alunoID =
                            parseInt(labelSplitArray[labelSplitArray.length - 1].split(")")[0]);

                        if (!isNaN(alunoID)) {
                            var E = $("#ddlResultadoEdicao").val(); //Edicação
                            var AC = $("#ddlResultadoAreaConhecimento").val();// Área de Conhecimento

                            baixarProvaAlunoPorAno(false, AC + '_' + alunoID + '_' + E);
                        }
                    }
                }
                catch (error) {
                    console.log(error);
                }
            },
            maintainAspectRatio: true,
            legend: {
                display: false
            },
            animation: {
                duration: 0,
                responsiveAnimationDuration: 0,
                onComplete: function () { 
                    if(nivel == "TURMA" && edicao == "ENTURMACAO_ATUAL")
                    {
                        adicionarGraficoDeBarrasAoPdfDaTurmaDetalhandoAlunosComEnturmacao(pdf, canvasElement, primeiraPagina, edicao, nivel);
                    }
                    else{
                        adicionarGraficoDeBarrasAoPdf(pdf, canvasElement, primeiraPagina, edicao, nivel);
                    }
                }
            },
            showAllTooltips: true,
            tooltips: {
                enabled: true,
                backgroundColor: "rgba(100,100,100,1)",
                callbacks: {
                    title: function () {
                        return "";
                    },
                    label: function (tooltipItem, data) {
                        var anoRef = 0;
                        var valorProficiencia = data['datasets'][0]['data'][tooltipItem['index']];

                        if (ciclo == "") {
                            anoRef = ano;
                            if (tooltipItem.datasetIndex == 0 && edicao == "ENTURMACAO_ATUAL") {
                                anoRef = anoAplicacaoProva;
                            }
                        }
                        else {
                            anoRef = "c" + ciclo;
                        }

                        var NivelProficienciaID_ENTURMACAO = 0;
                        if (valorProficiencia < reguaProficiencia[anoRef][0])
                            NivelProficienciaID_ENTURMACAO = 1;
                        else if (valorProficiencia >= reguaProficiencia[anoRef][0] && valorProficiencia < reguaProficiencia[anoRef][1])
                            NivelProficienciaID_ENTURMACAO = 2;
                        else if (valorProficiencia >= reguaProficiencia[anoRef][1] && valorProficiencia < reguaProficiencia[anoRef][2])
                            NivelProficienciaID_ENTURMACAO = 3;
                        else if (valorProficiencia >= reguaProficiencia[anoRef][2])
                            NivelProficienciaID_ENTURMACAO = 4;

                        if (ciclo == "") {
                            return "Régua do " + anoRef + "º ano: " + tituloNivel[NivelProficienciaID_ENTURMACAO];
                        }
                        else {
                            return "Régua do ciclo de " + labelsCiclos["ciclo" + ciclo];
                        }
                    }
                },
            },
            scales: {
                yAxes: [{
                    categoryPercentage: 0.6,
                    barPercentage: 0.6,
                    ticks: {
                        mirror: true,
                        labelOffset: 1,
                        fontStyle: "bold",
                        labelOffset: (edicao == "ENTURMACAO_ATUAL") ? -19.26 * 2 : - 33.48,
                    }
                }],
                xAxes: [{
                    ticks: {
                        beginAtZero: false,
                        fontFamily: "'Open Sans Bold', sans-serif",
                        fontSize: 11,
                        min: 0,
                        max: proficienciaMaxima,
                        stepSize: 50,
                        padding: 20,
                    },
                    stacked: false
                }]
            }
        }
    });
}

function limparChartDoPdf() {
    $("#divChartExportarPdf").empty();
}