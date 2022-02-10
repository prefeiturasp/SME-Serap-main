use se1426;

SELECT
aluno.cd_aluno CodigoAluno
,matrTurma.cd_matricula
,matrTurma.cd_turma_escola
,se.sg_resumida_serie as ano_turma
,MAX(matrTurma.dt_situacao_aluno) dt_situacao_aluno
into #DADOS
FROM
	v_historico_matricula_cotic matricula
INNER JOIN v_aluno_cotic aluno ON
	matricula.cd_aluno = aluno.cd_aluno
INNER JOIN historico_matricula_turma_escola matrTurma ON
	matricula.cd_matricula = matrTurma.cd_matricula
INNER JOIN turma_escola turesc ON
	matrTurma.cd_turma_escola = turesc.cd_turma_escola
INNER JOIN escola e ON
	turesc.cd_escola = e.cd_escola
INNER JOIN serie_turma_escola ste ON
	ste.cd_turma_escola = turesc.cd_turma_escola
INNER JOIN serie_ensino se ON 
	se.cd_serie_ensino = ste.cd_serie_ensino
WHERE
matrTurma.cd_situacao_aluno IN (1, 5, 6, 10, 13)
AND e.tp_escola IN (1, 3, 4, 13, 16)
AND matricula.an_letivo = 2021
and se.cd_etapa_ensino IN ( 4, 5, 12, 13 )
and se.sg_resumida_serie in ('2','3','4','5','6','7','8','9','S')
group by
aluno.cd_aluno
,matrTurma.cd_matricula
,matrTurma.cd_turma_escola
,se.sg_resumida_serie

select
CASE WHEN v_ue.cd_unidade_administrativa_referencia = '108300' THEN	'CS'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108200' THEN	'CL'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108700' THEN	'IQ'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108400' THEN	'FO'
 WHEN v_ue.cd_unidade_administrativa_referencia = '109200' THEN	'SM'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108500' THEN	'G '
 WHEN v_ue.cd_unidade_administrativa_referencia = '108900' THEN	'PE'
 WHEN v_ue.cd_unidade_administrativa_referencia = '109100' THEN	'SA'
 WHEN v_ue.cd_unidade_administrativa_referencia = '109000' THEN	'PJ'
 WHEN v_ue.cd_unidade_administrativa_referencia = '109300' THEN	'MP'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108100' THEN	'BT'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108600' THEN	'IP'
 WHEN v_ue.cd_unidade_administrativa_referencia = '108800' THEN	'JT' ELSE '--'
END sigla_dre,
e.cd_escola codigo_eol_ue,
te.sg_tp_escola + ' ' + v_ue.nm_unidade_educacao nome_ue,
a.ano_turma,
t.cd_turma_escola codigo_eol_turma, 
t.dc_turma_escola nome_turma,
COUNT(a.CodigoAluno) quantidade_alunos
from #DADOS a
inner join turma_escola t
inner join escola e on t.cd_escola = e.cd_escola
on a.cd_turma_escola = t.cd_turma_escola
inner join v_cadastro_unidade_educacao v_ue
on e.cd_escola = v_ue.cd_unidade_educacao
inner join tipo_escola te on te.tp_escola = e.tp_escola
group by 
v_ue.cd_unidade_administrativa_referencia,
e.cd_escola,
te.sg_tp_escola,
v_ue.nm_unidade_educacao,
a.ano_turma,t.cd_turma_escola,
t.dc_turma_escola

drop table #DADOS

