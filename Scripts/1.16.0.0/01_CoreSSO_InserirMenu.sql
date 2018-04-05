USE CoreSSO
GO

--Iniciar transação
BEGIN TRANSACTION
SET XACT_ABORT ON
		
		DECLARE @nomeSistema VARCHAR(100) = 'SERAp'

		-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Cadastros' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Área de conhecimento' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Consultar áreas de conhecimento'
			,@SiteMap1Url = '/KnowledgeArea/List'
			,@SiteMap2Nome = 'Cadastrar áreas de conhecimento' 
			,@SiteMap2Url = '/KnowledgeArea/Form'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual

			-- Insere modulo no menu do sistema no CoreSSO
		EXEC MS_InserePaginaMenu
			@nomeSistema = @nomeSistema -- Nome do sistema (obrigatório)			
			,@nomeModuloPai = 'Cadastros' -- Nome do módulo pai (Opicional, apénas quando houver)
			,@nomeModulo = 'Assunto e subassunto' -- Nome do módulo (Obrigatório)
			,@SiteMap1Nome = 'Consultar assuntos e subassuntos'
			,@SiteMap1Url = '/Subject/List'
			,@SiteMap2Nome = 'Cadastrar assuntos e subassuntos' 
			,@SiteMap2Url = '/Subject/Form'						
			,@possuiVisaoAdm = 1 -- Indicar se possui visão de administador
			,@possuiVisaoGestao = 1 -- Indicar se possui visão de Gestão
			,@possuiVisaoUA = 0 -- Indicar se possui visão de UA
			,@possuiVisaoIndividual = 0 -- Indicar se possui visão de individual
		
		PRINT '--> Termino da execução do script.'	
	
-- Fechar transação
SET XACT_ABORT OFF
COMMIT TRANSACTION	
