var User = function (usu_id, username, password, grupos /*questionarios*/)
{
    this.usu_id = usu_id;
    this.username = username;
    this.password = password;
    //this.questionarios = questionarios;
    this.grupos = grupos;
};

var Usuario = new User();

$(document).on("mobileinit", function ()
{
    $.mobile.loader.prototype.options.text = "loading";
    $.mobile.loader.prototype.options.textVisible = false;
    $.mobile.loader.prototype.options.theme = "a";
    $.mobile.loader.prototype.options.html = "";
});

// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in cordova-simulate or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.
(function ()
{
    "use strict";

    //https://stackoverflow.com/questions/8068052/phonegap-detect-if-running-on-desktop-browser
    //if (navigator.userAgent.match(/(iPhone|iPod|iPad|Android|BlackBerry|IEMobile)/) || typeof cordova != "undefined") {

    if (typeof cordova !== "undefined")
    {
        // PhoneGap application
        document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    } else
    {
        // Web page
        $(document).ready(function ()
        {
            // Handler for .ready() called.
            onDeviceReady(); //this is the browser
        });
    }

    function onDeviceReady()
    {


        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);

        $("#btnLoginEntrar").click(btnLoginEntrar_click);
        $("#Username,#Password").on('keypress', function (e)
        {
            if (e.which === 13)
            {
                btnLoginEntrar_click();
            }
        });


    };

    function onPause()
    {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume()
    {
        // TODO: This application has been reactivated. Restore application state here.
    };


})();








var dataLogin = null;

function btnLoginEntrar_click()
{
    $.mobile.loading("show", {
        text: "Validando login, aguarde...",
        textVisible: true,
        theme: "a",
        html: ""
    });
    
    var validarLoginOnline = false;

    if (navigator.connection != null)
    {
        if (!(navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN))
        {
            validarLoginOnline = true;
        }
    }

    if (validarLoginOnline)
    {
        var login = $("#Username").val();
        var senha = Password;
        $.post(urlBackEnd + "api/Login", { usu_login: login, usu_senha: senha })
            .done(function (jsonUsuario)
            {
                if (jsonUsuario.usu_id == "" || jsonUsuario.usu_id == null)
                {
                    $.mobile.loading("hide");
                    sweetAlert("Usuário ou senha inválido(s)", "", "error");
                }
                else
                {
                    loginSucesso(jsonUsuario);
                }
            })
            .fail(function (xhr, status, error)
            {
                $.mobile.loading("hide");
                sweetAlert("Falha de comunicação", "Não foi possível encontrar sua conta no modo offline. Foi feita uma tentativa de login online, mas resultou em erro (" + status + ") " + error, "error");
            });
        return;
    }
    else
    {
        validarLoginOffline();
    }

}

function validarLoginOffline()
{
    if (dataLogin == null)
    {
        $.ajax({
            type: "GET",
            url: "loginOffline.json",
            dataType: "JSON",
            async: true,
            success: function (data)
            {
                dataLogin = data;
                validarLoginOffline();
            },
            error: function (xhr, ajaxOptions, thrownError)
            {
                $.mobile.loading("hide");
                alert(xhr.status);
            }
        });
    }
    else
    {
        var Username = parseInt($("#Username").val()).toString(16).toUpperCase();
        var Password = sha512.hex(strEncodeUTF16($("#Password").val()));
        var Password_compare = Password.substring(0, 4);
        var l = dataLogin.length;

        var indice = buscaBinaria(dataLogin, Username);
        if (indice >= 0)
        {
            loginSucesso(dataLogin[indice]);
        }
        else
        {
            $.mobile.loading("hide");
            sweetAlert("Não foi possível encontrar sua conta no modo offline. Se possível, conecte-se à internet e tente novamente.", "", "error");
        }
    }
}

function buscaBinaria(ar, busca)
{
    var topo = 0;
    var base = ar.length - 1;
    var retorno = -1;
    var tentativas = 0;
    while (topo != base)
    {
        var indice = parseInt((topo + base) / 2);
        var compara = ar[indice].usu_login;
        if (compara == busca)
        {
            return indice;
        }
        else if (compara < busca)
        {
            topo = indice;
        }
        else
        {
            base = indice;
        }

        tentativas++;
        if (tentativas >= 100)
            return -1;
    }
}

function loginSucesso(jsonUsuario)
{
    Usuario = jsonUsuario;
    localStorage.setItem("Usuario", JSON.stringify(Usuario));
    window.location = "app.html";
}

function strEncodeUTF16(str)
{
    var buffer = new Array(str.length * 2);
    var index = 0;
    for (var i = 0; i < buffer.length; i++)
    {
        if (i % 2 == 0)
        {
            buffer[i] = str.charCodeAt(index);
            index++;
        }
        else
            buffer[i] = 0;
    }
    return buffer;
}