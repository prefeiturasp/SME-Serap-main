USE GestaoAvaliacao
GO

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION

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

GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) 
  ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 
  BEGIN
    PRINT 'The database update succeeded'
    COMMIT TRANSACTION
  END
ELSE 
  PRINT 'The database update failed - ROLLBACK aplied'
GO
DROP TABLE #tmpErrors 
GO