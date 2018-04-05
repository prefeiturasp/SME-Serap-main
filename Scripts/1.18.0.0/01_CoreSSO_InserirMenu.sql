USE PUB_DEV_SPO_CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(MAX) = 'SERAp' 
		DECLARE @nomeModuloAvo VARCHAR(MAX) 
		DECLARE @nomeModuloPai VARCHAR(MAX) 
		DECLARE @nomeModulo VARCHAR(MAX) 

		SET @nomeModuloAvo = NULL
		SET @nomeModuloPai = NULL
		SET @nomeModulo = 'Auditoria'
 
		EXEC MS_InserePaginaMenu 
		  @nomeSistema = @nomeSistema,  
		  @nomeModuloPai = @nomeModuloPai,  
		  @nomeModulo = @nomeModulo,  
		  @SiteMap1Nome = 'Auditoria',  
		  @SiteMap1Url = 'Auditoria',  
		  @possuiVisaoAdm = 1,  
		  @possuiVisaoGestao = 1,  
		  @possuiVisaoUA = 0,  
		  @possuiVisaoIndividual = 0,  
		  @nomeModuloAvo = @nomeModuloAvo  

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Auditoria' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Log de alterações nas respostas' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Log de alterações nas respostas'
			,@SiteMap1Url = '/ResponseChangeLog/Index'					
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
				
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
