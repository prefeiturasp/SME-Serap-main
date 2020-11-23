# GestaoAvaliacaoTesteRPA

Robot para automação dos testes das histórias e validação automática dos critérios de aceite.


## Preparação do ambiente

Para preparar o ambiente, pode seguir as instruções de instalação [aqui](https://github.com/robotframework/robotframework/blob/master/INSTALL.rst). 


## Organização do repositório

Para cada página/componente, deve-se criar ao menos 1 arquivo com a extensão `.lib.robot` que é a library de comandos possiveis naquele escopo. 
Os testes são escritos com a extensão `.test.robot` para sinalizar que é um arquivo de cenários de teste. O nome dos arquivos devem conter o nome da pagina/componente, exemplo: `Login.[js,aspx]` possui como teste `login.test.robot` e como biblioteca de comandos `login.lib.robot`.
O arquivo `src/configuracao/variaveis.robot` possui as variáveis globais necessárias para a execução dos testes no ambiente local de desenvolvimento, caso precisa rodar em outro ambiente deve passar as variaveis por linha de comando como descrito neste [link](https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#variable-files).


## Executando os testes

Para executar um teste específico basta rodar o comando `robot` apontando para o script de teste específico, exemplo com o Login:

```
robot \
    --variable BROWSER:Chrome \ 
    --variable SERVER:serap.sme.prefeitura.sp.gov.br \ 
    --variable SME_USER:RA0000000 \ 
    --variable SME_PASS:0000 \ 
    --outputdir ./reports \ 
    --timestampoutputs \ 
    src/serap.test.robot
```

Também é possível, e recomendado, utilizar o Docker para executar os testes. A imagem Docker permite que se escolha o browser a ser utilizado tendo como opções Chrome e Firefox. Também é necessário passar a variável `SERVER` com URL e porta do ambiente a ser executado os testes. Exemplo de chamada via Docker:

```
docker run \ 
    -v ./reports:/opt/robotframework/reports:Z \ 
    -v ./src:/opt/robotframework/tests:Z \ 
    -e BROWSER=Chrome \ 
    -e SERVER=serap.sme.prefeitura.sp.gov.br \ 
    -e SME_USER=RA0000000 \ 
    -e SME_PASS=0000 \ 
    ppodgorsek/robot-framework:latest 
```
