USE GestaoAvaliacao
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	UPDATE Test
		SET [Order] = Id

-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	