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

UPDATE AnswerSheetBatchQueue SET Situation = 1 WHERE Situation = 4

UPDATE AnswerSheetBatchFiles SET Situation = 1 WHERE Situation = 10

UPDATE AnswerSheetBatchFiles SET Situation = 7 WHERE Situation = 11

DELETE FROM AnswerSheetBatchLog WHERE AnswerSheetBatchFile_Id IN (
SELECT Id FROM AnswerSheetBatchFiles
WHERE UpdateDate > '2016-11-22 20:00'
AND UpdateDate < '2016-11-22 23:59'
AND Situation = 8)

UPDATE [dbo].[AnswerSheetBatchFiles] SET [Situation] = 1
 WHERE UpdateDate > '2016-11-22 20:00' AND UpdateDate < '2016-11-22 23:59' AND Situation = 8

UPDATE Test SET ProcessedCorrection = 0, ProcessedCorrectionDate = NULL

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