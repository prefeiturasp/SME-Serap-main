/*
   sexta-feira, 13 de novembro de 2017 - 16:03:04
   Database: ProvaSP
*/

USE ProvaSP

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
ALTER TABLE dbo.MediaEdicaoEscola
	DROP CONSTRAINT FK_MediaEscola_AreaConhecimento
GO
ALTER TABLE dbo.AreaConhecimento SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AreaConhecimento', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_MediaEdicaoEscola
	(
	esc_codigo varchar(20) NOT NULL,
	AreaConhecimentoID tinyint NOT NULL,
	Edicao varchar(10) NOT NULL,
	uad_sigla varchar(50) NOT NULL,
	Valor varchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_MediaEdicaoEscola SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.MediaEdicaoEscola)
	 EXEC('INSERT INTO dbo.Tmp_MediaEdicaoEscola (esc_codigo, AreaConhecimentoID, Edicao, Valor)
		SELECT esc_codigo, AreaConhecimentoID, Edicao, Valor FROM dbo.MediaEdicaoEscola WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.MediaEdicaoEscola
GO
EXECUTE sp_rename N'dbo.Tmp_MediaEdicaoEscola', N'MediaEdicaoEscola', 'OBJECT' 
GO
ALTER TABLE dbo.MediaEdicaoEscola ADD CONSTRAINT
	PK_MediaEscola PRIMARY KEY CLUSTERED 
	(
	esc_codigo,
	AreaConhecimentoID,
	Edicao
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MediaEdicaoEscola ADD CONSTRAINT
	FK_MediaEscola_AreaConhecimento FOREIGN KEY
	(
	AreaConhecimentoID
	) REFERENCES dbo.AreaConhecimento
	(
	AreaConhecimentoID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.MediaEdicaoEscola', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.MediaEdicaoEscola', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.MediaEdicaoEscola', 'Object', 'CONTROL') as Contr_Per 