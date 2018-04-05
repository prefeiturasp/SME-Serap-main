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
PRINT N'Dropping [dbo].[AnswerSheetBatchFiles].[IX_AnswerSheetBatch_Id]...';


GO
DROP INDEX [IX_AnswerSheetBatch_Id]
    ON [dbo].[AnswerSheetBatchFiles];


GO
PRINT N'Dropping [dbo].[FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id]...';


GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] DROP CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id];


GO
PRINT N'Altering [dbo].[AnswerSheetBatchFiles]...';


GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] ALTER COLUMN [AnswerSheetBatch_Id] BIGINT NULL;


GO
PRINT N'Creating [dbo].[AnswerSheetBatchFiles].[IX_AnswerSheetBatch_Id]...';


GO
CREATE NONCLUSTERED INDEX [IX_AnswerSheetBatch_Id]
    ON [dbo].[AnswerSheetBatchFiles]([AnswerSheetBatch_Id] ASC);


GO
PRINT N'Creating [dbo].[FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id]...';


GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id] FOREIGN KEY ([AnswerSheetBatch_Id]) REFERENCES [dbo].[AnswerSheetBatch] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] WITH CHECK CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id];


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
