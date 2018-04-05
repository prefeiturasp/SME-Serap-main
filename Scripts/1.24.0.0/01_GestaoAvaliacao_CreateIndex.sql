USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Adherence_01' AND object_id = OBJECT_ID('Adherence'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_Adherence_01]
		ON [dbo].[Adherence] ([TypeEntity],[State])
		INCLUDE ([EntityId],[TypeSelection],[Test_Id])
	END

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Adherence_02' AND object_id = OBJECT_ID('Adherence'))
	BEGIN	
		CREATE NONCLUSTERED INDEX [IX_Adherence_02]
		ON [dbo].[Adherence] ([EntityId],[TypeEntity],[Test_Id],[State])
		INCLUDE ([TypeSelection])
	END

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Adherence_03' AND object_id = OBJECT_ID('Adherence'))
	BEGIN		
		CREATE NONCLUSTERED INDEX [IX_Adherence_03]
		ON [dbo].[Adherence] ([TypeEntity],[TypeSelection],[Test_Id])
		INCLUDE ([Id],[EntityId])	
	END
	
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_AnswerSheetBatchFiles_01' AND object_id = OBJECT_ID('AnswerSheetBatchFiles'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_AnswerSheetBatchFiles_01]
		ON [dbo].[AnswerSheetBatchFiles] ([State])
		INCLUDE ([Id],[Student_Id],[Situation])
	END
		
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_AnswerSheetBatchLog_State' AND object_id = OBJECT_ID('AnswerSheetBatchLog'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_AnswerSheetBatchLog_State] 
		ON [dbo].[AnswerSheetBatchLog] ([State]) 
		INCLUDE ([AnswerSheetBatchFile_Id], [Situation])
	END

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION