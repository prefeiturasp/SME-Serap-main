USE [GestaoAvaliacao_Dev];

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

IF NOT EXISTS ( SELECT  Id
				FROM    Parameter
				WHERE   [Key] = 'QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA') 
BEGIN
	INSERT INTO  [Parameter] 
			([Key], [Value], [Description], [EntityId], [StartDate], [ParameterPage_Id], 
			 [CreateDate], [UpdateDate], [State], 
			 [ParameterCategory_Id], [ParameterType_Id])
	 VALUES ('QUANTIDADE_MESES_ANTES_DEPOIS_FILTRO_DATA','2', 'Quantidade de meses para considerar no filtro de data dos relatórios (antes de depois da data atual)', '6CF424DC-8EC3-E011-9B36-00155D033206',
			 GETDATE(), (SELECT TOP 1 Id FROM ParameterPage WHERE [Description] = 'Geral'),  
			 GETDATE(), GETDATE(), 1, 
			 (SELECT TOP 1 Id FROM ParameterCategory WHERE [Description] = 'Geral'), (SELECT TOP 1 Id FROM ParameterType WHERE [Description] = 'Input'))
END

IF NOT EXISTS ( SELECT  Id
				FROM    Parameter
				WHERE   [Key] = 'HTML_PAGINA_INICIAL') 
BEGIN
	INSERT INTO  [Parameter] 
			([Key], [Value], [Description], [EntityId], [StartDate], [ParameterPage_Id], 
			 [CreateDate], [UpdateDate], [State], 
			 [ParameterCategory_Id], [ParameterType_Id])
	 VALUES ('HTML_PAGINA_INICIAL',
			 '<div class=''col-md-5 texto'' style=''margin-top:-40px!important;margin-left:-100px!important;padding: 30px;border-color: #fff;border-style: solid;border-radius: 10px;''><p style=''text-align:center;font-size:20px;''>Acesse os resultados da Avaliação Diagnóstica<br /><br /><a style=''color: white;font-weight: bold;background-color: #3A7FCD;border-radius: 10px;padding: 10px 30px 10px 30px;border-color: #fff;border-style: solid;'' href=''http://avaliacaodiagnostica.serap.sme.prefeitura.sp.gov.br'' target=''_blank''>Clique aqui</a></p></div>', 
			 'HTML que deve ser exibido na página inicial do sistema', 
			 '6CF424DC-8EC3-E011-9B36-00155D033206',
			 GETDATE(), 
			 (SELECT TOP 1 Id FROM ParameterPage WHERE [Description] = 'Geral'),  
			 GETDATE(), 
			 GETDATE(), 
			 1, 
			 (SELECT TOP 1 Id FROM ParameterCategory WHERE [Description] = 'Geral'), (SELECT TOP 1 Id FROM ParameterType WHERE [Description] = 'Input'))
END


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