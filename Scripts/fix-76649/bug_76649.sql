use GestaoAvaliacao_SGP

declare 
@ent_id uniqueidentifier = '6CF424DC-8EC3-E011-9B36-00155D033206', 
@cal_ano int = 2022,
@server varchar(20) 'linked server sme'

	
	;WITH CteDestinationTurma AS (
		Select * from GestaoAvaliacao_SGP..TUR_Turma
		WHERE tur_id IN ( 
			Select t.tur_id from GestaoAvaliacao_SGP..TUR_Turma t 
			INNER JOIN GestaoAvaliacao_SGP..TUR_TurmaCurriculo tc ON tc.tur_id = t.tur_id
			INNER JOIN GestaoAvaliacao_SGP..ACA_Curso c ON c.cur_id = tc.cur_id 
			where c.tme_id IN (4,6,7,8)
		)
	)
		
	 -- TUR_Turma
    MERGE INTO CteDestinationTurma Destino
    USING (
	
	select tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, trn.ttn_id,
                  tur.tur_situacao, tur.tur_dataCriacao, tur.tur_dataAlteracao, tur.tur_tipo, tur.tur_codigoEOL
             from [@server].[GestaoPedagogica].[dbo].[TUR_Turma] tur with (nolock)
                  inner join [@server].[GestaoPedagogica].[dbo].[ACA_Turno] trn with (nolock)
                  on tur.trn_id = trn.trn_id
                  inner join GestaoAvaliacao_SGP..ACA_TipoTurno ttn
                  on trn.ttn_id = ttn.ttn_id
                  inner join GestaoAvaliacao_SGP..ESC_Escola esc
                  on tur.esc_id = esc.esc_id
                  inner join GestaoAvaliacao_SGP..ACA_CalendarioAnual cal
                  on tur.cal_id = cal.cal_id
				  INNER JOIN [@server].[GestaoPedagogica].[dbo].[TUR_TurmaCurriculo] tc ON tc.tur_id = tur.tur_id
			      INNER JOIN GestaoAvaliacao_SGP..ACA_Curso c ON c.cur_id = tc.cur_id
            where tur.tur_situacao <> 3
              and trn.trn_situacao <> 3
              and ttn.ttn_situacao <> 3
              and esc.ent_id = @ent_id
              and esc.esc_situacao <> 3
              and cal.cal_ano = @cal_ano
              and cal.ent_id = @ent_id
              and cal.cal_situacao <> 3
			  and tur_tipo IN (1,6)
			  and c.tme_id IN (4,6,7,8)
			  and cal.cal_id in (34,35,36)
            group by tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, trn.ttn_id,
                  tur.tur_situacao, tur.tur_dataCriacao, tur.tur_dataAlteracao, tur.tur_tipo, tur.tur_codigoEOL
				  
				  ) Origem
    ON Destino.tur_id = Origem.tur_id
    WHEN MATCHED
         AND ((Destino.tur_codigo COLLATE DATABASE_DEFAULT <> Origem.tur_codigo COLLATE DATABASE_DEFAULT)
               OR
              (Destino.tur_descricao COLLATE DATABASE_DEFAULT <> Origem.tur_descricao COLLATE DATABASE_DEFAULT)
               OR
              (Destino.ttn_id <> Origem.ttn_id)
               OR
              (Destino.tur_situacao <> Origem.tur_situacao)
			  OR
			  (Destino.tur_codigo_eol is null))
         THEN
         UPDATE SET tur_codigo = Origem.tur_codigo,
                    tur_descricao = Origem.tur_descricao,
                    ttn_id = Origem.ttn_id,
                    tur_situacao = Origem.tur_situacao,
                    tur_dataAlteracao = Origem.tur_dataAlteracao,
					tur_codigo_eol = Origem.tur_codigoEOL
    WHEN NOT MATCHED THEN
         INSERT (tur_id, esc_id, tur_codigo, tur_descricao, cal_id, ttn_id,
                 tur_situacao, tur_dataCriacao, tur_dataAlteracao, tur_tipo, tur_codigo_eol)
         VALUES (Origem.tur_id, Origem.esc_id, Origem.tur_codigo, Origem.tur_descricao, Origem.cal_id, Origem.ttn_id,
                 Origem.tur_situacao, Origem.tur_dataCriacao, Origem.tur_dataAlteracao, Origem.tur_tipo, Origem.tur_codigoEOL);    
    
    
MERGE INTO GestaoAvaliacao_SGP..TUR_Turma Destino
    USING (
select t.tur_id, t.esc_id, t.tur_codigo, t.tur_descricao, t.cal_id, t.ttn_id,
t.tur_situacao, t.tur_dataCriacao, t.tur_dataAlteracao, t.tur_tipo, t.tur_codigo_eol 
from [@server].[GestaoAvaliacao_SGP].[dbo].[TUR_Turma] t
where t.cal_id in (34,35,36)
and t.tur_situacao <> 3
) Origem
ON Destino.tur_id = Origem.tur_id
WHEN MATCHED
        AND ((Destino.tur_codigo COLLATE DATABASE_DEFAULT <> Origem.tur_codigo COLLATE DATABASE_DEFAULT)
            OR
            (Destino.tur_descricao COLLATE DATABASE_DEFAULT <> Origem.tur_descricao COLLATE DATABASE_DEFAULT)
            OR
            (Destino.ttn_id <> Origem.ttn_id)
            OR
            (Destino.tur_situacao <> Origem.tur_situacao)
			OR
			(Destino.tur_codigo_eol is null))
        THEN
        UPDATE SET tur_codigo = Origem.tur_codigo,
                tur_descricao = Origem.tur_descricao,
                ttn_id = Origem.ttn_id,
                tur_situacao = Origem.tur_situacao,
                tur_dataAlteracao = Origem.tur_dataAlteracao,
				tur_codigo_eol = Origem.tur_codigo_EOL
WHEN NOT MATCHED THEN
        INSERT (tur_id, esc_id, tur_codigo, tur_descricao, cal_id, ttn_id,
                tur_situacao, tur_dataCriacao, tur_dataAlteracao, tur_tipo, tur_codigo_eol)
        VALUES (Origem.tur_id, Origem.esc_id, Origem.tur_codigo, Origem.tur_descricao, Origem.cal_id, Origem.ttn_id,
                Origem.tur_situacao, Origem.tur_dataCriacao, Origem.tur_dataAlteracao, Origem.tur_tipo, Origem.tur_codigo_EOL);



MERGE INTO GestaoAvaliacao_SGP..ESC_Escola Destino
USING (select esc_id, ent_id, uad_id, esc_codigo, esc_nome, esc_situacao,
                esc_dataCriacao, esc_dataAlteracao, uad_idSuperiorGestao
            from [@server].[GestaoAvaliacao_SGP].[dbo].[ESC_Escola] with (nolock)
        where ent_id = '6CF424DC-8EC3-E011-9B36-00155D033206'
            and esc_situacao <> 3) Origem
