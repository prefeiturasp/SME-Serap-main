/**
-----MSTECH-----
 *Construtor do Objeto Usuário.

 *usu_id é o GUID do usuário

 *usu_username é o login

 *usu_senha é gerada no ProvaSP.Console. Por sua vez o ProvaSP.Console chama um método
DataUsuario.RetornarBaseUsuarioOffline(); que gera o loginOffline.json com as informações de
usuários salvas Offline. Importante ressaltar que a regra de senhas para o loginOffline foi alterada.

 *Senha Online != Senha Offline???
 *Esta era a estrutura de usuários antiga, da versão publicada na GooglePlay
*/
var User = function (usu_id, username, password, grupos /*questionarios*/) {
    this.usu_id = usu_id;
    this.username = username;
    this.password = password;
    /**
    -----MSTECH-----
     *questionario era um parâmetro da versão antiga (publicada na Play Store e analisada
     por engenharia reversa). Foi substituído por "Grupos"
    */
    //this.questionarios = questionarios;
    this.grupos = grupos;
};
/**
-----MSTECH-----
 *Aparentemente existe um erro de lógica aqui, tendo em vista que uma mesma var Usuario é definida
 no script global.js.

 *Não utilizaremos mais esta atribuição, tendo em vista que o construtor User é obsoleto.
*/
//var Usuario = new User();
/**
-----MSTECH-----
 *Variável global auxiliar para ação de login. Inicialmente estava posicionado junto ao módulo de Login,
 o que não é necessário.
*/
var dataLogin = null;

/**
-----MSTECH-----
 *Opções de configuração do Loading JQuery Mobile

 *Referência em: https://api.jquerymobile.com/loader/
*/
$(document).on("mobileinit", function () {
    $.mobile.loader.prototype.options.text = "loading";
    $.mobile.loader.prototype.options.textVisible = false;
    $.mobile.loader.prototype.options.theme = "a";
    $.mobile.loader.prototype.options.html = "";
});

// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in cordova-simulate or on Android devices/emulators: launch your app, set breakpoints,
// and then run "window.location.reload()" in the JavaScript Console.
(function () {
    "use strict";

    //https://stackoverflow.com/questions/8068052/phonegap-detect-if-running-on-desktop-browser
    //if (navigator.userAgent.match(/(iPhone|iPod|iPad|Android|BlackBerry|IEMobile)/) || typeof cordova != "undefined") {

    /**
    -----MSTECH-----
     *Verificando se o App está rodando em um dispositivo Mobile com o cordova ou em um browser Web

     *Esta validação é necessária pois a mesma solução deve funcionar em Mobile App e Web

     *Isso é demonstrado pelo script que transfere o código Cordova para o projeto GestaoAvaliacao
    */
    if (typeof cordova !== "undefined") {
        // PhoneGap/Cordova application
        document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    }
    else {
        // Web page
        $(document).ready(function () {
            // Handler for .ready() called.
            onDeviceReady(); //this is the browser
        });
    }

    /**
    -----MSTECH-----
     *Iniciando o App de fato, já sabendo se é mobile ou web
    */
    function onDeviceReady() {
        try {
            /**
            -----MSTECH-----
             *Bind nos eventos de PAUSE e RESUME

             *Tais eventos são padrão para os dispositivo mobile utilizando Android ou iOS, por exemplo.

             *Pause: Quando seleciona botão Home, minimiza ou fecha o App;

             *Resume: Quando volta ao App (com o mesmo ainda aberto)

             *Resumidamente, este código muito provavelmente é um código de Template, pois não existe ação
             nos eventos pause e resume
            */
            document.addEventListener('pause', onPause.bind(this), false);
            document.addEventListener('resume', onResume.bind(this), false);

            /**
            -----MSTECH-----
             *Evento do botão Login da tela inicial

             *Reparar também que o botão ENTER do teclado do dispositivo também recebeu BIND para
             agir da mesma maneira que o botão Login da UI.
            */
            $("#btnLoginEntrar").click(btnLoginEntrar_click);
            $("#Username,#PasswordInput").on('keypress', function (e) {
                if (e.which === 13) { //KEY para ENTER
                    btnLoginEntrar_click();
                }
            });
        }
        catch (error) { console.log(error); }
    };

    /**
    -----MSTECH-----
     *Evento pause não tem ação específica
    */
    function onPause() { };

    /**
    -----MSTECH-----
     *Evento resume não tem ação específica
    */
    function onResume() { };
})();

