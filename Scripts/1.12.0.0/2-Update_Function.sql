USE [GestaoAvaliacao]
GO
/****** Object:  UserDefinedFunction [dbo].[GetTestAdhered]    Script Date: 24/01/2017 17:31:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Luís Henrique Pupo Maron>
-- Create date: <06/01/2016>
-- Description:	<Seleciona os testes que estão aderidos conforme os parâmetros passados>
-- =============================================
ALTER FUNCTION [dbo].[GetTestAdhered]
(	
	@typeEntity int, 
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
	SELECT DISTINCT t.*, tt.Description AS TestTypeDescription
	FROM Test t
	INNER JOIN TestType tt ON tt.Id = t.TestType_Id
	INNER JOIN Adherence a ON t.Id = a.Test_Id AND a.TypeEntity = @typeEntity AND a.ParentId = ISNULL(@esc_id, a.ParentId)
	INNER JOIN SGP_TUR_Turma tur ON tur.tur_id = a.EntityId AND tur.ttn_id = ISNULL(@ttn_id, tur.ttn_id)
	INNER JOIN SGP_ESC_Escola e ON e.esc_id = tur.esc_id AND e.uad_idSuperiorGestao = ISNULL(@uad_id, e.uad_idSuperiorGestao)
	INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp ON ttcp.tur_id = tur.tur_id AND ttcp.tne_id = ISNULL(@tne_id, ttcp.tne_id) AND ttcp.crp_ordem = ISNULL(@crp_ordem, ttcp.crp_ordem)
	WHERE t.AllAdhered = 0 AND t.State = 1 AND tt.State = 1 AND tt.Global = 0

	UNION
	SELECT DISTINCT t.*, tt.Description AS TestTypeDescription
	FROM Test t
	LEFT JOIN TestType tt ON tt.Id = t.TestType_Id
	LEFT JOIN Adherence a ON t.Id = a.Test_Id AND a.TypeEntity = @typeEntity AND a.ParentId = ISNULL(@esc_id, a.ParentId)
	LEFT JOIN SGP_TUR_Turma tur ON tur.tur_id = a.EntityId AND tur.ttn_id = ISNULL(@ttn_id, tur.ttn_id)
	LEFT JOIN SGP_ESC_Escola e ON e.esc_id = tur.esc_id AND e.uad_idSuperiorGestao = ISNULL(@uad_id, e.uad_idSuperiorGestao)
	LEFT JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp ON ttcp.tur_id = tur.tur_id AND ttcp.tne_id = ISNULL(@tne_id, ttcp.tne_id) AND ttcp.crp_ordem = ISNULL(@crp_ordem, ttcp.crp_ordem)
	WHERE t.AllAdhered = 1 AND a.Id IS NULL AND t.State = 1 AND tt.State = 1 AND tt.Global = 0

)

