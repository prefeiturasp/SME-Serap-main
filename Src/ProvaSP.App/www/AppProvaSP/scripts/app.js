// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in cordova-simulate or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.

//debugger;
"use strict";

var mobile = false;
var db = null;
var notificacaoSincroniaAtivada = false;
//localStorage.removeItem("Usuario");   

var edicoesComTurmasAmostrais = ["2017"];
var questionarios = ["1", "2", "3", "8", "9", "10", "11", "12"];;
var listaQuestionariosNaoEnviados;

var questionarioCarregado = {};

//https://stackoverflow.com/questions/8068052/phonegap-detect-if-running-on-desktop-browser
//if (navigator.userAgent.match(/(iPhone|iPod|iPad|Android|BlackBerry|IEMobile)/) || typeof cordova != "undefined") {
if (typeof cordova !== "undefined")
{
    // PhoneGap application
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    mobile = true;
} else
{
    // Web page
    onDeviceReady();

}



function onDeviceReady()
{
    //INÍCIO

    if (typeof window.jQuery == "undefined")
    {
        //As vezes onDeviceReady dispara antes da carga do jQuery...
        setTimeout(onDeviceReady(), 1000);
        return;
    }

    var apresentarLoading = true;

    if (!mobile)
    {
        var url = window.location.href.toLocaleLowerCase().split("provasp")[0].replace("serap.sme", "provasp.sme");
        if (url.indexOf("file") == -1)
        {
            urlBackEnd = window.location.href.toLocaleLowerCase().split("provasp")[0].replace("serap.sme", "provasp.sme");
            //urlBackEnd = "http://localhost:52912/";
        }
    }
    else
    {
        if (!(navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN))
        {
            apresentarLoading = false;
        }
    }

    if (apresentarLoading)
    {
        $.mobile.loading('show');
    }
    else
    {
        $("#aguarde-page").hide();
        $("#menu-page").show();
    }


    $.ajax({
        url: urlBackEnd + "api/RetornarAppJson",
        type: "GET",
        dataType: "JSON",
        crossDomain: true,
        success: function (data)
        {
            if (Usuario.Aluno)
            {
                resultadoAlunoConfigurarInterface();
                return;
            }

            if (data.RelatorioAcompanhamentoVisivel && !Usuario.Professor) //O usuário tem acesso apenas ao questionário 12 (Professor). Ele não deve acessar o relatório de acompanhamento.
            {
                $("#btnAbrirRelatorioAcompanhamento").show();
            }
            else
            {
                $("#btnAbrirRelatorioAcompanhamento").hide();
            }

            if (data.DisponibilizarPreenchimentoQuestionariosFichas)
            {
                $("#aguarde-page").hide();
                $("#menu-page").show();

                $.mobile.loading('hide');
                $("#divMenuPrincipal").show();
            }
            else
            {
                $("#btnResultadoFechar").hide();
                $("#btnAbrirResultados").trigger("click");
            }
        },
        error: function (erro)
        {
            swal("Erro " + erro.status, erro.statusText, "error");
            $.mobile.loading("hide");
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

    //        if (data.RelatorioAcompanhamentoVisivel && Usuario.questionarios != "12") //O usuário tem acesso apenas ao questionário 12 (Professor). Ele não deve acessar o relatório de acompanhamento.
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
    //            $("#btnResultadoFechar").hide();
    //            $("#btnAbrirResultados").trigger("click");
    //        }
    //    })
    //    .fail(function (erro)
    //    {
    //        swal("Erro " + erro.status, erro.statusText, "error");
    //        $.mobile.loading("hide");
    //    });


    function resultadoAlunoConfigurarInterface()
    {

        $.mobile.loading("show", {
            text: "Aguarde...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        $(".page").hide();
        ;
        $("#btnResultadoFechar").hide();
        $("#resultado-aluno-page").show();


        $.post(urlBackEnd + "api/AlunoParticipacaoEdicoes?guid=" + newGuid(), { alu_matricula: Usuario.usu_login })
            .done(function (edicoes)
            {
                for (var i = 0; i < edicoes.length; i++)
                {
                    $("#ddlResultadoAlunoEdicao").append("<option value='" + edicoes[i] + "'>" + edicoes[i] + "</option>");
                }
                $.mobile.loading("hide");

            })
            .fail(function (erro)
            {
                swal("Erro " + erro.status, erro.statusText, "error");
                $.mobile.loading("hide");
            });

    }

    function resultadoAlunoApresentar(dataResultado)
    {

    }

    $(document).on("mobileinit", function ()
    {
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

            $(document).ajaxStart(function ()
            {
                $.mobile.loading('show');
            })
        $(document).ajaxStop(function ()
        {
            $.mobile.loading('hide');
        });
    });

    definirEventHandlers();

    if (!mobile)
    {
        if (window.location.href.indexOf("file:///") == 0)
        {
            Usuario = JSON.parse(localStorage.getItem("Usuario"));
        }
        else
        {
            configurarUsuarioSerap(grupoSerap, jsonUsuario);
        }
    }
    else
    {
        Usuario = JSON.parse(localStorage.getItem("Usuario"));

        cordova.plugins.notification.local.getScheduledIds(function (scheduledIds)
        {
            notificacaoSincroniaAtivada = (scheduledIds.length > 0);
        });

        try
        {
            db = window.sqlitePlugin.openDatabase(
                { name: "base.sqlite", location: 'default' },
                function ()
                {
                    //sucesso
                    console.log("db aberto.");

                    db.sqlBatch([
                        /*
                        "DROP TABLE IF EXISTS QuestionarioUsuario",
                        "DROP TABLE IF EXISTS QuestionarioRespostaItem",
                        */
                        "CREATE TABLE IF NOT EXISTS QuestionarioUsuario (QuestionarioUsuarioID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioID INTEGER, Guid TEXT, esc_codigo TEXT, tur_id INTEGER, usu_id TEXT, Enviado INTEGER, DataPreenchimento TEXT)",
                        "CREATE TABLE IF NOT EXISTS QuestionarioRespostaItem (QuestionarioRespostaItemID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioUsuarioID INTEGER, Numero TEXT, Valor TEXT);"
                    ], function ()
                        {
                            //sucesso
                            console.log("db - tabelas criadas.");
                            sincronizarLoop();

                        }, function (error)
                        {
                            alert('SQL batch ERROR: ' + error.message);
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
                function (er)
                {
                    //erro
                    alert(er);
                }
            );

        }
        catch (er)
        {
            alert("não foi possível abrir o banco de dados: " + er);
        }
    }

    //questionarios = Usuario.questionarios.split(",");
    if (Usuario.Supervisor)
    {
        $("#divAbrirQuestionarioID_1").show();
        $("#divAbrirQuestionarioID_9").show();
    }

    if (Usuario.Diretor)
    {
        $("#divAbrirQuestionarioID_2").show();
        $("#divAbrirQuestionarioID_10").show();
    }

    if (Usuario.Coordenador)
    {
        $("#divAbrirQuestionarioID_3").show();
        $("#divAbrirQuestionarioID_11").show();
    }
    if (Usuario.Professor)
    {
        $("#divAbrirQuestionarioID_12").show();
    }

    for (var i = 0; i < questionarios.length; i++)
    {
        var questionationarioID = questionarios[i]; //questionarios[i].split("=")[0];
        var btn = "#btnAbrirQuestionarioID_" + questionationarioID;
        $(btn).show();
        $(btn).click(
            function ()
            {
                var questionarioId = this.id.replace("btnAbrirQuestionarioID_", "");
                $(".page").hide(); $("#questionario-page").show();

                selectionarQuestionario(questionarioId);

                if (questionarioCarregado[questionarioId] == null)
                {
                    $("#divQuestionario" + questionarioId + "_Questoes").html("Aguarde...");

                    $.mobile.loading("show", {
                        text: "Aguarde...",
                        textVisible: true,
                        theme: "a",
                        html: ""
                    });

                    $("#divQuestionario" + questionarioId + "_Questoes").load("AppProvaSP/questionario_" + questionarioId + ".html?guid=" + newGuid(), function ()
                    {
                        $.mobile.loading("hide");

                        $("#divQuestionario" + questionarioId + "_Questoes").trigger("create");
                        questionarioCarregado[questionarioId] = true;
                        definirEventHandlers();

                        aplicarBIB();

                    });
                }

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


function aplicarBIB()
{
    //Aplica o BIB correto baseado no RF do usuário.
    var bloco = (parseInt(Usuario.username) % 3) + 1;
    $(".BIB_B1").hide();
    $(".BIB_B2").hide();
    $(".BIB_B3").hide();
    $(".BIB_B" + bloco).show();
}

function excluirNotificacaoLocal()
{

    cordova.plugins.notification.local.cancelAll(
        function ()
        {
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











var caminhoBackButton = null;

document.addEventListener('deviceready', function (event)
{
    document.addEventListener('backbutton', function (e)
    {
        //navigator.app.exitApp(); 
        //$("#foo").trigger("click");
        voltarCaminhoBackButton();
    });
}, false);

function voltarCaminhoBackButton()
{
    if (caminhoBackButton == null)
        caminhoBackButton = new Array();

    if (caminhoBackButton.length == 0)
    {
        if (window.location.href.indexOf("#") == -1)
            navigator.app.exitApp();
        return;
    }

    var indiceItemAtual = caminhoBackButton.length - 1;
    var btnAtual = caminhoBackButton[indiceItemAtual];
    $("#" + btnAtual).trigger("click");

}
function removerItemBackButton()
{
    if (caminhoBackButton == null)
        caminhoBackButton = new Array();

    if (caminhoBackButton.length == 0)
    {
        return;
    }
    caminhoBackButton.pop();
}

function adicionarItemBackButton(btn)
{
    if (caminhoBackButton == null)
        caminhoBackButton = new Array();

    caminhoBackButton.push(btn);
}

$(document).keyup(function (e)
{
    if (e.keyCode == 27)
    { // escape key maps to keycode `27`
        voltarCaminhoBackButton();
    }
});

function newGuid()
{
    //return uuidv4();
    return guidPart() + guidPart() + '-' + guidPart() + '-' + guidPart() + '-' +
        guidPart() + '-' + guidPart() + guidPart() + guidPart();
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

function guidPart()
{
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}




var questionarioId_atual = "";

function selectionarQuestionario(questionarioId)
{
    questionarioId_atual = questionarioId;
    $("#tituloQuestionario").html($("#tituloQuestionario" + questionarioId).val());
    $("#divQuestionarios").children("div").each(function () { $(this).hide() });

    $("#Questionarios form").hide();

    $("#Questionario" + questionarioId).show();

    if (questionarioId >= 1 && questionarioId <= 3 || questionarioId == 12)
    {
        aplicarBIB();

        $("#divQuestionario" + questionarioId + "_Intro").show();
        $("#divQuestionario" + questionarioId + "_Questoes").hide();
        $("#divTituloQuestionario").hide();
    }
    else if (questionarioId == 8)
    {
        $("#divCodigoTurma").show();
        $("#divMenuPrincipal").hide();
        $("#txtCodigoTurma").focus();
        adicionarItemBackButton("btnCodigoTurmaVoltar");
    }
    else if (questionarioId == 10 || questionarioId == 11)
    {

        swal({
            title: "Atenção!",
            text: "Essa ficha deve ser preenchida após o término do último dia de aplicação. Deseja continuar?",
            type: "warning",
            showCancelButton: true,

            confirmButtonText: "Não",
            cancelButtonText: "Sim",
            closeOnConfirm: false
        },
            function ()
            {
                //window.location = "menu.html";
                removerItemBackButton();
                $(".page").hide(); $("#menu-page").show();
                swal.close();
            });

    }

    if (questionarioId == 12 && !mobile)
    {

        swal({
            title: "Atenção!",
            text: "Esse questionário deve ser preenchido por Professores(as) regentes do 3ª ao 5º ano e Professores(as) de Ciências, Português e Matemática, do 6º ao 9º ano. Deseja continuar?",
            type: "warning",
            showCancelButton: true,

            confirmButtonText: "Não",
            cancelButtonText: "Sim",
            closeOnConfirm: false
        },
            function ()
            {
                //window.location = "menu.html";
                removerItemBackButton();
                $(".page").hide(); $("#menu-page").show();
                swal.close();
            });

    }
}

function recuperarCodigoENomeDaEscolaParaQuestionario(questionarioId)
{
    carregarDataEscola(
        function ()
        {
            var esc_codigo = "";
            var esc_nome = "";
            for (var i = 0; i < questionarios.length; i++)
            {
                var r = questionarios[i];
                var chaveValor = questionarios[i].split("=");
                if (chaveValor[0] == questionarioId)
                {
                    esc_codigo = chaveValor[1];
                }
            }
            $("#Questionario_" + questionarioId + "_Questao_1").val(esc_codigo);

            var l = dataEscola.length;
            for (var i = 0; i < l; i++)
            {
                var r = dataEscola[i].split(";");
                if (esc_codigo == r[1])
                {
                    esc_nome = r[2];
                    r = l;
                }
            }
            //$("#Questionario_" + questionarioId + "_esc_nome").val(esc_nome);
            $("#Questionario_" + questionarioId + "_esc_nome").html(esc_nome);
        }
        , ""
    );
}



var dataEscola = [];
function carregarDataEscola(callback, opcoes)
{
    if (dataEscola.length == 0)
    {
        $.ajax({
            type: "GET",
            url: "AppProvaSP/escola.csv",
            dataType: "text",
            success: function (data)
            {
                if (data.indexOf("\r\n") > 0)
                {
                    data = data.replace(/\r\n/g, "\n");
                }
                dataEscola = data.split("\n");
                callback(opcoes);
            }
        });
    }
    else
    {
        callback(opcoes);
    }
}

function selecionarDRE(uad_sigla)
{
    var l = dataEscola.length;
    $("#ddlEscola").empty();
    $("#ddlEscola").append("<option value=\"\">Selecione a Escola</option>");

    if (uad_sigla == "")
    {
        $("#ddlEscola").selectmenu("disable");
        //document.getElementById("ddlEscola").disabled = true;
        return;
    }

    $("#ddlEscola").selectmenu("enable");

    for (var i = 0; i < l; i++)
    {
        var r = dataEscola[i].split(";");
        if (uad_sigla == r[0])
        {
            //$("#ddlEscola").append(new Option(r[2], r[1], defaultSelected, nowSelected));
            //.append('<option>newvalue</option>');
            $("#ddlEscola").append("<option value=\"" + r[1] + "\">" + r[2] + "</option>");
        }
    }
}

function resetInstrumento()
{
    $("#Questionario_8_QuestoesInstrumentoPortugues,#Questionario_8_QuestoesInstrumentoCiencias").hide();
}



function salvarQuestionarioLocal()
{
    var esc_codigo = "";
    var tur_id = "";
    var usu_id = Usuario.usu_id;
    var Enviado = "0";
    var guid = newGuid();

    var d = new Date().toISOString();
    var t = new Date().toTimeString().replace(/.*(\d{2}:\d{2}:\d{2}).*/, "$1");
    var DataPreenchimento = d.slice(0, 10) + " " + t;

    if (questionarioId_atual == 8) //Ficha de Registro - Aplicador(a) de Prova
    {
        //tur_id: recuperar da caixa de texto #txtCodigoTurma
        var codigo = $("#txtCodigoTurma").val();
        tur_id = parseInt(codigo.substring(0, codigo.length - 1));
    }
    else if (questionarioId_atual == 9) //Ficha de Registro - Supervisor(a) Escolar
    {
        //esc_codigo: seleção
        esc_codigo = $("#ddlEscola").val();
    }
    else
    {
        for (var i = 0; i < questionarios.length; i++)
        {
            var questionationarioID = questionarios[i].split("=")[0];
            if (questionationarioID == questionarioId_atual)
            {
                esc_codigo = questionarios[i].split("=")[1];
                i = questionarios.length;
            }
        }
    }

    //Remove os itens duplicados com valor "default".
    var respostas = $("#Questionario" + questionarioId_atual).serializeArray();
    for (var i = 0; i < respostas.length; i++)
    {
        if (i > 0)
        {
            if (respostas[i - 1].value == "default" && respostas[i - 1].name == respostas[i].name)
            {
                respostas.splice(i - 1, 1);
            }
        }
    }

    var inclusaoLocalSucesso = false;

    db.transaction(function (tx)
    {
        //"CREATE TABLE IF NOT EXISTS QuestionarioUsuario (QuestionarioUsuarioID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioID INTEGER, Guid TEXT, esc_codigo TEXT, tur_id INTEGER, usu_id INTEGER, Enviado INTEGER, DataPreenchimento TEXT);" +
        //"CREATE TABLE IF NOT EXISTS QuestionarioRespostaItem (QuestionarioRespostaItemID INTEGER PRIMARY KEY AUTOINCREMENT, QuestionarioUsuarioID INTEGER, Numero TEXT, Valor TEXT);"
        tx.executeSql('INSERT INTO QuestionarioUsuario (QuestionarioID, Guid, esc_codigo, tur_id, usu_id, Enviado, DataPreenchimento) VALUES (?, ?, ?, ?, ?, ?, ?);', [questionarioId_atual, guid, esc_codigo, tur_id, usu_id, Enviado, DataPreenchimento], function (tx, results)
        {
            var QuestionarioUsuarioID = results.insertId;
            console.log("INSERT na tabela QuestionarioUsuario. Retorno da inclusão: QuestionarioUsuarioID=" + QuestionarioUsuarioID);
            for (var i = 0; i < respostas.length; i++)
            {
                var numero = respostas[i].name.replace("Questionario_" + questionarioId_atual + "_Questao_", "");
                var valor = respostas[i].value;
                tx.executeSql('INSERT INTO QuestionarioRespostaItem (QuestionarioUsuarioID, Numero, Valor) VALUES (?, ?, ?);', [QuestionarioUsuarioID, numero, valor],
                    function (tr, resultSet)
                    {
                        //INSERT QuestionarioRespostaItem OK
                    },
                    function (tx, error)
                    {
                        //INSERT ERRO
                    }
                );
            }
        },
            function (tx, error)
            {
                alert('Error: ' + error.message);
                $.mobile.loading("hide");
            }
        );


    },
        function (error)
        {
            alert('Transaction ERROR: ' + error.message);
            $.mobile.loading("hide");
        },
        function ()
        {
            console.log('INSERT Questionario OK');
            $.mobile.loading("hide");
            sincronizar();
        });


    if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN)
    {
        ativarNotificacaoSincronia("Não foi detectada uma conexão ativa com a internet. As respostas deste questionário foram salvas.\n\nNa próxima vez que for aberto o aplicativo e houver  uma conexão ativa, suas respostas serão enviadas.");
    }
    else
    {
        swal("Obrigado!", "As informações foram salvas com sucesso!", "success");
    }

    removerItemBackButton();
    $(".page").hide();
    $("#menu-page").show();
    $("#divCodigoTurma").hide();
    $("#divMenuPrincipal").show();

}

function ativarNotificacaoSincronia(mensagem)
{
    if (mensagem != "") alert(mensagem);
    if (!notificacaoSincroniaAtivada)
    {
        notificacaoSincroniaAtivada = true;
        cordova.plugins.notification.local.schedule({
            id: 0,
            text: "Existem informações da Prova São Paulo que ainda não foram enviadas via internet. Abra aqui e envie agora.",
            every: "day",
        });
    }
}

function sincronizarLoop()
{
    if (notificacaoSincroniaAtivada)
    {
        sincronizar();
    }
    setTimeout("sincronizarLoop()", 20000);
}

function sincronizar()
{
    if (navigator.connection.type != Connection.NONE)
    {
        sincronizarQuestionarios();
    }
}

function sincronizarQuestionarios()
{
    //urlBackEnd

    listaQuestionariosNaoEnviados = new Array();
    var hashRespostas = {};
    db.readTransaction(function (tx)
    {
        tx.executeSql('SELECT QuestionarioUsuarioID, QuestionarioID, Guid, esc_codigo, tur_id, usu_id, DataPreenchimento FROM QuestionarioUsuario WHERE Enviado=0', [],
            function (tx, quRows)
            {
                //console.log('row' + rs.rows.item(0).mycount);
                console.log('row');
                for (var i = 0; i < quRows.rows.length; i++)
                {
                    var qur = quRows.rows.item(i);
                    var qu = new QuestionarioUsuario(qur.QuestionarioUsuarioID, qur.QuestionarioID, qur.Guid, qur.esc_codigo, qur.tur_id, qur.usu_id, qur.DataPreenchimento, null);
                    listaQuestionariosNaoEnviados.push(qu);

                    tx.executeSql('SELECT QuestionarioUsuarioID, Numero, Valor FROM QuestionarioRespostaItem WHERE QuestionarioUsuarioID=?', [qur.QuestionarioUsuarioID], function (tx, qriRows)
                    {
                        var respostas = new Array();
                        var QuestionarioUsuarioID = "";
                        for (var i2 = 0; i2 < qriRows.rows.length; i2++)
                        {
                            var itemRow = qriRows.rows.item(i2);
                            var qri = new QuestionarioRespostaItem(itemRow.Numero, itemRow.Valor);
                            respostas.push(qri);
                            if (i2 == 0)
                            {
                                QuestionarioUsuarioID = itemRow.QuestionarioUsuarioID;
                            }
                            //listaQuestionarios
                        }
                        hashRespostas[QuestionarioUsuarioID] = respostas;

                    },
                        function (tx, error)
                        {
                            console.log('SELECT error: ' + error.message);
                            $.mobile.loading("hide");
                        }
                    );

                }


            },
            function (tx, error)
            {
                console.log('SELECT error: ' + error.message);
                $.mobile.loading("hide");
            }
        );
    },
        function (error)
        {
            $.mobile.loading("hide");
            alert('Transaction ERROR: ' + error.message);
        },
        function ()
        {

            if (listaQuestionariosNaoEnviados.length > 0)
            {
                for (var i = 0; i < listaQuestionariosNaoEnviados.length; i++)
                {
                    var QuestionarioUsuarioID = listaQuestionariosNaoEnviados[i].QuestionarioUsuarioID;
                    listaQuestionariosNaoEnviados[i].Respostas = hashRespostas[QuestionarioUsuarioID];
                }
                var jsonListaQuestionariosNaoEnviados = JSON.stringify(listaQuestionariosNaoEnviados);
                //jsonListaQuestionariosNaoEnviados = jsonListaQuestionariosNaoEnviados;

                $.post(urlBackEnd + "api/SincronizarQuestionario", { json: jsonListaQuestionariosNaoEnviados })
                    .done(function (data)
                    {
                        if (data.length == listaQuestionariosNaoEnviados.length)
                        {
                            excluirNotificacaoLocal();
                        }
                        atualizarGuidsComoEnviados(data);
                    })
                    .fail(function (xhr, status, error)
                    {
                        if (!notificacaoSincroniaAtivada)
                        {
                            ativarNotificacaoSincronia("");
                        }

                        $.mobile.loading("hide");
                        sweetAlert("Falha de comunicação", "Não foi possível sincronizar as informações com o servidor. (" + status + ") " + error, "error");
                    });
            }

        });
}

function atualizarGuidsComoEnviados(data)
{
    //alert(data);
    db.transaction(function (tx)
    {
        for (var i = 0; i < data.length; i++)
        {
            var guid = data[i];
            tx.executeSql('UPDATE QuestionarioUsuario SET Enviado=1 WHERE Guid=?;', [guid],
                function (tr, resultSet)
                {
                    console.log("UPDATE QuestionarioUsuario ok");


                },
                function (tx, error)
                {
                    console.log(error);
                }
            );
        }
    },
        function (error)
        {
            alert('Transaction ERROR: ' + error.message);
            $.mobile.loading("hide");
        },
        function ()
        {
            console.log('Questionários sincronizados com sucesso');
            $.mobile.loading("hide");
        });

}

function marcarDesmarcarAlunoAusente(chk)
{
    var idSpan = "#" + chk.id.replace("chk", "span");
    if (chk.checked)
    {
        $(idSpan).show();
        $(idSpan).addClass("listaPresencaAlunoAusente");
    }
    else
    {
        $(idSpan).hide();
        $(idSpan).removeClass("listaPresencaAlunoAusente");
    }

    calcularPresentesEAusentes();
}

function calcularPresentesEAusentes()
{
    var quantidadeAlunos = $(".listaPresencaAluno").length;
    var quantidadeAusentes = $(".listaPresencaAlunoAusente").length;
    var totalPresentes = quantidadeAlunos - quantidadeAusentes;
    $("#Questionario_8_Questao_5_Presentes").val(totalPresentes);
    $("#Questionario_8_Questao_6_Ausentes").val(quantidadeAusentes);
}

function escanearCodigoDeBarrasCadernoReserva(caderno)
{
    cordova.plugins.barcodeScanner.scan(
        function (result)
        {
            /*
            alert("We got a barcode\n" +
                "Result: " + result.text + "\n" +
                "Format: " + result.format + "\n" +
                "Cancelled: " + result.cancelled);
            */
            var valido = true;
            var resultado = result.text;
            if (resultado.length != 12)
            {
                valido = false;
            }
            else
            {
                var numeroInsricao = resultado.substr(0, 7);
                var verificador = resultado.substr(7, 1);
                valido = (calculaDigito(numeroInsricao) == verificador);
            }

            if (!valido)
            {
                sweetAlert("Código inválido", "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.", "error");
                //return;
            }

            $("#Questionario_8_Questao_11_CodigoBarrasCadernoReserva" + caderno).val(result.text);
        },
        function (error)
        {
            alert("Erro ao escanear: " + error);
        },
        {
            preferFrontCamera: false, // iOS and Android
            showFlipCameraButton: true, // iOS and Android
            showTorchButton: true, // iOS and Android
            torchOn: false, // Android, launch with the torch switched on (if available)
            saveHistory: true, // Android, save scan history (default false)
            prompt: "Coloque o código de no quadro de escaneamento", // Android
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

function resultado_configurarControles()
{

    var btnResultadoDesabilitado = true;

    var nivel = $("#ddlResultadoNivel").val();
    var edicao = $("#ddlResultadoEdicao").val();
    var areaConhecimento = $("#ddlResultadoAreaConhecimento").val();
    var ano = $("#ddlResultadoAno").val();
    var dres = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get();
    var escolas = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get();
    var turmas = $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get(); //$("#txtResultadoTurma").val();
    var alunos = $(".resultado-aluno-item-chk:checked").map(function () { return this.value; }).get();


    $("#ddlResultadoEdicao").selectmenu("disable");
    $("#ddlResultadoEdicao").selectmenu("refresh");

    $("#ddlResultadoAreaConhecimento").selectmenu("disable");
    $("#ddlResultadoAreaConhecimento").selectmenu("refresh");

    $("#ddlResultadoAno").selectmenu("disable");
    $("#ddlResultadoAno").selectmenu("refresh");

    $("#divResultadoDRE").hide();
    $("#divResultadoEscola").hide();
    $("#divResultadoTurma").hide();
    $("#divResultadoAluno").hide();

    $("#divResultadoApresentacao").hide();

    $("#ddlResultadoAno option").show();

    if (edicoesComTurmasAmostrais.indexOf(edicao) >= 0)
    {
        if (nivel == "ESCOLA")
        {
            if (ano == "4" || ano == "6" || ano == "8")
            {
                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").trigger("change");
            }

            $("#ddlResultadoAnoItem_Ano4").hide();
            $("#ddlResultadoAnoItem_Ano6").hide();
            $("#ddlResultadoAnoItem_Ano8").hide();
        }
    }

    if (edicao == "ENTURMACAO_ATUAL")
    {
        if (ano == "3")
        {
            $("#ddlResultadoAno").val("");
            $("#ddlResultadoAno").trigger("change");
        }

        $("#ddlResultadoAnoItem_Ano3").hide();
    }

    if (nivel == "SME")
    {
        if (edicao != "" && areaConhecimento != "" && ano != "")
        {
            btnResultadoDesabilitado = false;
        }
    }
    else if (nivel == "DRE" && edicao != "" && areaConhecimento != "" && ano != "" && dres.length > 0)
    {
        btnResultadoDesabilitado = false;
    }
    else if (nivel == "ESCOLA" && edicao != "" && areaConhecimento != "" && ano != "" && dres.length > 0 && escolas.length > 0)
    {
        btnResultadoDesabilitado = false;
    }
    else if (nivel == "TURMA" && edicao != "" && areaConhecimento != "" && ano != "" && dres.length > 0 && escolas.length > 0 && turmas.length > 0)
    {
        btnResultadoDesabilitado = false;
    }
    else if (nivel == "ALUNO" && edicao != "" && areaConhecimento != "" && ano != "" && dres.length > 0 && escolas.length > 0 && alunos.length > 0)
    {
        btnResultadoDesabilitado = false;
    }


    //SELEÇÃO NÍVEL (ATUALIZAÇÃO DO TEXTO DA OPTION TURMA)
    if (
        (edicao == "HISTORICO")
    )
    {
        if (nivel == "TURMA" && $("#ddlResultadoNivel_optionTurma").html() == "Turma detalhando Alunos")
        {
            swal("Detalhamento de alunos", "O detalhamento de alunos por turma não será apresentado no modo Histórico.", "warning");
        }
        $("#ddlResultadoNivel_optionTurma").html("Turma");
    }
    else
    {
        $("#ddlResultadoNivel_optionTurma").html("Turma detalhando Alunos");
    }
    $("#ddlResultadoNivel").selectmenu("refresh");


    //VISIBILIDADE SELEÇÃO EDIÇÃO
    if (
        (nivel != "")
    ) 
    {
        $("#ddlResultadoEdicao").selectmenu("enable");
        $("#ddlResultadoEdicao").selectmenu("refresh");
    }

    //VISIBILIDADE SELEÇÃO ÁREA DE CONHECIMENTO
    if (
        (nivel != "" && edicao != "")
    ) 
    {
        $("#ddlResultadoAreaConhecimento").selectmenu("enable");
        $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
        //$("#ddlResultadoAno").selectmenu("enable");
        //$("#ddlResultadoAno").selectmenu("refresh");
    }


    //VISIBILIDADE SELEÇÃO ÁREA DE CONHECIMENTO
    if (
        (nivel != "" && edicao != "" && areaConhecimento != "")
    ) 
    {
        $("#ddlResultadoAno").selectmenu("enable");
        $("#ddlResultadoAno").selectmenu("refresh");
    }


    //VISIBILIDADE SELEÇÃO DRE
    if (
        (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO") &&
        edicao != "" &&
        areaConhecimento != "" &&
        ano != ""
    ) 
    {
        $("#divResultadoDRE").show();

        if (!Usuario.AcessoNivelSME)
        {
            //PERMISSÃO DE VISIBILIDADE PARA CADA DRE:
            $(".resultado-dre-chk").parent().hide();
            $("#chkResultadoTodasDREs").show();
            for (var i = 0; i < Usuario.grupos.length; i++)
            {
                var uad_sigla = Usuario.grupos[i].uad_sigla;
                if (uad_sigla != null && uad_sigla != "")
                {
                    $('#divResultadoDRE label[for="chkResultado' + uad_sigla + '"]').parent().show();
                }
            }
        }
    }


    //VISIBILIDADE SELEÇÃO ESCOLA
    if (
        (nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO") &&
        edicao != "" &&
        ano != "" &&
        dres.length > 0
    ) 
    {
        $("#divResultadoEscola").show();
    }

    //VISIBILIDADE SELEÇÃO TURMA
    if (
        (nivel == "TURMA" || nivel == "ALUNO") &&
        edicao != "" &&
        ano != "" &&
        dres.length > 0 &&
        escolas.length > 0
    ) 
    {
        $("#divResultadoTurma").show();
    }

    //VISIBILIDADE SELEÇÃO ALUNO
    if (
        nivel == "ALUNO" &&
        edicao != "" &&
        ano != "" &&
        dres.length > 0 &&
        escolas.length > 0 &&
        turmas.length > 0
    ) 
    {
        $("#divResultadoAluno").show();
    }


    //VISIBILIDADE DOS TOTAIS SELECIONADOS
    $("#lblResultadoSelecaoTotalDRE").text("");
    if (dres.length > 1 && (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO"))
    {
        $("#lblResultadoSelecaoTotalDRE").text("Total de DREs: " + dres.length);
    }

    $("#lblResultadoSelecaoTotalEscola").text("");
    if (escolas.length > 1 && (nivel == "ESCOLA" || nivel == "TURMA" || nivel == "ALUNO"))
    {
        $("#lblResultadoSelecaoTotalEscola").text("Total de Escolas: " + escolas.length);
    }

    $("#lblResultadoSelecaoTotalTurma").text("");
    if (turmas.length > 1 && (nivel == "TURMA" || nivel == "ALUNO"))
    {
        $("#lblResultadoSelecaoTotalTurma").text("Total de Turmas: " + turmas.length);
    }

    $("#lblResultadoSelecaoTotalAluno").text("");
    if (alunos.length > 1 && nivel == "ALUNO")
    {
        $("#lblResultadoSelecaoTotalAluno").text("Total de Alunos: " + alunos.length);
    }



    $("#btnResultadoApresentar").prop("disabled", btnResultadoDesabilitado);
}

function definirEventHandlers()
{
    $("#btnAbrirResultados").unbind("click").click(
        function ()
        {


            if (!Usuario.AcessoNivelSME)
            {
                $("#ddlResultadoNivel_SME").remove();
                if (!Usuario.AcessoNivelDRE)
                {
                    $("#ddlResultadoNivel_DRE").remove();
                }
            }

            //resultado-page
            $(".page").hide();
            $("#resultado-page").show();

            $("#formResultadoOpcoes").show();
            $("#divResultadoApresentacao").hide();

            //$("#ddlResultadoNivel").unbind("change").change();
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
            //$("#txtResultadoTurma").val("");
            //$("#txtResultadoTurma")[0].selectize.clear();
            //$("#txtResultadoTurma")[0].selectize.clearOptions();

            $("#divResultadoAluno").hide();

            $("#divResultadoAlunoItens").html("");


            $("#ddlResultadoNivel").trigger("change");
            $("#divResultadoApresentacao").hide();

            $("#btnResultadoApresentar").prop("disabled", true);
            $("#btnResultadoApresentar").button("refresh");
        }
    );

    $("#ddlResultadoAlunoAreaConhecimento,#ddlResultadoAlunoEdicao").unbind("change").change(
        function ()
        {
            var Edicao = $("#ddlResultadoAlunoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAlunoAreaConhecimento").val();

            $(".divChartResultadoEscalaSaeb_1").html("");
            $(".divChartResultadoDetalhe").html("");

            if (Edicao != "" && AreaConhecimentoID != "")
            {
                $.mobile.loading("show", {
                    text: "Aguarde...",
                    textVisible: true,
                    theme: "a",
                    html: ""
                });


                var alu_matricula = Usuario.usu_login.replace("RA", "");
                var Edicao = $("#ddlResultadoAlunoEdicao").val();
                var AreaConhecimentoID = $("#ddlResultadoAlunoAreaConhecimento").val();

                $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), { Nivel: "ALUNO", Edicao: Edicao, AreaConhecimentoID: AreaConhecimentoID, AnoEscolar: "", lista_uad_sigla: "", lista_esc_codigo: "", lista_turmas: "", lista_alu_matricula: alu_matricula, ExcluirSme_e_Dre: "1" })
                    .done(function (dataResultado)
                    {
                        $.mobile.loading("hide");
                        resultadoApresentar($("#ddlResultadoAlunoEdicao").val(), $("#ddlResultadoAlunoAreaConhecimento").val(), dataResultado.AnoEscolar, "divResultadoApresentacaoAluno", dataResultado);
                    })
                    .fail(function (erro)
                    {
                        swal("Erro " + erro.status, erro.statusText, "error");
                        $.mobile.loading("hide");
                    });
            }
            else
            {
                $(".divChartResultadoDetalhe").html("");
                $(".divChartResultadoEscalaSaeb_1").html("");
            }
        }
    );

    $("#ddlResultadoNivel").unbind("change").change(
        function ()
        {
            /*
            $(".resultado-dre-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-escola-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-turma-chk").prop('checked', false).checkboxradio('refresh');
            $(".resultado-aluno-chk").prop('checked', false).checkboxradio('refresh');
            */

            $(".edicao-resultado-legado").hide();
            $(".edicao-resultado-legado").attr("disabled", "disabled");

            if (this.value == "ALUNO")
            {
                $(".edicao-resultado-legado").show();
                $(".edicao-resultado-legado").removeAttr("disabled");
            }

            if (this.value == "ESCOLA")
            {
                //$(".resultado-dre-chk").unbind("change").change(
                $(".resultado-dre-chk").trigger("change");
            }


            if (this.value != "")
            {
                $("#ddlResultadoEdicao").selectmenu("enable");

                if (this.value == "TURMA")
                {
                    $("#ddlResultadoEdicao_item_ENTURMACAO_ATUAL").show();
                }
                else
                {
                    if ($("#ddlResultadoEdicao").val() == "ENTURMACAO_ATUAL")
                    {
                        $("#ddlResultadoEdicao").val("");
                        $("#ddlResultadoEdicao").selectmenu("refresh");

                    }
                    $("#ddlResultadoEdicao_item_ENTURMACAO_ATUAL").hide();
                }
            }
            else
            {
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
        }
    );

    $("#ddlResultadoEdicao").unbind("change").change(
        function ()
        {
            resultado_configurarControles();

            if (this.value == "")
            {
                $("#ddlResultadoAreaConhecimento").val("");
                $("#ddlResultadoAreaConhecimento").selectmenu("refresh");
                $("#ddlResultadoAreaConhecimento").selectmenu("disable");

                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
                $("#ddlResultadoAno").selectmenu("disable");
            }

            var nivel = $("#ddlResultadoNivel").val();
            if (nivel == "TURMA" || nivel == "ALUNO")
            {
                $("#divResultadoTurmaItens").html("");
                $("#divResultadoAlunoItens").html("");
                $("#btnResultadoApresentar").prop("disabled", true);
            }
        }
    );

    $("#ddlResultadoAreaConhecimento").unbind("change").change(
        function ()
        {
            resultado_configurarControles();

            if (this.value == "")
            {
                $("#ddlResultadoAno").val("");
                $("#ddlResultadoAno").selectmenu("refresh");
                $("#ddlResultadoAno").selectmenu("disable");
            }
        }
    );

    $("#ddlResultadoAno").unbind("change").change(
        function ()
        {
            if (this.value != "")
            {
                var ano = this.value;
                //$("#txtResultadoTurma-selectized").attr("placeholder", "Informe uma ou mais turmas, ex: " + ano + "A, " + ano + "B, " + ano + "C");
            }

            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            resultado_configurarControles();
        }
    );


    $("#chkResultadoTodasDREs").unbind("click").click(
        function ()
        {
            $(".resultado-dre-chk").prop('checked', this.checked).checkboxradio('refresh');
        }
    );


    $(".resultado-dre-chk").unbind("change").change(
        function ()
        {

            $("#divResultadoEscolaItens").html("");
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            resultado_configurarControles();

            var DREs_selecionadas = $(".resultado-dre-chk:checked").map(function () { return this.value; }).get();

            var apresentarEscolas = false;

            if (DREs_selecionadas.length >= 1)
            {
                if ($("#ddlResultadoNivel").val() == "DRE")
                {
                    $("#btnResultadoApresentar").prop("disabled", false);
                }
                else
                {
                    apresentarEscolas = true;
                }
            }

            //
            if (apresentarEscolas)
            {
                ///////////////////////////////////////////////////////////////////////////////
                //Carregar escolas das DREs selecionadas:

                $.mobile.loading("show", {
                    text: "Aguarde...",
                    textVisible: true,
                    theme: "a",
                    html: ""
                });

                carregarDataEscola(
                    function ()
                    {

                        setTimeout(
                            function ()
                            {

                                var codigoEscolasAutorizadas = [];
                                for (var i = 0; i < Usuario.grupos.length; i++)
                                {
                                    if (Usuario.grupos[i].AcessoNivelEscola)
                                    {
                                        codigoEscolasAutorizadas.push(Usuario.grupos[i].esc_codigo);
                                    }
                                }


                                var l = dataEscola.length;
                                var codigoJSTodasEscolas = '$(".resultado-escola-item-chk").prop("checked", this.checked).checkboxradio("refresh"); resultado_configurarControles();';
                                $("#divResultadoEscolaItens").append("<label id='lblResultadoTodasEscolas' for='chkResultadoTodasEscolas'><input id='chkResultadoTodasEscolas' type='checkbox' name='chkResultadoEscola' class='resultado-escola-chk' value='TD' data-mini='true' onclick='" + codigoJSTodasEscolas + "' />(TODAS ESCOLAS)</label>");
                                var escolasEncontradas = 0;

                                for (var i = 1; i < l; i++)
                                {
                                    var r = dataEscola[i].split(";");
                                    //uad_sigla; esc_codigo; esc_nome
                                    var uad_sigla = r[0];
                                    var esc_codigo = r[1];
                                    var esc_nome = r[2];


                                    //PERMISSÃO DE VISIBILIDADE PARA A ESCOLA:
                                    var incluirEscola = (Usuario.AcessoNivelSME || Usuario.AcessoNivelDRE || codigoEscolasAutorizadas.indexOf(esc_codigo) >= 0);


                                    if (incluirEscola && (DREs_selecionadas.indexOf("TD") >= 0 || DREs_selecionadas.indexOf(uad_sigla) >= 0))
                                    {
                                        escolasEncontradas++;
                                        var chkID = "chkResultadoEscola_" + esc_codigo;
                                        var lblID = "lblResultadoEscola_" + esc_codigo;
                                        $("#divResultadoEscolaItens").append("<label id='" + lblID + "' for='" + chkID + "' class='resultado-escola-lbl'><input id='" + chkID + "' type='checkbox' name='chkResultadoEscola' class='resultado-escola-chk resultado-escola-item-chk' value='" + esc_codigo + "' data-mini='true' />" + esc_nome + "</label>");
                                        //console.log(lblID);
                                    }
                                }
                                if (escolasEncontradas > 0)
                                {
                                    $("#divResultadoEscola").show();
                                    $("#divResultadoEscolaItens").trigger("create");

                                    $(".resultado-escola-chk").unbind("click").click(
                                        function ()
                                        {
                                            $("#divResultadoTurmaItens").html("");
                                            $("#divResultadoAlunoItens").html("");
                                            //$("#divResultadoAlunoItens").trigger("create");

                                            resultado_configurarControles();
                                        }
                                    );
                                }
                                $.mobile.loading("hide");
                            }, 100
                        );


                        //$("#ddlResultadoEscola").selectmenu("refresh", true);
                    }
                    , ""
                );

                $("#txtResultadoEscolaFiltro").unbind("change").change(
                    function ()
                    {
                        var valorFiltro = $("#txtResultadoEscolaFiltro").val().trim().toUpperCase();
                        if (valorFiltro == "")
                        {
                            $(".resultado-escola-lbl").show();
                            $("#lblResultadoTodasEscolas").show();
                        }
                        else
                        {
                            $("#lblResultadoTodasEscolas").hide();
                            $(".resultado-escola-lbl").hide();
                            $(".resultado-escola-lbl").filter(
                                function ()
                                {
                                    return $(this).text().toUpperCase().indexOf(valorFiltro) >= 0 || $("#" + this.id.replace("lbl", "chk"))[0].checked;
                                }
                            ).show();

                            $("#divResultadoEscolaItens").trigger("create");
                        }

                    }
                );

            }

        }
    );

    $(".resultado-dre-item-chk").unbind("click").click(
        function ()
        {
            $("#chkResultadoTodasDREs").prop('checked', false).checkboxradio('refresh');
            resultado_configurarControles();
        }
    );

    $("#txtResultadoTurma").unbind("change").change(
        function ()
        {
            resultado_configurarControles();

        }
    );


    $("#btnResultadoAlterarParametros, #btnResultadoAlterarParametros2").unbind("click").click(
        function ()
        {
            $("#formResultadoOpcoes").show();
            $("#divResultadoApresentacao").hide();
            $.mobile.silentScroll(0);
        }
    );

    $("#btnResultadoBuscarTurmas").unbind("click").click(
        function ()
        {
            $("#divResultadoTurmaItens").html("");
            $("#divResultadoAlunoItens").html("");

            var Edicao = $("#ddlResultadoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
            var AnoEscolar = $("#ddlResultadoAno").val();
            var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();

            if (lista_esc_codigo.split(",").length >= 100)
            {
                sweetAlert("Sobrecarga", "Por gentileza especifique menos de 100 escolas na busca de turmas.", "error");
                return;
            }

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            var ResultadoNivel = $("#ddlResultadoNivel").val();

            $.post(urlBackEnd + "api/ResultadoRecuperarTurmas", { ResultadoNivel: ResultadoNivel, Edicao: Edicao, AreaConhecimentoID: AreaConhecimentoID, AnoEscolar: AnoEscolar, lista_esc_codigo: lista_esc_codigo })
                .done(function (data)
                {
                    $.mobile.loading("hide");

                    if (data.length == 0)
                    {
                        sweetAlert("Nenhuma turma encontrada", "", "error");
                        return;
                    }

                    //if (data.length > 1000)
                    //{
                    //    sweetAlert("Erro de sobrecarga", "Sua pesquisa retornou mais de 1000 turmas. Por gentileza especifique critérios mais restritos.", "error");
                    //    return;
                    //}

                    if (data.length > 0)
                    {
                        var codigoJSTodasTurmas = '$(".resultado-turma-item-chk").prop("checked", this.checked).checkboxradio("refresh"); resultado_configurarControles();';
                        $("#divResultadoTurmaItens").append("<label for='chkResultadoTodosTurmas' id='lblResultadoTodosTurmas'><input id='chkResultadoTodasTurmas' type='checkbox' name='chkResultadoTurma' class='resultado-turma-chk' value='TD' data-mini='true' onclick='" + codigoJSTodasTurmas + "' />(TODAS TURMAS)</label>");

                        for (var i = 0; i < data.length; i++)
                        {
                            var id = "";
                            var value = "";
                            var text = "";
                            if (data[i].tur_id > 0)
                            {
                                id = "chkResultadoTurma_" + data[i].tur_id;
                                value = data[i].tur_id;
                                if (lista_esc_codigo.split(",").length > 1)
                                    text = data[i].esc_nome + " - " + data[i].tur_codigo;
                                else
                                    text = data[i].tur_codigo;
                            }
                            else
                            {
                                id = "chkResultadoTurma_" + data[i].tur_codigo;
                                value = data[i].tur_codigo;
                                text = data[i].tur_codigo;
                            }
                            $("#divResultadoTurmaItens").append("<label for='" + id + "'><input id='" + id + "' type='checkbox' name='chkResultadoTurma' class='resultado-turma-chk resultado-turma-item-chk' value='" + value + "' data-mini='true' />" + text + "</label>");
                        }

                        $("#divResultadoTurmaItens").trigger("create");

                        $(".resultado-turma-item-chk").unbind("click").click(
                            function ()
                            {
                                $("#divResultadoAlunoItens").html("");
                                $("#chkResultadoTodasTurmas").prop('checked', false).checkboxradio('refresh');
                                resultado_configurarControles();
                            }
                        );
                    }

                })
                .fail(function (xhr, status, error)
                {
                    $.mobile.loading("hide");
                    sweetAlert("Falha de comunicação", "Não foi possível recuperar as turmas. (" + status + ") " + error, "error");
                });
        }
    );

    $("#btnResultadoBuscarAlunos").unbind("click").click(
        function ()
        {
            var Edicao = $("#ddlResultadoEdicao").val();
            var AreaConhecimentoID = $("#ddlResultadoAreaConhecimento").val();
            var AnoEscolar = $("#ddlResultadoAno").val();
            var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();
            var lista_turmas = $(".resultado-turma-chk:checked").map(function () { return this.value; }).get().toString(); //$("#txtResultadoTurma").val();

            if (lista_esc_codigo.split(",").length >= 20)
            {
                sweetAlert("Sobrecarga", "Por gentileza especifique menos de 20 escolas na busca de alunos.", "error");
                return;
            }

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            $("#divResultadoAlunoItens").html("");

            $.post(urlBackEnd + "api/ResultadoRecuperarAlunos", { Edicao: Edicao, AreaConhecimentoID: AreaConhecimentoID, AnoEscolar: AnoEscolar, lista_esc_codigo: lista_esc_codigo, lista_turmas: lista_turmas })
                .done(function (data)
                {
                    $.mobile.loading("hide");

                    if (data.length == 0)
                    {
                        sweetAlert("Nenhum aluno encontrado", "", "error");
                        return;
                    }

                    if (data.length > 1000)
                    {
                        sweetAlert("Erro de sobrecarga", "Sua pesquisa retornou mais de 1000 alunos. Por gentileza especifique critérios mais restritos.", "error");
                        return;
                    }

                    if (data.length > 0)
                    {
                        var codigoJSTodosAlunos = '$(".resultado-aluno-item-chk").prop("checked", this.checked).checkboxradio("refresh"); resultado_configurarControles();';
                        $("#divResultadoAlunoItens").append("<label for='chkResultadoTodosAlunos' id='lblResultadoTodosAlunos'><input id='chkResultadoTodosAlunos' type='checkbox' name='chkResultadoAluno' class='resultado-aluno-chk' value='TD' data-mini='true' onclick='" + codigoJSTodosAlunos + "' />(TODOS ALUNOS)</label>");

                        for (var i = 0; i < data.length; i++)
                        {
                            $("#divResultadoAlunoItens").append("<label for='chkResultadoAluno_" + data[i].alu_matricula + "'><input id='chkResultadoAluno_" + data[i].alu_matricula + "' type='checkbox' name='chkResultadoAluno' class='resultado-aluno-chk resultado-aluno-item-chk' value='" + data[i].alu_matricula + "' data-mini='true' />" + data[i].Nome + " (" + data[i].alu_matricula + ")</label>");
                        }

                        $("#divResultadoAlunoItens").trigger("create");

                        $(".resultado-aluno-item-chk").unbind("click").click(
                            function ()
                            {
                                $("#chkResultadoTodosAlunos").prop('checked', false).checkboxradio('refresh');
                                resultado_configurarControles();
                            }
                        );
                    }

                })
                .fail(function (xhr, status, error)
                {
                    $.mobile.loading("hide");
                    sweetAlert("Falha de comunicação", "Não foi possível recuperar os alunos. (" + status + ") " + error, "error");
                });
        }
    );

    //resultadoTabHabilidades
    $("#resultadoTabHabilidades").unbind("click").click(
        function ()
        {
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

            $.post(urlBackEnd + "api/ResultadoRecuperarAlunos", { Edicao: Edicao, AreaConhecimentoID: AreaConhecimentoID, AnoEscolar: AnoEscolar, lista_esc_codigo: lista_esc_codigo, lista_turmas: lista_turmas })
                .done(function (data)
                {
                    $.mobile.loading("hide");



                })
                .fail(function (xhr, status, error)
                {
                    $.mobile.loading("hide");
                    sweetAlert("Falha de comunicação", "Não foi possível recuperar as habilidades. (" + status + ") " + error, "error");
                });
        }
    );

    var chartResultadoAgregacao_ctx = null;
    var chartResultadoEscalaSaeb_ctx = null;
    var chartResultadoDetalhe_ctx = null;
    $("#btnResultadoApresentar, #chkResultadoFiltroAbaixoDoBasico, #chkResultadoFiltroBasico, #chkResultadoFiltroAdequado, #chkResultadoFiltroAvancado").unbind("click").click(
        function ()
        {

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            var nivel = $("#ddlResultadoNivel").val();
            var edicao = $("#ddlResultadoEdicao").val();
            var areaConhecimentoId = $("#ddlResultadoAreaConhecimento").val();
            var anoEscolar = $("#ddlResultadoAno").val();
            var lista_uad_sigla = "";
            var lista_esc_codigo = "";
            var lista_turmas = "";
            var lista_alu_matricula = "";

            if (nivel == "DRE")
            {
                lista_uad_sigla = $(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "ESCOLA")
            {
                lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "TURMA")
            {
                lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().toString();
                lista_turmas = $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get().toString();
            }
            else if (nivel == "ALUNO")
            {
                lista_alu_matricula = $(".resultado-aluno-item-chk:checked").map(function () { return this.value; }).get().toString();
            }

            //var lista_esc_codigo = $(".resultado-escola-chk:checked").map(function () { return this.value; }).get().toString();
            //var lista_turmas = $(".resultado-turma-chk:checked").map(function () { return this.value; }).get().toString(); //$("#txtResultadoTurma").val();

            $.post(urlBackEnd + "api/ResultadoPorNivel?guid=" + newGuid(), { Nivel: nivel, Edicao: edicao, AreaConhecimentoID: areaConhecimentoId, AnoEscolar: anoEscolar, lista_uad_sigla: lista_uad_sigla, lista_esc_codigo: lista_esc_codigo, lista_turmas: lista_turmas, lista_alu_matricula: lista_alu_matricula })
                .done(function (dataResultado)
                {
                    $.mobile.loading("hide");
                    resultadoApresentar($("#ddlResultadoEdicao").val(), $("#ddlResultadoAreaConhecimento").val(), $("#ddlResultadoAno").val(), "divResultadoApresentacao", dataResultado);
                })
                .fail(function (erro)
                {
                    swal("Erro " + erro.status, erro.statusText, "error");
                    $.mobile.loading("hide");
                });

        }
    );


    function resultadoApresentar(edicao, areaConhecimentoId, ano, divResultadoContainer, dataResultado)
    {
        configurarPluginsChartsJS();



        $("#formResultadoOpcoes").hide();
        $("#" + divResultadoContainer).show();

        if (divResultadoContainer == "divResultadoApresentacaoAluno")
            $(".lblResultadoTitulo").html("Resultado referente ao " + dataResultado.AnoEscolar + "º Ano");
        else
            $(".lblResultadoTitulo").html($("#ddlResultadoNivel option:selected").text());

        $("#lblResultadoEdicao").html($("#ddlResultadoEdicao option:selected").text());
        $("#lblResultadoAreaConhecimento").html($("#ddlResultadoAreaConhecimento option:selected").text());
        $("#lblResultadoAno").html($("#ddlResultadoAno option:selected").text());

        $("#lblResultadoTituloAgregacao").html("");
        $("#lblResultadoTituloDetalhe").html("");

        var nivel = $("#ddlResultadoNivel").val();
        var edicao = $("#ddlResultadoEdicao").val();
        var areaConhecimento = $("#ddlResultadoAreaConhecimento").val();
        var lista_esc_codigo = $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get();

        if (nivel == "SME")
        {
            $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da SME nos níveis: abaixo do básico, básico, adequado e avançado.");
            $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada DRE.");
        }
        else if (nivel == "DRE")
        {
            if (dataResultado.Agregacao.length == 1)
                $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da DRE nos níveis: abaixo do básico, básico, adequado e avançado.");
            else
                $("#lblResultadoTituloAgregacao").html("Nos gráficos abaixo encontram-se a distribuição de todos os alunos das DREs selecionadas nos níveis: abaixo do básico, básico, adequado e avançado.");
            $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Escola.");
        }
        else if (nivel == "ESCOLA")
        {
            if (dataResultado.Agregacao.length == 1)
                $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da Escola nos níveis: abaixo do básico, básico, adequado e avançado.");
            else
                $("#lblResultadoTituloAgregacao").html("Nos gráficos abaixo encontram-se a distribuição de todos os alunos das Escolas selecionadas nos níveis: abaixo do básico, básico, adequado e avançado.");
            $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Turma.");
        }
        else if (nivel == "TURMA")
        {
            if (dataResultado.Agregacao.length == 1)
                $("#lblResultadoTituloAgregacao").html("No gráfico abaixo encontra-se a distribuição de todos os alunos da Turma nos níveis: abaixo do básico, básico, adequado e avançado.");
            else
                $("#lblResultadoTituloAgregacao").html("Nos gráfico abaixo encontram-se a distribuição de todos os alunos das Turmas selecionadas nos níveis: abaixo do básico, básico, adequado e avançado.");
            $("#lblResultadoTituloDetalhe").html("Abaixo segue o detalhamento de proficiência de cada Aluno.");
        }

        if (dataResultado.Agregacao.length == 0)
            $("#divResultadoTituloAgregacao").hide();
        else
            $("#divResultadoTituloAgregacao").show();

        if (dataResultado.Itens.length == 0)
            $("#divResultadoTituloDetalhe").hide();
        else
            $("#divResultadoTituloDetalhe").show();


        if (edicoesComTurmasAmostrais.indexOf(edicao) >= 0 && ano % 2 == 0)
            $("#divResultadoTituloAmostral").show();
        else
            $("#divResultadoTituloAmostral").hide();


        var proficienciaMaxima = 500;

        var reguaProficiencia = {};

        if (areaConhecimentoId == "1") //Ciências
        {
            reguaProficiencia["3"] = [125, 175, 225]; //3° Ano
            reguaProficiencia["4"] = [150, 200, 250]; //4° Ano
            reguaProficiencia["5"] = [175, 225, 275]; //5° Ano
            reguaProficiencia["6"] = [190, 240, 290]; //6° Ano
            reguaProficiencia["7"] = [200, 250, 300]; //7° Ano
            reguaProficiencia["8"] = [210, 275, 325]; //8° Ano
            reguaProficiencia["9"] = [225, 300, 350]; //9° Ano
        }
        else if (areaConhecimentoId == "2") //Língua Portuguesa
        {
            reguaProficiencia["3"] = [115, 150, 200]; //3° Ano
            reguaProficiencia["4"] = [135, 175, 225]; //4° Ano
            reguaProficiencia["5"] = [150, 200, 250]; //5° Ano
            reguaProficiencia["6"] = [165, 215, 265]; //6° Ano
            reguaProficiencia["7"] = [175, 225, 275]; //7° Ano
            reguaProficiencia["8"] = [185, 250, 300]; //8° Ano
            reguaProficiencia["9"] = [200, 275, 325]; //9° Ano
        }
        else if (areaConhecimentoId == "3") //Matemática
        {
            reguaProficiencia["3"] = [125, 175, 225]; //3° Ano
            reguaProficiencia["4"] = [150, 200, 250]; //4° Ano
            reguaProficiencia["5"] = [175, 225, 275]; //5° Ano
            reguaProficiencia["6"] = [190, 240, 290]; //6° Ano
            reguaProficiencia["7"] = [200, 250, 300]; //7° Ano
            reguaProficiencia["8"] = [210, 275, 325]; //8° Ano
            reguaProficiencia["9"] = [225, 300, 350]; //9° Ano
        }
        else if (areaConhecimentoId == "4") //Redação
        {

            var proficienciaMaxima = 100;

            reguaProficiencia["3"] = [50, 65, 90]; //3° Ano
            reguaProficiencia["4"] = [50, 65, 90]; //4° Ano
            reguaProficiencia["5"] = [50, 65, 90]; //5° Ano
            reguaProficiencia["6"] = [50, 65, 90]; //6° Ano
            reguaProficiencia["7"] = [50, 65, 90]; //7° Ano
            reguaProficiencia["8"] = [50, 65, 90]; //8° Ano
            reguaProficiencia["9"] = [50, 65, 90]; //9° Ano
        }

        //var anoSelecionado = $("#ddlResultadoAno").val();

        var intervaloGrafico = [
            reguaProficiencia[ano][0],
            reguaProficiencia[ano][1] - reguaProficiencia[ano][0],
            reguaProficiencia[ano][2] - reguaProficiencia[ano][1],
            proficienciaMaxima - reguaProficiencia[ano][2]
        ];

        var tituloAbaixoDoBasico = "Abaixo do básico (<" + reguaProficiencia[ano][0] + ")";
        var tituloBasico = "Básico (>=" + reguaProficiencia[ano][0] + " e <" + reguaProficiencia[ano][1] + ")";
        var tituloAdequado = "Adequado (>=" + reguaProficiencia[ano][1] + " e <" + reguaProficiencia[ano][2] + ")";
        var tituloAvancado = "Avançado (>=" + reguaProficiencia[ano][2] + ")";








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

        var tituloNivel = ["", "Abaixo do básico", "Básico", "Adequado", "Avançado"];


        for (var i = 0; i < dataResultado.Agregacao.length; i++)
        {
            var agregacao = dataResultado.Agregacao[i];

            $("#divChartResultadoAgregacao").append("<canvas id='chartResultadoAgregacao" + i + "' style='margin-top:15px;'></canvas>");

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
                        text: [agregacao.Titulo, "Proficiência: " + agregacao.Valor + " (" + tituloNivel[agregacao.NivelProficienciaID] + ") - Total de alunos: " + agregacao.TotalAlunos, ""],
                        fontFamily: "'Open Sans Bold', sans-serif",
                        fontSize: 15,
                    },
                    legend: {
                        position: "right"
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem, data)
                            {
                                //get the concerned dataset
                                var dataset = data.datasets[tooltipItem.datasetIndex];
                                //calculate the total of this data set
                                var total = dataset.data.reduce(function (previousValue, currentValue, currentIndex, array)
                                {
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
                        onComplete: function ()
                        {
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

            if (nivel == "ESCOLA")
            {
                var areaConhecimentoBoletim = areaConhecimento;
                if (areaConhecimentoBoletim == 4) areaConhecimentoBoletim=2
                var urlBoletim = urlBackEnd + "boletim_escola/" + edicao + "/" + areaConhecimentoBoletim + "/" + agregacao.Chave + ".pdf";
                $("#divChartResultadoAgregacao").append("<a id='btnResultadoBoletimEscola" + i + "' class='ui-btn' href='" + urlBoletim + "' target='blank'>Baixar boletim da Escola</a>");
            }
        }



        $("#" + divResultadoContainer + " .lblResultadoTituloEscalaSaeb_1").html("");
        $("#" + divResultadoContainer + " .lblResultadoTituloEscalaSaeb_2").html("");

        $("#" + divResultadoContainer + " .divChartResultadoEscalaSaeb_1").empty();
        $("#" + divResultadoContainer + " .divChartResultadoEscalaSaeb_2").empty();

        var intervaloGrafico2 = intervaloGrafico;

        if (edicao == "ENTURMACAO_ATUAL")
        {
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
        }



        document.getElementById('divChartResultadoDetalhe').style.overflow = 'visible';
        document.getElementById('divChartResultadoDetalhe').style.height = '100%';

        if (edicao == "ENTURMACAO_ATUAL")
        {
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
                    label: "Abaixo do básico (<" + reguaProficiencia[anoAplicacaoProva][0] + ")" //label: "Abaixo do básico" //label: "Abaixo do básico (<" + intervaloGrafico[0] +")"
                }, {
                    data: [intervaloGrafico[1]],
                    backgroundColor: corNivelBasico,
                    label: "Básico (>=" + reguaProficiencia[anoAplicacaoProva][0] + " e <" + reguaProficiencia[anoAplicacaoProva][1] + ")" //label: "Básico" //label: "Básico (>=" + intervaloGrafico[0] + " e <" + intervaloGrafico[1] + ")"
                }, {
                    data: [intervaloGrafico[2]],
                    backgroundColor: corNivelAdequado,
                    label: "Adequado (>=" + reguaProficiencia[anoAplicacaoProva][1] + " e <" + reguaProficiencia[anoAplicacaoProva][2] + ")" //label: "Adequado" //label: "Adequado (>=" + intervaloGrafico[1] + " e <" + intervaloGrafico[2] + ")"
                }, {
                    data: [intervaloGrafico[3]],
                    backgroundColor: corNivelAvancado,
                    label: "Avançado (>=" + reguaProficiencia[anoAplicacaoProva][2] + ")" //label: "Avançado" //label: "Avançado (>=" + intervaloGrafico[3] + ")"
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
        else
        {
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




        $("#" + divResultadoContainer + " .divChartResultadoDetalhe").empty().append("<canvas id='chartResultadoDetalhe'></canvas>");


        if (chartResultadoDetalhe_ctx != null)
        {
            chartResultadoDetalhe_ctx.destroy();
        }


        var chartResultadoDetalhe_ctx = document.getElementById("chartResultadoDetalhe").getContext("2d");

       
        if (dataResultado.Itens.length == 1)
        {
            chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 45; //300;
        }
        else if (dataResultado.Itens.length <= 4)
        {
            chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 40; //300;
        }
        else if (dataResultado.Itens.length == 5)
        {
            chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 35; //300;
        }
        else
        {
            if (edicao == "ENTURMACAO_ATUAL")
            {
                chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 60; //300;
            }
            else
            {
                chartResultadoDetalhe_ctx.canvas.height = dataResultado.Itens.length * 50; //300;
            }

        }


        //$("#divChartResultadoDetalhe").show();
        //$("#imgChartResultadoDetalhe").hide();

        for (var i = 0; i < dataResultado.Itens.length; i++)
        {
            if (dataResultado.Itens[i].Valor == -1)
            {
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
                maintainAspectRatio: true,

                legend: {
                    display: false
                },
                animation: {

                    onComplete: function (c)
                    {
                        setTimeout(
                            function ()
                            {
                                var labelOffset = c.chart.getDatasetMeta(0).data[0]._model.height;

                                if ($("#ddlResultadoEdicao").val() == "ENTURMACAO_ATUAL")
                                {
                                    //No caso de ENTURMACAO_ATUAL são apresentados 2 gráficos: a proficiência do ano anterior e a atual.
                                    //Por essa razão é preciso dobrar o salto da label:
                                    labelOffset *= 2; 
                                }
                                
                                c.chart.options.scales.yAxes[0].ticks.minor.labelOffset = -labelOffset;
                                //c.chart.options.scales.yAxes[0].ticks.labelOffset = -labelOffset;
                                c.chart.update();
                            }
                            ,250
                        );

                        
                        
                    }
                },
                showAllTooltips: true,
                tooltips: {
                    enabled: true,
                    backgroundColor: "rgba(100,100,100,1)",
                    callbacks: {
                        title: function (tooltipItem, data)
                        {
                            //CUSTOMIZAÇÃO DA TOOLTIP

                            //return data['labels'][tooltipItem[0]['index']];
                            return "";
                        },
                        label: function (tooltipItem, data)
                        {
                            //CUSTOMIZAÇÃO DA TOOLTIP
                            var valorProficiencia = data['datasets'][0]['data'][tooltipItem['index']];

                            var anoRef = ano;
                            if (tooltipItem.datasetIndex == 0 && edicao == "ENTURMACAO_ATUAL")
                                anoRef = anoAplicacaoProva;


                            var NivelProficienciaID_ENTURMACAO = 0;
                            if (valorProficiencia < reguaProficiencia[anoRef][0])
                                NivelProficienciaID_ENTURMACAO = 1;
                            else if (valorProficiencia >= reguaProficiencia[anoRef][0] && valorProficiencia < reguaProficiencia[anoRef][1])
                                NivelProficienciaID_ENTURMACAO = 2;
                            else if (valorProficiencia >= reguaProficiencia[anoRef][1] && valorProficiencia < reguaProficiencia[anoRef][2])
                                NivelProficienciaID_ENTURMACAO = 3;
                            else if (valorProficiencia >= reguaProficiencia[anoRef][2])
                                NivelProficienciaID_ENTURMACAO = 4;

                            if (tooltipItem.datasetIndex == 0)
                                return "Régua do " + anoRef + "º ano: " + tituloNivel[NivelProficienciaID_ENTURMACAO];
                            else
                                return "Régua do " + anoRef + "º ano: " + tituloNivel[NivelProficienciaID_ENTURMACAO];

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

        if (edicao == "ENTURMACAO_ATUAL")
        {
            chartResultadoDetalhe.data.datasets.push({ data: [], backgroundColor: [] });
        }

        var filtroProficiencia = $(".resultado-filtro-proficiencia:checked").map(function () { return this.value; }).get();
        
        for (var i = 0; i < dataResultado.Itens.length; i++)
        {
            var item = dataResultado.Itens[i];

            if (filtroProficiencia.indexOf(item.NivelProficienciaID.toString())>=0)
            {

                var tituloAtual = item.Titulo + ": " + item.Valor;

                if (tituloAtual.length > maiorTitulo.length)
                {
                    maiorTitulo = tituloAtual;
                }
                chartResultadoDetalhe.data.labels.push(item.Titulo + ": " + item.Valor);
                chartResultadoDetalhe.data.datasets[0].data.push(item.Valor);
                chartResultadoDetalhe.data.datasets[0].backgroundColor.push(hashtableProficienciaId_cor[item.NivelProficienciaID]);
                if (edicao == "ENTURMACAO_ATUAL")
                {

                    chartResultadoDetalhe.data.datasets[1].data.push(item.Valor);

                    var NivelProficienciaID_ENTURMACAO;
                    if (item.Valor < reguaProficiencia[ano][0])
                        NivelProficienciaID_ENTURMACAO = 1;
                    else if (item.Valor >= reguaProficiencia[ano][0] && item.Valor < reguaProficiencia[ano][1])
                        NivelProficienciaID_ENTURMACAO = 2;
                    else if (item.Valor >= reguaProficiencia[ano][1] && item.Valor < reguaProficiencia[ano][2])
                        NivelProficienciaID_ENTURMACAO = 3;
                    else if (item.Valor >= reguaProficiencia[ano][2])
                        NivelProficienciaID_ENTURMACAO = 4;

                    chartResultadoDetalhe.data.datasets[1].backgroundColor.push(hashtableProficienciaId_enturmacao_cor[NivelProficienciaID_ENTURMACAO]);


                    chartResultadoDetalhe.data.datasets[0].label = "Régua do " + anoAplicacaoProva + "º ano";
                    chartResultadoDetalhe.data.datasets[1].label = "Régua do " + ano + "º ano";
                }

            }

            
        }

        if (dataResultado.Itens.length > 1)
        {
            if (dataResultado.Valor > 0)
            {
                chartResultadoDetalhe.data.labels.push("MÉDIA: " + dataResultado.Valor);
                chartResultadoDetalhe.data.datasets[0].data.push(dataResultado.Valor);
                chartResultadoDetalhe.data.datasets[0].backgroundColor.push(hashtableProficienciaId_cor[dataResultado.NivelProficienciaID]);
            }
        }

        //var yScale = chartResultadoDetalhe.scales['y-axis-0'];
        //var yLabelOffset = (yScale.getPixelForTick(0) - yScale.getPixelForTick(1)) / 2;
        //chartResultadoDetalhe.options.scales.yAxes[0].ticks.minor.labelOffset = yLabelOffset;
        


        chartResultadoDetalhe.update();

        if (dataResultado.Habilidades != null)
        {
            configurarHabilidades(dataResultado);
            $("#divTabsProficienciaHabilidade").show();
        }
        else
        {
            $("#divResultadoTabProficiencias").show();
            $("#divTabsProficienciaHabilidade").hide();
            $("#divResultadoTabHabilidades").hide();
        }


        $('#divChartResultadoDetalhe').scrollTop(0);

    }

    function configurarHabilidades(dataResultado)
    {
        var nivel = $("#ddlResultadoNivel").val();

        var selecaoMultipla = false;

        if (nivel == "DRE")
        {
            selecaoMultipla = ($(".resultado-dre-item-chk:checked").map(function () { return this.value; }).get().length > 1);
        }
        else if (nivel == "ESCOLA")
        {
            selecaoMultipla = ($(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().length > 1);
        }
        else if (nivel == "TURMA")
        {
            selecaoMultipla = (
                $(".resultado-turma-item-chk:checked").map(function () { return this.value; }).get().length > 1 ||
                $(".resultado-escola-item-chk:checked").map(function () { return this.value; }).get().length > 1);
        }


        $("#divResultadoTabHabilidades_conteudoDinamico").empty();

        for (var iTema = 0; iTema < dataResultado.Habilidades.length; iTema++)
        {
            var tema = dataResultado.Habilidades[iTema];
            $("#divResultadoTabHabilidades_conteudoDinamico").append("<h4 style='margin-top:15px;'>" + tema.Titulo + "</h4>");




            var htmTabela = "<table class='greyGridTable'><thead><tr><td>Habilidade</td><td>Descrição</td><td>SME(%)</td>";
            if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA")
            {
                htmTabela += "<td>DRE(%)</td>";
            }
            if (nivel == "ESCOLA" || nivel == "TURMA")
            {
                htmTabela += "<td>ESCOLA(%)</td>";
            }
            if (nivel == "TURMA")
            {
                htmTabela += "<td>TURMA(%)</td>";
            }
            htmTabela += "</tr></thead>";


            var estruturaHabilidadeMultiNivel = function ()
            {
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

            for (var iHabilidade = 0; iHabilidade < tema.Itens.length; iHabilidade++)
            {
                var habilidade = tema.Itens[iHabilidade];

                if (tesseratoHabilidade[habilidade.OrigemTitulo] == null)
                {
                    tesseratoHabilidade[habilidade.OrigemTitulo] = new estruturaHabilidadeMultiNivel();

                    if (tema.Itens.length < 3)
                    {

                        for (var itemp = 0; itemp < 3 - tema.Itens.length; itemp++)
                        {
                            //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeLabel.push("Eixo virtual");
                            //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorSME.push(100);
                            //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorDRE.push(100);
                            //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorESCOLA.push(100);
                            //tesseratoHabilidade[habilidade.OrigemTitulo].arrayHabilidadeValorTURMA.push(100);
                        }
                    }

                }

                var dimensaoHabilidade = tesseratoHabilidade[habilidade.OrigemTitulo];

                if (selecaoMultipla)
                {
                    htmTabela += "<tr><td colspan='6' style='background:rgb(200,200,200);'>" + habilidade.OrigemTitulo + ":</td></tr>";
                }

                htmTabela += "<tr><td>" + habilidade.Codigo + "</td><td style='text-align: left;'>" + habilidade.Descricao + "</td><td>" + habilidade.PercentualAcertosNivelSME + "</td>";


                arrayHabilidadeValorSME.push(habilidade.PercentualAcertosNivelSME);
                arrayHabilidadeLabel.push(habilidade.Codigo);

                dimensaoHabilidade.arrayHabilidadeValorSME.push(habilidade.PercentualAcertosNivelSME);
                dimensaoHabilidade.arrayHabilidadeLabel.push(habilidade.Codigo);

                if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA")
                {
                    htmTabela += "<td>" + habilidade.PercentualAcertosNivelDRE + "</td>";
                    arrayHabilidadeValorDRE.push(habilidade.PercentualAcertosNivelDRE);
                    dimensaoHabilidade.arrayHabilidadeValorDRE.push(habilidade.PercentualAcertosNivelDRE);
                }
                if (nivel == "ESCOLA" || nivel == "TURMA")
                {
                    htmTabela += "<td>" + habilidade.PercentualAcertosNivelEscola + "</td>";
                    arrayHabilidadeValorESCOLA.push(habilidade.PercentualAcertosNivelEscola);
                    dimensaoHabilidade.arrayHabilidadeValorESCOLA.push(habilidade.PercentualAcertosNivelEscola);
                }
                if (nivel == "TURMA")
                {
                    htmTabela += "<td>" + habilidade.PercentualAcertosNivelTurma + "</td>";
                    arrayHabilidadeValorTURMA.push(habilidade.PercentualAcertosNivelTurma);
                    dimensaoHabilidade.arrayHabilidadeValorTURMA.push(habilidade.PercentualAcertosNivelTurma);
                }
            }

            htmTabela += "</table>";


            //CONFIGURAÇÃO DO(s) GRÁFICO(s) DE RADAR:
            var itesserato = 0;
            for (var chaveTesserato in tesseratoHabilidade)
            {
                var dimensaoHabilidade = tesseratoHabilidade[chaveTesserato];

                if (chaveTesserato != "null")
                {
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

                var conjuntoDATASETS = [datasetSME];

                if (nivel == "DRE" || nivel == "ESCOLA" || nivel == "TURMA")
                {
                    conjuntoDATASETS.push(datasetDRE);
                }
                if (nivel == "ESCOLA" || nivel == "TURMA")
                {
                    conjuntoDATASETS.push(datasetESCOLA);
                }
                if (nivel == "TURMA")
                {
                    conjuntoDATASETS.push(datasetTURMA);
                }


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









            $("#divResultadoTabHabilidades_conteudoDinamico").append(htmTabela);
        }
    }

    function configurarPluginsChartsJS()
    {
        //PLUGIN "showAllTooltips":
        Chart.plugins.register({
            beforeRender: function (chart)
            {
                if (chart.config.options.showAllTooltips)
                {
                    // create an array of tooltips
                    // we can't use the chart tooltip because there is only one tooltip per chart
                    chart.pluginTooltips = [];
                    chart.config.data.datasets.forEach(function (dataset, i)
                    {
                        chart.getDatasetMeta(i).data.forEach(function (sector, j)
                        {
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
            afterDraw: function (chart, easing)
            {
                if (chart.config.options.showAllTooltips)
                {
                    // we don't want the permanent tooltips to animate, so don't do anything till the animation runs atleast once
                    if (!chart.allTooltipsOnce)
                    {
                        if (easing !== 1)
                            return;
                        chart.allTooltipsOnce = true;
                    }

                    // turn on tooltips
                    chart.options.tooltips.enabled = true;
                    Chart.helpers.each(chart.pluginTooltips, function (tooltip)
                    {
                        // This line checks if the item is visible to display the tooltip
                        if (!tooltip._active[0].hidden)
                        {
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

    function recuperarAnoEnturmacao(dataResultado)
    {
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

    function configurarReguaSaeb(divResultadoContainer, divRegua, canvasId, proficienciaMaxima, datasets)
    {
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




    $("#ddlDRE").unbind("change").change(
        function ()
        {
            var mywindow = window.open('', 'PRINT', 'height=400,width=600');

            mywindow.document.write('<html><head><title>' + document.title + '</title>');
            mywindow.document.write('</head><body >');
            mywindow.document.write('<h1>' + document.title + '</h1>');
            mywindow.document.write(document.getElementById(elem).innerHTML);
            mywindow.document.write('</body></html>');

            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10*/

            mywindow.print();
            mywindow.close();

            return true;
        }
    );

    $("#btnTermoAplicadorConcordo").unbind("click").click(
        function ()
        {
            /*
            $("#divCodigoTurma").show();
            $("#divMenuPrincipal").hide();
            $("#txtCodigoTurma").focus();
            adicionarItemBackButton("btnCodigoTurmaVoltar");
            */

            adicionarItemBackButton("btnQuestionarioSair");

            $("#divQuestionario8_TermoDeCompromisso").hide();
            $("#divMenuPrincipal").show();

            var questionarioId = 8;
            $(".page").hide(); $("#questionario-page").show();
            var tur_id = $("#txtCodigoTurma").val();
            $("#Questionario_8_Questao_1_tur_id").val(tur_id);
            $('#divQuestionario8_maisDias').hide();
            $("#divQuestionario8_exibirMaisDias").show();
            calcularPresentesEAusentes();

            selectionarQuestionario(questionarioId);

        }
    );

    $("#btnTermoAplicadorDiscordo").unbind("click").click(
        function ()
        {
            removerItemBackButton();

            $("#divQuestionario8_TermoDeCompromisso").hide();
            $("#btnCodigoTurmaVoltar").trigger("click");
        }
    );

    $("#btnCodigoTurmaProsseguir").unbind("click").click(
        function ()
        {
            var codigo = $("#txtCodigoTurma").val();

            if (!mobile)
            {
                if (window.location.href.indexOf("file:///") == -1)
                    $("#tblCadernoReserva .ui-btn").hide();
            }

            var tur_id = null;
            var codigoValido = true;
            if (!(codigo.length == 7 || codigo.length == 8))
            {
                codigoValido = false;
            }
            else if (!$.isNumeric(codigo))
            {
                codigoValido = false;
            }
            else
            {
                tur_id = parseInt(codigo.substring(0, codigo.length - 1));
                var verificador = parseInt(codigo.substring(codigo.length - 1, codigo.length));
                codigoValido = (calculaDigito(tur_id) == verificador);
            }

            if (!codigoValido)
            {
                sweetAlert("Código inválido", "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.", "error");
                return;
            }

            var arquivo = tur_id.toString();
            arquivo = arquivo.substring(arquivo.length - 1, arquivo.length);

            $.mobile.loading("show", {
                text: "Aguarde...",
                textVisible: true,
                theme: "a",
                html: ""
            });

            $.ajax({
                type: "GET",
                url: "AppProvaSP/turmas/" + arquivo + ".csv",
                dataType: "text",
                success: function (data)
                {
                    if (data.indexOf("\r\n") > 0)
                    {
                        data = data.replace(/\r\n/g, "\n");
                    }
                    var dataChamada = data.split("\n");
                    $("#divChamadaAlunos").empty();
                    var processandoTurma = false;
                    var tur_id = parseInt(codigo.substring(0, codigo.length - 1));
                    for (var i = 0; i < dataChamada.length; i++)
                    {
                        var registro = dataChamada[i].split(";");
                        if (registro[0] == tur_id)
                        {
                            processandoTurma = true;
                            var idCheckBox = "chkChamada_" + i.toString();
                            var idSpan = "spanChamada_" + i.toString();
                            $("#divChamadaAlunos").append('<label for="' + idCheckBox + '"><input id="' + idCheckBox + '" name="Questionario_8_Questao_4" class="listaPresencaAluno" onchange="marcarDesmarcarAlunoAusente(this);" type="checkbox" name="alu_matricula" value="' + registro[1] + '" /> ' + registro[2] + ' <span id="' + idSpan + '" style="color:red;display:none;">(AUSENTE)</span></label>');
                        }
                        else
                        {
                            if (processandoTurma)
                            {
                                i = dataChamada.length;
                            }
                        }
                    }

                    $.mobile.loading("hide");

                    $(".ui-page").trigger("create");

                    if (!processandoTurma)
                    {
                        sweetAlert("Código não encontrado", "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.", "error");
                    }
                    else
                    {
                        ///////////
                        $("#divQuestionario8_TermoDeCompromisso").show();
                        $("#divCodigoTurma").hide();
                    }
                },
                error: function ()
                {
                    $.mobile.loading("hide");
                    sweetAlert("Código não encontrado", "Verifique se o código informado corresponde ao apresentado na lista de presença impressa.", "error");
                }
            });


        }
    );

    $("#btnAbrirFichaAplicadorProvaOuChamada").unbind("click").click(
        function ()
        {
            $("#divCodigoTurma").show();
            $("#divMenuPrincipal").hide();
            $("#txtCodigoTurma").focus();
            adicionarItemBackButton("btnCodigoTurmaVoltar");
        }
    );

    $("#btnFichaAplicadorProvaOuChamadaVoltar").unbind("click").click(
        function ()
        {
            $("#divCodigoTurma").show();
            $("#divFichaAplicadorProvaOuChamada").hide();
            $("#txtCodigoTurma").focus();
            removerItemBackButton();
        }
    );


    $("#btnCodigoTurmaVoltar").unbind("click").click(
        function ()
        {
            $("#divCodigoTurma").hide();
            $("#divMenuPrincipal").show();
            removerItemBackButton();
        }
    );

    $("#btnSair").unbind("click").click(
        function ()
        {
            if (mobile)
            {
                window.location = "index.html";
            }
            else
            {
                parent.alertarSaidaApp();
            }
        }
    );



    $("#btnQuestionarioSair").unbind("click").click(
        function ()
        {
            swal({
                title: "Deseja realmente sair?",
                type: "warning",
                showCancelButton: true,

                confirmButtonText: "Sim",
                cancelButtonText: "Não",
                closeOnConfirm: false
            },
                function ()
                {
                    //window.location = "menu.html";
                    removerItemBackButton();
                    $(".page").hide(); $("#menu-page").show();
                    $("#divCodigoTurma").hide();
                    $("#divMenuPrincipal").show();
                    swal.close();
                });

        }
    );













    $("#btnQuestionario1_Iniciar,#btnQuestionario2_Iniciar,#btnQuestionario3_Iniciar,#btnQuestionario12_Iniciar").unbind("click").click(
        function ()
        {
            $("#divQuestionario" + questionarioId_atual + "_Intro").hide();
            $("#divQuestionario" + questionarioId_atual + "_Questoes").show();
            $("#divTituloQuestionario").show();
            $.mobile.silentScroll(0);
        }
    );

    //ALTERNATIVAS DE MÚLTIPLA ESCOLHA QUE DEVEM DESELECIONAR OUTRAS:

    $("#Questionario_2_Questao_9_A,#Questionario_2_Questao_9_B,#Questionario_2_Questao_9_C,#Questionario_2_Questao_9_D,#Questionario_2_Questao_9_E").unbind("click").click(
        function ()
        {
            $("#Questionario_2_Questao_9_F").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_2_Questao_9_F").unbind("click").click(
        function ()
        {
            $("#Questionario_2_Questao_9_A,#Questionario_2_Questao_9_B,#Questionario_2_Questao_9_C,#Questionario_2_Questao_9_D,#Questionario_2_Questao_9_E").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_9_A,#Questionario_3_Questao_9_B,#Questionario_3_Questao_9_C,#Questionario_3_Questao_9_D").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_9_E").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_9_E").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_9_A,#Questionario_3_Questao_9_B,#Questionario_3_Questao_9_C,#Questionario_3_Questao_9_D").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_14_A,#Questionario_3_Questao_14_B,#Questionario_3_Questao_14_C,#Questionario_3_Questao_14_D,#Questionario_3_Questao_14_E,#Questionario_3_Questao_14_F").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_14_G").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_14_G").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_14_A,#Questionario_3_Questao_14_B,#Questionario_3_Questao_14_C,#Questionario_3_Questao_14_D,#Questionario_3_Questao_14_E,#Questionario_3_Questao_14_F").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_15_A,#Questionario_3_Questao_15_B,#Questionario_3_Questao_15_C,#Questionario_3_Questao_15_D,#Questionario_3_Questao_15_E").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_15_F").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_15_F").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_15_A,#Questionario_3_Questao_15_B,#Questionario_3_Questao_15_C,#Questionario_3_Questao_15_D,#Questionario_3_Questao_15_E").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_19_A,#Questionario_3_Questao_19_B,#Questionario_3_Questao_19_C,#Questionario_3_Questao_19_D,#Questionario_3_Questao_19_E").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_19_F").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_19_F").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_19_A,#Questionario_3_Questao_19_B,#Questionario_3_Questao_19_C,#Questionario_3_Questao_19_D,#Questionario_3_Questao_19_E").prop('checked', false).checkboxradio('refresh');
        }
    );


    $("#Questionario_3_Questao_21_A,#Questionario_3_Questao_21_B,#Questionario_3_Questao_21_C,#Questionario_3_Questao_21_D,#Questionario_3_Questao_21_E,#Questionario_3_Questao_21_F,#Questionario_3_Questao_21_G,#Questionario_3_Questao_21_H").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_21_I").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_21_I").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_21_A,#Questionario_3_Questao_21_B,#Questionario_3_Questao_21_C,#Questionario_3_Questao_21_D,#Questionario_3_Questao_21_E,#Questionario_3_Questao_21_F,#Questionario_3_Questao_21_G,#Questionario_3_Questao_21_H").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#fieldSetQuestionario_3_Questao_21").delegate('.ui-checkbox', 'click', function (e)
    {
        //limita o número de alternativas selecionadas em 3.
        var alternativas = ["A", "B", "C", "D", "E", "F", "G", "H"];
        var quantidadeSelecao = 0;
        for (var i = 0; i < alternativas.length; i++)
        {
            if ($("#Questionario_3_Questao_21_" + alternativas[i]).prop("checked"))
            {
                quantidadeSelecao++;
            }
        }

        if (e.target.attributes["for"].value == "Questionario_3_Questao_21_I")
        {
            return;
        }

        var chk = $("#" + e.target.attributes["for"].value);
        if (!chk.prop("checked") && quantidadeSelecao >= 3)
        {
            //swal("", "Selecione no máximo três alternativas!", "error"); //swal manda a barra de rolagem para o topo.
            alert("Selecione no máximo três alternativas!");
            e.stopImmediatePropagation();
            e.preventDefault();
        }
    }
    );

    $("#Questionario_3_Questao_74_A,#Questionario_3_Questao_74_B,#Questionario_3_Questao_74_C,#Questionario_3_Questao_74_D,#Questionario_3_Questao_74_E,#Questionario_3_Questao_74_F").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_74_G").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_74_G").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_74_A,#Questionario_3_Questao_74_B,#Questionario_3_Questao_74_C,#Questionario_3_Questao_74_D,#Questionario_3_Questao_74_E,#Questionario_3_Questao_74_F").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_75_A,#Questionario_3_Questao_75_B,#Questionario_3_Questao_75_C,#Questionario_3_Questao_75_D,#Questionario_3_Questao_75_E,#Questionario_3_Questao_75_F,#Questionario_3_Questao_75_G,#Questionario_3_Questao_75_H").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_75_I").prop('checked', false).checkboxradio('refresh');
        }
    );

    $("#Questionario_3_Questao_75_I").unbind("click").click(
        function ()
        {
            $("#Questionario_3_Questao_75_A,#Questionario_3_Questao_75_B,#Questionario_3_Questao_75_C,#Questionario_3_Questao_75_D,#Questionario_3_Questao_75_E,#Questionario_3_Questao_75_F,#Questionario_3_Questao_75_G,#Questionario_3_Questao_75_H").prop('checked', false).checkboxradio('refresh');
        }
    );




    $("#Questionario_8_Questao_3_Portugues").unbind("click").click(
        function ()
        {
            resetInstrumento();
            $("#Questionario_8_QuestoesInstrumentoPortugues").show();
        }
    );

    $("#Questionario_8_Questao_3_Matematica").unbind("click").click(
        function ()
        {
            resetInstrumento();
        }
    );

    $("#Questionario_8_Questao_3_Ciencias").unbind("click").click(
        function ()
        {
            resetInstrumento();
            $("#Questionario_8_QuestoesInstrumentoCiencias").show();
        }
    );



    $("#Questionario_10_Questao_2_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_10_Questao_2_Motivos').textinput('disable');
        }
    );

    $("#Questionario_10_Questao_2_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_10_Questao_2_Motivos').textinput('enable');
        }
    );

    $("#Questionario_10_Questao_3_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_10_Questao_3_Motivos').textinput('disable');
        }
    );

    $("#Questionario_10_Questao_3_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_10_Questao_3_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_2_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_2_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_2_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_2_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_3_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_3_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_3_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_3_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_4_Satisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_4_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_4_Insatisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_4_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_5_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_5_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_5_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_5_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_6_Satisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_6_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_6_Insatisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_6_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_7_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_7_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_7_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_7_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_8_Satisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_8_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_8_Insatisfatoria").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_8_Motivos').textinput('enable');
        }
    );

    $("#Questionario_11_Questao_9_Sim").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_9_Motivos').textinput('disable');
        }
    );

    $("#Questionario_11_Questao_9_Nao").unbind("click").click(
        function ()
        {
            $('#Questionario_11_Questao_9_Motivos').textinput('enable');
        }
    );

    $("#btnAbrirRelatorioAcompanhamento").unbind("click").click(
        function ()
        {
            if (mobile)
            {
                if (navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN)
                {
                    sweetAlert("Falha de comunicação", "Para poder abrir o relatório de acompanhamento, é necessário estar conectado na internet.", "error");
                    return;
                }
            }

            window.open(urlBackEnd + "RelatorioAcompanhamento?usu_id=" + Usuario.usu_id);
        }
    );

    //RELATÓRIO DO SUPERVISOR:
    $("#btnAbrirRelatorioAcompanhamento_9").unbind("click").click(
        function ()
        {

        }
    );

    //RELATÓRIO DO DIRETOR:
    $("#btnAbrirRelatorioAcompanhamento_10").unbind("click").click(
        function ()
        {

        }
    );

    //RELATÓRIO DO COORDENADOR:
    $("#btnAbrirRelatorioAcompanhamento_11").unbind("click").click(
        function ()
        {

        }
    );

    $("#btnResultadoFechar").unbind("click").click(
        function ()
        {
            $(".page").hide();
            $("#menu-page").show();
        }
    );


}

function validarCadernosReservas()
{
    for (var i = 1; i <= 4; i++)
    {
        var caderno = 1;
        var valorCodigoBarras = $("#Questionario_8_Questao_11_CodigoBarrasCadernoReserva" + i).val();
        var codigoEolAluno = $("#Questionario_8_Questao_11_CodigoEolAlunoCadernoReserva" + i).val();

        if (valorCodigoBarras != "" || codigoEolAluno != "")
        {
            if (!validarCodigoBarrasCadernoReserva(caderno, valorCodigoBarras))
            {
                swal("Erro", "O código de barras do caderno reserva " + i + " é inválido!", "error");
                return false;
            }


            if (codigoEolAluno == "" || !$.isNumeric(codigoEolAluno))
            {
                swal("Erro", "O Código EOL informado para o caderno reserva " + i + " é inválido!", "error");
                return false;
            }
        }

    }
    return true;
}

function validarCodigoBarrasCadernoReserva(caderno, valorCodigoBarras)
{
    var valido = true;
    if (valorCodigoBarras.length != 12)
    {
        valido = false;
    }
    else
    {
        var numeroInsricao = valorCodigoBarras.substr(0, 7);
        var verificador = valorCodigoBarras.substr(7, 1);
        valido = (calculaDigito(numeroInsricao) == verificador);
    }
    return valido;
}

function enviarQuestionarioOnline()
{
    var esc_codigo = "";
    var tur_id = "";
    var Enviado = "1";
    var guid = newGuid();

    var DataPreenchimento = "";

    if (questionarioId_atual == 8) //Ficha de Registro - Aplicador(a) de Prova
    {
        //tur_id: recuperar da caixa de texto #txtCodigoTurma
        var codigo = $("#txtCodigoTurma").val();
        tur_id = parseInt(codigo.substring(0, codigo.length - 1));
    }
    else if (questionarioId_atual == 9) //Ficha de Registro - Supervisor(a) Escolar
    {
        //esc_codigo: seleção
        esc_codigo = $("#ddlEscola").val();
    }
    else
    {
        for (var i = 0; i < questionarios.length; i++)
        {
            var questionationarioID = questionarios[i].split("=")[0];
            if (questionationarioID == questionarioId_atual)
            {
                esc_codigo = questionarios[i].split("=")[1];
                i = questionarios.length;
            }
        }
    }

    //Remove os itens duplicados com valor "default".
    var respostas = $("#Questionario" + questionarioId_atual).serializeArray();
    for (var i = 0; i < respostas.length; i++)
    {
        if (i > 0)
        {
            if (respostas[i - 1].value == "default" && respostas[i - 1].name == respostas[i].name)
            {
                respostas.splice(i - 1, 1);
            }
        }
    }

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
    for (var i = 0; i < respostas.length; i++)
    {
        var numero = respostas[i].name.replace("Questionario_" + questionarioId_atual + "_Questao_", "");
        var valor = respostas[i].value;
        var qri = new QuestionarioRespostaItem(numero, valor);
        respostasArray.push(qri);
    }

    qu.Respostas = respostasArray;
    listaQuestionariosNaoEnviados = new Array();
    listaQuestionariosNaoEnviados.push(qu);
    var jsonListaQuestionariosNaoEnviados = JSON.stringify(listaQuestionariosNaoEnviados);

    $.post(urlBackEnd + "api/SincronizarQuestionario", { json: jsonListaQuestionariosNaoEnviados })
        .done(function (data)
        {
            swal("Obrigado!", "As informações foram enviadas com sucesso!", "success");
            $(".page").hide();
            $("#menu-page").show();
            $.mobile.loading("hide");
        })
        .fail(function (erro)
        {
            swal("Erro " + erro.status, erro.statusText, "error");
            $.mobile.loading("hide");
        });
}

function calculaDigito(dado)
{
    return calculaDigitoMod11(dado.toString(), 1, 9, true);
}

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
function calculaDigitoMod11(dado, numDig, limMult, x10)
{

    var mult, soma, i, n, dig;

    if (!x10) numDig = 1;
    for (n = 1; n <= numDig; n++)
    {
        soma = 0;
        mult = 2;
        for (i = dado.length - 1; i >= 0; i--)
        {
            soma += (mult * parseInt(dado.charAt(i)));
            if (++mult > limMult) mult = 2;
        }
        if (x10)
        {
            dig = ((soma * 10) % 11) % 10;
        } else
        {
            dig = soma % 11;
            if (dig == 10) dig = "X";
        }
        dado += (dig);
    }
    return dado.substr(dado.length - numDig, numDig);
}

function btnConcluirQuestionario()
{
    if (questionarioId_atual == 8) //Ficha de Registro - Aplicador(a) de Prova
    {
        //Valida o(s) código(s) de barras informado(s), e o(s) código(s) EOL do(s) caderno(s) reserva(s)
        if (!validarCadernosReservas())
        {
            return;
        }

        if (!($("#Questionario_8_Questao_3_Portugues").attr('checked') == "checked" || $("#Questionario_8_Questao_3_Matematica").attr('checked') == "checked" || $("#Questionario_8_Questao_3_Ciencias").attr('checked') == "checked"))
        {
            sweetAlert("Disciplina não informada", "Por gentileza selecione a disciplina antes de concluir.", "error");
            return;
        }
    }

    if (questionarioId_atual == 9) //Ficha de Registro - Aplicador(a) de Prova
    {
        if ($("#ddlEscola").val() == "")
        {
            sweetAlert("Escola não informada", "Por gentileza selecione a Escola antes de concluir.", "error");
            return;
        }
    }

    $.mobile.loading("show", {
        text: "Aguarde...",
        textVisible: true,
        theme: "a",
        html: ""
    });

    if (mobile)
    {
        salvarQuestionarioLocal();
    }
    else
    {
        enviarQuestionarioOnline();
    }
}