ON Destino.esc_id = Origem.esc_id and Destino.esc_codigo = Origem.esc_codigo
WHEN MATCHED
        AND ((Destino.esc_codigo COLLATE DATABASE_DEFAULT <> Origem.esc_codigo COLLATE DATABASE_DEFAULT)
            OR
            (REPLACE(Destino.esc_nome,'''','`') COLLATE DATABASE_DEFAULT <> REPLACE(Origem.esc_nome,'''','`') COLLATE DATABASE_DEFAULT)
            OR
            (Destino.uad_idSuperiorGestao <> Origem.uad_idSuperiorGestao)
            OR
            (Destino.esc_situacao <> Origem.esc_situacao))
        THEN
        UPDATE SET esc_codigo = Origem.esc_codigo,
                esc_nome = REPLACE(Origem.esc_nome,'''','`'),
                uad_idSuperiorGestao = Origem.uad_idSuperiorGestao,
                esc_situacao = Origem.esc_situacao,
                esc_dataAlteracao = Origem.esc_dataAlteracao
WHEN NOT MATCHED THEN
        INSERT (esc_id, ent_id, uad_id, esc_codigo, esc_nome, esc_situacao,
                esc_dataCriacao, esc_dataAlteracao, uad_idSuperiorGestao)
        VALUES (Origem.esc_id, Origem.ent_id, Origem.uad_id, Origem.esc_codigo, REPLACE(Origem.esc_nome,'''','`'),
                Origem.esc_situacao, Origem.esc_dataCriacao, Origem.esc_dataAlteracao, Origem.uad_idSuperiorGestao);

merge into TUR_TurmaTipoCurriculoPeriodo Destino
using (select ttc.* 
from [@server].[GestaoAvaliacao_SGP].[dbo].[TUR_TurmaTipoCurriculoPeriodo] ttc
inner join TUR_Turma t on ttc.tur_id = t.tur_id
where t.cal_id in (34,35,36)
and t.tur_situacao <> 3) Origem
on Destino.tur_id = Origem.tur_id
WHEN NOT MATCHED THEN
insert (tur_id,cur_id,tme_id,tne_id,crp_ordem,ttcr_situacao,esc_id)
values (Origem.tur_id,Origem.cur_id,Origem.tme_id,Origem.tne_id,Origem.crp_ordem,Origem.ttcr_situacao,Origem.esc_id);

merge into TUR_TurmaCurriculo Destino
using (select a.* 
from [@server].[GestaoAvaliacao_SGP].[dbo].[TUR_TurmaCurriculo] a
inner join TUR_Turma t on a.tur_id = t.tur_id
where t.cal_id in (34,35,36)
and t.tur_situacao <> 3) Origem
on Destino.tur_id = Origem.tur_id
WHEN NOT MATCHED THEN
insert (tur_id,cur_id,crr_id,crp_id,tcr_situacao,tcr_dataCriacao,tcr_dataAlteracao,tcp_id)
values (
Origem.tur_id,
Origem.cur_id,
Origem.crr_id,
Origem.crp_id,
Origem.tcr_situacao,
Origem.tcr_dataCriacao,
Origem.tcr_dataAlteracao,
Origem.tcp_id);

merge into aca_tipocurriculoperiodo Destino
using (select * from [@server].[GestaoAvaliacao_SGP].[dbo].[aca_tipocurriculoperiodo]) Origem
on Destino.tcp_id = Origem.tcp_id
WHEN NOT MATCHED THEN
insert (tcp_id,tne_id,tme_id,tcp_descricao,tcp_ordem,tcp_situacao,tcp_dataCriacao,tcp_dataAlteracao) 
values (
Origem.tcp_id,
Origem.tne_id,
Origem.tme_id,
Origem.tcp_descricao,
Origem.tcp_ordem,
Origem.tcp_situacao,
getdate(),getdate());


merge into aca_curso Destino
using (select * from [@server].[GestaoAvaliacao_SGP].[dbo].[aca_curso] where cur_situacao = 1) Origem
on Destino.cur_id = Origem.cur_id
WHEN NOT MATCHED THEN
insert (cur_id,ent_id,tne_id,tme_id,cur_codigo,cur_nome,cur_nome_abreviado,cur_situacao,
cur_dataCriacao,cur_dataAlteracao)
values (
Origem.cur_id,
Origem.ent_id,
Origem.tne_id,
Origem.tme_id,
Origem.cur_codigo,
Origem.cur_nome,
Origem.cur_nome_abreviado,
Origem.cur_situacao,
getdate(),getdate());

merge into aca_curriculoperiodo Destino
using (select * from [@server].[GestaoAvaliacao_SGP].[dbo].[aca_curriculoperiodo] where crp_situacao = 1) Origem
on Destino.cur_id = Origem.cur_id and Destino.crr_id = Origem.crr_id and Destino.crp_id = Origem.crp_id
WHEN NOT MATCHED THEN
insert (cur_id,crr_id,crp_id,crp_ordem,crp_descricao,crp_situacao,crp_dataCriacao,
crp_dataAlteracao,tcp_id)
values (
Origem.cur_id,
Origem.crr_id,
Origem.crp_id,
Origem.crp_ordem,
Origem.crp_descricao,
Origem.crp_situacao,
getdate(),
getdate(),
Origem.tcp_id);

merge into SGP_TUR_TurmaCurriculo Destino
using (
select distinct
 tt.tur_id
,tc.cur_id
,tc.crr_id
,tc.crp_id
,tc.tcr_prioridade
,tc.tcr_situacao
,tc.tcr_dataCriacao
,tc.tcr_dataAlteracao
,_tcp.tcp_id
from [@server].[GestaoPedagogica].[dbo].[TUR_Turma] t
inner join [@server].[GestaoPedagogica].[dbo].[TUR_TurmaCurriculo] tc
on t.tur_id = tc.tur_id
inner join SGP_TUR_Turma tt on tt.tur_codigo_eol = t.tur_codigoEOL
inner join [@server].[GestaoPedagogica].[dbo].[ACA_CurriculoPeriodo] cp 
on tc.cur_id = cp.cur_id
and tc.crr_id = cp.crr_id
and tc.crp_id = cp.crp_id
inner join [@server].[GestaoPedagogica].[dbo].[ACA_TipoCurriculoPeriodo] _tcp
on _tcp.tcp_id = cp.tcp_id
inner join sgp_aca_curriculoperiodo sgp_cp 
on sgp_cp.cur_id = cp.cur_id 
and sgp_cp.crr_id = cp.crr_id 
and sgp_cp.crp_id = cp.crp_id
and sgp_cp.tcp_id = _tcp.tcp_id
where tc.cur_id in (161,163)
and tc.tcr_situacao = 1
and t.tur_situacao = 1
and tt.tur_situacao = 1
) Origem
on Destino.tur_id = Origem.tur_id
and Destino.cur_id = Origem.cur_id
and Destino.crr_id = Origem.crr_id
and Destino.crp_id = Origem.crp_id
and Destino.tcr_situacao = 1
WHEN NOT MATCHED THEN
insert (tur_id,cur_id,crr_id,crp_id,tcr_situacao,tcr_dataCriacao,tcr_dataAlteracao,tcp_id)
values (
Origem.tur_id,
Origem.cur_id,
Origem.crr_id,
Origem.crp_id,
Origem.tcr_situacao,
Origem.tcr_dataCriacao,
Origem.tcr_dataAlteracao,
Origem.tcp_id);


merge into SGP_TUR_TurmaTipoCurriculoPeriodo Destino
using (
select t.tur_id,tc.cur_id, c.tme_id,c.tne_id,tc.crp_id crp_ordem,1 ttcr_situacao,t.esc_id  
from SGP_TUR_TurmaCurriculo tc
inner join SGP_ACA_Curso c on tc.cur_id = c.cur_id
inner join SGP_TUR_Turma t on t.tur_id = tc.tur_id
where tc.cur_id in (161,163)
AND tc.tcr_situacao = 1) Origem
on Destino.tur_id = Origem.tur_id
and Destino.cur_id = Origem.cur_id
and Destino.tme_id = Origem.tme_id
and Destino.tne_id = Origem.tne_id
and Destino.crp_ordem = Origem.crp_ordem
and Destino.ttcr_situacao = Origem.ttcr_situacao
and Destino.esc_id = Origem.esc_id
WHEN NOT MATCHED THEN
insert (tur_id,cur_id,tme_id,tne_id,crp_ordem,ttcr_situacao,esc_id)
values (
Origem.tur_id,
Origem.cur_id,
Origem.tme_id,
Origem.tne_id,
Origem.crp_ordem,
Origem.ttcr_situacao,
Origem.esc_id);


merge into testcurriculumgrade Destino
using (
select distinct 
tcg.tcp_id TypeCurriculumGradeId, 
getdate() CreateDate, getdate() UpdateDate, 1 State, t.Id Test_Id
from 
SGP_TUR_TurmaTipoCurriculoPeriodo ttcp (nolock)	
INNER JOIN SGP_TUR_TurmaCurriculo tc (nolock)
	ON ttcp.tur_id = tc.tur_id AND
		ttcp.crp_ordem = tc.crp_id AND
		tc.tcr_situacao = 1
INNER JOIN sgp_aca_tipocurriculoperiodo tcg (nolock)
    ON tcg.tcp_id = tc.tcp_id AND tc.tcr_situacao = 1
INNER JOIN testtypecourse ttc (nolock)
	ON tc.cur_id = ttc.CourseId AND		
		ttc.state = 1
INNER JOIN sgp_aca_curso cur (nolock)
    ON cur.cur_id = tc.cur_id AND
		ttcp.tme_id = cur.tme_id
INNER JOIN sgp_aca_curriculoperiodo crp (nolock)
    ON crp.cur_id = cur.cur_id AND
		crp.crp_ordem = tcg.tcp_ordem AND
		crp.tcp_id = tcg.tcp_id
inner join Test t on t.TestType_Id = ttc.TestType_Id
where ttcp.ttcr_situacao = 1
and tc.cur_id in (161,163)
and ttc.testtype_id = 21
and t.State = 1) Origem
on Destino.TypeCurriculumGradeId = Origem.TypeCurriculumGradeId
and Destino.State = Origem.State
and Destino.Test_Id = Origem.Test_Id
WHEN NOT MATCHED THEN
insert (TypeCurriculumGradeId,CreateDate,UpdateDate,State,Test_Id)
values (
Origem.TypeCurriculumGradeId,
Origem.CreateDate,
Origem.UpdateDate,
Origem.State,
Origem.Test_Id);