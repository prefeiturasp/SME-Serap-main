USE CoreSSO

GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO 

DECLARE @sis_id INT
SET @sis_id = 204

DELETE FROM SYS_VisaoModuloMenu WHERE sis_id = @sis_id
DELETE FROM SYS_VisaoModulo WHERE sis_id = @sis_id
DELETE FROM SYS_GrupoPermissao WHERE sis_id = @sis_id
DELETE FROM SYS_Modulo WHERE sis_id = @sis_id
DELETE FROM SYS_ModuloSiteMap WHERE sis_id = @sis_id


DECLARE @nomeSistema VARCHAR(MAX) = 'SERAp' 
DECLARE @nomeModuloAvo VARCHAR(MAX) 
DECLARE @nomeModuloPai VARCHAR(MAX) 
DECLARE @nomeModulo VARCHAR(MAX) 
 
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Cadastros'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Cadastros',  
  @SiteMap1Url = 'Cadastros',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo  
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Componente curricular'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar componentes curriculares',  
  @SiteMap1Url = '/Discipline/List',
  @SiteMap2Nome = 'Cadastrar componente curricular',  
  @SiteMap2Url = '/Discipline/Form',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Nível de dificuldade do item'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar nível de dificuldade do item',  
  @SiteMap1Url = '/ItemLevel/List',
  @SiteMap2Nome = 'Cadastrar níveis de dificuldade do item',  
  @SiteMap2Url = '/ItemLevel/Form',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Tipo do item'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar tipos dos itens',  
  @SiteMap1Url = '/ItemType/Index',
  @SiteMap2Nome = 'Cadastrar tipo do item',  
  @SiteMap2Url = '/ItemType/IndexForm',
  @SiteMap3Nome = 'Consultar tipos dos itens', 
  @SiteMap3Url = '/ItemType', 
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Competência cognitiva'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar competências cognitivas',  
  @SiteMap1Url = '/CognitiveCompetence/List', 
  @SiteMap2Nome = 'Cadastrar competência cognitiva',  
  @SiteMap2Url = '/CognitiveCompetence/Form',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Modelo de matriz de avaliação'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,
  @SiteMap1Nome = 'Consultar modelos de matriz de avaliação',  
  @SiteMap1Url = '/ModelEvaluationMatrix/Index',  
  @SiteMap2Nome = 'Cadastrar modelo de matriz de avaliação',  
  @SiteMap2Url = '/ModelEvaluationMatrix/IndexForm',
  @SiteMap3Nome = 'Consultar modelos de matriz de avaliação', 
  @SiteMap3Url = '/ModelEvaluationMatrix', 
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Matriz de avaliação'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar matrizes de avaliação',  
  @SiteMap1Url = '/EvaluationMatrix/Index',
  @SiteMap2Nome = 'Cadastrar matriz de avaliação',  
  @SiteMap2Url = '/EvaluationMatrix/IndexForm',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo
  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Correlação de habilidades'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Correlação de habilidades',  
  @SiteMap1Url = '/CorrelatedSkill/Form',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Tipo de prova'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,
  @SiteMap1Nome = 'Consultar tipos de prova',  
  @SiteMap1Url = '/TestType/List',  
  @SiteMap2Nome = 'Cadastrar tipo de prova',  
  @SiteMap2Url = '/TestType/Form',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Modelo de prova'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,
  @SiteMap1Nome = 'Consultar modelos de provas',  
  @SiteMap1Url = '/ModelTest/Index',  
  @SiteMap2Nome = 'Cadastrar modelo de prova',  
  @SiteMap2Url = '/ModelTest/IndexForm',
  @SiteMap3Nome = 'Consultar modelos de provas', 
  @SiteMap3Url = '/ModelTest', 
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo
  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Nível de desempenho'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,
  @SiteMap1Nome = 'Consultar nível de desempenho',  
  @SiteMap1Url = '/PerformanceLevel/List',  
  @SiteMap2Nome = 'Cadastrar níveis de desempenho',  
  @SiteMap2Url = '/PerformanceLevel/Form',
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo
  
                  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Motivo de ausência'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar motivos de ausência',  
  @SiteMap1Url = '/AbsenceReason/List',
  @SiteMap2Nome = 'Cadastrar motivo de ausência',  
  @SiteMap2Url = '/AbsenceReason/Form', 
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Cadastros'
SET @nomeModulo = 'Grupo de prova'

