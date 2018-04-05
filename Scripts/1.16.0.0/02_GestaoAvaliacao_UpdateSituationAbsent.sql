USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	UPDATE A SET Situation = 9
	FROM 
		AnswerSheetBatchFiles A WITH(NOLOCK)
		INNER JOIN AnswerSheetBatch AS ANS WITH(NOLOCK) 
			ON ANS.Id = A.AnswerSheetBatch_Id
	WHERE
		A.[Situation] = 6
		AND EXISTS (
			SELECT S.Id 
			FROM StudentTestAbsenceReason S WITH(NOLOCK)
			WHERE
				A.Student_Id = S.alu_id
				AND ANS.Test_Id = S.Test_id
				AND A.Section_Id = S.tur_id
				AND S.[State] <> 3
		)    

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION