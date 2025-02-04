USE [GestaoAvaliacao]
GO
/****** Object:  UserDefinedFunction [dbo].[TestsByUser]    Script Date: 08/03/2017 10:00:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Luís Maron>
-- Create date: <10/12/2015>
-- Description:	<Retorna as provas que um professor tem acesso>
-- =============================================
ALTER FUNCTION [dbo].[TestsByUser]
(	
	@usuId UNIQUEIDENTIFIER, 
	@pes_id UNIQUEIDENTIFIER, 
	@ent_id UNIQUEIDENTIFIER, 
	@state int, 
	@typeEntity int, 
	@typeSelected INT, 
	@typeNotSelected INT,
	@gru_id UNIQUEIDENTIFIER,
	@vis_id INT,
	@uad_id UNIQUEIDENTIFIER = NULL,
	@esc_id INT = NULL,
	@ttn_id INT = NULL,
	@tne_id INT = NULL,
	@crp_ordem INT = NULL
)
RETURNS TABLE 
AS
RETURN 
(
	WITH Turmas AS(
		SELECT tur_id
		FROM GetUserSection(@gru_id, @usuId, @pes_id, @ent_id,	@vis_id, @state, @esc_id, @uad_id, @ttn_id, @tne_id, @crp_ordem)
	)

	SELECT t.Id
	FROM Test t
	INNER JOIN Adherence a ON t.Id = a.Test_Id AND a.TypeEntity = @typeEntity
	INNER JOIN Turmas tur ON tur.tur_id = a.EntityId
	WHERE t.UsuId <> @usuId AND t.AllAdhered = 0 
	GROUP BY t.Id
	
	UNION

	SELECT t.Id
	FROM Turmas AS tur 
	INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON tur.tur_id = ttcp.tur_id AND ttcp.tur_id = tur.tur_id AND ttcp.ttcr_situacao = 1 
	INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id
	INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.State = 1
	INNER JOIN Test t WITH (NOLOCK) ON tcc.Test_Id = t.Id
	LEFT JOIN Adherence Adh WITH(NOLOCK) ON Adh.EntityId = tur.tur_id AND Adh.TypeEntity = @typeEntity AND Adh.Test_Id = t.Id AND Adh.TypeSelection = @typeNotSelected
	WHERE Adh.Id IS NULL
	AND t.AllAdhered = 1
	GROUP BY T.Id
	
	UNION
	SELECT t.Id
	FROM Test t
	WHERE t.UsuId = @usuId
	GROUP BY t.Id
)

