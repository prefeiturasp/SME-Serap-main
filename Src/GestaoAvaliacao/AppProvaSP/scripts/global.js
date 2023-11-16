//var urlBackEnd = "http://provasp-smesp.mstech.com.br/";
//var urlBackEnd = "http://provasp.sme.prefeitura.sp.gov.br/";
//var urlBackEnd = "http://provasp.sme.prefeitura.sp.gov.br/";
var urlBackEnd = "http://localhost:52912/";

var Usuario = null;

/**
-----MSTECH-----
 *Méodo para sobrescrever o BackButton do Android.
 *Foi adicionado para melhorar consideravelmente a experiência do usuário. Desta maneira o App não
 é fechado inesperadamente e alertas também são encerrados com o backButton
*/
document.addEventListener('backbutton', function () {
    try {
        //swal.close();
        if (window.location.href.indexOf("app") != -1) {
            voltarCaminhoBackButton();
        }
        else { ProvaSP_CloseApp(); }
    }
    catch (error) { navigator.app.exitApp(); }
}, false);

function ProvaSP_CloseApp () {
    try {
        navigator.notification.confirm(
            "Deseja realmente fechar o ProvaSP?",
            function (buttonIndex) {
                if (buttonIndex == 2) { navigator.app.exitApp(); }
            },
            "Encerrando...",
            ["Ainda não", "Sim"]
        );
    }
    catch (error) { console.log(error); }
}

/**
-----MSTECH-----
 *Método obsoleto utilizado em versão anterior (publicada na loja) do App. Para a conclusão
 da versão Web, simplificaremos o método no app.js.
 *Manteremos para fins de registro.
*/
function ConfigurarUsuarioSerap(grupoSerap, jsonUsuario) {
    Usuario = jsonUsuario;
}

/**
-----MSTECH-----
 *Melhorando tratamento de erro na versão Mobile para evitar que o popup de mensagem ocupe toda a tela,
 impedindo a interação com a interface de usuário.
 *Na versão Web do App, o popup continua sendo o sweetAlert
*/
function ProvaSP_Erro(title, msg) {
    try {
        $.mobile.loading("hide");

        var isMobile = false;
        if (typeof cordova !== "undefined") { isMobile = true; }

        if (isMobile) {
            navigator.notification.alert(
                msg,
                function () { }, title, "OK"
            );
        }
        else { sweetAlert(title, msg, "error"); }
    }
    catch (error) { console.log(error); }
}

function ProvaSP_Alerta(title, msg) {
    try {
        $.mobile.loading("hide");

        var isMobile = false;
        if (typeof cordova !== "undefined") { isMobile = true; }

        if (isMobile) {
            navigator.notification.alert(
                msg,
                function () { }, title, "OK"
            );
        }
        else { sweetAlert(title, msg, "warning"); }
    }
    catch (error) { console.log(error); }
}