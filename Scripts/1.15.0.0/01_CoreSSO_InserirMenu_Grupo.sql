USE [CoreSSO]
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(100) = 'SERAp'

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Cadastros' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Grupo de prova' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Consultar grupos de prova'
			,@SiteMap1Url = '/TestGroup/List'
			,@SiteMap2Nome = 'Cadastrar grupos de prova' 
			,@SiteMap2Url = '/TestGroup/Form'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
		
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
