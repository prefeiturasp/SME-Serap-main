"use strict";

$('.link-exportar-imagem-detalhes').click(function () {
    
    $.mobile.loading("show", {
        text: "Aguarde a geração da imagem...",
        textVisible: true,
        theme: "a",
        html: ""
    });


    let imageFormat = this.dataset.imageFormat;
    const imagemTituloDeApresentacao = gerarImagemDivTituloDeApresentacao();
    const imagemTituloDeApresentacaoFiltro = gerarImagemDivTituloDeApresentacaoFiltro();
    const imagemResultadoTituloDetalhe = gerarImagemDivResultadoTituloDetalhe();
    Promise.all([imagemTituloDeApresentacao, imagemTituloDeApresentacaoFiltro, imagemResultadoTituloDetalhe]).then(imagens => exportarImagem(imageFormat, imagens));
});

function gerarImagemDivTituloDeApresentacao() {
    return new Promise((resolve) => {
        domtoimage.toPng(document.getElementById('divResultadoApresentacaoTitulo'))
            .then(function (dataURL) {
                let imagemDivResultadoTituloDetalhe = new Image();
                imagemDivResultadoTituloDetalhe.onload = () => { resolve(imagemDivResultadoTituloDetalhe) };
                imagemDivResultadoTituloDetalhe.src = dataURL;
            });
    });
}

function gerarImagemDivTituloDeApresentacaoFiltro() {
    return new Promise((resolve) => {
        domtoimage.toPng(document.getElementById('lblResultadoSubTitulo'))
            .then(function (dataURL) {
                let imagemDivResultadoTituloDetalhe = new Image();
                imagemDivResultadoTituloDetalhe.onload = () => { resolve(imagemDivResultadoTituloDetalhe) };
                imagemDivResultadoTituloDetalhe.src = dataURL;
            });
    });
}

function gerarImagemDivResultadoTituloDetalhe() {
    return new Promise((resolve) => {
        domtoimage.toPng(document.getElementById('divResultadoTituloDetalhe'))
            .then(function (dataURL) {
                let imagemDivResultadoTituloDetalhe = new Image();
                imagemDivResultadoTituloDetalhe.onload = () => { resolve(imagemDivResultadoTituloDetalhe) };
                imagemDivResultadoTituloDetalhe.src = dataURL;
            });
    });
}

function exportarImagem(extensao, imagens) {
    let imgTituloDeApresentacao = imagens[0];
    let imgTituloDeApresentacaoFiltro = imagens[1];
    let imgTituloDetalhe = imagens[2];

    let canvasExport = document.createElement('canvas');
    let chartEscala = document.getElementById('chartResultadoEscalaSaeb_1');
    let chartResultado = document.getElementById("chartResultadoDetalhe");

    let widthRadar = 0;
    let heightRadar = 0;
    let radares = [];
    $("#divChartResultadoAgregacao").find("canvas").each(function () {
        var nomeDiv = $(this).attr('id');
        radares.push(nomeDiv);

        var canvas = document.getElementById(nomeDiv);
        widthRadar = canvas.width > widthRadar ? canvas.width : widthRadar;
        heightRadar += canvas.height + 20;

    });

    canvasExport.height = calcularCanvasHeight(heightRadar, imgTituloDeApresentacao, imgTituloDetalhe, chartEscala, chartResultado, radares);
    canvasExport.width = calcularCanvasWidth(widthRadar, chartEscala, chartResultado)

    let contextCanvasExport = canvasExport.getContext("2d");

    setFundoBrancoNoCanvas(contextCanvasExport, canvasExport);
    
    contextCanvasExport.drawImage(imgTituloDeApresentacao, 5, 20);
    contextCanvasExport.drawImage(imgTituloDeApresentacaoFiltro, 11, 40);

    let heightExportCanvas = 70;

    radares.forEach(radarChart => {
        let canvas = document.getElementById(radarChart);
        contextCanvasExport.drawImage(canvas, -25, heightExportCanvas);
        heightExportCanvas += canvas.height + 20;
    });
    
    contextCanvasExport.drawImage(imgTituloDetalhe, 30, heightExportCanvas);

    heightExportCanvas += imgTituloDetalhe.height + 10;
    contextCanvasExport.drawImage(chartEscala, 20, heightExportCanvas);
    heightExportCanvas += chartEscala.height + 10;
    contextCanvasExport.drawImage(chartResultado, 25, heightExportCanvas);

    let link = document.createElement('a');
    link.download = "Proficiencia." + extensao;
    link.href = canvasExport.toDataURL("image/" + extensao);
    link.click();

    $.mobile.loading("hide");
}

function calcularCanvasHeight(heightRadar, imgTituloDeApresentacao, imgTituloDetalhe, chartEscala, chartResultado, listaDeRadar)
{
    return heightRadar + imgTituloDeApresentacao.height + imgTituloDetalhe.height + chartEscala.height + chartResultado.height + 95 + listaDeRadar.length;
}

function calcularCanvasWidth(widthRadar, chartEscala, chartResultado)
{
    widthRadar = widthRadar > chartEscala.width ? widthRadar : chartEscala.width;
    widthRadar = widthRadar > chartResultado.width ? widthRadar : chartResultado.width;
    return widthRadar;
}

function setFundoBrancoNoCanvas(contextCanvasExport, canvasExport)
{
    contextCanvasExport.fillStyle = "white";
    contextCanvasExport.fillRect(0, 0, canvasExport.width, canvasExport.height);
}