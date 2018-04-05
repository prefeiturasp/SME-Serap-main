REM *** PRIMEIRA PARTE: GERAR O ARQUIVO loginOffline.json QUE É EMBARCADO COM O APP POR MEIO DA APLICAÇÃO ProvaSP.Console.exe
..\ProvaSP.Console\bin\Release\ProvaSP.Console.exe

REM *** SEGUNDA PARTE: ATUALIZAR O SERAP COM BASE NO CÓDIGO CORDOVA
@ECHO ATUALIZANDO SERAP...

@ECHO Copiando os arquivos .js e .css do app Cordova para o SERAp:
xcopy /i /E /Y www\AppProvaSP ..\GestaoAvaliacao\AppProvaSP

@ECHO Atualizando a View IndexApp.cshtml do SERAp com base no arquivo app.html do Cordova:
xcopy /i /Y www\app.html ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml

@ECHO Corrigindo o caminho relativo "AppProvaSP/" (Cordova) para "/AppProvaSP/" (SERAp) na View IndexApp.cshtml:
powershell -Command "(gc ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml) -replace 'AppProvaSP/', '/AppProvaSP/' | Out-File ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml"

@ECHO Substituindo "guid_gerado_por_script" para um Guid de fato na View IndexApp.cshtml:
powershell -Command "(gc ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml) -replace 'guid_gerado_por_script', [guid]::NewGuid() | Out-File ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml"


@ECHO Removendo comentários de chamadas para Viewbags da View IndexApp.cshtml (SERAp)
powershell -Command "(gc ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml) -replace '//@Html', '@Html' | Out-File ..\GestaoAvaliacao\Views\ProvaSP\IndexApp.cshtml"

@ECHO Corrigindo o caminho relativo "AppProvaSP/" (Cordova) para "/AppProvaSP/" (SERAp) no arquivo app.js:
powershell -Command "(gc ..\GestaoAvaliacao\AppProvaSP\scripts\app.js) -replace 'AppProvaSP/', '/AppProvaSP/' | Out-File ..\GestaoAvaliacao\AppProvaSP\scripts\app.js"


@ECHO SERAP ATUALIZADO!