/**
-----MSTECH-----
 *Depois do módulo inicial para inicialização correta do Cordova, seguem métodos responsáveis pelo
 Login do ProvaSP;

 *Temos basicamente um evento btnLoginEntrar_click bindado no botão "Logar" ou no botão "ENTER" do
 teclado do dispositivo.

 *Tal evento possui alguns eventos auxiliares para conclusão de sua lógica.
*/
function btnLoginEntrar_click() {
    try {
        /**
        -----MSTECH-----
         *Variável que identifica a presença de conexão com a internet
        */
        var validarLoginOnline = false;

        /**
        -----MSTECH-----
         *Mostrando estrutura de Loading do JQuery mobile;

         *Esta estrutura personalizada sobrescreve a estrutura inicial referenciada na inicialização do App
        */
        $.mobile.loading("show", {
            text: "Validando login, aguarde...",
            textVisible: true,
            theme: "a",
            html: ""
        });

        /**
        -----MSTECH-----
         *Verificando conexão do dispositivo. Se não houver conexão com a internet, haverá consulta
         ao arquivo loginOffline.json
        */
        if (navigator.connection != null) {
            if (!(navigator.connection.type == Connection.NONE || navigator.connection.type == Connection.UNKNOWN)) {
                validarLoginOnline = true;
            }
        }

        /**
        -----MSTECH-----
         *O trecho a seguir corresponde ao Login ONLINE;

         *Este tipo de login dá-se pela chamada da API api/Login apontando para o ambiente do ProvaSP

         *http://provasp.sme.prefeitura.sp.gov.br/

         *OBS: O ambiente existe - Faremos chamadas para identificar o comportamento
        */
        if (validarLoginOnline) {
            /**
            -----MSTECH-----
             *O campo de password retornada o DOM completo. Sendo assim, editamos para que receba o
             valor do campo correspondente.

             *Outra observação é que o Password não possui chamada explícita ao elemento do DOM, sendo
             reconhecido apenas pela correspondência com o ID do elemento no HTML, no caso "Password".
             Sendo assim, para evitar possibilidade de conflitos, renomeamos o id e referenciamos o element
             DOM corretamente.
            */
            var login = ($("#Username").val()).toUpperCase();
            //var senha = Password;
            //var senha = $("#PasswordInput").val(); //MSTECH - Tornando possível login

            /**
            -----MSTECH-----
             *Fazendo a engenharia reversa do APK publicado na Google Play Store, desobrimos que o
             Password deverá ser criptografado em SHA512
            */
            var senha = sha512.hex(strEncodeUTF16($("#PasswordInput").val()));
            $.post(urlBackEnd + "api/Login", { usu_login: login, usu_senha: senha })
                .done(function (jsonUsuario) {
                    /**
                    -----MSTECH-----
                     *Adicionada validação do objeto raiz.

                     *Foi feito pois, ao informar um login e senha incorretos, o server retorna null

                     *Sem tal validação o App para de funcionar
                    */
                    if (jsonUsuario == null || jsonUsuario.usu_id == "" || jsonUsuario.usu_id == null) {
                        $.mobile.loading("hide");
                        ProvaSP_Erro("Alerta", "Usuário ou senha inválido(s)");
                    }
                    else {
                        loginSucesso(jsonUsuario);
                    }
                })
                .fail(function (xhr, status, error) {
                    $.mobile.loading("hide");
                    ProvaSP_Erro("Falha de comunicação", "Não foi possível encontrar sua conta. " +
                        "Erro: (" + status + ") " + error);
                });
            return;
        }
        else {
            /**
            -----MSTECH-----
             *Chamada para loginOffline utilizando o arquivo loginOffline.json
            */
            validarLoginOffline();
        }
    }
    catch (error) {
        /**
        -----MSTECH-----
         *Não havia tratamento algum de erro. Adicionamos portanto uma validação para evitar que o App pare de funcionar
         em caso de problemas no Login.
        */
        $.mobile.loading("hide");
        ProvaSP_Erro("Alerta", "Houve um problema na validação da credencial. Código do erro (" +
            status + ") " + error);
    }
}

