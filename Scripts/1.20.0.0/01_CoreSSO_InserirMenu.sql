USE CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(MAX) = 'SERAp' 
		DECLARE @nomeModuloAvo VARCHAR(MAX) 
		DECLARE @nomeModuloPai VARCHAR(MAX) 
		DECLARE @nomeModulo VARCHAR(MAX) 

		SET @nomeModuloAvo = NULL
		SET @nomeModuloPai = 'Provas'
		SET @nomeModulo = 'Prova eletrônica'

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = @nomeModuloPai -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = @nomeModulo -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Consulta de provas eletrônicas'
			,@SiteMap1Url = '/ElectronicTest/Index'		
			,@SiteMap2Nome = 'Prova eletrônica'
			,@SiteMap2Url = '/ElectronicTest/Form'				
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 1 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 1 -- Indicar se possui visão de individual

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Parâmetros' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Configuração da página inicial' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Consulta de configurações da página inicial'
			,@SiteMap1Url = '/PageConfiguration/List'		
			,@SiteMap2Nome = 'Cadastro de configurações da página inicial'
			,@SiteMap2Url = '/PageConfiguration/Form'				
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 0 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
				
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
