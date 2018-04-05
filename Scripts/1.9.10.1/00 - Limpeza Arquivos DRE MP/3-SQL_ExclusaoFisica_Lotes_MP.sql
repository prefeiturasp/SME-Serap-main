USE GestaoAvaliacao
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

DECLARE @DRE UNIQUEIDENTIFIER = (SELECT TOP 1 UA.[uad_id] FROM [SGP_SYS_UnidadeAdministrativa] UA WHERE UA.[uad_sigla] = 'MP' AND UA.[uad_situacao] = 1)

DELETE FROM [AnswerSheetBatchLog] 
WHERE [AnswerSheetBatchFile_Id] IN (SELECT [Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE) 

DELETE FROM [StudentTestAbsenceReason] 
WHERE [tur_id] IN (SELECT [Section_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)
AND [alu_id]  IN (SELECT [Student_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)
AND [Test_Id] IN (SELECT [Test_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)

DELETE FROM [TestSectionStatusCorrection] 
WHERE [StatusCorrection] = 3 
AND [tur_id] IN (SELECT [Section_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE) 
AND [Test_Id] IN (SELECT [Test_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)

DELETE FROM [File] 
WHERE [OwnerType] IN (9,15) 
AND [Id] IN (SELECT [File_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)

DELETE FROM [AnswerSheetBatch] 
WHERE [Id] IN (SELECT [AnswerSheetBatch_Id] FROM [AnswerSheetBatchFiles] WHERE [SupAdmUnit_Id] = @DRE)

DELETE FROM [AnswerSheetBatchFiles] 
WHERE [SupAdmUnit_Id] = @DRE

DELETE FROM [AnswerSheetBatchQueue] 
WHERE [SupAdmUnit_Id] = @DRE

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