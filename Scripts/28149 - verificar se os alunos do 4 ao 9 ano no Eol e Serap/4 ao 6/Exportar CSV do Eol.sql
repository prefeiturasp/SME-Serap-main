DROP TABLE #serie_ensino
DROP TABLE #Turmas
DROP TABLE #AlunosETurmas

select * INTO #serie_ensino from serie_ensino
where
cd_modalidade_ensino  IN (1,2) AND
cd_etapa_ensino IN (12,13,4, 5) AND
(dc_serie_ensino like '%4%' OR
dc_serie_ensino like '%5%' OR
dc_serie_ensino like '%6%')
AND dt_fim IS NULL
order by dc_serie_ensino

select
te.cd_escola,
te.cd_turma_escola,
te.dc_turma_escola,
te.an_letivo,              
tp.dc_tipo_periodicidade,
tt.dc_tipo_turno,
te.st_turma_escola,
te.cd_tipo_turma,
te.dt_inicio_turma,
te.dt_fim_turma,
se.nr_ordem_serie,      
se.cd_modalidade_ensino,
se.cd_etapa_ensino,    
ee.nr_ordem_etapa,  
se.cd_serie_ensino,      
se.dc_serie_ensino,      
me.sg_modalidade_ensino
INTO #Turmas
FROM turma_escola te
inner join serie_turma_grade stg
on te.cd_turma_escola = stg.cd_turma_escola
inner join #serie_ensino se
on stg.cd_serie_ensino = se.cd_serie_ensino
inner join modalidade_ensino me
on se.cd_modalidade_ensino = me.cd_modalidade_ensino
inner join tipo_periodicidade tp
on te.cd_tipo_periodicidade = tp.cd_tipo_periodicidade
inner join tipo_turno tt
on te.cd_tipo_turno = tt.cd_tipo_turno
inner join etapa_ensino ee
on se.cd_etapa_ensino = ee.cd_etapa_ensino
Where te.an_letivo = 2020 and stg.dt_fim IS NULL


select
aluno.cd_aluno,
aluno.nm_aluno,
CASE
WHEN mte.cd_situacao_aluno = 1 THEN 'Ativo'
WHEN mte.cd_situacao_aluno = 2 THEN 'Desistente'
WHEN mte.cd_situacao_aluno = 3 THEN 'Transferido'
WHEN mte.cd_situacao_aluno = 4 THEN 'Vínculo Indevido'
WHEN mte.cd_situacao_aluno = 5 THEN 'Concluído'
WHEN mte.cd_situacao_aluno = 6 THEN 'Pendente de Rematrícula'
WHEN mte.cd_situacao_aluno = 7 THEN 'Falecido'
WHEN mte.cd_situacao_aluno = 8 THEN 'Não Compareceu'
WHEN mte.cd_situacao_aluno = 10 THEN 'Rematriculado'
WHEN mte.cd_situacao_aluno = 11 THEN 'Deslocamento'
WHEN mte.cd_situacao_aluno = 12 THEN 'Cessado'
WHEN mte.cd_situacao_aluno = 13 THEN 'Sem continuidade'
WHEN mte.cd_situacao_aluno = 14 THEN 'Remanejado Saída'
WHEN mte.cd_situacao_aluno = 15 THEN 'Reclassificado Saída'
ELSE 'Fora do domínio liberado pela PRODAM'
END SituacaoMatricula,
mte.nr_chamada_aluno,
t.cd_escola,
t.cd_turma_escola,
t.dc_turma_escola,
t.an_letivo,              
t.dc_tipo_periodicidade,
t.dc_tipo_turno,
t.st_turma_escola,
t.cd_tipo_turma,
t.dt_inicio_turma,
t.dt_fim_turma,
t.nr_ordem_serie,      
t.cd_modalidade_ensino,
t.cd_etapa_ensino,    
t.nr_ordem_etapa,  
t.cd_serie_ensino,      
t.dc_serie_ensino,      
t.sg_modalidade_ensino
INTO #AlunosETurmas
FROM v_aluno_cotic aluno
INNER JOIN v_matricula_cotic matr ON aluno.cd_aluno = matr.cd_aluno
INNER JOIN matricula_turma_escola mte ON matr.cd_matricula = mte.cd_matricula
INNER JOIN #Turmas t ON t.cd_turma_escola = mte.cd_turma_escola
WHERE matr.an_letivo = 2020 and matr.st_matricula=1 AND mte.cd_situacao_aluno NOT IN (2,3,4,8,11,14,15)
--and t.cd_turma_escola=2125737
order by nm_aluno

SELECT * FROM #AlunosETurmas
order by nm_aluno
