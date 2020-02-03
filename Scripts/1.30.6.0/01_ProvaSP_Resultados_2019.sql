BEGIN
	BEGIN TRAN
	
	/* RESULTADOS */
	--Aplica 0 os percentuais quem estão NULL
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;

	--Arredonda para 1 casa decimal a alfabetização
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;

	--Arredonda para 1 casa decimal os percentuais e valores
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoAluno SET 
		Valor = ROUND(Valor, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoDRE SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoEscola SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoSME SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoTurma SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)
		
	
	/* RESULTADOS CICLO */
	--Aplica 0 os percentuais quem estão NULL
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;
	
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET PercentualAbaixoDoBasico = 0 WHERE PercentualAbaixoDoBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET PercentualBasico = 0 WHERE PercentualBasico IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET PercentualAdequado = 0 WHERE PercentualAdequado IS NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET PercentualAvancado = 0 WHERE PercentualAvancado IS NULL;

	--Arredonda para 1 casa decimal a alfabetização
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET PercentualAlfabetizado = ROUND(PercentualAlfabetizado, 1) WHERE PercentualAlfabetizado IS NOT NULL;

	--Arredonda para 1 casa decimal os percentuais e valores
	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloAluno SET 
		Valor = ROUND(Valor, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloDRE SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloEscola SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloSME SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)

	UPDATE ProvaSP_Resultado2019.dbo.ResultadoCicloTurma SET 
		Valor = ROUND(Valor, 1),
		PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
		PercentualBasico = ROUND(PercentualBasico, 1),
		PercentualAdequado = ROUND(PercentualAdequado, 1),
		PercentualAvancado = ROUND(PercentualAvancado, 1)
		

	/* PARTICIPACAO*/
	--Arredonda para 1 casa decimal as participações
	UPDATE ProvaSP_Resultado2019.dbo.ParticipacaoDRE SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
	UPDATE ProvaSP_Resultado2019.dbo.ParticipacaoEscola SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
	UPDATE ProvaSP_Resultado2019.dbo.ParticipacaoSME SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
	UPDATE ProvaSP_Resultado2019.dbo.ParticipacaoTurma SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

	COMMIT
	
	
	BEGIN TRAN
	
	INSERT INTO ProvaSP.dbo.ResultadoCicloAluno
	  (Edicao, AreaConhecimentoID, uad_sigla, esc_codigo, CicloID, tur_codigo, tur_id, alu_matricula, alu_nome, NivelProficienciaID, Valor)
	  SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoCicloAluno
	INSERT INTO ProvaSP.dbo.ResultadoCicloDre SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoCicloDre
	INSERT INTO ProvaSP.dbo.ResultadoCicloEscola SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoCicloEscola
	INSERT INTO ProvaSP.dbo.ResultadoCicloSme SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoCicloSme
	INSERT INTO ProvaSP.dbo.ResultadoCicloTurma SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoCicloTurma
	INSERT INTO ProvaSP.dbo.ResultadoAluno
	  (Edicao, AreaConhecimentoID, uad_sigla, esc_codigo, AnoEscolar, tur_codigo, tur_id, alu_matricula, alu_nome, NivelProficienciaID, Valor, REDQ1, REDQ2, REDQ3, REDQ4, REDQ5)
	  SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoAluno
	INSERT INTO ProvaSP.dbo.ResultadoDre SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoDre
	INSERT INTO ProvaSP.dbo.ResultadoEscola SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoEscola
	INSERT INTO ProvaSP.dbo.ResultadoSme SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoSme
	INSERT INTO ProvaSP.dbo.ResultadoTurma SELECT * FROM ProvaSP_Resultado2019.dbo.ResultadoTurma

	INSERT INTO ProvaSP.dbo.ParticipacaoDRE SELECT * FROM ProvaSP_Resultado2019.dbo.ParticipacaoDRE
	INSERT INTO ProvaSP.dbo.ParticipacaoEscola SELECT * FROM ProvaSP_Resultado2019.dbo.ParticipacaoEscola
	INSERT INTO ProvaSP.dbo.ParticipacaoSME SELECT * FROM ProvaSP_Resultado2019.dbo.ParticipacaoSME
	INSERT INTO ProvaSP.dbo.ParticipacaoTurma SELECT * FROM ProvaSP_Resultado2019.dbo.ParticipacaoTurma
	
	COMMIT
END