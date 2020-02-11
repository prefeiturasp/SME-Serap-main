ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [Edicao] [varchar](10) NOT NULL
ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [AnoEscolar] [varchar](3) NOT NULL
ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [TotalPrevisto] [int] NOT NULL
ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [TotalPresente] [int] NOT NULL
ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [PercentualParticipacao] [decimal](6, 2) NOT NULL
ALTER TABLE ParticipacaoSMEAreAconhecimento ALTER COLUMN [AreaConhecimentoID] [tinyint] NOT NULL


ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [Edicao] [varchar](10) NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [uad_sigla] [varchar](4) NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [AnoEscolar] [varchar](3) NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [TotalPrevisto] [int] NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [TotalPresente] [int] NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [PercentualParticipacao] [decimal](6, 2) NOT NULL
ALTER TABLE ParticipacaoDREAreaConhecimento ALTER COLUMN [AreaConhecimentoID] [tinyint] NOT NULL


ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [Edicao] [varchar](10) NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [uad_sigla] [varchar](4) NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [esc_codigo] [varchar](20) NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [AnoEscolar] [varchar](3) NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [TotalPrevisto] [int] NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [TotalPresente] [int] NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [PercentualParticipacao] [decimal](6, 2) NOT NULL
ALTER TABLE ParticipacaoEscolaAreAconhecimento ALTER COLUMN [AreaConhecimentoID] [tinyint] NOT NULL


ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [Edicao] [varchar](10) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [uad_sigla] [varchar](4) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [esc_codigo] [varchar](20) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [AnoEscolar] [varchar](3) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [tur_codigo] [varchar](20) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [tur_id] [bigint] NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [TotalPrevisto] [int] NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [TotalPresente] [int] NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [PercentualParticipacao] [decimal](6, 2) NOT NULL
ALTER TABLE ParticipacaoTurmaAreAconhecimento ALTER COLUMN [AreaConhecimentoID] [tinyint] NOT NULL

UPDATE ParticipacaoDREAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoEscolaAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoSMEAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoTurmaAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

ALTER TABLE [dbo].[ParticipacaoSMEAreaConhecimento] ADD  CONSTRAINT [PK_ParticipacaoSMEAreaConhecimento] PRIMARY KEY CLUSTERED
(
	[Edicao],
	[AnoEscolar],
	[AreaConhecimentoID]
)

ALTER TABLE [dbo].[ParticipacaoDREAreaConhecimento] ADD  CONSTRAINT [PK_ParticipacaoDREAreaConhecimento] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[uad_sigla] ASC,
	[AnoEscolar] ASC,
	[AreaConhecimentoID]
)

ALTER TABLE [dbo].[ParticipacaoEscolaAreaConhecimento] ADD  CONSTRAINT [PK_ParticipacaoEscolaAreaConhecimento] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[uad_sigla] ASC,
	[esc_codigo] ASC,
	[AnoEscolar] ASC,
	[AreaConhecimentoID]
)

ALTER TABLE [dbo].[ParticipacaoTurmaAreaConhecimento] ADD  CONSTRAINT [PK_ParticipacaoTurmaAreaConhecimento] PRIMARY KEY CLUSTERED 
(
	[Edicao] ASC,
	[uad_sigla] ASC,
	[esc_codigo] ASC,
	[AnoEscolar] ASC,
	[tur_codigo] ASC,
	[AreaConhecimentoID]
)


