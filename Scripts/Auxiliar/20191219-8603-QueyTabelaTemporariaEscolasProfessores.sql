ALTER FUNCTION FN_CodigosEscolasProfessor
(
	@pes_id UNIQUEIDENTIFIER,
	@ano_letivo INT
)
RETURNS @Tabela TABLE(Codigos VARCHAR(1000),
					  Escolas VARCHAR(MAX))
AS
BEGIN
	DECLARE @CodigosEscolas VARCHAR(1000) = ''
	DECLARE @Escolas VARCHAR(1000) = ''
	DECLARE @tmpTable TABLE (Codigo VARCHAR(1000),
							 Escola VARCHAR(MAX),
							 Considerada BIT)

	INSERT INTO @tmpTable
	SELECT DISTINCT e.esc_codigo, 
					e.esc_nome,
					'False' considerada	
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
	WHERE d.pes_id = @pes_id AND
		  t.tur_situacao <> 3 AND
		  ca.cal_ano = @ano_letivo	

	WHILE (EXISTS (SELECT Codigo FROM @tmpTable WHERE Considerada = 'False'))
	BEGIN
		SELECT TOP 1 @CodigosEscolas = @CodigosEscolas + Codigo + ' / ',
					 @Escolas = @Escolas + Escola + ' / '
		FROM @tmpTable
		WHERE Considerada = 'False'
		
		UPDATE TOP (1) @tmpTable
		SET considerada = 'True'
		WHERE considerada = 'False'
		
	END			
		   
	INSERT INTO @Tabela
	VALUES (LEFT(@CodigosEscolas, LEN(@CodigosEscolas) - 1), 
		    LEFT(@Escolas, LEN(@Escolas) - 1))

	RETURN
	
END
