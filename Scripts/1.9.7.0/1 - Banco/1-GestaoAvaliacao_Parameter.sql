USE [GestaoAvaliacao]
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

------------------------------------------------------------------------------------------------------
---Antes de executar esse script atualizar as seguintes variáveis conforme as orientações abaixo------
---
---@storage_path - incluir nesta variável o valor da variável StoragePath presente no web.config do--- 
---ambiente a ser atualizado
DECLARE @storage_path VARCHAR(MAX) = N'\\pastanarede\GestaoAvaliacao_Files\Teste\'
---
---@virtual_path - incluir nesta variável a url do ambiente a ser atualizado + /Files/----------------
DECLARE @virtual_path VARCHAR(MAX) =  N'http://localhost:2549/Files/'
---
------------------------------------------------------------------------------------------------------


PRINT N'INSERT NEW PARAMETERS on [dbo].[Parameter]...';

DECLARE @ent_id UNIQUEIDENTIFIER = (SELECT TOP 1 [EntityId] FROM [dbo].[Parameter])

DECLARE @page_prova BIGINT = (SELECT [Id] FROM [dbo].[ParameterPage] WHERE [Description] = N'Prova')
DECLARE @category_geral BIGINT = (SELECT [Id] FROM [dbo].[ParameterCategory] WHERE [Description] = N'Geral')
DECLARE @page_sistema BIGINT = (SELECT [Id] FROM [dbo].[ParameterPage] WHERE [Description] = N'Sistema')

IF((SELECT COUNT([Id]) FROM [dbo].[ParameterCategory] WHERE [Description] = N'Configuração') = 0)
BEGIN
INSERT INTO [dbo].[ParameterCategory] ([Description],[CreateDate],[UpdateDate],[State]) VALUES (N'Configuração',GETDATE(),GETDATE(),1)
PRINT 'Categoria Configuração cadastrada.'
END 

DECLARE @category_config BIGINT = (SELECT [Id] FROM [dbo].[ParameterCategory] WHERE [Description] = N'Configuração')

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'DOWNLOAD_OMR_FILE') = 0)
BEGIN
INSERT [dbo].[Parameter] ([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) VALUES (N'DOWNLOAD_OMR_FILE', N'False', N'Habilitar o download dos gabaritos dos alunos do OMR', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_geral, 2)
PRINT 'Parâmetro DOWNLOAD_OMR_FILE cadastrado.'
END 

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'SHOW_MANUAL') = 0)
BEGIN
INSERT [dbo].[Parameter] ([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) VALUES (N'SHOW_MANUAL', N'True', N'Exibir link de manual para download', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_geral, 2)
PRINT 'Parâmetro SHOW_MANUAL cadastrado.'
END 

/* SPRINT 14 */
IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'ZIP_FILES_ALLOWED') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'ZIP_FILES_ALLOWED', N'image/jpeg;image/png;', N'Arquivos permitidos dentro do zip', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_config, 1)
PRINT 'Parâmetro ZIP_FILES_ALLOWED cadastrado.'
END 

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'IMAGE_FILES') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'IMAGE_FILES', N'image/jpeg;image/png;', N'Arquivos de imagens permitidos', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_config, 1)
PRINT 'Parâmetro IMAGE_FILES cadastrado.'
END

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'ZIP_FILES') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'ZIP_FILES', N'application/x-zip-compressed;application/zip;application/x-rar-compressed;application/rar;zip;rar;', N'Arquivos compactados permitidos', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_config, 1)
PRINT 'Parâmetro ZIP_FILES cadastrado.'
END

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'STORAGE_PATH') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'STORAGE_PATH', @storage_path, N'Caminho físico responsável por armazenar os arquivos do sistema', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_config, 1)
PRINT 'Parâmetro STORAGE_PATH cadastrado.'
END

/* SPRINT 15 */
IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'GLOBAL_TERM') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'GLOBAL_TERM', N'Externo', N'Nomenclatura para o termo Global do sistema', @ent_id, GETDATE(), NULL, NULL, NULL, @page_prova, GETDATE(), GETDATE(), 1, @category_geral, 1)
PRINT 'Parâmetro GLOBAL_TERM cadastrado.'
END

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'LOCAL_TERM') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'LOCAL_TERM', N'Interno', N'Nomenclatura para o termo Local do sistema', @ent_id, GETDATE(), NULL, NULL, NULL, @page_prova, GETDATE(), GETDATE(), 1, @category_geral, 1)
PRINT 'Parâmetro LOCAL_TERM cadastrado.'
END

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'VIRTUAL_PATH') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'VIRTUAL_PATH', @virtual_path, N'Caminho virtual do armazenamento dos arquivos do sistema', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_config, 1)
PRINT 'Parâmetro VIRTUAL_PATH cadastrado.'
END

IF((SELECT COUNT([Id]) FROM [dbo].[Parameter] WHERE [Key] = N'DELETE_BATCH_FILES') = 0)
BEGIN
INSERT [dbo].[Parameter] 
([Key], [Value], [Description], [EntityId], [StartDate], [EndDate], [Obligatory], [Versioning], [ParameterPage_Id], [CreateDate], [UpdateDate], [State], [ParameterCategory_Id], [ParameterType_Id]) 
VALUES (N'DELETE_BATCH_FILES', N'false', N'Habilitar exclusão dos arquivos da correção automática de provas', @ent_id, GETDATE(), NULL, NULL, NULL, @page_sistema, GETDATE(), GETDATE(), 1, @category_geral, 2)
PRINT 'Parâmetro DELETE_BATCH_FILES cadastrado.'
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