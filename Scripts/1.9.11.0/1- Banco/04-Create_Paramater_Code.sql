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

DECLARE @ent_id AS UNIQUEIDENTIFIER = ( SELECT  ent_id
										FROM    SYS_Entidade
										WHERE   
										ent_sigla = 'SMESP' and ent_situacao = 1
									  )

USE GestaoAvaliacao


DECLARE @CAT INT = (SELECT Id FROM [dbo].[ParameterCategory] WHERE [Description] = 'Geral')
DECLARE @ckeckboxtype_id AS INT = (SELECT Id FROM ParameterType WHERE Description = 'Checkbox')
DECLARE @geralpage_id AS INT = (SELECT Id FROM ParameterPage WHERE Description = 'Item')


IF NOT EXISTS ( SELECT  Id
				FROM    Parameter
				WHERE   [Key] = 'CODE') 
BEGIN
	INSERT INTO [dbo].[Parameter]
			   ([Key]
			   ,[Value]
			   ,[Description]
			   ,[EntityId]
			   ,[StartDate]
			   ,[EndDate]
			   ,[Obligatory]
			   ,[Versioning]
			   ,[ParameterPage_Id]
			   ,[CreateDate]
			   ,[UpdateDate]
			   ,[State]
			   ,[ParameterCategory_Id]
			   ,[ParameterType_Id])
		 VALUES
			   ('CODE'
			   ,'Código'
			   ,'Código do item'
			   ,@ent_id
			   ,CAST(GETDATE() AS DATE)
			   ,null
			   ,0
			   ,null
			   ,@geralpage_id
			   ,CAST(GETDATE() AS DATE)
			   ,CAST(GETDATE() AS DATE)
			   ,1
			   ,@CAT
			   ,@ckeckboxtype_id)
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


