USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	; WITH Grupos AS (
		SELECT Gru.gru_id, Gru.gru_nome
		FROM 
			Synonym_Core_SYS_Grupo AS Gru WITH(NOLOCK)
		WHERE
			Gru.sis_id = 204
			AND Gru.vis_id <> 1
			AND Gru.gru_situacao = 1
	)

	INSERT INTO TestPermission(gru_id, Test_Id, AllowAnswer, ShowResult, TestHide, CreateDate, UpdateDate, State)
	SELECT 
		G.gru_id, 
		T.Id AS Test_Id, 
		1 AS AllowAnswer, 
		1 AS ShowResult, 
		1 AS TestHide, 
		GETDATE() AS CreateDate, 
		GETDATE() AS UpdateDate, 
		1 AS [State]
	FROM 
		Test T WITH(NOLOCK) 
		CROSS APPLY Grupos G
	WHERE 
		T.Id <= 181
		AND T.[State] <> 3
		AND NOT EXISTS (
			SELECT 
				Tp.Id 
			FROM 
				TestPermission Tp WITH(NOLOCK)
			WHERE
				Tp.[State] <> 3
				AND Tp.gru_id = G.gru_id
				AND Tp.Test_Id = T.Id
		)
	ORDER BY 
		T.Id

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION