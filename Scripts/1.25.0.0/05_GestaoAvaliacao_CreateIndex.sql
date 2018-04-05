USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_File_01' AND object_id = OBJECT_ID('File'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_File_01]
		ON [dbo].[File] ([State],[OwnerType],[ParentOwnerId])
		INCLUDE ([Id],[Name],[Path],[OwnerId],[OriginalName])
	END  

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION