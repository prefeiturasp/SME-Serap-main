use ProvaSP

CREATE TABLE [dbo].[Ciclo](
	[CicloId] [int] NOT NULL,
	[Nome] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Ciclo] PRIMARY KEY CLUSTERED 
(
	[CicloId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CicloAnoEscolar](
	[CicloId] [int] NOT NULL,
	[AnoEscolar] [int] NOT NULL,
 CONSTRAINT [PK_CicloAnoEscolar] PRIMARY KEY CLUSTERED 
(
	[CicloId] ASC,
	[AnoEscolar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CicloAnoEscolar]  WITH CHECK ADD  CONSTRAINT [FK_CicloAnoEscolar_Ciclo] FOREIGN KEY([CicloId])
REFERENCES [dbo].[Ciclo] ([CicloId])
GO

ALTER TABLE [dbo].[CicloAnoEscolar] CHECK CONSTRAINT [FK_CicloAnoEscolar_Ciclo]
GO

CREATE TABLE [dbo].[ResultadoCicloEscola](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[CicloId] [int] NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[PercentualAlfabetizado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoCicloEscola] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[esc_codigo] ASC,
	[CicloId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResultadoCicloEscola]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoCicloEscola_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO

ALTER TABLE [dbo].[ResultadoCicloEscola] CHECK CONSTRAINT [FK_ResultadoCicloEscola_NivelProficiencia]
GO

CREATE TABLE [dbo].[ResultadoCicloSme](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[CicloID] [int] NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[TotalAlunos] [int] NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[PercentualAlfabetizado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoCicloSme] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[CicloID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResultadoCicloSme]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoCicloSme_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO

ALTER TABLE [dbo].[ResultadoCicloSme] CHECK CONSTRAINT [FK_ResultadoCicloSme_NivelProficiencia]
GO

CREATE TABLE [dbo].[ResultadoCicloTurma](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[CicloId] [int] NULL,
	[tur_codigo] [varchar](20) NOT NULL,
	[tur_id] [bigint] NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[PercentualAlfabetizado] [decimal](6, 2) NULL,
 CONSTRAINT [PK_ResultadoCicloTurma] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[AreaConhecimentoID] ASC,
	[esc_codigo] ASC,
	[tur_codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResultadoCicloTurma]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoCicloTurma_NivelProficiencia] FOREIGN KEY([NivelProficienciaID])
REFERENCES [dbo].[NivelProficiencia] ([NivelProficienciaID])
GO

ALTER TABLE [dbo].[ResultadoCicloTurma] CHECK CONSTRAINT [FK_ResultadoCicloTurma_NivelProficiencia]
GO