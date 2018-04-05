USE GestaoAvaliacao;
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

PRINT N'Creating [dbo].[AnswerSheetBatchQueue]...';
CREATE TABLE [dbo].[AnswerSheetBatchQueue] (
    [Id] [bigint] NOT NULL IDENTITY,
    [File_Id] [bigint] NOT NULL,
    [AnswerSheetBatch_Id] [bigint],
    [SupAdmUnit_Id] [uniqueidentifier],
    [School_Id] [int],
    [CountFiles] [int] NULL,
    [Situation] [tinyint] NOT NULL,
    [Description] [varchar](max),
    [CreatedBy_Id] [uniqueidentifier] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
    [CreateDate] [datetime] NOT NULL,
    [UpdateDate] [datetime] NOT NULL,
    [State] [tinyint] NOT NULL,
    CONSTRAINT [PK_dbo.AnswerSheetBatchQueue] PRIMARY KEY ([Id])
)
ALTER TABLE [dbo].[AnswerSheetBatchFiles] ADD [AnswerSheetBatchQueue_Id] [bigint]
CREATE INDEX [IX_AnswerSheetBatchQueue_Id] ON [dbo].[AnswerSheetBatchFiles]([AnswerSheetBatchQueue_Id])
CREATE INDEX [IX_File_Id] ON [dbo].[AnswerSheetBatchQueue]([File_Id])
CREATE INDEX [IX_AnswerSheetBatch_Id] ON [dbo].[AnswerSheetBatchQueue]([AnswerSheetBatch_Id])
ALTER TABLE [dbo].[AnswerSheetBatchFiles] ADD CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatchQueue_AnswerSheetBatchQueue_Id] FOREIGN KEY ([AnswerSheetBatchQueue_Id]) REFERENCES [dbo].[AnswerSheetBatchQueue] ([Id])
ALTER TABLE [dbo].[AnswerSheetBatchQueue] ADD CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.AnswerSheetBatch_AnswerSheetBatch_Id] FOREIGN KEY ([AnswerSheetBatch_Id]) REFERENCES [dbo].[AnswerSheetBatch] ([Id])
ALTER TABLE [dbo].[AnswerSheetBatchQueue] ADD CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.File_File_Id] FOREIGN KEY ([File_Id]) REFERENCES [dbo].[File] ([Id]) ON DELETE CASCADE
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