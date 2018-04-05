USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	; WITH Dados AS (
		SELECT 
			ROW_NUMBER() OVER(ORDER BY G.Description, S.Description) AS Ordem,
			S.Id 
		FROM 
			TestSubGroup S WITH(NOLOCK)
			INNER JOIN TestGroup G WITH(NOLOCK)
				ON S.TestGroup_Id = G.Id
	)

	UPDATE S SET [Order] = Dados.Ordem
	FROM 
		Dados 
		INNER JOIN TestSubGroup AS S 
			ON Dados.Id = S.Id    

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION