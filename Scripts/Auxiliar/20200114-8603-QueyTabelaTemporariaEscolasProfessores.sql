IF OBJECT_ID('tempdb..##ProfessoresEscolas2') IS NOT NULL
    DROP TABLE ##ProfessoresEscolas2

SELECT CONVERT(UNIQUEIDENTIFIER, '') pes_id,
	   CONVERT(VARCHAR(1000), '') esc_codigo,
	   CONVERT(VARCHAR(MAX), '') esc_nome,
	   CONVERT(VARCHAR(500), '') tur_codigo
INTO ##ProfessoresEscolas2
WHERE 1 = 0	

DECLARE 
    @pes_id UNIQUEIDENTIFIER,
	@Codigo VARCHAR(1000),
	@Escola VARCHAR(MAX),
	@AnoEscolar VARCHAR(500)
 
DECLARE lista CURSOR FAST_FORWARD
FOR SELECT DISTINCT d.pes_id,
					e.esc_codigo, 
					e.esc_nome,
					t.tur_codigo				
	FROM GestaoAvaliacao_SGP.dbo.ACA_Docente d (NOLOCK)
		INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td (NOLOCK)
			ON d.doc_id = td.doc_id
		INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud (NOLOCK)
			ON td.tud_id = tud.tud_id
		INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t (NOLOCK)
			ON tud.tur_id = t.tur_id
		INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e (NOLOCK)
			ON t.esc_id = e.esc_id
		INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca (NOLOCK)
			ON t.cal_id = ca.cal_id
	WHERE t.tur_situacao <> 3 AND
		  ca.cal_ano = 2019
 
OPEN lista;
 
FETCH NEXT FROM lista INTO 
    @pes_id,
	@Codigo,
	@Escola,
	@AnoEscolar

 
WHILE @@FETCH_STATUS = 0
    BEGIN
        IF (EXISTS (SELECT 1 FROM ##ProfessoresEscolas2 WHERE pes_id = @pes_id AND esc_codigo LIKE ('%' + @Codigo + '%')))
		BEGIN
			UPDATE ##ProfessoresEscolas2
			SET tur_codigo = tur_codigo + ' / ' + @AnoEscolar
			WHERE pes_id = @pes_id AND
				  esc_codigo LIKE ('%' + @Codigo + '%') AND
				  tur_codigo NOT LIKE ('%' + @AnoEscolar + '%')
		END
		ELSE IF (EXISTS (SELECT 1 FROM ##ProfessoresEscolas2 WHERE pes_id = @pes_id))
		BEGIN		
			UPDATE ##ProfessoresEscolas2
			SET esc_codigo = esc_codigo + ' / ' + @Codigo,
				esc_nome = esc_nome + ' / ' + @Escola,
				tur_codigo = tur_codigo + ' / ' + @AnoEscolar
			WHERE pes_id = @pes_id
		END
		ELSE
		BEGIN
			INSERT INTO ##ProfessoresEscolas2
			SELECT @pes_id,
				   @Codigo,
				   @Escola,
				   @AnoEscolar
		END
        FETCH NEXT FROM lista INTO 
            @pes_id,
			@Codigo,
			@Escola,
			@AnoEscolar;
    END;
 
CLOSE lista;
 
DEALLOCATE lista;