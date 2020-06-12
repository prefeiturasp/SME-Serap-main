declare @turmas_eja table (cd_escola char(6),
						   cd_turma_escola int,
						   dc_turma_escola varchar(15),
						   an_letivo int,
						   dc_tipo_periodicidade varchar(40),
						   dc_tipo_turno varchar(20),
						   st_turma_escola char(1),
						   cd_tipo_turma int,
						   dt_inicio_turma datetime, 
						   dt_fim_turma datetime,
						   nr_ordem_serie smallint,
						   cd_modalidade_ensino int,
						   cd_etapa_ensino int,
						   nr_ordem_etapa smallint,
						   dc_serie_ensino varchar(40),
						   sg_modalidade_ensino varchar(10),
						   sg_tp_escola char(12))
						   
insert into @turmas_eja
select te.cd_escola,
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
       se.dc_serie_ensino,       
       me.sg_modalidade_ensino,
       NULL sg_tp_escola
	from turma_escola te
		inner join serie_turma_grade stg
			on te.cd_turma_escola = stg.cd_turma_escola		
		inner join serie_ensino se
			on stg.cd_serie_ensino = se.cd_serie_ensino
		inner join modalidade_ensino me
			on se.cd_modalidade_ensino = me.cd_modalidade_ensino
		inner join tipo_periodicidade tp
			on te.cd_tipo_periodicidade = tp.cd_tipo_periodicidade	
		inner join tipo_turno tt
			on te.cd_tipo_turno = tt.cd_tipo_turno	
		inner join etapa_ensino ee
			on se.cd_etapa_ensino = ee.cd_etapa_ensino	
where te.an_letivo = '2019' and
	  te.st_turma_escola = 'O' and
	  se.dc_serie_ensino like '%eja%'
	  
union	  
	  
select te.cd_escola,
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
       se.dc_serie_ensino,       
       me.sg_modalidade_ensino,
       tpe.sg_tp_escola       
	from turma_escola te
		inner join escola e
			on te.cd_escola = e.cd_escola
		inner join tipo_escola tpe
			on e.tp_escola = tpe.tp_escola
		inner join serie_turma_grade stg
			on te.cd_turma_escola = stg.cd_turma_escola		
		inner join serie_ensino se
			on stg.cd_serie_ensino = se.cd_serie_ensino
		inner join modalidade_ensino me
			on se.cd_modalidade_ensino = me.cd_modalidade_ensino	
		inner join tipo_periodicidade tp
			on te.cd_tipo_periodicidade = tp.cd_tipo_periodicidade		
		inner join tipo_turno tt
			on te.cd_tipo_turno = tt.cd_tipo_turno
		inner join etapa_ensino ee
			on se.cd_etapa_ensino = ee.cd_etapa_ensino	
where te.an_letivo = '2019' and
	  te.st_turma_escola = 'O' and
	  tpe.sg_tp_escola = 'CIEJA'
order by te.cd_escola	

select * from @turmas_eja
	
select distinct a.cd_aluno,
	            m.dt_status_matricula,
				m.st_matricula
	from @turmas_eja te
		inner join v_matricula_cotic m
			on te.cd_escola = m.cd_escola
		inner join v_aluno_cotic a
			on m.cd_aluno = a.cd_aluno
where m.an_letivo = 2019
order by cd_aluno
	


