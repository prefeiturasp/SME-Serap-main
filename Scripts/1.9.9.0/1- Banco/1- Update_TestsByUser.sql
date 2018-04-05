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


/****** Object:  UserDefinedFunction [dbo].[TestsByUser]    Script Date: 10/21/2016 14:25:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestsByUser]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[TestsByUser]
GO


/****** Object:  UserDefinedFunction [dbo].[TestsByUser]    Script Date: 10/21/2016 14:25:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Luís Maron>
-- Create date: <10/12/2015>
-- Description:	<Retorna as provas que um professor tem acesso>
-- =============================================
CREATE FUNCTION [dbo].[TestsByUser]
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
	INNER JOIN Discipline d ON d.Id = t.Discipline_Id
	INNER JOIN Adherence a ON t.Id = a.Test_Id AND a.TypeEntity = @typeEntity
	INNER JOIN Turmas tur ON tur.tur_id = a.EntityId
	WHERE t.UsuId <> @usuId AND t.AllAdhered = 0 
	
	UNION
	SELECT t.Id
	FROM Test t
	LEFT JOIN (
		SELECT DISTINCT a.Test_Id
		FROM Adherence a 
		INNER JOIN Turmas tur ON tur.tur_id = a.EntityId
		WHERE a.TypeEntity = @typeEntity AND TypeSelection = @typeNotSelected) adherence ON adherence.Test_Id = t.Id
	WHERE t.UsuId <> @usuId AND t.AllAdhered = 1 AND adherence.Test_Id IS NULL
	AND (@vis_id <> 3 OR (@vis_id = 3 AND ((SELECT COUNT(tur_id) FROM Turmas) > 0)))
	
	UNION
	SELECT t.Id
	FROM Test t
	WHERE t.UsuId = @usuId
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


