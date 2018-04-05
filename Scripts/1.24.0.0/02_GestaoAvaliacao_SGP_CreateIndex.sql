USE [GestaoAvaliacao_SGP]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ACA_Aluno_01' AND object_id = OBJECT_ID('ACA_Aluno'))
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_ACA_Aluno_01]
		ON [dbo].[ACA_Aluno] ([pes_id])
	END
	
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_TUR_Turma_01' AND object_id = OBJECT_ID('TUR_Turma'))
	BEGIN		
		CREATE NONCLUSTERED INDEX [IX_TUR_Turma_01]
		ON [dbo].[TUR_Turma] ([esc_id])
	END
	
--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION