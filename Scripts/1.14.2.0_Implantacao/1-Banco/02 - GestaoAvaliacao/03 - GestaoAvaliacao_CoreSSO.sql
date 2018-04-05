USE CoreSSO
GO

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


DECLARE
	@ent_id_smesp UNIQUEIDENTIFIER
	, @sis_caminho VARCHAR(2000)
	, @sis_caminhoLogout VARCHAR(2000)
	, @sis_id INT
	, @gru_id_administrador UNIQUEIDENTIFIER
	, @usu_id UNIQUEIDENTIFIER

SET @sis_id = 204
SET @ent_id_smesp = (SELECT ent_id FROM SYS_Entidade WHERE ent_sigla = 'SMESP')
SET @sis_caminho = 'http://alterarURL/Account/LoginSSO'
SET @sis_caminhoLogout = 'http://alterarURL/Account/LogoutSSO'

IF (NOT EXISTS(SELECT * FROM SYS_Sistema WHERE sis_id = @sis_id))
BEGIN
	INSERT INTO SYS_Sistema (sis_id, sis_nome, sis_descricao, sis_caminho, sis_tipoAutenticacao, sis_situacao, sis_caminhoLogout)
	VALUES (@sis_id, 'Avalia+', '', @sis_caminho, 1, 1, @sis_caminhoLogout)
END


IF (NOT EXISTS(SELECT * FROM SYS_SistemaEntidade WHERE sis_id = @sis_id AND ent_id = @ent_id_smesp))
BEGIN
	INSERT INTO SYS_SistemaEntidade (sis_id, ent_id, sen_situacao) VALUES (@sis_id, @ent_id_smesp, 1)
END

IF (NOT EXISTS(SELECT * FROM SYS_Grupo WHERE sis_id = @sis_id AND gru_nome = 'Administrador'))
BEGIN
	INSERT INTO SYS_Grupo (gru_nome, gru_situacao, vis_id, sis_id, gru_integridade)
	VALUES ('Administrador', 1, 1, @sis_id, 1)
END

SET @gru_id_administrador = (SELECT gru_id FROM SYS_Grupo WHERE sis_id = @sis_id AND gru_nome = 'Administrador')
SET @usu_id = (SELECT usu_id FROM SYS_Usuario WHERE usu_login = 'admin' AND ent_id = @ent_id_smesp)

IF (NOT EXISTS(SELECT * FROM SYS_UsuarioGrupo WHERE gru_id = @gru_id_administrador AND usu_id = @usu_id))
BEGIN
	INSERT INTO SYS_UsuarioGrupo (usu_id, gru_id, usg_situacao)
	VALUES (@usu_id, @gru_id_administrador, 1)
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

