/*
   sexta-feira, 14 de dezembro de 2017 - 12:02:04
   Database: ProvaSP
*/

USE ProvaSP

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.MediaEdicaoAluno
	DROP CONSTRAINT FK_MediaAluno_AreaConhecimento
GO
ALTER TABLE dbo.MediaEdicaoEscola
	DROP CONSTRAINT FK_MediaEscola_AreaConhecimento
GO
ALTER TABLE dbo.MediaEdicaoDre
	DROP CONSTRAINT FK_MediaDRE_AreaConhecimento
GO
ALTER TABLE dbo.MediaEdicaoSme
	DROP CONSTRAINT FK_MediaSME_AreaConhecimento
GO
ALTER TABLE dbo.MediaEdicaoTurmaAmostral
	DROP CONSTRAINT FK_MediaEdicaoTurmaAmostral_AreaConhecimento
GO
ALTER TABLE dbo.MediaAtualTurma
	DROP CONSTRAINT FK_MediaAtualTurma_AreaConhecimento
GO
ALTER TABLE dbo.MediaEdicaoTurma
	DROP CONSTRAINT FK_MediaTurma_AreaConhecimento
GO
ALTER TABLE dbo.AreaConhecimento SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_MediaAtualTurma
	(
	tur_id bigint NOT NULL,
	AreaConhecimentoID tinyint NOT NULL,
	Ano smallint NULL,
	tur_codigo varchar(20) NULL,
	Valor varchar(50) NULL,
	tur_dataAlteracao smalldatetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_MediaAtualTurma SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.MediaAtualTurma)
	 EXEC('INSERT INTO dbo.Tmp_MediaAtualTurma (tur_id, AreaConhecimentoID, Valor, tur_dataAlteracao)
		SELECT tur_id, AreaConhecimentoID, Valor, tur_dataAlteracao FROM dbo.MediaAtualTurma WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.MediaAtualTurma
GO
EXECUTE sp_rename N'dbo.Tmp_MediaAtualTurma', N'MediaAtualTurma', 'OBJECT' 
GO
ALTER TABLE dbo.MediaAtualTurma ADD CONSTRAINT
	PK_MediaAtualTurma PRIMARY KEY CLUSTERED 
	(
	tur_id,
	AreaConhecimentoID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MediaAtualTurma ADD CONSTRAINT
	FK_MediaAtualTurma_AreaConhecimento FOREIGN KEY
	(
	AreaConhecimentoID
	) REFERENCES dbo.AreaConhecimento
	(
	AreaConhecimentoID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.MediaAtualTurma', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.MediaAtualTurma', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.MediaAtualTurma', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_MediaEdicaoAluno
	(
	alu_matricula varchar(50) NOT NULL,
	AreaConhecimentoID tinyint NOT NULL,
	Edicao varchar(10) NOT NULL,
	Ano smallint NULL,
	esc_codigo varchar(20) NOT NULL,
	tur_id bigint NULL,
	tur_codigo varchar(20) NOT NULL,
	Valor varchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_MediaEdicaoAluno SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.MediaEdicaoAluno)
	 EXEC('INSERT INTO dbo.Tmp_MediaEdicaoAluno (alu_matricula, AreaConhecimentoID, Edicao, esc_codigo, tur_id, tur_codigo, Valor)
		SELECT alu_matricula, AreaConhecimentoID, Edicao, esc_codigo, tur_id, tur_codigo, Valor FROM dbo.MediaEdicaoAluno WITH (HOLDLOCK TABLOCKX)')
GO
COMMIT
