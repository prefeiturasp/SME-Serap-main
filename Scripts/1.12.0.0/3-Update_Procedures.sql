USE [GestaoAvaliacao];

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

PRINT N'Altering [dbo].[MS_Test_SearchFiltered]...';


GO

-- =============================================
-- Author:		Guilherme Mendonça
-- Create date: 27/05/2015
-- Description:	Retorna informações da prova de acordo com os filtros
-- =============================================
ALTER PROCEDURE [dbo].[MS_Test_SearchFiltered] 
	@TestId BIGINT = NULL,
	@TestType BIGINT = NULL,
	@DisciplineId BIGINT = NULL,
	@CreationDateStart DATETIME = NULL,
	@CreationDateEnd DATETIME = NULL,
	@Pendente BIT = NULL,
	@Cadastrada BIT = NULL,
	@Andamento BIT = NULL,
	@Aplicada BIT = NULL,
	@global BIT = NULL,
	@pageSize INT,
    @pageNumber INT,
	@uad_id UNIQUEIDENTIFIER = NULL,
	@esc_id INT = NULL,
	@ttn_id INT = NULL,
	@tne_id INT = NULL,
	@crp_ordem INT = NULL,
	@typeEntity TINYINT,
	@visible BIT = NULL,
	@multidiscipline BIT = NULL,	
	@testFrequencyApplication INT = NULL
