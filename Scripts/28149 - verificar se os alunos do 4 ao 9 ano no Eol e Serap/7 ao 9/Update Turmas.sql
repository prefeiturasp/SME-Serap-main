SELECT
	DISTINCT tur_id
INTO
	#tempTurmasProdan
FROM
	AlunosETurmasDo7Ao9EnsinoFundamental;

SELECT
	'Prodan' AS DB1,
	prodan.tur_id AS tur_id_prodan,
	'Gestão Pedagogica' AS DB2,
	tur_peda.tur_id AS tur_id_pedagogica,
	tur_peda.esc_id AS esc_id_pedagogica,
	tur_peda.tur_codigo AS tur_codigo_pedagogica,
	tur_peda.cal_id AS cal_id_pedagogica,
	'SGP' AS DB2,
	tur_sgp.tur_id AS tur_id_sgp,
	tur_sgp.esc_id AS esc_id_sgp,
	tur_sgp.tur_codigo AS tur_codigo_sgp,
	tur_sgp.cal_id AS cal_id_sgp,
	CASE WHEN tur_peda.cal_id <> tur_sgp.cal_id THEN 1 ELSE 0  END 'Calendário diferente?',
	CASE WHEN tur_peda.esc_id <> tur_sgp.esc_id THEN 1 ELSE 0  END 'Escola diferente?'
INTO #tempTurmasSerapParaSeremAjustadas
FROM
	#tempTurmasProdan prodan (NOLOCK)
INNER JOIN
	GestaoPedagogica..TUR_Turma tur_peda (NOLOCK)
	ON prodan.tur_id = tur_peda.tur_id
INNER JOIN
	TUR_Turma tur_sgp (NOLOCK)
	ON tur_peda.tur_id = tur_sgp.tur_id
WHERE
	tur_peda.cal_id <> tur_sgp.cal_id
	OR tur_peda.esc_id <> tur_sgp.esc_id;

BEGIN
UPDATE
	t1
SET
	t1.cal_id = t2.cal_id_pedagogica,
	t1.esc_id = t2.esc_id_pedagogica
FROM
	TUR_Turma t1
INNER JOIN
	#tempTurmasSerapParaSeremAjustadas t2
	ON t1.tur_id = t2.tur_id_sgp;

--ROLLBACK
--COMMIT

