USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON


	INSERT INTO BlockKnowledgeArea(Block_Id, KnowledgeArea_Id, [Order], CreateDate, UpdateDate, [State])
	SELECT 
		B.Id AS Block_Id, K.Id AS KnowledgeArea_Id,
		(ROW_NUMBER() OVER(PARTITION BY T.Id, B.Id ORDER BY K.[Description]) - 1) AS Ordem,
		GETDATE(), GETDATE(), 1 AS [State]
	FROM 
		Test AS T WITH(NOLOCK) 
		INNER JOIN Block B WITH(NOLOCK)
			ON T.Id = B.Test_Id
			AND B.[State] <> 3
		INNER JOIN BlockItem Bi WITH(NOLOCK)
			ON B.Id = Bi.Block_Id
			AND Bi.[State] <> 3
		INNER JOIN Item I WITH(NOLOCK)
			ON Bi.Item_Id = I.Id
			AND I.[State] <> 3
		INNER JOIN KnowledgeArea K WITH(NOLOCK)
			ON I.KnowledgeArea_Id = K.Id
			AND K.[State] <> 3
	WHERE 
		NOT EXISTS (
			SELECT * FROM BlockKnowledgeArea Bk WITH(NOLOCK)
			WHERE
				Bk.Block_id = B.Id
				AND Bk.KnowledgeArea_Id = K.Id
		)
	GROUP BY
		T.Id, B.Id, K.Id, K.[Description]
	ORDER BY
		T.Id

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION