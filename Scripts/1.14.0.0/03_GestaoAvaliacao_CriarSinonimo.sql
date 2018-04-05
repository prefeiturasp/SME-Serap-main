USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	IF (NOT EXISTS (SELECT * FROM sys.synonyms WITH(NOLOCK) WHERE name = 'SGP_ACA_CalendarioAnual'))
	BEGIN
		CREATE SYNONYM [dbo].[SGP_ACA_CalendarioAnual] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_CalendarioAnual]
	END

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION