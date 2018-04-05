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
			,@nomeModuloPai = 'Resultados' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Comparativo de desempenho da rede' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Comparativo de desempenho da rede'
			,@SiteMap1Url = '/ReportPerformance/Index'					
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
				
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
