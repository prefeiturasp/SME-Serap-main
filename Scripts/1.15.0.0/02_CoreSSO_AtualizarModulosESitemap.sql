USE CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON

	DECLARE @nomeSistema VARCHAR(100) = 'SERAp'
	DECLARE @idSistema INT = (SELECT sis_id FROM SYS_Sistema WHERE sis_nome = @nomeSistema)

	UPDATE SYS_Modulo 
		SET mod_nome = 'Relatório de envio de folhas de respostas' 
	WHERE sis_id = @idSistema 
		AND mod_nome = 'Acompanhamento de envio de folhas de respostas' 
		AND mod_situacao = 1

	UPDATE SYS_Modulo 
		SET mod_nome = 'Relatório de processamento de correção'
	WHERE sis_id = @idSistema 
		AND mod_nome = 'Acompanhamento de processamento de correção' 
		AND mod_situacao = 1

	UPDATE SYS_ModuloSiteMap 
		SET msm_nome = 'Relatório de envio de folhas de respostas'
	WHERE sis_id = @idSistema 
		AND msm_nome = 'Acompanhamento de envio de folhas de respostas'

	UPDATE SYS_ModuloSiteMap
		SET msm_nome = 'Relatório de processamento de correção'
	WHERE sis_id = @idSistema 
	AND msm_nome = 'Acompanhamento de processamento de correção'

	UPDATE SYS_Modulo 
		SET mod_nome = 'Relatório de desempenho por item' 	
	WHERE sis_id = @idSistema 
		AND mod_nome = 'Relatório de desempenho por questão' 
		AND mod_situacao = 1
	
	UPDATE SYS_ModuloSiteMap 
		SET msm_nome = 'Relatório de desempenho por item'	
	WHERE sis_id = @idSistema 
		AND msm_nome = 'Relatório de desempenho por questão'

	UPDATE SYS_Modulo 
		SET mod_nome = 'Relatório de desempenho por alternativa' 	
	WHERE sis_id = @idSistema 
		AND mod_nome = 'Relatório de escolha por alternativa' 
		AND mod_situacao = 1
	
	UPDATE SYS_ModuloSiteMap 
		SET msm_nome = 'Relatório de desempenho por alternativa'	
	WHERE sis_id = @idSistema 
		AND msm_nome = 'Relatório de escolha por alternativa'

-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	