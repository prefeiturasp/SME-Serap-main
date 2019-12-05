USE [GestaoAvaliacao_SGP]
GO

ALTER TABLE [dbo].[MTR_MatriculaTurma] DROP CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id]
ALTER TABLE [dbo].[TUR_TurmaCurriculo] DROP CONSTRAINT [PK_dbo.TUR_TurmaCurriculo]

ALTER TABLE [dbo].[ACA_CurriculoDisciplina] DROP CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]
ALTER TABLE [dbo].[TUR_TurmaCurriculo] DROP CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]
ALTER TABLE [dbo].[ACA_CurriculoPeriodo] DROP CONSTRAINT [PK_dbo.ACA_CurriculoPeriodo]

ALTER TABLE [dbo].[ACA_CurriculoDisciplina] DROP CONSTRAINT [PK_dbo.ACA_CurriculoDisciplina]

--Adiciona novas colunas
ALTER TABLE [dbo].[TUR_TurmaCurriculo] ADD [tcp_id] [int] NULL
ALTER TABLE [dbo].[ACA_CurriculoDisciplina] ADD [tcp_id] [int] NULL
ALTER TABLE [dbo].[MTR_MatriculaTurma] ADD [tcp_id] [int] NULL

begin tran

UPDATE cp SET cp.tcp_id = tcp.tcp_id
FROM ACA_CurriculoPeriodo cp
  JOIN ACA_TipoCurriculoPeriodo tcp ON tcp.tcp_descricao = cp.crp_descricao
WHERE CP.tcp_id IS NULL

update tc
set tc.tcp_id = cp.tcp_id
from TUR_TurmaCurriculo tc
	inner join ACA_CurriculoPeriodo cp
		on tc.cur_id = cp.cur_id and
		   tc.crr_id = cp.crr_id and
		   tc.crp_id = cp.crp_id
WHERE tc.tcp_id IS NULL

update cd
set cd.tcp_id = cp.tcp_id
from ACA_CurriculoDisciplina cd
	inner join ACA_CurriculoPeriodo cp
		on cd.cur_id = cp.cur_id and
		   cd.crr_id = cp.crr_id and
		   cd.crp_id = cp.crp_id
WHERE cd.tcp_id IS NULL

update mt
set mt.tcp_id = null
from MTR_MatriculaTurma mt


update mt
set mt.tcp_id = tc.tcp_id
from MTR_MatriculaTurma mt
	inner join TUR_TurmaCurriculo tc
		on mt.tur_id = tc.tur_id and
		   mt.cur_id = tc.cur_id and
		   mt.crr_id = tc.crr_id and
		   mt.crp_id = tc.crp_id
WHERE mt.tcp_id IS NULL

COMMIT
-- rollback


ALTER TABLE [dbo].[TUR_TurmaCurriculo] ALTER COLUMN [tcp_id] [int] NOT NULL
ALTER TABLE [dbo].[ACA_CurriculoDisciplina] ALTER COLUMN [tcp_id] [int] NOT NULL
ALTER TABLE [dbo].[ACA_CurriculoPeriodo] ALTER COLUMN [tcp_id] [int] NOT NULL
ALTER TABLE [dbo].[MTR_MatriculaTurma] ALTER COLUMN [tcp_id] [int] NOT NULL


ALTER TABLE [dbo].[ACA_CurriculoDisciplina] ADD  CONSTRAINT [PK_dbo.ACA_CurriculoDisciplina] PRIMARY KEY CLUSTERED 
(
	[cur_id] ASC,
	[crr_id] ASC,
	[crp_id] ASC,
	[tcp_id] ASC,
	[tds_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ACA_CurriculoPeriodo] ADD  CONSTRAINT [PK_dbo.ACA_CurriculoPeriodo] PRIMARY KEY CLUSTERED 
(
	[cur_id] ASC,
	[crr_id] ASC,
	[crp_id] ASC,
	[tcp_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TUR_TurmaCurriculo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id] FOREIGN KEY([cur_id], [crr_id], [crp_id], [tcp_id])
REFERENCES [dbo].[ACA_CurriculoPeriodo] ([cur_id], [crr_id], [crp_id], [tcp_id])
ALTER TABLE [dbo].[TUR_TurmaCurriculo] CHECK CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]
GO

ALTER TABLE [dbo].[ACA_CurriculoDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id] FOREIGN KEY([cur_id], [crr_id], [crp_id], [tcp_id])
	REFERENCES [dbo].[ACA_CurriculoPeriodo] ([cur_id], [crr_id], [crp_id], [tcp_id])
ALTER TABLE [dbo].[ACA_CurriculoDisciplina] CHECK CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]
GO

ALTER TABLE [dbo].[TUR_TurmaCurriculo] ADD  CONSTRAINT [PK_dbo.TUR_TurmaCurriculo] PRIMARY KEY CLUSTERED 
(
	[tur_id] ASC,
	[cur_id] ASC,
	[crr_id] ASC,
	[crp_id] ASC, 
	[tcp_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MTR_MatriculaTurma]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id] FOREIGN KEY([tur_id], [cur_id], [crr_id], [crp_id], [tcp_id])
REFERENCES [dbo].[TUR_TurmaCurriculo] ([tur_id], [cur_id], [crr_id], [crp_id], [tcp_id])
ALTER TABLE [dbo].[MTR_MatriculaTurma] CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id]
GO