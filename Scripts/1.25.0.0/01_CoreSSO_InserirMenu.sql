USE CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(MAX) = 'SERAp' 
		DECLARE @nomeModuloAvo VARCHAR(MAX) 
		DECLARE @nomeModuloPai VARCHAR(MAX) 
		DECLARE @nomeModulo VARCHAR(MAX) 

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Parâmetros' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Tipos de unidade administrativa' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Tipos de unidade administrativa'
			,@SiteMap1Url = '/AdministrativeUnitType'			
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 0 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
				
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
