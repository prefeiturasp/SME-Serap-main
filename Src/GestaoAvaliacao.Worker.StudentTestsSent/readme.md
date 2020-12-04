# GestaoAvaliacao.Worker.StudentTestsSent
Este projeto visa prover o processamento das provas realizadas no SERAP. Este processamento consiste na coleta das respostas dos alunos e na geração de métricas em função dos percentuais de acerto nos níveis de **TURMA, ESCOLA, DRE e SME**.
O projeto compõe um HostedService com execução agendada utilizando a library NCronTab

------------

## Configuração de ambiente
Configuração inicial de ambiente de desenvolvimento

### Sentry
Para reporte de erros e validações de negócio utilizando o [Sentry](https://sentry.io/welcome/).
Em ambiente local basta criar uma conta gratuita e criar uma aplicação do tipo .NET, assim a plataforma irá fornecer a chave para utilização no Worker.
Para os demais ambientes, a chave deverá ser obtida através das plataforams de comunicação do aplicativo.

### Utilizando Kestrel

#### Variáveis de ambiente
> Todas as variáveis de configuração são mantidas como variáveis de ambiente. 

**Modificar o arquivo /Environments/create-environment-variables.ps1 com a chave do Sentry obtida, a instância SQL do banco GestaoAvaliacao e a instância   MongoDB do GestaoAvaliacao_OMR.

1- Procurar por **POWERSHELL** no inciar. 

2- Clicar com botão direito e executar como administrador

3- Executar o comando: `Set-ExecutionPolicy Unrestricted`

4- Digitar s ou y (dependendo do idioma do sistema operacional)

	** O comando acima é para habilitar scripts não criados na maquina local e sem certificado a serem executados;
	
5- Navegar até a pasta do projeto `Environments` pelo powershell

6- Executar o comando `& .\create-environment-variables.ps1`

7- Executar o comando: `Set-ExecutionPolicy Restricted`

8- Digitar s ou y (dependendo do idioma do sistema operacional)

	** O comando acima é para desabilitar scripts de serem executados;

### Utilizando Docker

#### Variáveis de ambiente
> As variáveis de ambiente são declaradas no arquivo `sme-serap-worker.env` localizado na pasta da solução.

**Modificar o arquivo sme-serap-worker.env com a chave do Sentry obtida, a instância SQL do banco GestaoAvaliacao e a instância   MongoDB do GestaoAvaliacao_OMR.

#### Executanto

1- Procurar por **POWERSHELL** no inciar. 

2- Navegar até a pasta da solução.

3- Executar o comando:
```bash
	docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build -d
```

#### Depuração

1- Localizar o docker-compose na solução.

2- Clique com botão direito e selecione `Set as Startup Project`.

3- Execute. 


