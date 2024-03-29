USE [ProvaSP]
GO

DROP TABLE MediaAtualTurma
DROP TABLE MediaEdicaoAluno
DROP TABLE MediaEdicaoDre
DROP TABLE MediaEdicaoEscola
DROP TABLE MediaEdicaoSme
DROP TABLE MediaEdicaoTurma
DROP TABLE MediaEdicaoTurmaAmostral


/****** Object:  Table [dbo].[NivelProficiencia]    Script Date: 16/01/2018 10:22:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NivelProficiencia](
	[NivelProficienciaID] [tinyint] NOT NULL,
	[Nome] [varchar](50) NULL,
 CONSTRAINT [PK_NivelProficiencia] PRIMARY KEY CLUSTERED 
(
	[NivelProficienciaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoAluno]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoAluno](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[AnoEscolar] [varchar](3) NULL,
	[tur_codigo] [varchar](20) NOT NULL,
	[tur_id] [bigint] NULL,
	[alu_matricula] [varchar](50) NOT NULL,
	[alu_nome] [varchar](200) NULL,
	[ResultadoLegadoID] [int] NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[Valor] [decimal](6, 3) NULL,
 CONSTRAINT [PK_ResultadoAluno] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[alu_matricula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoDre]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoDre](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoMediaDre] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[uad_sigla] ASC,
	[AnoEscolar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoEnturmacaoAtual]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoEnturmacaoAtual](
	[tur_id] [bigint] NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[AnoEscolar] [varchar](3) NULL,
	[tur_codigo] [varchar](20) NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[tur_dataAlteracao] [smalldatetime] NULL,
 CONSTRAINT [PK_ResultadoEnturmacaoAtual] PRIMARY KEY CLUSTERED 
(
	[tur_id] ASC,
	[AreaConhecimentoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoEscola]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoEscola](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoMediaEscola] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[esc_codigo] ASC,
	[AnoEscolar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoLegado]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoLegado](
	[ResultadoLegadoID] [int] IDENTITY(1,1) NOT NULL,
	[ANO] [int] NOT NULL,
	[num_aluno] [int] NOT NULL,
	[num_serie] [int] NULL,
	[cod_serie] [nvarchar](max) NULL,
	[codigo_escola_sme] [nvarchar](max) NULL,
	[num_escola] [int] NOT NULL,
	[COD_COORD] [nvarchar](max) NULL,
	[COD_TURMA] [int] NULL,
	[TURMA] [nvarchar](max) NULL,
	[CL_ALU_COD] [int] NULL,
	[COD_INEP_ALUNO] [nvarchar](255) NULL,
	[ALU_NOME] [nvarchar](max) NULL,
	[ID_NEE] [float] NULL,
	[areaLP] [int] NULL,
	[siglaLP] [nvarchar](max) NULL,
	[profixLP] [float] NULL,
	[pesoLP] [float] NULL,
	[areaMT] [int] NULL,
	[siglaMT] [nvarchar](max) NULL,
	[profixMT] [float] NULL,
	[pesoMT] [float] NULL,
	[areaCI] [int] NULL,
	[siglaCI] [nvarchar](max) NULL,
	[profixCI] [float] NULL,
	[pesoCI] [float] NULL,
	[areaRE] [int] NULL,
	[siglaRE] [nvarchar](max) NULL,
	[profixRE] [float] NULL,
	[pesoRE] [float] NULL,
	[profix_media] [float] NULL,
	[NivelPFxLP] [float] NULL,
	[NivelPFxMT] [float] NULL,
	[NivelPFxCI] [float] NULL,
	[NivelPFxRE] [float] NULL,
	[PontoPFxLP] [float] NULL,
	[PontoPFxMT] [float] NULL,
	[PontoPFxCI] [float] NULL,
	[PontoPFxRE] [float] NULL,
 CONSTRAINT [PK_ResultadoLegado] PRIMARY KEY CLUSTERED 
(
	[ResultadoLegadoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoSme]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoSme](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[TotalAlunos] [int] NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoMediaSme] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[AnoEscolar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultadoTurma]    Script Date: 16/01/2018 10:22:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultadoTurma](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[AnoEscolar] [varchar](3) NULL,
	[tur_codigo] [varchar](20) NOT NULL,
	[tur_id] [bigint] NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoMediaTurma] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[esc_codigo] ASC,
	[tur_codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ResultadoAluno]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoAluno_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO
ALTER TABLE [dbo].[ResultadoAluno] CHECK CONSTRAINT [FK_ResultadoAluno_NivelProficiencia]
GO
ALTER TABLE [dbo].[ResultadoDre]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoDre_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO
ALTER TABLE [dbo].[ResultadoDre] CHECK CONSTRAINT [FK_ResultadoDre_NivelProficiencia]
GO
ALTER TABLE [dbo].[ResultadoEscola]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoEscola_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO
ALTER TABLE [dbo].[ResultadoEscola] CHECK CONSTRAINT [FK_ResultadoEscola_NivelProficiencia]
GO
ALTER TABLE [dbo].[ResultadoSme]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoSme_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO
ALTER TABLE [dbo].[ResultadoSme] CHECK CONSTRAINT [FK_ResultadoSme_NivelProficiencia]
GO
ALTER TABLE [dbo].[ResultadoTurma]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoTurma_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO
ALTER TABLE [dbo].[ResultadoTurma] CHECK CONSTRAINT [FK_ResultadoTurma_NivelProficiencia]
GO