EXEC MS_InserePaginaMenu
	@nomeSistema = @nomeSistema 
	,@nomeModuloPai = @nomeModuloPai
	,@nomeModulo = @nomeModulo
	,@SiteMap1Nome = 'Consultar grupos de prova'
	,@SiteMap1Url = '/TestGroup/List'
	,@SiteMap2Nome = 'Cadastrar grupos de prova' 
	,@SiteMap2Url = '/TestGroup/Form'						
	,@possuiVisaoAdm = 1
	,@possuiVisaoGestao = 1 
	,@possuiVisaoUA = 0
	,@possuiVisaoIndividual = 0 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Itens'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Itens',  
  @SiteMap1Url = 'Itens',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Itens'
SET @nomeModulo = 'Cadastrar item'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Cadastrar item',  
  @SiteMap1Url = '/Item/Form',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
  
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Itens'
SET @nomeModulo = 'Banco de itens'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Banco de itens',  
  @SiteMap1Url = '/Item/List',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Provas'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Provas',  
  @SiteMap1Url = 'Provas',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 1,  
  @possuiVisaoIndividual = 1,  
  @nomeModuloAvo = @nomeModuloAvo 
 
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Cadastrar prova'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Cadastrar prova',  
  @SiteMap1Url = '/Test/IndexForm',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Consultar provas'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Consultar provas',  
  @SiteMap1Url = '/Test',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 1,  
  @possuiVisaoIndividual = 1,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Gerar folha de resposta'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Gerar folha de resposta',  
  @SiteMap1Url = '/AnswerSheet/AnswerSheetLot',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 1,  
  @possuiVisaoIndividual = 1,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Solicitações de anulação de itens'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Solicitações de anulação de itens',  
  @SiteMap1Url = '/Test/IndexRequestRevoke',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo  

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Enviar folhas de respostas'


EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Enviar folhas de respostas', 
		@SiteMap1Url = '/AnswerSheet/IndexBatchDetails', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 1, 
		@nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Provas'
SET @nomeModulo = 'Exportação dos resultados'


EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Exportação dos resultados', 
		@SiteMap1Url = '/Test/IndexImport', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 0, 
		@possuiVisaoIndividual = 0, 
		@nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Arquivos'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Arquivos',  
  @SiteMap1Url = 'Arquivos',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Arquivos'
SET @nomeModulo = 'Upload de arquivos'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Upload de arquivos',  
  @SiteMap1Url = '/File',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Resultados'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Resultados',  
  @SiteMap1Url = 'Resultados',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 1,  
  @possuiVisaoIndividual = 1,  
  @nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Resultados'
SET @nomeModulo = 'Relatório de desempenho por prova'


EXEC MS_InserePaginaMenu
		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Relatório de desempenho por prova', 
		@SiteMap1Url = '/ReportTestPerformance/IndexDRE', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 0, 
		@possuiVisaoIndividual = 0, 
		@nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Resultados'
SET @nomeModulo = 'Relatório de desempenho por item'

EXEC MS_InserePaginaMenu
		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Relatório de desempenho por item', 
		@SiteMap1Url = '/ReportItemPerformance/IndexDRE', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 0, 
		@possuiVisaoIndividual = 0, 
		@nomeModuloAvo = @nomeModuloAvo


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Resultados'
SET @nomeModulo = 'Relatório de desempenho por alternativa'

