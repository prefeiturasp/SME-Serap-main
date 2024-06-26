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
-- Author:		Leticia Goes
-- Create date: 16/10/2014
-- Description:	Busca o banco de itens
-- =============================================
-- =============================================
-- Alter by:		Marcelo Franco
-- Date: 01/04/2015
-- Description:	Melhora performance e correção na busca de itens versionados
-- =============================================
-- =============================================
-- Alter by:		Gabriel Moreli
-- Date: 01/11/2016
-- Description:	Campo ItemCode foi alterado de INT para VARCHAR,
--				sendo assim necessário a modificação da consulta
-- =============================================
ALTER PROCEDURE [dbo].[MS_Item_SearchFiltered]
    @ItemCode VARCHAR(32),
	@Revoked BIT,
    @ItemSituation VARCHAR(MAX),	
    @ShowVersion BIT,
    @ProficiencyStart INT,
    @ProficiencyEnd INT,
    @Keywords VARCHAR(MAX),
    @DisciplineId BIGINT, 
    @EvaluationMatrixId BIGINT,
    @ShowItemNarrated BIT,
    @Skills VARCHAR(MAX),
    @TypeCurriculumGrades VARCHAR(MAX),
    @pageSize INT,
    @pageNumber INT,
    @totalRecords INT OUTPUT
AS 
    BEGIN
    
    IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp

    SELECT  ItemId,
			ItemCode, 
			Revoked,
			ItemVersion, 
			BaseTextDescription, 
			Statement,
			MatrixDescription, 
			DescriptorSentence,
			BaseTextId,
			MatrixId,
			LastVersion,
			ItemNarrated,
			Discipline_Id AS DisciplineId,
			DisciplineDescription,
			ROW_NUMBER() OVER ( ORDER BY ItemCode, ItemVersion) AS RowNumber
	INTO #tmp
	FROM    (
							SELECT  i.Id AS ItemId,
									i.ItemCode, 
									i.Revoked,
									i.ItemVersion, 
									bt.Description AS BaseTextDescription, 
									i.Statement,
									em.Description AS MatrixDescription, 
									i.descriptorSentence AS DescriptorSentence,
									bt.Id AS BaseTextId,
									em.Id AS MatrixId,
									i.ItemSituation_Id,
									i.proficiency,
									i.LastVersion,
									i.Keywords,
									em.Discipline_Id,
									d.Description AS DisciplineDescription,
									i.EvaluationMatrix_Id,
									ik.Skill_Id,
									icg.TypeCurriculumGradeId,
									i.ItemNarrated,
									ROW_NUMBER() OVER ( PARTITION BY i.Id ORDER BY i.Id ) AS RowNumber2
							FROM    Item i WITH ( NOLOCK )
									LEFT JOIN BaseText bt WITH ( NOLOCK ) ON bt.Id = i.BaseText_Id AND bt.State = 1
									INNER JOIN ItemSkill ik WITH ( NOLOCK ) ON ik.Item_Id = i.Id 
									INNER JOIN ItemCurriculumGrade icg WITH ( NOLOCK ) ON icg.Item_Id = i.Id
									INNER JOIN EvaluationMatrix em WITH ( NOLOCK ) ON em.Id = i.EvaluationMatrix_Id
									LEFT JOIN Discipline d WITH ( NOLOCK) ON d.Id = em.Discipline_Id
							WHERE   i.State = 1 AND em.State = 1 AND ik.State = 1 AND  icg.State = 1
							AND (@ItemCode IS NULL OR @ItemCode IS NOT NULL AND UPPER(LTRIM(RTRIM(i.ItemCode))) = UPPER(LTRIM(RTRIM(@ItemCode))))
							AND ((@Revoked IS NULL OR @Revoked = 0 )OR @Revoked IS NOT NULL AND i.Revoked = @Revoked)
							AND (@ItemSituation IS NULL OR @ItemSituation IS NOT NULL AND ItemSituation_Id IN (SELECT Items FROM dbo.SplitString(@ItemSituation,',')))
							AND (@ProficiencyStart IS NULL AND @ProficiencyEnd IS NULL 
							OR @ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NULL AND proficiency >= @ProficiencyStart
							OR @ProficiencyStart IS NULL AND @ProficiencyEnd IS NOT NULL AND proficiency <= @ProficiencyEnd
							OR @ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NOT NULL AND proficiency BETWEEN @ProficiencyStart AND @ProficiencyEnd)
							AND (@ShowVersion = 1 OR @ShowVersion IS NOT NULL AND @ShowVersion = 0 AND LastVersion = 1)
							AND (@ShowItemNarrated = 0 OR @ShowItemNarrated IS NOT NULL AND @ShowItemNarrated = 1 AND i.ItemNarrated = 1)
							AND (@Keywords IS NULL OR (@Keywords IS NOT NULL AND EXISTS (SELECT SI.Items FROM dbo.SplitString(Keywords,';') SI INNER JOIN dbo.SplitString(@Keywords,',') I ON SI.Items = I.Items)))
							AND (@DisciplineId IS NULL OR @DisciplineId IS NOT NULL AND Discipline_Id = @DisciplineId)
							AND (@EvaluationMatrixId IS NULL OR @EvaluationMatrixId IS NOT NULL AND EvaluationMatrix_Id = @EvaluationMatrixId)
							AND (@TypeCurriculumGrades IS NULL OR @TypeCurriculumGrades IS NOT NULL AND TypeCurriculumGradeId IN (SELECT Items FROM dbo.SplitString(@TypeCurriculumGrades,',')))
							AND (@Skills IS NULL OR Skill_Id IN (SELECT Items FROM dbo.SplitString(@Skills,',')))
					) as R
	WHERE @Skills IS NOT NULL OR (@Skills IS NULL AND RowNumber2 = 1)
	ORDER BY ItemCode, ItemVersion

	SELECT @totalRecords = COUNT(ItemId) from #tmp
		
	SELECT TOP (@pageSize) ItemId, ItemCode, Revoked, ItemVersion, BaseTextDescription, Statement, MatrixDescription, DescriptorSentence, BaseTextId, MatrixId,  LastVersion, ItemNarrated, DisciplineId, DisciplineDescription, RowNumber
	FROM #tmp
	WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
	ORDER BY RowNumber
	
    END

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