AS
BEGIN
	
	IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp
	IF OBJECT_ID('tempdb..#tmpTestes') > 0 DROP TABLE #tmpTestes

	CREATE TABLE #tmpTestes (
	[Id] [bigint] NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Bib] [bit] NOT NULL,
	[NumberItemsBlock] [int] NOT NULL,
	[NumberBlock] [int] NOT NULL,
	[NumberItem] [int] NULL,
	[ApplicationStartDate] [datetime] NOT NULL,
	[ApplicationEndDate] [datetime] NOT NULL,
	[CorrectionStartDate] [datetime] NOT NULL,
	[CorrectionEndDate] [datetime] NOT NULL,
	[UsuId] [uniqueidentifier] NOT NULL,
	[TestSituation] [int] NOT NULL,
	[AllAdhered] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Discipline_Id] [bigint] NULL,
	[FormatType_Id] [bigint] NULL,
	[TestType_Id] [bigint] NOT NULL,
	[Visible] [bit] NOT NULL,
	[TestFrequencyApplication] [int] NOT NULL,
	[Multidiscipline] [bit] NULL,
	[TestTypeDescription] [varchar](500) NOT NULL,
	[ItemType_Id] [bigint] NULL,
	[Global] [bit] NOT NULL,
	[TestTypeFrequencyApplication] [int] NOT NULL)

	IF(@global = 1 OR (@global = 0 AND @uad_id IS NULL) OR @global IS NULL)
	BEGIN
		INSERT INTO #tmpTestes
		SELECT t.Id,t.Description,t.Bib,t.NumberItemsBlock,t.NumberBlock,t.NumberItem,t.ApplicationStartDate,t.ApplicationEndDate,t.CorrectionStartDate,t.CorrectionEndDate,t.UsuId,
		t.TestSituation,t.AllAdhered,t.CreateDate,t.UpdateDate,t.State,t.Discipline_Id,t.FormatType_Id,t.TestType_Id, t.Visible, t.FrequencyApplication AS TestFrequencyApplication,
		t.Multidiscipline,
		tt.Description AS TestTypeDescription, tt.ItemType_Id, tt.[Global], tt.FrequencyApplication AS TestTypeFrequencyApplication 		
		FROM Test t
		INNER JOIN TestType tt ON tt.Id = t.TestType_Id
		WHERE t.State = 1 AND tt.State = 1 AND tt.Global = ISNULL(@global, tt.Global) AND t.Id = ISNULL(@TestId, t.Id)
	END

	ELSE
	BEGIN
		INSERT INTO #tmpTestes
		SELECT t.Id,t.Description,t.Bib,t.NumberItemsBlock,t.NumberBlock,t.NumberItem,t.ApplicationStartDate,t.ApplicationEndDate,t.CorrectionStartDate,t.CorrectionEndDate,t.UsuId,
		t.TestSituation,t.AllAdhered,t.CreateDate,t.UpdateDate,t.State,t.Discipline_Id,t.FormatType_Id,t.TestType_Id, t.TestTypeDescription, t.Visible, t.FrequencyApplication AS TestFrequencyApplication, 
		t.Multidiscipline,
		tt.ItemType_Id, tt.[Global], tt.FrequencyApplication AS TestTypeFrequencyApplication 	
		FROM GetTestAdhered(@typeEntity, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) AS t		
		INNER JOIN TestType tt ON tt.Id = t.TestType_Id
	END

	SELECT  TestId,
			UsuId,
			TestDescription, 
			TestTypeDescription,
			[Global], 
			ItemType_Id,
			CreateDate, 
			TestCreateDate,
			Discipline,
			TestFrequencyApplication, 
			TestTypeFrequencyApplication,
			ApplicationStartDate,
			ApplicationEndDate,
			CorrectionStartDate,
			CorrectionEndDate,
			Bib,
			Desempenho,
			TestSituation,
			Visible,
			ROW_NUMBER() OVER ( ORDER BY TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC) AS RowNumber
	INTO #tmp
	FROM    (
	
			 SELECT t.Id AS TestId,
					t.UsuId AS UsuId,
					t.Description AS TestDescription,
					t.TestTypeDescription,
					t.[Global],
					t.ItemType_Id,
					CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
					t.CreateDate AS TestCreateDate,
					CASE 
						WHEN d.Description IS NULL AND t.Multidiscipline = 1 THEN 'Multidisciplinar'
						ELSE d.Description 
					END AS Discipline,
					t.TestFrequencyApplication,
					t.TestTypeFrequencyApplication,
					t.Visible,
					CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
					CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
					CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
					CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
					t.Bib,
					CONVERT(Bit,(CASE WHEN COUNT(tpl.Id) > 0 THEN 1 ELSE 0 END)) AS Desempenho,
					(CASE
						WHEN t.TestSituation = 1 THEN 1
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
					 END) AS TestSituation
			 FROM #tmpTestes t WITH(NOLOCK)
				  LEFT JOIN Discipline d WITH(NOLOCK) ON d.Id = t.Discipline_Id AND d.State = 1 AND t.Discipline_Id = ISNULL(@DisciplineId, t.Discipline_Id)
				  LEFT JOIN TestPerformanceLevel tpl WITH(NOLOCK) ON tpl.Test_Id = t.Id AND tpl.State = 1
			 WHERE t.Id = ISNULL(@TestId, t.Id)
			 AND t.TestType_Id = ISNULL(@TestType, t.TestType_Id)
			 AND t.TestFrequencyApplication = ISNULL(@testFrequencyApplication, t.TestFrequencyApplication)
			 AND (@CreationDateStart IS NULL AND @CreationDateEnd IS NULL 
				 OR (@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NULL AND CAST(t.CreateDate AS Date) >= CAST(@CreationDateStart AS Date))
				 OR (@CreationDateStart IS NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date) <= CAST(@CreationDateEnd AS Date))
				 OR (@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date)  BETWEEN CAST(@CreationDateStart AS Date) AND CAST(@CreationDateEnd AS Date))
			 )
			 AND (@Pendente IS NULL AND @Cadastrada IS NULL AND @Andamento IS NULL AND @Aplicada IS NULL
				 OR (@Pendente IS NOT NULL AND t.TestSituation = 1)
				 OR (@Cadastrada IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) < CAST(t.ApplicationStartDate AS Date)))
				 OR (@Andamento IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN CAST(t.ApplicationStartDate AS Date) AND CAST(t.ApplicationEndDate AS Date)))
				 OR (@Aplicada IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) > CAST(t.ApplicationEndDate AS Date)))
			 )
			 AND t.Visible = ISNULL(@visible, t.Visible)
			 AND t.Multidiscipline = ISNULL(@multidiscipline, t.Multidiscipline)
			 GROUP BY t.Id,
					  t.UsuId,
					  t.Description,
					  t.TestTypeDescription,
					  t.[Global],
					  t.ItemType_Id,
					  t.CreateDate,
					  d.Description,
					  t.Multidiscipline,
					  t.TestFrequencyApplication,
					  t.TestTypeFrequencyApplication,
					  t.ApplicationStartDate,
					  t.ApplicationEndDate,
					  t.CorrectionStartDate,
					  t.CorrectionEndDate,
					  t.Bib,
					  t.TestSituation,
					  t.Visible
			) AS R
	ORDER BY TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC

	

		
	SELECT TOP (@pageSize) TestId, UsuId, TestDescription, TestTypeDescription, [Global], ItemType_Id, CreateDate, Discipline, TestFrequencyApplication,
					TestTypeFrequencyApplication, ApplicationStartDate, ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, Bib, Visible, Desempenho, TestSituation, RowNumber
	FROM #tmp
	WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
	ORDER BY RowNumber
	 
	SELECT COUNT(TestId) from #tmp
	 
END
GO
PRINT N'Altering [dbo].[MS_Test_SearchFilteredUser]...';


GO

-- =============================================
-- Author:		Guilherme Mendonça
-- Create date: 27/05/2015
-- Description:	Retorna informações da prova de acordo com os filtros
-- =============================================
ALTER PROCEDURE [dbo].[MS_Test_SearchFilteredUser] 
	@TestId BIGINT = NUll,
	@TestType BIGINT = NUll,
	@DisciplineId BIGINT = NUll,
	@Bimester INT = NUll,
	@CreationDateStart DATETIME = NULL,
	@CreationDateEnd DATETIME = NULL,
	@Pendente BIT = NUll,
	@Cadastrada BIT = NUll,
	@Andamento BIT = NUll,
	@Aplicada BIT = NUll,
	@global BIT = NULL,
	@pageSize INT = NUll,
    @pageNumber INT,
	@ent_id UNIQUEIDENTIFIER,
	@pes_id UNIQUEIDENTIFIER,
	@usuId UNIQUEIDENTIFIER,
	@state TINYINT,
	@typeEntity TINYINT,
	@typeSelected TINYINT,
	@typeNotSelected TINYINT,
	@gru_id UNIQUEIDENTIFIER,
	@vis_id INT,
	@uad_id UNIQUEIDENTIFIER = NULL,
	@esc_id INT = NULL,
	@ttn_id INT = NULL,
	@tne_id INT = NULL,
	@crp_ordem INT = NULL,
	@multidiscipline BIT = NULL,
	@testFrequencyApplication INT = NULL
AS
BEGIN
	
	IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp
	
	SELECT  TestId,
			UsuId,
			TestDescription, 
			TestTypeDescription, 
			[Global],
			ItemType_Id,
			CreateDate, 
			TestCreateDate,
			Discipline,
			TestFrequencyApplication,
			TestTypeFrequencyApplication, 
			ApplicationStartDate,
			ApplicationEndDate,
			CorrectionStartDate,
			CorrectionEndDate,
			Bib,
			Desempenho,
			TestSituation,
			Visible,
			ROW_NUMBER() OVER ( ORDER BY TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC) AS RowNumber
	INTO #tmp
	FROM    (
	
			 SELECT t.Id AS TestId,
					t.UsuId AS UsuId,
					t.Description AS TestDescription,
					tt.Description AS TestTypeDescription,
					tt.[Global],
					tt.ItemType_Id,
					CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
					t.CreateDate AS TestCreateDate,
					CASE 
						WHEN d.Description IS NULL AND t.Multidiscipline = 1 THEN 'Multidisciplinar'
						ELSE d.Description 
					END AS Discipline,
					t.FrequencyApplication AS TestFrequencyApplication,
					tt.FrequencyApplication AS TestTypeFrequencyApplication,
					CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
					CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
					CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
					CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
					t.Bib,
					t.Visible,
					CONVERT(Bit,(CASE WHEN COUNT(tpl.Id) > 0 THEN 1 ELSE 0 END)) AS Desempenho,
					(CASE
						WHEN t.TestSituation = 1 THEN 1
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
						WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
					 END) AS TestSituation
			 FROM Test t WITH(NOLOCK)
				  INNER JOIN TestType tt WITH(NOLOCK) ON tt.Id = t.TestType_Id 
				  LEFT JOIN Discipline d WITH(NOLOCK) ON d.Id = t.Discipline_Id AND d.State = @state AND t.Discipline_Id = ISNULL(@DisciplineId, t.Discipline_Id)
				  INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) teacher ON t.Id = teacher.Id
				  LEFT JOIN TestPerformanceLevel tpl WITH(NOLOCK) ON tpl.Test_Id = t.Id AND tpl.State = @state
			 WHERE t.State = @state AND tt.State = @state 
			 AND t.Id = ISNULL(@TestId, t.Id)
			 AND t.TestType_Id = ISNULL(@TestType, t.TestType_Id)
			 AND t.FrequencyApplication = ISNULL(@testFrequencyApplication, t.FrequencyApplication)
			 AND (@CreationDateStart IS NULL AND @CreationDateEnd IS NULL 
				 OR (@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NULL AND CAST(t.CreateDate AS Date) >= CAST(@CreationDateStart AS Date))
				 OR (@CreationDateStart IS NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date) <= CAST(@CreationDateEnd AS Date))
				 OR (@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date)  BETWEEN CAST(@CreationDateStart AS Date) AND CAST(@CreationDateEnd AS Date))
			 )
			 AND (@Pendente IS NULL AND @Cadastrada IS NULL AND @Andamento IS NULL AND @Aplicada IS NULL
				 OR (@Pendente IS NOT NULL AND t.TestSituation = 1)
				 OR (@Cadastrada IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) < CAST(t.ApplicationStartDate AS Date)))
				 OR (@Andamento IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN CAST(t.ApplicationStartDate AS Date) AND CAST(t.ApplicationEndDate AS Date)))
				 OR (@Aplicada IS NOT NULL AND (t.TestSituation = 2 AND CAST(GETDATE() AS Date) > CAST(t.ApplicationEndDate AS Date)))
			 )
			 AND tt.[Global] = ISNULL(@global, tt.[Global])
			 AND t.Visible = 1
			 AND t.Multidiscipline = ISNULL(@multidiscipline, t.Multidiscipline)
			 GROUP BY t.Id,
					  t.UsuId,
					  t.Description,
					  tt.Description,
					  tt.[Global],
					  tt.ItemType_Id,
					  t.CreateDate,
					  d.Description,
					  t.Multidiscipline,
					  t.FrequencyApplication,
					  tt.FrequencyApplication,
					  t.ApplicationStartDate,
					  t.ApplicationEndDate,
					  t.CorrectionStartDate,
					  t.CorrectionEndDate,
					  t.Bib,
					  t.TestSituation,
					  t.Visible
			) AS R
	ORDER BY TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC


		
	SELECT TOP (@pageSize) TestId, UsuId, TestDescription, TestTypeDescription, [Global], ItemType_Id, CreateDate, Discipline, TestFrequencyApplication,
					TestTypeFrequencyApplication, ApplicationStartDate, ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, Bib, Visible, Desempenho, TestSituation, RowNumber
	FROM #tmp
	WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
	ORDER BY RowNumber
	 
	SELECT COUNT(TestId) from #tmp
	 
END
GO
PRINT N'Altering [dbo].[MS_Block_SearchItems]...';


GO



-- =============================================
-- Author:		Guilherme Mendonca
-- Create date: 22/05/2015
-- Description:	Busca itens para inclusão no bloco
-- =============================================
-- =============================================
-- Alter by:		Gabriel Moreli
-- Date: 01/11/2016
-- Description:	Campo ItemCode foi alterado de INT para VARCHAR,
--				sendo assim necessário a modificação da consulta
-- =============================================

ALTER PROCEDURE [dbo].[MS_Block_SearchItems]
    @ItemCode VARCHAR(32),    
    @ProficiencyStart INT,
    @ProficiencyEnd INT,
    @Keywords VARCHAR(MAX),
    @DisciplineId BIGINT, 
    @EvaluationMatrixId BIGINT,
    @Skills VARCHAR(MAX),
	@ItemLevel VARCHAR(MAX),
    @TypeCurriculumGrades VARCHAR(MAX),
	@Global BIT,
    @pageSize INT,
    @pageNumber INT,
	@ItemTypeID BIGINT = NULL,
    @totalRecords INT OUTPUT
AS 
    BEGIN
    
    IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp

    SELECT  ItemId,
			ItemCode, 
			ItemVersion, 
			Revoked,
			BaseTextDescription, 
			[Statement],
			MatrixDescription, 
			DescriptorSentence,
			BaseTextId,
			MatrixId,
			LastVersion,
			ItemLevelDescription,
			ItemLevelValue,
			TypeCurriculumGradeId,
			[Order],
			DisciplineDescription,
			ROW_NUMBER() OVER ( ORDER BY ItemCode, ItemVersion) AS RowNumber
	INTO #tmp
	FROM    (
							SELECT  i.Id AS ItemId,
									i.ItemCode, 
									i.ItemVersion, 
									i.Revoked,
									bt.[Description] AS BaseTextDescription, 
									i.[Statement],
									em.[Description] AS MatrixDescription, 
									i.descriptorSentence AS DescriptorSentence,
									bt.Id AS BaseTextId,
									em.Id AS MatrixId,
									i.ItemSituation_Id,
									i.proficiency,
									i.LastVersion,
									i.Keywords,
									em.Discipline_Id,
									i.EvaluationMatrix_Id,
									ik.Skill_Id,
									icg.TypeCurriculumGradeId,
									it.[Description] AS ItemLevelDescription,
									it.Value AS ItemLevelValue,
									bi.[Order],
									d.[Description] AS DisciplineDescription,
									ROW_NUMBER() OVER ( PARTITION BY i.Id ORDER BY i.Id ) AS RowNumber2
							FROM    Item i WITH ( NOLOCK )
									LEFT JOIN BaseText bt WITH ( NOLOCK ) ON bt.Id = i.BaseText_Id AND bt.[State] = 1
									INNER JOIN ItemSkill ik WITH ( NOLOCK ) ON ik.Item_Id = i.Id 
									INNER JOIN ItemCurriculumGrade icg WITH ( NOLOCK ) ON icg.Item_Id = i.Id
									INNER JOIN EvaluationMatrix em WITH ( NOLOCK ) ON em.Id = i.EvaluationMatrix_Id
									INNER JOIN Discipline d WITH(NOLOCK) ON em.Discipline_Id =d.Id
									LEFT JOIN ItemLevel it WITH ( NOLOCK ) ON i.ItemLevel_Id = it.Id AND it.[State] = 1
									INNER JOIN ItemSituation its WITH ( NOLOCK ) ON i.ItemSituation_Id = its.Id
									LEFT JOIN BlockItem bi WITH ( NOLOCK ) ON bi.Item_Id = i.Id AND bi.[State] = 1
									
							WHERE   i.[State] = 1 AND em.[State] = 1 AND ik.[State] = 1 AND  icg.[State] = 1 AND its.[State] = 1
							AND (i.ItemNarrated IS NULL OR i.ItemNarrated = 0)
							AND i.ItemCode = ISNULL(UPPER(LTRIM(RTRIM(@ItemCode))), UPPER(LTRIM(RTRIM(i.ItemCode))))

							AND (i.IsRestrict = 0 OR (i.IsRestrict = 1 AND @Global = 1))

							and i.ItemType_Id = ISNULL(@ItemTypeID, i.ItemType_Id)
							AND ((@ProficiencyStart IS NULL AND @ProficiencyEnd IS NULL) 
								OR (@ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NULL AND proficiency >= @ProficiencyStart)
								OR (@ProficiencyStart IS NULL AND @ProficiencyEnd IS NOT NULL AND proficiency <= @ProficiencyEnd)
								OR (@ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NOT NULL AND proficiency BETWEEN @ProficiencyStart AND @ProficiencyEnd)
							)
							AND (@Keywords IS NULL 
								OR (@Keywords IS NOT NULL AND EXISTS (SELECT SI.Items FROM dbo.SplitString(Keywords,';') SI INNER JOIN dbo.SplitString(@Keywords,',') I ON SI.Items = I.Items))
							)
							AND Discipline_Id = ISNULL(@DisciplineId, Discipline_Id)
							AND EvaluationMatrix_Id = ISNULL(@EvaluationMatrixId, EvaluationMatrix_Id)
							AND (@TypeCurriculumGrades IS NULL 
								OR (@TypeCurriculumGrades IS NOT NULL AND TypeCurriculumGradeId IN (SELECT Items FROM dbo.SplitString(@TypeCurriculumGrades,',')))
							)
							AND (@Skills IS NULL 
								OR Skill_Id IN (SELECT Items FROM dbo.SplitString(@Skills,','))
							)
							AND (@ItemLevel IS NULL 
								OR i.ItemLevel_Id IN (SELECT Items from dbo.SplitString(@ItemLevel,','))
							)
							AND LastVersion = 1
							AND its.[Description] = 'Aceito'
														
					) as R
	WHERE @Skills IS NOT NULL OR (@Skills IS NULL AND RowNumber2 = 1)
	ORDER BY ItemCode, ItemVersion

	SELECT @totalRecords = COUNT(ItemId) from #tmp
		
	SELECT	TOP (@pageSize) 
			ItemId, 
			ItemCode, 
			ItemVersion, 
			Revoked,
			BaseTextDescription, 
			[Statement], 
			MatrixDescription, 
			DescriptorSentence, 
			BaseTextId, 
			MatrixId,  
			LastVersion, 
			ItemLevelDescription, 
			ItemLevelValue,
			TypeCurriculumGradeId,  
			[Order],
			DisciplineDescription, 
			RowNumber
	FROM	#tmp
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