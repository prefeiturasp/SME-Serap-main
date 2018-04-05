USE GestaoAvaliacao_SGP

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
PRINT N'Creating [dbo].[ACA_Aluno]...';


GO
CREATE TABLE [dbo].[ACA_Aluno] (
    [alu_id]                BIGINT           NOT NULL,
    [alu_nome]              VARCHAR (200)    NOT NULL,
    [ent_id]                UNIQUEIDENTIFIER NOT NULL,
    [alu_matricula]         VARCHAR (50)     NULL,
    [alu_dataCriacao]       DATETIME         NOT NULL,
    [alu_dataAlteracao]     DATETIME         NOT NULL,
    [alu_situacao]          TINYINT          NOT NULL,
    [MatriculaTurma_alu_id] BIGINT           NULL,
    [MatriculaTurma_mtu_id] INT              NULL,
    CONSTRAINT [PK_dbo.ACA_Aluno] PRIMARY KEY CLUSTERED ([alu_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_CalendarioAnual]...';


GO
CREATE TABLE [dbo].[ACA_CalendarioAnual] (
    [cal_id]            INT              NOT NULL,
    [ent_id]            UNIQUEIDENTIFIER NOT NULL,
    [cal_padrao]        BIT              NOT NULL,
    [cal_ano]           INT              NOT NULL,
    [cal_descricao]     VARCHAR (200)    NOT NULL,
    [cal_dataInicio]    DATE             NOT NULL,
    [cal_dataFim]       DATE             NOT NULL,
    [cal_situacao]      TINYINT          NOT NULL,
    [cal_dataCriacao]   DATETIME         NOT NULL,
    [cal_dataAlteracao] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.ACA_CalendarioAnual] PRIMARY KEY CLUSTERED ([cal_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_Curriculo]...';


GO
CREATE TABLE [dbo].[ACA_Curriculo] (
    [cur_id]            INT           NOT NULL,
    [crr_id]            INT           NOT NULL,
    [crr_nome]          VARCHAR (200) NULL,
    [crr_situacao]      TINYINT       NOT NULL,
    [crr_dataCriacao]   DATETIME      NOT NULL,
    [crr_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ACA_Curriculo] PRIMARY KEY CLUSTERED ([cur_id] ASC, [crr_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_CurriculoDisciplina]...';


GO
CREATE TABLE [dbo].[ACA_CurriculoDisciplina] (
    [cur_id]            INT      NOT NULL,
    [crr_id]            INT      NOT NULL,
    [crp_id]            INT      NOT NULL,
    [tds_id]            INT      NOT NULL,
    [crd_tipo]          TINYINT  NOT NULL,
    [crd_situacao]      TINYINT  NOT NULL,
    [crd_dataCriacao]   DATETIME NOT NULL,
    [crd_dataAlteracao] DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.ACA_CurriculoDisciplina] PRIMARY KEY CLUSTERED ([cur_id] ASC, [crr_id] ASC, [crp_id] ASC, [tds_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_CurriculoPeriodo]...';


GO
CREATE TABLE [dbo].[ACA_CurriculoPeriodo] (
    [cur_id]            INT           NOT NULL,
    [crr_id]            INT           NOT NULL,
    [crp_id]            INT           NOT NULL,
    [crp_ordem]         INT           NOT NULL,
    [crp_descricao]     VARCHAR (200) NOT NULL,
    [crp_situacao]      TINYINT       NOT NULL,
    [crp_dataCriacao]   DATETIME      NOT NULL,
    [crp_dataAlteracao] DATETIME      NOT NULL,
    [tcp_id]            INT           NULL,
    CONSTRAINT [PK_dbo.ACA_CurriculoPeriodo] PRIMARY KEY CLUSTERED ([cur_id] ASC, [crr_id] ASC, [crp_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_Curso]...';


GO
CREATE TABLE [dbo].[ACA_Curso] (
    [cur_id]             INT              NOT NULL,
    [ent_id]             UNIQUEIDENTIFIER NOT NULL,
    [tne_id]             INT              NOT NULL,
    [tme_id]             INT              NOT NULL,
    [cur_codigo]         VARCHAR (10)     NULL,
    [cur_nome]           VARCHAR (200)    NOT NULL,
    [cur_nome_abreviado] VARCHAR (20)     NULL,
    [cur_situacao]       TINYINT          NOT NULL,
    [cur_dataCriacao]    DATETIME         NOT NULL,
    [cur_dataAlteracao]  DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.ACA_Curso] PRIMARY KEY CLUSTERED ([cur_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_Docente]...';


GO
CREATE TABLE [dbo].[ACA_Docente] (
    [doc_id]            BIGINT           NOT NULL,
    [pes_id]            UNIQUEIDENTIFIER NOT NULL,
    [ent_id]            UNIQUEIDENTIFIER NOT NULL,
    [doc_nome]          VARCHAR (200)    NOT NULL,
    [doc_situacao]      TINYINT          NOT NULL,
    [doc_dataCriacao]   DATETIME         NOT NULL,
    [doc_dataAlteracao] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.ACA_Docente] PRIMARY KEY CLUSTERED ([doc_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_TipoCurriculoPeriodo]...';


GO
CREATE TABLE [dbo].[ACA_TipoCurriculoPeriodo] (
    [tcp_id]            INT           NOT NULL,
    [tne_id]            INT           NOT NULL,
    [tme_id]            INT           NOT NULL,
    [tcp_descricao]     VARCHAR (100) NOT NULL,
    [tcp_ordem]         TINYINT       NOT NULL,
    [tcp_situacao]      TINYINT       NOT NULL,
    [tcp_dataCriacao]   DATETIME      NOT NULL,
    [tcp_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ACA_TipoCurriculoPeriodo] PRIMARY KEY CLUSTERED ([tcp_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_TipoDisciplina]...';


GO
CREATE TABLE [dbo].[ACA_TipoDisciplina] (
    [tds_id]            INT           NOT NULL,
    [tne_id]            INT           NOT NULL,
    [tds_nome]          VARCHAR (100) NOT NULL,
    [tds_situacao]      TINYINT       NOT NULL,
    [tds_dataCriacao]   DATETIME      NOT NULL,
    [tds_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ACA_TipoDisciplina] PRIMARY KEY CLUSTERED ([tds_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_TipoModalidadeEnsino]...';


GO
CREATE TABLE [dbo].[ACA_TipoModalidadeEnsino] (
    [tme_id]            INT           NOT NULL,
    [tme_nome]          VARCHAR (100) NOT NULL,
    [tme_situacao]      TINYINT       NOT NULL,
    [tme_dataCriacao]   DATETIME      NOT NULL,
    [tme_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ACA_TipoModalidadeEnsino] PRIMARY KEY CLUSTERED ([tme_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_TipoNivelEnsino]...';


GO
CREATE TABLE [dbo].[ACA_TipoNivelEnsino] (
    [tne_id]            INT           NOT NULL,
    [tne_nome]          VARCHAR (100) NOT NULL,
    [tne_situacao]      TINYINT       NOT NULL,
    [tne_dataCriacao]   DATETIME      NOT NULL,
    [tne_dataAlteracao] DATETIME      NOT NULL,
    [tne_ordem]         INT           NOT NULL,
    CONSTRAINT [PK_dbo.ACA_TipoNivelEnsino] PRIMARY KEY CLUSTERED ([tne_id] ASC)
);


GO
PRINT N'Creating [dbo].[ACA_TipoTurno]...';


GO
CREATE TABLE [dbo].[ACA_TipoTurno] (
    [ttn_id]            INT           NOT NULL,
    [ttn_nome]          VARCHAR (100) NOT NULL,
    [ttn_situacao]      TINYINT       NOT NULL,
    [ttn_dataCriacao]   DATETIME      NOT NULL,
    [ttn_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ACA_TipoTurno] PRIMARY KEY CLUSTERED ([ttn_id] ASC)
);


GO
PRINT N'Creating [dbo].[ESC_Escola]...';


GO
CREATE TABLE [dbo].[ESC_Escola] (
    [esc_id]               INT              NOT NULL,
    [ent_id]               UNIQUEIDENTIFIER NOT NULL,
    [uad_id]               UNIQUEIDENTIFIER NOT NULL,
    [esc_codigo]           VARCHAR (20)     NULL,
    [esc_nome]             VARCHAR (200)    NOT NULL,
    [esc_situacao]         TINYINT          NOT NULL,
    [esc_dataCriacao]      DATETIME         NOT NULL,
    [esc_dataAlteracao]    DATETIME         NOT NULL,
    [uad_idSuperiorGestao] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.ESC_Escola] PRIMARY KEY CLUSTERED ([esc_id] ASC)
);


GO
PRINT N'Creating [dbo].[MTR_MatriculaTurma]...';


GO
CREATE TABLE [dbo].[MTR_MatriculaTurma] (
    [alu_id]            BIGINT   NOT NULL,
    [mtu_id]            INT      NOT NULL,
    [esc_id]            INT      NOT NULL,
    [tur_id]            BIGINT   NOT NULL,
    [cur_id]            INT      NOT NULL,
    [crr_id]            INT      NOT NULL,
    [crp_id]            INT      NOT NULL,
    [mtu_situacao]      TINYINT  NOT NULL,
    [mtu_dataCriacao]   DATETIME NOT NULL,
    [mtu_dataAlteracao] DATETIME NOT NULL,
    [mtu_numeroChamada] INT      NULL,
    [mtu_dataMatricula] DATE     NULL,
    [mtu_dataSaida]     DATE     NULL,
    CONSTRAINT [PK_dbo.MTR_MatriculaTurma] PRIMARY KEY CLUSTERED ([alu_id] ASC, [mtu_id] ASC)
);


GO
PRINT N'Creating [dbo].[MTR_MatriculaTurma].[IX_MTR_MatriculaTurma_tur_id_mtu_situacao>]...';


GO
CREATE NONCLUSTERED INDEX [IX_MTR_MatriculaTurma_tur_id_mtu_situacao>]
    ON [dbo].[MTR_MatriculaTurma]([tur_id] ASC, [mtu_situacao] ASC);


GO
PRINT N'Creating [dbo].[MTR_MatriculaTurma].[IX_MTR_MatriculaTurma_tur_id]...';


GO
CREATE NONCLUSTERED INDEX [IX_MTR_MatriculaTurma_tur_id]
    ON [dbo].[MTR_MatriculaTurma]([tur_id] ASC);


GO
PRINT N'Creating [dbo].[MTR_MatriculaTurmaDisciplina]...';


GO
CREATE TABLE [dbo].[MTR_MatriculaTurmaDisciplina] (
    [alu_id]            BIGINT   NOT NULL,
    [mtu_id]            INT      NOT NULL,
    [mtd_id]            INT      NOT NULL,
    [tud_id]            BIGINT   NOT NULL,
    [mtd_numeroChamada] INT      NULL,
    [mtd_situacao]      TINYINT  NOT NULL,
    [mtd_dataCriacao]   DATETIME NOT NULL,
    [mtd_dataAlteracao] DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.MTR_MatriculaTurmaDisciplina] PRIMARY KEY CLUSTERED ([alu_id] ASC, [mtu_id] ASC, [mtd_id] ASC)
);


GO
PRINT N'Creating [dbo].[SYS_UnidadeAdministrativa]...';


GO
CREATE TABLE [dbo].[SYS_UnidadeAdministrativa] (
    [ent_id]            UNIQUEIDENTIFIER NOT NULL,
    [uad_id]            UNIQUEIDENTIFIER NOT NULL,
    [uad_codigo]        VARCHAR (20)     NULL,
    [uad_nome]          VARCHAR (200)    NOT NULL,
    [uad_sigla]         VARCHAR (50)     NULL,
    [uad_situacao]      TINYINT          NOT NULL,
    [uad_dataCriacao]   DATETIME         NOT NULL,
    [uad_dataAlteracao] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.SYS_UnidadeAdministrativa] PRIMARY KEY CLUSTERED ([ent_id] ASC, [uad_id] ASC)
);


GO
PRINT N'Creating [dbo].[TUR_Turma]...';


GO
CREATE TABLE [dbo].[TUR_Turma] (
    [tur_id]            BIGINT         NOT NULL,
    [esc_id]            INT            NOT NULL,
    [tur_codigo]        VARCHAR (30)   NULL,
    [tur_descricao]     VARCHAR (2000) NULL,
    [cal_id]            INT            NOT NULL,
    [ttn_id]            INT            NULL,
    [tur_situacao]      TINYINT        NOT NULL,
    [tur_dataCriacao]   DATETIME       NOT NULL,
    [tur_dataAlteracao] DATETIME       NOT NULL,
    [tur_tipo]          TINYINT        NOT NULL,
    CONSTRAINT [PK_dbo.TUR_Turma] PRIMARY KEY CLUSTERED ([tur_id] ASC)
);


GO
PRINT N'Creating [dbo].[TUR_Turma].[IX_TUR_Turma_tur_situacao]...';


GO
CREATE NONCLUSTERED INDEX [IX_TUR_Turma_tur_situacao]
    ON [dbo].[TUR_Turma]([tur_situacao] ASC)
    INCLUDE([tur_id], [tur_codigo], [ttn_id]);


GO
PRINT N'Creating [dbo].[TUR_TurmaCurriculo]...';


GO
CREATE TABLE [dbo].[TUR_TurmaCurriculo] (
    [tur_id]            BIGINT   NOT NULL,
    [cur_id]            INT      NOT NULL,
    [crr_id]            INT      NOT NULL,
    [crp_id]            INT      NOT NULL,
    [tcr_situacao]      TINYINT  NOT NULL,
    [tcr_dataCriacao]   DATETIME NOT NULL,
    [tcr_dataAlteracao] DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.TUR_TurmaCurriculo] PRIMARY KEY CLUSTERED ([tur_id] ASC, [cur_id] ASC, [crr_id] ASC, [crp_id] ASC)
);


GO
PRINT N'Creating [dbo].[TUR_TurmaDisciplina]...';


GO
CREATE TABLE [dbo].[TUR_TurmaDisciplina] (
    [tud_id]            BIGINT        NOT NULL,
    [tur_id]            BIGINT        NOT NULL,
    [tds_id]            INT           NOT NULL,
    [tud_codigo]        VARCHAR (30)  NOT NULL,
    [tud_nome]          VARCHAR (200) NOT NULL,
    [tud_tipo]          TINYINT       NOT NULL,
    [tud_situacao]      TINYINT       NOT NULL,
    [tud_dataCriacao]   DATETIME      NOT NULL,
    [tud_dataAlteracao] DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.TUR_TurmaDisciplina] PRIMARY KEY CLUSTERED ([tud_id] ASC)
);


GO
PRINT N'Creating [dbo].[TUR_TurmaDocente]...';


GO
CREATE TABLE [dbo].[TUR_TurmaDocente] (
    [tud_id]            BIGINT   NOT NULL,
    [tdt_id]            INT      NOT NULL,
    [doc_id]            BIGINT   NOT NULL,
    [tdt_situacao]      TINYINT  NOT NULL,
    [tdt_dataCriacao]   DATETIME NOT NULL,
    [tdt_dataAlteracao] DATETIME NOT NULL,
    [tdt_posicao]       TINYINT  NOT NULL,
    CONSTRAINT [PK_dbo.TUR_TurmaDocente] PRIMARY KEY CLUSTERED ([tud_id] ASC, [tdt_id] ASC)
);


GO
PRINT N'Creating [dbo].[TUR_TurmaTipoCurriculoPeriodo]...';


GO
CREATE TABLE [dbo].[TUR_TurmaTipoCurriculoPeriodo] (
    [tur_id]        BIGINT NOT NULL,
    [cur_id]        INT    NOT NULL,
    [tme_id]        INT    NOT NULL,
    [tne_id]        INT    NOT NULL,
    [crp_ordem]     INT    NOT NULL,
    [ttcr_situacao] INT    NOT NULL,
    [esc_id]        INT    NOT NULL,
    CONSTRAINT [PK_dbo.TUR_TurmaTipoCurriculoPeriodo] PRIMARY KEY CLUSTERED ([tur_id] ASC, [cur_id] ASC, [tme_id] ASC, [tne_id] ASC, [crp_ordem] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[TUR_TurmaTipoCurriculoPeriodo]...';


GO
ALTER TABLE [dbo].[TUR_TurmaTipoCurriculoPeriodo]
    ADD DEFAULT ((0)) FOR [ttcr_situacao];


GO
PRINT N'Creating unnamed constraint on [dbo].[TUR_TurmaTipoCurriculoPeriodo]...';


GO
ALTER TABLE [dbo].[TUR_TurmaTipoCurriculoPeriodo]
    ADD DEFAULT ((0)) FOR [esc_id];


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_Aluno_dbo.MTR_MatriculaTurma_MatriculaTurma_alu_id_MatriculaTurma_mtu_id]...';


GO
ALTER TABLE [dbo].[ACA_Aluno] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_Aluno_dbo.MTR_MatriculaTurma_MatriculaTurma_alu_id_MatriculaTurma_mtu_id] FOREIGN KEY ([MatriculaTurma_alu_id], [MatriculaTurma_mtu_id]) REFERENCES [dbo].[MTR_MatriculaTurma] ([alu_id], [mtu_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_Curriculo_dbo.ACA_Curso_cur_id]...';


GO
ALTER TABLE [dbo].[ACA_Curriculo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_Curriculo_dbo.ACA_Curso_cur_id] FOREIGN KEY ([cur_id]) REFERENCES [dbo].[ACA_Curso] ([cur_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]...';


GO
ALTER TABLE [dbo].[ACA_CurriculoDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id] FOREIGN KEY ([cur_id], [crr_id], [crp_id]) REFERENCES [dbo].[ACA_CurriculoPeriodo] ([cur_id], [crr_id], [crp_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_TipoDisciplina_tds_id]...';


GO
ALTER TABLE [dbo].[ACA_CurriculoDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_TipoDisciplina_tds_id] FOREIGN KEY ([tds_id]) REFERENCES [dbo].[ACA_TipoDisciplina] ([tds_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_Curriculo_cur_id_crr_id]...';


GO
ALTER TABLE [dbo].[ACA_CurriculoPeriodo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_Curriculo_cur_id_crr_id] FOREIGN KEY ([cur_id], [crr_id]) REFERENCES [dbo].[ACA_Curriculo] ([cur_id], [crr_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_TipoCurriculoPeriodo_tcp_id]...';


GO
ALTER TABLE [dbo].[ACA_CurriculoPeriodo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_TipoCurriculoPeriodo_tcp_id] FOREIGN KEY ([tcp_id]) REFERENCES [dbo].[ACA_TipoCurriculoPeriodo] ([tcp_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_Curso_dbo.ACA_TipoModalidadeEnsino_tme_id]...';


GO
ALTER TABLE [dbo].[ACA_Curso] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_Curso_dbo.ACA_TipoModalidadeEnsino_tme_id] FOREIGN KEY ([tme_id]) REFERENCES [dbo].[ACA_TipoModalidadeEnsino] ([tme_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_Curso_dbo.ACA_TipoNivelEnsino_tne_id]...';


GO
ALTER TABLE [dbo].[ACA_Curso] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_Curso_dbo.ACA_TipoNivelEnsino_tne_id] FOREIGN KEY ([tne_id]) REFERENCES [dbo].[ACA_TipoNivelEnsino] ([tne_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoModalidadeEnsino_tme_id]...';


GO
ALTER TABLE [dbo].[ACA_TipoCurriculoPeriodo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoModalidadeEnsino_tme_id] FOREIGN KEY ([tme_id]) REFERENCES [dbo].[ACA_TipoModalidadeEnsino] ([tme_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoNivelEnsino_tne_id]...';


GO
ALTER TABLE [dbo].[ACA_TipoCurriculoPeriodo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoNivelEnsino_tne_id] FOREIGN KEY ([tne_id]) REFERENCES [dbo].[ACA_TipoNivelEnsino] ([tne_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ACA_TipoDisciplina_dbo.ACA_TipoNivelEnsino_tne_id]...';


GO
ALTER TABLE [dbo].[ACA_TipoDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ACA_TipoDisciplina_dbo.ACA_TipoNivelEnsino_tne_id] FOREIGN KEY ([tne_id]) REFERENCES [dbo].[ACA_TipoNivelEnsino] ([tne_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.ESC_Escola_dbo.SYS_UnidadeAdministrativa_ent_id_uad_idSuperiorGestao]...';


GO
ALTER TABLE [dbo].[ESC_Escola] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.ESC_Escola_dbo.SYS_UnidadeAdministrativa_ent_id_uad_idSuperiorGestao] FOREIGN KEY ([ent_id], [uad_idSuperiorGestao]) REFERENCES [dbo].[SYS_UnidadeAdministrativa] ([ent_id], [uad_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.MTR_MatriculaTurma_dbo.ACA_Aluno_alu_id]...';


GO
ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.ACA_Aluno_alu_id] FOREIGN KEY ([alu_id]) REFERENCES [dbo].[ACA_Aluno] ([alu_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.MTR_MatriculaTurma_dbo.ESC_Escola_esc_id]...';


GO
ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.ESC_Escola_esc_id] FOREIGN KEY ([esc_id]) REFERENCES [dbo].[ESC_Escola] ([esc_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id]...';


GO
ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id] FOREIGN KEY ([tur_id], [cur_id], [crr_id], [crp_id]) REFERENCES [dbo].[TUR_TurmaCurriculo] ([tur_id], [cur_id], [crr_id], [crp_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.MTR_MatriculaTurma_alu_id_mtu_id]...';


GO
ALTER TABLE [dbo].[MTR_MatriculaTurmaDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.MTR_MatriculaTurma_alu_id_mtu_id] FOREIGN KEY ([alu_id], [mtu_id]) REFERENCES [dbo].[MTR_MatriculaTurma] ([alu_id], [mtu_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.TUR_TurmaDisciplina_tud_id]...';


GO
ALTER TABLE [dbo].[MTR_MatriculaTurmaDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.TUR_TurmaDisciplina_tud_id] FOREIGN KEY ([tud_id]) REFERENCES [dbo].[TUR_TurmaDisciplina] ([tud_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_Turma_dbo.ACA_CalendarioAnual_cal_id]...';


GO
ALTER TABLE [dbo].[TUR_Turma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_Turma_dbo.ACA_CalendarioAnual_cal_id] FOREIGN KEY ([cal_id]) REFERENCES [dbo].[ACA_CalendarioAnual] ([cal_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_Turma_dbo.ACA_TipoTurno_ttn_id]...';


GO
ALTER TABLE [dbo].[TUR_Turma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_Turma_dbo.ACA_TipoTurno_ttn_id] FOREIGN KEY ([ttn_id]) REFERENCES [dbo].[ACA_TipoTurno] ([ttn_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_Turma_dbo.ESC_Escola_esc_id]...';


GO
ALTER TABLE [dbo].[TUR_Turma] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_Turma_dbo.ESC_Escola_esc_id] FOREIGN KEY ([esc_id]) REFERENCES [dbo].[ESC_Escola] ([esc_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaCurriculo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id] FOREIGN KEY ([cur_id], [crr_id], [crp_id]) REFERENCES [dbo].[ACA_CurriculoPeriodo] ([cur_id], [crr_id], [crp_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaCurriculo_dbo.TUR_Turma_tur_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaCurriculo] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.TUR_Turma_tur_id] FOREIGN KEY ([tur_id]) REFERENCES [dbo].[TUR_Turma] ([tur_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaDisciplina_dbo.ACA_TipoDisciplina_tds_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaDisciplina_dbo.ACA_TipoDisciplina_tds_id] FOREIGN KEY ([tds_id]) REFERENCES [dbo].[ACA_TipoDisciplina] ([tds_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaDisciplina_dbo.TUR_Turma_tur_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaDisciplina] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaDisciplina_dbo.TUR_Turma_tur_id] FOREIGN KEY ([tur_id]) REFERENCES [dbo].[TUR_Turma] ([tur_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaDocente_dbo.ACA_Docente_doc_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaDocente] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaDocente_dbo.ACA_Docente_doc_id] FOREIGN KEY ([doc_id]) REFERENCES [dbo].[ACA_Docente] ([doc_id]);


GO
PRINT N'Creating [dbo].[FK_dbo.TUR_TurmaDocente_dbo.TUR_TurmaDisciplina_tud_id]...';


GO
ALTER TABLE [dbo].[TUR_TurmaDocente] WITH NOCHECK
    ADD CONSTRAINT [FK_dbo.TUR_TurmaDocente_dbo.TUR_TurmaDisciplina_tud_id] FOREIGN KEY ([tud_id]) REFERENCES [dbo].[TUR_TurmaDisciplina] ([tud_id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[ACA_Aluno] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_Aluno_dbo.MTR_MatriculaTurma_MatriculaTurma_alu_id_MatriculaTurma_mtu_id];

ALTER TABLE [dbo].[ACA_Curriculo] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_Curriculo_dbo.ACA_Curso_cur_id];

ALTER TABLE [dbo].[ACA_CurriculoDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id];

ALTER TABLE [dbo].[ACA_CurriculoDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_CurriculoDisciplina_dbo.ACA_TipoDisciplina_tds_id];

ALTER TABLE [dbo].[ACA_CurriculoPeriodo] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_Curriculo_cur_id_crr_id];

ALTER TABLE [dbo].[ACA_CurriculoPeriodo] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_CurriculoPeriodo_dbo.ACA_TipoCurriculoPeriodo_tcp_id];

ALTER TABLE [dbo].[ACA_Curso] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_Curso_dbo.ACA_TipoModalidadeEnsino_tme_id];

ALTER TABLE [dbo].[ACA_Curso] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_Curso_dbo.ACA_TipoNivelEnsino_tne_id];

ALTER TABLE [dbo].[ACA_TipoCurriculoPeriodo] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoModalidadeEnsino_tme_id];

ALTER TABLE [dbo].[ACA_TipoCurriculoPeriodo] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_TipoCurriculoPeriodo_dbo.ACA_TipoNivelEnsino_tne_id];

ALTER TABLE [dbo].[ACA_TipoDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.ACA_TipoDisciplina_dbo.ACA_TipoNivelEnsino_tne_id];

ALTER TABLE [dbo].[ESC_Escola] WITH CHECK CHECK CONSTRAINT [FK_dbo.ESC_Escola_dbo.SYS_UnidadeAdministrativa_ent_id_uad_idSuperiorGestao];

ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH CHECK CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.ACA_Aluno_alu_id];

ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH CHECK CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.ESC_Escola_esc_id];

ALTER TABLE [dbo].[MTR_MatriculaTurma] WITH CHECK CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurma_dbo.TUR_TurmaCurriculo_tur_id_cur_id_crr_id_crp_id];

ALTER TABLE [dbo].[MTR_MatriculaTurmaDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.MTR_MatriculaTurma_alu_id_mtu_id];

ALTER TABLE [dbo].[MTR_MatriculaTurmaDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.MTR_MatriculaTurmaDisciplina_dbo.TUR_TurmaDisciplina_tud_id];

ALTER TABLE [dbo].[TUR_Turma] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_Turma_dbo.ACA_CalendarioAnual_cal_id];

ALTER TABLE [dbo].[TUR_Turma] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_Turma_dbo.ACA_TipoTurno_ttn_id];

ALTER TABLE [dbo].[TUR_Turma] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_Turma_dbo.ESC_Escola_esc_id];

ALTER TABLE [dbo].[TUR_TurmaCurriculo] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.ACA_CurriculoPeriodo_cur_id_crr_id_crp_id];

ALTER TABLE [dbo].[TUR_TurmaCurriculo] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaCurriculo_dbo.TUR_Turma_tur_id];

ALTER TABLE [dbo].[TUR_TurmaDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaDisciplina_dbo.ACA_TipoDisciplina_tds_id];

ALTER TABLE [dbo].[TUR_TurmaDisciplina] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaDisciplina_dbo.TUR_Turma_tur_id];

ALTER TABLE [dbo].[TUR_TurmaDocente] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaDocente_dbo.ACA_Docente_doc_id];

ALTER TABLE [dbo].[TUR_TurmaDocente] WITH CHECK CHECK CONSTRAINT [FK_dbo.TUR_TurmaDocente_dbo.TUR_TurmaDisciplina_tud_id];


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