EXEC MS_InserePaginaMenu
		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Relatório de desempenho por alternativa', 
		@SiteMap1Url = '/ReportItemChoice', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 0, 
		@possuiVisaoIndividual = 0, 
		@nomeModuloAvo = @nomeModuloAvo

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Relatórios'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Relatórios',  
  @SiteMap1Url = 'Relatorios',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 1,  
  @possuiVisaoIndividual = 1,  
  @nomeModuloAvo = @nomeModuloAvo 


SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Relatórios'
SET @nomeModulo = 'Relatório quantitativo de itens'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Relatório quantitativo de itens',  
  @SiteMap1Url = '/ReportItem',  
  @SiteMap2Nome = 'Relatório',  
  @SiteMap2Url = '/ReportItem/Index', 
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 0,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Relatórios'
SET @nomeModulo = 'Acompanhamento de envio de folhas de respostas'


EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Acompanhamento de envio de folhas de respostas', 
		@SiteMap1Url = '/ReportAnswerSheet/FollowUpIdentification', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 1, 
		@nomeModuloAvo = @nomeModuloAvo
		
EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Acompanhamento de envio de folhas de respostas', 
		@SiteMap1Url = '/ReportAnswerSheet/FollowUpIdentificationDRE', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 1, 
		@nomeModuloAvo = @nomeModuloAvo
		
EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Acompanhamento de envio de folhas de respostas', 
		@SiteMap1Url = '/ReportAnswerSheet/FollowUpIdentificationFiles', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 1, 
		@nomeModuloAvo = @nomeModuloAvo
		
EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Acompanhamento de envio de folhas de respostas', 
		@SiteMap1Url = '/ReportAnswerSheet/FollowUpIdentificationSchool', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 1, 
		@nomeModuloAvo = @nomeModuloAvo

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Relatórios'
SET @nomeModulo = 'Acompanhamento de processamento de correção'


EXEC MS_InserePaginaMenu

		@nomeSistema = @nomeSistema, 
		@nomeModuloPai = @nomeModuloPai, 
		@nomeModulo = @nomeModulo, 
		@SiteMap1Nome = 'Acompanhamento de processamento de correção', 
		@SiteMap1Url = '/ReportCorrection', 
		@possuiVisaoAdm = 1, 
		@possuiVisaoGestao = 1, 
		@possuiVisaoUA = 1, 
		@possuiVisaoIndividual = 0, 
		@nomeModuloAvo = @nomeModuloAvo
	
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = NULL
SET @nomeModulo = 'Parâmetros'

EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Parâmetros',  
  @SiteMap1Url = '/Parameter',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
 
 
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Parâmetros'
SET @nomeModulo = 'Parâmetros do cadastro de item'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Parâmetros do cadastro de item',  
  @SiteMap1Url = '/Parameter?Id=2',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
 
SET @nomeModulo = 'Parâmetros gerais'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Parâmetros gerais',  
  @SiteMap1Url = '/Parameter?Id=1',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 
   
SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Parâmetros'
SET @nomeModulo = 'Parâmetros da prova'
 
EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Parâmetros da prova',  
  @SiteMap1Url = '/Parameter?Id=3',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 

SET @nomeModuloAvo = NULL
SET @nomeModuloPai = 'Parâmetros'
SET @nomeModulo = 'Parâmetros do sistema'

EXEC MS_InserePaginaMenu 
  @nomeSistema = @nomeSistema,  
  @nomeModuloPai = @nomeModuloPai,  
  @nomeModulo = @nomeModulo,  
  @SiteMap1Nome = 'Parâmetros do sistema',  
  @SiteMap1Url = '/Parameter?Id=4',  
  @possuiVisaoAdm = 1,  
  @possuiVisaoGestao = 1,  
  @possuiVisaoUA = 0,  
  @possuiVisaoIndividual = 0,  
  @nomeModuloAvo = @nomeModuloAvo 


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) 
  ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 
  BEGIN
    PRINT 'The database update succeeded'
    COMMIT TRANSACTION
  END
ELSE 
  PRINT 'The database update failed - ROLLBACK aplied'
GO
DROP TABLE #tmpErrors 
GO