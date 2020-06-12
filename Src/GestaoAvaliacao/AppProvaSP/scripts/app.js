// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in cordova-simulate or on Android devices/emulators: launch your app, set breakpoints,
// and then run "window.location.reload()" in the JavaScript Console.
//debugger;
"use strict";

var db = null;
var mobile = false;
var opcaoResultadoSelecionada = -1;
var opcaoConfiguracoesSelecionada = -1;

/**
-----MSTECH-----
 *Novas variáveis globais. A primeira armazena as configurações do App. A segunda
 a definição dos ciclos de aprendizagem
*/
var provaAlunoCicloSelecionado = -1;
var provaSP_configuracoes = {
    dadosAtuais: {},
    configuracoes: {
        DisponibilizarPreenchimentoQuestionariosFichas: false,
        RelatorioAcompanhamentoVisivel: false,
        UrlImagemAlunos: "",
        RepresentatividadeSegundoINEP: "",
        PossuiPerfilEdicaoAtual: false
    }
};
var areasConhecimento = ["Ciências da Natureza", "Língua Portuguesa", "Matemática", "Redação"];
var cicloTotalAlunos = {};
var modeloCiclos = { Ciclo1: [-1, 2, 3], Ciclo2: [4, 5, 6], Ciclo3: [7, 8, 9] };
//MSTECH - Objeto para salvar os dados de agração de Série Histórica - Ano atual e Ano anterior
var serieHistorica = { anoAtual: {}, anoAnterior: {} };
/**
-----MSTECH-----
 *Flag para identificar notificações locais (quando dados precisam ser sincronizados)
*/
var notificacaoSincroniaAtivada = false;
/**
-----MSTECH-----
 *Objeto para armazenar os IDs e a Descrição dos contructos e suas variáveis
*/
//var graficosVariaveis = false;
var variaveisConstructo = {};
/**
-----MSTECH-----
 *Cache das informações de corte - Em Configuraçõesc
*/
var corteCache = [];

/**
-----MSTECH-----
 *Questionários por tipo de usuário

 OBS: Perceba que professor não tem Ficha de Registro

 Questionários: Perguntas pessoais e administrativas para o usuário;
 Questionário 8: Funcionalidades de acompanhamento da ProvaSP no dia da mesma;
 Fichas de registro: Perguntas aos diretores e responsáveis sobre andamento técnico da ProvaSP
*/
var edicoesComTurmasAmostrais = ["2017", "2018", "2019"];
var edicoesRevistasPedagogicas = ["2018", "2019"]; // ["2017"] é Boletim
var questionarios = [
    "1",/*Questionário Supervisor*/
    //"2",/*Questionário Diretor 2018*/
    "3",/*Questionário Coordenador*/
    "8",/*Controle da Prova a ser aplicada*/
    "9",/*Ficha de registro Supervisor*/
    "10",/*Ficha de registro Diretor*/
    "11",/*Ficha de registro Coordenador*/
    //"12",/*Questionário Professor 2018*/
    //"13",/*Questionário Assistente de Diretoria 2018*/
    "14",/*Questionário do Auxiliar Técnico da Educação*/
    "15",/*Questionário do Agente Escolar: Merendeira*/
    "16",/*Questionário do Agente Escolar: Portaria*/
    "17",/*Questionário do Agente Escolar: Zeladoria*/
    //"18",/*Questionário dos Alunos do 3º ano 2018*/
    //"19",/*Questionário dos Alunos do 4º ao 6º ano 2018*/
    //"20",/*Questionário dos Alunos do 7º ao 9º ano ID 2018*/
    "21",/*NOVO Questionário dos Alunos do 3º ao 6º ano 2019*/
    "22",/*NOVO Questionário dos Alunos do 7º ao 9º ano 2019*/
    "23",/*NOVO Questionário Professor 2019*/
    "24",/*NOVO Questionário Diretor 2019*/
    "25",/*NOVO Questionário Assistente de Diretoria 2019*/
];
/**
-----MSTECH-----
 *Comporta o ID do questionário selecionado. Tudo indica que o vetor qustionario é modificado para
 comportar também o código da escola. Este trecho está faltando

 Verificar se existe um código atualizado que complementa os IDs dos questionarios. Apenas os índices
 do vetor inicial não funcionarão corretamente.
 RESPONDIDO: A identificação dos questionários era diferente na versão anterior do App (publicada na
 Play Store). Esta, portanto, é a versão atualizada e deve-se implementar os trechos que identificavam
 os questionários por uma string com base no vetor de índices de questionários.
*/
var questionarioId_atual = "";
/**
-----MSTECH-----
 *É preciso manter o código da turma numa variável global por haver a possibilidade dele estar
 associado a um código diferente, impresso nas provas.
*/
var codigoTurma_atual = "";
/**
-----MSTECH-----
 *Vetor que comporta os elementos DOM, geralmente botões, que têm o evento para voltar à uma funcionalidade.
 *Ou seja, ao disparar o método de volta, o App dispara em seguida o evento do botão armazenado.
*/
var caminhoBackButton = null;
/**
-----MSTECH-----
 *Esta variável comportará o vetor de registros das escolas, obtidos através do arquivo escolas.CSV
*/
var dataEscola = [];
/**
-----MSTECH-----
 *Armazena os índices dos questionários carregados.
*/
var questionarioCarregado = {};
/**
-----MSTECH-----
 *Vetor com todos os questionários ainda não enviados. Tais questionários são salvos no banco de dados
 local SQLite.
*/
var listaQuestionariosNaoEnviados;

//https://stackoverflow.com/questions/8068052/phonegap-detect-if-running-on-desktop-browser
//if (navigator.userAgent.match(/(iPhone|iPod|iPad|Android|BlackBerry|IEMobile)/) || typeof cordova != "undefined") {
if (typeof cordova !== "undefined") {
    // PhoneGap application
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    mobile = true;
}
else {
    // Web page
    onDeviceReady();
}

/**
-----MSTECH-----
 *Módulo 1 - Início
 *Este módulo determina as primeiras execuções do App quando o usuário loga com sucesso.
 *São feitas ações como:
 -Montar interface com base no tipo de usuário;
 -Baixar informações sobre fase atual da edição do ProvaSP;
 -Veiricar dados não sincronizados (notificações locais).
*/

/**
-----MSTECH-----
 *Primeiro método executado depois do login
*/
function onDeviceReady() {
    try {
        //INÍCIO

        /**
        -----MSTECH-----
         *Recarrega o método inicial assíncrono se o Jquery não for carregado a tempo
        */
        if (typeof window.jQuery == "undefined") {
            //As vezes onDeviceReady dispara antes da carga do jQuery...
            setTimeout(onDeviceReady(), 1000);
            return;
        }

        /**
        -----MSTECH-----
         *Primeiro método executado depois do login
        */
        var apresentarLoading = true;

        if (!mobile) {
            var url = window.location.href.toLocaleLowerCase().split("provasp")[0].replace("serap.sme", "provasp.sme");
            if (url.indexOf("file") == -1) {
                if (window.location.href.indexOf('54127') >= 0)
                    urlBackEnd = url.replace("54127", "52912");//url DEV
                else
                    urlBackEnd = window.location.href.toLocaleLowerCase().split("provasp")[0].replace("serap.sme", "provasp.sme");
            }
        }
        else {
            /**
            -----MSTECH-----
             *Sendo mobile e sem conexão, não necessita mostrar Loading
            */
            if (!(navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN)) {
                apresentarLoading = false;
            }
        }

        /**
        -----MSTECH-----
         *Loading enquanto carrega informações do servidor ProvaSP
        */
        if (apresentarLoading) {
            $.mobile.loading('show');
        }
        else {
            /**
            -----MSTECH-----
             *Comportamento padrão Offline - Apenas Mobile APP.
             *Como não há conexão, não tem que fazer requisições ao server, mostra a tela de Menu diretamente
            */
            $("#aguarde-page").hide();
            $("#menu-page").show();
        }

        /**
        -----MSTECH-----
         *Primeira requisição do App - Verifica a condição atual do ProvaSP. De acordo com as flags retornadas,
         o App se molda de tal maneira a mostrar apenas as funcionalidades necessárias.
         *EX: Na ocasião da análise do código de 2017, apenas a verificação de resultados é possível
         -RelatorioAcompanhamentoVisivel: Verifica se existe a possibilidade de utilizar o Relatório de
         acompanhamento do ProvaSP (apenas no dia da Prova)
         -DisponibilizarPreenchimentoQuestionariosFichas: indica a possibilidade de preencher questionários

         *Basicamente esta primeira requisição determina a visibilidade de dois botões do App, referentes às
         funcionalidades controladas pelas Flags
        */
        Usuario = ObterUsuario();

        $.ajax({
            url: urlBackEnd + "api/RetornarAppJson",
            data: { edicao: String(new Date().getFullYear()), usu_id: Usuario.usu_id },
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            cache: false,
            success: function (data) {
                //MSTECH - Atribuindo novos valores vindos do servidor ao objeto de configurações
                provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas =
                    (data.DisponibilizarPreenchimentoQuestionariosFichas === 'true');

                provaSP_configuracoes.configuracoes.PossuiPerfilEdicaoAtual =
                    (data.PossuiPerfilEdicao === 'True');

                provaSP_configuracoes.configuracoes.RelatorioAcompanhamentoVisivel =
                    (data.RelatorioAcompanhamentoVisivel === 'true');

                provaSP_configuracoes.configuracoes.UrlImagemAlunos =
                    data.UrlImagemAlunos;

                provaSP_configuracoes.configuracoes.RepresentatividadeSegundoINEP =
                    data.RepresentatividadeSegundoINEP;

                /**
                -----MSTECH-----
                 *Sendo o usuário um aluno, ele terá acesso apenas ao seu resultado de prova, quando disponível.
                 *Sendo assim, os valores das flags são irrelevantes para este tipo de usuário

                 *Quando não está mais em tempo de preenchimento de questionários, permitir apenas
                 que os alunos vejam o resultado da ProvaSP.
                */
                if (Usuario.Aluno) {
                    if (!provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas) {
                        resultadoAlunoConfigurarInterface();
                        return;
                    }
                }

                /**
                -----MSTECH-----
                 *A funcionalidade de Relatório de Acompanhamento não deve ser mostrada aos professores.
                 *Ele só deve estar disponível quando a flag RelatorioAcompanhamentoVisivel for TRUE
                 *Na versão atual, tal relatório é mostrado de acordo com o ID do usuário.
                */
                //O usuário tem acesso apenas ao questionário 23 (Professor). Ele não deve acessar o relatório de acompanhamento.
                if (provaSP_configuracoes.configuracoes.RelatorioAcompanhamentoVisivel &&
                    (Usuario.AcessoNivelDRE || Usuario.AcessoNivelSME || Usuario.Diretor || Usuario.AssistenteDeDiretoria || Usuario.Supervisor)) {
                    $("#btnAbrirRelatorioAcompanhamento").show();
                }
                else {
                    $("#btnAbrirRelatorioAcompanhamento").hide();
                    // Desabilitar temporariamente a ficha de registro dos professores 22/10/19
                    $("#div_fichasderegistro").hide();
                }

                /**
                -----AMCOM-----
                 *Botão que determina o preenchimento de questionários
                 *Os questionários estarem disponíveis ou não depende da flag vinda do servidor ProvaSP
                 *Se estiver fora do período de preenchimento de questionários,
                 o ProvaSP mostra a tela de resultados. Dependendo do tipo de usuário,
                 é necessário filtrar a informação.
                 *É importante destacar que esta permissão  DisponibilizarPreenchimentoQuestionariosFichas
                 sendo falsa, a permissão RelatorioAcompanhamentoVisivel torna-se irrelevante
                */
                if (Usuario.AcessoNivelSME) {
                    /**
                    -----AMCOM-----
                      *Agora é possível acessar o menu independentemente do estado da ProvaSP para usuários
                      com nível SME. Isso se dá pelo fato de existir uma nova tela de configuração acessível
                      pelo menu principal.
                      *Para tal, escondemos as opções de acesso às funcionalidades da ProvaSP e disponibilizamos
                      apenas acesso ao resultado (padrão) e à tela de configuração.
                    */
                    if (provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas) {
                        $("#provaSP_disponivel").show();
                    } else {
                        $("#provaSP_disponivel").hide();
                    }
                    $("#provaSP_resultados").show();

                    $("#aguarde-page").hide();
                    $("#btnConfiguracoes").show();
                    $("#menu-page").show();

                    $.mobile.loading('hide');
                    $("#divMenuPrincipal").show();
                } else {
                    //quando não tem perfil, não vai responder nenhum questionário
                    if (!provaSP_configuracoes.configuracoes.PossuiPerfilEdicaoAtual)
                        $("button-abrir-questionario").hide();

                    //Os perfis de Supervisor/Diretor/Coordernador sempre podem ver a prova pq nela tem o Painel a Acompanhamento
                    var possuiPermissaoAcessoProvaResultado = provaSP_configuracoes.configuracoes.PossuiPerfilEdicaoAtual
                        || Usuario.Supervisor
                        || Usuario.Diretor
                        || Usuario.Coordenador;

                    if (provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas && possuiPermissaoAcessoProvaResultado) {
                        $("#aguarde-page").hide();
                        $("#menu-page").show();

                        $.mobile.loading('hide');
                        $("#divMenuPrincipal").show();

                        /**
                        -----AMCOM-----
                        Os resultados das provas estarão disponíveis inclusive durante uma prova ativa.
                        Assim será possível continuar analisando os resultados antigos durante a prova atual.
                        */
                        $("#provaSP_resultados").show();
                    } else {
                        abrirResultados(true);
                    }
                    $("#btnConfiguracoes").hide();
                }
            },
            error: function (erro) {
                $.mobile.loading("hide");

                /**
                -----MSTECH-----
                 *Quando não há conexão, o App deve funcionar em modo OFFLINE
                */
                if (erro.status == 0) {
                    $("#aguarde-page").hide();
                    $("#menu-page").show();
                    $("#divMenuPrincipal").show();
                    ProvaSP_Erro("Erro " + erro.status, "Sem conexão");
                }
                else { ProvaSP_Erro("Erro " + erro.status, erro.statusText); }
            }
        });


        //$.getJSON(urlBackEnd + "api/RetornarAppJson?callback=?",
        //    function (data)
        //    {
        //        if (Usuario.Aluno)
        //        {
        //            resultadoAlunoConfigurarInterface();
        //            return;
        //        }

        //        if (data.RelatorioAcompanhamentoVisivel && Usuario.questionarios != "23") //O usuário tem acesso apenas ao questionário 23 (Professor). Ele não deve acessar o relatório de acompanhamento.
        //        {
        //            $("#btnAbrirRelatorioAcompanhamento").show();
        //        }
        //        else
        //        {
        //            $("#btnAbrirRelatorioAcompanhamento").hide();
        //        }

        //        if (data.DisponibilizarPreenchimentoQuestionariosFichas)
        //        {
        //            $("#aguarde-page").hide();
        //            $("#menu-page").show();

        //            $.mobile.loading('hide');
        //            $("#divMenuPrincipal").show();
        //        }
        //        else
        //        {
        //            abrirResultados(true);
        //        }
        //    })
        //    .fail(function (erro)
        //    {
        //        swal("Erro " + erro.status, erro.statusText, "error");
        //        $.mobile.loading("hide");
        //    });

        /**
        -----MSTECH-----
         *Método não utilizado

         Método realmente não utilizado?
         RESPONDIDO: Realmente o método não é utilizado nem tampouco tem um equivalente na versão Web em C#.
         Manteremos para fins de registro.
        */
        //function resultadoAlunoApresentar(dataResultado) {

        //}

        /**
        -----MSTECH-----
         *Aqui foi utilizado um truque do JQuery Mobile para evitar que, ao começar uma sincronização
         o usuário possa tocar de novo na interface para iniciar uma segunda sync em paralelo.
         *De fato, é uma forma de bloquear a interface enquanto a requisição é processada.
        */
        $(document).on("mobileinit", function () {
            $.mobile.loader.prototype.options.text = "loading...";
            $.mobile.loader.prototype.options.textVisible = true;
            $.mobile.loader.prototype.options.theme = "a";
            $.mobile.loader.prototype.options.html = "";

            //Thanks: https://github.com/jquery/jquery-mobile/issues/3414
            $.mobile.loader.prototype.defaultHtml = "<div class='ui-loader'>" +
                "<span class='ui-icon ui-icon-loading'></span>" +
                "<h1></h1>" +
                "<div class='ui-loader-curtain'></div>" +
                "</div>",
                $(document).ajaxStart(function () {
                    $.mobile.loading('show');
                })
            $(document).ajaxStop(function () {
                $.mobile.loading('hide');
            });
        });

        /**
        -----MSTECH-----
         *Método absurdamente extenso que determina as ações de todos os elementos do App.
         *O método é muito grande por conter, além dos handlers dos eventos, as implementações
         dos mesmos.

         OBS: Análise mais detalhada na implementação do evento.
        */
        definirEventHandlers();

        /**
        -----MSTECH-----
         *Implementação sem sentido.
         *Aparentemente quando é Web e o App é aberto num dispositivo mobile, utilizar o Usuário salvo em
         LocalStorage.
         *Se for o caso de um browser, usa o método ConfigurarUsuarioSerap abaixo. Mas tal método não tem
         implementação concluída no global.js

         OBS: Reparar que não existe abertura do SQLite no modo Web

         ConfigurarUsuarioSerap é mais completo em outro repositório atualizado?
         RESPONDIDO: Não há mais necessidade de utilizar o método ConfigurarUsuarioSerap da maneira original
         concebida. Ele era utilizado na versão anterior do App (publicada na GooglePlay).
        */
        if (mobile) {
            Usuario = JSON.parse(localStorage.getItem("Usuario"));

            /**
            -----MSTECH-----
             *No caso do mobile App, verifica as notificações locais pendentes. Se houver, muda flag para
             true.
            */
            cordova.plugins.notification.local.getScheduledIds(function (scheduledIds) {
                notificacaoSincroniaAtivada = (scheduledIds.length > 0);
            });

            /**
            -----MSTECH-----
             *Este trecho cria um banco de dados simples SQLite para armazenar as informações de questionários
             ou mesmo dos detalhes da prova aplicada quando o usuário não tem conexão com a internet.
             *Sendo assim, mesmo sem conexão, as informações ficam salvas e serão sincronizadas posteriormente.
            */
            try {
                db = window.sqlitePlugin.openDatabase(
                    { name: "base.sqlite", location: 'default' },
                    function () {
                        //sucesso
                        console.log("db aberto.");

                        db.sqlBatch([
                            /*
                            "DROP TABLE IF EXISTS QuestionarioUsuario",
                            "DROP TABLE IF EXISTS QuestionarioRespostaItem",
                            */
                            "CREATE TABLE IF NOT EXISTS QuestionarioUsuario (QuestionarioUsuarioID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioID INTEGER, Guid TEXT, esc_codigo TEXT, tur_id INTEGER, usu_id TEXT, Enviado INTEGER, DataPreenchimento TEXT)",
                            "CREATE TABLE IF NOT EXISTS QuestionarioRespostaItem (QuestionarioRespostaItemID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioUsuarioID INTEGER, Numero TEXT, Valor TEXT);"
                        ], function () {
                            //sucesso
                            console.log("db - tabelas criadas.");
                            /**
                            -----MSTECH-----
                             *Tenta enviar informações pendentes com base na flag notificacaoSincroniaAtivada
                             obtida através da análise das notificações locais disponíveis.
                             *O Loop será repetido a cada 20 segundos e executado mediante a flag mencionada
                             acima e conexão com a internet.
                            */
                            sincronizarLoop();

                        }, function (error) {
                            ProvaSP_Erro("Alerta", "SQL batch ERROR: " + error.message);
                        });

                        /*
                        db.transaction(function (tx)
                        {
                            tx.executeSql(
                                "CREATE TABLE IF NOT EXISTS QuestionarioUsuario (QuestionarioUsuarioID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioID INTEGER, Guid TEXT, esc_codigo TEXT, tur_id INTEGER, usu_id TEXT, Enviado INTEGER, DataPreenchimento TEXT);" +
                                "CREATE TABLE IF NOT EXISTS QuestionarioRespostaItem (QuestionarioRespostaItemID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioUsuarioID INTEGER, Numero TEXT, Valor TEXT);"
                            );
                        }, function (error)
                            {
                                alert('Transaction ERROR: ' + error.message);
                            }, function ()
                            {
                                //sucesso
                                console.log("db - tabelas criadas.");
                                sincronizar();
                        });
                        */

                    },
                    function (er) { ProvaSP_Erro("Alerta", er); }
                );
            }
            catch (er) { ProvaSP_Erro("Alerta", "Não foi possível abrir o banco de dados: " + er); }
        }

        /**
        -----AMCOM-----
         *Mostrando divs de questionários para tipos específicos de usuário
         *Questionário do Supervisor foi descontinuado em 2018
        */
        if (Usuario.Diretor) {
            $("#divAbrirQuestionarioID_24").show();
            // Desabilitar temporariamente a ficha de registro dos professores 22/10/19
            //$("#divAbrirQuestionarioID_10").show();
        }
        if (Usuario.Professor) {
            $("#divAbrirQuestionarioID_23").show();
        }
        if (Usuario.Aluno) {
            var turma_ano = parseInt(Usuario.Ano);
            if (Usuario.Ano == null) { turma_ano = 0 }

            $("#div_fichasderegistro").hide();
            if (turma_ano >= 3 && turma_ano <= 6) { $("#divAbrirQuestionarioID_21").show(); }
            else if (turma_ano >= 7 && turma_ano <= 9) { $("#divAbrirQuestionarioID_22").show(); }

            //Estudante não deve ser capaz de "aplicar prova" 4904227
            $("#btnAbrirFichaAplicadorProvaOuChamada").hide();
        }
        if (Usuario.AssistenteDeDiretoria) {
            $("#divAbrirQuestionarioID_25").show();
        }
        // Desabilitar temporariamente Auxiliar, Agente, Coordenador e Supervisor. 04/11/2019
        /*if (Usuario.Supervisor) {
            //$("#divAbrirQuestionarioID_1").show();
            $("#divAbrirQuestionarioID_9").show();
        }
        if (Usuario.Coordenador) {
            $("#divAbrirQuestionarioID_3").show();
            $("#divAbrirQuestionarioID_11").show();
        }
        if (Usuario.AuxiliarTecnicoEducacao) {
            $("#divAbrirQuestionarioID_14").show();
        }
        if (Usuario.AgenteEscolar) {
            $("#divAbrirQuestionarioID_15").show();
            $("#divAbrirQuestionarioID_16").show();
            $("#divAbrirQuestionarioID_17").show();
        }*/

        /**
        -----MSTECH-----
         *Apesar dos botões referentes aos questionários serem determinados pelo tipo de usuário acima,
         O trecho abaixo determina o evento de click de cada um dos botões de questionários com índice
         presente no Array questionarios. Ou seja, mesmo que o botão jamais seja clicado, ele recebe um
         evento correspondente.
        */
        for (var i = 0; i < questionarios.length; i++) {
            var questionationarioID = questionarios[i];
            var btn = "#btnAbrirQuestionarioID_" + questionationarioID;
            $(btn).show();
            /**
            -----MSTECH-----
             *Evento de CLICK do botão selecionado
            */
            $(btn).click(
                function () {
                    /**
                    -----MSTECH-----
                     *Obtendo o ID do questionário através do ID do elemento DOM
                     *Mostrando tela de questionário
                    */
                    var questionarioId = parseInt(this.id.replace("btnAbrirQuestionarioID_", ""));
                    $(".page").hide();
                    $("#questionario-page").show();

                    /**
                    -----MSTECH-----
                     *Configurando questionário bem como atribuindo mensagens oportunas de acordo com o tipo
                     de questionário carregado.
                    */
                    selecionarQuestionario(questionarioId);

                    if (questionarioCarregado[questionarioId] == null) {
                        /**
                        -----MSTECH-----
                         *O HTML da div que receberá o questionário é temporariamente substituido por uma
                         mensagem de carregamento
                        */
                        $("#divQuestionario" + questionarioId + "_Questoes").html("Aguarde...");
                        $.mobile.loading("show", {
                            text: "Aguarde...",
                            textVisible: true,
                            theme: "a",
                            html: ""
                        });

                        /**
                        -----MSTECH-----
                         *Em seguida é carregado no mesmo div da mensagem de espera um dos arquivos HTML
                         referentes ao questionário selecionado.

                         IMPORTANTE: É neste trecho que os questionários são carregados.
                        */
                        $("#divQuestionario" + questionarioId + "_Questoes").load("/AppProvaSP/questionario_" + questionarioId + ".html?guid=" + newGuid(), function () {
                            $.mobile.loading("hide");

                            /**
                            -----MSTECH-----
                             *Notar que, depois que um questionário é criado, o método definirEventHandlers
                             é chamado novamente.
                            */
                            $("#divQuestionario" + questionarioId + "_Questoes").trigger("create");
                            questionarioCarregado[questionarioId] = true;
                            definirEventHandlers();

                            /**
                            -----MSTECH-----
                             *Aplicamos uma lógica de BIB diferente para os questionários dos alunos,
                             tendo em vista a estruturação dos mesmos.
                            */
                            if (questionarioId != 14) { aplicarBIB(); }
                            else {
                                for (var i = 0; i < 3; i++) {
                                    $(".BIB_BA" + (i + 1)).hide();
                                    $(".BIB_BB" + (i + 1)).hide();
                                }
                            }

                            /**
                            -----MSTECH-----
                             *Quando for questionário 9 ou 10, colocar o identificador do
                             supervidor/diretor no primeiro item.
                            */
                            if (questionarioId >= 9 && questionarioId <= 11) {
                                $("#Questionario_" + questionarioId + "_Questao_1").val(Usuario.usu_login);
                            }
                        });
                    }
                    /**
                    -----MSTECH-----
                     *Manipulando botão voltar no dispositivo para executar uma ação específica.
                     *Neste caso, sair do questionário.
                    */
                    adicionarItemBackButton("btnQuestionarioSair");
                }
            );

            /*
            var btnRelatorioAcompanhamento = "#btnAbrirRelatorioAcompanhamento_" + questionationarioID;
            if ($(btnRelatorioAcompanhamento) != null)
            {
                $("#btnAbrirRelatorioAcompanhamento_" + questionationarioID).show();
            }
            */
        }
    }
    catch (error) {
        $("#loadingLabelPrincipal").html("Acesso negado. Por favor verifique suas credenciais.");
        console.log(error);
    }
}

function ObterUsuario() {
    if (!mobile) {
        if (window.location.href.indexOf("file:///") == 0) {
            return JSON.parse(localStorage.getItem("Usuario"));
        }
        else {
            return jsonUsuario;
            //ConfigurarUsuarioSerap(grupoSerap, jsonUsuario);
        }
    }
    else {
        return JSON.parse(localStorage.getItem("Usuario"));
    }
}

/**
-----MSTECH-----
 *Método para preparar a visualização do Aluno. Sobretudo para obter os resultados das
 edições do ProvaSP que ele participou.

 *Método reposicionado para funcionar na aplicação inteira
*/
function resultadoAlunoConfigurarInterface() {
    try {
        $.mobile.loading("show", {
            text: "Aguarde...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $(".page").hide();
        $("#resultado-aluno-page").show();

        /**
        -----MSTECH-----
         *Busca no servidor ProvaSP as participações do aluno em edições do ProvaSP.
         Tais opões são disponibilizadas num SELECT para visualização dos resultados
        */
        $.post(urlBackEnd + "api/AlunoParticipacaoEdicoes?guid=" + newGuid(), { alu_matricula: Usuario.usu_login })
            .done(function (edicoes) {
                for (var i = 0; i < edicoes.length; i++) {
                    $("#ddlResultadoAlunoEdicao").append("<option value='" + edicoes[i] + "'>" + edicoes[i] + "</option>");
                }
                $.mobile.loading("hide");
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
    }
    catch (error) { console.log(error); }
}
/**
-----MSTECH-----
 *Fim do Módulo 1 - Início
*/



/**
-----MSTECH-----
 *Módulo 2 - Métodos auxiliares
 *Os trecho de código abaixo possuem a definição de métodos auxiliares para funcionamento de
 diversos detalhes do App como:
 -BackButton;
 -GUID;
 -BIB.
*/

/**
-----MSTECH-----
 *O BIB é um identificador especial, baseado no número de login do usuário que determina
 se alguns itens específicos dos questionários serão mostrados.
 *Aparentemente, de acordo com o usu_login, usuário responderão questões diferentes aleatoriamente.

 Continuar aplicando BIB da forma descrita abaixo? Ou seja, bloquear itens dos questionários
 para usuários específicos através do usu_login?
 RESPONDIDO: Sim, continuaremos utilizando o método de Blocos Incompletos Balanceados, inclusive
 incrementando um pouco a implementação para os questionários de alunos.
*/
function aplicarBIB() {
    try {
        var bloco = 0;
        var qtdCadernos = 1;

        /**
        -----AMCOM-----
         *Questionário dos alunos.
        */
        // Lógica aplicada para edição 2019
        if (questionarioId_atual == 21 || questionarioId_atual == 22) { // Alunos 3-6 anos e 7-9 anos
            var turma_ano = parseInt(Usuario.Ano);
            if (Usuario.Ano == null) { turma_ano = 0 }

            //Aplica o BIB correto baseado no RF do usuário. Cada BIB corresponde a um caderno de respostas.
            qtdCadernos = 6;
        }
        else if (questionarioId_atual == 23) { qtdCadernos = 6; } // Professor
        else if (questionarioId_atual == 3) { qtdCadernos = 7; }
        else { qtdCadernos = 3; }

        /**
        -----AMCOM-----
         *Faz um cálculo do BIB com base no Usuario.usu_login
         *Determinando o caderno correto e os blocos correspondentes
        */
        let usuLoginNum = parseInt(Usuario.usu_login.replace(/\D/g, ""));
        if (isNaN(usuLoginNum))
            usuLoginNum = 0;
        bloco = (usuLoginNum % qtdCadernos) + 1;
        for (var i = 0; i < qtdCadernos; i++) {
            $(".BIB_B" + (i + 1)).hide();
        }
        $(".BIB_B" + bloco).show();

        /** -----AMCOM-----
         * Aplica de forma automática a numeração das questões visíveis.
         */
        let bibVisivel = $(".BIB_TODOS, .BIB_B" + bloco);
        let numeroQuestoes = bibVisivel.find('.num-questao');
        let numAtual = 1;

        numeroQuestoes.each(function (index) {
            this.textContent = numAtual + '.';
            numAtual++;
        });
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Adicionado BIB para o questionário 14 de Auxiliar Técnico da Educação
 *O BIB é determinado de acordo com a seleção do primeiro ITEM.
 -Se for A: Secretaria
 -Se for B: Inspetoria
*/
function aplicarBIBQuestionario14(ASelecionado) {
    try {
        var bloco = (parseInt(Usuario.usu_login.replace(/\D/g, "")) % 3) + 1;

        for (var i = 0; i < 3; i++) {
            $(".BIB_BA" + (i + 1)).hide();
            $(".BIB_BB" + (i + 1)).hide();
        }
        if (ASelecionado) { $(".BIB_BA" + bloco).show(); }
        else { $(".BIB_BB" + bloco).show(); }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Método para excluir todas as notificações locais do ProvaSP.
 *A execução se dá apenas quando os dados são sincronizados com sucesso.

 OBS: Na verdade existirá apenas uma notificação quando houver dados a serem sincronizados.
 Tal notificação é diária e persiste até o correto envio das informações.
*/
function excluirNotificacaoLocal() {
    cordova.plugins.notification.local.cancelAll(
        function () {
            notificacaoSincroniaAtivada = false;
            console.log("Notificação local cancelada.");
        }, this);

    //cordova.plugins.notification.local.clear(0, function ()
    //{
    //    notificacaoSincroniaAtivada = false;
    //    console.log("Notificação local cancelada.");
    //});

    /*
    cordova.plugins.notification.local.clearAll(function ()
    {
        console.log("Notificação local cancelada.");
    }, this);
    */
}

/**
-----MSTECH-----
 *Métodos para tratamento do BackButton do Android nas telas de funcionalidades
*/

//document.addEventListener('deviceready', function (event)
//{
//    document.addEventListener('backbutton', function (e)
//    {
//        //navigator.app.exitApp();
//        //$("#foo").trigger("click");
//        voltarCaminhoBackButton();
//    });
//}, false);

/**
-----MSTECH-----
 *Parte da lógica do backbutton foi trasnferida para o global.js
 *Assim, antes de encerrar o App, uma dialogo é mostrado e o backButton se aplica também à tela index.
*/
function voltarCaminhoBackButton() {
    try {
        if (caminhoBackButton == null)
            caminhoBackButton = new Array();

        /**
        -----MSTECH-----
         *Se não existe caminho para Back, fecha o App
        */
        if (caminhoBackButton.length == 0) {
            if (window.location.href.indexOf("#") == -1)
                ProvaSP_CloseApp();
        }
        else {
            var indiceItemAtual = caminhoBackButton.length - 1;
            var btnAtual = caminhoBackButton[indiceItemAtual];
            $("#" + btnAtual).trigger("click");
        }
    }
    catch (error) {
        console.log(error);
        navigator.app.exitApp();
    }
}

/**
-----MSTECH-----
 *Abaixo métodos para manipular os eventos de BackButton do Android
 *Reparar que o backbutton foi construído adicionando o elemento do botão ao vetor de armazenamento
 *e, através do JQuery, chamando o evento do botão correspondente.
*/
function removerItemBackButton() {
    try {
        if (caminhoBackButton == null)
            caminhoBackButton = new Array();

        if (caminhoBackButton.length == 0) {
            return;
        }
        caminhoBackButton.pop();
    }
    catch (error) {
        console.log(error);
    }
}

function adicionarItemBackButton(btn) {
    try {
        if (caminhoBackButton == null)
            caminhoBackButton = new Array();

        caminhoBackButton.push(btn);
    }
    catch (error) {
        console.log(error);
    }
}

$(document).keyup(function (e) {
    try {
        if (e.keyCode == 27) { // escape key maps to keycode `27`
            voltarCaminhoBackButton();
        }
    }
    catch (error) {
        console.log(error);
    }
});

/**
-----MSTECH-----
 *Métodos para criar identificador único GUID
*/
function newGuid() {
    try {
        //return uuidv4();
        return guidPart() + guidPart() + '-' + guidPart() + '-' + guidPart() + '-' +
            guidPart() + '-' + guidPart() + guidPart() + guidPart();
    }
    catch (error) {
        console.log(error);
        return "";
    }
}
/*
//a função uuidv4() não roda em android 4 (dispositivo testado Samsung Tab E)
//https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
function uuidv4()
{
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    )
}
*/

function guidPart() {
    try {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    catch (error) {
        console.log(error);
        return "";
    }
}

/**
-----MSTECH-----
 *Os métodos a seguir servem para validar os códigos das provas aplicadas.
 *É um algoritmo de cálculo matemático que fornece a validação necessária para verificar,
 por exemplo se o código das provas obtido pelo leitor de códigos de barras é realmente válido.
 *Outra situação para validação dos códigos das provas é quando o aplicador erra um número
 ao digitar manualmente.

 *O algoritmo retorna um dígito calculado de acordo com o código de entrada. Tal dígito deve ser
 igual ao dígito validador do código.
 POR EXEMPLO:
    -Os 7 primeiros dígitos do código da prova são o número de inscrição.
    -O oitavo dígito é o dígito validador;
    -Os 7 primeiros dígitos são analisados pelo algoritmo abaixo;
    -O retorno da função deve ser igual ao dígito validator  (o oitavo dígito do código
    de identificação da prova).

 Sendo assim, tais métodos de validação garantem a coerência dos dados das provas.


 ATUALIZAÇÃO: Método descontinuados. Validações da versão de 2018 não são mais feitas
 através de dígito validador.
*/
//function calculaDigito(dado) {
//    try {
//        return calculaDigitoMod11(dado.toString(), 1, 9, true);
//    }
//    catch (error) {
//        console.log(error);
//        return -1;
//    }
//}

/**
 * Retorna o(s) numDig Dígitos de Controle Módulo 11 do
 * dado, limitando o Valor de Multiplicação em limMult,
 * multiplicando a soma por 10, se indicado:
 *
 *    Números Comuns:   numDig:   limMult:   x10:
 *      CPF                2         12      true
 *      CNPJ               2          9      true
 *      PIS,C/C,Age        1          9      true
 *      RG SSP-SP          1          9      false
 *
 * @version                V5.0 - Mai/2001~Out/2015
 * @author                 CJDinfo
 * @param  string  dado    String dado contendo o número (sem o DV)
 * @param  int     numDig  Número de dígitos a calcular
 * @param  int     limMult Limite de multiplicação
 * @param  boolean x10     Se true multiplica soma por 10
 * @return string          Dígitos calculados
 */
//function calculaDigitoMod11(dado, numDig, limMult, x10) {
//    try {
//        var mult, soma, i, n, dig;
//
//        if (!x10) numDig = 1;
//
//        for (n = 1; n <= numDig; n++) {
//            soma = 0;
//            mult = 2;
//
//            for (i = dado.length - 1; i >= 0; i--) {
//                soma += (mult * parseInt(dado.charAt(i)));
//                if (++mult > limMult) mult = 2;
//            }
//
//            if (x10) {
//                dig = ((soma * 10) % 11) % 10;
//            }
//            else {
//                dig = soma % 11;
//                if (dig == 10) dig = "X";
//            }
//            dado += (dig);
//        }
//        return dado.substr(dado.length - numDig, numDig);
//    }
//    catch (error) {
//        console.log(error);
//        return -1;
//    }
//}


/**
-----MSTECH-----
 *Fim do Módulo 2 - Métodos auxiliares
*/


/**
-----MSTECH-----
 *Módulo 3 - Questionários
 -Envio de questionário;
 -Salvamento de questionários localmente;
 -Criação de notificação lembrete;
 -Manipulação de elementos dos questionários com base no tipo de usuário.
*/

/**
-----MSTECH-----
 *Este método determina a seleção de questionários.
 *Perceber que podemos identificar o objetivo de cada um dos tipos de questionário
 *
*/
function selecionarQuestionario(questionarioId) {
    try {
        questionarioId_atual = questionarioId;
        $("#tituloQuestionario").html($("#tituloQuestionario" + questionarioId).val());
        /**
        -----MSTECH-----
         *Aparentemente não existe uma div de nome divQuestionarios
         *
        */
        $("#divQuestionarios").children("div").each(function () { $(this).hide() });

        /**
        -----MSTECH-----
         *Esconde todos os questionários e depois mostra apenas o selecionado
         *
        */
        $("#Questionarios form").hide();
        $("#Questionario" + questionarioId).show();

        /**
        -----MSTECH-----
         *Questionários 1, 24, 3 e 23 são questionários padrão sobre dados pessoais e dia a dia das
         pessoas atuantes nas escolas. Veja detalhes:
         -Questionários 1: Questionário do Supervisor Escolar
         -Questionários 24: Questionário do Diretor de Escola
         -Questionários 3: Questionário do Coordenador Pedagógico
         -Questionários 13: Questionário Assistente de Diretoria
         -Questionários 23: Questionário do(a) Professor(a)

         -Questionários 18: Questionário dos alunos do 3º ano
         -Questionários 19: Questionário dos alunos do 4º ao 6º ano
         -Questionários 20: Questionário dos alunos do 7º ao 9º ano

         *OBS: Apesar de usarem a mesma estrutura base, como existem questionários muito distintos
         (veremos abaixo) cada tipo de questionário exige manipulação dos elementos da UI
        */
        if ((questionarioId >= 1 && questionarioId <= 3) ||
            (questionarioId >= 14 && questionarioId <= 25)) {
            /**
            -----MSTECH-----
             *Com base no usu_login do usuáro, determina itens específicos do questionários selecionado
            */
            if (questionarioId != 14) { aplicarBIB(); }

            $("#divQuestionario" + questionarioId + "_Intro").show();
            $("#divQuestionario" + questionarioId + "_Questoes").hide();
            $("#divTituloQuestionario").hide();
        }
        /**
        -----MSTECH-----
         *Questionários do tipo 8 são, de fato, o acompanhamento do ProvaSP no dia da aplicação.
         *É através desses questionários que o usuário aplicará a prova, utilizando todas as funcionalidades
         para tal, como:
          -Fornecimento do código da prova e do candidato;
          -Lista de presença;
          -Observações;
          -E etc.
        */
        else if (questionarioId == 8) {
            $("#divCodigoTurma").show();
            $("#divMenuPrincipal").hide();
            $("#txtCodigoTurma").focus();
            adicionarItemBackButton("btnCodigoTurmaVoltar");
        }
        /**
        -----MSTECH-----
         *Questionários dos tipos 9, 10 e 11 são Fichas de registro. Seguem:
         -Questionário 9: Ficha de Registro - Supervisor Escolar
         -Questionário 10: Ficha de Registro - Diretor(a) de Escola
         -Questionário 11: Ficha de Registro - Coordenador(a) Pedagógico(a)

         *Ficha de registro 9 não é tratada? Também não é tratada na versão publicada do App
        */
        else if (/*questionarioId == 9 || */questionarioId == 10 || questionarioId == 11) {
            //Questionário 9, ficha de registro de Supervisor Escolar não é carregada?
            //RESPONDIDO: É carregada, apenas não existe uma mensagem indicando que deva ser respondida
            //após o término do último dia de aplicação.
            /**
            -----MSTECH-----
             *Alerta apenas para orientar o usuário sobre o preenchimento do questionário ao fim da ProvaSP
             *Aparentemente não existe forma de impedir o preenchimento antes da data correta.
            */
            swal({
                title: "Atenção!",
                text: "Essa ficha deve ser preenchida após o término do último dia de aplicação. Deseja continuar?",
                type: "warning",
                showCancelButton: true,

                confirmButtonText: "Não",
                cancelButtonText: "Sim",
                closeOnConfirm: false
            },
                function () {
                    //window.location = "menu.html";
                    removerItemBackButton();
                    $(".page").hide();
                    $("#menu-page").show();
                    swal.close();
                });
        }

        /**
        -----MSTECH-----
         *Trecho específico para professores e na WEB
        */
        //if (questionarioId == 23 && !mobile) {
        /**
        -----MSTECH-----
         *Idem ao alerta anterior.
         *Não existe forma de identificar se o professor não é regente do 3º ao 5º anos.

         Atualizado: Removido dialog de confirmação. Todos os docentes devem responder ao
         questionário em 2018
        */
        //window.location = "menu.html";
        //    removerItemBackButton();
        //    $(".page").hide();
        //    $("#menu-page").show();
        //    swal.close();
        //}
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Função Callback para requisição local e carregamento do arquivo escolas.csv.
 *Basicamente o aquivo escolas.csv é aberto e com base no ID do questionário, busca-se
 o nome e o código da escola correspondente.

 Este método é irrelevante (Não é usado)
 *
*/
function recuperarCodigoENomeDaEscolaParaQuestionario(questionarioId) {
    carregarDataEscola(
        function () {
            try {
                var esc_codigo = "";
                var esc_nome = "";

                /**
                -----MSTECH-----
                 *Lembrando que array de questionário é [1,2,3,8,9,10,11,23].
                 *Aparentemente está faltando um trecho do código aqui para determinar o valor da chave
                 do questionário
                 *No que diz respeito ao código abaixo, o valor de esc_codigo vai ser vazio

                 OBS: Aparentemente a variável "r" não é utilizada

                 Faltando complemento dos índices de questionarios
                 RESPONDIDO: Método não utilizado. Desconsiderado em relação a versão anterior.
                */
                for (var i = 0; i < questionarios.length; i++) {
                    var r = questionarios[i];
                    var chaveValor = questionarios[i].split("=");
                    if (chaveValor[0] == questionarioId) {
                        esc_codigo = chaveValor[1];
                    }
                }
                $("#Questionario_" + questionarioId + "_Questao_1").val(esc_codigo);

                /**
                -----MSTECH-----
                 *Depois de atribuir o código da escola automaticamente, através dele será possível obter
                 também o nome da escola.

                 OBS: dataEscola é o objeto gerado após a leitura do arquivo escolas.csv
                 OBS2: Perceber que o nome da escola é obtivo através do r[2], pois no arquivo CSV,
                 separando as informações por ";", o nome da escola está na posição 2
                 OBS3: Foi adiciona um "break" pois não há necessidade de percorrer o código inteiro
                 depois de obter o valor buscado.
                */
                var l = dataEscola.length;
                for (var i = 0; i < l; i++) {
                    var r = dataEscola[i].split(";");
                    if (esc_codigo == r[1]) {
                        esc_nome = r[2];
                        r = l;
                        break;
                    }
                }
                //$("#Questionario_" + questionarioId + "_esc_nome").val(esc_nome);
                $("#Questionario_" + questionarioId + "_esc_nome").html(esc_nome);
            }
            catch (error) {
                console.log(error);
            }
        }
        /**
        -----MSTECH-----
         *Não há opções adicionais.
        */
        , ""
    );
}

/**
-----MSTECH-----
 *Método responsável por obter o conteúdo do arquivo escola.csv utilizando requisição AJAX.
 *O método seta a variável global dataEscola.
 *Reparar que opções é, na verdade, o método de retorna da chamada a este método. Ou seja, ao buscar
 as informações do arquivo escola.csv, recursivamente o método da chamada é executado em seguida como
 retorno.
*/
function carregarDataEscola(callback, opcoes) {
    return new Promise((resolve, reject) => {
        try {
            if (dataEscola.length == 0) {
                /**
                -----MSTECH-----
                 *Converte o conteúdo do arquivo CSV em um vetor cujas posição são as linhas do arquivo original.
                */
                $.ajax({
                    type: "GET",
                    url: "/AppProvaSP/escola.csv",
                    dataType: "text",
                    success: function (data) {
                        if (data.indexOf("\r\n") > 0) {
                            data = data.replace(/\r\n/g, "\n");
                        }
                        dataEscola = data.split("\n");

                        if (typeof callback === 'function') {
                            callback(opcoes);
                            resolve();
                        } else {
                            callback.then(() => {
                                resolve();
                            });
                        }

                    }
                });
            } else {
                if (typeof callback === 'function') {
                    callback(opcoes);
                    resolve();
                } else {
                    callback.then(() => {
                        resolve();
                    });
                }
            }
        }
        catch (error) {
            console.log(error);
        }
    });
}

/**
-----MSTECH-----
 *Popula o SELECT de nomes das escolas com base na DRE do questionário 9
 *Reparar que "Selecione a Escola" é uma opção do select, mas sem trigger
 *Reparar ainda que se a uad_sigla for vazia, não mostra o select
*/
function selecionarDRE(uad_sigla) {
    try {
        var l = dataEscola.length;
        $("#ddlEscola").empty();
        $("#ddlEscola").append("<option value=\"\">Selecione a Escola</option>");

        if (uad_sigla == "") {
            $("#ddlEscola").selectmenu("disable");
            //document.getElementById("ddlEscola").disabled = true;
            return;
        }

        /**
        -----MSTECH-----
         *Adicionando as opções ao SELECT com base nas escolas obtidas no arquivo escolas.csv.
         *São adicionadas apenas escolas da DRE correspondente com base na uad_sigla
        */
        $("#ddlEscola").selectmenu("enable");
        for (var i = 0; i < l; i++) {
            var r = dataEscola[i].split(";");
            if (uad_sigla == r[0]) {
                //$("#ddlEscola").append(new Option(r[2], r[1], defaultSelected, nowSelected));
                //.append('<option>newvalue</option>');
                $("#ddlEscola").append("<option value=\"" + r[1] + "\">" + r[2] + "</option>");
            }
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Esconde itens específicos do questionário 8 (acompanhamento da ProvaSP) referente às disciplinas de
 Português e Ciências.
*/
function resetInstrumento() {
    try {
        /**
        -----MSTECH-----
         *Removidos itens extras de Ciências em 2018
        */
        //$("#Questionario_8_QuestoesInstrumentoPortugues,#Questionario_8_QuestoesInstrumentoCiencias").hide();
        $("#Questionario_8_QuestoesInstrumentoPortugues").hide();
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Método simples de manipulação da interface de usuário.
 *Marca um aluno como ausente e recalcula a quantidade de alunos presentes e ausentes.

 OBS: Reparar que as informações que o questionário 8 de fato armazena de tais funcionaldiades é apenas
 PRESENTE / AUSENTE.
 Para identificar o específico que faltou, deve-se verificar as informações das provas.
*/
function marcarDesmarcarAlunoAusente(chk) {
    try {
        var idSpan = "#" + chk.id.replace("chk", "span");
        if (chk.checked) {
            $(idSpan).show();
            $(idSpan).addClass("listaPresencaAlunoAusente");
        }
        else {
            $(idSpan).hide();
            $(idSpan).removeClass("listaPresencaAlunoAusente");
        }
        calcularPresentesEAusentes();
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Calcula quantidade de alunos presentes e ausentes com base na quantidade de elementos do HTML
 com o estilo específico para tais situações.
*/
function calcularPresentesEAusentes() {
    try {
        var quantidadeAlunos = $(".listaPresencaAluno").length;
        var quantidadeAusentes = $(".listaPresencaAlunoAusente").length;
        var totalPresentes = quantidadeAlunos - quantidadeAusentes;
        $("#Questionario_8_Questao_5_Presentes").val(totalPresentes);
        $("#Questionario_8_Questao_6_Ausentes").val(quantidadeAusentes);
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Método para utilização do plugin de leitura de BARCODE (código de barras) para obtenção do
 código do caderno de prova dos Alunos.
*/
function escanearCodigoDeBarrasCadernoReserva(caderno) {
    cordova.plugins.barcodeScanner.scan(
        function (result) {
            /**
            -----MSTECH-----
             *Novo método para validação do código de barras dos cadernos reserva.
            */
            validarCodigoCadernoReserva(result, caderno);
            /*
            alert("We got a barcode\n" +
                "Result: " + result.text + "\n" +
                "Format: " + result.format + "\n" +
                "Cancelled: " + result.cancelled);
            */
            /**
            -----MSTECH-----
             *A utilização do plugin é simples. Veja:
             -Se mesmo com sucesso o resultado do escaneamento for uma string com número de caracteres
             diferente de 12, haverá uma mensagem de erro.
             -Caso contrário, o App fará um cálculo matemático preestabelecido para verificar a
             validade do número da prova.
             -Estando tudo OK, o valor escaneado será inserido no elemento HTML correspondente da UI

             O trecho abaixo tornou-se obsoleto. A prova de 2018 será verificada localmente e não
             mais por dígito verificador.
            */
            //var valido = true;
            //var resultado = result.text;
            //if (resultado.length != 14) {
            //    valido = false;
            //}
            //else {
            //    var numeroInsricao = resultado.substr(0, 7);
            //    var verificador = resultado.substr(7, 1);
            //    valido = (calculaDigito(numeroInsricao) == verificador);
            //}

            /**
            -----MSTECH-----
             *Observar que o código é mostrado como inválido mas, mesmo assim, adicionado ao campo
             correspondente.
            */
            //if (!valido) {
            //    ProvaSP_Erro("Código inválido",
            //        "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.");
            //}
            //$("#Questionario_8_Questao_13_CodigoBarrasCadernoReserva" + caderno).val(result.text);
        },
        function (error) {
            ProvaSP_Erro("Alerta", "Erro ao escanear: " + error);
        },
        /**
        -----MSTECH-----
         *Objeto de opções de escaneamento. É possível configurar as opções da câmera para melhor
         utilização do plugin. Manteremos como está.
        */
        {
            preferFrontCamera: false, // iOS and Android
            showFlipCameraButton: true, // iOS and Android
            showTorchButton: true, // iOS and Android
            torchOn: false, // Android, launch with the torch switched on (if available)
            saveHistory: true, // Android, save scan history (default false)
            prompt: "Coloque o código no quadro de escaneamento", // Android
            resultDisplayDuration: 500, // Android, display scanned text for X ms. 0 suppresses it entirely, default 1500
            /*
            formats : "QR_CODE,PDF_417", // default: all but PDF_417 and RSS_EXPANDED
            orientation : "landscape", // Android only (portrait|landscape), default unset so it rotates with the device
            */
            disableAnimations: true, // iOS
            disableSuccessBeep: false // iOS and Android
        }
    );
}

/**
-----MSTECH-----
 *Agora os cadernos reservas são verificados localmente por meio de comparação simples.
 *Não existe mais necessidade de validar os códigos de barras dos cadernos reservas através do
 dígito verificador.
*/
function validarCodigoCadernoReserva(codigoEscaneado, cadernoSelecionado) {
    try {
        //Caderno Selecionado
        var cs = $("#Questionario_8_Questao_13_CodigoBarrasCadernoReserva" + cadernoSelecionado);

        $.ajax({
            type: "GET",
            url: "cb.txt",
            dataType: "text",
            success: function (data) {
                if (data.indexOf("\r\n") > 0) {
                    data = data.replace(/\r\n/g, "\n");
                }
                var dataCodigos = data.split("\n");

                if (dataCodigos.indexOf(codigoEscaneado) != -1) {
                    cs.val(codigoEscaneado);
                }
                else {
                    cs.val("");
                    ProvaSP_Erro("Código inexistente",
                        "Por favor verifique o código escaneado e tente novamente.");
                }
                $.mobile.loading("hide");
            },
            error: function () {
                //Resetando valor do caderno
                cs.val("");
                ProvaSP_Erro("Código inválido",
                    "Por favor verifique o código de barras do caderno reserva e tente novamente.");
            }
        });
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro ao escanear: " + error);
    }
}

/**
-----MSTECH-----
 *Método responsável por salvar localmente um questionário pendente.
 *Primeiramente são setadas variáveis específicas para cada tipo de questionário.
 *Em seguida são obtidas as respostas do questionário no momento do salvamento.
 *Assim que todas as informações são salvas localmente, o App tenta sincronizar os questionários.
 *Caso não haja conexão estável no momento, os dados serão sincronizados em outra oportunidade.
 *No início do App, por exemplo, existe uma tentativa de sincronização se houver notificações locais
 pendentes.
*/
function salvarQuestionarioLocal() {
    try {
        var esc_codigo = "";
        var tur_id = "";
        var usu_id = Usuario.usu_id;
        var Enviado = "0";
        var guid = newGuid();
        var inclusaoLocalSucesso = false;

        var d = new Date().toISOString();
        var t = new Date().toTimeString().replace(/.*(\d{2}:\d{2}:\d{2}).*/, "$1");
        var DataPreenchimento = d.slice(0, 10) + " " + t;

        if (questionarioId_atual == 8) { //Ficha de Registro - Aplicador(a) de Prova
            //tur_id: recuperar da caixa de texto #txtCodigoTurma
            var codigo = codigoTurma_atual;//$("#txtCodigoTurma").val();
            tur_id = parseInt(codigo);
            //tur_id = parseInt(codigo.substring(0, codigo.length - 1));
        }
        else if (questionarioId_atual == 9) { //Ficha de Registro - Supervisor(a) Escolar
            //esc_codigo: seleção
            esc_codigo = $("#ddlEscola").val();
        }
        //else {
        //    for (var i = 0; i < questionarios.length; i++) {
        //        var questionationarioID = questionarios[i].split("=")[0];
        //        if (questionationarioID == questionarioId_atual) {
        //            esc_codigo = questionarios[i].split("=")[1];
        //            i = questionarios.length; //Funciona como um break
        //        }
        //    }
        //}

        /**
        -----MSTECH-----
         *Remove os itens duplicados com valor "default".
         *Começa a iteração a partir do do segundo elemento.
         *Elementos com resposta padrão são removidos do Array de respostas.
         *Aparentemente a exclusão dos itens com valor default também serve para obter a resposta única
         dos itens de múltipla escolha.

         *Itens não duplicados com valor "default" são tratatos no servidor
         Analisar melhor este trecho no Debug.
        */
        var respostas = $("#Questionario" + questionarioId_atual).serializeArray();
        for (var i = 0; i < respostas.length; i++) {
            if (i > 0) {
                if (respostas[i - 1].value == "default" && respostas[i - 1].name == respostas[i].name) {
                    respostas.splice(i - 1, 1);
                }
            }
        }

        /**
        -----MSTECH-----
         *Cria item no banco SQLite na tabela de QuestionarioUsuario com o questionário atual.
         *O questionário armazenado será enviado posteriormente, quando houver conexão.
        */
        db.transaction(function (tx) {
            //"CREATE TABLE IF NOT EXISTS QuestionarioUsuario (QuestionarioUsuarioID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioID INTEGER, Guid TEXT, esc_codigo TEXT, tur_id INTEGER, usu_id INTEGER, Enviado INTEGER, DataPreenchimento TEXT);" +
            //"CREATE TABLE IF NOT EXISTS QuestionarioRespostaItem (QuestionarioRespostaItemID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioUsuarioID INTEGER, Numero TEXT, Valor TEXT);"
            tx.executeSql('INSERT INTO QuestionarioUsuario (QuestionarioID, Guid, esc_codigo, tur_id, usu_id, Enviado, DataPreenchimento) VALUES (?, ?, ?, ?, ?, ?, ?);',
                [questionarioId_atual, guid, esc_codigo, tur_id, usu_id, Enviado, DataPreenchimento], function (tx, results) {
                    /**
                    -----MSTECH-----
                     *Com o ID de retorno da criação do questionário localmente, armazena-se cada resposta
                     individualmente na tabela QuestionarioRespostaItem com referência ao mesmo ID criado
                     inicialmente.
                    */
                    var QuestionarioUsuarioID = results.insertId;

                    console.log("INSERT na tabela QuestionarioUsuario. Retorno da inclusão: QuestionarioUsuarioID=" + QuestionarioUsuarioID);
                    for (var i = 0; i < respostas.length; i++) {
                        /**
                        -----MSTECH-----
                         *Reparar que cada resposta é salva com o número exato do elemento HTML da UI. Isso se
                         dá para garantir a identificação do elemento do questionário.
                        */
                        var numero = respostas[i].name.replace("Questionario_" + questionarioId_atual + "_Questao_", "");
                        var valor = respostas[i].value;
                        tx.executeSql('INSERT INTO QuestionarioRespostaItem (QuestionarioUsuarioID, Numero, Valor) VALUES (?, ?, ?);',
                            [QuestionarioUsuarioID, numero, valor],
                            function (tr, resultSet) {
                                //INSERT QuestionarioRespostaItem OK
                            },
                            function (tx, error) {
                                //INSERT ERRO
                            }
                        );
                    }
                },
                function (tx, error) {
                    ProvaSP_Erro("Alerta", error.message);
                });
        },
            function (error) {
                ProvaSP_Erro("Alerta", "Transaction ERROR: " + error.message);
            },
            function () {
                /**
                -----MSTECH-----
                 *Sucesso em todas as inserções no banco da dados local.
                 *Ao concluir tais inserções, o App tenta sincronizar as informações logo em seguida.
                */
                console.log('INSERT Questionario OK');
                $.mobile.loading("hide");
                sincronizar();
            });

        /**
        -----MSTECH-----
         *Quando não há conexão, informa ao usuário que os dados foram salvos localmente.

         *OBS: Reparar que o questionário não é enviado imediatamente. O App tentatrá na próxima vez
         que o método "sincronizar", executado a cada 20 segundos for invocado.
        */
        if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN) {
            ativarNotificacaoSincronia("Não foi detectada uma conexão ativa com a internet. As respostas " +
                "deste questionário foram salvas.\n\nSuas respostas serão enviadas assim que a conexão " +
                "for reestabelecida.");
        }
        else {
            swal("Obrigado!", "As informações foram salvas com sucesso!", "success");
        }

        /**
        -----MSTECH-----
         *Ajustando interface para ocasião do envio
        */
        removerItemBackButton();
        $(".page").hide();
        $("#menu-page").show();
        $("#divCodigoTurma").hide();
        $("#divMenuPrincipal").show();
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Métdo para adicionar ao plugin de notificações locais uma notificação diária para abrir o App e enviar
 os dados não sincronizados quando houver conexão.
 *Tais notificações serão encerradas quando as informações forem de fato sincronizadas.
 *Daí a necessidade do método inicial que verifica se existem notificações locais agendadas.
 *Se existirem, existe também a necessidade de tentar sincronizar os dados.
*/
function ativarNotificacaoSincronia(mensagem) {
    try {
        if (mensagem != "") { ProvaSP_Erro("Alerta", mensagem); }
        if (!notificacaoSincroniaAtivada) {
            notificacaoSincroniaAtivada = true;
            cordova.plugins.notification.local.schedule({
                id: 0,
                text: "Existem informações da Prova São Paulo que ainda não foram enviadas via internet. Abra aqui e envie agora.",
                every: "day",
            });
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Tenta sincronizar os dados pendentes a cada 20 segundos se houver um notificação local agendada e
 se houver conexão com a internet.
 *Caso contrário será um método irrelevante toda vez que for chamado, ou seja, não realiza ação alguma.
*/
function sincronizarLoop() {
    try {
        if (notificacaoSincroniaAtivada) {
            sincronizar();
        }
        setTimeout("sincronizarLoop()", 20000);
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Se houver conexão, tenta sincronizar questionários pendentes
*/
function sincronizar() {
    try {
        if (navigator.connection.type != Connection.NONE) {
            sincronizarQuestionarios();
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Tenta enviar para o servidor Web os questionários salvos localmente no banco da dados SQLite
 *Reparar que os questionários não enviados possuem a flag "Enviado" com valor 0.
 *Reparar ainda que mesmo questionários enviados são mantidos no banco da dados local.
*/
function sincronizarQuestionarios() {
    listaQuestionariosNaoEnviados = new Array();
    var hashRespostas = {};
    db.readTransaction(function (tx) {
        tx.executeSql('SELECT QuestionarioUsuarioID, QuestionarioID, Guid, esc_codigo, tur_id, usu_id, DataPreenchimento FROM QuestionarioUsuario WHERE Enviado=0', [],
            function (tx, quRows) {
                try {
                    //console.log('row' + rs.rows.item(0).mycount);
                    console.log('row');
                    for (var i = 0; i < quRows.rows.length; i++) {
                        var qur = quRows.rows.item(i);
                        /**
                        -----MSTECH-----
                         *Cria um objeto QuestionarioUsuario cujos atributos foram obtidos no SQLite
                         *Adicionar o objeto criado ao vetor de questionários não enviados
                        */
                        var qu = new QuestionarioUsuario(qur.QuestionarioUsuarioID, qur.QuestionarioID, qur.Guid, qur.esc_codigo, qur.tur_id, qur.usu_id, qur.DataPreenchimento, null);
                        listaQuestionariosNaoEnviados.push(qu);

                        /**
                        -----MSTECH-----
                         *Depois de armazenar as informações base do questionário, armazenaremos também
                         as respostas de tal questionário buscando-as na tabela QuestionarioRespostaItem
                         através do ID do questionário em questão.
                        */
                        tx.executeSql('SELECT QuestionarioUsuarioID, Numero, Valor FROM QuestionarioRespostaItem WHERE QuestionarioUsuarioID=?',
                            [qur.QuestionarioUsuarioID], function (tx, qriRows) {
                                var respostas = new Array();
                                var QuestionarioUsuarioID = "";

                                /**
                                -----MSTECH-----
                                 *Cria um vetor de objetos do tipo QuestionarioRespostaItem que possui número
                                 (para identificar o item) e valor.
                                 *Tal vetor será incorporado ao objeto hashRespostas com chave de acesso igual
                                 ao ID do questionário em questão.
                                */
                                for (var i2 = 0; i2 < qriRows.rows.length; i2++) {
                                    var itemRow = qriRows.rows.item(i2);
                                    var qri = new QuestionarioRespostaItem(itemRow.Numero, itemRow.Valor);
                                    respostas.push(qri);
                                    if (i2 == 0) {
                                        QuestionarioUsuarioID = itemRow.QuestionarioUsuarioID;
                                    }
                                    //listaQuestionarios
                                }
                                hashRespostas[QuestionarioUsuarioID] = respostas;

                            },
                            function (tx, error) {
                                console.log('SELECT error: ' + error.message);
                                $.mobile.loading("hide");
                            });
                    }
                    /**
                    -----MSTECH-----
                     *Portanto, antes de sincronizar com a Web, teremos:
                     -Um vetor listaQuestionariosNaoEnviados: contém os objetos de informações base de
                     cada questionário salvo localmente e ainda não enviado;
                     -Um objeto hashRespostas: contém um conjunto de objetos com as respostas dos
                     questionários organizadas pelo ID de cada questionário.
                    */
                }
                catch (error) {
                    ProvaSP_Erro("Alerta", "ITERATOR error: " + error).statusText;
                }
            },
            function (tx, error) {
                ProvaSP_Erro("Alerta", "SELECT error: " + error.statusText);
            }
        );
    },
        function (error) {
            ProvaSP_Erro("Alerta", "Transaction error: " + error.statusText);
        },
        function () {
            try {
                /**
                -----MSTECH-----
                 *Callback de sucesso ao obter informações do SQLite
                 *Este método é responsável por incorporar o objeto de respostas ao vetor de questionários
                 criando um novo vetor, mais completo, com todas as informações base do questionário e
                 todas as respostas do usuário ao mesmo questionário.
                */
                if (listaQuestionariosNaoEnviados.length > 0) {
                    /**
                    -----MSTECH-----
                     *Adiciona a cada objeto QuestionarioUsuario presente no vetor o conjunto de respostas
                     equivalente. Em seguida converte o objeto JSON correspondente em uma string serializável.
                    */
                    for (var i = 0; i < listaQuestionariosNaoEnviados.length; i++) {
                        var QuestionarioUsuarioID = listaQuestionariosNaoEnviados[i].QuestionarioUsuarioID;
                        listaQuestionariosNaoEnviados[i].Respostas = hashRespostas[QuestionarioUsuarioID];
                    }
                    var jsonListaQuestionariosNaoEnviados = JSON.stringify(listaQuestionariosNaoEnviados);
                    //jsonListaQuestionariosNaoEnviados = jsonListaQuestionariosNaoEnviados;

                    /**
                    -----MSTECH-----
                     *Chamada POST ao servidor do ProvaSP. Os questionários serão salvos no banco da dados central
                    */
                    $.post(urlBackEnd + "api/SincronizarQuestionario", { json: jsonListaQuestionariosNaoEnviados })
                        .done(function (data) {
                            /**
                            -----MSTECH-----
                             *Em caso de sucesso no envio, exclui a notificação diária lembrete de
                             sincronização e atualiza no banco de dados os questionários enviados setando
                             a flag "Enviado" para 1
                            */
                            if (data.length == listaQuestionariosNaoEnviados.length) {
                                excluirNotificacaoLocal();
                            }
                            atualizarGuidsComoEnviados(data);
                        })
                        .fail(function (xhr, status, error) {
                            /**
                            -----MSTECH-----
                             *Não sendo possível enviar os quesitonários, ativa a notificação local de
                             lembrete diária silenciosamente se ainda não estiver habilitada e mostra o
                             erro em questão.
                            */
                            if (!notificacaoSincroniaAtivada) {
                                ativarNotificacaoSincronia("");
                            }
                            ProvaSP_Erro("Falha de comunicação",
                                "Não foi possível sincronizar as informações com o servidor. (" +
                                status + ") " + error);
                        });
                }
            }
            catch (error) {
                $.mobile.loading("hide");
                console.log(error);
            }
        });
}

/**
-----MSTECH-----
 *Atualiza todas as flags "Enviado" dos questionários sincronizados para 1.
 *Isso garante que o ProvaSP não tente enviar tais questionários novamente.
*/
function atualizarGuidsComoEnviados(data) {
    try {
        //alert(data);
        db.transaction(function (tx) {
            for (var i = 0; i < data.length; i++) {
                var guid = data[i];
                tx.executeSql('UPDATE QuestionarioUsuario SET Enviado=1 WHERE Guid=?;', [guid],
                    function (tr, resultSet) {
                        console.log("UPDATE QuestionarioUsuario ok");
                    },
                    function (tx, error) {
                        console.log(error);
                    }
                );
            }
        },
            function (error) {
                ProvaSP_Erro("Alerta", "Transaction error: " + error.message);
            },
            function () {
                console.log("Questionários sincronizados com sucesso");
            });
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Transaction error: " + error.message);
    }
}

/**
-----MSTECH-----
 *Validações para permitir utilização de cardenos reservas.
 *Aparentemente são 4 cadernos reservas para cada aplicação.
 *Os cadernos reservas resolvem problemas como: alunos transferidos de turmas ou escola, problemas
 nas informaçõe geradas do sistema central e etc.
 *Assim sendo, o aluno recebe o cardeno reserva que faz a prova associando seu EOL ao código da prova.

 *Não há mais necessidade de validar os códigos das provas reservas através de dígito validador
 em 2018. Sendo assim, implementa-se a validação por verificação abaixo.
*/
function validarCadernosReservas() {
    try {
        var codigosDeBarraDisponiveis = $.ajax({
            type: "GET",
            async: false,
            url: "cb.txt",
            dataType: "text",
        }).responseText;
        if (codigosDeBarraDisponiveis.indexOf("\r\n") > 0) {
            codigosDeBarraDisponiveis = codigosDeBarraDisponiveis.replace(/\r\n/g, "\n");
        }
        var vetorCodigosDeBarra = codigosDeBarraDisponiveis.split("\n");
        /**
        -----MSTECH-----
         *Percorre as informações dos cadernos reservas validando o código da prova e fazendo
         verificação simples do código EOL do aluno (não valida).

         OBS: Reparar que o preenchimento das informações dos cadernos reservas nada mais é do
         que um conjunto de itens do questionário 8.
        */
        for (var i = 1; i <= 4; i++) {
            var valorCodigoBarras = $("#Questionario_8_Questao_13_CodigoBarrasCadernoReserva" + i).val();
            var codigoEolAluno = $("#Questionario_8_Questao_13_CodigoEolAlunoCadernoReserva" + i).val();

            /**
            -----MSTECH-----
             *A validação só deve ser feita se o caderno foi utilizado, ou seja, os dados
             foram preenchidos.

             OBS: Primeiro valida-se o código da prova, em seguido o código do aluno
            */
            if (valorCodigoBarras != "" || codigoEolAluno != "") {
                if (vetorCodigosDeBarra.indexOf(valorCodigoBarras) == -1) {
                    //if (!validarCodigoBarrasCadernoReserva(caderno, valorCodigoBarras)) {
                    ProvaSP_Erro("Erro", "O código de barras do caderno reserva " + i + " é inválido!");
                    return false;
                }

                if (codigoEolAluno == "" || !$.isNumeric(codigoEolAluno)) {
                    ProvaSP_Erro("Erro", "O Código EOL informado para o caderno reserva " + i + " é inválido!");
                    return false;
                }
            }
        }
        return true;
    }
    catch (error) {
        console.log(error);
        return false;
    }
}

/**
-----MSTECH-----
 *Validação do código de barras da prova.
 *Observar que é exatamente a mesma validação das provas regulares aplicadas.

 Este código é irrelevante para a edição de 2018, tendo em vista que a validação dos cadernos
 reservas não é mais feito através de dígito validador.
*/
//function validarCodigoBarrasCadernoReserva(caderno, valorCodigoBarras) {
//    try {
//        var valido = true;
//        if (valorCodigoBarras.length != 12) {
//            valido = false;
//        }
//        else {
//            var numeroInsricao = valorCodigoBarras.substr(0, 7);
//            var verificador = valorCodigoBarras.substr(7, 1);
//            valido = (calculaDigito(numeroInsricao) == verificador);
//        }
//        return valido;
//    }
//    catch (error) {
//        console.log(error);
//        return false;
//    }
//}

/**
-----MSTECH-----
 *Este método é responsável por enviar o questionário quando o usuário utiliza o ProvaSP na Web.
 *Vimos que para enviar o questionário usando o mobile App, salvamos o mesmo e tentamos enviar. Se
 não for possível, o questionário encontra-se salva localmente e uma notificação será enviada
 diariamente informando sobre a possibilidade e envio.

 OBS: Veremos que o evento do botão "CONCLUIR QUESTIONÁRIO" filtra o envio da seguinte maneira:
 -Mobile App: salvarQuestionarioLocal
 -Web App: enviarQuestionarioOnline
*/
function enviarQuestionarioOnline() {
    try {
        /**
        -----MSTECH-----
         *Os trechos iniciais abaixo são EXATAMENTE iguais às validações feitas em
         salvarQuestionarioLocal.
         *Poderíamos lapidar o código aqui melhorando a orientação a objeto. Não o faremos pela
         questão do tempo e testes.
        */
        var esc_codigo = "";
        var tur_id = "";
        var Enviado = "1";
        var guid = newGuid();

        var DataPreenchimento = "";

        if (questionarioId_atual == 8) {//Ficha de Registro - Aplicador(a) de Prova
            //tur_id: recuperar da caixa de texto #txtCodigoTurma
            var codigo = codigoTurma_atual;//$("#txtCodigoTurma").val();
            tur_id = parseInt(codigo);
            //tur_id = parseInt(codigo.substring(0, codigo.length - 1));
        }
        else if (questionarioId_atual == 9) { //Ficha de Registro - Supervisor(a) Escolar
            //esc_codigo: seleção
            esc_codigo = $("#ddlEscola").val();
        }
        else {
            /**
            -----MSTECH-----
              Aqui questionarioId_atual não retorna esc_codigo. esc_codigo será ""
              RESPONDIDO: Este trecho não funciona na versão atual, tendo em vista que o vetor
              questionarios não possui nada além dos índices dos questionários.
            */
            //for (var i = 0; i < questionarios.length; i++) {
            //    var questionationarioID = questionarios[i].split("=")[0];
            //    if (questionarios[i] == questionarioId_atual) {
            //        esc_codigo = questionarios[i].split("=")[1];
            //        i = questionarios.length;
            //    }
            //}
        }

        //Remove os itens duplicados com valor "default".
        //Itens não duplicados com valor "default" são tratatos no servidor
        var respostas = $("#Questionario" + questionarioId_atual).serializeArray();
        for (var i = 0; i < respostas.length; i++) {
            if (i > 0) {
                if (respostas[i - 1].value == "default" && respostas[i - 1].name == respostas[i].name) {
                    respostas.splice(i - 1, 1);
                }
            }
        }

        /**
        -----MSTECH-----
         *A partir daqui, diferentemente da versão Mobile App em que obtemos N questionários salvos
         no banco local para sincronizar, na versão Web tentaremos sincronizar apenas o questionário
         atual.

         O fluxo do código se dá assim:
         -Montagem dos dados base do questionário;
         -Obtenção das respostas ao itens;
         -Reparar que existe apenas um objeto de questionário, mesmo assim, para atender à estrutura
         de envio à Web, teremos um vetor de UMA posição com o questionário atual.
         -O vetor de objetos de questionário (no caso, apenas 1 objeto) será transformado em string
         e enviado à Web pelo mesmo POST que tenta enviar na versão Mobile App.
        */
        var qu = new QuestionarioUsuario();
        qu.QuestionarioUsuarioID = 0;
        qu.QuestionarioID = questionarioId_atual;
        qu.Guid = "";
        qu.esc_codigo = esc_codigo;
        qu.tur_id = tur_id;
        qu.usu_id = Usuario.usu_id;
        qu.DataPreenchimento = "";

        var respostasArray = new Array();
        var QuestionarioUsuarioID = "";
        for (var i = 0; i < respostas.length; i++) {
            var numero = respostas[i].name.replace("Questionario_" + questionarioId_atual + "_Questao_", "");
            var valor = respostas[i].value;
            var qri = new QuestionarioRespostaItem(numero, valor);
            respostasArray.push(qri);
        }

        qu.Respostas = respostasArray;
        listaQuestionariosNaoEnviados = new Array();
        listaQuestionariosNaoEnviados.push(qu);
        var jsonListaQuestionariosNaoEnviados = JSON.stringify(listaQuestionariosNaoEnviados);

        /**
        -----MSTECH-----
         *Post de envio do questionário respondido.

         OBS: Reparar que na Web, nada além de ajustes de interface é feito, por não haver tratamentos
         adicionais.
        */
        $.post(urlBackEnd + "api/SincronizarQuestionario", { json: jsonListaQuestionariosNaoEnviados })
            .done(function (data) {
                swal("Obrigado!", "As informações foram enviadas com sucesso!", "success");
                $(".page").hide();
                $("#menu-page").show();
                $.mobile.loading("hide");
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Botão que, de fato, conclui o envio dos questionários.
 *A ação final deste botão é diferente se o ProvaSP for usado no dispositivo Mobile ou na Web
*/
function btnConcluirQuestionario() {
    try {
        var ObjetoDeValidacao = validarInputs();
        if (ObjetoDeValidacao.validador) {
            /**
            -----MSTECH-----
             *Abaixo duas validações simples para o questionário do tipo Controle da Prova. Se alguma
             das situações abaixo ocorrer, o método é interrompido junto a uma mensagem de erro.

             *1- Se existem problemas na validação de cadernos reservas quando utilizados;
             *2- Se nenhuma disciplina for selecionada.
            */
            if (questionarioId_atual == 8) { //Controle/Acompanhamento da prova
                //Valida o(s) código(s) de barras informado(s), e o(s) código(s) EOL do(s) caderno(s) reserva(s)
                if (!validarCadernosReservas()) {
                    return;
                }

                if (!($("#Questionario_8_Questao_3_Portugues").attr('checked') == "checked" ||
                    $("#Questionario_8_Questao_3_Matematica").attr('checked') == "checked" ||
                    $("#Questionario_8_Questao_3_Ciencias").attr('checked') == "checked")) {
                    sweetAlert("Disciplina não informada", "Por gentileza selecione a disciplina antes de concluir.", "error");
                    return;
                }
            }

            /**
            -----MSTECH-----
             *Abaixo uma validação simples para o questionário do tipo Ficha de registro - Supervisor escolar
             *É obrigatório, para o tipo 9 de questionário, informar a escola
            */
            if (questionarioId_atual == 9) {//Ficha de Registro - Aplicador(a) de Prova
                if ($("#ddlEscola").val() == "") {
                    ProvaSP_Erro("Escola não informada", "Por gentileza selecione a Escola antes de concluir.");
                    return;
                }
            }

            /**
            -----MSTECH-----
             *Dispara um loading antes de invocar o método de envio do Mobile App ou do ProvaSP Web.
            */
            $.mobile.loading("show", {
                text: "Aguarde...", textVisible: true, theme: "a", html: ""
            });
            if (mobile) { salvarQuestionarioLocal(); }
            else { enviarQuestionarioOnline(); }
        }
        else {
            var swalMsg = { titulo: "", texto: "" };
            var itemDeValidacao = ObjetoDeValidacao.item;

            if (itemDeValidacao == "") {
                //Erro genérico
                swalMsg.titulo = "Dados inválidos";
                swalMsg.texto = "Ocorreu um erro ao validar os dados. Por favor tente novamente mais tarde.";
            }
            else if (itemDeValidacao == "15_AB") {
                //Erro de item 15 do questionário do Diretor
                swalMsg.titulo = "Item 15";
                swalMsg.texto = "A soma dos elementos do Item 15 tem que ser, obrigatoriamente, 100%.";
            }
            else {
                if (Usuario.Coordenador) {
                    //Erro Coordenador, tendo em vista que os itens podem ter índices diferentes
                    swalMsg.titulo = "Item inválido";
                    swalMsg.texto = "Por favor verifique os itens com entrada numérica e tente novamente.";
                }
                else {
                    //Erro em item específico
                    swalMsg.titulo = "Item " + itemDeValidacao + " inválido";
                    swalMsg.texto = "Por favor verifique o item " + itemDeValidacao + " e tente novamente.";
                }
            }
            swal(swalMsg.titulo, swalMsg.texto, "warning");
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Novo método para validação dos itens com input number nos questionários.
*/
function validarInputs() {
    try {
        /*
         * -----AMCOM-----
         * Na versão 2k19 o questionário de diretor não terá questões numéricas
         */
        /*if (questionarioId_atual == 24) { //DIRETOR
            //Items fixados no aplicativo para acesso também Offline
            var inputsDiretor = [
                { qtID: "3", max: 50 }, { qtID: "4", max: 50 }, { qtID: "6", max: 50 },
                { qtID: "15_A", max: 100 }, { qtID: "15_B", max: 100 }
            ];
            var qt15A = parseInt($("#Questionario_24_Questao_15_A").val());
            var qt15B = parseInt($("#Questionario_24_Questao_15_B").val());

            //Validação simples de todos os itens do questionário de diretor
            for (var i = 0; i < inputsDiretor.length; i++) {
                var itemDiretorAtual = $("#Questionario_24_Questao_" + inputsDiretor[i].qtID).val();

                if (itemDiretorAtual != "") {
                    var itemDiretorInt = parseInt(itemDiretorAtual);

                    //Valores do input fora dos limites estabelecidos pelo App
                    if (itemDiretorInt < 0 || itemDiretorInt > inputsDiretor[i].max) {
                        return { validador: false, item: inputsDiretor[i].qtID }
                    }
                }
            }

            //Validação específica do item 15. Ícones A e B não podem ultrapassar 100% somados.
            if (qt15A + qt15B != 100) {
                return { validador: false, item: "15_AB" }
            }
            return { validador: true, item: "" }
        }*/
        if (questionarioId_atual == 3) { //COORDENADOR
            //Items fixados no aplicativo para acesso também Offline
            var inputsCoordenador = [
                { qtID: "4", min: 0, max: 50 }, { qtID: "5", min: 0, max: 50 },
                { qtID: "6", min: 0, max: 50 }, { qtID: "8", min: 0, max: 50 }
            ];

            //Validação simples de todos os itens do questionário de coordenador
            for (var j = 0; j < inputsCoordenador.length; j++) {
                var itemCoordenadorAtual = $("#Questionario_3_Questao_" + inputsCoordenador[j].qtID).val();

                if (itemCoordenadorAtual != "") {
                    var itemCoordenadorInt = parseInt(itemCoordenadorAtual);

                    //Valores do input fora dos limites estabelecidos pelo App
                    if (itemCoordenadorInt < inputsCoordenador[j].min ||
                        itemCoordenadorInt > inputsCoordenador[j].max) {
                        return { validador: false, item: inputsCoordenador[j].qtID }
                    }
                }
            }
            return { validador: true, item: "" }
        }
        else {
            return { validador: true, item: "" };
        }
    }
    catch (error) {
        console.log(error);
    }
    return { validador: false, item: "" };
}

/**
-----MSTECH-----
 *Fim do Módulo 3 - Questionários
*/

/**
-----MSTECH-----
 *Módulo 4 - Manipulação da UI
 *Este módulo é formado por apenas 2 métodos. No entanto, tais método são absurdamente grandes e
 difíceis de rastrear. Portante, adicionaremos submódulos sempre que necessário.
*/

/**
-----MSTECH-----
 Módulo 4.1: Controles na ocasião de verificar os resultados da prova

 *Este método irá tratar os filtros para obtenção dos resultados da ProvaSP.
 *Este método é disparado em diversos pontos do código. Portante ele serve para
 organizar os elements de seleção dos resultados de acordo com as escolhas do usuário.
 *
*/
function resultado_configurarControles() {
    try {
        var btnResultadoDesabilitado = true;
        var btnParticipacaoDesabilitado = true;

        /**
        -----MSTECH-----
         *Estes quatro primeiros elementos armazenam os valores dos respectivos SELECTS (ComboBox)
         *
        */
        var nivel = $("#ddlResultadoNivel").val();
        var edicao = $("#ddlResultadoEdicao").val();
        var areaConhecimento = $("#ddlResultadoAreaConhecimento").val();
        var cicloAprendizagem = $("#ddlResultadoCiclo").val();
        var ano = $("#ddlResultadoAno").val();
        var cicloTD = document.getElementById("resultado_selectCiclo");

        /**
        -----MSTECH-----
         *Em seguida são obtidos todos os elements de DREs, Escolsas, Turmas e Alunos selecionados pelo
         usuário em forma de checkbox.
         *Portanto teremos:
         -Nível, edição, área de conhecimento e ano: Valores únicos;
         -DREs, Escolas, Turmas e Alunos: Vetores.
         *
        */
        var dres = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get();
        var escolas = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get();
        var turmas = $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get(); //$("#txtResultadoTurma").val();
        var alunos = $(".resultado-aluno-item-chk:checked").map(function () { return this.value; }).get();

        /**
        -----MSTECH-----
         *Aqui todos os elementos de escolha para o resultado da ProvaSP são desabilitados e
         resetados, com exceção do primeiro elemento: nível.

         *OBS: Reparar que a div que de fato comporta os resultados (divResultadoApresentacao)
         também é escondida neste momento
        */
        $("#ddlResultadoEdicao").selectmenu("disable");
        $("#ddlResultadoEdicao").selectmenu("refresh");

        $("#ddlResultadoAreaConhecimento").selectmenu("disable");
        $("#ddlResultadoAreaConhecimento").selectmenu("refresh");

        $("#ddlResultadoCiclo").selectmenu("disable");
        $("#ddlResultadoCiclo").selectmenu("refresh");

        $("#ddlResultadoAno").selectmenu("disable");
        $("#ddlResultadoAno").selectmenu("refresh");

        $("#divResultadoDRE").hide();
        $("#divResultadoEscola").hide();
        $("#divResultadoTurma").hide();
        $("#divResultadoAluno").hide();

        //$("#divResultadoApresentacao").hide();
        $("#ddlResultadoAno option").show();

        /**
        -----MSTECH-----
         *Se a edição selecionada é um edição com turmas amostrais de 2017 (edições mais recentes),  o nível
         selecionado dos resultados é ESCOLA e os anos são 4º, 6º ou 8º, reseta o select de ano e esconde
         tais turmas.
        */
        if (edicoesComTurmasAmostrais.indexOf(edicao) >= 0 && edicao == 2017) {
            if (nivel == "ESCOLA") {
                if (ano == "4" || ano == "6" || ano == "8") {
                    $("#ddlResultadoAno").val("");
                    $("#ddlResultadoAno").trigger("change");
                }
                $("#ddlResultadoAnoItem_Ano4").hide();
                $("#ddlResultadoAnoItem_Ano6").hide();
                $("#ddlResultadoAnoItem_Ano8").hide();
            }
        }

        /**
        -----AMCOM-----
         *Sendo ENTURMACAO_ATUAL a edição escolhida junto ao 2º ano, deve-se resetar o select
         e esconder a opção do 2º ano. Quando é ciências também esconde a opção do 2º ano.
        */
        if (edicao == "ENTURMACAO_ATUAL") {
            if (ano == "2") {
                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").trigger("change");
            }
            $("#ddlResultadoAnoItem_Ano2").hide();
        } else if (areaConhecimento == "1") { //Ciências
            $("#ddlResultadoAnoItem_Ano2").hide();
        }
        else {
            $("#ddlResultadoAnoItem_Ano2").show();
        }

        /**
        -----MSTECH-----
         *As validações abaixo verificam se o botão para mostrar os resultados deve ser habilitado de
         acordo com as escolhas.
         *Essa validação é necessária pois os nível de escolha necessários para verificar resultados das
         provas varia de um tipo para outro de usuário ou mesmo de um nível selecionado para o outro.

         -POR EXEMPLO (usuário): Um usuário do tipo "nível SME" (permissões totais do sistema)
         terá acesso ao possibilidades de resultados muito mais abrangestes que um docente.
         -POR EXEMPLO (nível): Em nível SME (opção do select), o usuário verifica comparativos gerais
         entre DRES nas ProvaSP. Ao selecionar TURMA, o usuário poderá ver o resultado de um aluno específico.
        */
        if (nivel == "SME") {
            //MSTECH - Melhorando desempenho do App na troca de filtros
            $("#divResultadoEscolaItens").html("");
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            /**
            -----MSTECH-----
             *Em nível SME é necessário pelo menos:
             -Selecionar uma edição;
             -Selecionar uma área de conhecimento;
             -Selecionar um ano.
            */
            if (edicao != "" && areaConhecimento != "" && ano != "") {
                btnResultadoDesabilitado = false;
                btnParticipacaoDesabilitado = false;
            }
        }
        else if (nivel == "DRE") {
            //MSTECH - Melhorando desempenho do App na troca de filtros
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            if (edicao != "" && areaConhecimento != "" && ano != "" && dres.length > 0) {
                btnResultadoDesabilitado = false;
                btnParticipacaoDesabilitado = false
            }
            /**
            -----MSTECH-----
             *Em nível DRE é necessário pelo menos:
             -Idem SME
             -Selecionar N DREs em forma de checkbox.
            */
        }
        else if (nivel == "ESCOLA") {
            //MSTECH - Melhorando desempenho do App na troca de filtros
            $("#divResultadoAlunoItens").html("");

            if (edicao != "" && areaConhecimento != "" &&
                (ano != "" || cicloAprendizagem != "") && dres.length > 0 && escolas.length > 0) {
                /**
                -----MSTECH-----
                 *Em nível ESCOLA é necessário pelo menos:
                 -Idem DRE
                 -Selecionar N Escolas em forma de checkbox.
                */
                btnResultadoDesabilitado = false;
                btnParticipacaoDesabilitado = false;
            }
        }
        else if (nivel == "TURMA" && edicao != "" && areaConhecimento != "" &&
            ano != "" && dres.length > 0 && escolas.length > 0 && turmas.length > 0) {
            /**
            -----MSTECH-----
             *Em nível TURMA é necessário pelo menos:
             -Idem ESCOLA
             -Selecionar N Turmas em forma de checkbox.
            */
            btnResultadoDesabilitado = false;
            btnParticipacaoDesabilitado = false;
        }
        else if (nivel == "ALUNO" && edicao != "" && areaConhecimento != "" &&
            ano != "" && dres.length > 0 && escolas.length > 0 && alunos.length > 0) {
            /**
            -----MSTECH-----
             *Em nível ALUNO é necessário pelo menos:
             -Idem TURMA
             -Selecionar N Alunos em forma de checkbox.
            */
            btnResultadoDesabilitado = false;
        }

        /**
        -----MSTECH-----
         *SELEÇÃO NÍVEL (ATUALIZAÇÃO DO TEXTO DA OPTION TURMA)
         *Ao selecionar a edicao HISTORICO tendo selecionado anteriormente a opção TURMA, o App
         mostra um avisa sobre a impossibilidade de mostrar os detalhes da prova por turma detalhando
         o desempenho dos alunos.
         *Muda-se inclusive o texto do SELECT, caso a situação acima aconteça.

         OBS: Muito provavelmente esta informação não existe ou não é trivial obtê-la
        */
        if (edicao == "HISTORICO") {
            if (nivel == "TURMA" && $("#ddlResultadoNivel_optionTurma").html() == "Turma detalhando Alunos") {
                swal("Detalhamento de alunos", "O detalhamento de alunos por turma não será apresentado no modo Histórico.", "warning");
            }
            $("#ddlResultadoNivel_optionTurma").html("Turma");
        }
        else {
            $("#ddlResultadoNivel_optionTurma").html("Turma detalhando Alunos");
        }
        $("#ddlResultadoNivel").selectmenu("refresh");

        /**
        -----MSTECH-----
         *As validações abaixo são para a situação do usuário escolher a opção padrão dos SELECTS.
         *Neste app do ProvaSP, a opção padrão é vazia e deve resetar as opções subsequentes, como
         se a seleção fosse desfeita.

         *Reparar ainda que os trecho a seguir desfazer o trecho inicial do método, o qual
         desabilita todos os elementos dos filtros.
        */
        //VISIBILIDADE SELEÇÃO EDIÇÃO
        if (nivel != "") {
            $("#ddlResultadoEdicao").selectmenu("enable");
            $("#ddlResultadoEdicao").selectmenu("refresh");
        }

        //VISIBILIDADE SELEÇÃO ÁREA DE CONHECIMENTO e CICLO DE APRENDIZAGEM
        if (nivel != "" && edicao != "") {
            if (cicloTD.style.display == "table-row") {
                $("#ddlResultadoCiclo").selectmenu("enable");
            }
            else {
                $("#ddlResultadoAreaConhecimento").selectmenu("enable");
            }
        }

        if (nivel != "" && edicao != "") {
            if (cicloAprendizagem != "") {
                $("#ddlResultadoAreaConhecimento").selectmenu("enable");

                if (nivel == "SME") {
                    if (areaConhecimento != "") {
                        btnResultadoDesabilitado = false;
                    }
                }
            }
            else if (areaConhecimento != "") {
                $("#ddlResultadoAno").selectmenu("enable");
            }
        }

        /**
        -----MSTECH-----
         *VISIBILIDADE SELEÇÃO DRE
         *Esta validação acontece quando Nível, Edição, Área de Conhecimento e Ano já foram selecionados e
         dentre as opções de Nível, a escolhida foi: DRE/ESCOLA/TURMA/ALUNO, excluindo-se SME
        */
        if ((nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO") &&
            edicao != "" && areaConhecimento != "" && (ano != "" || cicloAprendizagem != "")) {
            $("#divResultadoDRE").show();

            /**
            -----MSTECH-----
             *Com exceção dos usuários nível SME, todos os outros terão acesso aos resultados dessa
             filtragem contanto que tenham a permissão necessária no objeto de usuário, mais
             especificamente no atributo de grupos

             OBS: Perceber este trecho consome a informação de grupos do usuário, presente também
             no arquivo loginOffline.json
            */
            if (!Usuario.AcessoNivelSME) {
                /**
                -----MSTECH-----
                 *PERMISSÃO DE VISIBILIDADE PARA CADA DRE:
                 *Fluxo do código:
                 -Esconde os labels das opções;
                 -Mostra a div geral;
                 -Se o usuário possui grupos associados, mostra as labels correspondentes e,
                 consequentemente, a opção checkBox correspondente.
                */
                $(".resultado-dre-chk").parent().hide();
                $("#chkResultadoTodasDREs").show();
                for (var i = 0; i < Usuario.grupos.length; i++) {
                    var uad_sigla = Usuario.grupos[i].uad_sigla;

                    if (uad_sigla != null && uad_sigla != "") {
                        $('#divResultadoDRE label[for="chkResultado' + uad_sigla + '"]').parent().show();
                    }
                }
            }
        }

        /**
        -----MSTECH-----
         *O tratamentos abaixo determinam a visibilidade de conjuntos de CheckBoxes de acordo com as
         opções escolhidas previamente. Veja:
         -VISIBILIDADE SELEÇÃO ESCOLA
         -Para escolher escolas, por exemplo, é necessário escolher o nível ESCOLA, TURMA ou ALUNO
         -Além disso, deve-se escolher uma edição, um ano e pelo menos uma DRE.
         -O mesmo para TURMAS e ALUNOS, com exceção do nível mais específico e necessidade de escolher
         pelo menos uma escola no caso de turmas e pelo menos uma turma no caso de alunos.
        */
        if ((nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO") &&
            edicao != "" && (ano != "" || cicloAprendizagem != "") && dres.length > 0) {
            $("#divResultadoEscola").show();
        }

        //VISIBILIDADE SELEÇÃO TURMA
        if ((nivel == "TURMA" || nivel == "ALUNO") &&
            edicao != "" && ano != "" && dres.length > 0 && escolas.length > 0) {
            $("#divResultadoTurma").show();
        }

        //VISIBILIDADE SELEÇÃO ALUNO
        if (nivel == "ALUNO" && edicao != "" && ano != "" && dres.length > 0 &&
            escolas.length > 0 && turmas.length > 0) {
            $("#divResultadoAluno").show();
        }


        /**
        -----MSTECH-----
         *Os trecho abaixo são bem simples e triviais. Os tratamentos existem para que os elementos
         da UI não apareçam desnecessariamente.
         *Basicamente os métodos determinam quando mostrar labels que indicam quantas DRES, Escolas ou Alunos
         foram selecionados. Param tanto, as opções corretas devem ter sido selecionadas.

         OBS: Em todas as situações, o texto da label é zerado a princípio
        */

        /**
        -----MSTECH-----
         *Quantidade de DREs selecionadas
        */
        $("#lblResultadoSelecaoTotalDRE").text("");
        if (dres.length > 1 &&
            (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO")) {
            $("#lblResultadoSelecaoTotalDRE").text("Total de DREs: " + dres.length);
        }

        /**
        -----MSTECH-----
         *Quantidade de Escolas selecionadas
        */
        $("#lblResultadoSelecaoTotalEscola").text("");
        if (escolas.length > 1 &&
            (nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO")) {
            $("#lblResultadoSelecaoTotalEscola").text("Total de Escolas: " + escolas.length);
        }

        /**
        -----MSTECH-----
         *Quantidade de TURMAS selecionadas
        */
        $("#lblResultadoSelecaoTotalTurma").text("");
        if (turmas.length > 1 && (nivel == "TURMA" || nivel == "ALUNO")) {
            $("#lblResultadoSelecaoTotalTurma").text("Total de Turmas: " + turmas.length);
        }

        /**
        -----MSTECH-----
         *Quantidade de ALUNOS selecionados
        */
        $("#lblResultadoSelecaoTotalAluno").text("");
        if (alunos.length > 1 && nivel == "ALUNO") {
            $("#lblResultadoSelecaoTotalAluno").text("Total de Alunos: " + alunos.length);
        }

        /**
        -----MSTECH-----
         *Finalmente manipula-se o botão de mostrar resultados de acordo com os tratamentos feitos
         acima.
        */
        $("#btnResultadoApresentar").prop("disabled", btnResultadoDesabilitado);

        //MSTECH - Não mostrar opção de participação quando for filtragem por ciclo
        if (cicloTD.style.display == "table-row") { btnParticipacaoDesabilitado = true; }
        $("#btnParticipacaoApresentar").toggle(!btnParticipacaoDesabilitado);
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----AMCOM-----
 Módulo 4.1: Controles na ocasião de verificar os revistasBoletins da prova

 *Este método irá tratar os filtros para obtenção dos revistasBoletins da ProvaSP.
 *Este método é disparado em diversos pontos do código. Portante ele serve para
 organizar os elements de seleção dos revistasBoletins de acordo com as escolhas do usuário.
 *
*/
function revistasBoletins_configurarControles() {
    try {
        /**
        -----AMCOM-----
         *Estes quatro primeiros elementos armazenam os valores dos respectivos SELECTS (ComboBox)
         *
        */
        var edicao = $("#ddlRevistasBoletinsEdicao").val();
        var areaConhecimento = $("#ddlRevistasBoletinsAreaConhecimento").val();
        var cicloAprendizagem = $("#ddlRevistasBoletinsCiclo").val();

        if (edicao && edicoesRevistasPedagogicas.includes(edicao)) {
            $("#ddlRevistasBoletinsCiclo").selectmenu("enable");
        } else {
            $("#ddlRevistasBoletinsCiclo").selectmenu("disable");
        }
        $("#ddlRevistasBoletinsCiclo").selectmenu("refresh");

        /**
        -----AMCOM-----
         *Em seguida são obtidos todos os elements de DREs, Escolsas, Turmas e Alunos selecionados pelo
         usuário em forma de checkbox.
         *Portanto teremos:
         -Nível, edição, área de conhecimento e ano: Valores únicos;
         -DREs, Escolas, Turmas e Alunos: Vetores.
         *
        */
        var qtdDRES = $(".revistasBoletins-dre-item-chk:checked").length;

        $("#divRevistasBoletinsDRE").hide();
        $("#divRevistasBoletinsEscola").hide();

        /**
        -----AMCOM-----
         *VISIBILIDADE SELEÇÃO DRE
         *Esta validação acontece quando Nível, Edição, Área de Conhecimento e Ano já foram selecionados e
         dentre as opções de Nível, a escolhida foi: DRE/ESCOLA/TURMA/ALUNO, excluindo-se SME
        */
        if (edicao != "" && areaConhecimento != "" && (!edicoesRevistasPedagogicas.includes(edicao) || cicloAprendizagem != "")) {
            $("#divRevistasBoletinsDRE").show();

            /**
            -----AMCOM-----
             *Com exceção dos usuários nível SME, todos os outros terão acesso aos revistasBoletins dessa
             filtragem contanto que tenham a permissão necessária no objeto de usuário, mais
             especificamente no atributo de grupos

             OBS: Perceber este trecho consome a informação de grupos do usuário, presente também
             no arquivo loginOffline.json
            */
            if (!Usuario.AcessoNivelSME) {
                /**
                -----AMCOM-----
                 *PERMISSÃO DE VISIBILIDADE PARA CADA DRE:
                 *Fluxo do código:
                 -Esconde os labels das opções;
                 -Mostra a div geral;
                 -Se o usuário possui grupos associados, mostra as labels correspondentes e,
                 consequentemente, a opção checkBox correspondente.
                */
                $(".revistasBoletins-dre-chk").parent().hide();
                $("#chkRevistasBoletinsTodasDREs").show();
                for (var i = 0; i < Usuario.grupos.length; i++) {
                    var uad_sigla = Usuario.grupos[i].uad_sigla;

                    if (uad_sigla != null && uad_sigla != "") {
                        $('#divRevistasBoletinsDRE label[for="chkRevistasBoletins' + uad_sigla + '"]').parent().show();
                    }
                }
            }

            /**
            -----AMCOM-----
             *O tratamentos abaixo determinam a visibilidade de conjuntos de CheckBoxes de acordo com as
             opções escolhidas previamente. Veja:
             -VISIBILIDADE SELEÇÃO ESCOLA
             -Para escolher escolas, por exemplo, é necessário escolher o nível ESCOLA, TURMA ou ALUNO
             -Além disso, deve-se escolher uma edição, um ano e pelo menos uma DRE.
            */
            if (qtdDRES > 0) {
                $("#divRevistasBoletinsEscola").show();
            }
        }

        /**
        -----AMCOM-----
         *Os trecho abaixo são bem simples e triviais. Os tratamentos existem para que os elementos
         da UI não apareçam desnecessariamente.
         *Basicamente os métodos determinam quando mostrar labels que indicam quantas DRES, Escolas ou Alunos
         foram selecionados. Param tanto, as opções corretas devem ter sido selecionadas.

         OBS: Em todas as situações, o texto da label é zerado a princípio
        */

        /**
        -----AMCOM-----
         *Quantidade de DREs selecionadas
        */
        $("#lblRevistasBoletinsSelecaoTotalDRE").text("DREs selecionadas: " + qtdDRES);
    }
    catch (error) {
        console.log(error);
    }
}

function carregarListaEscolaRevistasBoletins() {
    try {
        /**
        -----AMCOM-----
         *Limpa HTMLs que contêm as opções de seleção de Escolas, Turmas e Alunos
         *Em seguida executa o método geral de reset.
        */
        $("#divRevistasBoletinsEscolaItens").html("");
        $("#txtRevistasBoletinsEscolaFiltro").val("");

        revistasBoletins_configurarControles();

        /**
        -----AMCOM-----
         *Seleciona através dos Checks as escolas selecionadas.
         *Se o nível selecionado for DRE o botão de revistasBoletins é desabilitados
         *Senão, a flag apresentar escolas é setada como TRUE.
         Ou seja, quando o nível é DRE, não é necessário selecionar uma escola e os revistasBoletins
         são mostrados de acordo com as DREs selecionadas.
        */
        var DREs_selecionadas = $(".revistasBoletins-dre-chk:checked").map(function () { return this.value; }).get();

        $("#lblRevistasBoletinsSelecaoTotalEscola").text("Escolas:");
        if (DREs_selecionadas.length >= 1) {
            ///////////////////////////////////////////////////////////////////////////////
            //Carregar escolas das DREs selecionadas:

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            /**
            -----AMCOM-----
             *Para carregar as escolas com base nas DREs escolhidas, vamos novamente carregar
             o arquivo escolas.CSV e, a partir dele, obter as informações necessárias.
            */
            let promiseCarga = carregarDataEscola(
                new Promise ((resolve, reject) => {
                    setTimeout(function () {
                        try {
                            /**
                            -----AMCOM-----
                             *Reparar que apenas usuários autorizados podem ver os revistasBoletins
                             das respectivas escolas.
                            */
                            let codigoEscolasAutorizadas = [];
                            for (let i = 0; i < Usuario.grupos.length; i++) {
                                if (Usuario.grupos[i].AcessoNivelEscola) {
                                    codigoEscolasAutorizadas.push(Usuario.grupos[i].esc_codigo);
                                }
                            }

                            let escolasEncontradas = 0;
                            let htmlListaEscolas = "";

                            /**
                            -----AMCOM-----
                             *Percorrendo escolas para adicionar à lista para seleção.
                             *Perceber que se o usuário for nível SME ou DRE ele poderá ver as escolas
                             sem restrições de grupo.
                            */
                            for (let i = 1; i < dataEscola.length; i++) {
                                let r = dataEscola[i].split(";");
                                //uad_sigla; esc_codigo; esc_nome
                                let uad_sigla = r[0];
                                let esc_codigo = r[1];
                                let esc_nome = r[2];


                                //PERMISSÃO DE VISIBILIDADE PARA A ESCOLA:
                                let incluirEscola =
                                    (Usuario.AcessoNivelSME ||
                                        Usuario.AcessoNivelDRE ||
                                        codigoEscolasAutorizadas.indexOf(esc_codigo) >= 0);

                                /**
                                -----AMCOM-----
                                 *Se o usuário pode acessar os dados da escola, incrementamos escolasEncontradas
                                 e montamos um elemento HTML na lista de escolas a serem selecionadas.
                                */
                                if (incluirEscola
                                    && (DREs_selecionadas.indexOf("TD") >= 0 || DREs_selecionadas.indexOf(uad_sigla) >= 0)) {

                                    escolasEncontradas++;
                                    let lblID = "lblRevistasBoletinsEscola_" + esc_codigo;
                                    htmlListaEscolas += `<div id='${lblID}' class='revistasBoletins-escola-lbl ui-btn ui-corner-all ui-btn-inherit' style='text-align: left;'>${esc_nome}`
                                        + `<a href='#' onclick='abrirLinkRevistaBoletim(this);' data-esc_codigo='${esc_codigo}' target='_blank' class='revistasBoletins-escola-lnk'>Abrir</a>`
                                        + "</div>";
                                }
                            }
                            $("#divRevistasBoletinsEscolaItens").html("");
                            //faz apenas um append
                            $("#divRevistasBoletinsEscolaItens").append(htmlListaEscolas);

                            /**
                            -----AMCOM-----
                             *Havendo mais de uma escola disponível, mostrar-se-á as estruturas
                             para seleção de escolas (filtro de escolas).
                             *Além disso, cada elemento de escola a ser selecionado receberá um evento
                             correspondente ao reset das informações dos filtros.
                            */
                            if (escolasEncontradas > 0) {
                                //$("#divRevistasBoletinsEscola").show();
                                $("#divRevistasBoletinsEscolaItens").trigger("create");

                                $("#lblRevistasBoletinsSelecaoTotalEscola").text(`Escolas (${escolasEncontradas}):`);
                            }
                            $.mobile.loading("hide");
                            resolve();
                        }
                        catch (error) {
                            console.log(error);
                        }
                    }, 100);
                    //$("#ddlRevistasBoletinsEscola").selectmenu("refresh", true);
                })/*fim promise*/, ""
            );

            /**
            -----AMCOM-----
             *Este evento serve para filtrar as escolas selecionadas acima. Ou seja, da listagem de
             escolas que será mostrada, poderemos escrever uma palavra para filtrar os registros
             de acordo com a necessidade.
            */
            $("#txtRevistasBoletinsEscolaFiltro").unbind("change").change(function () {
                try {
                    /**
                    -----AMCOM-----
                     *Se o filtro for vazio, mostra todas as escolas selecionadas.
                     *Reparar que mostra também a opção padrão TODAS AS ESCOLAS
                    */
                    var valorFiltro = $("#txtRevistasBoletinsEscolaFiltro").val().trim().toUpperCase();
                    if (valorFiltro == "") {
                        $(".revistasBoletins-escola-lbl").show();
                        $("#lblRevistasBoletinsTodasEscolas").show();
                    }
                    else {
                        /**
                        -----AMCOM-----
                         *Esconde a opção padrão TODAS AS ESCOLAS, bem como os registros das escolas
                         selecionadas. Depois disso, filtra as escolas encontradas pela estrutura
                         de buscas e as mostra novamente.
                        */
                        $("#lblRevistasBoletinsTodasEscolas").hide();
                        $(".revistasBoletins-escola-lbl").hide();
                        $(".revistasBoletins-escola-lbl").filter(
                            function () {
                                return $(this).text().toUpperCase().indexOf(valorFiltro) >= 0;
                            }
                        ).show();
                        $("#divRevistasBoletinsEscolaItens").trigger("create");
                    }
                    setTimeout(function () { $.mobile.loading('hide'); }, 100);
                }
                catch (error) {
                    console.log(error);
                }
            });

            return promiseCarga;
        }
    }
    catch (error) {
        console.log(error);
    }
    return null;
}

function abrirLinkRevistaBoletim(source) {
    let esc_codigo = source.dataset.esc_codigo;

    let edicao = $("#ddlRevistasBoletinsEdicao").val();

    let compAreaConhecimento = document.querySelector("#ddlRevistasBoletinsAreaConhecimento");
    let idAreaConhecimento = compAreaConhecimento.value;
    let descAreaConhecimento = compAreaConhecimento.options[compAreaConhecimento.selectedIndex].text;

    let compCiclo = document.querySelector("#ddlRevistasBoletinsCiclo");
    let descCicloAprendizagem = compCiclo.options[compCiclo.selectedIndex].text;

    let url;
    if (edicao && edicoesRevistasPedagogicas.includes(edicao)) {
        //REVISTA
        let urlRevista = provaSP_configuracoes.configuracoes.UrlImagemAlunos
            + "Revistas Pedagógicas/" + descAreaConhecimento
            + "/Ciclo " + descCicloAprendizagem
            + "/" + parseInt(esc_codigo) + ".pdf";

        url = encodeURI(urlRevista);
    } else {
        //BOLETIM
        if (idAreaConhecimento == 4)
            idAreaConhecimento = 2;//Redação (4) está junto com Lingua Portuguesa (2)
        let urlBoletim = urlBackEnd + "boletim_escola/" + edicao
            + "/" + idAreaConhecimento
            + "/" + esc_codigo + ".pdf";

        url = encodeURI(urlBoletim);
    }

    //antes de abrir a URL valida se existe o arquivo no servidor
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();

    if (http.status == 404) {
        swal("Atenção!", "Não foi possível encontrar a Revista Pedagógica/Boletim para esta escola.", "warning");
        return;
    }

    let win = window.open(url, '_blank');
    win.focus();
}

/**
-----MSTECH-----
 Módulo 4.2: Definição de handlers e eventos do App

 *Reparar que TODAS as determinações de eventos Click ou Change têm uma atribuição "unbind". Isso
 quer dizer que, a cada execução do definirEventHandlers o evento anteriormente setado é perdido,
 dando origem a um novo que será determinado na ocasião da execução atual.

 -Módulo 4.2.1 - Botões para OBTER RESULTADOS
 -Módulo 4.2.2 - Botões para tratamento dos filtros de obtenção de resultados
 -Módulo 4.2.3 - Método para apresentação dos resultados, montagem de gráficos e manipulação
 -Módulo 4.2.4 - Métodos e eventos associados ao ProvaSP de fato, questionário do tipo 8.
 -Módulo 4.2.5 - Eventos dos botões de questionários
 -Módulo 4.2.6 - Botões para tratamento dos filtros de obtenção de revistas pedagógicas e boletins
*/
function definirEventHandlers() {
    /**
    -----MSTECH-----
     Módulo 4.2.1 - Botões para OBTER RESULTADOS
    */

    /**
    -----MSTECH-----
     *Retorna da selação de resultado da ProvaSP por parte do Aluno.
    */
    $("#btnAlunoVoltar").unbind("click").click(function () {
        try {
            swal({
                title: "Deseja realmente voltar ao menu?",
                type: "warning",
                showCancelButton: true,

                confirmButtonText: "Sim",
                cancelButtonText: "Não",
                closeOnConfirm: false
            },
                function () {
                    //window.location = "menu.html";
                    removerItemBackButton();
                    $(".page").hide();
                    $("#menu-page").show();
                    $("#divCodigoTurma").hide();
                    $("#divMenuPrincipal").show();
                    swal.close();
                });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Novo botão para tela de configurações
    */
    $("#btnConfiguracoes").unbind("click").click(function () {
        try {
            if (mobile && (navigator.connection.type == Connection.NONE ||
                navigator.connection.type == Connection.UNKNOWN)) {
                ProvaSP_Erro("Alerta",
                    "Não há conexão com a internet.\n\nVerifique sua conexão antes de atualizar as configurações.");
            }
            else {
                if (Usuario.AcessoNivelSME) {
                    $.mobile.loading("show", {
                        text: "Obtendo configurações atualizadas...",
                        textVisible: true,
                        theme: "a",
                        html: ""
                    });

                    //Tentando obter as configurações mais recentes
                    $.ajax({
                        url: urlBackEnd + "api/RetornarAppJson",
                        type: "GET",
                        dataType: "JSON",
                        crossDomain: true,
                        cache: false,
                        success: function (data) {
                            provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas =
                                (data.DisponibilizarPreenchimentoQuestionariosFichas === 'true');

                            provaSP_configuracoes.configuracoes.RelatorioAcompanhamentoVisivel =
                                (data.RelatorioAcompanhamentoVisivel === 'true');

                            provaSP_configuracoes.configuracoes.UrlImagemAlunos =
                                data.UrlImagemAlunos;

                            //Usando informações de configuração mais recentes (caso outro usuário
                            //tenha alterado)
                            direcionarTelaConfiguracoes();
                        },
                        error: function (erro) {
                            /**
                            -----MSTECH-----
                             *Erro ao obter configuração atualizada. Usar informações de
                             configurações obtidas inicialmente
                            */
                            direcionarTelaConfiguracoes();
                            ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                        }
                    });
                }
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Novo botão para encerramento da tela de configurações. Ao encerrar as edições na tela de
     configuração, deve ser possível enviar os dados ao servidor.
    */
    $("#btnSincronizarOpcoes").unbind("click").click(function () {
        try {
            $.mobile.loading("show", {
                text: "Atualizando configurações...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            //Executar o POST apenas se houver uma atualização em relação ao objeto inicial.
            if (JSON.stringify(provaSP_configuracoes.dadosAtuais) !=
                JSON.stringify(provaSP_configuracoes.configuracoes)) {
                //Objeto de sincronização
                var syncOBJ = [
                    {
                        Chave: "DisponibilizarPreenchimentoQuestionariosFichas",
                        Valor: provaSP_configuracoes.configuracoes.
                            DisponibilizarPreenchimentoQuestionariosFichas.toString()
                    },
                    {
                        Chave: "RelatorioAcompanhamentoVisivel",
                        Valor: provaSP_configuracoes.configuracoes.
                            RelatorioAcompanhamentoVisivel.toString()
                    },
                    {
                        Chave: "RepresentatividadeSegundoINEP",
                        Valor: provaSP_configuracoes.configuracoes.
                            RepresentatividadeSegundoINEP.toString()
                    }
                ];

                $.post(urlBackEnd + "api/Configuracao", { json: JSON.stringify(syncOBJ) })
                    .done(function (result) {
                        //Mensagem de sucesso!
                        ProvaSP_Erro("Sucesso!", "Opções da ProvaSP sincronizadas com sucesso.");
                    })
                    .fail(function (erro) {
                        ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                    });
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#btnEnviarProficiencia").unbind("click").click(function () {
        try {
            var anoCiclo = $("#confAnoCiclo").val();
            var proficiencia = $("#confNivelProficiência").val();
            var nomeProficiencia = $("#confProficienciaNome").val();
            var descricaoProficiencia = $("#confProficienciaDescricao").val();

            if (anoCiclo == "") {
                ProvaSP_Erro("Alerta", "Por favor selecione um Ano ou Ciclo de Aprendizagem.");
            }
            else if (proficiencia == "") {
                ProvaSP_Erro("Alerta", "Por favor selecione um nível de proficiência.");
            }
            else if (nomeProficiencia.length == 0) {
                ProvaSP_Erro("Alerta", "Por favor preencha o campo 'Nome da proficiência.'");
            }
            else if (descricaoProficiencia.length == 0) {
                ProvaSP_Erro("Alerta", "Por favor preencha o campo 'Descrição da proficiência.'");
            }
            else {
                //MSTECH - Definindo URL correta de sincronização
                var urlSync = "api/NivelProficienciaAnoEscolar";
                if (anoCiclo.indexOf("c") != -1) {
                    urlSync = "api/NivelProficienciaCiclo";
                    anoCiclo = anoCiclo.replace("c", "");
                }
                //MSTECH - Objeto de envio
                var proficienciaAtualizada = [{
                    NivelProficienciaID: proficiencia,
                    AnoCiclo: anoCiclo,
                    Nome: nomeProficiencia,
                    Descricao: descricaoProficiencia
                }];

                $.mobile.loading("show", {
                    text: "Atualizando proficiência...",
                    textVisible: true,
                    theme: "a",
                    html: ""
                });

                $.post(urlBackEnd + urlSync, { json: JSON.stringify(proficienciaAtualizada) })
                    .done(function (result) {
                        //Mensagem de sucesso!
                        limparCamposConfiguracaoProficiencia();
                        ProvaSP_Erro("Sucesso!", "As informações da Proficiência foram atualizadas com sucesso!");
                    })
                    .fail(function (erro) {
                        ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                    });
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#btnConfiguracoesSair").unbind("click").click(function () {
        try {
            voltarMenu_deConfiguracoes();

            //MSTECH - Atualizar MENU
            if (Usuario.AcessoNivelSME) {
                if (provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas) {
                    $("#provaSP_resultados").hide();
                    $("#provaSP_disponivel").show();
                }
                else {
                    $("#provaSP_disponivel").hide();
                    $("#provaSP_resultados").show();
                }
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Botão responsável por abrir a página de filtragem e obtenção dos resultados do ProvaSP

     *Reparar também que este handler de evento de Click prepara a tela de resultados quando o botão
     btnAbrirResultados for selecionado. Reparar que o método do item 4.1 é o "inverso" deste,
     sendo o responsável por tratar os elementos de filtro e mostrar os resultados.
    */
    $("#btnAbrirResultados").unbind("click").click(function () {
        try {
            if (Usuario.Aluno) {
                adicionarItemBackButton("btnAlunoVoltar");
                $("#btnAlunoVoltar").show();
            }
            else {
                adicionarItemBackButton("btnResultadoFechar");
                $("#btnResultadoFechar").show();
            }
            abrirResultados(true);
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Este handler de evento binda DOIS elementos diferentes. Além disso, o evento é destinado apenas
     à mostragem de resultados do Aluno, que é diferente da mostragem geral e comparativa dos outros
     tipos de usuário.
     *O evento é disparado sempre que o aluno seleciona uma área de conhecimento ou um ano de
     edição do ProvaSP. No entanto, para a chamada ser realizada de fato, ambos os campos devem ser
     preenchidos.
    */
    $("#ddlResultadoAlunoAreaConhecimento,#ddlResultadoAlunoEdicao").unbind("change").change(function () {
        try {
            /**
            -----MSTECH-----
             *Fluxo de código:
             -Obter os valores dos SELECTS ddlResultadoAlunoEdicao e ddlResultadoAlunoAreaConhecimento
             -Limpar o HTML de todos os elementos com style divChartResultadoEscalaSaeb_1
             e divChartResultadoDetalhe;
             -Validar se informações dos select são válidas;
             -Buscar no servidor do ProvaSP os resultados com base no RA do aluno, edição e área
             de conhecimento.
            */
            var Edicao = $("#ddlResultadoAlunoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAlunoAreaConhecimento").val();

            $(".divChartResultadoEscalaSaeb_1").html("");
            $(".divChartResultadoDetalhe").html("");

            if (Edicao != "" && AreaConhecimentoID != "") {
                $.mobile.loading("show", {
                    text: "Aguarde...",
                    textVisible: true,
                    theme: "a",
                    html: ""
                });

                /**
                -----MSTECH-----
                 *Obtendo RA do aluno, Edicao e Área de conhecimento escolhidas.
                */
                var alu_matricula = Usuario.usu_login.replace("RA", "");
                var Edicao = $("#ddlResultadoAlunoEdicao").val();
                var AreaConhecimentoID = $("#ddlResultadoAlunoAreaConhecimento").val();

                /**
                -----MSTECH-----
                 *Requisição para obter resultados da ProvaSP.
                 *Reparar que este é um POST com retorno de informações. Ou seja, é necessário
                 enviar um objeto de entrada para obter o resultado do simulado
                 *Reparar também que este botão obtém o resultado específico do Aluno, apesar de ser
                 uma requisição de POST mais abrangente. Tanto que, a maioria das flags está vazia.
                */
                $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(),
                    {
                        /**
                        -----MSTECH-----
                         *Informações de entrada ALUNO, EDICAO e AREA DE CONHECIMENTO.
                        */
                        Nivel: "ALUNO",
                        Edicao: Edicao,
                        AreaConhecimentoID: AreaConhecimentoID,
                        /**
                        -----MSTECH-----
                         *Informações de entrada vazias.
                        */
                        AnoEscolar: "", lista_uad_sigla: "",
                        lista_esc_codigo: "",
                        lista_turmas: "",
                        lista_alu_matricula: alu_matricula,
                        ExcluirSme_e_Dre: "1"
                    })
                    .done(function (dataResultado) {
                        $.mobile.loading("hide");
                        /**
                        -----MSTECH-----
                         *Apresentação do resultado obtido.

                         Entender como funciona o resultado do PROVASP.
                         RESPONDIDO: Já debugamos e entendemos a criação dos gráficos com o ChartJS
                        */
                        resultadoApresentar(
                            "",
                            $("#ddlResultadoAlunoEdicao").val(),
                            $("#ddlResultadoAlunoAreaConhecimento").val(),
                            dataResultado.AnoEscolar,
                            "divResultadoApresentacaoAluno",
                            dataResultado,
                            {});
                    })
                    .fail(function (erro) {
                        ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                    });
            }
            else {
                /**
                -----MSTECH-----
                 *Aparentemente este ELSE é desnecessário, pois o mesmo trecho é executado antes da
                 validação.
                */
                $(".divChartResultadoDetalhe").html("");
                $(".divChartResultadoEscalaSaeb_1").html("");

                /**
                -----MSTECH-----
                 *Escondendo botão de Prova do Aluno essencialmente. Mas também a Div de resultados do aluno
                */
                $("#divResultadoApresentacaoAluno").hide();
            }
        }
        catch (error) {
            console.log(error);
        }
    });
    /**
    -----MSTECH-----
     *Fim do Módulo 4.2.1 - Botões para OBTER RESULTADOS
    */

    /**
    -----MSTECH-----
     Módulo 4.2.2 - Botões para tratamento dos filtros de obtenção de resultados
     (o que é feito ao selecionar cada opção)
    */

    /**
    -----MSTECH-----
     *Evento CHANGE do select de nível. Sempre que o usuário alterar o NÍVEL na seleção dos filtros de
     resultados, os tratamentos do handler devem ser executados. Basicamente são manipulações de UI.
    */
    $("#ddlResultadoNivel").unbind("change").change(function () {
        try {
            /*
            $(".resultado-dre-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-escola-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-turma-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-aluno-chk").prop('checked', false).checkboxradio('refresh');
            */

            /**
            -----MSTECH-----
             *Fluxo do código:
             -A princípio, esconde as opções das versões legadas (antes de 2012)
             -Tais versões legadas devem estar disponíveis apenas para o filtro de nível ALUNO
             -Se o filtro for ESCOLA, devemos disparar TODOS os eventos dos CHECKS da DREs.
             Desta maneira, todas as escolas serão carregadas em paralelo.
            */
            $(".edicao-resultado-legado").hide();
            $(".edicao-resultado-legado").attr("disabled", "disabled");

            if (this.value == "ALUNO") {
                $(".edicao-resultado-legado").show();
                $(".edicao-resultado-legado").removeAttr("disabled");
            }

            if (this.value == "ESCOLA") {
                //$(".resultado-dre-chk").unbind("change").change(
                $(".resultado-dre-chk").trigger("change");
            }

            if (this.value != "") {
                /**
                -----MSTECH-----
                 *Se o select de NÍVEL for diferente de vazio:
                 -Habilita select de EDICAO;
                 -Se o select de NÍVEL for TURMA, mostramos uma opção select com um texto de referência:
                 Enturmação atual. Aparentemente este texto não tem trigger.
                 -Não sendo o Nível de turma, remove a opção de referência.
                */
                $("#ddlResultadoEdicao").selectmenu("enable");

                if (this.value == "TURMA") {
                    $("#ddlResultadoEdicao_item_ENTURMACAO_ATUAL").show();
                }
                else {
                    if ($("#ddlResultadoEdicao").val() == "ENTURMACAO_ATUAL") {
                        $("#ddlResultadoEdicao").val("");
                        $("#ddlResultadoEdicao").selectmenu("refresh");

                    }
                    $("#ddlResultadoEdicao_item_ENTURMACAO_ATUAL").hide();
                }
            }
            else {
                /**
                -----MSTECH-----
                 *Sendo a opção de nível vazia, reseta todos os filtros legados e dependentes do mesmo.

                 OBS: Os resets feitos abaixo são repetidos no método resultado_configurarControles.
                 O que não faz muito sentido mas também não prejudica o funcionamento do App.
                */
                $("#ddlResultadoEdicao").val("");
                $("#ddlResultadoEdicao").selectmenu("refresh");
                $("#ddlResultadoEdicao").selectmenu("disable");

                $("#ddlResultadoAreaConhecimento").val("");
                $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
                $("#ddlResultadoAreaConhecimento").selectmenu("disable");

                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
                $("#ddlResultadoAno").selectmenu("disable");

                $("#divResultadoDRE").hide();
                $(".resultado-dre-chk").prop('checked', false).checkboxradio('refresh');
            }
            resultado_configurarControles();

            //MSTECH - Nova estrutura para resultados por Ciclo de Aprendizagem
            if (this.value == "SME" || this.value == "DRE" || this.value == "ESCOLA") {
                $("#resultados_opcaoCicloAprendizagem").show();

                //MSTECH - Mostrar Area de Conhecimento se Edição estiver preenchida
                if ($("#ddlResultadoEdicao").val() != "" && $("#ddlResultadoCiclo").val() != "") {
                    $("#ddlResultadoAreaConhecimento").selectmenu("enable");
                }
            }
            else {
                document.getElementById("resultado_selectCiclo").style.display = "none";
                document.getElementById("resultado_selectAno").style.display = "table-row";

                $("#resultados_opcaoCicloAprendizagemSpan").html(
                    "<span class='mdi mdi-checkbox-blank-outline'></span>"
                );
                $("#resultados_opcaoCicloAprendizagem").hide();

                //Resetando campos
                limparCamposSelecionados();

                //MSTECH - Mostrar Area de Conhecimento se Edição estiver preenchida
                if ($("#ddlResultadoEdicao").val() != "") {
                    $("#ddlResultadoAreaConhecimento").selectmenu("enable");
                }
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento CHANGE do select de ESCOLA. Primeiramente executa o método resultado_configurarControles
     para limpar todos os elementos da seleção. Em seguida trata a possibilidade do valor do SELECT
     de edições ser vazio.
    */
    $("#ddlResultadoEdicao").unbind("change").change(function () {
        try {
            resultado_configurarControles();

            if (this.value == "") {
                $("#ddlResultadoAreaConhecimento").val("");
                $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
                $("#ddlResultadoAreaConhecimento").selectmenu("disable");

                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
                $("#ddlResultadoAno").selectmenu("disable");

                $("#ddlResultadoCiclo").val("");
                $("#ddlResultadoCiclo").selectmenu("refresh");
                $("#ddlResultadoCiclo").selectmenu("disable");
            }
            else {
                $("#ddlResultadoAreaConhecimento").selectmenu("enable");
                $("#ddlResultadoCiclo").selectmenu("enable");
            }

            /**
            -----MSTECH-----
             *Por fim, se na mudança da edição o nível for TURMA ou ALUNO, reseta o HTML de resultados e
             desabilita o botão btnResultadoApresentar
            */
            var nivel = $("#ddlResultadoNivel").val();
            if (nivel == "TURMA" || nivel == "ALUNO") {
                $("#divResultadoTurmaItens").html("");
                $("#divResultadoAlunoItens").html("");
                $("#btnResultadoApresentar").prop("disabled", true);
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Novo CUSTOM CheckBox para controlar as opções de Ciclo de aprendizagem. Se o checkbox for
     selecionado, devemos esconder o select de Anos e Mostrar o de Ciclos de aprendizagem. Se ele for
     desmarcado, devemos mostrar o select de Anos.
    */
    $("#resultados_opcaoCicloAprendizagem").unbind("click").click(function () {
        try {
            var TDCiclo = document.getElementById("resultado_selectCiclo");
            var TDAno = document.getElementById("resultado_selectAno");
            var provaEdicao = $("#ddlResultadoEdicao").val();

            if (TDCiclo.style.display == "none") {
                TDAno.style.display = "none";
                TDCiclo.style.display = "table-row";

                $("#resultados_opcaoCicloAprendizagemSpan").html(
                    "<span class='mdi mdi-checkbox-marked'></span>"
                );

                $("#ddlResultadoAreaConhecimento").selectmenu("disable");
                if (provaEdicao != "") { $("#ddlResultadoCiclo").selectmenu("enable"); }
            }
            else {
                TDCiclo.style.display = "none";
                TDAno.style.display = "table-row";

                $("#resultados_opcaoCicloAprendizagemSpan").html(
                    "<span class='mdi mdi-checkbox-blank-outline'></span>"
                );

                $("#ddlResultadoCiclo").selectmenu("disable");
                if (provaEdicao != "") { $("#ddlResultadoAreaConhecimento").selectmenu("enable"); }
            }
            $("#btnResultadoApresentar").prop("disabled", true);

            //Limpando os campos
            limparCamposSelecionados();
            resultado_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    function limparCamposSelecionados() {
        try {
            $("#ddlResultadoCiclo").val("");
            $("#ddlResultadoCiclo").selectmenu("refresh");

            $("#ddlResultadoAno").val("");
            $("#ddlResultadoAno").selectmenu("refresh");

            $("#ddlResultadoAreaConhecimento").val("");
            $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
        }
        catch (error) {
            console.log(error);
        }
    };

    /**
    -----AMCOM-----
     *Assim como o EDIÇÃO, ao alterar a ÁREA de CONHECIMENTO, os filtros para o resultado da
     ProvaSP são resetados e abaixo é tratada a situação de SELECT vazio.
    */
    $("#ddlResultadoAreaConhecimento").unbind("change").change(function () {
        try {
            resultado_configurarControles();

            if (this.value == "") {
                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
                $("#ddlResultadoAno").selectmenu("disable");
            }
            else if (this.value == 1) { //Ciências
                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Novo input select para filtrar pelo ciclo de aprendizagem
    */
    $("#ddlResultadoCiclo").unbind("change").change(function () {
        try {
            resultado_configurarControles();

            if (this.value == "") {
                $("#ddlResultadoAreaConhecimento").val("");
                $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
                $("#ddlResultadoAreaConhecimento").selectmenu("disable");
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Assim como o os elementos SELECT anteriores, executamos o método de reset geral junto ao reset
     das divs divResultadoTurmaItens e divResultadoAlunoItens que comportam as Turmas e Alunos em forma
     de CHECK a serem selecionados.

     OBS: Aparentemente o trecho de validação é irrelevante
    */
    $("#ddlResultadoAno").unbind("change").change(function () {
        try {
            if (this.value != "") {
                var ano = this.value;
                //$("#txtResultadoTurma-selectized").attr("placeholder", "Informe uma ou mais turmas, ex: " + ano + "A, " + ano + "B, " + ano + "C");
            }

            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            resultado_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento especial para a opção TODAS AS DREs da select de DREs.
     *Reparar que as demais opções do select são tratados de acordo com o evento change
     com base no estilo .resultado-dre-chk
    */
    $("#chkResultadoTodasDREs").unbind("click").click(function () {
        try {
            $(".resultado-dre-chk").prop('checked', this.checked).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento dos checks de DREs. A diferença crucial entre este método e o próximo é a iserção da
     opção TODAS AS DRES.
    */
    $(".resultado-dre-chk").unbind("change").change(function () {
        try {
            /**
            -----MSTECH-----
             *Limpa HTMLs que contêm as opções de seleção de Escolas, Turmas e Alunos
             *Em seguida executa o método geral de reset.
            */
            $("#divResultadoEscolaItens").html("");
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            resultado_configurarControles();

            /**
            -----MSTECH-----
             *Seleciona através dos Checks as escolas selecionadas.
             *Se o nível selecionado for DRE o botão de resultado é desabilitados
             *Senão, a flag apresentar escolas é setada como TRUE.
             Ou seja, quando o nível é DRE, não é necessário selecionar uma escola e os resultados
             são mostrados de acordo com as DREs selecionadas.
            */
            var DREs_selecionadas = $(".resultado-dre-chk:checked").map(function () { return this.value; }).get();
            var apresentarEscolas = false;

            if (DREs_selecionadas.length >= 1) {
                if ($("#ddlResultadoNivel").val() == "DRE") {
                    $("#btnResultadoApresentar").prop("disabled", false);
                    $("#btnParticipacaoApresentar").prop("disabled", false);
                }
                else {
                    apresentarEscolas = true;
                }
            }

            /**
            -----MSTECH-----
             *Se o nível NÃO é DRE, devemos tratar e mostrar as escolas.
            */
            if (apresentarEscolas) {
                ///////////////////////////////////////////////////////////////////////////////
                //Carregar escolas das DREs selecionadas:

                $.mobile.loading("show", {
                    text: "Aguarde...",
                    textVisible: true,
                    theme: "a",
                    html: ""
                });

                /**
                -----MSTECH-----
                 *Para carregar as escolas com base nas DREs escolhidas, vamos novamente carregar
                 o arquivo escolas.CSV e, a partir dele, obter as informações necessárias.
                */
                carregarDataEscola(
                    function () {
                        setTimeout(function () {
                            try {
                                /**
                                -----MSTECH-----
                                 *Reparar que apenas usuários autorizados podem ver os resultados
                                 das respectivas escolas.
                                */
                                var codigoEscolasAutorizadas = [];
                                for (var i = 0; i < Usuario.grupos.length; i++) {
                                    if (Usuario.grupos[i].AcessoNivelEscola) {
                                        codigoEscolasAutorizadas.push(Usuario.grupos[i].esc_codigo);
                                    }
                                }

                                /**
                                -----MSTECH-----
                                 *Ao obter as escolas do arquivo escolas.CSV, cria-se uma opção para
                                 seleção de todas as escolas.
                                 *A opção de todas as escolas será mostrada junto às opções das escolas
                                 cuja permissão de acesso encontra-se no objeto do Usuário logado, mais
                                 especificamente em "grupos".
                                */
                                var l = dataEscola.length;
                                var codigoJSTodasEscolas = '$(".resultado-escola-item-chk").prop("checked", this.checked).checkboxradio("refresh"); resultado_configurarControles();';
                                $("#divResultadoEscolaItens").append("<label id='lblResultadoTodasEscolas' for='chkResultadoTodasEscolas'><input id='chkResultadoTodasEscolas' type='checkbox' name='chkResultadoEscola' class='resultado-escola-chk' value='TD' data-mini='true' onclick='" + codigoJSTodasEscolas + "' />(TODAS ESCOLAS)</label>");
                                var escolasEncontradas = 0;

                                /**
                                -----MSTECH-----
                                 *Percorrendo escolas para adicionar à lista para seleção.
                                 *Perceber que se o usuário for nível SME ou DRE ele poderá ver as escolas
                                 sem restrições de grupo.
                                */
                                for (var i = 1; i < l; i++) {
                                    var r = dataEscola[i].split(";");
                                    //uad_sigla; esc_codigo; esc_nome
                                    var uad_sigla = r[0];
                                    var esc_codigo = r[1];
                                    var esc_nome = r[2];


                                    //PERMISSÃO DE VISIBILIDADE PARA A ESCOLA:
                                    var incluirEscola =
                                        (Usuario.AcessoNivelSME ||
                                            Usuario.AcessoNivelDRE ||
                                            codigoEscolasAutorizadas.indexOf(esc_codigo) >= 0);

                                    /**
                                    -----MSTECH-----
                                     *Se o usuário pode acessar os dados da escola, incrementamos escolasEncontradas
                                     e montamos um elemento HTML na lista de escolas a serem selecionadas.
                                    */
                                    if (incluirEscola && (DREs_selecionadas.indexOf("TD") >= 0 ||
                                        DREs_selecionadas.indexOf(uad_sigla) >= 0)) {
                                        escolasEncontradas++;
                                        var chkID = "chkResultadoEscola_" + esc_codigo;
                                        var lblID = "lblResultadoEscola_" + esc_codigo;
                                        $("#divResultadoEscolaItens").append("<label id='" + lblID + "' for='" + chkID + "' class='resultado-escola-lbl'><input id='" + chkID + "' type='checkbox' name='chkResultadoEscola' class='resultado-escola-chk resultado-escola-item-chk' value='" + esc_codigo + "' data-mini='true' />" + esc_nome + "</label>");
                                        //console.log(lblID);
                                    }
                                }

                                /**
                                -----MSTECH-----
                                 *Havendo mais de uma escola disponível, mostrar-se-á as estruturas
                                 para seleção de escolas (filtro de escolas).
                                 *Além disso, cada elemento de escola a ser selecionado receberá um evento
                                 correspondente ao reset das informações dos filtros.
                                */
                                if (escolasEncontradas > 0) {
                                    $("#divResultadoEscola").show();
                                    $("#divResultadoEscolaItens").trigger("create");

                                    $(".resultado-escola-chk").unbind("click").click(
                                        function () {
                                            $("#divResultadoTurmaItens").html("");
                                            $("#divResultadoAlunoItens").html("");
                                            //$("#divResultadoAlunoItens").trigger("create");

                                            resultado_configurarControles();
                                        }
                                    );
                                }
                                $.mobile.loading("hide");
                            }
                            catch (error) {
                                console.log(error);
                            }
                        }, 100);
                        //$("#ddlResultadoEscola").selectmenu("refresh", true);
                    }, ""
                );

                /**
                -----MSTECH-----
                 *Este evento serve para filtrar as escolas selecionadas acima. Ou seja, da listagem de
                 escolas que será mostrada, poderemos escrever uma palavra para filtrar os registros
                 de acordo com a necessidade.
                */
                $("#txtResultadoEscolaFiltro").unbind("change").change(function () {
                    try {
                        /**
                        -----MSTECH-----
                         *Se o filtro for vazio, mostra todas as escolas selecionadas.
                         *Reparar que mostra também a opção padrão TODAS AS ESCOLAS
                        */
                        var valorFiltro = $("#txtResultadoEscolaFiltro").val().trim().toUpperCase();
                        if (valorFiltro == "") {
                            $(".resultado-escola-lbl").show();
                            $("#lblResultadoTodasEscolas").show();
                        }
                        else {
                            /**
                            -----MSTECH-----
                             *Esconde a opção padrão TODAS AS ESCOLAS, bem como os registros das escolas
                             selecionadas. Depois disso, filtra as escolas encontradas pela estrutura
                             de buscas e as mostra novamente.
                            */
                            $("#lblResultadoTodasEscolas").hide();
                            $(".resultado-escola-lbl").hide();
                            $(".resultado-escola-lbl").filter(
                                function () {
                                    return $(this).text().toUpperCase().indexOf(valorFiltro) >= 0 || $("#" + this.id.replace("lbl", "chk"))[0].checked;
                                }
                            ).show();
                            $("#divResultadoEscolaItens").trigger("create");
                        }
                    }
                    catch (error) {
                        console.log(error);
                    }
                });
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Ao selecionar qualquer uma das DREs específicas, desmarca a opção padrão TODAS AS DREs e reseta
     todos os elementos da filtragem
    */
    $(".resultado-dre-item-chk").unbind("click").click(function () {
        try {
            $("#chkResultadoTodasDREs").prop('checked', false).checkboxradio('refresh');
            resultado_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Sempre que este elemento que é um input simples de texto muda, devemos resetar os filtros de busca.

     OBS: Reparar que este modo de busca por turmas não é mais utilizado no ProvaSP. Todas as chamadas
     para este método estão comentadas. A seleção de turmas se dá de maneira parecida com as DREs e etc.
     Deve-se escolher a opção através CHECKS montados no App.
    */
    $("#txtResultadoTurma").unbind("change").change(function () {
        try {
            resultado_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Estes botões determinam a alteração de parâmetros para visualização dos resultados da ProvaSP

     OBS: Como o evento de reset é executado constantemente em cada ação de filtragem, estes botões
     nada fazem além de esconder os resultados e posicionar a tela nos filtros.
    */
    $("#btnResultadoAlterarParametros, #btnResultadoAlterarParametros2").unbind("click").click(function () {
        try {
            //$("#formResultadoOpcoes").show();
            //$("#divResultadoApresentacao").hide();
            divResultadosFixada(true);
            mostrarTelaResultados(false, "divResultadoApresentacao", -1);
            divResultadoProva(0);

            $.mobile.silentScroll(0);

            if (caminhoBackButton.indexOf("btnResultadoAlterarParametros") != -1) {
                removerItemBackButton();
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Assim como na seleção de DREs temos a montagem de uma lista de escola a serem selecionadas, teremos
     a montagem de uma lista de TURMAS a serem selecionadas na escola de cada uma das escolas. No entanto,
     diferente das DREs e Escolas que possuem informações armazenadas localment no App através do arquivo
     escola.CSV, as informações de Turmas e, posteriormente, de Alunos deverão ser buscadas no servidor.
    */
    $("#btnResultadoBuscarTurmas").unbind("click").click(function () {
        try {
            /**
            -----MSTECH-----
             *Reseta listagem de Turmas e alunos
             *Em seguida obtém toda as outras informações dos filtros. Tais informações serão parâmetros
             para a busca de informação no servidor.
             *Reparar que é selecionada uma lista em String com todos os códigos das escolas selecionadas.
             *Por fim, há uma validação de sobrecarga para não permitir buscar turmas de mais de 100 escolas.
            */
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            var Edicao = $("#ddlResultadoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
            var AnoEscolar = $("#ddlResultadoAno").val();
            var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();

            if (lista_esc_codigo.split(",").length >= 100) {
                ProvaSP_Erro("Sobrecarga", "Por gentileza especifique menos de 100 escolas na busca de turmas.");
                return;
            }

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });
            var ResultadoNivel = $("#ddlResultadoNivel").val();

            /**
            -----MSTECH-----
             *Post com primeiro objeto sendo as informações de entrada da busca;
             *O segundo, o nó de sucesso e o terceiro, o nó de falha.
            */
            $.post(urlBackEnd + "api/ResultadoRecuperarTurmas", {
                ResultadoNivel: ResultadoNivel,
                Edicao: Edicao,
                AreaConhecimentoID: AreaConhecimentoID,
                AnoEscolar: AnoEscolar,
                lista_esc_codigo: lista_esc_codigo
            })
                .done(function (data) {
                    $.mobile.loading("hide");

                    if (data.length == 0) {
                        ProvaSP_Erro("Alerta", "Nenhuma turma encontrada");
                        return;
                    }

                    /**
                    -----MSTECH-----
                     *Devolvida validação de 1000 turmas, para ter coerência com a mesma validação
                     na busca por alunos.
                    */
                    if (data.length > 1000) {
                        ProvaSP_Erro("Erro de sobrecarga",
                            "Sua pesquisa retornou mais de 1000 turmas. Por gentileza especifique critérios mais restritos.");
                        return;
                    }

                    /**
                    -----MSTECH-----
                     *A validação, teoricamente, não é necessária. Assim como em Escolas, cada turma
                     retornada receberá um elemento na listagem e existirá um elemento inicial TODAS
                     AS TURMAS.

                     OBS: O For de navegação das turmas monta os elementos HTML tratando o ID e o NOME
                     das turmas de acordo com a necessidade.
                    */
                    if (data.length > 0) {
                        var codigoJSTodasTurmas = '$(".resultado-turma-item-chk").prop("checked", this.checked).checkboxradio("refresh"); resultado_configurarControles();';
                        $("#divResultadoTurmaItens").append("<label for='chkResultadoTodosTurmas' id='lblResultadoTodosTurmas'><input id='chkResultadoTodasTurmas' type='checkbox' name='chkResultadoTurma' class='resultado-turma-chk' value='TD' data-mini='true' onclick='" + codigoJSTodasTurmas + "' />(TODAS TURMAS)</label>");

                        for (var i = 0; i < data.length; i++) {
                            var id = "";
                            var value = "";
                            var text = "";
                            if (data[i].tur_id > 0) {
                                id = "chkResultadoTurma_" + data[i].tur_id;
                                value = data[i].tur_id;
                                if (lista_esc_codigo.split(",").length > 1)
                                    text = data[i].esc_nome + " - " + data[i].tur_codigo;
                                else
                                    text = data[i].tur_codigo;
                            }
                            else {
                                id = "chkResultadoTurma_" + data[i].tur_codigo;
                                value = data[i].tur_codigo;
                                text = data[i].tur_codigo;
                            }
                            $("#divResultadoTurmaItens").append("<label for='" + id + "'><input id='" + id + "' type='checkbox' name='chkResultadoTurma' class='resultado-turma-chk resultado-turma-item-chk' value='" + value + "' data-mini='true' />" + text + "</label>");
                        }
                        $("#divResultadoTurmaItens").trigger("create");

                        /**
                        -----MSTECH-----
                         *Cada item de Turma, assim como os itens de Escola, receberão um evento de
                         seleção que executará o método de reset dos filtros.
                        */
                        $(".resultado-turma-item-chk").unbind("click").click(function () {
                            $("#divResultadoAlunoItens").html("");
                            $("#chkResultadoTodasTurmas").prop('checked', false).checkboxradio('refresh');
                            resultado_configurarControles();
                        });
                    }
                })
                .fail(function (xhr, status, error) {
                    ProvaSP_Erro("Falha de comunicação", "Não foi possível recuperar as turmas. (" + status + ") " + error);
                });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *A filtragem de alunos funciona de forma muito parecida com a filtragem de turmas. Obtém-se os
     dados dos alunos com base nos outro filtros selecionados e através de uma requisição ao servidor
     do ProvaSP.
    */
    $("#btnResultadoBuscarAlunos").unbind("click").click(function () {
        try {
            /**
            -----MSTECH-----
             *Especficamente valores dos filtros;
             *Não permite a selação de mais de 20 escolas;
             *Reseta dis de listagem de alunos.
            */
            var Edicao = $("#ddlResultadoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
            var AnoEscolar = $("#ddlResultadoAno").val();
            var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();
            var lista_turmas = $(".resultado-turma-chk:checked").map(function () { return this.value; }).get().toString(); //$("#txtResultadoTurma").val();

            if (lista_esc_codigo.split(",").length >= 20) {
                ProvaSP_Erro("Sobrecarga", "Por gentileza especifique menos de 20 escolas na busca de alunos.");
                return;
            }

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });
            $("#divResultadoAlunoItens").html("");

            /**
            -----MSTECH-----
             *Reparar, por exemplo, que para esta requisição teremos uma lista de escolas e uma lista
             de turmas. Ambas strings com os dados separados por vírgula ","
            */
            $.post(urlBackEnd + "api/ResultadoRecuperarAlunos", {
                Edicao: Edicao,
                AreaConhecimentoID: AreaConhecimentoID,
                AnoEscolar: AnoEscolar,
                lista_esc_codigo: lista_esc_codigo,
                lista_turmas: lista_turmas
            })
                .done(function (data) {
                    $.mobile.loading("hide");

                    if (data.length == 0) {
                        ProvaSP_Erro("Alerta", "Nenhum aluno encontrado");
                        return;
                    }

                    if (data.length > 1000) {
                        ProvaSP_Erro("Erro de sobrecarga",
                            "Sua pesquisa retornou mais de 1000 alunos. Por gentileza especifique critérios mais restritos.");
                        return;
                    }

                    /**
                    -----MSTECH-----
                     *Assim como em escolas e turmas, os alunos serão organizados numa lista de CHECKS
                     com um elemento inicial TODOS OS ALUNOS.
                     *Os resultados serão mostrados de acordo com os alunos selecionados.

                     ATUALIZADO: Reformulamos a forma como a estrutura de alunos é montada para incluir
                     o botão de provas do aluno (imagens da prova real).
                    */
                    if (data.length > 0) {
                        var alunosHTML = "";
                        var checkBoxesWidth = window.screen.width - 65;
                        var codigoJSTodosAlunos =
                            '$(".resultado-aluno-item-chk").prop("checked", this.checked).checkboxradio("refresh");' +
                            ' resultado_configurarControles();';

                        //TODOS OS ALUNOS - Opção Default
                        alunosHTML += "<label for='chkResultadoTodosAlunos' id='lblResultadoTodosAlunos'>";
                        alunosHTML += "<input id='chkResultadoTodosAlunos' type='checkbox' name='chkResultadoAluno' ";
                        alunosHTML += "class='resultado-aluno-chk' value='TD' data-mini='true' onclick='";
                        alunosHTML += codigoJSTodosAlunos + "' />(TODOS ALUNOS)</label>";

                        //Montando itens individuais para cada aluno
                        for (var i = 0; i < data.length; i++) {
                            var alunoAPIString = "0_" + data[i].alu_matricula + "_" + Edicao;

                            //Estrutura de seleção do aluno
                            alunosHTML += "<div class='alunos_checkDiv' style=' width: " + checkBoxesWidth + "px; '>";
                            alunosHTML += "<label for='chkResultadoAluno_" + data[i].alu_matricula + "'>";
                            alunosHTML += "<input id='chkResultadoAluno_" + data[i].alu_matricula +
                                "' type='checkbox' name='chkResultadoAluno' " +
                                "class='resultado-aluno-chk resultado-aluno-item-chk' value='" +
                                data[i].alu_matricula + "' data-mini='true' />";
                            alunosHTML += data[i].Nome + " (" + data[i].alu_matricula + ")";
                            alunosHTML += "</label>";
                            alunosHTML += "</div>";

                            //Estrutura para abertura das imagens das provas do aluno
                            alunosHTML += "<div class='alunos_iconeDiv'>";
                            alunosHTML += "<img class='alunos_iconeImg' src='/AppProvaSP/images/provas.png' " +
                                "onclick=\"baixarProvaAlunoPorAno(false, '" + alunoAPIString + "')\" />";
                            alunosHTML += "</div>";

                        }
                        $("#divResultadoAlunoItens").html(alunosHTML)
                        $("#divResultadoAlunoItens").trigger("create");

                        /**
                        -----MSTECH-----
                         *Por fim, a seleção de cada aluno executa o evento de reset dos filtros.

                         OBS: Quando mais adentramos os níveis de filtragem dos resultados da ProvaSP,
                         menos elementos do DOM são de fato resetados no momento da execução do
                         resultado_configurarControles
                        */
                        $(".resultado-aluno-item-chk").unbind("click").click(function () {
                            $("#chkResultadoTodosAlunos").prop('checked', false).checkboxradio('refresh');
                            resultado_configurarControles();
                        });
                    }

                })
                .fail(function (xhr, status, error) {
                    ProvaSP_Erro("Falha de comunicação", "Não foi possível recuperar os alunos. (" + status + ") " + error);
                });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *resultadoTabHabilidades
     *Existe uma opção na tela de filtragem para obtenção dos resultados da ProvaSP que
     mostra uma avalição das habilidade da prova, fazendo uma comparação de acordo com os filtros
     selecionados.
    */
    $("#resultadoTabHabilidades").unbind("click").click(function () {
        try {
            /**
            -----MSTECH-----
             *Obtendo todas as informações dos filtros de busca de resultados.
            */
            var nivel = $("#ddlResultadoNivel").val();
            var Edicao = $("#ddlResultadoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
            var AnoEscolar = $("#ddlResultadoAno").val();
            var lista_uad_sigla = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().toString();
            var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();
            var lista_turmas = $(".resultado-turma-chk:checked").map(function () { return this.value; }).get().toString();

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });
            $("#divResultadoTabHabilidades").html("");

            $.post(urlBackEnd + "api/ResultadoRecuperarAlunos", {
                Edicao: Edicao,
                AreaConhecimentoID: AreaConhecimentoID,
                AnoEscolar: AnoEscolar,
                lista_esc_codigo: lista_esc_codigo,
                lista_turmas: lista_turmas
            })
                .done(function (data) {
                    /**
                    -----MSTECH-----
                     -Estas informações só podem ser obtidas depois que os resultados da prova forem
                     mostrados.
                    */
                    $.mobile.loading("hide");
                })
                .fail(function (xhr, status, error) {
                    ProvaSP_Erro("Falha de comunicação",
                        "Não foi possível recuperar as habilidades. (" + status + ") " + error);
                });
        }
        catch (error) {
            console.log(error);
        }
    });
    /**
    -----MSTECH-----
     *Fim do Módulo 4.2.2 - Botões para tratamento dos filtros de obtenção de resultados
    */

    /**
    -----MSTECH-----
     Módulo 4.2.3 - Método para apresentação dos resultados, montagem de gráficos e manipulação
     da UI
    */
    var chartResultadoAgregacao_ctx = null;
    var chartResultadoEscalaSaeb_ctx = null;
    var chartResultadoDetalhe_ctx = null;

    /**
    -----MSTECH-----
     *Os handlers abaixo coletam todas as informações selecionadas pelo usuário nos filtros e mostra
     o resultado da ProvaSP com base no retorno do POST ao servidor.

     OBS: Reparar que os filtros ResultadoFiltro também chamam a requisição ao server.
    */
    $("#btnResultadoApresentar, #chkResultadoFiltroAbaixoDoBasico, #chkResultadoFiltroBasico, #chkResultadoFiltroAdequado, #chkResultadoFiltroAvancado").unbind("click").click(function () {
        try {
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
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            /**
            -----MSTECH-----
             *Abaixo a determinação de informações específicas com base no nível selecionado.
            */
            $('#exportar_graficos').hide();
            $('#exportar-dados').hide();
            $('#exportar-dados-csv-alunos').hide();
            $('#imprimir_graficos').hide();
            
            if (nivel == "DRE") {
                $("#exportar_graficos").show();
                $('#exportar-dados').show();
                lista_uad_sigla = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "ESCOLA") {
                $("#exportar_graficos").show();
                $('#exportar-dados').show();
                lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "TURMA") {
                $("#exportar_graficos").show();

                if (edicao == "ENTURMACAO_ATUAL") {
                    $('#exportar-dados-csv-alunos').show();
                } else {
                    $('#exportar-dados').show();
                }

                lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
                lista_turmas = $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "ALUNO") {
                $("#imprimir_graficos").show();
                lista_alu_matricula = $(".resultado-aluno-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "SME")
            {
                $("#imprimir_graficos").show();
            }

            if ((nivel == "DRE" || nivel == "ESCOLA") && $('span').hasClass('mdi-checkbox-marked'))
            {
                $('#exportar_graficos').hide();
                $('#exportar-dados').hide();
                $('#imprimir_graficos').hide();
            }

            //MSTECH - Construindo objeto para enviar ao servidor. Ele será reaproveitado em situações de busca
            //por ciclo.
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

            //var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();
            //var lista_turmas = $(".resultado-turma-chk:checked").map(function () { return this.value; }).get().toString(); //$("#txtResultadoTurma").val();
            $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), objEnvio)
                .done(function (dataResultado) {
                    /**
                    -----MSTECH-----
                     *No sucesso da requisição, mostra resultados.
                    */
                    $.mobile.loading("hide");
                    resultadoApresentar(
                        ciclo,
                        edicao,
                        areaConhecimentoId,
                        anoEscolar,
                        "divResultadoApresentacao",
                        dataResultado,
                        objEnvio);
                })
                .fail(function (erro) {
                    ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Método que mostra os resultados da ProvaSP em forma de gráficos e etc.
     *É um método extremamente extenso por conter diversos tratamentos, manipulação de UI e manipulação
     da lib de gráficos chart.js

     *Reparar que existem diversos gráficos. Por exemplo um montado para os valores da agredação, outro
     para os valores dos Itens dos resultados.
     *Reparar ainda que existem uma situação em que são montados 2 gráficos da escola SAEB. Mais
     especificamente quando nenhuma edição é escolhida. Sendo assim, o App deverá comparar o ano atual
     com o anterior.
    */
    function resultadoApresentar(ciclo, edicao, areaConhecimentoId, ano, divResultadoContainer, dataResultado, objetoEnviado) {
        try {
            //MSTEHC - Nova informação de Proficiências. Se não houver dados vindos do servidor, usar valor padrão
            var proficienciasAtuais = [
                { Nome: "Indefinido" },
                { Nome: "Abaixo do básico" },
                { Nome: "Básico" },
                { Nome: "Adequado" },
                { Nome: "Avançado" }
            ];
            var labelsCiclos = { ciclo1: "Alfabetização", ciclo2: "Interdisciplinar", ciclo3: "Autoral" };

            if (dataResultado.hasOwnProperty("Proficiencias")) {
                if (dataResultado.Proficiencias.length > 0) {
                    proficienciasAtuais = dataResultado.Proficiencias;
                }
            }

            /**
            -----MSTECH-----
             *Configura ChartJS para mostrar Tooltips por padrão e melhorar o desempenho dos gráficos.
             *Em seguida esconde página de resultados com base nos filtros.
             *Em seguida mostra a Div que comportará os resultados e manipula a interface para se
             moldar de acordo com as opções escolhidas.
            */
            configurarPluginsChartsJS();

            //MSTECH - Novo evento para voltar aos parâmetros quando backbutton do Android
            if (divResultadoContainer == "divResultadoApresentacao") {
                mostrarTelaResultados(true, "divResultadoApresentacao", 0);
                adicionarItemBackButton("btnResultadoAlterarParametros");

                $(".lblResultadoTitulo").html(
                    $("#ddlResultadoNivel option:selected").text()
                );
            }
            else if (divResultadoContainer == "divResultadoApresentacaoAluno") {
                $("#formResultadoOpcoes").hide();
                $("#divResultadoApresentacaoAluno").show();

                $(".lblResultadoTitulo").html(
                    "Resultado referente ao " + dataResultado.AnoEscolar + "º Ano"
                );
            }

            $("#lblResultadoEdicao").html($("#ddlResultadoEdicao option:selected").text());
            $("#lblResultadoAreaConhecimento").html($("#ddlResultadoAreaConhecimento option:selected").text());
            $("#lblResultadoAno").html($("#ddlResultadoAno option:selected").text());

            $("#lblResultadoTituloAgregacao").html("");
            $("#lblResultadoTituloDetalhe").html("");

            var nivel = $("#ddlResultadoNivel").val();

            /**
            -----MSTECH-----
             *Mostra diferentes títulos e descrições com base no tipo de usuário selecionado.
            */
            if (nivel == "SME") {
                if (parseInt(ciclo) > 1) {
                    $("#lblResultadoTituloAgregacao").html("O primeiro gráfico abaixo é o resultado da agragação das informações dos gráficos seguintes (por ano), ou seja, o total do ciclo aprendizagem para cada nível.");
                }
                else {
                    $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da SME nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                    $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada DRE.");
                }
            }
            else if (nivel == "DRE") {
                if (dataResultado.Agregacao.length == 1)
                    $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da DRE nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                else
                    $("#lblResultadoTituloAgregacao").html("Nos gráficos abaixo encontram-se a distribuição de todos os alunos das DREs selecionadas nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Escola.");
            }
            else if (nivel == "ESCOLA") {
                if (dataResultado.Agregacao.length == 1)
                    $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da Escola nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                else
                    $("#lblResultadoTituloAgregacao").html("Nos gráficos abaixo encontram-se a distribuição de todos os alunos das Escolas selecionadas nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Turma.");
            }
            else if (nivel == "TURMA") {
                if (dataResultado.Agregacao.length == 1)
                    $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da Turma nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                else
                    $("#lblResultadoTituloAgregacao").html("Nos gráfico abaixo encontram-se a distribuição de todos os alunos das Turmas selecionadas nos níveis: " +
                        proficienciasAtuais[1].Nome + ", " +
                        proficienciasAtuais[2].Nome + ", " +
                        proficienciasAtuais[3].Nome + " e " +
                        proficienciasAtuais[4].Nome + ".");
                $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Aluno. No gráfico, toque na barra correspondente ao aluno para visualizar informações detalhadas sobre seu respectivo desempenho.");
            }

            /**
            -----MSTECH-----
             *Mais detalhes de interface.

             Testar através de debug
             RESPONDIDO: Já testando e enterdemos que existem diferenças na apresentação dos resultados
             dependendo dos filtros escolhidos.
            */
            if (dataResultado.Agregacao.length == 0)
                $("#divResultadoTituloAgregacao").hide();
            else
                $("#divResultadoTituloAgregacao").show();

            if (dataResultado.Itens.length == 0 || ciclo != "")
                $("#divResultadoTituloDetalhe").hide();
            else
                $("#divResultadoTituloDetalhe").show();

            if (edicoesComTurmasAmostrais.indexOf(edicao) >= 0 && edicao == 2017 && ano % 2 == 0)
                $("#divResultadoTituloAmostral").show();
            else
                $("#divResultadoTituloAmostral").hide();

            /**
            -----MSTECH-----
             *Configurando valores de referência para os gráficos de Área de Conhecimento e para
             cada ano escolar.
             *Tais configurações são importantes para melhorar a visualização dos resultados
             por parte dos usuários verificando que a nota máxima para todas as disciplinas é 500,
             com exceção de redação que é 100.
            */
            var proficienciaMaxima = 500;
            var reguaProficiencia = {};

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
                var proficienciaMaxima = 100;
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

            /**
            -----MSTECH-----
             *Determinando os intervalos do gráfico com base nos valores preestabelecidos
             informados acima.
             *Em seguida, temos a montagem das legendas, também utilizando a base da régua de
             proficiência
            */
            var intervaloGrafico = [];
            var tituloAbaixoDoBasico = "";
            var tituloBasico = "";
            var tituloAdequado = "";
            var tituloAvancado = "";

            if (ciclo == "") {
                intervaloGrafico = [
                    reguaProficiencia[ano][0],
                    reguaProficiencia[ano][1] - reguaProficiencia[ano][0],
                    reguaProficiencia[ano][2] - reguaProficiencia[ano][1],
                    proficienciaMaxima - reguaProficiencia[ano][2]
                ];
                tituloAbaixoDoBasico = proficienciasAtuais[1].Nome + " (<" + reguaProficiencia[ano][0] + ")";
                tituloBasico = proficienciasAtuais[2].Nome + " (>=" + reguaProficiencia[ano][0] + " e <" + reguaProficiencia[ano][1] + ")";
                tituloAdequado = proficienciasAtuais[3].Nome + " (>=" + reguaProficiencia[ano][1] + " e <" + reguaProficiencia[ano][2] + ")";
                tituloAvancado = proficienciasAtuais[4].Nome + " (>=" + reguaProficiencia[ano][2] + ")";
            }
            else {
                intervaloGrafico = [
                    reguaProficiencia["c" + ciclo][0],
                    reguaProficiencia["c" + ciclo][1] - reguaProficiencia["c" + ciclo][0],
                    reguaProficiencia["c" + ciclo][2] - reguaProficiencia["c" + ciclo][1],
                    proficienciaMaxima - reguaProficiencia["c" + ciclo][2]
                ];
                tituloAbaixoDoBasico = proficienciasAtuais[1].Nome + " (<" + reguaProficiencia["c" + ciclo][0] + ")";
                tituloBasico = proficienciasAtuais[2].Nome + " (>=" + reguaProficiencia["c" + ciclo][0] + " e <" + reguaProficiencia["c" + ciclo][1] + ")";
                tituloAdequado = proficienciasAtuais[3].Nome + " (>=" + reguaProficiencia["c" + ciclo][1] + " e <" + reguaProficiencia["c" + ciclo][2] + ")";
                tituloAvancado = proficienciasAtuais[4].Nome + " (>=" + reguaProficiencia["c" + ciclo][2] + ")";
            }


            /**
            -----MSTECH-----
             *Aqui temos a determinação das variáveis que compõem a interface e estilos dos gráficos
             basicamente especificação de cores.
            */
            var legendaAgregacao = { position: "right" };
            var corNivelAbaixoDoBasico_ref = "rgba(255,0,0,alpha)";
            var corNivelBasico_ref = "rgba(253,173,0,alpha)";
            var corNivelAdequado_ref = "rgba(0,0,255,alpha)";
            var corNivelAvancado_ref = "rgba(0,255,0,alpha)";

            var corNivelAbaixoDoBasico = corNivelAbaixoDoBasico_ref.replace("alpha", "0.55");
            var corNivelBasico = corNivelBasico_ref.replace("alpha", "0.3");
            var corNivelAdequado = corNivelAdequado_ref.replace("alpha", "0.3");
            var corNivelAvancado = corNivelAvancado_ref.replace("alpha", "0.3");

            var corNivelAbaixoDoBasico_polar = corNivelAbaixoDoBasico_ref.replace("alpha", "0.65");
            var corNivelBasico_polar = corNivelBasico_ref.replace("alpha", "0.4");
            var corNivelAdequado_polar = corNivelAdequado_ref.replace("alpha", "0.4");
            var corNivelAvancado_polar = corNivelAvancado_ref.replace("alpha", "0.4");

            var corNivelAbaixoDoBasico_enturmacao = corNivelAbaixoDoBasico_ref.replace("alpha", "0.55");
            var corNivelBasico_enturmacao = corNivelBasico_ref.replace("alpha", "0.3");
            var corNivelAdequado_enturmacao = corNivelAdequado_ref.replace("alpha", "0.3");
            var corNivelAvancado_enturmacao = corNivelAvancado_ref.replace("alpha", "0.3");

            $("#divChartResultadoAgregacao").empty();
            var tituloNivel = [
                "",
                proficienciasAtuais[1].Nome,
                proficienciasAtuais[2].Nome,
                proficienciasAtuais[3].Nome,
                proficienciasAtuais[4].Nome
            ];

            /**
            -----MSTECH-----
             *Montagem do gráfico de resultado Agregação.
             *O código abaixo é a estruturação do ChartJS, gráfico do tipo PolarArea.
             *Reparar que os atributos do gráfico são montados de acordo com os dados retornados
             pelo servidor, mas especificamente os dados contidos no objeto Agregacao.
            */
            for (var i = 0; i < dataResultado.Agregacao.length; i++) {
                var graficosAgregacaoHTML = "";
                var agregacao = dataResultado.Agregacao[i];
                var textoProficiencia = [agregacao.Titulo, "Total de alunos: " + agregacao.TotalAlunos];

                graficosAgregacaoHTML += "<canvas id='chartResultadoAgregacao" + i + "' style='margin-top:15px;'></canvas>";

                //MSTECH - Nova div para gráficos do Ciclo de Aprendizagem
                if (ciclo != "") {
                    //CICLO DE ALFABETIZAÇÃO - Gráfico sobreposto
                    graficosAgregacaoHTML += "<div style='margin-top:15px;'>";
                    graficosAgregacaoHTML += baseGraficoAprendizagem(i);
                    if (ciclo == 1) {
                        graficosAgregacaoHTML += "<div id='divResultadoCiclo2Ano_" + i + "'></div>";
                    }
                    legendaAgregacao = { display: false };
                    graficosAgregacaoHTML += baseGraficoSerieHistorica(i, areaConhecimentoId);
                    graficosAgregacaoHTML += "</div>";
                }
                else {
                    textoProficiencia = [
                        agregacao.Titulo, "Proficiência: " + agregacao.Valor + " (" +
                        tituloNivel[agregacao.NivelProficienciaID] + ") - Total de alunos: " +
                        agregacao.TotalAlunos, ""];
                }
                $("#divChartResultadoAgregacao").append(graficosAgregacaoHTML);

                chartResultadoAgregacao_ctx = document.getElementById("chartResultadoAgregacao" + i).getContext("2d");
                chartResultadoAgregacao_ctx.canvas.height = 150;

                var chartResultadoAgregacao = new Chart(chartResultadoAgregacao_ctx, {
                    type: 'polarArea',
                    data: {
                        //labels: ["Abaixo do básico:" + dataResultado.PercentualAbaixoDoBasico + "%", "Básico:" + dataResultado.PercentualBasico + "%", "Adequado:" + dataResultado.PercentualAdequado + "%", "Avançado:" + dataResultado.PercentualAvancado + "%"],
                        labels: [tituloAbaixoDoBasico + ":  " + agregacao.PercentualAbaixoDoBasico + "%", tituloBasico + ":  " + agregacao.PercentualBasico + "%", tituloAdequado + ":  " + agregacao.PercentualAdequado + "%", tituloAvancado + ":  " + agregacao.PercentualAvancado + "%"],

                        datasets: [
                            {
                                data: [agregacao.PercentualAbaixoDoBasico, agregacao.PercentualBasico, agregacao.PercentualAdequado, agregacao.PercentualAvancado],
                                backgroundColor: [corNivelAbaixoDoBasico_polar, corNivelBasico_polar, corNivelAdequado_polar, corNivelAvancado_polar]
                            }
                        ]
                    },
                    options: {
                        //showAllTooltips: false,
                        title: {
                            display: true,
                            text: textoProficiencia,
                            fontFamily: "'Open Sans Bold', sans-serif",
                            fontSize: 15,
                        },
                        legend: legendaAgregacao,
                        tooltips: {
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    /**
                                    -----MSTECH-----
                                     *Determina a label do gráfico em porcentagem com base nas
                                     informações contidas no mesmo.
                                    */
                                    //get the concerned dataset
                                    var dataset = data.datasets[tooltipItem.datasetIndex];
                                    //calculate the total of this data set
                                    var total = dataset.data.reduce(function (previousValue, currentValue, currentIndex, array) {
                                        return previousValue + currentValue;
                                    });
                                    //get the current items value
                                    var currentValue = dataset.data[tooltipItem.index];
                                    //calculate the precentage based on the total and current item, also this does a rough rounding to give a whole number
                                    var precentage = Math.floor(((currentValue / total) * 100) + 0.5);

                                    var titulo = data.labels[tooltipItem.index].split(":")[0].trim();
                                    return titulo + ": " + precentage + "%";
                                    //return precentage + "%";
                                }
                            }
                        },
                        animation: {
                            onComplete: function () {
                                //chartResultadoAgregacao_ctx.fillText('Space to add something here with 50x100 padding',5,20);
                                /*
                                this.data.datasets[4].data = this.data.datasets[3].data;
                                setTimeout(
                                    function()
                                    {
                                        this.update();
                                    }.bind(this), 1000
                                );
                                */
                            }
                        }
                    }
                });

                /**
                -----MSTECH-----
                 *Se o filtro de nível for ESCOLA, ainda oferece a possibilidade de verificar o boletim
                 da mesma, gerado em arquivo PDF.
                */
                if (nivel == "ESCOLA") {
                    let cicloRevistaBoletim = ciclo;
                    if (!cicloRevistaBoletim && ano) {
                        // busca o ciclo com base no ano letivo, assim facilita a exibição dos dados ao consultar boletins/revistas
                        cicloRevistaBoletim = buscarCicloPeloAnoLetivo(ano).toString();
                    }

                    //informações dos filtros selecionados + nome da escola(Titulo)
                    let scriptRevistaEscola = `abrirConsultaRevistasBoletins(); `
                        + `aplicarFiltrosRevistasBoletins('${edicao}', '${areaConhecimentoId}', '${cicloRevistaBoletim}', '${agregacao.Titulo}');`;

                    $("#divChartResultadoAgregacao").append(
                        `<a id="btnResultadoBoletimEscola${i}" class="ui-btn" href="#" onclick="${scriptRevistaEscola}">Consultar Revista/Boletim da Escola</a>`
                    );
                }
            }

            /**
            -----MSTECH-----
             *Além dos gráficos mais objetos com o desempenho na ProvaSP com base no nível selecionado,
             é deve-se montar também gráficos com base no SAEB Sistema de Avaliação da Educação Básica.
             *Para tal, abaixo configuramos as informações para a montagem dos gráficos correspondentes.

             *Reparar ainda que no caso da ENTURMACAO_ATUAL, são montadas duas réguas com dois gráficos e
             na outra situação apenas 1. Se escolher ENTURMACAO_ATUAL monta o gráfico do ano atual e anterior
             para fins de comparação.

             Debugar para enteder melhor o funcionamento e a razão divergência da criação dos gráficos.
             RESPONDIDO: Dois gráficos são criados quando compara-se o ano atual com o ano anterior.
            */
            $("#" + divResultadoContainer + " .lblResultadoTituloEscalaSaeb_1").html("");
            $("#" + divResultadoContainer + " .lblResultadoTituloEscalaSaeb_2").html("");

            $("#" + divResultadoContainer + " .divChartResultadoEscalaSaeb_1").empty();
            $("#" + divResultadoContainer + " .divChartResultadoEscalaSaeb_2").empty();

            var intervaloGrafico2 = intervaloGrafico;
            document.getElementById('divChartResultadoDetalhe').style.overflow = 'visible';
            document.getElementById('divChartResultadoDetalhe').style.height = '100%';

            if (edicao == "ENTURMACAO_ATUAL" && ciclo == "") {
                var anoAplicacaoProva = recuperarAnoEnturmacao(dataResultado);
                if (anoAplicacaoProva > 9)
                    anoAplicacaoProva = 9;
                intervaloGrafico = [
                    reguaProficiencia[anoAplicacaoProva][0],
                    reguaProficiencia[anoAplicacaoProva][1] - reguaProficiencia[anoAplicacaoProva][0],
                    reguaProficiencia[anoAplicacaoProva][2] - reguaProficiencia[anoAplicacaoProva][1],
                    proficienciaMaxima - reguaProficiencia[anoAplicacaoProva][2]
                ];

                $(".lblResultadoTituloEscalaSaeb_1").html("Régua do " + anoAplicacaoProva + "º ano");
                $(".lblResultadoTituloEscalaSaeb_2").html("Régua do " + ano + "º ano");

                //CONFIGURA 2 RÉGUAS
                document.getElementById('divChartResultadoDetalhe').style.overflow = 'auto';
                document.getElementById('divChartResultadoDetalhe').style.height = '350px';

                configurarReguaSaeb(
                    divResultadoContainer,
                    "divChartResultadoEscalaSaeb_1",
                    "chartResultadoEscalaSaeb_1",
                    proficienciaMaxima,
                    [{
                        data: [intervaloGrafico[0]],
                        backgroundColor: corNivelAbaixoDoBasico,
                        label: proficienciasAtuais[1].Nome + " (<" + reguaProficiencia[anoAplicacaoProva][0] + ")" //label: "Abaixo do básico" //label: "Abaixo do básico (<" + intervaloGrafico[0] +")"
                    }, {
                        data: [intervaloGrafico[1]],
                        backgroundColor: corNivelBasico,
                        label: proficienciasAtuais[2].Nome + " (>=" + reguaProficiencia[anoAplicacaoProva][0] + " e <" + reguaProficiencia[anoAplicacaoProva][1] + ")" //label: "Básico" //label: "Básico (>=" + intervaloGrafico[0] + " e <" + intervaloGrafico[1] + ")"
                    }, {
                        data: [intervaloGrafico[2]],
                        backgroundColor: corNivelAdequado,
                        label: proficienciasAtuais[3].Nome + " (>=" + reguaProficiencia[anoAplicacaoProva][1] + " e <" + reguaProficiencia[anoAplicacaoProva][2] + ")" //label: "Adequado" //label: "Adequado (>=" + intervaloGrafico[1] + " e <" + intervaloGrafico[2] + ")"
                    }, {
                        data: [intervaloGrafico[3]],
                        backgroundColor: corNivelAvancado,
                        label: proficienciasAtuais[4].Nome + " (>=" + reguaProficiencia[anoAplicacaoProva][2] + ")" //label: "Avançado" //label: "Avançado (>=" + intervaloGrafico[3] + ")"
                    }]
                );

                configurarReguaSaeb(
                    divResultadoContainer,
                    "divChartResultadoEscalaSaeb_2",
                    "chartResultadoEscalaSaeb_2",
                    proficienciaMaxima,
                    [{
                        data: [intervaloGrafico2[0]],
                        backgroundColor: corNivelAbaixoDoBasico,
                        label: tituloAbaixoDoBasico //label: "Abaixo do básico" //label: "Abaixo do básico (<" + intervaloGrafico[0] +")"
                    }, {
                        data: [intervaloGrafico2[1]],
                        backgroundColor: corNivelBasico,
                        label: tituloBasico //label: "Básico" //label: "Básico (>=" + intervaloGrafico[0] + " e <" + intervaloGrafico[1] + ")"
                    }, {
                        data: [intervaloGrafico2[2]],
                        backgroundColor: corNivelAdequado,
                        label: tituloAdequado //label: "Adequado" //label: "Adequado (>=" + intervaloGrafico[1] + " e <" + intervaloGrafico[2] + ")"
                    }, {
                        data: [intervaloGrafico2[3]],
                        backgroundColor: corNivelAvancado,
                        label: tituloAvancado //label: "Avançado" //label: "Avançado (>=" + intervaloGrafico[3] + ")"
                    }]
                );
            }
            else {
                //CONFIGURA 1 RÉGUA
                configurarReguaSaeb(
                    divResultadoContainer,
                    "divChartResultadoEscalaSaeb_1",
                    "chartResultadoEscalaSaeb_1",
                    proficienciaMaxima,
                    [{
                        data: [intervaloGrafico[0]],
                        backgroundColor: corNivelAbaixoDoBasico,
                        label: tituloAbaixoDoBasico //label: "Abaixo do básico" //label: "Abaixo do básico (<" + intervaloGrafico[0] +")"
                    }, {
                        data: [intervaloGrafico[1]],
                        backgroundColor: corNivelBasico,
                        label: tituloBasico //label: "Básico" //label: "Básico (>=" + intervaloGrafico[0] + " e <" + intervaloGrafico[1] + ")"
                    }, {
                        data: [intervaloGrafico[2]],
                        backgroundColor: corNivelAdequado,
                        label: tituloAdequado //label: "Adequado" //label: "Adequado (>=" + intervaloGrafico[1] + " e <" + intervaloGrafico[2] + ")"
                    }, {
                        data: [intervaloGrafico[3]],
                        backgroundColor: corNivelAvancado,
                        label: tituloAvancado //label: "Avançado" //label: "Avançado (>=" + intervaloGrafico[3] + ")"
                    }]
                );
            }

            /**
            -----MSTECH-----
             *Ajustando dimensões dos gráficos de acordo com os Itens gerados.
            */
            $("#" + divResultadoContainer + " .divChartResultadoDetalhe").empty().append("<canvas id='chartResultadoDetalhe'></canvas>");

            if (chartResultadoDetalhe_ctx != null) {
                chartResultadoDetalhe_ctx.destroy();
            }

            var chartResultadoDetalhe_ctx = document.getElementById("chartResultadoDetalhe").getContext("2d");

            if (dataResultado.Itens.length == 1) {
                chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 45; //300;
            }
            else if (dataResultado.Itens.length <= 4) {
                chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 40; //300;
            }
            else if (dataResultado.Itens.length == 5) {
                chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 35; //300;
            }
            else {
                if (edicao == "ENTURMACAO_ATUAL") {
                    chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 60; //300;
                }
                else {
                    chartResultadoDetalhe_ctx.canvas.height = (dataResultado.Itens.length * 50); //300;
                }

            }
            //$("#divChartResultadoDetalhe").show();
            //$("#imgChartResultadoDetalhe").hide();

            /**
            -----MSTECH-----
             *Montagem do gráfico de barras com base nas informações dos itens dos resultados.
            */
            for (var i = 0; i < dataResultado.Itens.length; i++) {
                if (dataResultado.Itens[i].Valor == -1) {
                    dataResultado.Itens[i].Valor = "Profic. não calculada";
                }
            }

            var chartResultadoDetalhe = new Chart(chartResultadoDetalhe_ctx, {
                //var chartResultadoDetalhe = new Chart(chartResultadoDetalhe_ctx, {
                type: 'horizontalBar',
                data: {
                    labels: [],
                    datasets: [
                        {
                            data: [],
                            backgroundColor: []
                        }
                    ]
                },
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
                        onComplete: function (c) {
                            setTimeout(
                                function () {
                                    var labelOffset = c.chart.getDatasetMeta(0).data[0]._model.height;
                                    if ($("#ddlResultadoEdicao").val() == "ENTURMACAO_ATUAL") {
                                        //No caso de ENTURMACAO_ATUAL são apresentados 2 gráficos: a proficiência do ano anterior e a atual.
                                        //Por essa razão é preciso dobrar o salto da label:
                                        labelOffset *= 2;
                                    }

                                    c.chart.options.scales.yAxes[0].ticks.minor.labelOffset = -labelOffset;
                                    //c.chart.options.scales.yAxes[0].ticks.labelOffset = -labelOffset;
                                    c.chart.update();
                                }, 250);
                        }
                    },
                    showAllTooltips: true,
                    tooltips: {
                        enabled: true,
                        backgroundColor: "rgba(100,100,100,1)",
                        callbacks: {
                            title: function (tooltipItem, data) {
                                //CUSTOMIZAÇÃO DA TOOLTIP

                                //return data['labels'][tooltipItem[0]['index']];
                                return "";
                            },
                            label: function (tooltipItem, data) {
                                /**
                                -----MSTECH-----
                                 *CUSTOMIZAÇÃO DA TOOLTIP;
                                 *De acordo com os valores passados ao gráfico, mostrar tooltips
                                 personalizadas. Por exemplo: dependendo dos resultados obtidos, informar
                                 se ficou abaixo da médio, acima ou etc.
                                */
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


                                //return data['datasets'][0]['data'][tooltipItem['index']];
                            }
                            /*,
                            afterLabel: function (tooltipItem, data)
                            {
                                //CUSTOMIZAÇÃO DA TOOLTIP

                                var dataset = data['datasets'][0];
                                //var percent = Math.round((dataset['data'][tooltipItem['index']] / dataset["_meta"][0]['total']) * 100)
                                //return '(%)';
                            }
                            */
                        },
                    },
                    scales: {
                        yAxes: [{
                            categoryPercentage: 0.6,
                            barPercentage: 0.6,
                            ticks: {
                                mirror: true,
                                labelOffset: 1,
                                fontStyle: "bold"
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
                            scaleLabel: {

                            },
                            gridLines: {

                            },
                            stacked: false
                        }]
                    }
                }
            });

            /**
            -----MSTECH-----
             *Trechos responsável pela filtragem de dados dos gráficos.
            */
            var hashtableProficienciaId_cor = {};
            hashtableProficienciaId_cor["1"] = corNivelAbaixoDoBasico;
            hashtableProficienciaId_cor["2"] = corNivelBasico;
            hashtableProficienciaId_cor["3"] = corNivelAdequado;
            hashtableProficienciaId_cor["4"] = corNivelAvancado;

            var hashtableProficienciaId_enturmacao_cor = {};
            hashtableProficienciaId_enturmacao_cor["1"] = corNivelAbaixoDoBasico_enturmacao;
            hashtableProficienciaId_enturmacao_cor["2"] = corNivelBasico_enturmacao;
            hashtableProficienciaId_enturmacao_cor["3"] = corNivelAdequado_enturmacao;
            hashtableProficienciaId_enturmacao_cor["4"] = corNivelAvancado_enturmacao;

            var maiorTitulo = "";

            if (edicao == "ENTURMACAO_ATUAL") {
                chartResultadoDetalhe.data.datasets.push({ data: [], backgroundColor: [] });
            }

            var existeValor = false;
            var filtroProficiencia = $(".resultado-filtro-proficiencia:checked").map(function () { return this.value; }).get();

            for (var i = 0; i < dataResultado.Itens.length; i++) {
                var item = dataResultado.Itens[i];

                //MSTECH - Quando existe valor em pelo menos um dos ITENS, mostra o gráfico em ciclo
                if (item.Valor != 0) { existeValor = true; }

                if (item.NivelProficienciaID == 0 || filtroProficiencia.indexOf(item.NivelProficienciaID.toString()) >= 0) {
                    var tituloAtual = item.Titulo + ": " + item.Valor;

                    if (tituloAtual.length > maiorTitulo.length) {
                        maiorTitulo = tituloAtual;
                    }
                    chartResultadoDetalhe.data.labels.push(item.Titulo + ": " + item.Valor);
                    chartResultadoDetalhe.data.datasets[0].data.push(item.Valor);
                    chartResultadoDetalhe.data.datasets[0].backgroundColor.push(hashtableProficienciaId_cor[item.NivelProficienciaID]);
                    if (edicao == "ENTURMACAO_ATUAL")
                        chartResultadoDetalhe.data.datasets[1].data.push(item.Valor);

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
                        chartResultadoDetalhe.data.datasets[0].label = "Régua do " + anoAplicacaoProva + "º ano";

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
                        chartResultadoDetalhe.data.datasets[0].label =
                            "Régua do ciclo de " + labelsCiclos["ciclo" + ciclo];
                    }

                    /**
                    -----MSTECH-----
                     *Corrigido bug de atribuição de dados ao dataset. Isso ocorrerá apenas quando a
                     base de dados para tal existir
                    */
                    if (edicao == "ENTURMACAO_ATUAL") {
                        chartResultadoDetalhe.data.datasets[1].backgroundColor.push(hashtableProficienciaId_enturmacao_cor[NivelProficienciaID_ENTURMACAO]);
                        chartResultadoDetalhe.data.datasets[1].label = "Régua do " + ano + "º ano";
                    }
                }
            }

            /**
            -----MSTECH-----
             *Com os gráficos montados, serão criadas métricas de média.
             *Tais métricas são adicionadas ao gráficos

             Verificar por debug como as médias são mostradas.
             RESPONDIDO: Verificamos no Debug a montagem de todos os tipos de gráficos.
            */
            if (dataResultado.Itens.length > 1) {
                if (dataResultado.Valor > 0) {
                    chartResultadoDetalhe.data.labels.push("MÉDIA: " + dataResultado.Valor);
                    chartResultadoDetalhe.data.datasets[0].data.push(dataResultado.Valor);
                    chartResultadoDetalhe.data.datasets[0].backgroundColor.push(hashtableProficienciaId_cor[dataResultado.NivelProficienciaID]);
                }
            }
            chartResultadoDetalhe.update();
            //var yScale = chartResultadoDetalhe.scales['y-axis-0'];
            //var yLabelOffset = (yScale.getPixelForTick(0) - yScale.getPixelForTick(1)) / 2;
            //chartResultadoDetalhe.options.scales.yAxes[0].ticks.minor.labelOffset = yLabelOffset;

            /**
            -----MSTECH-----
             *Por fim, se do servidor vieram informações quanto ao desempenho em habilidades específicas
             montar-se-á a tab de Habilidades. Caso contrário a tab não será mostrada.
            */
            if (ciclo != "") {
                if (!existeValor) {
                    $("#divResultadoTabProficiencias").hide();
                }
                $("#divTabsProficienciaHabilidade").hide();
                $("#divResultadoTabHabilidades").hide();
            }
            else {
                $("#divResultadoTabProficiencias").show();
                if (dataResultado.Habilidades != null) {
                    configurarHabilidades(dataResultado);
                    $("#divTabsProficienciaHabilidade").show();
                }
                else {
                    $("#divTabsProficienciaHabilidade").hide();
                    $("#divResultadoTabHabilidades").hide();
                }
            }
            $('#divChartResultadoDetalhe').scrollTop(0);

            /**
            -----MSTECH-----
             *Novo método para mostrar os gráficos do ciclo de aprendizagem
            */
            cicloTotalAlunos = {};
            //MSTECH - Resetando série histórica
            serieHistorica = { anoAtual: {}, anoAnterior: {} };
            if (ciclo != "" && Object.keys(objetoEnviado).length > 0) {
                //MSTECH - Validação para verificar se as bases dos gráficos foram criadas ao haver informação
                if (dataResultado.Agregacao.length > 0) {
                    for (var i = 0; i < modeloCiclos["Ciclo" + ciclo].length; i++) {
                        downloadResultadosCiclos(i, objetoEnviado, ciclo);
                    }
                }
            }
            else {
                limparCicloAprendizagem("");
            }
        }
        catch (error) {
            console.log(error);
        }
    }

    /**
    -----MSTECH-----
     *Este método determina a configuração dos gráficos do ChartJS.
     *Primeiramente percebe-se que o código torna os gráficos mais eficientes;
     *Em segundo lugar, ele serve para sempre mostrar as tooltips (legendas e labels de informação)

     Referência: https://stackoverflow.com/questions/36992922/chart-js-v2-how-to-make-tooltips-always-appear-on-pie-chart
    */
    function configurarPluginsChartsJS() {
        try {
            //PLUGIN "showAllTooltips":
            Chart.plugins.register({
                beforeRender: function (chart) {
                    if (chart.config.options.showAllTooltips) {
                        // create an array of tooltips
                        // we can't use the chart tooltip because there is only one tooltip per chart
                        chart.pluginTooltips = [];
                        chart.config.data.datasets.forEach(function (dataset, i) {
                            chart.getDatasetMeta(i).data.forEach(function (sector, j) {
                                chart.pluginTooltips.push(new Chart.Tooltip({
                                    _chart: chart.chart,
                                    _chartInstance: chart,
                                    _data: chart.data,
                                    _options: chart.options.tooltips,
                                    _active: [sector]
                                }, chart));
                            });
                        });
                        // turn off normal tooltips
                        chart.options.tooltips.enabled = false;
                    }
                },
                afterDraw: function (chart, easing) {
                    if (chart.config.options.showAllTooltips) {
                        // we don't want the permanent tooltips to animate, so don't do anything till the animation runs atleast once
                        if (!chart.allTooltipsOnce) {
                            if (easing !== 1)
                                return;
                            chart.allTooltipsOnce = true;
                        }

                        // turn on tooltips
                        chart.options.tooltips.enabled = true;
                        /**
                       -----MSTECH-----
                        *Havia um erro aqui, CHART agora é carregado localmente.
                       */
                        Chart.helpers.each(chart.pluginTooltips, function (tooltip) {
                            // This line checks if the item is visible to display the tooltip
                            if (!tooltip._active[0].hidden) {
                                tooltip.initialize();
                                tooltip.update();
                                // we don't actually need this since we are not animating tooltips
                                tooltip.pivot();
                                tooltip.transition(easing).draw();
                            }
                        });
                        chart.options.tooltips.enabled = false;
                    }
                }
            })
        }
        catch (error) {
            console.log(error);
        }
    }

    /**
    -----MSTECH-----
     *Obtém as informações das habilidades através do objeto retornado do servidor.
     *Com base nas informações de habilidades, manipuladas por nível, montam-se tabelas de
     informações e grádicos de radar para análise dos dados.
    */
    function configurarHabilidades(dataResultado) {
        try {
            var nivel = $("#ddlResultadoNivel").val();
            var selecaoMultipla = false;

            /**
            -----MSTECH-----
             *Verificando se usuário escolheu mais de uma opção em qualquer um dos filtros que
             possibilitam tal ação.
             *Em seguir reseta o conteúdo HTML da div divResultadoTabHabilidades_conteudoDinamico, a
             qual comporta a análise de habilidades.
            */
            if (nivel == "DRE") {
                selecaoMultipla = ($(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().length > 1);
            }
            else if (nivel == "ESCOLA") {
                selecaoMultipla = ($(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().length > 1);
            }
            else if (nivel == "TURMA") {
                selecaoMultipla = (
                    $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get().length > 1 ||
                    $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().length > 1);
            }
            $("#divResultadoTabHabilidades_conteudoDinamico").empty();

            /**
            -----MSTECH-----
             *O Loop a seguir monta o HTML de uma tabela com as informações das habilidade de acordo
             com o retorno do servidor.
             *Reparar, portanto, que a construção da análise de habilidades se dá depois da obtenção do
             resultado da ProvaSP com base nos filtros em questão.
            */
            for (var iTema = 0; iTema < dataResultado.Habilidades.length; iTema++) {
                /**
                -----MSTECH-----
                 *Selecionando tema atual e criando o título correspondente
                 *Em seguida cria a tabela com a base SME e os níveis de acordo com os filtros escolhidos
                 pelos usuários no Head da mesma.
                */
                var tema = dataResultado.Habilidades[iTema];
                $("#divResultadoTabHabilidades_conteudoDinamico").append("<h4 style='margin-top:15px;'>" + tema.Titulo + "</h4>");

                var htmTabela = "<table class='greyGridTable'><thead><tr><td>Habilidade</td><td>Descrição</td><td>SME(%)</td>";
                if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA") {
                    htmTabela += "<td>DRE(%)</td>";
                }
                if (nivel == "ESCOLA" || nivel == "TURMA") {
                    htmTabela += "<td>ESCOLA(%)</td>";
                }
                if (nivel == "TURMA") {
                    htmTabela += "<td>TURMA(%)</td>";
                }
                htmTabela += "</tr></thead>";

                /**
                -----MSTECH-----
                 *Este trecho demonstra que as habilidades também são divididas por nível. Sendo assim,
                 criamos vários vetores para armazenar as informações das habilidades com base em tais
                 níveis.
                 *Uma mesma habilidade poderá ser analisada tanto no âmbito SME quanto no âmbite TURMA
                 por exemplo.
                */
                var estruturaHabilidadeMultiNivel = function () {
                    this.arrayHabilidadeValorSME = [];
                    this.arrayHabilidadeValorDRE = [];
                    this.arrayHabilidadeValorESCOLA = [];
                    this.arrayHabilidadeValorTURMA = [];
                    this.arrayHabilidadeLabel = [];
                };
                var arrayHabilidadeValorSME = [];
                var arrayHabilidadeValorDRE = [];
                var arrayHabilidadeValorESCOLA = [];
                var arrayHabilidadeValorTURMA = [];
                var arrayHabilidadeLabel = [];

                var tesseratoHabilidade = {};

                /**
                -----MSTECH-----
                 *Depois de percorrer as habilidades, percorreremos os itens de cada uma delas.
                */
                for (var iHabilidade = 0; iHabilidade < tema.Itens.length; iHabilidade++) {
                    var habilidade = tema.Itens[iHabilidade];

                    /**
                    -----MSTECH-----
                     *Este primeiro tratamento cria um objeto com o nome da habilidade e conteúdo geral
                     do tipo estruturaHabilidadeMultiNivel.
                     *O trecho seguinte é irrelevante por encontrar-se comentado.
                    */
                    if (tesseratoHabilidade[habilidade.OrigemTitulo] == null) {
                        tesseratoHabilidade[habilidade.OrigemTitulo] = new estruturaHabilidadeMultiNivel();

                        if (tema.Itens.length < 3) {
                            for (var itemp = 0; itemp < 3 - tema.Itens.length; itemp++) {
                                //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeLabel.push("Eixo virtual");
                                //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorSME.push(100);
                                //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorDRE.push(100);
                                //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorESCOLA.push(100);
                                //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorTURMA.push(100);
                            }
                        }
                    }

                    /**
                    -----MSTECH-----
                     *Seleciona o objeto multinível correspondente à habilidade atual;
                     *Em seguida cria uma linha na tabela para o título da habilidade se houver mais
                     de uma seleção por nível;
                     *Depois, cria uma linha da tabela para o código e a descrição da habilidade
                    */
                    var dimensaoHabilidade = tesseratoHabilidade[habilidade.OrigemTitulo];

                    if (selecaoMultipla) {
                        htmTabela += "<tr><td colspan='6' style='background:rgb(200,200,200);'>" + habilidade.OrigemTitulo + ":</td></tr>";
                    }
                    htmTabela += "<tr><td>" + habilidade.Codigo + "</td><td style='text-align: left;'>" + habilidade.Descricao + "</td><td>" + habilidade.PercentualAcertosNivelSME + "</td>";

                    /**
                    -----MSTECH-----
                     *Por fim, obtém de cada habilidade o valor correspondente para
                     o nível SME por padrão e para os outros níveis de acordo com a escolha de filtros
                     do usuário.

                     *Reparar que a informação é salva tanto no objeto estruturaHabilidadeMultiNivel
                     quanto nos vetores individuais.
                     *Perceber ainda que os valores armazenados nos vetores individuais não são utilizados,
                     tornando-se irrelevantes.
                    */
                    arrayHabilidadeValorSME.push(habilidade.PercentualAcertosNivelSME);
                    arrayHabilidadeLabel.push(habilidade.Codigo);

                    dimensaoHabilidade.arrayHabilidadeValorSME.push(habilidade.PercentualAcertosNivelSME);
                    dimensaoHabilidade.arrayHabilidadeLabel.push(habilidade.Codigo);

                    if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA") {
                        htmTabela += "<td>" + habilidade.PercentualAcertosNivelDRE + "</td>";
                        arrayHabilidadeValorDRE.push(habilidade.PercentualAcertosNivelDRE);
                        dimensaoHabilidade.arrayHabilidadeValorDRE.push(habilidade.PercentualAcertosNivelDRE);
                    }

                    if (nivel == "ESCOLA" || nivel == "TURMA") {
                        htmTabela += "<td>" + habilidade.PercentualAcertosNivelEscola + "</td>";
                        arrayHabilidadeValorESCOLA.push(habilidade.PercentualAcertosNivelEscola);
                        dimensaoHabilidade.arrayHabilidadeValorESCOLA.push(habilidade.PercentualAcertosNivelEscola);
                    }

                    if (nivel == "TURMA") {
                        htmTabela += "<td>" + habilidade.PercentualAcertosNivelTurma + "</td>";
                        arrayHabilidadeValorTURMA.push(habilidade.PercentualAcertosNivelTurma);
                        dimensaoHabilidade.arrayHabilidadeValorTURMA.push(habilidade.PercentualAcertosNivelTurma);
                    }
                }
                htmTabela += "</table>";


                /**
                -----MSTECH-----
                 *CONFIGURAÇÃO DO(s) GRÁFICO(s) DE RADAR:
                 *Monta-se os gráficos de radar com base nas informações armazenadas no objeto
                 tesseratoHabilidade.
                 -Primeiramente seleciona-se os elementos HTML;
                 -Em seguida monta-se a estrutura de objetos DataSet do chart.js com base nos vetores
                 de informações obtidos apra cada nível;
                 -Por fim, inclui os datasets dos níveis selecionados e mostra o gráfico.
                */
                var itesserato = 0;
                for (var chaveTesserato in tesseratoHabilidade) {
                    var dimensaoHabilidade = tesseratoHabilidade[chaveTesserato];
                    if (chaveTesserato != "null") {
                        $("#divResultadoTabHabilidades_conteudoDinamico").append("<h4>" + chaveTesserato + "</h4>");
                    }

                    var chartID = "chartResultadoHabilidade_" + (++itesserato) + "_" + iTema;
                    $("#divResultadoTabHabilidades_conteudoDinamico").append("<canvas id='" + chartID + "' style='margin-top:15px;'></canvas>");

                    var datasetSME = {
                        backgroundColor: "rgba(120,120,255,0.5)",
                        borderColor: "rgb(120,120,255)",
                        data: dimensaoHabilidade.arrayHabilidadeValorSME,
                        label: 'SME'
                    };

                    var datasetDRE = {
                        backgroundColor: "rgba(0,205,0,0.5)",
                        borderColor: "rgb(0,205,0)",
                        data: dimensaoHabilidade.arrayHabilidadeValorDRE,
                        label: 'DRE'
                    };

                    var datasetESCOLA = {
                        backgroundColor: "rgba(255,120,120,0.5)",
                        borderColor: "rgb(255,120,120)",
                        data: dimensaoHabilidade.arrayHabilidadeValorESCOLA,
                        label: 'ESCOLA'
                    };

                    var datasetTURMA = {
                        backgroundColor: "rgba(205,205,0,0.5)",
                        borderColor: "rgb(205,205,0)",
                        data: dimensaoHabilidade.arrayHabilidadeValorTURMA,
                        label: 'TURMA'
                    };

                    /**
                    -----MSTECH-----
                     *SME é padrão
                    */
                    var conjuntoDATASETS = [datasetSME];

                    if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA") {
                        conjuntoDATASETS.push(datasetDRE);
                    }
                    if (nivel == "ESCOLA" || nivel == "TURMA") {
                        conjuntoDATASETS.push(datasetESCOLA);
                    }
                    if (nivel == "TURMA") {
                        conjuntoDATASETS.push(datasetTURMA);
                    }

                    /**
                    -----MSTECH-----
                     *Estrutura ChartJS do gráfico de radar.
                    */
                    var chartResultadoHabilidade_ctx = document.getElementById(chartID).getContext("2d");
                    chartResultadoHabilidade_ctx.canvas.height = 150;
                    var chartResultadoHabilidade = new Chart(chartResultadoHabilidade_ctx, {
                        type: 'radar',
                        data: {
                            //labels: ["Abaixo do básico:" + dataResultado.PercentualAbaixoDoBasico + "%", "Básico:" + dataResultado.PercentualBasico + "%", "Adequado:" + dataResultado.PercentualAdequado + "%", "Avançado:" + dataResultado.PercentualAvancado + "%"],
                            labels: dimensaoHabilidade.arrayHabilidadeLabel,
                            datasets: conjuntoDATASETS
                        },
                        options: {
                            scale: {
                                ticks: {
                                    beginAtZero: true,
                                    min: 0,
                                    max: 100,
                                    stepSize: 25
                                }
                            }
                        }
                    });
                }
                /**
                -----MSTECH-----
                 *Tabela de habilidades
                */
                $("#divResultadoTabHabilidades_conteudoDinamico").append(htmTabela);
            }
        }
        catch (error) {
            console.log(error);
        }
    }

    /**
    -----MSTECH-----
     *Método que retorna apenas o ano correspondente à turma selecionada menos 1. Aparentemente
     este valor é usado para fins de métrica de gráficos.

     *Mais especificamente, ao selecioanr ENTURMACAO_ATUAL o ProvaSP deve montar um comparativa
     entre o ano atual e o anterior.

     *Reparar que havia uma forma de obter este valor com base nos dados do servidor.

     Verificar por debug a utilização deste valor
     RESPONDIDO: Retorna o ano anterior o ano selecionado, para comparação entre eles.
    */
    function recuperarAnoEnturmacao(dataResultado) {
        try {
            //var maiorAnoEscolar = 0;
            //for (var i = 0; i < dataResultado.Itens.length; i++)
            //{
            //    var item = dataResultado.Itens[i];
            //    if (item.AnoEscolar > maiorAnoEscolar && item.AnoEscolar != null)
            //        maiorAnoEscolar = item.AnoEscolar;
            //}
            //return maiorAnoEscolar;
            var ano = parseInt($("#ddlResultadoAno").val()) - 1;
            return ano;
        }
        catch (error) {
            console.log(error);
        }
    }

    /**
    -----MSTECH-----
     *Este método cria gráficos de barras com comparativos dos dados retornados pelo servidor.
     *São grafícos que comparam, de acordo com o nível, as informações com base nos filtros selecionados
     oferecendo aos usuários mais opções de análise e visualização
    */
    function configurarReguaSaeb(divResultadoContainer, divRegua, canvasId, proficienciaMaxima, datasets) {
        try {
            $("#" + divResultadoContainer + " ." + divRegua).empty().append("<canvas id='" + canvasId + "'></canvas>");

            var chartResultadoEscalaSaeb_ctx = document.getElementById(canvasId).getContext("2d");
            chartResultadoEscalaSaeb_ctx.canvas.height = 100;

            var chartResultadoEscalaSaeb = new Chart(chartResultadoEscalaSaeb_ctx, {
                type: 'horizontalBar',
                data: {
                    labels: [""],

                    datasets: datasets
                },

                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    tooltips: {
                        enabled: false
                    },
                    hover: {
                        animationDuration: 0
                    },
                    scales: {
                        xAxes: [{
                            ticks: {
                                fontFamily: "'Open Sans Bold', sans-serif",
                                fontSize: 11,
                                min: 0,
                                max: proficienciaMaxima,
                                stepSize: 50
                            },
                            scaleLabel: {
                                display: false
                            },
                            gridLines: {

                            },
                            stacked: true
                        }],
                        yAxes: [{
                            gridLines: {
                                display: false,
                                color: "#fff",
                                zeroLineColor: "#fff",
                                zeroLineWidth: 0,
                                fontColor: "blue"
                            },
                            ticks: {
                                fontColor: "white",
                                fontFamily: "'Open Sans Bold', sans-serif"

                            },
                            stacked: true
                        }]
                    },
                    legend: {
                        display: true,
                        labels: {
                            fontSize: 11,
                            boxWidth: 15,
                            boxHeight: 20
                        },
                        onClick: function (event, legendItem) { } //DESABILITA CLICK NA LEGENDA
                    },

                }
            });
        }
        catch (error) {
            console.log(error);
        }
    }
    /**
    -----MSTECH-----
     *Fim do Módulo 4.2.3 - Método para apresentação dos resultados, montagem de gráficos e manipulação
    */

    /**
    -----MSTECH-----
     Módulo 4.2.4 - Métodos e eventos associados ao ProvaSP de fato, questionário do tipo 8. Neste
     módulo temos funcionalidades como:
     -Termo de sigilo e compromisso;
     -Seleção de turma;
     -Frequências ou chamada;
     -Etc.
     da UI
    */

    /**
    -----MSTECH-----
     *Aparentemente este método sugere uma impressão. O método parece inacabado por alguns motivos:
     -Não possui elemento "elem" que entrará como Body do documento;
     -Referencia um select do questionário 9 Sem especificar lógica alguma;
     -Aparentemente o questionário 9, do supervidor escolar, nem mesmo é utilizado.

     Questão do questionário 9 - É utilizado?
     RESPONDIDO: Este método é irrelevante. Ao selecionar um DRE no questionário do tipo 9, em vez
     de serem mostradas as escolas, uma página para impressão vazia é mostrada. O que não faz sentido.

     Corrigido: Foi corrigido um bug neste método com base no código da versão publicada na PlayStore.
     Agora ao selecionar uma DRE, é possível selecionar a escola.
    */
    $("#ddlDRE").unbind("change").change(function () {
        try {
            //            var mywindow = window.open('', 'PRINT', 'height=400,width=600');
            //
            //            mywindow.document.write('<html><head><title>' + document.title + '</title>');
            //            mywindow.document.write('</head><body >');
            //            mywindow.document.write('<h1>' + document.title + '</h1>');
            //            mywindow.document.write(document.getElementById(elem).innerHTML);
            //            mywindow.document.write('</body></html>');
            //
            //            mywindow.document.close(); // necessary for IE >= 10
            //            mywindow.focus(); // necessary for IE >= 10*/
            //
            //            mywindow.print();
            //            mywindow.close();
            //
            //            return true;
            var uad_sigla = this.value;
            carregarDataEscola(selecionarDRE, uad_sigla);
        }
        catch (error) {
            console.log(error);
        }
        //return false;
    });

    /**
    -----MSTECH-----
     *Momento em que o termo de sigilo e compromisso é aceito pelo usuário aplicador da ProvaSP.
     *Reparar que, ao executar este evento de aceite, o questionário 8 será habilitado.
    */
    $("#btnTermoAplicadorConcordo").unbind("click").click(function () {
        try {
            /*
            $("#divCodigoTurma").show();
            $("#divMenuPrincipal").hide();
            $("#txtCodigoTurma").focus();
            adicionarItemBackButton("btnCodigoTurmaVoltar");
            */

            /**
            -----MSTECH-----
             *Abaixo temos todos os tratamentos iniciais para início do ProvaSP ou questionário do
             tipo 8.
            */
            adicionarItemBackButton("btnQuestionarioSair");

            $("#divQuestionario8_TermoDeCompromisso").hide();
            $("#divMenuPrincipal").show();

            var questionarioId = 8;
            $(".page").hide();
            $("#questionario-page").show();

            var tur_id = codigoTurma_atual;//$("#txtCodigoTurma").val();
            $("#Questionario_8_Questao_1_tur_id").val(tur_id);
            $('#divQuestionario8_maisDias').hide();
            $("#divQuestionario8_exibirMaisDias").show();
            calcularPresentesEAusentes();
            selecionarQuestionario(questionarioId);
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Quando o usuário não aceia o termo de sigilo e compromisso, o App simplesmente volta
     ao estado imediatamente anterior.
    */
    $("#btnTermoAplicadorDiscordo").unbind("click").click(function () {
        try {
            removerItemBackButton();

            $("#divQuestionario8_TermoDeCompromisso").hide();
            $("#btnCodigoTurmaVoltar").trigger("click");
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Este método é responsável por buscar a lista de alunos da turma informada pelo aplicador da prova.
     *É importante verificar que a lista de alunos é buscada através dos arquivps CSV no pacote do
     aplicativo, ou seja, é possível aplicar a prova Offline.
     *Além disso, é importante destacar a maneira como os alunos das turmas são obtidos. Todos os alunos
     de turmas terminadas em um determinado dígito são armazenados no arquivo CSV com nome correspondente
     a este último dígito de validação. Destta maneira, pelo código da turma, é possível carregar o aquivo
     correspondente diretamente.
    */
    $("#btnCodigoTurmaProsseguir").unbind("click").click(function () {
        try {
            var codigo = codigoTurma_atual = retornaCodigoValido($("#txtCodigoTurma").val());

            /**
            -----MSTECH-----
             *Não deve ser possível utilizar caderno reserva no ProvaSP Web
            */
            if (!mobile) {
                if (window.location.href.indexOf("file:///") == -1)
                    $("#tblCadernoReserva .ui-btn").hide();
            }

            /**
            -----MSTECH-----
             *Validando código de turma informado.
             *Sendo um código válido, obter o dígito verificador.
             *Sendo um código inválido, informar ao usuário
            */

            /**
            -----MSTECH-----
             *Em 2018 removemos o dígito verificador por não haver tal informação nos códigos de turma
             fornecidos. Portanto o tratamento de validação é feito verificando a existência
             da turma apenas.
            */
            var tur_id = null;
            var codigoValido = true;
            if (!(codigo.length == 6 || codigo.length == 7)) {
                codigoValido = false;
            }
            else if (!$.isNumeric(codigo)) {
                codigoValido = false;
            }
            //else {
            //    tur_id = parseInt(codigo.substring(0, codigo.length - 1));
            //    var verificador = parseInt(codigo.substring(codigo.length - 1, codigo.length));
            //    codigoValido = (calculaDigito(tur_id) == verificador);
            //}

            if (!codigoValido) {
                ProvaSP_Erro("Código inválido",
                    "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.");
                return;
            }
            else {
                tur_id = parseInt(codigo);
            }

            var arquivo = tur_id.toString();
            arquivo = arquivo.substring(arquivo.length - 1, arquivo.length);

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            /**
            -----MSTECH-----
             *Buscando localmente informações dos alunos de uma turma específica com base no último
             dígito do código da turma.
            */
            $.ajax({
                type: "GET",
                url: "/AppProvaSP/turmas/" + arquivo + ".csv",
                dataType: "text",
                success: function (data) {
                    /**
                    -----MSTECH-----
                     *Obtendo as informações de cada aluno do arquivo separando os dados por linha.
                     *Cada elemento linha do arquivo correspondente a um aluno será transformado em
                     um elemento de um vetor lista de alunos.
                    */
                    if (data.indexOf("\r\n") > 0) {
                        data = data.replace(/\r\n/g, "\n");
                    }
                    var dataChamada = data.split("\n");
                    $("#divChamadaAlunos").empty();
                    var processandoTurma = false;
                    var tur_id = parseInt(codigo);
                    //var tur_id = parseInt(codigo.substring(0, codigo.length - 1));

                    /**
                    -----MSTECH-----
                     *Comparando a turma informada pelo aplicador com a turma de cada um dos alunos
                     armazenados no arquivo CSV. Sendo a turma correspondente, cria uma estrutura de
                     seleção do aluno, onde será possível informar a presença (CHAMADA)

                     *Reparar que cada aluno selecionado terá um ID de checkbox e um ID de span
                    */
                    for (var i = 0; i < dataChamada.length; i++) {
                        var registro = dataChamada[i].split(";");
                        if (registro[0] == tur_id) {
                            processandoTurma = true;
                            var idCheckBox = "chkChamada_" + i.toString();
                            var idSpan = "spanChamada_" + i.toString();
                            $("#divChamadaAlunos").append('<label for="' + idCheckBox + '"><input id="' + idCheckBox + '" name="Questionario_8_Questao_4" class="listaPresencaAluno" onchange="marcarDesmarcarAlunoAusente(this);" type="checkbox" name="alu_matricula" value="' + registro[1] + '" /> ' + registro[2] + ' <span id="' + idSpan + '" style="color:red;display:none;">(AUSENTE)</span></label>');
                        }
                        else {
                            /**
                            -----MSTECH-----
                             *Funciona como um BREAK. É esperado que todos os alunos de uma mesma turma
                             tenham seus registro um seguido do outro. Logo, quando o Loop identificar
                             uma turma diferente, o mesmo deve ser interrompido.
                            */
                            if (processandoTurma) {
                                i = dataChamada.length;
                            }
                        }
                    }
                    $.mobile.loading("hide");
                    $(".ui-page").trigger("create");

                    /**
                    -----MSTECH-----
                     *Ao carregar a lista de alunos com sucesso, o termo de compromisso será mostrado.
                    */
                    if (!processandoTurma) {
                        ProvaSP_Erro("Código não encontrado",
                            "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.");
                    }
                    else {
                        $("#divQuestionario8_TermoDeCompromisso").show();
                        $("#divCodigoTurma").hide();
                    }
                },
                error: function () {
                    ProvaSP_Erro("Código não encontrado",
                        "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.");
                }
            });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento do botão Ficha de Registro / Aplicador da Prova.
     *Mostra a primeira div de Codigo de Turma e esconde o menu principal.
    */
    $("#btnAbrirFichaAplicadorProvaOuChamada").unbind("click").click(function () {
        try {
            $("#divCodigoTurma").show();
            $("#divMenuPrincipal").hide();
            $("#txtCodigoTurma").focus();
            adicionarItemBackButton("btnCodigoTurmaVoltar");
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *O botão para este evento aparentemente não existe.

     Verificar com debug possibilidade de sair do questionário de aplicação de prova
     RESPONDIDO: Botão realmente não existe.
    */
    $("#btnFichaAplicadorProvaOuChamadaVoltar").unbind("click").click(function () {
        try {
            $("#divCodigoTurma").show();
            $("#divFichaAplicadorProvaOuChamada").hide();
            $("#txtCodigoTurma").focus();
            removerItemBackButton();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Método simples para sair da tela de seleção de turma e voltar para o menu principal
    */
    $("#btnCodigoTurmaVoltar").unbind("click").click(function () {
        try {
            $("#divCodigoTurma").hide();
            $("#divQuestionario8_TermoDeCompromisso").hide();
            $("#divMenuPrincipal").show();
            removerItemBackButton();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento para voltar a tela de login.
     *Reparar que no ProvaSP Web deveria executar o método alertarSaidaApp, o qual não existe.

     Verificar existência do método alertarSaidaApp
     RESPONDIDO: Existe uma equivalência na versão web no código C#
    */
    $("#btnSair").unbind("click").click(function () {
        try {
            swal({
                title: "Deseja realmente voltar à tela de Login?",
                type: "warning",
                showCancelButton: true,

                confirmButtonText: "Sim",
                cancelButtonText: "Não",
                closeOnConfirm: false
            },
                function () {
                    if (mobile) { window.location = "index.html"; }
                    else { parent.alertarSaidaApp(); }
                    swal.close();
                });
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Evento para sair do questionário (seja qual for o tipo)
     *Reparar que quando está na tela de escolha de turma, volta diretamente e quando está na tela
     de questionários, mostra o sweet alert.
    */
    $("#btnQuestionarioSair").unbind("click").click(function () {
        try {
            swal({
                title: "Deseja realmente sair?",
                type: "warning",
                showCancelButton: true,

                confirmButtonText: "Sim",
                cancelButtonText: "Não",
                closeOnConfirm: false
            },
                function () {
                    //window.location = "menu.html";
                    removerItemBackButton();
                    $(".page").hide();
                    $("#menu-page").show();
                    $("#divCodigoTurma").hide();
                    $("#divMenuPrincipal").show();
                    swal.close();
                });
        }
        catch (error) {
            console.log(error);
        }
    });
    /**
    -----MSTECH-----
     *Fim do Módulo 4.2.4 - Métodos e eventos associados ao ProvaSP de fato, questionário do tipo 8.
    */

    /**
    -----MSTECH-----
     Módulo 4.2.5 - Eventos dos botões de questionários
     -Neste módulo temos as ações ao selecionar, por exemplo, as alternativas dos questionários.
    */

    function iniciarQuestionario() {
        $("#divQuestionario" + questionarioId_atual + "_Intro").hide();
        $("#divQuestionario" + questionarioId_atual + "_Questoes").show();
        $("#divTituloQuestionario").show();
        $.mobile.silentScroll(0);
    }

    /**
    -----MSTECH-----
     *O item inicial dos questionários não é uma questão, é uma label da Introdução
     para iniciar o questionário de fato.
     *Reparar que esconde a introdução e mostra as questões
    */
    $("#btnQuestionario1_Iniciar,#btnQuestionario24_Iniciar,#btnQuestionario3_Iniciar," +
        "#btnQuestionario23_Iniciar,#btnQuestionario25_Iniciar,#btnQuestionario14_Iniciar," +
        "#btnQuestionario15_Iniciar,#btnQuestionario16_Iniciar,#btnQuestionario17_Iniciar," +
        "#btnQuestionario18_Iniciar,#btnQuestionario21_Iniciar,#btnQuestionario22_Iniciar")
        .unbind("click").click(function () {
            try {
                let urlVerificarQuestionarioRespondido = urlBackEnd
                    + "api/Usuario/RespondeuQuestionario?edicao="
                    + String(new Date().getFullYear())
                    + "&questionarioID=" + questionarioId_atual
                    + "&usu_id=" + Usuario.usu_id;

                if (mobile) {
                    $.mobile.loading("show", {
                        textVisible: false,
                        theme: "a",
                        html: ""
                    });
                }

                $.ajax({
                    url: urlVerificarQuestionarioRespondido,
                    type: "GET",
                    dataType: "TEXT",
                    crossDomain: true,
                    cache: false,
                    success: function (dataResultado) {
                        if (mobile)
                            $.mobile.loading("hide");

                        if (dataResultado === "True") {
                            swal("Atenção!", "Este questionário já foi respondido.", "warning");
                        }
                        else {
                            //os questionários de Diretor e Assistente de Diretor são bem longos
                            if (questionarioId_atual === 24 || questionarioId_atual === 25) {
                                swal(
                                    {
                                        title: "Gostaria de iniciar o preenchimento agora?",
                                        text: "Este questionário é extenso e não será possível salvá-lo para continuar em outro momento.",
                                        type: "info",
                                        showCancelButton: true,

                                        confirmButtonText: "Sim",
                                        cancelButtonText: "Não",
                                        closeOnConfirm: true
                                    },
                                    function (isConfirm) {
                                        if (isConfirm)
                                            iniciarQuestionario();
                                    },
                                );
                            } else {
                                iniciarQuestionario();
                            }
                        }
                    },
                    error: function (error) {
                        ProvaSP_Erro("Erro " + error.status, error.statusText);
                    }
                });
            }
            catch (error) {
                console.log(error);
            }
        });

    /**
    -----AMCOM-----
     * Pergunta passou a ser escolha única na edição de 2k19
     *ALTERNATIVAS DE MÚLTIPLA ESCOLHA QUE DEVEM DESELECIONAR OUTRAS
     *Todos os handlers abaixo são de itens de questionários que aceitam mais de uma resposta. No
     entanto, geralmente existe uma opção que anula a seleção das demais
    */
    /*$("#Questionario_24_Questao_5_A,#Questionario_24_Questao_5_B,#Questionario_24_Questao_5_C,#Questionario_24_Questao_5_D,#Questionario_24_Questao_5_E").unbind("click").click(function () {
        try {
            $("#Questionario_24_Questao_5_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_24_Questao_5_F").unbind("click").click(function () {
        try {
            $("#Questionario_24_Questao_5_A,#Questionario_24_Questao_5_B,#Questionario_24_Questao_5_C,#Questionario_24_Questao_5_D,#Questionario_24_Questao_5_E").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });*/

    $("#Questionario_3_Questao_7_A,#Questionario_3_Questao_7_B,#Questionario_3_Questao_7_C,#Questionario_3_Questao_7_D,#Questionario_3_Questao_7_E").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_7_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_7_F").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_7_A,#Questionario_3_Questao_7_B,#Questionario_3_Questao_7_C,#Questionario_3_Questao_7_D,#Questionario_3_Questao_7_E").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_31_A,#Questionario_3_Questao_31_B,#Questionario_3_Questao_31_C,#Questionario_3_Questao_31_D,#Questionario_3_Questao_31_E,#Questionario_3_Questao_31_F").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_31_G").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_31_G").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_31_A,#Questionario_3_Questao_31_B,#Questionario_3_Questao_31_C,#Questionario_3_Questao_31_D,#Questionario_3_Questao_31_E,#Questionario_3_Questao_31_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_35_A,#Questionario_3_Questao_35_B,#Questionario_3_Questao_35_C,#Questionario_3_Questao_35_D,#Questionario_3_Questao_35_E").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_35_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_35_F").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_35_A,#Questionario_3_Questao_35_B,#Questionario_3_Questao_35_C,#Questionario_3_Questao_35_D,#Questionario_3_Questao_35_E").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_95_A,#Questionario_3_Questao_95_B,#Questionario_3_Questao_95_C,#Questionario_3_Questao_95_D,#Questionario_3_Questao_95_E,#Questionario_3_Questao_95_F").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_95_G").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_95_G").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_95_A,#Questionario_3_Questao_95_B,#Questionario_3_Questao_95_C,#Questionario_3_Questao_95_D,#Questionario_3_Questao_95_E,#Questionario_3_Questao_95_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_96_A,#Questionario_3_Questao_96_B,#Questionario_3_Questao_96_C,#Questionario_3_Questao_96_D,#Questionario_3_Questao_96_E,#Questionario_3_Questao_96_F,#Questionario_3_Questao_96_G,#Questionario_3_Questao_96_H").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_96_I").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_3_Questao_96_I").unbind("click").click(function () {
        try {
            $("#Questionario_3_Questao_96_A,#Questionario_3_Questao_96_B,#Questionario_3_Questao_96_C,#Questionario_3_Questao_96_D,#Questionario_3_Questao_96_E,#Questionario_3_Questao_96_F,#Questionario_3_Questao_96_G,#Questionario_3_Questao_96_H").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    /* ----AMCOM----
     * Na versão 2019 a pergunta passou a ser de escolha única
    $("#Questionario_23_Questao_8_A,#Questionario_23_Questao_8_B,#Questionario_23_Questao_8_C,#Questionario_23_Questao_8_D,#Questionario_23_Questao_8_E").unbind("click").click(function () {
        try {
            $("#Questionario_23_Questao_8_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_23_Questao_8_F").unbind("click").click(function () {
        try {
            $("#Questionario_23_Questao_8_A,#Questionario_23_Questao_8_B,#Questionario_23_Questao_8_C,#Questionario_23_Questao_8_D,#Questionario_23_Questao_8_E").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });*/

    /**
    -----AMCOM-----
    * Pergunta passou a ser escolha única na edição de 2k19
    */
    /*$("#Questionario_25_Questao_6_A,#Questionario_25_Questao_6_B,#Questionario_25_Questao_6_C,#Questionario_25_Questao_6_D,#Questionario_25_Questao_6_E").unbind("click").click(function () {
        try {
            $("#Questionario_25_Questao_6_F").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_25_Questao_6_F").unbind("click").click(function () {
        try {
            $("#Questionario_25_Questao_6_A,#Questionario_25_Questao_6_B,#Questionario_25_Questao_6_C,#Questionario_25_Questao_6_D,#Questionario_25_Questao_6_E").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });*/

    $("#Questionario_14_Questao_7_B,#Questionario_14_Questao_7_C").unbind("click").click(function () {
        try {
            $("#Questionario_14_Questao_7_A").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_14_Questao_7_A").unbind("click").click(function () {
        try {
            $("#Questionario_14_Questao_7_B,#Questionario_14_Questao_7_C").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_15_Questao_6_B,#Questionario_15_Questao_6_C").unbind("click").click(function () {
        try {
            $("#Questionario_15_Questao_6_A").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_15_Questao_6_A").unbind("click").click(function () {
        try {
            $("#Questionario_15_Questao_6_B,#Questionario_15_Questao_6_C").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_16_Questao_6_B,#Questionario_16_Questao_6_C").unbind("click").click(function () {
        try {
            $("#Questionario_16_Questao_6_A").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_16_Questao_6_A").unbind("click").click(function () {
        try {
            $("#Questionario_16_Questao_6_B,#Questionario_16_Questao_6_C").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_17_Questao_6_B,#Questionario_17_Questao_6_C").unbind("click").click(function () {
        try {
            $("#Questionario_17_Questao_6_A").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_17_Questao_6_A").unbind("click").click(function () {
        try {
            $("#Questionario_17_Questao_6_B,#Questionario_17_Questao_6_C").prop('checked', false).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Os handlers abaixo habilitam/desabilitam campos de justificativa de questões com
     resposta SIM e NÃO.
    */
    $("#Questionario_8_Questao_14_6_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_8_Questao_14_6_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_8_Questao_14_6_Nao,#Questionario_8_Questao_14_6_NaoSeAplica").unbind("click").click(function () {
        try {
            $('#Questionario_8_Questao_14_6_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_8_Questao_14_7_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_8_Questao_14_7_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_8_Questao_14_7_Nao,#Questionario_8_Questao_14_7_NaoSeAplica").unbind("click").click(function () {
        try {
            $('#Questionario_8_Questao_14_7_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_10_Questao_3_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_10_Questao_3_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_10_Questao_3_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_10_Questao_3_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_10_Questao_4_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_10_Questao_4_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_10_Questao_4_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_10_Questao_4_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_3_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_3_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_3_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_3_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_4_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_4_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_4_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_4_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_5_Satisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_5_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_5_Insatisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_5_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_6_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_6_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_6_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_6_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_7_Satisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_7_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_7_Insatisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_7_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_8_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_8_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_8_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_8_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_9_Satisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_9_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_9_Insatisfatoria").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_9_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_10_Sim").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_10_Motivos').textinput('disable');
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_11_Questao_10_Nao").unbind("click").click(function () {
        try {
            $('#Questionario_11_Questao_10_Motivos').textinput('enable');
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Os métodos abaixo são específicos para a seleção da Área de Conhecimento. Dependendo da
     escolha.
     *O método resetInstrumento esconde elementos extras para o questionário de Ciências e Português.
     *Matemática não possui itens extras
    */
    $("#Questionario_8_Questao_3_Portugues").unbind("click").click(function () {
        try {
            resetInstrumento();
            $("#Questionario_8_QuestoesInstrumentoPortugues").show();
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_8_Questao_3_Matematica").unbind("click").click(function () {
        try {
            resetInstrumento();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Aplicação de BIB do questionário 14 - Auxiliar Técnico da Educação
    */
    $("#Questionario_14_Questao_1_A").unbind("click").click(function () {
        try {
            aplicarBIBQuestionario14(true);
        }
        catch (error) {
            console.log(error);
        }
    });

    $("#Questionario_14_Questao_1_B").unbind("click").click(function () {
        try {
            aplicarBIBQuestionario14(false);
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Removido evento para itens extras de Ciências em 2018
    */
    //    $("#Questionario_8_Questao_3_Ciencias").unbind("click").click(function () {
    //        try {
    //            resetInstrumento();
    //            $("#Questionario_8_QuestoesInstrumentoCiencias").show();
    //        }
    //        catch (error) {
    //            console.log(error);
    //        }
    //    });

    /**
    -----MSTECH-----
     *Este handler em específico trata para que sejam escolhidas apenas 3 alternativas dentre
     as disponíveis para a questão. É um caso extremamente particular

     Validação não utilizada no questionário atualizado.
    */
    //    $("#fieldSetQuestionario_3_Questao_21").delegate('.ui-checkbox', 'click', function (e) {
    //        try {
    //            //limita o número de alternativas selecionadas em 3.
    //            var alternativas = ["A", "B", "C", "D", "E", "F", "G", "H"];
    //            var quantidadeSelecao = 0;
    //            for (var i = 0; i < alternativas.length; i++) {
    //                if ($("#Questionario_3_Questao_21_" + alternativas[i]).prop("checked")) {
    //                    quantidadeSelecao++;
    //                }
    //            }
    //
    //            if (e.target.attributes["for"].value == "Questionario_3_Questao_21_I") {
    //                return;
    //            }
    //
    //            var chk = $("#" + e.target.attributes["for"].value);
    //            if (!chk.prop("checked") && quantidadeSelecao >= 3) {
    //                //swal("", "Selecione no máximo três alternativas!", "error"); //swal manda a barra de rolagem para o topo.
    //                alert("Selecione no máximo três alternativas!");
    //                e.stopImmediatePropagation();
    //                e.preventDefault();
    //            }
    //        }
    //        catch (error) {
    //            console.log(error);
    //        }
    //    });

    /**
    -----MSTECH-----
     *Mostra os relatórios de acompanhamento com base no ID do usuário.
     *Se for mobile, verificar conexão.
     *Abre uma página dentro do App.

     OBS: Relatório de acompanhamento são informações sobre o nível de participação no ProvaSP.
    */
    $("#btnAbrirRelatorioAcompanhamento").unbind("click").click(function () {
        try {
            /**
            -----MSTECH-----
             *Corrigida implementação do método.
             -Usamos o código correto para abrir uma URL;
             -A URL funciona com o usu_login e não com o usu_id
            */
            if (mobile) {
                if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN) {
                    ProvaSP_Erro("Falha de comunicação",
                        "Para poder abrir o relatório de acompanhamento, é necessário estar conectado na internet.");
                    return;
                }
                else {
                    navigator.app.loadUrl(urlBackEnd +
                        "RelatorioAcompanhamento?usu_id=" + Usuario.usu_login, { openExternal: true });
                }
            }
            else {
                window.open(urlBackEnd + "RelatorioAcompanhamento?usu_id=" + Usuario.usu_login);
            }
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Método simpels que esconde todas as outras "pages" e mostra o menu
    */
    $("#btnResultadoFechar").unbind("click").click(function () {
        try {
            $(".page").hide();
            $("#menu-page").show();

            removerItemBackButton();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Novo método para fechar o popup de Prova do Aluno. Nele são mostrados os links para as imagens
     da prova física do aluno.
    */
    $("#btnProvaAlunoVoltar").unbind("click").click(function () {
        try {
            $(".page").hide();
            if (Usuario.Aluno) {
                $("#resultado-aluno-page").show();
            }
            else {
                $("#resultado-page").show();
            }
            removerItemBackButton();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Botão para abrir popup de prova física do aluno. Primeiro devemos tentar buscar no servidor
     as URLs, depois mostrar o popup de acesso.
    */
    $("#btnProvaAlunoPopup").unbind("click").click(function () {
        try {
            var alunoIndividualAPIString = $("#ddlResultadoAlunoAreaConhecimento").val() +
                "_" + Usuario.usu_login.replace("RA", "") + "_" + $("#ddlResultadoAlunoEdicao").val();

            baixarProvaAlunoPorAno(true, alunoIndividualAPIString);
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----MSTECH-----
     *Provavelmente haveria uma forma de visualizar os relatórios de acompanhamento com base no tipo
     de usuário. No entanto, as informações passaram a ser mostradas com base no ID do usuário. Ou
     seja, o tratamento é feito na Web.

     OBS: Métodos abaixo são irrelevantes/obsoletos tendo em vista que o relatório de acompanhamento
     é gerado com base no USU_LOGIN do usuário.
    */
    //RELATÓRIO DO SUPERVISOR:
    $("#btnAbrirRelatorioAcompanhamento_9").unbind("click").click(function () {
        try {

        }
        catch (error) {
            console.log(error);
        }
    });

    //RELATÓRIO DO DIRETOR:
    $("#btnAbrirRelatorioAcompanhamento_10").unbind("click").click(function () {
        try {

        }
        catch (error) {
            console.log(error);
        }
    });

    //RELATÓRIO DO COORDENADOR:
    $("#btnAbrirRelatorioAcompanhamento_11").unbind("click").click(function () {
        try {

        }
        catch (error) {
            console.log(error);
        }
    });
    /**
    -----MSTECH-----
     *Fim do Módulo 4.2.5 - Eventos dos botões de questionários
    */


    /**
    -----AMCOM-----
     Módulo 4.2.6 - Botões para tratamento dos filtros de obtenção de revistas pedagógicas e boletins
     (o que é feito ao selecionar cada opção)
    */

    /**
    -----AMCOM-----
     * Controla a visualização das DREs e escolas
    */
    $("#ddlRevistasBoletinsEdicao").unbind("change").change(function () {
        try {
            revistasBoletins_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----AMCOM-----
     * Controla a visualização das DREs e escolas
    */
    $("#ddlRevistasBoletinsAreaConhecimento").unbind("change").change(function () {
        try {
            revistasBoletins_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----AMCOM-----
     *Novo input select para filtrar pelo ciclo de aprendizagem
    */
    $("#ddlRevistasBoletinsCiclo").unbind("change").change(function () {
        try {
            revistasBoletins_configurarControles();
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----AMCOM-----
     *Evento especial para a opção TODAS AS DREs da select de DREs.
     *Reparar que as demais opções do select são tratados de acordo com o evento change
     com base no estilo .revistasBoletins-dre-chk
    */
    $("#chkRevistasBoletinsTodasDREs").unbind("click").click(function () {
        try {
            $(".revistasBoletins-dre-chk").prop('checked', this.checked).checkboxradio('refresh');
        }
        catch (error) {
            console.log(error);
        }
    });

    /**
    -----AMCOM-----
     *Evento dos checks de DREs.
    */
    $(".revistasBoletins-dre-chk").unbind("change").change(function () {
        if (!this.checked)
            $("#chkRevistasBoletinsTodasDREs").prop('checked', false).checkboxradio('refresh');
        carregarListaEscolaRevistasBoletins();
    });

    /**
-----AMCOM-----
 *Fim do Módulo 4.2.6 - Botões para tratamento dos filtros de obtenção de revistas pedagógicas e boletins
*/

}
/**
-----MSTECH-----
 *Fim do Módulo 4 - Manipulação da UI
*/




/**
-----MSTECH-----
 *Módulo 5 - Implementações MSTECH
 Novos métodos criados pela MSTECH para novas funcionalidades do App
*/
/**


/**
-----MSTECH-----
 Módulo 5.1: Métodos gerais para resolução de questões específicas
 *
*/

/*-----MSTECH-----
 Obtenção de códigos de turma válidos
 -Método adicional criado pela MSTEHC para converter os códigos impressos nas provas em códigos
 válidos de turmas da SME-SP.
 -Verificar que o método permite que o código seja validado em outra ocasião, caso não esteja
 presente no arquivo.
*/
function retornaCodigoValido(codigoBase) {
    try {
        //Códigos imrpessos nas provas têm, obrigatoriamente, 7 dígitos.
        if (codigoBase.length == 7) {
            var codigosDeTurmaDisponiveis = $.ajax({
                type: "GET",
                async: false,
                url: "DeParaTurmas.csv",
                dataType: "text",
            }).responseText;
            if (codigosDeTurmaDisponiveis.indexOf("\r\n") > 0) {
                codigosDeTurmaDisponiveis = codigosDeTurmaDisponiveis.replace(/\r\n/g, "\n");
            }
            var vetorCodigoDeTurmas = codigosDeTurmaDisponiveis.split("\n");

            for (var i = 0; i < vetorCodigoDeTurmas.length; i++) {
                var conjuntoCodigoAtual = vetorCodigoDeTurmas[i].split(";");

                if (conjuntoCodigoAtual[0] == codigoBase) {
                    return conjuntoCodigoAtual[1];
                }
            }
        }
    }
    catch (error) {
        console.log(error);
    }
    return codigoBase;
}

/**
-----MSTECH-----
 Manipula filtros e divs dos resultados da prova para oferecer condições mais dinâmicas de dados
 sem necessidade de navegar pelas telas.

 ATUALIZAÇÃO: Adicionado tratamento para tornar o menu fixado
*/
//MSTECH - Menu fixado para filtrar resultados
var flagDivFixado = false;
var tabDiv = document.getElementById("resultados_tabEdit");
var tabDivPadding = tabDiv.offsetTop;
window.onscroll = function () { divResultadosFixada(false) };

function divResultadosFixada(resetar) {
    try {
        if (resetar) {
            flagDivFixado = false;

            tabDiv.style.width = "100%";
            tabDiv.classList.remove("genericoFixedDiv");
        }
        else {
            if (flagDivFixado) {
                if (window.pageYOffset > tabDivPadding) {
                    if (!mobile) {
                        tabDiv.style.width = "68%";
                    }
                    else { tabDiv.style.width = (window.innerWidth - 20) + "px"; }
                    tabDiv.classList.add("genericoFixedDiv");
                }
                else {
                    tabDiv.style.width = "100%";
                    tabDiv.classList.remove("genericoFixedDiv");
                }
            }
        }
    }
    catch (error) {
        console.log(error);
    }
}

function mostrarTelaResultados(isShow, divResultados, opcaoTab) {
    try {
        if (isShow) {
            divResultadoProva(-1);

            for (var i = 0; i < 5; i++) {
                $("#resultados_tab" + i).hide();
            }
            $("#btnResultadoFechar").hide();
            $("#resultados_tab" + opcaoTab).show();
            $("#" + divResultados).show();

            //We should fix menu0
            if (opcaoTab < 3) { flagDivFixado = true; }
        }
        else {
            for (var i = 0; i < 5; i++) {
                $("#resultados_tab" + i).show();
            }
            $("#" + divResultados).hide();
            $("#btnResultadoFechar").show();
        }
        $.mobile.silentScroll(0);
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 ATUALIZAÇÃO - Criamos um método para a chamada do botão visando atender à duas situações no momento
 do clique no botão. Ou seja, quando não há clique no botão, não devemos permitir o BACKBUTTON
*/
function abrirResultados(limparFiltros) {
    try {
        /**
        -----MSTECH-----
         *Nova implemenetação para mostrar resultados da ProvaSP para os alunos.
        */
        if (Usuario.Aluno) {
            resultadoAlunoConfigurarInterface();
        }
        else {
            /**
            -----MSTECH-----
             *Removendo opções de resultados SME e DRE de acordo com o tipo de usuário
            */
            if (!Usuario.AcessoNivelSME) {
                $("#ddlResultadoNivel_SME").remove();

                if (!Usuario.AcessoNivelDRE) {
                    $("#ddlResultadoNivel_DRE").remove();
                }
            }

            /**
            -----MSTECH-----
             *Basicamente temos:
             -Esconde todas as "pages";
             -Mostra a "page" de resultado;
             -Reseta todos os elementos de filtragem (incluindo SELECTS e CHECKS);
             -Esconde divs de resultados;
             -Desabilita o botão Mostrar Resultado (ele será habilitado no método resultado_configurarControles)
            */
            //resultado-page
            $(".page").hide();
            $("#resultado-page").show();

            if (limparFiltros) {
                $("#ddlResultadoNivel").val("");
                $("#ddlResultadoNivel").selectmenu("refresh");

                $("#ddlResultadoAreaConhecimento").val("");
                $("#ddlResultadoAreaConhecimento").selectmenu("refresh");

                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");

                $("#divResultadoDRE").hide();
                $(".resultado-dre-chk").prop('checked', false).checkboxradio('refresh');

                $("#divResultadoEscola").hide();
                $(".resultado-escola-chk").prop('checked', false).checkboxradio('refresh');

                $("#divResultadoTurma").hide();
                $("#divResultadoAluno").hide();
                $("#divResultadoAlunoItens").html("");
            }

            $("#ddlResultadoNivel").trigger("change");

            $("#btnResultadoApresentar").prop("disabled", true);
        }
        $.mobile.loading("hide");
    }
    catch (error) {
        console.log(error);
    }
};

/* 
    Funcionalidades para Revistas Pedagógicas e Boletins
 */
function abrirConsultaRevistasBoletins() {
    try {
        /**
        -----AMCOM-----
            *Basicamente temos:
            -Esconde todas as "pages";
            -Mostra a "page" de ;
            -Reseta todos os elementos de filtragem (incluindo SELECTS e CHECKS);
            -Esconde divs de revistasBoletins;
            -Desabilita o botão Mostrar RevistasBoletins (ele será habilitado no método revistasBoletins_configurarControles)
        */
        $(".page").hide();
        $("#revistasBoletins-page").show();

        $(".revistasBoletins-dre-chk").prop('checked', false).checkboxradio('refresh');
        carregarListaEscolaRevistasBoletins();

        $("#ddlRevistasBoletinsEdicao").selectmenu("enable");
        $("#ddlRevistasBoletinsEdicao").selectmenu("refresh");

        adicionarItemBackButton("btnRevistasBoletinsVoltar");

        $.mobile.loading("hide");
    }
    catch (error) {
        console.log(error);
    }
}

function aplicarFiltrosRevistasBoletins(edicao, areaConhecimentoId, ciclo, esc_nome) {
    //modifica os filtros de Edição, Área de Conhecimento e Ciclo
    $("#ddlRevistasBoletinsEdicao").val(edicao ? edicao : "");
    $("#ddlRevistasBoletinsEdicao").selectmenu("refresh");

    $("#ddlRevistasBoletinsAreaConhecimento").val(areaConhecimentoId ? areaConhecimentoId : "");
    $("#ddlRevistasBoletinsAreaConhecimento").selectmenu("refresh");

    $("#ddlRevistasBoletinsCiclo").val(ciclo ? ciclo : "");
    $("#ddlRevistasBoletinsCiclo").selectmenu("refresh");

    //cria os componentes de DREs
    revistasBoletins_configurarControles();

    //marca as DREs da consulta de Revistas/Boletins da mesma forma que a consulta de Resultados
    let lista_uad_sigla = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get();
    $(".revistasBoletins-dre-chk").each(function (index) {
        $(this).prop('checked', lista_uad_sigla.includes(this.value)).checkboxradio('refresh');
    });

    //exibe as escolas conforme as DREs selecionadas logo acima
    let promiseCargaEscolas = carregarListaEscolaRevistasBoletins();

    if (esc_nome && promiseCargaEscolas) {
        promiseCargaEscolas.then(() => {
            let _txtRevistasBoletinsEscolaFiltro = $("#txtRevistasBoletinsEscolaFiltro");
            _txtRevistasBoletinsEscolaFiltro.val(esc_nome);
            _txtRevistasBoletinsEscolaFiltro.trigger("change");
            _txtRevistasBoletinsEscolaFiltro.trigger("focus");
        });
    }
}

$("#btnRevistasBoletinsVoltar").unbind("click").click(function () {
    try {
        if (caminhoBackButton && caminhoBackButton.includes("btnRevistasBoletinsVoltar"))
            removerItemBackButton();
        abrirResultados(false);
    }
    catch (error) {
        console.log(error);
    }
});

/**
-----MSTECH-----
 *Fim do Módulo 5.1 - Métodos gerais para resolução de questões específicas
*/


/**
-----MSTECH-----
 Módulo 5.2: Métodos referentes à tela de configurações
 *
*/

/**
-----MSTECH-----
 Evento que direciona para a tela de configurações. De fato, a tela é mostrada com os elementos
 necessários.
*/
function direcionarTelaConfiguracoes() {
    try {
        provaSP_configuracoes.dadosAtuais = JSON.parse(JSON.stringify(provaSP_configuracoes.configuracoes));

        $.mobile.loading("hide");

        adicionarItemBackButton("btnConfiguracoesSair");
        carregarConfiguracoes();

        //Esconder todas as páginas e mostrar apenas a de configuração
        $(".page").hide();
        $("#configuracoes-page").show();
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 Voltar à tela de menu vindo da tela de configurações (apenas layout).
*/
function voltarMenu_deConfiguracoes() {
    try {
        $.mobile.loading("hide");
        removerItemBackButton();

        $(".page").hide();
        $("#menu-page").show();
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 Nova função para carregar as informações da tela de Configurações.
*/
function carregarConfiguracoes() {
    try {
        if (Usuario.AcessoNivelSME) {
            var labelPreenchimentoQuestionarios = document.getElementById("opcaoPreenchimentoQuestionarios");
            var labelRelatorioAcompanhamento = document.getElementById("opcaoRelatorioAcompanhamento");

            //Opção para disponibilização de questionários (sejam eles socioeconômicos ou
            //mesmo aplicações de prova)
            if (provaSP_configuracoes.configuracoes.DisponibilizarPreenchimentoQuestionariosFichas) {
                labelPreenchimentoQuestionarios.innerText = "ATIVADO";
                labelPreenchimentoQuestionarios.style.color = "green";
            }
            else {
                labelPreenchimentoQuestionarios.innerText = "DESATIVADO";
                labelPreenchimentoQuestionarios.style.color = "red";
            }

            //Opção para disponibilização de acesso aos relatórios de acompanhamento
            if (provaSP_configuracoes.configuracoes.RelatorioAcompanhamentoVisivel) {
                labelRelatorioAcompanhamento.innerText = "ATIVADO";
                labelRelatorioAcompanhamento.style.color = "green";
            }
            else {
                labelRelatorioAcompanhamento.innerText = "DESATIVADO";
                labelRelatorioAcompanhamento.style.color = "red";
            }

            //Opção para alterar o mínimo de participação para representatividade segundo o INEP
            $("#confRepresentatividadeSegundoINEP").val(
                provaSP_configuracoes.configuracoes.RepresentatividadeSegundoINEP
            );
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 Evento para a alteração das configurações (Só bool)
*/
function alterarConfiguracao(opcaoAlterada) {
    try {
        if (Usuario.AcessoNivelSME) {
            provaSP_configuracoes.configuracoes[opcaoAlterada] =
                !provaSP_configuracoes.configuracoes[opcaoAlterada];
            carregarConfiguracoes();
        }
    }
    catch (error) {
        console.log(error);
    }
}

$("#confRepresentatividadeSegundoINEP").unbind("change").change(function () {
    try {
        if (Usuario.AcessoNivelSME) {
            if ($("#confRepresentatividadeSegundoINEP").val() < 0 ||
                $("#confRepresentatividadeSegundoINEP").val() > 100) {
                //Default set
                provaSP_configuracoes.configuracoes.RepresentatividadeSegundoINEP = 25;
                $("#confRepresentatividadeSegundoINEP").val("25");

                ProvaSP_Erro("Inválido", "Por favor forneça um valor entre 0 e 100");
            }
            else {
                provaSP_configuracoes.configuracoes.RepresentatividadeSegundoINEP =
                    $("#confRepresentatividadeSegundoINEP").val();
            }
        }
    }
    catch (error) {
        console.log(error);
    }
})

function divConfiguracoes(opcaoSelecionada) {
    try {
        for (var i = 0; i < 3; i++) {
            document.getElementById("configuracoes_div" + i).style.display = "none";
            document.getElementById("configuracoes_icone" + i).innerHTML =
                "<span class='mdi mdi-arrow-down-drop-circle-outline'></span>";
        }

        //Escondendo todas as opções
        if (opcaoSelecionada != -1 && opcaoConfiguracoesSelecionada != opcaoSelecionada) {
            document.getElementById("configuracoes_div" + opcaoSelecionada).style.display = "block";
            document.getElementById("configuracoes_icone" + opcaoSelecionada).innerHTML =
                "<span class='mdi mdi-arrow-up-drop-circle-outline'></span>";
        }
        else { opcaoSelecionada = -1; }
        opcaoConfiguracoesSelecionada = opcaoSelecionada;
    }
    catch (error) {
        console.log(error);
    }
}

function obterCorte() {
    try {
        if (opcaoConfiguracoesSelecionada != 2) {
            $.mobile.loading("show", {
                text: "Obtendo dados de Corte...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            $.ajax({
                url: urlBackEnd + "api/SequenciaDidatica/GetCorte",
                type: "GET",
                dataType: "JSON",
                crossDomain: true,
                success: function (dataCorte) {
                    $.mobile.loading("hide");

                    if (dataCorte.length > 0) {
                        corteCache = dataCorte;
                        montarSelectCorte(dataCorte);
                    }
                    else {
                        if (corteCache.length > 0) { montarSelectCorte(corteCache); }
                        else {
                            ProvaSP_Erro("Não há informações de Corte",
                                "Por favor tente novamente mais tarde.");
                        }
                    }
                },
                error: function (erro) {
                    if (corteCache.length > 0) { montarSelectCorte(corteCache); }
                    else { ProvaSP_Erro("Erro " + erro.status, erro.statusText); }
                }
            });
        }
        else { divConfiguracoes(2); }
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro: " + error);
    }
}

function montarSelectCorte(corteData) {
    try {
        var corteHTML = "";

        corteHTML += "<option value='' selected='selected'>(Corte...)</option>";

        for (var i = 0; i < corteData.length; i++) {
            corteHTML += "<option value='" + corteData[i].CorteId + "'>" + corteData[i].Nome + "</option>";
        }
        $("#ddlConfSDCorte").html(corteHTML);

        //Reset elementos
        $("#detalhesSD").hide();
        $("#ddlConfSDCorte").val("");
        $("#ddlConfSDCorte").selectmenu("refresh");
        $("#btnObterSequenciaDidatica").prop("disabled", true);
        divConfiguracoes(2);
    }
    catch (error) {
        console.log(error);
    }
}

$("#ddlConfSDEdicao,#ddlConfSDAno,#ddlConfSDAreaConhecimento,#ddlConfSDCorte").unbind("change").change(function () {
    try {
        var enableSDBtn = true;

        if ($("#ddlConfSDEdicao").val() != "" && $("#ddlConfSDAno").val() != "" &&
            $("#ddlConfSDAreaConhecimento").val() != "" && $("#ddlConfSDCorte").val() != "") {
            enableSDBtn = false;
        }
        $("#detalhesSD").hide();
        $("#btnObterSequenciaDidatica").prop("disabled", enableSDBtn);
    }
    catch (error) {
        console.log(error);
    }
});

$("#btnObterSequenciaDidatica").unbind("click").click(function () {
    try {
        var edicaoSD = $("#ddlConfSDEdicao").val();
        var anoSD = $("#ddlConfSDAno").val();
        var areaConhecimentoSD = $("#ddlConfSDAreaConhecimento").val();
        var corteSD = $("#ddlConfSDCorte").val();
        var apiURL = urlBackEnd + "api/SequenciaDidatica/GetSequenciaDidatica" +
            "?edicao=" + edicaoSD +
            "&anoEscolar=" + anoSD +
            "&areaConhecimentoId=" + areaConhecimentoSD +
            "&corteId=" + corteSD;

        $.mobile.loading("show", {
            text: "Obtendo sequência de atividades...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.ajax({
            url: apiURL,
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            success: function (dataSequenciaDidatica) {
                $.mobile.loading("hide");

                if (dataSequenciaDidatica != null && Object.keys(dataSequenciaDidatica).length > 0) {
                    $("#tituloSD").val(dataSequenciaDidatica.Titulo);
                    $("#textSD").val(dataSequenciaDidatica.Texto);
                    $("#linkSD").val(dataSequenciaDidatica.Link);
                }
                else {
                    $("#tituloSD").val("");
                    $("#textSD").val("");
                    $("#linkSD").val("");
                }
                $("#detalhesSD").show();
            },
            error: function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            }
        });
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro: " + error);
    }
});

$("#btnCommitSD").unbind("click").click(function () {
    try {
        var tituloSD = $("#tituloSD").val();
        var textoSD = $("#textSD").val();
        var linkSD = $("#linkSD").val();

        if (tituloSD.length == 0 || textoSD.length == 0) {
            ProvaSP_Erro("Dados inválidos", "Por favor preencha os campos de Título e Texto para continuar.");
        }
        else if (!validURL(linkSD)) {
            ProvaSP_Erro("Link inválido", "Por favor o campo Link com uma URL válida.");
        }
        else {
            var objSDEnvio = {
                Edicao: $("#ddlConfSDEdicao").val(),
                AnoEscolar: $("#ddlConfSDAno").val(),
                AreaConhecimentoId: $("#ddlConfSDAreaConhecimento").val(),
                CorteId: $("#ddlConfSDCorte").val(),
                Titulo: tituloSD,
                Texto: textoSD,
                Link: linkSD
            };
            var objSDEnvioString = JSON.stringify(objSDEnvio);

            $.mobile.loading("show", {
                text: "Cadastrando informações...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            $.post(urlBackEnd + "api/SequenciaDidatica/Salvar?guid=" + newGuid(), { json: objSDEnvioString })
                .done(function (resultSD) {
                    $.mobile.loading("hide");
                    swal("Obrigado!", "As informações da Sequência de Atividades foram salvas com sucesso!", "success");
                })
                .fail(function (erro) {
                    ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                });
        }
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro: " + error);
    }
});

function validURL(str) {
    try {
        var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
            '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return !!pattern.test(str);
    }
    catch (error) {
        console.log(error);
    }
    return false;
}

$("#ddlSDEdicao").unbind("change").change(function () {
    try {
        $("#SDResultados").hide();
        $("#SDResultadosBlocos").html("");

        $("#ddlSDCiclo").val("");
        if ($("#ddlSDEdicao").val() == "") {
            $("#ddlSDCiclo").selectmenu("disable");
        }
        else {
            $("#ddlSDCiclo").selectmenu("enable");
        }
        $("#ddlSDCiclo").selectmenu("refresh");

        $("#ddlSDAreaConhecimento").val("");
        $("#ddlSDAreaConhecimento").selectmenu("disable");
        $("#ddlSDAreaConhecimento").selectmenu("refresh");
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlSDCiclo").unbind("change").change(function () {
    try {
        $("#SDResultados").hide();
        $("#SDResultadosBlocos").html("");

        $("#ddlSDAreaConhecimento").val("");
        if ($("#ddlSDCiclo").val() == "") {
            $("#ddlSDAreaConhecimento").selectmenu("disable");
        }
        else {
            $("#ddlSDAreaConhecimento").selectmenu("enable");
        }
        $("#ddlSDAreaConhecimento").selectmenu("refresh");
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlSDAreaConhecimento").unbind("change").change(function () {
    try {
        $("#SDResultados").hide();
        $("#SDResultadosBlocos").html("");

        var edicaoResultadoSD = $("#ddlSDEdicao").val();
        var cicloResultadoSD = $("#ddlSDCiclo").val();
        var areaConhecimentoResultadoSD = $("#ddlSDAreaConhecimento").val();

        if (edicaoResultadoSD != "" && cicloResultadoSD != "" && areaConhecimentoResultadoSD != "") {
            var apiSDResultado = urlBackEnd + "api/SequenciaDidatica/SelecionarSequenciasDidaticas" +
                "?edicao=" + edicaoResultadoSD +
                "&cicloId=" + cicloResultadoSD +
                "&areaConhecimentoId=" + areaConhecimentoResultadoSD;

            $.mobile.loading("show", {
                text: "Obtendo sequências de atividades...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            $.ajax({
                url: apiSDResultado,
                type: "GET",
                dataType: "JSON",
                crossDomain: true,
                success: function (dataSDResultado) {
                    $.mobile.loading("hide");

                    if (dataSDResultado.length > 0) {
                        montarSequenciaDidatica(dataSDResultado);
                    }
                    else {
                        ProvaSP_Erro("Não há informações ",
                            "Ainda não há informações sobre a Sequência de Atividades selecionada." +
                            "\n\nPor favor tente novamente.");
                    }
                },
                error: function (erro) {
                    ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                }
            });
        }
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro: " + error);
    }
});

function montarSequenciaDidatica(resultadoSequenciaDidatica) {
    try {
        var SDHTML = "";

        for (var i = 0; i < resultadoSequenciaDidatica.length; i++) {
            SDHTML += "<p class='sequenciadidatica_headerTitle'>";
            SDHTML += resultadoSequenciaDidatica[i].Nome;
            SDHTML += "</p>";

            SDHTML += "<div class='sequenciadidatica_mainDiv'>";
            for (var j = 0; j < resultadoSequenciaDidatica[i].SequenciasDidaticas.length; j++) {
                //Sequência de atividades atual
                var sda = resultadoSequenciaDidatica[i].SequenciasDidaticas[j];

                SDHTML += "<div class='sequenciadidatica_blockDiv'>";
                SDHTML += "<table class='sequenciadidatica_blockTable'>";
                SDHTML += "<tr>";
                SDHTML += "<td class='sequenciadidatica_blockTDTitle'>";
                SDHTML += "<p class='sequenciadidatica_blockTitle'>" + sda.AnoEscolar + "º Ano</p>";
                SDHTML += "</td>";
                SDHTML += "<td onclick=\"abrirLinkSD('" + sda.Link +
                    "')\" class='sequenciadidatica_blockTDIcon'>";
                SDHTML += "<p class='sequenciadidatica_blockIcon'>";
                SDHTML += "<span class='mdi mdi-link'></span>";
                SDHTML += "</p>";
                SDHTML += "</td>";
                SDHTML += "</tr>";
                SDHTML += "</table>";
                SDHTML += "<div class='sequenciadidatica_blockTextDiv'>";
                SDHTML += "<p class='sequenciadidatica_blockText'>";
                SDHTML += "<b>" + sda.Titulo + "</b> " + sda.Texto;
                SDHTML += "</p>";
                SDHTML += "</div>";
                SDHTML += "</div>";
            }
        }
        SDHTML += "</div>";
        $("#SDResultadosBlocos").html(SDHTML);
        $("#SDResultados").show();
    }
    catch (error) {
        console.log(error);
    }
}

function abrirLinkSD(urlSD) {
    try {
        if (mobile) {
            if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN) {
                ProvaSP_Erro("Sem conexão",
                    "Por favor verifique sua conexão com a internet e tente novamente mais tarde.");
                return;
            }
            else { navigator.app.loadUrl(urlSD, { openExternal: true }); }
        }
        else { window.open(urlSD); }
    }
    catch (error) {
        console.log(error);
    }
}

function limparCamposConfiguracaoProficiencia() {
    try {
        $("#confAnoCiclo").val("");
        $("#confNivelProficiência").val("");
        $("#confProficienciaNome").val("");
        $("#confProficienciaDescricao").val("");

        $("#confAnoCiclo").selectmenu("refresh");
        $("#confNivelProficiência").selectmenu("refresh");

        $.mobile.silentScroll(0);
    }
    catch (error) {
        console.log(error);
    }
}
/**
-----MSTECH-----
 *Fim do Módulo 5.2 - Tela de Configurações
*/


/**
-----MSTECH-----
 Módulo 5.3: Prova do Aluno - Métodos para baixar e mostrar a prova física do Aluno
 *
*/

/**
-----MSTECH-----
 Novo método para abrir as imagens da prova real de um aluno específico baseado no Ano selecionado.
 O App receberá um retorno do servidor com as urls das imagens da prova real do aluno.
*/
function baixarProvaAlunoPorAno(alunoIndividual, alunoString) {
    try {
        var urlImagensAluno = "";
        var alunoStringArray = alunoString.split("_");
        var areaConhecimentoID = alunoStringArray[0];

        //MSTECH - Atualizando a área de conhecimento em seleção de aluno quando muda no form.
        //Este trecho executa quando não é um aluno individual
        if (!alunoIndividual) {
            areaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
        }

        urlImagensAluno = urlBackEnd + "api/" +
            "ImagemAluno?Edicao=" + alunoStringArray[2] +
            "&AreaConhecimentoID=" + areaConhecimentoID +
            "&alu_matricula=" + alunoStringArray[1];

        $.mobile.loading("show", {
            text: "Obtendo informações...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.ajax({
            url: urlImagensAluno,
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            success: function (dataResultado) {
                $.mobile.loading("hide");

                if (dataResultado.length > 0) {
                    popupProvaAluno(dataResultado);
                }
                else {
                    ProvaSP_Erro("Não há arquivos",
                        "Não existem registros de arquivos referentes à prova do Aluno para " +
                        "os filtros selecionados.");
                }
            },
            error: function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            }
        });
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 Mostrar popup com os arquivos da prova física do Aluno quando houver arquivos
*/
function popupProvaAluno(serverRetorno) {
    try {
        var alunoLabel = false;

        $("#provaaluno_urlsDiv").empty();
        adicionarItemBackButton("btnProvaAlunoVoltar");

        for (var i = 0; i < serverRetorno.length; i++) {
            var itemProvaDownloaHTML = "";

            if (serverRetorno[i].hasOwnProperty("alu_nome") && !alunoLabel) {
                document.getElementById("provaAluno_NomeAluno").innerText = serverRetorno[i].alu_nome;
            }

            //Linha com link para imagem
            itemProvaDownloaHTML += "<p class='provaaluno_item' onclick=\"abrirArquivoAluno('" +
                serverRetorno[i].caminho + "')\"><span class='provaaluno_downloadBtn'>Visualizar prova</span>" +
                "<span class='mdi mdi-open-in-new'></span> Página: " + serverRetorno[i].pagina + " - " +
                serverRetorno[i].questao + " (" + serverRetorno[i].Edicao +
                ")</p>";

            //Critérios de redação
            itemProvaDownloaHTML += "<p class='provaaluno_criterios'>";
            for (var j = 1; j < 6; j++) {
                if (serverRetorno[i].hasOwnProperty("REDQ" + j)) {
                    itemProvaDownloaHTML += "Critério " + j + ": ";
                    itemProvaDownloaHTML += serverRetorno[i]["REDQ" + j];
                    itemProvaDownloaHTML += "<br />";
                }
            }
            itemProvaDownloaHTML += "</p>";
            $("#provaaluno_urlsDiv").append(itemProvaDownloaHTML);

            //Obtendo Ciclo correspondente
            provaAlunoCicloSelecionado = popupProvaAlunoObtemCiclo();
        }
        $(".page").hide();
        $("#provaalunopopup-page").show();
        $.mobile.silentScroll(0);
    }
    catch (error) {
        console.log(error);
    }
}

function popupProvaAlunoObtemCiclo() {
    try {
        var anoCorrespondente = "";

        //Configurando Ciclo
        if (Usuario.Aluno) {
            anoCorrespondente = parseInt(Usuario.Ano); //Usuário aluno
        }
        else {
            var ciclo = $("#ddlResultadoCiclo").val();

            if (ciclo != "") { return parseInt(ciclo); } //Filtro de ciclo
            else { anoCorrespondente = parseInt($("#ddlResultadoAno").val()); } //Filtro de ano
        }

        return buscarCicloPeloAnoLetivo(anoCorrespondente);
    }
    catch (error) {
        console.log(error);
    }
    return 1;
}

function buscarCicloPeloAnoLetivo(anoLetivo) {
    if (anoLetivo == 2 || anoLetivo == 3) { return 1; } //Alfabetização
    else if (anoLetivo >= 4 && anoLetivo <= 6) { return 2; } //Interdisciplinar
    else if (anoLetivo >= 7 && anoLetivo <= 9) { return 3; } //Autoral
}

$("#btnProvaAlunoCriterios").unbind("click").click(function () {
    try {
        var disciplinaID = $("#ddlResultadoAreaConhecimento").val();
        var linkCriterio = provaSP_configuracoes.configuracoes.UrlImagemAlunos +
            "CriterioCorrecao/" + disciplinaID + "/" + provaAlunoCicloSelecionado + ".pdf";

        if (mobile) {
            if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN) {
                ProvaSP_Erro("Sem conexão",
                    "Por favor verifique sua conexão com a internet para obter a descrição dos critérios.");
                return;
            }
            else {
                navigator.app.loadUrl(linkCriterio, { openExternal: true });
            }
        }
        else {
            window.open(linkCriterio);
        }
    }
    catch (error) {
        console.log(error);
    }
});

/**
-----MSTECH-----
 Abre arquivo da prova física do Aluno - Redirecionamento WEB
*/
function abrirArquivoAluno(urlArquivo) {
    try {
        if (mobile) {
            if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN) {
                ProvaSP_Erro("Sem conexão",
                    "Por favor verifique sua conexão com a internet para abrir este arquivo.");
                return;
            }
            else {
                navigator.app.loadUrl(
                    provaSP_configuracoes.configuracoes.UrlImagemAlunos + urlArquivo, { openExternal: true }
                );
            }
        }
        else {
            window.open(provaSP_configuracoes.configuracoes.UrlImagemAlunos + urlArquivo);
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 Mostra o conjunto de elementos correspondentes à opção de RESULTADOS DA PROVASP escolhida.
*/
function divResultadoProva(opcaoSelecionada) {
    try {
        for (let i = 0; i < 5; i++) {
            let resultadoDiv = document.getElementById("resultados_div" + i);
            //alguma das opções podem não ter o DIV de conteúdo
            if (resultadoDiv == null)
                continue;
            resultadoDiv.style.display = "none";
            document.getElementById("resultados_icone" + i).innerHTML =
                "<span class='mdi mdi-arrow-down-drop-circle-outline'></span>";
        }

        //Escondendo todas as opções
        if (opcaoSelecionada != -1 && opcaoResultadoSelecionada != opcaoSelecionada) {
            document.getElementById("resultados_div" + opcaoSelecionada).style.display = "block";
            document.getElementById("resultados_icone" + opcaoSelecionada).innerHTML =
                "<span class='mdi mdi-arrow-up-drop-circle-outline'></span>";
        }
        else { opcaoSelecionada = -1; }
        opcaoResultadoSelecionada = opcaoSelecionada;
    }
    catch (error) {
        console.log(error);
    }
}
/**
-----MSTECH-----
 *Fim do Módulo 5.3 - Prova do Aluno
*/


/**
-----MSTECH-----
 Módulo 5.4: Gráficos de Ciclo de Aprendizagem
 *
*/
function downloadResultadosCiclos(indice, objEnvio, cicloSelecionado) {
    try {
        if (modeloCiclos["Ciclo" + cicloSelecionado][indice] != -1) {
            var graficoAlfabetizacao = false;
            var objEnvioSelecionado = JSON.parse(JSON.stringify(objEnvio));
            var anoEscolarAtual = modeloCiclos["Ciclo" + cicloSelecionado][indice];

            objEnvioSelecionado["Ciclo"] = "";
            objEnvioSelecionado["AnoEscolar"] = anoEscolarAtual;

            $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), objEnvioSelecionado)
                .done(function (dataResultado) {
                    var indiceGrafico = (indice + 1);

                    /**
                    -----MSTECH-----
                     *No sucesso da requisição, mostra resultados.
                    */
                    if (dataResultado.Agregacao.length > 0) {
                        for (var i = 0; i < dataResultado.Agregacao.length; i++) {
                            if (cicloSelecionado == "1" && indice == 1) {
                                graficoResultadoCicloAprendizagem2Ano(dataResultado.Agregacao[i], i);
                            }

                            //Gráfico ciclo de aprendizagem
                            graficoResultadoCicloAprendizagem(
                                indiceGrafico, dataResultado.Agregacao[i], anoEscolarAtual, i
                            );
                        }

                        //Série histórica
                        serieHistorica.anoAtual["agregacao_indice" + indiceGrafico] = dataResultado.Agregacao;
                        downloadSerieHistoricaAnoAnterior(indiceGrafico, objEnvioSelecionado, cicloSelecionado);
                    }
                    else {
                        limparCicloAprendizagem(indiceGrafico);
                    }
                })
                .fail(function (erro) {
                    ProvaSP_Erro("Erro " + erro.status, erro.statusText);
                });
        }
    }
    catch (error) {
        console.log(error);
    }
};

function downloadSerieHistoricaAnoAnterior(indice, objEnvio, ciclo) {
    try {
        var objEnvioSerieHistorica = JSON.parse(JSON.stringify(objEnvio));
        var edicaoAnterior = parseInt(objEnvioSerieHistorica.Edicao) - 1;
        objEnvioSerieHistorica.Edicao = edicaoAnterior.toString();

        $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), objEnvioSerieHistorica)
            .done(function (dataResultado) {
                serieHistorica.anoAnterior["agregacao_indice" + indice] = dataResultado.Agregacao;
                var sAn = serieHistorica.anoAnterior;
                var sAt = serieHistorica.anoAtual;

                //When all the object is filled
                if (Object.keys(sAn).length > 0 && Object.keys(sAt).length > 0) {
                    if (ciclo != "1") {
                        if ((sAn.hasOwnProperty("agregacao_indice1") && sAn.hasOwnProperty("agregacao_indice2") &&
                            sAn.hasOwnProperty("agregacao_indice3")) && (sAt.hasOwnProperty("agregacao_indice1") &&
                                sAt.hasOwnProperty("agregacao_indice2") && sAt.hasOwnProperty("agregacao_indice3"))) {
                            graficoResultadoSerieHistorica(objEnvioSerieHistorica, false);
                        }
                    }
                    else { //Série histórica do ciclo de alfabetização
                        if ((sAt.hasOwnProperty("agregacao_indice2") && sAt.hasOwnProperty("agregacao_indice3")) &&
                            (sAn.hasOwnProperty("agregacao_indice2") && sAn.hasOwnProperty("agregacao_indice3"))) {
                            graficoResultadoSerieHistorica(objEnvioSerieHistorica, true);
                        }
                    }
                }
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
    }
    catch (error) {
        console.log(error);
    }
};

function baseGraficoAprendizagem(indiceAgregacao) {
    try {
        var baseGraficoHTML = "";

        //Título só é necessário para alfabetização por conta dos 3 gráficos sobrepostos
        baseGraficoHTML += "<div class='resultados_graficoAprendizagemChartDiv2'>";
        baseGraficoHTML += "<div id='divResultadoCiclo1_" + indiceAgregacao + "'></div>";
        baseGraficoHTML += "<div id='divResultadoCiclo2_" + indiceAgregacao + "'></div>";
        baseGraficoHTML += "<div id='divResultadoCiclo3_" + indiceAgregacao + "'></div>";
        baseGraficoHTML += "</div>";

        return baseGraficoHTML;
    }
    catch (error) {
        console.log(error);
        return "";
    }
}

function graficoResultadoCicloAprendizagem2Ano(dataServidor, indiceAgregacao) {
    try {
        var chartResultadoCicloAprendizagem2Ano_ctx = null;

        /**
        -----MSTECH-----
         *Novos gráficos para ciclo de aprendizagem. Deve aparecer somente se for o ciclo de aprendizagem 1
         ALFABETIZAÇÃO
        */
        $("#divResultadoCiclo2Ano_" + indiceAgregacao).empty("");

        $("#divResultadoCiclo2Ano_" + indiceAgregacao).append(
            "<canvas id='chartResultadoCiclo2Ano_" + indiceAgregacao + "'></canvas>"
        );
        chartResultadoCicloAprendizagem2Ano_ctx =
            document.getElementById("chartResultadoCiclo2Ano_" + indiceAgregacao).getContext("2d");
        chartResultadoCicloAprendizagem2Ano_ctx.canvas.height = 150;

        var donut2Ano = new Chart(chartResultadoCicloAprendizagem2Ano_ctx, {
            type: 'doughnut',
            data: {
                labels: ["Não alfabetizados", "Alfabetizados"],
                datasets: [
                    {
                        backgroundColor: ["#9C9B9B", "#AAD3A6"],
                        data: [
                            parseFloat(100 - dataServidor.PercentualAlfabetizado).toFixed(2),
                            dataServidor.PercentualAlfabetizado
                            //parseInt(dataServidor.TotalAlunos * ((100 - dataServidor.PercentualAlfabetizado) / 100)),
                            //parseInt(dataServidor.TotalAlunos * (dataServidor.PercentualAlfabetizado / 100))
                        ]
                    }
                ]
            },

            options: {
                //showAllTooltips: false,
                title: {
                    display: true,
                    text: "Ciclo de aprendizagem - Alfabetização 2º Ano",
                    fontFamily: "'Open Sans Bold', sans-serif",
                    fontSize: 15,
                },
                legend: {
                    position: "right"
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            var dataset = data.datasets[tooltipItem.datasetIndex];
                            return data.labels[tooltipItem.index] + ": " + dataset.data[tooltipItem.index] + "%";
                        }
                    }
                }
            }
        });
    }
    catch (error) {
        console.log(error);
    }
}

function graficoResultadoCicloAprendizagem(indiceGrafico, dataServidor, anoEscolarGrafico, indiceAgregacao) {
    try {
        var proficienciasGrafico = [
            { Nome: "Indefinido" },
            { Nome: "Abaixo do básico" },
            { Nome: "Básico" },
            { Nome: "Adequado" },
            { Nome: "Avançado" }
        ];

        if (dataServidor.hasOwnProperty("Proficiencias")) {
            proficienciasGrafico = dataServidor.Proficiencias;
        }

        var chartResultadoCicloAprendizagem1_ctx = null;
        var chartResultadoCicloAprendizagem2_ctx = null;
        var chartResultadoCicloAprendizagem3_ctx = null;

        /**
        -----MSTECH-----
         *Novos gráficos para ciclo de aprendizagem. Deve aparecer somente se for o ciclo de aprendizagem 1
         ALFABETIZAÇÃO
        */
        if (indiceGrafico == 1) {
            $("#divResultadoCiclo1_" + indiceAgregacao).empty("");
            var GA_Div1 = document.getElementById("divResultadoCiclo1_" + indiceAgregacao);

            $("#divResultadoCiclo1_" + indiceAgregacao).append(
                "<canvas id='chartResultadoCiclo1_" + indiceAgregacao + "' style='width: 160px; height: 150px;'>" +
                "</canvas>"
            );
            var chartResultadoCicloAprendizagem1_ctx =
                document.getElementById("chartResultadoCiclo1_" + indiceAgregacao).getContext("2d");

            GA_Div1.className = "resultados_graficoBlock";

            var donut1 = new Chart(chartResultadoCicloAprendizagem1_ctx, {
                type: 'doughnut',
                data: {
                    labels: [
                        proficienciasGrafico[1].Nome,
                        proficienciasGrafico[2].Nome,
                        proficienciasGrafico[3].Nome,
                        proficienciasGrafico[4].Nome
                    ],
                    datasets: [
                        {
                            backgroundColor: ["#FF5959", "#FEDE99", "#9999FF", "#99FF99"],
                            data: [
                                dataServidor.PercentualAbaixoDoBasico,
                                dataServidor.PercentualBasico,
                                dataServidor.PercentualAdequado,
                                dataServidor.PercentualAvancado
                            ]
                        }
                    ]
                },
                options: {
                    cutoutPercentage: 50,
                    responsive: false,
                    elements: { arc: { borderWidth: 0 } },
                    legend: { display: false },
                    title: {
                        display: true,
                        text: anoEscolarGrafico + 'º Ano'
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem, data) {
                                var dataset = data.datasets[tooltipItem.datasetIndex];
                                return dataset.data[tooltipItem.index] + "%";
                            },
                            footer: function (tooltipItemArray, data) {
                                return data.labels[tooltipItemArray[0].index];
                            }
                        }
                    }
                }
            });
        }
        else if (indiceGrafico == 2) {
            $("#divResultadoCiclo2_" + indiceAgregacao).empty("");
            var GA_Div2 = document.getElementById("divResultadoCiclo2_" + indiceAgregacao);

            $("#divResultadoCiclo2_" + indiceAgregacao).append(
                "<canvas id='chartResultadoCiclo2_" + indiceAgregacao + "' style='width: 160px; height: 150px;'>" +
                "</canvas>"
            );
            chartResultadoCicloAprendizagem2_ctx =
                document.getElementById("chartResultadoCiclo2_" + indiceAgregacao).getContext("2d");

            GA_Div2.className = "resultados_graficoBlock";

            var donut2 = new Chart(chartResultadoCicloAprendizagem2_ctx, {
                type: 'doughnut',
                data: {
                    labels: [
                        proficienciasGrafico[1].Nome,
                        proficienciasGrafico[2].Nome,
                        proficienciasGrafico[3].Nome,
                        proficienciasGrafico[4].Nome
                    ],
                    datasets: [
                        {
                            backgroundColor: ["#FF5959", "#FEDE99", "#9999FF", "#99FF99"],
                            data: [
                                dataServidor.PercentualAbaixoDoBasico,
                                dataServidor.PercentualBasico,
                                dataServidor.PercentualAdequado,
                                dataServidor.PercentualAvancado
                            ]
                        }
                    ]
                },
                options: {
                    cutoutPercentage: 50,
                    responsive: false,
                    elements: { arc: { borderWidth: 0 } },
                    legend: { display: false },
                    title: {
                        display: true,
                        text: anoEscolarGrafico + 'º Ano'
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem, data) {
                                var dataset = data.datasets[tooltipItem.datasetIndex];
                                return dataset.data[tooltipItem.index] + "%";
                            },
                            footer: function (tooltipItemArray, data) {
                                return data.labels[tooltipItemArray[0].index];
                            }
                        }
                    }
                }
            });
        }
        else if (indiceGrafico == 3) {
            $("#divResultadoCiclo3_" + indiceAgregacao).empty("");
            var GA_Div3 = document.getElementById("divResultadoCiclo3_" + indiceAgregacao);

            $("#divResultadoCiclo3_" + indiceAgregacao).append(
                "<canvas id='chartResultadoCiclo3_" + indiceAgregacao + "' style='width: 160px; height: 150px;'>" +
                "</canvas>"
            );
            chartResultadoCicloAprendizagem3_ctx =
                document.getElementById("chartResultadoCiclo3_" + indiceAgregacao).getContext("2d");

            GA_Div3.className = "resultados_graficoBlock";

            var donut3 = new Chart(chartResultadoCicloAprendizagem3_ctx, {
                type: 'doughnut',
                data: {
                    labels: [
                        proficienciasGrafico[1].Nome,
                        proficienciasGrafico[2].Nome,
                        proficienciasGrafico[3].Nome,
                        proficienciasGrafico[4].Nome
                    ],
                    datasets: [
                        {
                            backgroundColor: ["#FF5959", "#FEDE99", "#9999FF", "#99FF99"],
                            data: [
                                dataServidor.PercentualAbaixoDoBasico,
                                dataServidor.PercentualBasico,
                                dataServidor.PercentualAdequado,
                                dataServidor.PercentualAvancado
                            ]
                        }
                    ]
                },
                options: {
                    cutoutPercentage: 50,
                    responsive: false,
                    elements: { arc: { borderWidth: 0 } },
                    legend: { display: false },
                    title: {
                        display: true,
                        text: anoEscolarGrafico + 'º Ano'
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem, data) {
                                var dataset = data.datasets[tooltipItem.datasetIndex];
                                return dataset.data[tooltipItem.index] + "%";
                            },
                            footer: function (tooltipItemArray, data) {
                                return data.labels[tooltipItemArray[0].index];
                            }
                        }
                    }
                }
            });
        }
    }
    catch (error) {
        console.log(error);
    }
}

function cicloAprendizagemLegenda(flagAlfabetizados, totalAlunos, percentualAlfabetizados) {
    try {
        var minhaLegenda = "";

        if (flagAlfabetizados) {
            minhaLegenda = parseInt(totalAlunos * (percentualAlfabetizados / 100)) + " Alunos (" +
                percentualAlfabetizados.toFixed(2) + "%)";
        }
        else {
            minhaLegenda =
                parseInt(totalAlunos * ((100 - percentualAlfabetizados) / 100)) + " Alunos (" +
                (100 - percentualAlfabetizados.toFixed(2)) + "%)";
        }
        return minhaLegenda;
    }
    catch (error) {
        console.log(error);
    }
    return "";
}

function limparCicloAprendizagem(param) {
    try {
        if (param == "") {
            $("#divResultadoCiclo1").empty("");
            $("#divResultadoCiclo2").empty("");
            $("#divResultadoCiclo3").empty("");
        }
        else {
            $("#divResultadoCiclo" + param).empty("");

            if ($("#divResultadoCiclo1").html() == "" && $("#divResultadoCiclo2").html() == "" &&
                $("#divResultadoCiclo3").html() == "") {
            }
        }
    }
    catch (error) {
        console.log(error);
    }
}

function baseGraficoSerieHistorica(indiceAgregacao, acID) {
    try {
        var baseGraficoHTML = "";

        //Título só é necessário para alfabetização por conta dos 3 gráficos sobrepostos
        baseGraficoHTML += "<div class='resultados_graficoSerieHistoricaMainDiv' style='display: none;'>";
        baseGraficoHTML += "<p class='resultados_graficoAprendizagemTitle'>";
        baseGraficoHTML += "Série histórica - " + areasConhecimento[parseInt(acID) - 1];
        baseGraficoHTML += "</p>";
        baseGraficoHTML += "<div class='resultados_graficoSerieHistorica'>";
        baseGraficoHTML += "<div id='divResultadoSerieHistorica_" + indiceAgregacao + "'></div>";
        baseGraficoHTML += "</div>";
        baseGraficoHTML += "</div>";

        return baseGraficoHTML;
    }
    catch (error) {
        console.log(error);
        return "";
    }
}

function graficoResultadoSerieHistorica(baseOBJ, flagAlfabetizacao) {
    try {
        var chartLabelsPorCiclo = [];
        var chartResultadoSerieHistorica_ctx = null;
        var coresPorAreaConhecimento = [
            { colorBackground: "#6B91A8", colorBorder: "#59798C" }, //Ciências da Natureza
            { colorBackground: "#F4CA00", colorBorder: "#D8B100" }, //Língua Portuguesa
            { colorBackground: "#E97457", colorBorder: "#CE654E" }, //Matemática
            { colorBackground: "#FF6A00", colorBorder: "#E55B00" } //Redação
        ];
        var chartBack = coresPorAreaConhecimento[parseInt(baseOBJ.AreaConhecimentoID) - 1].colorBackground;
        var chartBorder = coresPorAreaConhecimento[parseInt(baseOBJ.AreaConhecimentoID) - 1].colorBorder;

        if (baseOBJ.AnoEscolar < 4) { chartLabelsPorCiclo = ["2º Ano", "3º Ano"]; }
        else if (baseOBJ.AnoEscolar >= 4 && baseOBJ.AnoEscolar <= 6) { chartLabelsPorCiclo = ["4º Ano", "5º Ano", "6º Ano"]; }
        else { chartLabelsPorCiclo = ["7º Ano", "8º Ano", "9º Ano"]; }

        for (var i = 0; i < serieHistorica.anoAtual.agregacao_indice3.length; i++) {
            $("#divResultadoSerieHistorica_" + i).empty("");

            $("#divResultadoSerieHistorica_" + i).append(
                "<canvas id='chartResultadoSerieHistorica_" + i + "'></canvas>"
            );
            chartResultadoSerieHistorica_ctx =
                document.getElementById("chartResultadoSerieHistorica_" + i).getContext("2d");

            var valoresAlfabetizacao = {
                anoAtual: { agregacao_indice1: null, agregacao_indice2: null, agregacao_indice3: null },
                anoAnterior: { agregacao_indice1: null, agregacao_indice2: null, agregacao_indice3: null }
            };

            for (var j in valoresAlfabetizacao) {
                for (var k in valoresAlfabetizacao[j]) {
                    if (serieHistorica[j].hasOwnProperty(k)) {
                        if (serieHistorica[j][k].length > 0) {
                            if (serieHistorica[j][k][i].hasOwnProperty("Valor")) {
                                valoresAlfabetizacao[j][k] =
                                    serieHistorica[j][k][i].Valor;
                            }
                        }
                    }
                }
            }

            if (flagAlfabetizacao) {
                var linesSerieHistorica = new Chart(chartResultadoSerieHistorica_ctx, {
                    type: 'line',
                    data: {
                        labels: chartLabelsPorCiclo,
                        datasets: [
                            {
                                lineTension: 0,
                                label: parseInt(baseOBJ.Edicao) + 1,
                                data: [
                                    valoresAlfabetizacao.anoAtual.agregacao_indice2,
                                    valoresAlfabetizacao.anoAtual.agregacao_indice3
                                ],
                                backgroundColor: chartBack,
                                borderColor: chartBorder,
                                fill: false,
                                pointRadius: 7.5,
                                pointHoverRadius: 7.5
                            },
                            {
                                lineTension: 0,
                                label: parseInt(baseOBJ.Edicao),
                                data: [
                                    valoresAlfabetizacao.anoAnterior.agregacao_indice2,
                                    valoresAlfabetizacao.anoAnterior.agregacao_indice3
                                ],
                                backgroundColor: chartBack,
                                borderColor: chartBorder,
                                fill: false,
                                borderDash: [5, 5],
                                pointRadius: 7.5,
                                pointHoverRadius: 7.5
                            }
                        ]
                    },
                    options: {
                        legend: {
                            //position: 'right',
                            labels: { useLineStyle: true }
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                }
                            }],
                            yAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                ticks: {
                                    display: false
                                }
                            }]
                        },
                        animation: {
                            duration: 0,
                            onComplete: function () {
                                // render the value of the chart above the bar
                                var ctx = this.chart.ctx;
                                ctx.font = Chart.helpers.fontString(
                                    Chart.defaults.global.defaultFontSize,
                                    'bold',
                                    Chart.defaults.global.defaultFontFamily
                                );
                                ctx.fillStyle = this.chart.config.options.defaultFontColor;
                                ctx.textAlign = 'center';
                                ctx.textBaseline = 'bottom';
                                this.data.datasets.forEach(function (dataset) {
                                    for (var i = 0; i < dataset.data.length; i++) {
                                        var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                                        ctx.fillText(dataset.data[i], model.x, model.y - 5);
                                    }
                                });
                            }
                        }
                    }
                });
            }
            else {
                var linesSerieHistorica = new Chart(chartResultadoSerieHistorica_ctx, {
                    type: 'line',
                    data: {
                        labels: chartLabelsPorCiclo,
                        datasets: [
                            {
                                lineTension: 0,
                                label: parseInt(baseOBJ.Edicao) + 1,
                                data: [
                                    valoresAlfabetizacao.anoAtual.agregacao_indice1,
                                    valoresAlfabetizacao.anoAtual.agregacao_indice2,
                                    valoresAlfabetizacao.anoAtual.agregacao_indice3
                                ],
                                backgroundColor: chartBack,
                                borderColor: chartBorder,
                                fill: false,
                                pointRadius: 7.5,
                                pointHoverRadius: 7.5
                            },
                            {
                                lineTension: 0,
                                label: parseInt(baseOBJ.Edicao),
                                data: [
                                    valoresAlfabetizacao.anoAnterior.agregacao_indice1,
                                    valoresAlfabetizacao.anoAnterior.agregacao_indice2,
                                    valoresAlfabetizacao.anoAnterior.agregacao_indice3
                                ],
                                backgroundColor: chartBack,
                                borderColor: chartBorder,
                                fill: false,
                                borderDash: [5, 5],
                                pointRadius: 7.5,
                                pointHoverRadius: 7.5
                            }
                        ]
                    },
                    options: {
                        legend: {
                            //position: 'right',
                            labels: { useLineStyle: true }
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                }
                            }],
                            yAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                ticks: {
                                    display: false
                                }
                            }]
                        },
                        animation: {
                            duration: 0,
                            onComplete: function () {
                                // render the value of the chart above the bar
                                var ctx = this.chart.ctx;
                                ctx.font = Chart.helpers.fontString(
                                    Chart.defaults.global.defaultFontSize,
                                    'bold',
                                    Chart.defaults.global.defaultFontFamily
                                );
                                ctx.fillStyle = this.chart.config.options.defaultFontColor;
                                ctx.textAlign = 'center';
                                ctx.textBaseline = 'bottom';
                                this.data.datasets.forEach(function (dataset) {
                                    for (var i = 0; i < dataset.data.length; i++) {
                                        var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;
                                        ctx.fillText(dataset.data[i], model.x, model.y - 5);
                                    }
                                });
                            }
                        }
                    }
                });
            }
        }
        mostrarGraficosSerieHistorica();
    }
    catch (error) {
        console.log(error);
    }
}

function mostrarGraficosSerieHistorica() {
    try {
        //MSTECH - Mostrar gráficos da série histórica apenas quando houver dados
        var divsSH = document.getElementsByClassName("resultados_graficoSerieHistoricaMainDiv");

        for (var i = 0; i < divsSH.length; i++) {
            divsSH[i].style.display = "block";
        }
    }
    catch (error) {
        console.log(error);
    }
}
/**
-----MSTECH-----
 *Fim do Módulo 5.4 - Ciclo de Aprendizagem
*/

/**
-----MSTECH-----
 *Fim do Módulo 5.5 - Participação
*/

/**
-----MSTECH-----
 Mostrar popup de participação da ProvaSP pela tela de filtros
*/
function popupParticipacao() {
    try {
        var objParticipacaoEnvio = {};
        var nivel = $("#ddlResultadoNivel").val();
        var edicao = $("#ddlResultadoEdicao").val();
        var areaConhecimento = $("#ddlResultadoAreaConhecimento").val();
        var anoEscolar = $("#ddlResultadoAno").val();
        var lista_uad_sigla = "";
        var lista_esc_codigo = "";
        var lista_turmas = "";

        $.mobile.loading("show", {
            text: "Obtendo dados de participação...",
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

        objParticipacaoEnvio = {
            Nivel: nivel,
            Edicao: edicao,
            AreaConhecimento: areaConhecimento,
            AnoEscolar: anoEscolar,
            lista_uad_sigla: lista_uad_sigla,
            lista_esc_codigo: lista_esc_codigo,
            lista_turmas: lista_turmas
        };

        $.post(urlBackEnd + "api/Participacao?guid=" + newGuid(), objParticipacaoEnvio)
            .done(function (dataParticipacao) {
                $.mobile.loading("hide");

                adicionarItemBackButton("btnParticipacaoVoltar");
                montarQuadroParticipacao(dataParticipacao);

                $(".page").hide();
                $("#participacao-page").show();
                $.mobile.silentScroll(0);
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
    }
    catch (error) {
        ProvaSP_Erro("Alerta", "Erro ao buscar informações: " + error);
    }
}

function montarQuadroParticipacao(participacaoData) {
    try {
        var participacaoHTML = "";

        for (var i = 0; i < participacaoData.length; i++) {
            //Header com caret para acesso
            participacaoHTML += "<p id='participacaoHeader" + i +
                "' class='participacao_header' onclick='mostrarParticipacao(this.id)'>";
            participacaoHTML += participacaoData[i].Titulo;
            participacaoHTML += "<span id='participacaoIcon" + i +
                "' class='mdi mdi-menu-down participacaoICON'></span>";
            participacaoHTML += "</p>";

            participacaoHTML += "<div id='participacaoDiv" + i +
                "' class='participacao_subDiv' style='display: none;'>";

            //O primeiro bloco é referente ao nível macro
            participacaoHTML += blocoDadosParticipacao(participacaoData[i]);
            if (participacaoData[i].Itens != null) {
                for (var j = 0; j < participacaoData[i].Itens.length; j++) {
                    participacaoHTML += blocoDadosParticipacao(participacaoData[i].Itens[j]);
                }
            }
            participacaoHTML += "</div>";
        }
        $("#participacaoConteudo").html(participacaoHTML);
    }
    catch (error) {
        console.log(error);
    }
}

function blocoDadosParticipacao(blocoOBJ) {
    try {
        let blocoHTML = "";
        blocoHTML += "<div class='participacao_blockDiv'>";
        blocoHTML += "<p class='participacao_title'>";
        blocoHTML += "<span class='participacao_coloredLine'>| </span>" + blocoOBJ.Titulo;
        blocoHTML += "</p>";

        blocoHTML += "<table>";

        //Bloco de informações Geral, sem considerar a Área de Conhecimento selecionada
        blocoHTML = apresentarBlocoParticipacaoArea("Geral", blocoHTML, blocoOBJ.PercentualParticipacaoGeral,
            blocoOBJ.TotalPrevistoGeral, blocoOBJ.TotalPresenteGeral);
        //Bloco de informações da Área de Conhecimento selecionada
        if (blocoOBJ.AreaConhecimentoID) {
            blocoHTML = apresentarBlocoParticipacaoArea(areasConhecimento[blocoOBJ.AreaConhecimentoID - 1], blocoHTML, blocoOBJ.PercentualParticipacaoAreaConhecimento,
                blocoOBJ.TotalPrevistoAreaConhecimento, blocoOBJ.TotalPresenteAreaConhecimento);
        }

        blocoHTML += "<tr>";
        blocoHTML += "<td class='participacao_defaultTD'>";
        blocoHTML += "<p class='participacao_text'>PREVISTOS</p>";
        blocoHTML += "</td>";
        blocoHTML += "<td class='participacao_defaultTD'>";
        blocoHTML += "<p class='participacao_text'>PRESENTES</p>";
        blocoHTML += "</td>";
        blocoHTML += "<td class='participacao_coloredTD' colspan='2'>";
        blocoHTML += "<p class='participacao_text' style='text-align: left;'>% DE PARTICIPAÇÃO</p>";
        blocoHTML += "</td>";
        blocoHTML += "</tr>";
        blocoHTML += "</table>";
        blocoHTML += "</div>";

        return blocoHTML;
    }
    catch (error) {
        console.log(error);
        return "";
    }
}

function apresentarBlocoParticipacaoArea(descricaoArea, blocoHTML, percentualParticipacao, totalPrevisto, totalPresente) {
    let coloredValidator = false;

    if (parseFloat(percentualParticipacao) >
        parseFloat(provaSP_configuracoes.configuracoes.RepresentatividadeSegundoINEP)) {
        coloredValidator = true;
    }

    blocoHTML += "<tr>";
    blocoHTML += "<td colspan='3'>";
    blocoHTML += "<p class='participacao_descricao'>";
    blocoHTML += descricaoArea;
    blocoHTML += "</p>";
    blocoHTML += "</td>";
    blocoHTML += "</tr>";

    blocoHTML += "<tr>";
    blocoHTML += "<td class='participacao_defaultTD'>";
    blocoHTML += "<p class='participacao_values'>";
    blocoHTML += totalPrevisto;
    blocoHTML += "</p>";
    blocoHTML += "</td>";

    blocoHTML += "<td class='participacao_defaultTD'>";
    blocoHTML += "<p class='participacao_values'>";
    blocoHTML += totalPresente;
    blocoHTML += "</p>";
    blocoHTML += "</td>";

    blocoHTML += "<td class='participacao_coloredTD'>";
    if (coloredValidator) {
        blocoHTML += "<p class='participacao_colored participacao_coloredOK'>";
    } else {
        blocoHTML += "<p class='participacao_colored'>";
    }
    blocoHTML += percentualParticipacao;
    blocoHTML += "</p>";
    blocoHTML += "</td>";

    blocoHTML += "<td class='participacao_flagTD'>";
    if (coloredValidator) {
        blocoHTML += "<p class='participacao_flagIcon'><span class='mdi mdi-flag-variant'></span></p>";
    }
    blocoHTML += "</td>";
    blocoHTML += "</tr>";

    return blocoHTML;
}

function mostrarParticipacao(headerID) {
    try {
        var openFlag = true;
        var participacaoID = headerID.replace("participacaoHeader", "");

        var pIcons = document.getElementsByClassName("participacaoICON");
        var pDivs = document.getElementsByClassName("participacao_subDiv");

        if (pDivs[participacaoID].style.display == "block") { openFlag = false; }
        for (var i = 0; i < pIcons.length; i++) {
            pIcons[i].className = "mdi mdi-menu-down participacaoICON";
            pDivs[i].style.display = "none";
        }

        if (openFlag) {
            pIcons[participacaoID].className = "mdi mdi-menu-up participacaoICON";
            pDivs[participacaoID].style.display = "block";
        }
    }
    catch (error) {
        console.log(error);
    }
}

$("#btnParticipacaoVoltar").unbind("click").click(function () {
    try {
        $(".page").hide();
        $("#resultado-page").show();

        $("#participacaoConteudo").html("");
        removerItemBackButton();
    }
    catch (error) {
        console.log(error);
    }
});
/**
-----MSTECH-----
 *Fim do Módulo 5.5 - Participação
*/

/**
-----MSTECH-----
 *Fim do Módulo 5.6 - Questionários Resultados - Por Fatores associados e Caracterização de Famílias e Escolas
*/
$("#ddlCFENivel").unbind("change").change(function () {
    try {
        var nivel = $("#ddlCFENivel").val();

        $("#divCFEEscolaItens").html("");

        $("#ddlCFECiclo").val("");
        $("#ddlCFECiclo").selectmenu("disable");
        $("#ddlCFECiclo").selectmenu("refresh");

        $("#ddlCFEQuestionario").html("<option value='' selected='selected'>(Questionário)</option>");
        $("#ddlCFEQuestionario").val("");
        $("#ddlCFEQuestionario").selectmenu("disable");
        $("#ddlCFEQuestionario").selectmenu("refresh");

        $("#btnCFEApresentar").prop("disabled", true);

        if (nivel == "" || nivel == "SME") {
            $("#divCFEDRE").hide();

            if (nivel == "") { $("#ddlCFEEdicao").selectmenu("disable"); }
            else { $("#ddlCFEEdicao").selectmenu("enable"); }
        }
        else {
            $("#divCFEDRE").show();
            $("#ddlCFEEdicao").selectmenu("disable");
            document.getElementById("divCFEDRE").style.width = "100%";
        }
        $("#divCFEEscola").hide();
        $("#ddlCFEEdicao").val("");
        $("#ddlCFEEdicao").selectmenu("refresh");

        $(".cfe-dre-radio").prop('checked', false).checkboxradio('refresh');
    }
    catch (error) {
        console.log(error);
    }
});

$(".cfe-dre-radio").unbind("change").change(function () {
    try {
        var divCFEEscolas = document.getElementById("divCFEDRE");

        if ($("#ddlCFENivel").val() != "ESCOLA") {
            divCFEEscolas.style.width = "100%";

            $("#ddlCFEEdicao").selectmenu("enable");
            $("#ddlCFEEdicao").selectmenu("refresh");
        }
        else {
            divCFEEscolas.style.width = "160px";

            $.mobile.loading("show", {
                text: "Obtendo escolas...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            carregarDataEscola(
                function () {
                    setTimeout(function () {
                        try {
                            var l = dataEscola.length;
                            var escolasEncontradas = 0;
                            var DRE_selecionada = $(".cfe-dre-radio:checked").map(function () { return this.value; }).get();

                            $("#divCFEEscolaItens").html("");

                            for (var i = 1; i < l; i++) {
                                var r = dataEscola[i].split(";");
                                //uad_sigla; esc_codigo; esc_nome
                                var uad_sigla = r[0];
                                var esc_codigo = r[1];
                                var esc_nome = r[2];

                                if (DRE_selecionada.indexOf("TD") >= 0 || DRE_selecionada.indexOf(uad_sigla) >= 0) {
                                    escolasEncontradas++;
                                    var radiokID = "radioCFEEscola_" + esc_codigo;
                                    var lblID = "lblCFEEscola_" + esc_codigo;
                                    $("#divCFEEscolaItens").append(
                                        "<label id='" + lblID + "' for='" + radiokID +
                                        "' class='cfe-escola-lbl'><input id='" + radiokID +
                                        "' type='radio' name='radioCFEEscola' " +
                                        "class='cfe-escola-radio cfe-escola-item-radio' value='" +
                                        esc_codigo + "' data-mini='true' />" + esc_nome + "</label>"
                                    );
                                }
                            }

                            if (escolasEncontradas > 0) {
                                $("#divCFEEscola").show();
                                $("#divCFEEscolaItens").trigger("create");

                                $(".cfe-escola-radio").unbind("change").change(function () {
                                    try {
                                        if ($("#ddlCFENivel").val() == "ESCOLA") {
                                            $("#ddlCFEEdicao").selectmenu("enable");
                                            $("#ddlCFEEdicao").selectmenu("refresh");
                                        }
                                    }
                                    catch (error) {
                                        console.log(error);
                                    }
                                });
                            }
                            $.mobile.loading("hide");
                        }
                        catch (error) {
                            console.log(error);
                        }
                    }, 100);
                }, ""
            );
        }
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlCFEEdicao").unbind("change").change(function () {
    try {
        $("#ddlCFECiclo").val("");

        $("#ddlCFEQuestionario").html("<option value='' selected='selected'>(Questionário)</option>");
        $("#ddlCFEQuestionario").val("");
        $("#ddlCFEQuestionario").selectmenu("disable");
        $("#ddlCFEQuestionario").selectmenu("refresh");

        if (this.value == "") { $("#ddlCFECiclo").selectmenu("disable"); }
        else { $("#ddlCFECiclo").selectmenu("enable"); }

        $("#btnCFEApresentar").prop("disabled", true);

        $("#ddlCFECiclo").selectmenu("refresh");
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlFAEdicao").unbind("change").change(function () {
    try {
        $("#ddlFACiclo").val("");

        $("#ddlFAQuestionario").html("<option value='' selected='selected'>(Questionário)</option>");
        $("#ddlFAQuestionario").val("");

        $("#ddlFAConstructo").html("<option value='' selected='selected'>(Constructo)</option>");
        $("#ddlFAConstructo").val("");

        $("#ddlFAQuestionario").selectmenu("disable");
        $("#ddlFAQuestionario").selectmenu("refresh");

        $("#ddlFAConstructo").selectmenu("disable");
        $("#ddlFAConstructo").selectmenu("refresh");

        if (this.value == "") { $("#ddlFACiclo").selectmenu("disable"); }
        else { $("#ddlFACiclo").selectmenu("enable"); }

        $("#btnFAApresentar").prop("disabled", true);

        $("#ddlFACiclo").selectmenu("refresh");
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlCFECiclo").unbind("change").change(function () {
    try {
        $("#ddlCFEQuestionario").html("<option value='' selected='selected'>(Questionário)</option>");
        $("#ddlCFEQuestionario").val("");

        $("#btnCFEApresentar").prop("disabled", true);

        if (this.value == "") {
            $("#ddlCFEQuestionario").selectmenu("disable");
            $("#ddlCFEQuestionario").selectmenu("refresh");
        }
        else { getQuestionarios(false); }
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlFACiclo").unbind("change").change(function () {
    try {
        $("#ddlFAQuestionario").html("<option value='' selected='selected'>(Questionário)</option>");
        $("#ddlFAQuestionario").val("");

        $("#ddlFAConstructo").html("<option value='' selected='selected'>(Constructo)</option>");
        $("#ddlFAConstructo").val("");

        $("#ddlFAConstructo").selectmenu("disable");
        $("#ddlFAConstructo").selectmenu("refresh");

        $("#btnFAApresentar").prop("disabled", true);

        if (this.value == "") {
            $("#ddlFAQuestionario").selectmenu("disable");
            $("#ddlFAQuestionario").selectmenu("refresh");
        }
        else { getQuestionarios(true); }
    }
    catch (error) {
        console.log(error);
    }
});

function getQuestionarios(fatorAssociado) {
    try {
        var edicao = "";

        if (fatorAssociado) { edicao = $("#ddlFAEdicao").val(); }
        else { edicao = $("#ddlCFEEdicao").val(); }

        var urlObterQuestionariosPorEdicao = urlBackEnd + "api/" +
            "FatorAssociado/GetQuestionario?edicao=" + edicao;

        $.mobile.loading("show", {
            text: "Obtendo questionários...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.ajax({
            url: urlObterQuestionariosPorEdicao,
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            success: function (dataResultado) {
                $.mobile.loading("hide");

                if (dataResultado.length > 0) {
                    var inputParametro = "";
                    var questionariosSelectHTML = "<option value='' selected='selected'>(Questionário)</option>";

                    for (var i = 0; i < dataResultado.length; i++) {
                        questionariosSelectHTML += "<option value='" + dataResultado[i].QuestionarioID +
                            "' selected='selected'>" + dataResultado[i].Nome + "</option>";
                    }
                    if (fatorAssociado) { inputParametro = "ddlFAQuestionario" }
                    else { inputParametro = "ddlCFEQuestionario"; }

                    $("#" + inputParametro).html(questionariosSelectHTML);
                    $("#" + inputParametro).val("");
                    $("#" + inputParametro).selectmenu("enable");
                    $("#" + inputParametro).selectmenu("refresh");

                }
                else {
                    ProvaSP_Erro("Não há informações aqui",
                        "Não foi possível obter os questionários. Por favor tente novamente mais tarde.");
                }
            },
            error: function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            }
        });
    }
    catch (error) {
        ProvaSP_Erro("Erro ao sincronizar: " + error);
    }
}

$("#ddlCFEQuestionario").unbind("change").change(function () {
    try {
        if (this.value == "") { $("#btnCFEApresentar").prop("disabled", true); }
        else { $("#btnCFEApresentar").prop("disabled", false); }
    }
    catch (error) {
        console.log(error);
    }
});

$("#ddlFAQuestionario").unbind("change").change(function () {
    try {
        $("#ddlFAConstructo").html("<option value='' selected='selected'>(Constructo)</option>");
        $("#ddlFAConstructo").val("");

        $("#btnFAApresentar").prop("disabled", true);

        if (this.value == "") {
            $("#ddlFAConstructo").selectmenu("disable");
            $("#ddlFAConstructo").selectmenu("refresh");
        }
        else { getConstructos(); }
    }
    catch (error) {
        console.log(error);
    }
});

function getConstructos() {
    try {
        var edicao = $("#ddlFAEdicao").val();
        var ciclo = $("#ddlFACiclo").val();
        var quesitonario = $("#ddlFAQuestionario").val();

        var urlObterConstructos = urlBackEnd +
            "api/" + "FatorAssociado/GetConstructo?edicao=" + edicao + "&cicloId=" + ciclo +
            "&questionarioId=" + quesitonario;

        $.mobile.loading("show", {
            text: "Obtendo constructos...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.ajax({
            url: urlObterConstructos,
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            success: function (dataResultado) {
                $.mobile.loading("hide");

                if (dataResultado.length > 0) {
                    var constructosSelectHTML = "<option value='' selected='selected'>(Constructo)</option>";

                    for (var i = 0; i < dataResultado.length; i++) {
                        constructosSelectHTML += "<option value='" + dataResultado[i].ConstructoId +
                            "' selected='selected'>" + dataResultado[i].Nome + "</option>";
                    }
                    $("#ddlFAConstructo").html(constructosSelectHTML);
                    //Escolha vazia
                    $("#ddlFAConstructo").val("");
                    $("#ddlFAConstructo").selectmenu("enable");
                    $("#ddlFAConstructo").selectmenu("refresh");
                }
                else {
                    ProvaSP_Erro("Não há informações aqui",
                        "Não foi possível obter os constructos. Por favor tente novamente mais tarde.");
                }
            },
            error: function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            }
        });
    }
    catch (error) {
        ProvaSP_Erro("Erro ao sincronizar: " + error);
    }
}

$("#ddlFAConstructo").unbind("change").change(function () {
    try {
        var btnFADesabilitado = false;
        if (this.value == "") { btnFADesabilitado = true; }

        $("#btnFAApresentar").prop("disabled", btnFADesabilitado);
    }
    catch (error) {
        console.log(error);
    }
});

$("#btnCFEApresentar").unbind("click").click(function () {
    try {
        var nivel = $("#ddlCFENivel").val();
        var objCaracterizacaoEnvio = {
            Nivel: nivel,
            Edicao: $("#ddlCFEEdicao").val(),
            CicloId: $("#ddlCFECiclo").val(),
            QuestionarioId: $("#ddlCFEQuestionario").val()
        };

        if (nivel == "DRE") {
            objCaracterizacaoEnvio["uad_sigla"] = $(".cfe-dre-radio:checked").map(function () { return this.value; }).get()[0];
        }
        else if (nivel == "ESCOLA") {
            objCaracterizacaoEnvio["uad_sigla"] = $(".cfe-dre-radio:checked").map(function () { return this.value; }).get()[0];
            objCaracterizacaoEnvio["esc_codigo"] = $(".cfe-escola-radio:checked").map(function () { return this.value; }).get()[0];
        }

        $.mobile.loading("show", {
            text: "Obtendo informações...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.post(urlBackEnd + "api/FatorAssociado/GetResultadoItem?guid=" + newGuid(), objCaracterizacaoEnvio)
            .done(function (dataConstructo) {
                var nivelAtual = "";
                var nivelSuperior = "";

                $.mobile.loading("hide");
                adicionarItemBackButton("btnConstructoVoltar");

                $("#constructoHeaderTitle").text($("#ddlCFENivel").val());
                $("#constructoHeaderText").text(
                    $("#ddlCFEEdicao option:selected").text() + " - " +
                    $("#ddlCFEQuestionario option:selected").text() + " - " +
                    $("#ddlCFECiclo option:selected").text()
                );

                for (var i = 0; i < dataConstructo.length; i++) {
                    var variavelID = "";

                    for (var j in variaveisConstructo) {
                        if (dataConstructo[i].VariavelDescricao == variaveisConstructo[j].descricao) {
                            variavelID = j;
                            break;
                        }
                    }

                    if (variavelID == "") {
                        variaveisConstructo[newGuid()] = {
                            descricao: dataConstructo[i].VariavelDescricao,
                            variaveis: [dataConstructo[i]]
                        };
                    }
                    else {
                        variaveisConstructo[variavelID].variaveis.push(dataConstructo[i]);
                    }
                }
                baseGraficosConstructo();
                //graficosVariaveis = true;

                if (nivel == "SME") { nivelAtual = "SME"; }
                else if (nivel == "DRE") {
                    nivelAtual = "Sua DRE";
                    nivelSuperior = "SME";
                }
                else if (nivel == "ESCOLA") {
                    nivelAtual = "Sua Escola";
                    nivelSuperior = "DRE";
                }
                montarGraficosConstructo(variaveisConstructo, nivelAtual, nivelSuperior);

                //                $(".page").hide();
                //                $("#constructo-page").show();
                mostrarTelaResultados(true, "divResultadoConstructo", 2);
                $.mobile.silentScroll(0);
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
    }
    catch (error) {
        ProvaSP_Erro("Erro ao sincronizar: " + error);
    }
});

$("#btnFAApresentar").unbind("click").click(function () {
    try {
        var edicao = $("#ddlFAEdicao").val();
        var ciclo = $("#ddlFACiclo").val();
        var quesitonario = $("#ddlFAQuestionario").val();
        var constructo = $("#ddlFAConstructo").val();

        var urlObterFA = urlBackEnd +
            "api/" + "FatorAssociado/GetFatorAssociado?edicao=" + edicao + "&cicloId=" + ciclo +
            "&questionarioId=" + quesitonario + "&constructoId=" + constructo;

        $.mobile.loading("show", {
            text: "Obtendo fatores associados...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.ajax({
            url: urlObterFA,
            type: "GET",
            dataType: "JSON",
            crossDomain: true,
            success: function (dataResultado) {
                $.mobile.loading("hide");
                apresentarResultadoFA(dataResultado);
            },
            error: function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            }
        });
    }
    catch (error) {
        ProvaSP_Erro("Erro ao sincronizar: " + error);
    }
});

function apresentarResultadoFA(dadosServidor) {
    try {
        adicionarItemBackButton("btnFAVoltar");

        //Header text
        $("#faHeaderText").text(
            $("#ddlFAEdicao option:selected").text() + " - " +
            $("#ddlFAQuestionario option:selected").text() + " - " +
            $("#ddlFACiclo option:selected").text()
        );
        $("#faHeaderTitle").text($("#ddlFAConstructo option:selected").text());
        $("#faVarsConstructo").text($("#ddlFAConstructo option:selected").text());

        //Construindo resultados
        for (var i = 0; i < dadosServidor.Resultados.length; i++) {
            $("#faTitle" + dadosServidor.Resultados[i].AreaConhecimentoId).text(
                dadosServidor.Resultados[i].AreaConhecimentoNome
            );
            $("#faValor" + dadosServidor.Resultados[i].AreaConhecimentoId).text(
                dadosServidor.Resultados[i].Pontos
            );
        }
        montarVariaveisDeFatorAssociado(dadosServidor.Variaveis);

        mostrarTelaResultados(true, "divResultadoFA", 1);
        //        $(".page").hide();
        //        $("#divResultadoFA").show();
        $.mobile.silentScroll(0);
    }
    catch (error) {
        console.log(error);
    }
}

function montarVariaveisDeFatorAssociado(faVariaveis) {
    try {
        var variaveisHTML = "";

        for (var i = 0; i < faVariaveis.length; i++) {
            variaveisHTML += "<p onclick=\"abrirGraficoFA('" + faVariaveis[i].VariavelId + "')\" ";
            variaveisHTML += "class='fatoresassociados_variaveisOption'>";
            variaveisHTML += "<span id='variavelConstructoIcon_" + faVariaveis[i].VariavelId +
                "' class='mdi mdi-chevron-down'></span>"
            variaveisHTML += faVariaveis[i].VariavelDescricao;
            variaveisHTML += "</p>";
            variaveisHTML += "<div id='variavelConstructo_" + faVariaveis[i].VariavelId +
                "' class='fatoresassociados_variaveisBlockDiv'></div>";

            variaveisConstructo[faVariaveis[i].VariavelId] = {
                descricao: faVariaveis[i].VariavelDescricao,
                variaveis: []
            };
        }
        $("#fa_variaveisDoConstructo").html(variaveisHTML);

        //Montando gráficos abaixo de cada opção
        downloadDadosDaVariavel();
    }
    catch (error) {
        console.log(error);
    }
}

$("#btnFAVoltar").unbind("click").click(function () {
    try {
        //graficosVariaveis = false;
        variaveisConstructo = {};
        $("#constructDivContent").html("");

        //        $(".page").hide();
        //        $("#resultado-page").show();
        divResultadosFixada(true);
        mostrarTelaResultados(false, "divResultadoFA", -1);
        removerItemBackButton();
    }
    catch (error) {
        console.log(error);
    }
});

function downloadDadosDaVariavel() {
    try {
        //        if (graficosVariaveis) {
        //            adicionarItemBackButton("btnConstructoVoltar");
        //
        //            $(".page").hide();
        //            $("#constructo-page").show();
        //            $.mobile.silentScroll(0);
        //        }
        //        else {
        var objConstructoEnvio = {
            Nivel: "SME",
            Edicao: $("#ddlFAEdicao").val(),
            CicloId: $("#ddlFACiclo").val(),
            QuestionarioId: $("#ddlFAQuestionario").val(),
            ConstructoId: $("#ddlFAConstructo").val()
        };

        $.mobile.loading("show", {
            text: "Obtendo dados da variável...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $.post(urlBackEnd + "api/FatorAssociado/GetResultadoItem?guid=" + newGuid(), objConstructoEnvio)
            .done(function (dataConstructo) {
                $.mobile.loading("hide");
                //adicionarItemBackButton("btnConstructoVoltar");

                $("#constructoHeaderTitle").text("SME");
                $("#constructoHeaderText").text(
                    $("#ddlFAEdicao option:selected").text() + " - " +
                    $("#ddlFAQuestionario option:selected").text() + " - " +
                    $("#ddlFACiclo option:selected").text()
                );
                baseGraficosConstructo();

                //Building necessary data
                for (var i in variaveisConstructo) {
                    for (var j = 0; j < dataConstructo.length; j++) {
                        if (variaveisConstructo[i].descricao == dataConstructo[j].VariavelDescricao) {
                            variaveisConstructo[i].variaveis.push(dataConstructo[j]);
                        }
                    }
                }
                //graficosVariaveis = true;
                montarGraficosConstructo(variaveisConstructo, "SME", "");

                //$(".page").hide();
                //$("#constructo-page").show();
                //$.mobile.silentScroll(0);
            })
            .fail(function (erro) {
                ProvaSP_Erro("Erro " + erro.status, erro.statusText);
            });
        //        }
    }
    catch (error) {
        ProvaSP_Erro("Erro ao sincronizar: " + error);
    }
}

function baseGraficosConstructo() {
    try {
        var baseConstructoHTML = "";

        for (var i in variaveisConstructo) {
            baseConstructoHTML += "<p class='fatoresassociados_variaveisText'>";
            baseConstructoHTML += "<b>";
            baseConstructoHTML += "<span class='mdi mdi-chevron-right'></span>";
            baseConstructoHTML += variaveisConstructo[i].descricao;
            baseConstructoHTML += "</b>";
            baseConstructoHTML += "</p>";

            //Div para o gráfico
            baseConstructoHTML += "<div id='variavelConstructo_" + i + "' style='border: 2px solid #ccc; margin-bottom: 20px;'>";
            baseConstructoHTML += "</div>";
        }
        $("#constructDivContent").html(baseConstructoHTML);
    }
    catch (error) {
        console.log(error);
    }
}

function montarGraficosConstructo(constructo, nivel, nivelSuperior) {
    try {
        for (var i in constructo) {
            $("#variavelConstructo_" + i).append("<canvas id='chartVariavel_" + i + "'></canvas>");
            var chartVariavel_ctx = document.getElementById("chartVariavel_" + i).getContext("2d");
            var constructoDataSet = [];
            var constructoLabel = vetorLabelsConstructo(constructo[i]);

            if (constructoLabel.length > 0) {
                //Nível padrão do constructo
                constructoDataSet.push({
                    label: nivel, data: vetorDadosConstructo(constructo[i], false), backgroundColor: "#083C59"
                });

                //Nível superior do constructo
                if (nivelSuperior != "") {
                    constructoDataSet.push({
                        label: nivelSuperior, data: vetorDadosConstructo(constructo[i], true), backgroundColor: "#91C8D7"
                    });
                }

                var linesSerieHistorica = new Chart(chartVariavel_ctx, {
                    type: 'horizontalBar',
                    data: {
                        labels: constructoLabel,
                        datasets: constructoDataSet
                    },
                    options: {
                        responsive: true,
                        tooltips: { enabled: false },
                        scales: {
                            xAxes: [{
                                display: true,
                                gridLines: { display: false },
                                ticks: {
                                    stepSize: 50,
                                    beginAtZero: true,
                                    max: 100
                                }
                            }],
                            yAxes: [{
                                display: true,
                                //                                gridLines: { display : false },
                                ticks: {
                                    fontSize: 14,
                                    fontStyle: 600,
                                    fontColor: '#606060'
                                }
                            }]
                        },
                        animation: {
                            onComplete: function () {
                                var chartInstance = this.chart,
                                    ctx = chartInstance.ctx;

                                ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                                ctx.textAlign = 'center';
                                ctx.textBaseline = 'bottom';

                                this.data.datasets.forEach(function (dataset, i) {
                                    var meta = chartInstance.controller.getDatasetMeta(i);
                                    meta.data.forEach(function (bar, index) {
                                        var dataValue = dataset.data[index];

                                        if (dataValue < 20) {
                                            ctx.fillStyle = '#606060';
                                            ctx.fillText(dataValue + "%", bar._model.x + 20, bar._model.y + 7.5);
                                        }
                                        else {
                                            if (bar._model.backgroundColor == "#083C59") { ctx.fillStyle = '#f0f0f0'; }
                                            else { ctx.fillStyle = '#606060'; }
                                            ctx.fillText(dataValue + "%", bar._model.x - 22.5, bar._model.y + 7.5);
                                        }
                                    });
                                });
                            }
                        }
                    }
                });
            }
        }
    }
    catch (error) {
        console.log(error);
    }
}

function abrirGraficoFA(idDiv) {
    try {
        var currentSelectedFADiv = document.getElementById("variavelConstructo_" + idDiv);
        var currentSelectedFAIcon = document.getElementById("variavelConstructoIcon_" + idDiv);

        if (currentSelectedFADiv.style.display == "block") {
            currentSelectedFADiv.style.display = "none";
            currentSelectedFAIcon.className = "mdi mdi-chevron-down";
        }
        else {
            currentSelectedFADiv.style.display = "block";
            currentSelectedFAIcon.className = "mdi mdi-chevron-up";
        }
    }
    catch (error) {
        console.log(error);
    }
}

function vetorLabelsConstructo(variavelAtual) {
    try {
        var vetorLabels = [];

        for (var i = 0; i < variavelAtual.variaveis.length; i++) {
            vetorLabels.push(variavelAtual.variaveis[i].ItemDescricao);
        }
        return vetorLabels;
    }
    catch (error) {
        console.log(error);
    }
    return [];
}

function vetorDadosConstructo(variavelAtual, nivelSuperior) {
    try {
        var vetorData = [];

        for (var i = 0; i < variavelAtual.variaveis.length; i++) {
            if (nivelSuperior) { vetorData.push(parseFloat(variavelAtual.variaveis[i].ValorSuperior).toFixed(2)); }
            else { vetorData.push(parseFloat(variavelAtual.variaveis[i].Valor).toFixed(2)); }
        }
        return vetorData;
    }
    catch (error) {
        console.log(error);
    }
    return [];
}

$("#btnConstructoVoltar").unbind("click").click(function () {
    try {
        //$(".page").hide();

        if (caminhoBackButton.indexOf("btnFAVoltar") != -1) { $("#divResultadoFA").show(); }
        else {
            //graficosVariaveis = false;
            variaveisConstructo = {};
            $("#constructDivContent").html("");

            divResultadosFixada(true);
            mostrarTelaResultados(false, "divResultadoConstructo", -1);
            //$("#resultado-page").show();
        }
        removerItemBackButton();
    }
    catch (error) {
        console.log(error);
    }
});

/**
-----MSTECH-----
 *Fim do Módulo 5 - Novas implementações da MSTECH
*/