/**
-----MSTECH-----
 *Método possui dois fluxos:

 1- Abrir arquivo loginOffline.json e armazenar o conteúdo do arquivo na variável global dataLogin
 2- Usar o conteúdo de dataLogin para validar o login e senha do usuário Offline
*/
function validarLoginOffline() {
    try {
        /**
        -----MSTECH-----
         *Se a variável global dataLogin é vazia, abrir arquivo loginOffline.json
        */
        if (dataLogin == null) {
            $.ajax({
                type: "GET",
                url: "loginOffline.json",
                dataType: "JSON",
                async: true,
                success: function (data) {
                    /**
                    -----MSTECH-----
                     *Após obter as informações do arquivo loginOffline.json e armazena-las em dataLogin,
                     devemos, através de recursividade, chamar novamente este método para executar a
                     contrapartida.
                    */
                    dataLogin = data;
                    validarLoginOffline();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $.mobile.loading("hide");
                    ProvaSP_Erro("Alerta", xhr.status);
                }
            });
        }
        else {
            /**
            -----MSTECH-----
             *Trecho importantíssimo;

             *O Login do usuário é transformado num valor HEXADECIMAL para comparação com a informação
             no arquivo loginOffline.json.

             *Como loginOffline.json é gerado pelo projeto ProvaSP.Console, que por sua vez instancia o projeto
             ProvaSP.Data, o trecho que gera o login em hexa é:

                int usu_login_int;
                if (Int32.TryParse(usuario.usu_login, out usu_login_int))
                {
                    usuario.usu_login = usu_login_int.ToString("X");
                }
                usuario.usu_senha = usu_senha.Substring(0,4); //Redução do tamanho do HASH

             *Perceber que o trecho usu_login_int.ToString("X"); é justamente o responsável pela conversão
            */
            var Username = parseInt($("#Username").val()).toString(16).toUpperCase();
            /**
            -----MSTECH-----
             *O mesmo é feito com a senha do usuário.

             *Muito provavelmente como a equipe não tinha acesso às credenciais, obtiveram um meio de tratar
             o código criptografado das senhas para armazenar Offline apenas os 4 últimos dígitos da senha tratada.
             Assim se o usuário logar com uma senha válida, a comparação será feita com o HASH de 4 dígitos finais
             gerados pelo ProvaSP.Console com a mesma validação sobre a senha que a pessoa digitar. Se os valores
             forem correspondentes, o login será permitido.

             *Analisando melhor, aparentemente a senha não é utilizada para NADA, sendo o login
             determinado pelo Username
            */
            var Password_encode = sha512.hex(strEncodeUTF16($("#PasswordInput").val()));
            var Password_compare = Password_encode.substring(0, 4);
            var l = dataLogin.length;

            /**
            -----MSTECH-----
             *Se o login digitado convertido em Hexa tiver alguma correspondência no arquivo loginOffline
             *permite o Login - Lembrando que o Password não é validado.
            */
            var indice = buscaBinaria(dataLogin, Username);
            if (indice >= 0) {
                /**
                -----MSTECH-----
                 *Implementação correta com base na engenharia reversa do Aplicativo publicado na
                 Google Play Store

                 OBS: A segunda verificação é uma forma de logar sem precisar necessariamente saber a senha
                 usada apenas para testes
                */
                var currentUser = dataLogin[indice];

                if ((Username == currentUser.usu_login && Password_compare == currentUser.usu_senha)
                    || ($("#Username").val().substr($("#Username").val().length - 4, 4) == $("#PasswordInput").val())
                ) {
                    loginSucesso(currentUser);
                    return;
                }
                else {
                    $.mobile.loading("hide");
                    ProvaSP_Erro("Alerta", "Não foi possível encontrar sua conta no modo offline." +
                        " Se possível, conecte-se à internet e tente novamente.");
                }
            }
            else {
                $.mobile.loading("hide");
                ProvaSP_Erro("Alerta", "Não foi possível encontrar sua conta no modo offline." +
                    " Se possível, conecte-se à internet e tente novamente.");
            }
        }
    }
    catch (error) { console.log(error); }
}

/**
-----MSTECH-----
 *Este método busca no objeto obtido do arquivo loginOffline.json uma equivalência do username
 convertido em hexadecimal. Se hovuer correspondência, retorna o índice do objeto no vetor de usuários
 Offline. Assim é possível obter o objeto de login do usuário escolhido
*/
function buscaBinaria(ar, busca) {
    try {
        var topo = 0;
        var base = ar.length - 1;
        var retorno = -1;
        var tentativas = 0;
        while (topo != base) {
            var indice = parseInt((topo + base) / 2);
            var compara = ar[indice].usu_login;
            if (compara == busca) {
                return indice;
            }
            else if (compara < busca) {
                topo = indice;
            }
            else {
                base = indice;
            }

            tentativas++;
            if (tentativas >= 100)
                return -1;
        }
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Se um objeto de usuário válido for obtido, abre o App de fato. Para tanto, carrega-se a página app.html/app.js
 com base no usuário obtido. O objeto do usuário logado é salvo no LocalStorage do App.

 *Convertemos novamente o usu_login para o formato decimal, correspondente ao registro EOL do aluno.
*/
function loginSucesso(jsonUsuario) {
    try {
        Usuario = jsonUsuario;
        localStorage.setItem("Usuario", JSON.stringify(Usuario));
        window.location = "app.html";
    }
    catch (error) {
        console.log(error);
    }
}

/**
-----MSTECH-----
 *Método para tratar o hash da senha. Observa-se que o método é irrelevante
 por não haver validação do Login Offline.
*/
function strEncodeUTF16(str) {
    try {
        var buffer = new Array(str.length * 2);
        var index = 0;
        for (var i = 0; i < buffer.length; i++) {
            if (i % 2 == 0) {
                buffer[i] = str.charCodeAt(index);
                index++;
            }
            else
                buffer[i] = 0;
        }
        return buffer;
    }
    catch (error) {
        console.log(error);
        return "";
    }
}