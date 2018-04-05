USE [GestaoAvaliacao]
GO

DELETE FROM [AnswerSheetBatchLog] 
DELETE FROM [AnswerSheetBatchFiles] 
DELETE FROM [AnswerSheetBatch] 
DELETE FROM [File] WHERE OwnerType IN (9,15) 
DELETE FROM [TestSectionStatusCorrection] WHERE [StatusCorrection] = 3
