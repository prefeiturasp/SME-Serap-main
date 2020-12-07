DROP TABLE #tempMatriculas;
DROP TABLE #tempMatriculasQuePrecisamSerAjustadas;

SELECT
	prodan.cd_aluno AS AluEolProdan,
	prodan.cd_turma_escola AS TurmaEOLProdan,
	prodan.dc_turma_escola AS TurmaCodigoProdan,
	alu.alu_id AS AluIdSGP,
	alu.alu_matricula AS AluEolSGP,
	mtu.mtu_id AS MatriculaId,
	mtu.tur_id AS TurmaIdMatricula,
	mtu.esc_id AS MatriculaEscolaId,
	mtu.mtu_situacao AS SituacaoMatricula,
	turma_sgp.tur_codigo AS TurmaCodigoSGP,
	turma_pedagogica.tur_id AS TurmaIdPedagogica,
	turma_pedagogica.tur_codigoEOL AS TurmaEOLPedagogica,
	turma_pedagogica.esc_id AS TurmaEscolaIdPedagogica,
	turma_pedagogica.tur_codigo AS TurmaCodigoPedagogica,
	tcr.cur_id  AS CurId,
	tcr.crr_id AS CcrId,
	tcr.crp_id AS CrpId,
	tcr.tcp_id AS TcpId
INTO #tempMatriculas
FROM 
	AlunosETurmasDo7Ao9EnsinoFundamental prodan (NOLOCK)
LEFT JOIN
	ACA_Aluno alu (NOLOCK)
	on prodan.cd_aluno = alu.alu_matricula
LEFT JOIN
	MTR_MatriculaTurma mtu (NOLOCK)
	ON alu.alu_id = mtu.alu_id
LEFT JOIN
	TUR_Turma turma_sgp (NOLOCK)
	ON mtu.tur_id = turma_sgp.tur_id
LEFT JOIN
	GestaoPedagogica..TUR_Turma turma_pedagogica (NOLOCK)
	ON prodan.cd_turma_escola = turma_pedagogica.tur_codigoEOL
LEFT JOIN
	TUR_TurmaCurriculo tcr (NOLOCK)
	ON turma_pedagogica.tur_id = tcr.tur_id
WHERE
	mtu.mtu_situacao = 1 AND
	tcr.tcr_situacao = 1 AND
	YEAR(mtu_dataMatricula) = 2020;

SELECT 
	*
--INTO #tempMatriculasQuePrecisamSerAjustadas
FROM 
	#tempMatriculas
WHERE
	TurmaIdMatricula <> TurmaIdPedagogica
	OR 
	MatriculaEscolaId <> TurmaEscolaIdPedagogica;

SELECT * FROM #tempMatriculasQuePrecisamSerAjustadas
WHERE
	TurmaIdMatricula <> TurmaIdPedagogica

BEGIN TRAN
UPDATE t1
SET
	t1.tur_id = t2.TurmaIdPedagogica,
	t1.esc_id = t2.TurmaEscolaIdPedagogica,
	t1.crp_id = t2.CrpId,
	t1.crr_id = t2.CcrId,
	t1.cur_id = t2.CurId,
	t1.tcp_id = t2.TcpId
FROM
	MTR_MatriculaTurma t1
INNER JOIN
	#tempMatriculasQuePrecisamSerAjustadas t2
	ON t1.mtu_id = t2.MatriculaId
	AND t1.alu_id = t2.AluIdSGP
--COMMIT
--ROLLBACK

