USE [GestaoAvaliacao]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	DECLARE @dataAtual DATE = GETDATE()

	INSERT INTO Adherence(EntityId, ParentId, TypeEntity, TypeSelection, Test_Id, CreateDate, UpdateDate, [State])
	SELECT	
		Mtu.alu_id AS EntityId, 
		Mtu.tur_id AS ParentId, 
		3 AS TypeEntity, --Student
		1 AS TypeSelection, 
		T.Id, 
		@dataAtual AS CreateDate, 
		@dataAtual AS UpdateDate, 
		1 AS [State] 
	FROM 
		Test T WITH(NOLOCK) 
		INNER JOIN Adherence A WITH(NOLOCK)
			ON T.Id = A.Test_Id
			AND A.State = 1
			AND A.TypeEntity = 1 -- Section
			AND A.TypeSelection = 1
		INNER JOIN SGP_MTR_MatriculaTurma AS Mtu WITH(NOLOCK)
			ON Mtu.tur_id = A.EntityId
			AND Mtu.mtu_situacao <> 3
			AND Mtu.mtu_dataMatricula <= T.ApplicationStartDate
			AND (Mtu.mtu_dataSaida IS NULL OR Mtu.mtu_dataSaida > T.ApplicationStartDate)
		INNER JOIN SGP_ACA_Aluno AS Alu WITH(NOLOCK)
			ON Mtu.alu_id = Alu.alu_id
			AND Alu.alu_situacao <> 3
	WHERE 
		T.AllAdhered = 0 --Adesao parcial
		AND T.State = 1
		AND NOT EXISTS (
			SELECT A.Id FROM Adherence AAluno WITH(NOLOCK)
			WHERE
				AAluno.EntityId = Mtu.alu_id
				AND AAluno.Test_Id = T.Id
				AND AAluno.State <> 3
				
		)

--Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION