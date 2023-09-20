USE [GestaoAvaliacao]
GO
/****** Object:  StoredProcedure [dbo].[MS_Test_SearchFilteredUser]    Script Date: 16/05/2023 20:06:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
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
	@testFrequencyApplication INT = NULL,
	@getGroup BIT = NULL,
	@TestGroupId BIGINT = NULL,
	@TestSubGroupId BIGINT = NULL,
	@ordenacao TINYINT = NULL

AS
BEGIN
	
	IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp
	
	SELECT  TestId,
			[Order],
			UsuId,
			TestDescription, 
			TestTypeDescription, 
			[Global],
			ItemType_Id,
			CreateDate,
			UpdateDate,
			ShowOnSerapEstudantes,
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
			TestGroupId,
			TestSubGroupId,
			OrderSubGroup,
			TestGroupName,
			TestSubGroupName,
			TestGroupCreateDate,
			ROW_NUMBER() OVER ( ORDER BY (CASE @ordenacao WHEN 1 THEN TestCreateDate WHEN 2 THEN TestId WHEN 3 THEN [Order] END) DESC, [Order] DESC, TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC) AS RowNumber
	INTO #tmp
	FROM    (
	
			 SELECT t.Id AS TestId,
					t.[Order] AS [Order],
					t.UsuId AS UsuId,
					t.Description AS TestDescription,
					tt.Description AS TestTypeDescription,
					tt.[Global],
					tt.ItemType_Id,
					CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
					CONVERT(VARCHAR(50), t.UpdateDate, 103) AS UpdateDate,
					t.ShowOnSerapEstudantes,
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
					 END) AS TestSituation,
					 ISNULL(S.TestGroup_id, 0) AS TestGroupId,
					 ISNULL(t.TestSubGroup_id, 0) AS TestSubGroupId,
					 S.[Order] AS OrderSubGroup,
					 ISNULL(G.Description, 'Provas antigas') AS TestGroupName,
					 ISNULL(S.Description, 'Provas antigas') AS TestSubGroupName,
					 G.CreateDate AS TestGroupCreateDate
			 FROM Test t WITH(NOLOCK)
				  INNER JOIN TestType tt WITH(NOLOCK) ON tt.Id = t.TestType_Id 
				  INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) teacher ON t.Id = teacher.Id
				  LEFT JOIN Discipline d WITH(NOLOCK) ON d.Id = t.Discipline_Id AND d.State = @state AND t.Discipline_Id = ISNULL(@DisciplineId, t.Discipline_Id)
				  LEFT JOIN TestPerformanceLevel tpl WITH(NOLOCK) ON tpl.Test_Id = t.Id AND tpl.State = @state
				  LEFT JOIN TestPermission TP WITH(NOLOCK) ON TP.Test_Id = t.Id AND TP.[State] = 1 AND TP.gru_id = @gru_id
				  LEFT JOIN TestSubGroup S WITH(NOLOCK) ON t.TestSubGroup_Id = S.Id AND S.State = 1
				  LEFT JOIN TestGroup G WITH(NOLOCK) ON S.TestGroup_id = G.Id AND G.State = 1
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
			 AND (@TestGroupId IS NULL OR S.TestGroup_id = @TestGroupId OR (@TestGroupId = 0 AND S.TestGroup_id IS NULL))
			 AND (@TestSubGroupId IS NULL OR  t.TestSubGroup_id = @TestSubGroupId OR (@TestSubGroupId = 0 AND t.TestSubGroup_id IS NULL))
			 AND tt.[Global] = ISNULL(@global, tt.[Global])
			 AND t.Visible = 1
			 AND ISNULL(TP.TestHide, 0) = 0
			 AND t.Multidiscipline = ISNULL(@multidiscipline, t.Multidiscipline)
			 GROUP BY t.Id,
					  t.[Order],
					  t.UsuId,
					  t.Description,
					  tt.Description,
					  tt.[Global],
					  tt.ItemType_Id,
					  t.CreateDate,
					  t.UpdateDate,
					  t.ShowOnSerapEstudantes,
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
					  t.Visible,
					  S.TestGroup_id,
					  t.TestSubGroup_id,
					  S.[Order],
					  G.Description,
					  S.Description,
					  G.CreateDate
			) AS R
	ORDER BY (CASE @ordenacao WHEN 1 THEN TestCreateDate WHEN 2 THEN TestId WHEN 3 THEN [Order] END) DESC, R.[Order] DESC, TestCreateDate DESC, TestTypeDescription ASC, TestDescription ASC

	DECLARE @qtd INT = (SELECT COUNT(TestId) from #tmp)
			
	IF (@getGroup = 1)
	BEGIN
		SET @pageSize = @qtd
	END
		
	SELECT TOP (@pageSize) TestId, UsuId, TestDescription, TestTypeDescription, [Global], ItemType_Id, CreateDate, UpdateDate, ShowOnSerapEstudantes, Discipline, TestFrequencyApplication,
					TestTypeFrequencyApplication, ApplicationStartDate, ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, Bib, Visible, Desempenho, TestSituation, RowNumber,
					TestGroupId, TestSubGroupId, OrderSubGroup, TestGroupName, TestSubGroupName, TestGroupCreateDate
	FROM #tmp
	WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
	ORDER BY RowNumber
	 
	SELECT @qtd

END

