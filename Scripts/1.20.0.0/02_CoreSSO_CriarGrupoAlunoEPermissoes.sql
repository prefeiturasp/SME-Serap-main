USE PUB_GestaoAvaliacao_CoreSSO_Teste
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	DECLARE @vis_id INT = 4 -- Individual
	DECLARE @nomeSistema VARCHAR(MAX) = 'SERAp' 
	DECLARE @sis_id INT = (SELECT sis_id FROM SYS_Sistema WHERE sis_nome = 'SERAp'  AND sis_situacao <> 3)


	IF NOT EXISTS (SELECT gru_id FROM SYS_Grupo WHERE gru_nome = 'Aluno' AND gru_situacao <> 3 AND sis_id = @sis_id)
	BEGIN
		INSERT INTO SYS_Grupo
		(
			gru_id, 
			gru_nome, 
			gru_situacao, 
			gru_dataCriacao, 
			gru_dataAlteracao, 
			vis_id, 
			sis_id, 
			gru_integridade
		)
		VALUES 
		(
			NEWID(),
			'Aluno',
			1,
			GETDATE(),
			GETDATE(),
			@vis_id,
			@sis_id,
			0
		)

		DECLARE @gru_id UNIQUEIDENTIFIER = (SELECT gru_id FROM SYS_Grupo WHERE gru_nome = 'Aluno' AND gru_situacao <> 3 AND sis_id = @sis_id)

		DECLARE @mod_idProvas INT = (SELECT mod_id FROM SYS_Modulo WHERE mod_nome = 'Provas' AND mod_situacao <> 3 AND sis_id = @sis_id)
		DECLARE @mod_idProvaEletronica INT = (SELECT mod_id FROM SYS_Modulo WHERE mod_nome = 'Prova eletrônica' AND mod_situacao <> 3 AND sis_id = @sis_id)

		--INSERT INTO SYS_GrupoPermissao
		--(
		--	gru_id, 
		--	sis_id, 
		--	mod_id, 
		--	grp_consultar, 
		--	grp_inserir, 
		--	grp_alterar, 
		--	grp_excluir
		--)
		--VALUES 
		--(
		--	@gru_id,
		--	@sis_id,
		--	@mod_idProvas,
		--	1,
		--	1,
		--	1,
		--	1
		--)

		--INSERT INTO SYS_GrupoPermissao
		--(
		--	gru_id, 
		--	sis_id, 
		--	mod_id, 
		--	grp_consultar, 
		--	grp_inserir, 
		--	grp_alterar, 
		--	grp_excluir
		--)
		--VALUES 
		--(
		--	@gru_id,
		--	@sis_id,
		--	@mod_idProvaEletronica,
		--	1,
		--	1,
		--	1,
		--	1
		--)

	END


-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
