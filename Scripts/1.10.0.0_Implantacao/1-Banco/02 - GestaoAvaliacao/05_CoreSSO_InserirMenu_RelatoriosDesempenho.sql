USE CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(100) = 'SERAp'

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Relatórios' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Relatório de desempenho por prova' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Relatório de desempenho por prova'
			,@SiteMap1Url = '/ReportTestPerformance/IndexDRE'
			,@SiteMap2Nome = 'Relatório de desempenho por prova' 
			,@SiteMap2Url = '/ReportTestPerformance/IndexSchool'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 0 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Relatórios' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Relatório de desempenho por questão' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Relatório de desempenho por questão'
			,@SiteMap1Url = '/ReportItemPerformance/IndexDRE'
			,@SiteMap2Nome = 'Relatório de desempenho por questão' 
			,@SiteMap2Url = '/ReportItemPerformance/IndexSchool'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 0 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Relatórios' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Relatório de escolha por alternativa' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Relatório de escolha por alternativa'
			,@SiteMap1Url = '/ReportItemChoice'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 0 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
		
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
