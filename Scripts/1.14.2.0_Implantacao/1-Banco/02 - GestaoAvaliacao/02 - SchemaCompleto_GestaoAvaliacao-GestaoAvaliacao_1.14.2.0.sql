USE [GestaoAvaliacao]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_Aluno]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_Aluno] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_Aluno]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_CalendarioAnual]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_CalendarioAnual] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_CalendarioAnual]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_CurriculoPeriodo]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_CurriculoPeriodo] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_CurriculoPeriodo]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_Curso]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_Curso] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_Curso]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_Docente]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_Docente] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_Docente]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_TipoCurriculoPeriodo]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_TipoCurriculoPeriodo] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_TipoCurriculoPeriodo]
GO
/****** Object:  Synonym [dbo].[SGP_ACA_TipoTurno]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ACA_TipoTurno] FOR [GestaoAvaliacao_SGP].[dbo].[ACA_TipoTurno]
GO
/****** Object:  Synonym [dbo].[SGP_ESC_Escola]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_ESC_Escola] FOR [GestaoAvaliacao_SGP].[dbo].[ESC_Escola]
GO
/****** Object:  Synonym [dbo].[SGP_MTR_MatriculaTurma]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_MTR_MatriculaTurma] FOR [GestaoAvaliacao_SGP].[dbo].[MTR_MatriculaTurma]
GO
/****** Object:  Synonym [dbo].[SGP_SYS_UnidadeAdministrativa]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_SYS_UnidadeAdministrativa] FOR [GestaoAvaliacao_SGP].[dbo].[SYS_UnidadeAdministrativa]
GO
/****** Object:  Synonym [dbo].[SGP_TUR_Turma]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_TUR_Turma] FOR [GestaoAvaliacao_SGP].[dbo].[TUR_Turma]
GO
/****** Object:  Synonym [dbo].[SGP_TUR_TurmaCurriculo]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_TUR_TurmaCurriculo] FOR [GestaoAvaliacao_SGP].[dbo].[TUR_TurmaCurriculo]
GO
/****** Object:  Synonym [dbo].[SGP_TUR_TurmaDisciplina]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_TUR_TurmaDisciplina] FOR [GestaoAvaliacao_SGP].[dbo].[TUR_TurmaDisciplina]
GO
/****** Object:  Synonym [dbo].[SGP_TUR_TurmaDocente]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_TUR_TurmaDocente] FOR [GestaoAvaliacao_SGP].[dbo].[TUR_TurmaDocente]
GO
/****** Object:  Synonym [dbo].[SGP_TUR_TurmaTipoCurriculoPeriodo]    Script Date: 02/05/2017 09:51:15 ******/
CREATE SYNONYM [dbo].[SGP_TUR_TurmaTipoCurriculoPeriodo] FOR [GestaoAvaliacao_SGP].[dbo].[TUR_TurmaTipoCurriculoPeriodo]
GO
/****** Object:  Synonym [dbo].[Synonym_Core_PES_Pessoa]    Script Date: 02/05/2017 09:51:16 ******/
CREATE SYNONYM [dbo].[Synonym_Core_PES_Pessoa] FOR [CoreSSO].[dbo].[PES_Pessoa]
GO
/****** Object:  Synonym [dbo].[Synonym_Core_SYS_Grupo]    Script Date: 02/05/2017 09:51:16 ******/
CREATE SYNONYM [dbo].[Synonym_Core_SYS_Grupo] FOR [CoreSSO].[dbo].[SYS_Grupo]
GO
/****** Object:  Synonym [dbo].[Synonym_Core_SYS_Usuario]    Script Date: 02/05/2017 09:51:16 ******/
CREATE SYNONYM [dbo].[Synonym_Core_SYS_Usuario] FOR [CoreSSO].[dbo].[SYS_Usuario]
GO
/****** Object:  Synonym [dbo].[Synonym_Core_SYS_UsuarioGrupo]    Script Date: 02/05/2017 09:51:16 ******/
CREATE SYNONYM [dbo].[Synonym_Core_SYS_UsuarioGrupo] FOR [CoreSSO].[dbo].[SYS_UsuarioGrupo]
GO
/****** Object:  Synonym [dbo].[Synonym_Core_SYS_UsuarioGrupoUA]    Script Date: 02/05/2017 09:51:16 ******/
CREATE SYNONYM [dbo].[Synonym_Core_SYS_UsuarioGrupoUA] FOR [CoreSSO].[dbo].[SYS_UsuarioGrupoUA]
GO
/****** Object:  UserDefinedFunction [dbo].[GetUserSection]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Luís Maron>
-- Create date: <11/12/2015>
-- Description:	<Retorna as turmas que um usuário tem acesso>
-- =============================================
CREATE FUNCTION [dbo].[GetUserSection]
(	
	@gru_id UNIQUEIDENTIFIER,
	@usu_id UNIQUEIDENTIFIER, 
	@pes_id UNIQUEIDENTIFIER, 
	@ent_id UNIQUEIDENTIFIER, 
	@vis_id INT,
	@state INT,
	@esc_id INT = NULL,
	@uad_id UNIQUEIDENTIFIER = NULL,
	@ttn_id INT = NULL,
	@tne_id INT = NULL,
	@crp_ordem INT = NULL
)
RETURNS @tur_ids TABLE 
(
	tur_id BIGINT
)
AS
BEGIN
	IF(@vis_id = 2)
	BEGIN	
		INSERT INTO @tur_ids
		select t.tur_id		
		from Synonym_Core_SYS_UsuarioGrupo ug (NOLOCK)
		INNER JOIN Synonym_Core_SYS_UsuarioGrupoUA ua (NOLOCK) ON ug.gru_id = ua.gru_id AND ug.usu_id = ua.usu_id
		INNER JOIN SGP_ESC_Escola e (NOLOCK) ON e.uad_idSuperiorGestao = ua.uad_id AND e.esc_situacao = @state
		INNER JOIN SGP_TUR_Turma t (NOLOCK) ON t.esc_id = e.esc_id AND t.tur_situacao = @state
		INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp (NOLOCK) ON ttcp.tur_id = t.tur_id
		where ug.usu_id = @usu_id and ug.gru_id = @gru_id AND ug.usg_situacao = @state 
			AND e.esc_id = ISNULL(@esc_id, e.esc_id) AND e.uad_idSuperiorGestao = ISNULL(@uad_id, e.uad_idSuperiorGestao)
			AND t.ttn_id = ISNULL(@ttn_id, t.ttn_id) AND ttcp.tne_id = ISNULL(@tne_id, ttcp.tne_id) AND ttcp.crp_ordem = ISNULL(@crp_ordem, ttcp.crp_ordem)
	END

	ELSE IF (@vis_id = 3)
	BEGIN
		INSERT INTO @tur_ids
		select t.tur_id
		from Synonym_Core_SYS_UsuarioGrupo ug (NOLOCK)
		INNER JOIN Synonym_Core_SYS_UsuarioGrupoUA ua (NOLOCK) ON ug.gru_id = ua.gru_id AND ug.usu_id = ua.usu_id
		INNER JOIN SGP_ESC_Escola e (NOLOCK) ON e.uad_id = ua.uad_id AND e.esc_situacao = @state
		INNER JOIN SGP_TUR_Turma t (NOLOCK) ON t.esc_id = e.esc_id AND t.tur_situacao = @state
		INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp (NOLOCK) ON ttcp.tur_id = t.tur_id
		where ug.usu_id = @usu_id and ug.gru_id = @gru_id AND ug.usg_situacao = @state
			AND e.esc_id = ISNULL(@esc_id, e.esc_id) AND e.uad_idSuperiorGestao = ISNULL(@uad_id, e.uad_idSuperiorGestao)
			AND t.ttn_id = ISNULL(@ttn_id, t.ttn_id) AND ttcp.tne_id = ISNULL(@tne_id, ttcp.tne_id) AND ttcp.crp_ordem = ISNULL(@crp_ordem, ttcp.crp_ordem)
	END
	ELSE IF(@vis_id = 4)
	BEGIN
		INSERT INTO @tur_ids
		SELECT DISTINCT tud.tur_id
		FROM SGP_ACA_Docente d (NOLOCK)
		INNER JOIN SGP_TUR_TurmaDocente td (NOLOCK) ON d.doc_id = td.doc_id AND td.tdt_situacao = @state
		INNER JOIN SGP_TUR_TurmaDisciplina tud (NOLOCK) ON tud.tud_id = td.tud_id AND tud.tud_situacao = @state
		INNER JOIN SGP_TUR_Turma t (NOLOCK) ON t.tur_id = tud.tur_id
		INNER JOIN SGP_ESC_Escola e (NOLOCK) ON e.esc_id = t.esc_id
		INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp (NOLOCK) ON ttcp.tur_id = t.tur_id
		WHERE d.pes_id = @pes_id AND d.ent_id = @ent_id AND d.doc_situacao = @state 
			AND e.esc_id = ISNULL(@esc_id, e.esc_id) AND e.uad_idSuperiorGestao = ISNULL(@uad_id, e.uad_idSuperiorGestao)
			AND t.ttn_id = ISNULL(@ttn_id, t.ttn_id) AND ttcp.tne_id = ISNULL(@tne_id, ttcp.tne_id) AND ttcp.crp_ordem = ISNULL(@crp_ordem, ttcp.crp_ordem)
	END
	RETURN
END


GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString] (@String nvarchar(4000), @Delimiter char(1))
RETURNS @Results TABLE (
  Items nvarchar(4000)
)
AS
BEGIN
  DECLARE @Index int
  DECLARE @Slice nvarchar(4000)

  SELECT
    @Index = 1
  IF @String IS NULL
    RETURN

  WHILE @Index != 0
  BEGIN
    SELECT
      @Index = CHARINDEX(@Delimiter, @String)
    IF @Index <> 0

      SELECT
        @Slice = LEFT(@String, @Index - 1)

    ELSE

      SELECT
        @Slice = @String
    INSERT INTO @Results (Items)
      VALUES (@Slice)
    SELECT
      @String = RIGHT(@String, LEN(@String) - @Index)

    IF LEN(@String) = 0
      BREAK

  END
  RETURN
END


GO
/****** Object:  Table [dbo].[AbsenceReason]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AbsenceReason](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[AllowRetry] [bit] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.AbsenceReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Adherence]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Adherence](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityId] [bigint] NOT NULL,
	[ParentId] [int] NULL,
	[TypeEntity] [tinyint] NOT NULL,
	[TypeSelection] [tinyint] NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.Adherence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Alternative]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Alternative](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL,
	[Correct] [bit] NULL,
	[Order] [int] NULL,
	[Justificative] [varchar](max) NULL,
	[Numeration] [varchar](10) NULL,
	[TCTBiserialCoefficient] [decimal](9, 3) NULL,
	[TCTDificulty] [decimal](9, 3) NULL,
	[TCTDiscrimination] [decimal](9, 3) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Item_Id] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Alternative] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AnswerSheetBatch]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AnswerSheetBatch](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[SupAdmUnit_Id] [uniqueidentifier] NULL,
	[School_Id] [int] NULL,
	[Section_Id] [bigint] NULL,
	[CreatedBy_Id] [uniqueidentifier] NULL,
	[Processing] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[BatchType] [tinyint] NOT NULL,
	[OwnerEntity] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.AnswerSheetBatch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AnswerSheetBatchFiles]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnswerSheetBatchFiles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[File_Id] [bigint] NOT NULL,
	[AnswerSheetBatch_Id] [bigint] NULL,
	[Student_Id] [bigint] NULL,
	[Sent] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Section_Id] [bigint] NULL,
	[SupAdmUnit_Id] [uniqueidentifier] NULL,
	[School_Id] [int] NULL,
	[Situation] [tinyint] NOT NULL,
	[AnswerSheetBatchQueue_Id] [bigint] NULL,
	[CreatedBy_Id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.AnswerSheetBatchFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AnswerSheetBatchLog]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AnswerSheetBatchLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AnswerSheetBatchFile_Id] [bigint] NOT NULL,
	[Situation] [tinyint] NOT NULL,
	[Description] [varchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[File_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.AnswerSheetBatchLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AnswerSheetBatchQueue]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AnswerSheetBatchQueue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[File_Id] [bigint] NOT NULL,
	[AnswerSheetBatch_Id] [bigint] NULL,
	[SupAdmUnit_Id] [uniqueidentifier] NULL,
	[School_Id] [int] NULL,
	[CountFiles] [int] NULL,
	[Situation] [tinyint] NOT NULL,
	[Description] [varchar](max) NULL,
	[CreatedBy_Id] [uniqueidentifier] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.AnswerSheetBatchQueue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AnswerSheetLot]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AnswerSheetLot](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Test_Id] [bigint] NULL,
	[StateExecution] [tinyint] NOT NULL,
	[uad_id] [uniqueidentifier] NULL,
	[esc_id] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[RequestDate] [datetime] NULL,
	[Parent_Id] [bigint] NULL,
	[ExecutionOwner] [varchar](1000) NULL,
 CONSTRAINT [PK_dbo.AnswerSheetLot] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BaseText]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BaseText](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Source] [varchar](max) NULL,
	[InitialOrientation] [nvarchar](500) NULL,
	[InitialStatement] [nvarchar](300) NULL,
	[NarrationInitialStatement] [bit] NULL,
	[StudentBaseText] [bit] NULL,
	[NarrationStudentBaseText] [bit] NULL,
	[BaseTextOrientation] [nvarchar](300) NULL,
 CONSTRAINT [PK_dbo.BaseText] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Block]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Block](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](10) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Booklet_Id] [bigint] NULL,
	[Test_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.Block] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BlockItem]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlockItem](
	[Block_Id] [bigint] NOT NULL,
	[Item_Id] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.BlockItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Booklet]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booklet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Test_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.Booklet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CognitiveCompetence]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CognitiveCompetence](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.CognitiveCompetence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CorrelatedSkill]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorrelatedSkill](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Skill1_Id] [bigint] NULL,
	[Skill2_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.CorrelatedSkill] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Discipline]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Discipline](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[DisciplineTypeId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[TypeLevelEducationId] [int] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.Discipline] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EvaluationMatrix]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EvaluationMatrix](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[Edition] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Discipline_Id] [bigint] NULL,
	[ModelEvaluationMatrix_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.EvaluationMatrix] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EvaluationMatrixCourse]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EvaluationMatrixCourse](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EvaluationMatrix_Id] [bigint] NOT NULL,
	[TypeLevelEducationId] [int] NOT NULL,
	[ModalityId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.EvaluationMatrixCourse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EvaluationMatrixCourseCurriculumGrade]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EvaluationMatrixCourseCurriculumGrade](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CurriculumGradeId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EvaluationMatrixCourse_Id] [bigint] NOT NULL,
	[TypeCurriculumGradeId] [int] NOT NULL,
	[Ordem] [int] NOT NULL,
 CONSTRAINT [PK_dbo.EvaluationMatrixCourseCurriculumGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExportAnalysis]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExportAnalysis](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[StateExecution] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ExportAnalysis] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[File]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[File](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Path] [varchar](500) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[ContentType] [varchar](500) NOT NULL,
	[OwnerId] [bigint] NULL,
	[OwnerType] [tinyint] NULL,
	[ParentOwnerId] [bigint] NULL,
	[OriginalName] [nvarchar](max) NULL,
	[CreatedBy_Id] [uniqueidentifier] NULL,
	[DeletedBy_Id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.File] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FormatType]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FormatType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.FormatType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Item]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Item](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemCode] [varchar](32) NOT NULL,
	[ItemVersion] [int] NOT NULL,
	[Statement] [varchar](max) NULL,
	[Keywords] [varchar](max) NULL,
	[Tips] [varchar](max) NULL,
	[TRIDiscrimination] [decimal](9, 3) NULL,
	[TRIDifficulty] [decimal](9, 3) NULL,
	[TRICasualSetting] [decimal](9, 3) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[BaseText_Id] [bigint] NULL,
	[EvaluationMatrix_Id] [bigint] NOT NULL,
	[ItemLevel_Id] [bigint] NULL,
	[ItemSituation_Id] [bigint] NOT NULL,
	[ItemType_Id] [bigint] NOT NULL,
	[proficiency] [int] NULL,
	[descriptorSentence] [varchar](170) NULL,
	[LastVersion] [bit] NULL,
	[IsRestrict] [bit] NOT NULL,
	[ItemNarrated] [bit] NULL,
	[StudentStatement] [bit] NULL,
	[NarrationStudentStatement] [bit] NULL,
	[NarrationAlternatives] [bit] NULL,
	[Revoked] [bit] NULL,
	[ItemCodeVersion] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Item] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemCurriculumGrade]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemCurriculumGrade](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Item_Id] [bigint] NULL,
	[TypeCurriculumGradeId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ItemCurriculumGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[Value] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.ItemLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemSituation]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemSituation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[AllowVersion] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ItemSituation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemSkill]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSkill](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OriginalSkill] [bit] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Item_Id] [bigint] NOT NULL,
	[Skill_Id] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.ItemSkill] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemType]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[UniqueAnswer] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[IsVisibleTestType] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[QuantityAlternative] [int] NULL,
 CONSTRAINT [PK_dbo.ItemType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModelEvaluationMatrix]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ModelEvaluationMatrix](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ModelEvaluationMatrix] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModelSkillLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ModelSkillLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[Level] [int] NOT NULL,
	[LastLevel] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[ModelEvaluationMatrix_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.ModelSkillLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModelTest]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModelTest](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DefaultModel] [bit] NOT NULL,
	[ShowCoverPage] [bit] NOT NULL,
	[ShowBorder] [bit] NOT NULL,
	[LogoHeaderPosition] [int] NOT NULL,
	[LogoHeaderSize] [int] NOT NULL,
	[LogoHeaderWaterMark] [bit] NOT NULL,
	[MessageHeaderPosition] [int] NOT NULL,
	[ShowMessageHeader] [bit] NOT NULL,
	[MessageHeader] [text] NULL,
	[MessageHeaderWaterMark] [bit] NOT NULL,
	[ShowLineBelowHeader] [bit] NOT NULL,
	[ShowLogoHeader] [bit] NOT NULL,
	[FileHeader_Id] [bigint] NULL,
	[HeaderHtml] [text] NULL,
	[LogoFooterPosition] [int] NOT NULL,
	[LogoFooterSize] [int] NOT NULL,
	[LogoFooterWaterMark] [bit] NOT NULL,
	[MessageFooterPosition] [int] NOT NULL,
	[ShowMessageFooter] [bit] NOT NULL,
	[MessageFooter] [text] NULL,
	[MessageFooterWaterMark] [bit] NOT NULL,
	[ShowLineAboveFooter] [bit] NOT NULL,
	[ShowLogoFooter] [bit] NOT NULL,
	[FileFooter_Id] [bigint] NULL,
	[FooterHtml] [text] NULL,
	[ShowSchool] [bit] NOT NULL,
	[ShowStudentName] [bit] NOT NULL,
	[ShowTeacherName] [bit] NOT NULL,
	[ShowClassName] [bit] NOT NULL,
	[ShowStudentNumber] [bit] NOT NULL,
	[ShowDate] [bit] NOT NULL,
	[ShowLineBelowStudentInformation] [bit] NOT NULL,
	[StudentInformationHtml] [text] NULL,
	[CoverPageText] [text] NULL,
	[ShowStudentInformationsOnCoverPage] [bit] NOT NULL,
	[ShowHeaderOnCoverPage] [bit] NOT NULL,
	[ShowFooterOnCoverPage] [bit] NOT NULL,
	[ShowBorderOnCoverPage] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[ShowItemLine] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ModelTest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Parameter]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Parameter](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Key] [varchar](100) NOT NULL,
	[Value] [varchar](300) NOT NULL,
	[Description] [varchar](200) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[Obligatory] [bit] NULL,
	[Versioning] [bit] NULL,
	[ParameterCategory_Id] [bigint] NOT NULL,
	[ParameterPage_Id] [bigint] NOT NULL,
	[ParameterType_Id] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Parameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParameterCategory]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParameterCategory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ParameterCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParameterPage]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParameterPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[pageVersioning] [bit] NOT NULL,
	[pageObligatory] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ParameterPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParameterType]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParameterType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.ParameterType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PerformanceLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PerformanceLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Code] [varchar](15) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.PerformanceLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RequestRevoke]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RequestRevoke](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UsuId] [uniqueidentifier] NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[Situation] [int] NOT NULL,
	[Justification] [varchar](500) NOT NULL,
	[BlockItem_Id] [bigint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.RequestRevoke] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Skill]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Skill](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[Code] [varchar](500) NOT NULL,
	[LastLevel] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[EvaluationMatrix_Id] [bigint] NULL,
	[ModelSkillLevel_Id] [bigint] NULL,
	[Parent_Id] [bigint] NULL,
	[CognitiveCompetence_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.Skill] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StudentTestAbsenceReason]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentTestAbsenceReason](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[alu_id] [bigint] NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[tur_id] [bigint] NOT NULL,
	[AbsenceReason_Id] [bigint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.StudentTestAbsenceReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Test]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Test](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Bib] [bit] NOT NULL,
	[TestType_Id] [bigint] NOT NULL,
	[NumberItemsBlock] [int] NOT NULL,
	[Discipline_Id] [bigint] NULL,
	[NumberBlock] [int] NOT NULL,
	[NumberItem] [int] NULL,
	[ApplicationStartDate] [datetime] NOT NULL,
	[ApplicationEndDate] [datetime] NOT NULL,
	[FormatType_Id] [bigint] NOT NULL,
	[CorrectionStartDate] [datetime] NOT NULL,
	[CorrectionEndDate] [datetime] NOT NULL,
	[UsuId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[AllAdhered] [bit] NOT NULL,
	[TestSituation] [int] NOT NULL,
	[PublicFeedback] [bit] NOT NULL,
	[ProcessedCorrection] [bit] NOT NULL,
	[ProcessedCorrectionDate] [datetime] NULL,
	[Visible] [bit] NOT NULL,
	[FrequencyApplication] [int] NOT NULL,
	[Multidiscipline] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Test] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestCurriculumGrade]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestCurriculumGrade](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TypeCurriculumGradeId] [bigint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[Test_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.TestCurriculumGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestFiles]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestFiles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[File_Id] [bigint] NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TestFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestItemLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestItemLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[ItemLevel_Id] [bigint] NULL,
	[Test_Id] [bigint] NULL,
	[PercentValue] [int] NULL,
 CONSTRAINT [PK_dbo.TestItemLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestPerformanceLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestPerformanceLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [int] NOT NULL,
	[Value1] [int] NULL,
	[Value2] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[PerformanceLevel_Id] [bigint] NULL,
	[Test_Id] [bigint] NULL,
 CONSTRAINT [PK_dbo.TestPerformanceLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestPermission]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestPermission](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[gru_id] [uniqueidentifier] NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[AllowAnswer] [bit] NOT NULL,
	[ShowResult] [bit] NOT NULL,
	[TestHide] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TestPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestSectionStatusCorrection]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestSectionStatusCorrection](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Test_Id] [bigint] NOT NULL,
	[tur_id] [bigint] NOT NULL,
	[StatusCorrection] [tinyint] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_dbo.TestSectionStatusCorrection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestType]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[AnswerSheet_Id] [bigint] NOT NULL,
	[FormatType_Id] [bigint] NULL,
	[Bib] [bit] NOT NULL,
	[Global] [bit] NOT NULL,
	[TypeLevelEducationId] [int] NOT NULL,
	[ItemType_Id] [bigint] NULL,
	[ModelTest_Id] [bigint] NULL,
	[FrequencyApplication] [int] NOT NULL,
 CONSTRAINT [PK_dbo.TestType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestTypeCourse]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestTypeCourse](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[TestType_Id] [bigint] NOT NULL,
	[ModalityId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.TestTypeCourse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestTypeCourseCurriculumGrade]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestTypeCourseCurriculumGrade](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CurriculumGradeId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[TestTypeCourse_Id] [bigint] NOT NULL,
	[TypeCurriculumGradeId] [int] NOT NULL,
	[Ordem] [int] NOT NULL,
 CONSTRAINT [PK_dbo.TestTypeCourseCurriculumGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestTypeItemLevel]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestTypeItemLevel](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[State] [tinyint] NOT NULL,
	[ItemLevel_Id] [bigint] NOT NULL,
	[TestType_Id] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.TestTypeItemLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[GetTestAdhered]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Luís Henrique Pupo Maron>
-- Create date: <06/01/2016>
-- Description:	<Seleciona os testes que estão aderidos conforme os parâmetros passados>
-- =============================================
CREATE FUNCTION [dbo].[GetTestAdhered]
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
/****** Object:  UserDefinedFunction [dbo].[TestsByUser]    Script Date: 02/05/2017 09:51:16 ******/
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


GO
/****** Object:  View [dbo].[View_Files]    Script Date: 02/05/2017 09:51:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[View_Files] AS

WITH BatchFiles AS (
SELECT [Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id],[Sent],[CreateDate],[UpdateDate],[State]
,[Section_Id],[SupAdmUnit_Id],[School_Id],[Situation],[CreatedBy_Id],[AnswerSheetBatchQueue_Id] 
,ROW_NUMBER() OVER(PARTITION BY AnswerSheetBatch_Id, SupAdmUnit_Id, School_Id, Section_Id, Student_Id 
ORDER BY UpdateDate DESC) AS RowNumber 
FROM AnswerSheetBatchFiles WITH(NOLOCK)
WHERE AnswerSheetBatch_Id IS NOT NULL AND Student_Id IS NOT NULL 
--AND Section_Id = @turmaId 

),

BatchFilesDistinct AS (
SELECT 
[Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id],[Sent],[CreateDate],[UpdateDate],[State]
,[Section_Id],[SupAdmUnit_Id],[School_Id],[Situation],[CreatedBy_Id],[AnswerSheetBatchQueue_Id] 
FROM BatchFiles WITH(NOLOCK)
WHERE RowNumber = 1
)

SELECT distinct A.Test_Id, B.AnswerSheetBatch_Id, B.student_id, B.School_Id, B.supadmunit_id--, COUNT(Situation) 
FROM BatchFilesDistinct B
INNER JOIN AnswerSheetBatch A ON B.AnswerSheetBatch_Id = A.Id
where B.CreateDate > '2017-01-03 14:41:53.897'
--GROUP BY A.Test_Id, B.AnswerSheetBatch_Id




GO
ALTER TABLE [dbo].[AbsenceReason] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[AnswerSheetBatch] ADD  DEFAULT ((0)) FOR [BatchType]
GO
ALTER TABLE [dbo].[AnswerSheetBatch] ADD  DEFAULT ((0)) FOR [OwnerEntity]
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] ADD  DEFAULT ((0)) FOR [Situation]
GO
ALTER TABLE [dbo].[AnswerSheetLot] ADD  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[AnswerSheetLot] ADD  DEFAULT ((0)) FOR [Parent_Id]
GO
ALTER TABLE [dbo].[Item] ADD  DEFAULT ('') FOR [descriptorSentence]
GO
ALTER TABLE [dbo].[Item] ADD  DEFAULT ((0)) FOR [IsRestrict]
GO
ALTER TABLE [dbo].[Item] ADD  DEFAULT ((0)) FOR [ItemCodeVersion]
GO
ALTER TABLE [dbo].[ModelTest] ADD  DEFAULT ((0)) FOR [ShowItemLine]
GO
ALTER TABLE [dbo].[Parameter] ADD  DEFAULT ((0)) FOR [ParameterCategory_Id]
GO
ALTER TABLE [dbo].[Parameter] ADD  DEFAULT ((0)) FOR [ParameterPage_Id]
GO
ALTER TABLE [dbo].[Parameter] ADD  DEFAULT ((0)) FOR [ParameterType_Id]
GO
ALTER TABLE [dbo].[ParameterPage] ADD  DEFAULT ((0)) FOR [pageVersioning]
GO
ALTER TABLE [dbo].[ParameterPage] ADD  DEFAULT ((0)) FOR [pageObligatory]
GO
ALTER TABLE [dbo].[Test] ADD  DEFAULT ((0)) FOR [PublicFeedback]
GO
ALTER TABLE [dbo].[Test] ADD  DEFAULT ((0)) FOR [ProcessedCorrection]
GO
ALTER TABLE [dbo].[Test] ADD  DEFAULT ((1)) FOR [Visible]
GO
ALTER TABLE [dbo].[Test] ADD  DEFAULT ((1)) FOR [FrequencyApplication]
GO
ALTER TABLE [dbo].[Test] ADD  DEFAULT ((0)) FOR [Multidiscipline]
GO
ALTER TABLE [dbo].[TestType] ADD  DEFAULT ((0)) FOR [Bib]
GO
ALTER TABLE [dbo].[TestType] ADD  DEFAULT ((0)) FOR [Global]
GO
ALTER TABLE [dbo].[TestType] ADD  DEFAULT ((0)) FOR [TypeLevelEducationId]
GO
ALTER TABLE [dbo].[TestType] ADD  DEFAULT ((1)) FOR [FrequencyApplication]
GO
ALTER TABLE [dbo].[TestTypeCourse] ADD  DEFAULT ((0)) FOR [ModalityId]
GO
ALTER TABLE [dbo].[Adherence]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Adherence_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Adherence] CHECK CONSTRAINT [FK_dbo.Adherence_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[Alternative]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Alternative_dbo.Item_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Item] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Alternative] CHECK CONSTRAINT [FK_dbo.Alternative_dbo.Item_Item_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatch]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatch_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AnswerSheetBatch] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatch_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id] FOREIGN KEY([AnswerSheetBatch_Id])
REFERENCES [dbo].[AnswerSheetBatch] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatch_AnswerSheetBatch_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatchQueue_AnswerSheetBatchQueue_Id] FOREIGN KEY([AnswerSheetBatchQueue_Id])
REFERENCES [dbo].[AnswerSheetBatchQueue] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.AnswerSheetBatchQueue_AnswerSheetBatchQueue_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.File_File_Id] FOREIGN KEY([File_Id])
REFERENCES [dbo].[File] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AnswerSheetBatchFiles] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchFiles_dbo.File_File_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchLog]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchLog_dbo.AnswerSheetBatchFiles_AnswerSheetBatchFile_Id] FOREIGN KEY([AnswerSheetBatchFile_Id])
REFERENCES [dbo].[AnswerSheetBatchFiles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AnswerSheetBatchLog] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchLog_dbo.AnswerSheetBatchFiles_AnswerSheetBatchFile_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchLog]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchLog_dbo.File_File_Id] FOREIGN KEY([File_Id])
REFERENCES [dbo].[File] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetBatchLog] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchLog_dbo.File_File_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchQueue]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.AnswerSheetBatch_AnswerSheetBatch_Id] FOREIGN KEY([AnswerSheetBatch_Id])
REFERENCES [dbo].[AnswerSheetBatch] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetBatchQueue] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.AnswerSheetBatch_AnswerSheetBatch_Id]
GO
ALTER TABLE [dbo].[AnswerSheetBatchQueue]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.File_File_Id] FOREIGN KEY([File_Id])
REFERENCES [dbo].[File] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AnswerSheetBatchQueue] CHECK CONSTRAINT [FK_dbo.AnswerSheetBatchQueue_dbo.File_File_Id]
GO
ALTER TABLE [dbo].[AnswerSheetLot]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.AnswerSheetLot_Parent_Id] FOREIGN KEY([Parent_Id])
REFERENCES [dbo].[AnswerSheetLot] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetLot] CHECK CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.AnswerSheetLot_Parent_Id]
GO
ALTER TABLE [dbo].[AnswerSheetLot]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[AnswerSheetLot] CHECK CONSTRAINT [FK_dbo.AnswerSheetLot_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[Block]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Block_dbo.Booklet_Booklet_Id] FOREIGN KEY([Booklet_Id])
REFERENCES [dbo].[Booklet] ([Id])
GO
ALTER TABLE [dbo].[Block] CHECK CONSTRAINT [FK_dbo.Block_dbo.Booklet_Booklet_Id]
GO
ALTER TABLE [dbo].[Block]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Block_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[Block] CHECK CONSTRAINT [FK_dbo.Block_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[BlockItem]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BlockItem_dbo.Block_Block_Id] FOREIGN KEY([Block_Id])
REFERENCES [dbo].[Block] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlockItem] CHECK CONSTRAINT [FK_dbo.BlockItem_dbo.Block_Block_Id]
GO
ALTER TABLE [dbo].[BlockItem]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BlockItem_dbo.Item_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Item] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlockItem] CHECK CONSTRAINT [FK_dbo.BlockItem_dbo.Item_Item_Id]
GO
ALTER TABLE [dbo].[Booklet]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Booklet_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[Booklet] CHECK CONSTRAINT [FK_dbo.Booklet_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[CorrelatedSkill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CorrelatedSkill_dbo.Skill_Skill1_Id] FOREIGN KEY([Skill1_Id])
REFERENCES [dbo].[Skill] ([Id])
GO
ALTER TABLE [dbo].[CorrelatedSkill] CHECK CONSTRAINT [FK_dbo.CorrelatedSkill_dbo.Skill_Skill1_Id]
GO
ALTER TABLE [dbo].[CorrelatedSkill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CorrelatedSkill_dbo.Skill_Skill2_Id] FOREIGN KEY([Skill2_Id])
REFERENCES [dbo].[Skill] ([Id])
GO
ALTER TABLE [dbo].[CorrelatedSkill] CHECK CONSTRAINT [FK_dbo.CorrelatedSkill_dbo.Skill_Skill2_Id]
GO
ALTER TABLE [dbo].[EvaluationMatrix]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EvaluationMatrix_dbo.Discipline_Discipline_Id] FOREIGN KEY([Discipline_Id])
REFERENCES [dbo].[Discipline] ([Id])
GO
ALTER TABLE [dbo].[EvaluationMatrix] CHECK CONSTRAINT [FK_dbo.EvaluationMatrix_dbo.Discipline_Discipline_Id]
GO
ALTER TABLE [dbo].[EvaluationMatrix]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EvaluationMatrix_dbo.ModelEvaluationMatrix_ModelEvaluationMatrix_Id] FOREIGN KEY([ModelEvaluationMatrix_Id])
REFERENCES [dbo].[ModelEvaluationMatrix] ([Id])
GO
ALTER TABLE [dbo].[EvaluationMatrix] CHECK CONSTRAINT [FK_dbo.EvaluationMatrix_dbo.ModelEvaluationMatrix_ModelEvaluationMatrix_Id]
GO
ALTER TABLE [dbo].[EvaluationMatrixCourse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EvaluationMatrixCourse_dbo.EvaluationMatrix_EvaluationMatrix_Id] FOREIGN KEY([EvaluationMatrix_Id])
REFERENCES [dbo].[EvaluationMatrix] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EvaluationMatrixCourse] CHECK CONSTRAINT [FK_dbo.EvaluationMatrixCourse_dbo.EvaluationMatrix_EvaluationMatrix_Id]
GO
ALTER TABLE [dbo].[EvaluationMatrixCourseCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EvaluationMatrixCourseCurriculumGrade_dbo.EvaluationMatrixCourse_EvaluationMatrixCourse_Id] FOREIGN KEY([EvaluationMatrixCourse_Id])
REFERENCES [dbo].[EvaluationMatrixCourse] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EvaluationMatrixCourseCurriculumGrade] CHECK CONSTRAINT [FK_dbo.EvaluationMatrixCourseCurriculumGrade_dbo.EvaluationMatrixCourse_EvaluationMatrixCourse_Id]
GO
ALTER TABLE [dbo].[ExportAnalysis]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ExportAnalysis_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExportAnalysis] CHECK CONSTRAINT [FK_dbo.ExportAnalysis_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Item_dbo.BaseText_BaseText_Id] FOREIGN KEY([BaseText_Id])
REFERENCES [dbo].[BaseText] ([Id])
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_dbo.Item_dbo.BaseText_BaseText_Id]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Item_dbo.EvaluationMatrix_EvaluationMatrix_Id] FOREIGN KEY([EvaluationMatrix_Id])
REFERENCES [dbo].[EvaluationMatrix] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_dbo.Item_dbo.EvaluationMatrix_EvaluationMatrix_Id]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Item_dbo.ItemLevel_ItemLevel_Id] FOREIGN KEY([ItemLevel_Id])
REFERENCES [dbo].[ItemLevel] ([Id])
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_dbo.Item_dbo.ItemLevel_ItemLevel_Id]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Item_dbo.ItemSituation_ItemSituation_Id] FOREIGN KEY([ItemSituation_Id])
REFERENCES [dbo].[ItemSituation] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_dbo.Item_dbo.ItemSituation_ItemSituation_Id]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Item_dbo.ItemType_ItemType_Id] FOREIGN KEY([ItemType_Id])
REFERENCES [dbo].[ItemType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_dbo.Item_dbo.ItemType_ItemType_Id]
GO
ALTER TABLE [dbo].[ItemCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemCurriculumGrade_dbo.Item_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Item] ([Id])
GO
ALTER TABLE [dbo].[ItemCurriculumGrade] CHECK CONSTRAINT [FK_dbo.ItemCurriculumGrade_dbo.Item_Item_Id]
GO
ALTER TABLE [dbo].[ItemSkill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemSkill_dbo.Item_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Item] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ItemSkill] CHECK CONSTRAINT [FK_dbo.ItemSkill_dbo.Item_Item_Id]
GO
ALTER TABLE [dbo].[ItemSkill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemSkill_dbo.Skill_Skill_Id] FOREIGN KEY([Skill_Id])
REFERENCES [dbo].[Skill] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ItemSkill] CHECK CONSTRAINT [FK_dbo.ItemSkill_dbo.Skill_Skill_Id]
GO
ALTER TABLE [dbo].[ModelSkillLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ModelSkillLevel_dbo.ModelEvaluationMatrix_ModelEvaluationMatrix_Id] FOREIGN KEY([ModelEvaluationMatrix_Id])
REFERENCES [dbo].[ModelEvaluationMatrix] ([Id])
GO
ALTER TABLE [dbo].[ModelSkillLevel] CHECK CONSTRAINT [FK_dbo.ModelSkillLevel_dbo.ModelEvaluationMatrix_ModelEvaluationMatrix_Id]
GO
ALTER TABLE [dbo].[ModelTest]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ModelTest_dbo.File_FileFooter_Id] FOREIGN KEY([FileFooter_Id])
REFERENCES [dbo].[File] ([Id])
GO
ALTER TABLE [dbo].[ModelTest] CHECK CONSTRAINT [FK_dbo.ModelTest_dbo.File_FileFooter_Id]
GO
ALTER TABLE [dbo].[ModelTest]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ModelTest_dbo.File_FileHeader_Id] FOREIGN KEY([FileHeader_Id])
REFERENCES [dbo].[File] ([Id])
GO
ALTER TABLE [dbo].[ModelTest] CHECK CONSTRAINT [FK_dbo.ModelTest_dbo.File_FileHeader_Id]
GO
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Parameter_dbo.ParameterCategory_ParameterCategory_Id] FOREIGN KEY([ParameterCategory_Id])
REFERENCES [dbo].[ParameterCategory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_dbo.Parameter_dbo.ParameterCategory_ParameterCategory_Id]
GO
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Parameter_dbo.ParameterPage_ParameterPage_Id] FOREIGN KEY([ParameterPage_Id])
REFERENCES [dbo].[ParameterPage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_dbo.Parameter_dbo.ParameterPage_ParameterPage_Id]
GO
ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Parameter_dbo.ParameterType_ParameterType_Id] FOREIGN KEY([ParameterType_Id])
REFERENCES [dbo].[ParameterType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_dbo.Parameter_dbo.ParameterType_ParameterType_Id]
GO
ALTER TABLE [dbo].[RequestRevoke]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RequestRevoke_dbo.BlockItem_BlockItem_Id] FOREIGN KEY([BlockItem_Id])
REFERENCES [dbo].[BlockItem] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RequestRevoke] CHECK CONSTRAINT [FK_dbo.RequestRevoke_dbo.BlockItem_BlockItem_Id]
GO
ALTER TABLE [dbo].[RequestRevoke]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RequestRevoke_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RequestRevoke] CHECK CONSTRAINT [FK_dbo.RequestRevoke_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Skill_dbo.CognitiveCompetence_CognitiveCompetence_Id] FOREIGN KEY([CognitiveCompetence_Id])
REFERENCES [dbo].[CognitiveCompetence] ([Id])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_dbo.Skill_dbo.CognitiveCompetence_CognitiveCompetence_Id]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Skill_dbo.EvaluationMatrix_EvaluationMatrix_Id] FOREIGN KEY([EvaluationMatrix_Id])
REFERENCES [dbo].[EvaluationMatrix] ([Id])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_dbo.Skill_dbo.EvaluationMatrix_EvaluationMatrix_Id]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Skill_dbo.ModelSkillLevel_ModelSkillLevel_Id] FOREIGN KEY([ModelSkillLevel_Id])
REFERENCES [dbo].[ModelSkillLevel] ([Id])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_dbo.Skill_dbo.ModelSkillLevel_ModelSkillLevel_Id]
GO
ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Skill_dbo.Skill_Parent_Id] FOREIGN KEY([Parent_Id])
REFERENCES [dbo].[Skill] ([Id])
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_dbo.Skill_dbo.Skill_Parent_Id]
GO
ALTER TABLE [dbo].[StudentTestAbsenceReason]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StudentTestAbsenceReason_dbo.AbsenceReason_AbsenceReason_Id] FOREIGN KEY([AbsenceReason_Id])
REFERENCES [dbo].[AbsenceReason] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentTestAbsenceReason] CHECK CONSTRAINT [FK_dbo.StudentTestAbsenceReason_dbo.AbsenceReason_AbsenceReason_Id]
GO
ALTER TABLE [dbo].[StudentTestAbsenceReason]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StudentTestAbsenceReason_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentTestAbsenceReason] CHECK CONSTRAINT [FK_dbo.StudentTestAbsenceReason_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Test_dbo.Discipline_Discipline_Id] FOREIGN KEY([Discipline_Id])
REFERENCES [dbo].[Discipline] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Test] CHECK CONSTRAINT [FK_dbo.Test_dbo.Discipline_Discipline_Id]
GO
ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Test_dbo.FormatType_FormatType_Id] FOREIGN KEY([FormatType_Id])
REFERENCES [dbo].[FormatType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Test] CHECK CONSTRAINT [FK_dbo.Test_dbo.FormatType_FormatType_Id]
GO
ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Test_dbo.TestType_TestType_Id] FOREIGN KEY([TestType_Id])
REFERENCES [dbo].[TestType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Test] CHECK CONSTRAINT [FK_dbo.Test_dbo.TestType_TestType_Id]
GO
ALTER TABLE [dbo].[TestCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestCurriculumGrade_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[TestCurriculumGrade] CHECK CONSTRAINT [FK_dbo.TestCurriculumGrade_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestFiles_dbo.File_File_Id] FOREIGN KEY([File_Id])
REFERENCES [dbo].[File] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestFiles] CHECK CONSTRAINT [FK_dbo.TestFiles_dbo.File_File_Id]
GO
ALTER TABLE [dbo].[TestFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestFiles_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestFiles] CHECK CONSTRAINT [FK_dbo.TestFiles_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestItemLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestItemLevel_dbo.ItemLevel_ItemLevel_Id] FOREIGN KEY([ItemLevel_Id])
REFERENCES [dbo].[ItemLevel] ([Id])
GO
ALTER TABLE [dbo].[TestItemLevel] CHECK CONSTRAINT [FK_dbo.TestItemLevel_dbo.ItemLevel_ItemLevel_Id]
GO
ALTER TABLE [dbo].[TestItemLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestItemLevel_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[TestItemLevel] CHECK CONSTRAINT [FK_dbo.TestItemLevel_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestPerformanceLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestPerformanceLevel_dbo.PerformanceLevel_PerformanceLevel_Id] FOREIGN KEY([PerformanceLevel_Id])
REFERENCES [dbo].[PerformanceLevel] ([Id])
GO
ALTER TABLE [dbo].[TestPerformanceLevel] CHECK CONSTRAINT [FK_dbo.TestPerformanceLevel_dbo.PerformanceLevel_PerformanceLevel_Id]
GO
ALTER TABLE [dbo].[TestPerformanceLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestPerformanceLevel_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
GO
ALTER TABLE [dbo].[TestPerformanceLevel] CHECK CONSTRAINT [FK_dbo.TestPerformanceLevel_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestPermission]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestPermission_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestPermission] CHECK CONSTRAINT [FK_dbo.TestPermission_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestSectionStatusCorrection]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestSectionStatusCorrection_dbo.Test_Test_Id] FOREIGN KEY([Test_Id])
REFERENCES [dbo].[Test] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestSectionStatusCorrection] CHECK CONSTRAINT [FK_dbo.TestSectionStatusCorrection_dbo.Test_Test_Id]
GO
ALTER TABLE [dbo].[TestType]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestType_dbo.FormatType_FormatType_Id] FOREIGN KEY([FormatType_Id])
REFERENCES [dbo].[FormatType] ([Id])
GO
ALTER TABLE [dbo].[TestType] CHECK CONSTRAINT [FK_dbo.TestType_dbo.FormatType_FormatType_Id]
GO
ALTER TABLE [dbo].[TestType]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestType_dbo.ItemType_ItemType_Id] FOREIGN KEY([ItemType_Id])
REFERENCES [dbo].[ItemType] ([Id])
GO
ALTER TABLE [dbo].[TestType] CHECK CONSTRAINT [FK_dbo.TestType_dbo.ItemType_ItemType_Id]
GO
ALTER TABLE [dbo].[TestType]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestType_dbo.ModelTest_ModelTest_Id] FOREIGN KEY([ModelTest_Id])
REFERENCES [dbo].[ModelTest] ([Id])
GO
ALTER TABLE [dbo].[TestType] CHECK CONSTRAINT [FK_dbo.TestType_dbo.ModelTest_ModelTest_Id]
GO
ALTER TABLE [dbo].[TestTypeCourse]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTypeCourse_dbo.TestType_TestType_Id] FOREIGN KEY([TestType_Id])
REFERENCES [dbo].[TestType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestTypeCourse] CHECK CONSTRAINT [FK_dbo.TestTypeCourse_dbo.TestType_TestType_Id]
GO
ALTER TABLE [dbo].[TestTypeCourseCurriculumGrade]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTypeCourseCurriculumGrade_dbo.TestTypeCourse_TestTypeCourse_Id] FOREIGN KEY([TestTypeCourse_Id])
REFERENCES [dbo].[TestTypeCourse] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestTypeCourseCurriculumGrade] CHECK CONSTRAINT [FK_dbo.TestTypeCourseCurriculumGrade_dbo.TestTypeCourse_TestTypeCourse_Id]
GO
ALTER TABLE [dbo].[TestTypeItemLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTypeItemLevel_dbo.ItemLevel_ItemLevel_Id] FOREIGN KEY([ItemLevel_Id])
REFERENCES [dbo].[ItemLevel] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestTypeItemLevel] CHECK CONSTRAINT [FK_dbo.TestTypeItemLevel_dbo.ItemLevel_ItemLevel_Id]
GO
ALTER TABLE [dbo].[TestTypeItemLevel]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TestTypeItemLevel_dbo.TestType_TestType_Id] FOREIGN KEY([TestType_Id])
REFERENCES [dbo].[TestType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestTypeItemLevel] CHECK CONSTRAINT [FK_dbo.TestTypeItemLevel_dbo.TestType_TestType_Id]
GO
/****** Object:  StoredProcedure [dbo].[MS_Block_SearchItems]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
CREATE PROCEDURE [dbo].[MS_Block_SearchItems]
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
/****** Object:  StoredProcedure [dbo].[MS_CorrelatedSkill_SELECTBY_EvaluationMatrixId]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Leticia Martin
-- Create date: 01/10/2014
-- Description:	Lista todas as habilidades correlacionadas entre as matrizes
-- =============================================
CREATE PROCEDURE [dbo].[MS_CorrelatedSkill_SELECTBY_EvaluationMatrixId]
    @MatrizId INT ,
    @pageSize INT ,
    @pageNumber INT ,
    @totalRecords INT OUTPUT
AS 
    BEGIN

        SELECT  @totalRecords = COUNT(DISTINCT ck.Id)
        FROM    CorrelatedSkill ck WITH ( NOLOCK )
                INNER JOIN Skill sk1 WITH ( NOLOCK ) ON ck.Skill1_Id = sk1.Id
                INNER JOIN Skill sk2 WITH ( NOLOCK ) ON ck.skill2_id = sk2.Id
                INNER JOIN EvaluationMatrix em1 WITH ( NOLOCK ) ON SK1.EvaluationMatrix_Id = em1.Id
                INNER JOIN EvaluationMatrix em2 WITH ( NOLOCK ) ON SK2.EvaluationMatrix_Id = em2.Id
        WHERE   ck.id IN (
                SELECT  cs.Id
                FROM    Skill s WITH ( NOLOCK )
                        INNER JOIN CorrelatedSkill cs WITH ( NOLOCK ) ON cs.Skill1_Id = s.Id OR cs.Skill2_Id = s.Id
                WHERE   s.EvaluationMatrix_Id = @MatrizId )
                AND sk1.State = 1
                AND sk2.State = 1
                AND ck.State = 1
                AND em1.State = 1
                AND em2.State = 1


        SELECT TOP ( @pageSize )
                Matriz1 ,
                UltimoNivel1 ,
                Matriz2 ,
                UltimoNivel2,
                Id
        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id ) AS RowNumber ,
                            Matriz1 ,
                            UltimoNivel1 ,
                            Matriz2 ,
                            UltimoNivel2,
                           Id 
                  FROM      ( SELECT    em1.Description AS Matriz1 ,
                                        sk1.Description AS UltimoNivel1 ,
                                        em2.Description AS Matriz2 ,
                                        sk2.Description AS UltimoNivel2 ,
                                        ck.Id
                              FROM      CorrelatedSkill ck WITH ( NOLOCK )
                                        INNER JOIN Skill sk1 WITH ( NOLOCK ) ON ck.Skill1_Id = sk1.Id
                                        INNER JOIN Skill sk2 WITH ( NOLOCK ) ON ck.skill2_id = sk2.Id
                                        INNER JOIN EvaluationMatrix em1 WITH ( NOLOCK ) ON SK1.EvaluationMatrix_Id = em1.Id
                                        INNER JOIN EvaluationMatrix em2 WITH ( NOLOCK ) ON SK2.EvaluationMatrix_Id = em2.Id
                              WHERE     ck.id IN (
                                        SELECT  cs.Id
                                        FROM    Skill s WITH ( NOLOCK )
                                                INNER JOIN CorrelatedSkill cs WITH ( NOLOCK ) ON cs.Skill1_Id = s.Id
                                        WHERE   s.EvaluationMatrix_Id = @MatrizId )
                                        AND sk1.State = 1
                                        AND sk2.State = 1
                                        AND ck.State = 1
                                        AND em1.State = 1
                                        AND em2.State = 1
                              UNION
                              SELECT    em2.Description AS Matriz1 ,
                                        sk2.Description AS UltimoNivel1 ,
                                        em1.Description AS Matriz2 ,
                                        sk1.Description AS UltimoNivel2 ,
                                        ck.Id
                              FROM      CorrelatedSkill ck WITH ( NOLOCK )
                                        INNER JOIN Skill sk1 WITH ( NOLOCK ) ON ck.Skill1_Id = sk1.Id
                                        INNER JOIN Skill sk2 WITH ( NOLOCK ) ON ck.skill2_id = sk2.Id
                                        INNER JOIN EvaluationMatrix em1 WITH ( NOLOCK ) ON SK1.EvaluationMatrix_Id = em1.Id
                                        INNER JOIN EvaluationMatrix em2 WITH ( NOLOCK ) ON SK2.EvaluationMatrix_Id = em2.Id
                              WHERE     ck.id IN (
                                        SELECT  cs.Id
                                        FROM    Skill s WITH ( NOLOCK )
												INNER JOIN CorrelatedSkill cs WITH ( NOLOCK ) ON cs.Skill2_Id = s.Id
                                        WHERE   s.EvaluationMatrix_Id = @MatrizId )
                                        AND sk1.State = 1
                                        AND sk2.State = 1
                                        AND ck.State = 1
                                        AND em1.State = 1
                                        AND em2.State = 1
                            ) AS tab
                ) AS tab1
        WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
        ORDER BY Matriz1 ,
                Matriz2 ,
                RowNumber
    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItem]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <14/10/2014>
-- Description:	<Lista a quantidade total de itens cadastrados e quantidade de itens por disciplina>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItem]
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT,
    @total INT = NULL
	
AS 
    BEGIN

        SELECT  @total = COUNT(item.Id)
        FROM    Item item WITH ( NOLOCK )
				INNER JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
				INNER JOIN [Discipline] dis WITH ( NOLOCK ) ON dis.Id = em.Discipline_Id
                LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.id = em.ModelEvaluationMatrix_Id
        WHERE item.State = 1
				AND em.State = 1
                AND mem.State = 1
				AND mem.EntityId = @EntityId
				AND (@typeLevelEducation IS NULL OR dis.TypeLevelEducationId = @typeLevelEducation)
				AND item.LastVersion = 1

        SELECT  @total AS TotalItem ,
                COUNT(item.Id) AS Total ,
                dis.Description AS Description
        FROM    [Item] item WITH ( NOLOCK )
                INNER JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
                LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.id = em.ModelEvaluationMatrix_Id
                INNER JOIN [Discipline] dis WITH ( NOLOCK ) ON dis.Id = em.Discipline_Id
        WHERE   item.state = 1
                AND em.State = 1
                AND dis.state = 1
                AND mem.State = 1
				AND (@typeLevelEducation IS NULL OR dis.TypeLevelEducationId = @typeLevelEducation)
                AND mem.EntityId = @EntityId
                AND item.LastVersion = 1
        GROUP BY dis.Description

    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemCurriculumGrade]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <14/10/2014>
-- Description:	<retorna quantidade de questões vinculadas aos periodos escolares>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemCurriculumGrade]
    @disciplina INT ,
    @situacao INT ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT
AS 
    BEGIN

  SELECT  Id ,
						SUM(total) AS Total
        FROM    ( 
        SELECT    icg.TypeCurriculumGradeId AS Id, COUNT(item.id) AS Total
                  FROM   [ItemCurriculumGrade] icg WITH ( NOLOCK )
                            LEFT JOIN [Item] item WITH ( NOLOCK ) ON icg.Item_Id = item.id AND item.LastVersion = 1
                            LEFT JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
                            LEFT JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
                            LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
                            LEFT JOIN [ItemSituation] its WITH ( NOLOCK ) ON its.Id = item.ItemSituation_Id
                  WHERE     ( @disciplina IS NULL
                              OR d.id = @disciplina
                            )
                            AND its.Id = @situacao
							AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
                            AND icg.state = 1
                            AND item.state = 1
                            AND em.state = 1
                            AND d.state = 1
                            AND mem.state = 1
                            AND mem.entityId = @EntityId
                  GROUP BY icg.TypeCurriculumGradeId
                  
                  UNION
                  SELECT    emcc.TypeCurriculumGradeId AS Id, 0
                  FROM     [EvaluationMatrixCourseCurriculumGrade] emcc WITH ( NOLOCK )
							LEFT JOIN [EvaluationMatrixCourse] emc WITH (NOLOCK) ON emc.id = emcc.EvaluationMatrixCourse_Id
							LEFT JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = emc.EvaluationMatrix_Id
							LEFT JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
							LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
                  WHERE     emcc.state = 1
                            AND emc.state = 1
                            AND em.state = 1
                            AND mem.state = 1
                            AND mem.entityId = @EntityId
							AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
                  GROUP BY  emcc.TypeCurriculumGradeId
                ) Tabela
        GROUP BY Id


    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemLevel]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <14/10/2014>
-- Description:	<Retorna total de questões de cada grau de dificuldade>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemLevel]
    @disciplina INT ,
    @situacao INT ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT ,
    @total DECIMAL = NULL
AS 
    BEGIN
        SELECT  @total = COUNT(item.Id)
        FROM   [ItemLevel] itlevel WITH ( NOLOCK )
                LEFT JOIN [Item] item WITH ( NOLOCK ) ON itlevel.id = ItemLevel_id AND item.LastVersion = 1
                LEFT JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
                LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
                LEFT JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
                LEFT JOIN [ItemSituation] its WITH ( NOLOCK ) ON its.Id = item.ItemSituation_Id
        WHERE    ( @disciplina IS NULL
                              OR d.id = @disciplina
                            )
                            AND its.Id = @situacao
                            AND mem.EntityId = @EntityId
							AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
                            AND itlevel.state = 1
                            AND item.state = 1
                            AND em.State = 1
                            AND d.State = 1
                            AND mem.State = 1
                            AND its.State = 1

        SELECT  description ,
                SUM(total) AS Total
        FROM    ( SELECT    itlevel.Description AS Description ,
                            CAST( COUNT(item.ItemLevel_id) / @total * 100 AS DECIMAL(18, 2)) AS Total
                  FROM      [ItemLevel] itlevel WITH ( NOLOCK )
                            LEFT JOIN [Item] item WITH ( NOLOCK ) ON itlevel.id = ItemLevel_id AND item.LastVersion = 1
                            LEFT JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
                            LEFT JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
                            LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
                            LEFT JOIN [ItemSituation] its WITH ( NOLOCK ) ON its.Id = item.ItemSituation_Id
                  WHERE     ( @disciplina IS NULL
                              OR d.id = @disciplina
                            )
                            AND its.Id = @situacao
                            AND mem.EntityId = @EntityId
							AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
                            AND itlevel.state = 1
                            AND item.state = 1
                            AND em.State = 1
                            AND d.State = 1
                            AND mem.State = 1
                            AND its.State = 1
                  GROUP BY  itlevel.Description
                  UNION
                  SELECT    description ,
                            0
                  FROM      [ItemLevel] itlevel WITH ( NOLOCK )
				  WHERE state = 1  
				  AND EntityId = @EntityId
                ) Tabela
        GROUP BY description

    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemSituation]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <14/10/2014>
-- Description:	<retorna quantidade de itens conforme sua situação>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemSituation]
    @disciplina INT ,
    @inicio VARCHAR(10) ,
    @fim VARCHAR(10) ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT
AS 
    BEGIN

-- codigo 103 para converter data yyyy-mm-dd
              
        SELECT  description ,
			SUM(total) AS Total
	FROM    ( SELECT    its.Description AS Description ,
						COUNT(item.id) AS Total
			  FROM      [ItemSituation] its WITH ( NOLOCK )
						LEFT JOIN [Item] item WITH ( NOLOCK ) ON item.ItemSituation_Id = its.id AND item.LastVersion = 1
						LEFT JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
						LEFT JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
						LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
			  WHERE     ( @disciplina IS NULL
						  OR d.id = @disciplina
						)
						AND (@inicio IS NULL AND @fim IS NULL 
						OR @inicio IS NOT NULL AND @fim IS NULL AND CAST(item.CreateDate AS DATE) >= @inicio
						OR @inicio IS NULL AND @fim IS NOT NULL AND CAST(item.CreateDate AS DATE)<= @fim
						OR @inicio IS NOT NULL AND @fim IS NOT NULL AND CAST(item.CreateDate AS DATE) 
						BETWEEN  CONVERT(DATE, @inicio) AND CONVERT(DATE, @fim)
						)
						AND its.state = 1
						AND item.state = 1
						AND em.State = 1
						AND d.State = 1
						AND mem.State = 1
						AND mem.entityId = @EntityId
						AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
			  GROUP BY  its.Description
			  UNION
			  SELECT    description ,
						0
			  FROM      [ItemSituation] its WITH ( NOLOCK )
			  WHERE state = 1  
			  AND EntityId = @EntityId  
			) Tabela
	GROUP BY description

    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemSkill]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <22/10/2014>
-- Description:	<retorna lista de ultimos níveis da skill>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemSkill]
    @disciplina INT ,
    @skill INT ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT
AS 
    BEGIN
        SELECT  
					mem.Description AS ModelDescription, s.Description, s.Code as Code, COUNT (its.Item_Id) AS Total
        FROM [Skill] s WITH ( NOLOCK )
					INNER JOIN [ItemSkill] its WITH (NOLOCK) ON its.Skill_Id = s.id AND s.LastLevel = 1
					INNER JOIN [Item] item WITH ( NOLOCK ) ON item.Id = its.Item_Id AND item.LastVersion = 1
					INNER JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
					INNER JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
					INNER JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
        WHERE ( @disciplina IS NULL
                          OR d.id = @disciplina
                        )
					AND mem.EntityId = @EntityId
					AND s.Parent_Id = @skill
					AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
					AND s.state = 1
					AND item.state = 1
					AND em.State = 1
					AND mem.State = 1
					AND d.State = 1
					AND its.State = 1
        GROUP BY s.Description, mem.Description, s.Code
    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemSkillOneLevel]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 
-- Description:	<retorna lista de ultimo níveis da matriz de 1 nível>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemSkillOneLevel]
    @disciplina INT ,
    @MatrizId INT ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT
AS 
    BEGIN
        SELECT  
					mem.Description AS ModelDescription, s.Description, s.Code as Code, COUNT (its.Item_Id) AS Total
        FROM [Skill] s WITH ( NOLOCK )
					INNER JOIN [ItemSkill] its WITH (NOLOCK) ON its.Skill_Id = s.id --AND s.LastLevel = 1
					INNER JOIN [Item] item WITH ( NOLOCK ) ON item.Id = its.Item_Id AND item.LastVersion = 1
					INNER JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
					INNER JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
					INNER JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
        WHERE ( @disciplina IS NULL
                          OR d.id = @disciplina
                        )
					AND mem.EntityId = @EntityId
					AND em.Id = @MatrizId
					AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
					AND s.state = 1
					AND item.state = 1
					AND em.State = 1
					AND mem.State = 1
					AND d.State = 1
					AND its.State = 1
        GROUP BY s.Description, mem.Description, s.Code
    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_ReportItemType]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Leticia Martin Corral>
-- Create date: <13/10/2014>
-- Description:	<Retorna dados para o relatorio de tipos de questão>
-- =============================================
CREATE PROCEDURE [dbo].[MS_Item_ReportItemType]
    @disciplina INT ,
    @situacao INT ,
    @EntityId UNIQUEIDENTIFIER ,
	@typeLevelEducation INT
AS 
    BEGIN

        SELECT  description ,
                SUM(total) AS Total
        FROM    ( SELECT    it.Description AS Description ,
                            COUNT(item.Id) AS Total
                  FROM      [ItemType] it WITH ( NOLOCK )
                            INNER JOIN [Item] item WITH ( NOLOCK ) ON it.Id = item.ItemType_Id AND item.LastVersion = 1
                            INNER JOIN [EvaluationMatrix] em WITH ( NOLOCK ) ON em.Id = item.EvaluationMatrix_Id
                            INNER JOIN [Discipline] d WITH ( NOLOCK ) ON d.Id = em.Discipline_Id
                            LEFT JOIN [ModelEvaluationMatrix] mem WITH ( NOLOCK ) ON mem.Id = em.ModelEvaluationMatrix_Id
                            LEFT JOIN [ItemSituation] its WITH ( NOLOCK ) ON its.Id = item.ItemSituation_Id
                  WHERE     ( @disciplina IS NULL
                              OR d.id = @disciplina
                            )
                            AND its.Id = @situacao
                            AND mem.EntityId = @EntityId
							AND (@typeLevelEducation IS NULL OR d.TypeLevelEducationId = @typeLevelEducation)
                            AND it.State = 1
                            AND item.State = 1
                            AND em.State = 1
                            AND d.State = 1
                            AND mem.State = 1
                            AND its.State = 1
                  GROUP BY  it.description
                  UNION
                  SELECT    description ,
                            0
                  FROM      [ItemType] it WITH ( NOLOCK )
                  WHERE state = 1  
                  AND EntityId = @EntityId  
                ) Tabela
        GROUP BY description

    END


GO
/****** Object:  StoredProcedure [dbo].[MS_Item_SearchFiltered]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
CREATE PROCEDURE [dbo].[MS_Item_SearchFiltered]
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
/****** Object:  StoredProcedure [dbo].[MS_Test_SearchFiltered]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Guilherme Mendonça
-- Create date: 27/05/2015
-- Description:	Retorna informações da prova de acordo com os filtros
-- =============================================
CREATE PROCEDURE [dbo].[MS_Test_SearchFiltered] 
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
/****** Object:  StoredProcedure [dbo].[MS_Test_SearchFilteredUser]    Script Date: 02/05/2017 09:51:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Guilherme Mendonça
-- Create date: 27/05/2015
-- Description:	Retorna informações da prova de acordo com os filtros
-- =============================================
CREATE PROCEDURE [dbo].[MS_Test_SearchFilteredUser] 
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
				  INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) teacher ON t.Id = teacher.Id
				  LEFT JOIN Discipline d WITH(NOLOCK) ON d.Id = t.Discipline_Id AND d.State = @state AND t.Discipline_Id = ISNULL(@DisciplineId, t.Discipline_Id)
				  LEFT JOIN TestPerformanceLevel tpl WITH(NOLOCK) ON tpl.Test_Id = t.Id AND tpl.State = @state
				  LEFT JOIN TestPermission TP WITH(NOLOCK) ON TP.Test_Id = t.Id AND TP.[State] = 1 AND TP.gru_id = @gru_id
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
			 AND ISNULL(TP.TestHide, 0) = 0
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
