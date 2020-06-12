INSERT INTO ACA_Curso
SELECT ((SELECT TOP 1 cur_id FROM ACA_Curso ORDER BY cur_id DESC) + 1),
	   '6CF424DC-8EC3-E011-9B36-00155D033206',
	   tne_id,
	   tme_id,
	   3,
	   '2019 - EJA Modular',
	   'EJA MOD',
	   1,
	   GETDATE(),
	   GETDATE()
	FROM ACA_TipoNivelEnsino tne,
		 ACA_TipoModalidadeEnsino tme		 		  	
WHERE tne_ordem = 7 AND
	  tme.tme_nome = 'EJA Modular'
SELECT @@IDENTITY

select * from ACA_Curso where cur_nome like '%2019%' and cur_nome like '%EJA%'

--begin tran
--insert into ACA_TipoNivelEnsino
--values (4, 'EJA - Ensino Fundamental', 1, GETDATE(), GETDATE(), 7)

--insert into ACA_TipoNivelEnsino
--values (5, 'EJA - CIEJA', 1, GETDATE(), GETDATE(), 8)

--commit

SELECT COUNT(0)
	FROM TUR_TurmaTipoCurriculoPeriodo ttcp 		 
		 INNER JOIN ESC_Escola e
			ON ttcp.esc_id = e.esc_id
		 INNER JOIN ACA_TipoModalidadeEnsino tme
			ON ttcp.tme_id = tme.tme_id
		 INNER JOIN ACA_TipoNivelEnsino tne
			ON ttcp.tne_id = tne.tne_id
WHERE ttcp.tur_id = @tur_id AND
	  ttcp.cur_id = @cur_id AND
	  tme.tme_nome = @tme_nome AND
	  tne.tne_ordem = @tne_ordem AND
	  ttcp.crp_ordem = @crp_ordem AND
	  e.esc_codigo = @esc_codigo

INSERT INTO TUR_TurmaTipoCurriculoPeriodo
SELECT @tur_id,
	   @cur_id,
	   tme.tme_id,
	   tne.tne_id,
	   @crp_ordem,
	   1,
	   e.esc_id
	FROM TUR_TurmaTipoCurriculoPeriodo ttcp 		 
		 INNER JOIN ESC_Escola e
			ON ttcp.esc_id = e.esc_id
		 INNER JOIN ACA_TipoModalidadeEnsino tme
			ON ttcp.tme_id = tme.tme_id
		 INNER JOIN ACA_TipoNivelEnsino tne
			ON ttcp.tne_id = tne.tne_id
WHERE tme.tme_nome = @tme_nome AND
	  tne.tne_ordem = @tne_ordem AND	  
	  e.esc_codigo = @esc_codigo


INSERT INTO ACA_TipoCurriculoPeriodo
SELECT (SELECT TOP 1 tcp_id FROM ACA_TipoCurriculoPeriodo ORDER BY tcp_id DESC) + 1,
	   tne.tne_id,
	   tme.tme_id,
	   @tcp_descricao,
	   @tcp_ordem,
	   1,
	   GETDATE(),
	   GETDATE()
	FROM ACA_TipoCurriculoPeriodo tpcp
		INNER JOIN ACA_TipoNivelEnsino tne
			ON tpcp.tne_id = tne.tne_id
		INNER JOIN ACA_TipoModalidadeEnsino tme
			ON tpcp.tme_id = tme.tme_id
WHERE tne.tne_ordem = 7 AND
	  tme.tme_nome = 'EJA Regular' AND
	  tpcp.tcp_ordem = 2

SELECT *
	FROM ACA_TipoCurriculoPeriodo tpcp
		INNER JOIN ACA_TipoNivelEnsino tne
			ON tpcp.tne_id = tne.tne_id
		INNER JOIN ACA_TipoModalidadeEnsino tme
			ON tpcp.tme_id = tme.tme_id
where tpcp.tcp_descricao like '%EJA%'

select * from ACA_TipoModalidadeEnsino where tme_nome like '%EJA%'
select * 
	from TUR_TurmaTipoCurriculoPeriodo  ttcp
		inner join TUR_Turma t
			on ttcp.tur_id = t.tur_id		
where tme_id in (3,4,6,7,8)

begin tran
INSERT INTO TUR_Turma
SELECT ((SELECT TOP 1 tur_id FROM TUR_Turma ORDER BY tur_id DESC) + 1),
	   esc_id,
	   'EJA-1A',
	   NULL,
	   21,
	   tt.ttn_id,
	   1,
	   GETDATE(),
	   GETDATE(),
	   1
	FROM ESC_Escola e,
		 ACA_TipoTurno tt
WHERE e.esc_codigo = '200050' AND
	   tt.ttn_nome = 'Manhã'
SELECT @@IDENTITY


SELECT TOP 1 c.cur_id
	FROM ACA_Curso c
		INNER JOIN ACA_TipoModalidadeEnsino tme
			ON c.tme_id = tme.tme_id
WHERE c.cur_nome LIKE '%EJA%' AND
	   c.cur_nome LIKE '%2019%' AND
	   c.cur_codigo = 2 AND
	   tme.tme_nome = 'CIEJA'
DECLARE @tcp_id BIGINT = (SELECT TOP 1 tcp_id FROM ACA_TipoCurriculoPeriodo ORDER BY tcp_id DESC) + 1


SELECT 1,
	    tne.tne_id,
	    tme.tme_id,
	    'teste',
	    1,
	    1,
	    GETDATE(),
	    GETDATE()
	FROM ACA_TipoNivelEnsino tne,
		 ACA_TipoModalidadeEnsino tme
WHERE tne.tne_ordem = 8 AND
	   tme.tme_nome = 'M I'


begin tran
delete from ug
from CoreSSO.dbo.SYS_UsuarioGrupo ug
	inner join CoreSSO.dbo.SYS_Usuario u
		on ug.usu_id = u.usu_id
where CONVERT(date, u.usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from p
from CoreSSO.dbo.PES_Pessoa p
	inner join CoreSSO.dbo.SYS_Usuario u
		on p.pes_id = u.pes_id
where CONVERT(date, u.usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from CoreSSO.dbo.SYS_Usuario
where CONVERT(date, usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from MTR_MatriculaTurma
where CONVERT(date, mtu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete TUR_TurmaCurriculo
where CONVERT(date, tcr_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete ACA_CurriculoPeriodo
where CONVERT(date, crp_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from ttcp
from TUR_TurmaTipoCurriculoPeriodo ttcp
	inner join TUR_Turma t
		on ttcp.tur_id = t.tur_id
where CONVERT(date, t.tur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from TUR_Turma
where CONVERT(date, tur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete ACA_Curriculo
where CONVERT(date, crr_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from ACA_Curso
where CONVERT(date, cur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete ACA_TipoCurriculoPeriodo
where CONVERT(date, tcp_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete ACA_Aluno
where CONVERT(date, alu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from GestaoAvaliacao.dbo.TestTypeCourse
where CONVERT(date, CreateDate) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

delete from GestaoAvaliacao.dbo.TestCurriculumGrade
where CONVERT(date, CreateDate) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))


-- commit
-- rollback


select * from CoreSSO.dbo.SYS_UsuarioGrupo ug (nolock)
	inner join CoreSSO.dbo.SYS_Usuario u (nolock)
		on ug.usu_id = u.usu_id
where CONVERT(date, u.usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select *
from CoreSSO.dbo.PES_Pessoa p (nolock)
	inner join CoreSSO.dbo.SYS_Usuario u (nolock)
		on p.pes_id = u.pes_id
where CONVERT(date, u.usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from CoreSSO.dbo.SYS_Usuario (nolock)
where CONVERT(date, usu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from MTR_MatriculaTurma (nolock)
where CONVERT(date, mtu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from TUR_TurmaCurriculo (nolock)
where CONVERT(date, tcr_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from ACA_CurriculoPeriodo (nolock)
where CONVERT(date, crp_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from TUR_TurmaTipoCurriculoPeriodo ttcp (nolock)
	inner join TUR_Turma t (nolock)
		on ttcp.tur_id = t.tur_id
where CONVERT(date, t.tur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from TUR_Turma (nolock)
where CONVERT(date, tur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from ACA_Curso (nolock)
where CONVERT(date, cur_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from ACA_TipoCurriculoPeriodo (nolock)
where CONVERT(date, tcp_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from ACA_Aluno (nolock)
where CONVERT(date, alu_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from ACA_Curriculo (nolock)
where CONVERT(date, crr_dataCriacao) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from GestaoAvaliacao.dbo.TestTypeCourse (nolock)
where CONVERT(date, CreateDate) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

select * from GestaoAvaliacao.dbo.TestCurriculumGrade (nolock)
where CONVERT(date, CreateDate) in (CONVERT(date, '2019-12-04'), CONVERT(date, '2019-12-05'))

SELECT *
	FROM TUR_TurmaTipoCurriculoPeriodo ttcp (nolock)
		 INNER JOIN ESC_Escola e (nolock)
			ON ttcp.esc_id = e.esc_id
INNER JOIN ACA_TipoModalidadeEnsino tme
   ON ttcp.tme_id = tme.tme_id
INNER JOIN ACA_TipoNivelEnsino tne
   ON ttcp.tne_id = tne.tne_id
WHERE  ttcp.tur_id = 315357 AND
	   ttcp.cur_id = 101 AND
	   tme.tme_nome = 'CIEJA' AND
	   tne.tne_ordem = 8 AND
	   ttcp.crp_ordem = 5 AND
	   e.esc_codigo = 200050



select * from GestaoAvaliacao.dbo.TestCurriculumGrade where Test_Id = 521
select * from ACA_TipoCurriculoPeriodo (nolock)
where CONVERT(date, tcp_dataCriacao) = CONVERT(date, '2019-11-21')
select * from ACA_TipoCurriculoPeriodo where tcp_id in (65, 82)


select * from ACA_TipoNivelEnsino

select *
	from GestaoAvaliacao.dbo.Test
where Description like '%EJA%' and
	  ApplicationEndDate > GETDATE()


select distinct dc_serie_ensino
	from GestaoAvaliacao.dbo.TurmasEjaEol
order by dc_serie_ensino


SELECT TOP 1 *
	FROM ACA_Curso c
		INNER JOIN ACA_TipoModalidadeEnsino tme
			ON c.tme_id = tme.tme_id
WHERE c.cur_nome LIKE '%EJA%' AND
	   c.cur_nome LIKE '%2019%' AND
	   c.cur_codigo = 3 AND
	   tme.tme_nome = 'EJA Especial'


select distinct c.ent_id, 
				c.tne_id, 
				c.tme_id, 
				c.cur_codigo, 
				c.cur_nome, 
				c.cur_nome_abreviado, 
				c.cur_situacao,
				tme.tme_nome
	from ACA_Curso (nolock) c
		inner join ACA_TipoModalidadeEnsino tme
			on c.tme_id = tme.tme_id
where CONVERT(date, c.cur_dataCriacao) in (CONVERT(date, '2019-11-21'), CONVERT(date, '2019-11-22'))

SELECT TOP 1 c.cur_id, tme.tme_nome, c.cur_dataCriacao
	FROM ACA_Curso c (nolock)
		INNER JOIN ACA_TipoModalidadeEnsino tme (nolock)
			ON c.tme_id = tme.tme_id
WHERE c.cur_nome LIKE '%EJA%' AND
	   c.cur_nome LIKE '%2019%' AND
	   c.cur_codigo = 3 AND
	   tme.tme_nome = 'EJA Especial'

select * 
	from ACA_Curso (nolock) c
		inner join ACA_TipoModalidadeEnsino tme
			on c.tme_id = tme.tme_id
where CONVERT(date, c.cur_dataCriacao) in (CONVERT(date, '2019-11-22'))


select distinct dc_serie_ensino
	from GestaoAvaliacao.dbo.TurmasEjaEol
order by dc_serie_ensino


select * from ACA_TipoCurriculoPeriodo where tcp_id = 110
select * from ACA_TipoNivelEnsino where tne_ordem = 7
select * from ACA_TipoModalidadeEnsino where tme_nome = 'CIEJA'

select * from ACA_TipoCurriculoPeriodo where tcp_descricao = 'EJA FINAL II'


select distinct e.esc_nome
from TUR_TurmaTipoCurriculoPeriodo ttcp (nolock)
	inner join TUR_Turma t (nolock)
		on ttcp.tur_id = t.tur_id
	inner join ESC_Escola e
		on ttcp.esc_id = e.esc_id	
where CONVERT(date, t.tur_dataCriacao) in (CONVERT(date, '2019-11-22'))

select * from CoreSSO.dbo.SYS_Usuario where CONVERT(date, usu_dataCriacao) = CONVERT(date, '2019-11-22')
select * from CoreSSO.dbo.SYS_UsuarioGrupo where usu_id = 'ED46986A-D375-44BA-A6BA-FFFD24740DDE'

select * from CoreSSO.dbo.SYS_Grupo where gru_id = 'BD6D9CE6-9456-E711-9541-782BCB3D218E'
select * from GestaoAvaliacao_SGP.dbo.ACA_Aluno where pes_id = 'C93981DF-28A4-4956-9F0D-DECB1E6338B1'

select * from SGP_ACA_Aluno order by alu_id desc
select top 10 * from SGP_MTR_MatriculaTurma order by alu_id desc



INSERT INTO ACA_Aluno
SELECT ((SELECT TOP 1 alu_id FROM SGP_ACA_Aluno ORDER BY alu_id DESC) + 1),
	   @alu_nome,
	   '6CF424DC-8EC3-E011-9B36-00155D033206',
	   @alu_matricula,
	   GETDATE(),
	   GETDATE(),
	   1,
	   NULL,
	   NULL,
	   @pes_id

INSERT INTO MTR_MatriculaTurma
SELECT @alu_id,
	   1,
	   e.esc_id,
	   t.tur_id,
	   @cur_id,
	   1,
	   @crp_id,
	   1,
	   GETDATE(),
	   GETDATE(),
	   @mtu_numeroChamada,
	   @mtu_dataMtricula,
	   NULL
FROM ESC_Escola e,
	 TUR_Turma t	 
WHERE e.esc_codigo = @esc_codigo AND
      t.tur_codigo = @tur_codigo AND
	  t.esc_id = e.esc_id
	  

SELECT *
FROM ESC_Escola e (nolock),
     TUR_Turma t (nolock)
WHERE e.esc_codigo = '19303' AND
      t.tur_codigo = '2103327' AND
      t.esc_id = e.esc_id

select *
	from ESC_Escola (nolock) where esc_codigo = '019303'


INSERT INTO TUR_TurmaCurriculo
SELECT tur_id,
	   @cur_id,
	   1,
	   @crp_id,
	   1,
	   GETDATE(),
	   GETDATE()
FROM TUR_Turma t,
	 ESC_Escola e	  
WHERE t.tur_codigo = @tur_codigo AND
      e.esc_codigo = @esc_codigo AND
	  t.esc_id = e.esc_id AND
	  NOT EXISTS (SELECT tur_id
				  FROM TUR_TurmaCurriculo
				  WHERE tur_id = @tur_id AND
						cur_id = @cur_id AND
						crr_id = 1 AND
						crp_id = @crp_id)



select * from MTR_MatriculaTurma (nolock) where alu_id = 2647187

select t.*
FROM ESC_Escola e (nolock),
     TUR_Turma t (nolock)
WHERE e.esc_codigo = '200050' AND
      t.tur_codigo = 'EJA-2A' AND
      t.esc_id = e.esc_id

select * from ACA_CalendarioAnual where cal_id = 21
		 
select e.esc_nome,
	   t.tur_codigo,
	   a.alu_matricula,
	   tpcp.tcp_descricao,
	   a.alu_nome,
	   tme.tme_nome
	from ACA_Aluno a
		inner join MTR_MatriculaTurma m
			on a.alu_id = m.alu_id
		inner join ESC_Escola e
			on m.esc_id = e.esc_id		
		inner join TUR_Turma t
			on m.tur_id = t.tur_id
		inner join TUR_TurmaTipoCurriculoPeriodo ttcp
			on t.tur_id = ttcp.tur_id
		inner join ACA_TipoCurriculoPeriodo tpcp
			on ttcp.tme_id = tpcp.tme_id and
			   ttcp.tne_id = tpcp.tne_id
		inner join ACA_TipoModalidadeEnsino tme
			on tpcp.tme_id = tme.tme_id
where e.esc_nome not like '%CIEJA%'


select distinct e.esc_id,
				e.esc_nome,
				t.tur_codigo,
				tpcp.tcp_descricao,
				tpcp.tcp_dataCriacao,
				tcg.Test_Id       
	from ESC_Escola e (nolock)
		inner join TUR_TurmaTipoCurriculoPeriodo ttcp (nolock)
			on e.esc_id = ttcp.esc_id
		inner join TUR_Turma t (nolock)
			on ttcp.tur_id = t.tur_id
		inner join ACA_TipoCurriculoPeriodo tpcp (nolock)
			on ttcp.tme_id = tpcp.tme_id and
			   ttcp.tne_id = tpcp.tne_id and
			   ttcp.crp_ordem = tpcp.tcp_ordem
		left join GestaoAvaliacao.dbo.TestCurriculumGrade tcg
			on tpcp.tcp_id = tcg.TypeCurriculumGradeId
where t.tur_codigo like 'EJA-%'	  
order by e.esc_nome,
		 t.tur_codigo,
		 tpcp.tcp_descricao,
		 tcg.Test_Id



begin tran
update cp
set tcp_id = tpcp.tcp_id
from ACA_CurriculoPeriodo cp,
	ACA_TipoCurriculoPeriodo tpcp
where cp.crp_descricao = tpcp.tcp_descricao and
	  cp.crp_ordem = tpcp.tcp_ordem and
	  cp.tcp_id is null

-- commit


select top 1 a.alu_nome,
		     a.alu_matricula,
			 me.tme_nome,
			 e.esc_nome,
			 t.tur_codigo			 			 
	from ACA_TipoModalidadeEnsino me
		inner join TUR_TurmaTipoCurriculoPeriodo ttcp
			on me.tme_id = ttcp.tme_id
		inner join MTR_MatriculaTurma m
			on ttcp.tur_id = m.tur_id
		inner join ACA_Aluno a
			on m.alu_id = a.alu_id
		inner join ESC_Escola e
			on m.esc_id = e.esc_id
		inner join ACA_TipoCurriculoPeriodo tpcp
			on ttcp.tme_id = tpcp.tme_id and
			   ttcp.tne_id = tpcp.tne_id and
			   ttcp.crp_ordem = tpcp.tcp_ordem
		inner join TUR_Turma t
			on ttcp.tur_id = t.tur_id		
where me.tme_nome in ('CIEJA') and
	  tpcp.tcp_descricao in ('EJA FINAL II', '4ª EJA MODULAR', 'M IV') and
	  CONVERT(date, m.mtu_dataMatricula) < CONVERT(date, '2019-11-04') 
	  
	  

union all

select top 1 a.alu_nome,
		     a.alu_matricula,
			 me.tme_nome,
			 e.esc_nome,
			 t.tur_codigo
	from ACA_TipoModalidadeEnsino me
		inner join TUR_TurmaTipoCurriculoPeriodo ttcp
			on me.tme_id = ttcp.tme_id
		inner join MTR_MatriculaTurma m
			on ttcp.tur_id = m.tur_id
		inner join ACA_Aluno a
			on m.alu_id = a.alu_id
		inner join ESC_Escola e
			on m.esc_id = e.esc_id
		inner join ACA_TipoCurriculoPeriodo tpcp
			on ttcp.tme_id = tpcp.tme_id and
			   ttcp.tne_id = tpcp.tne_id and
			   ttcp.crp_ordem = tpcp.tcp_ordem
		inner join TUR_Turma t
			on ttcp.tur_id = t.tur_id
where me.tme_nome in ('EJA Regular') and 
	  tpcp.tcp_descricao in ('EJA FINAL II', 'Final 2', '4ª EJA MODULAR', 'M IV') and
	  CONVERT(date, m.mtu_dataMatricula) < CONVERT(date, '2019-11-04')

union all

select top 1 a.alu_nome,
		     a.alu_matricula,
			 me.tme_nome,
			 e.esc_nome,
			 t.tur_codigo
	from ACA_TipoModalidadeEnsino me
		inner join TUR_TurmaTipoCurriculoPeriodo ttcp
			on me.tme_id = ttcp.tme_id
		inner join MTR_MatriculaTurma m
			on ttcp.tur_id = m.tur_id
		inner join ACA_Aluno a
			on m.alu_id = a.alu_id
		inner join ESC_Escola e
			on m.esc_id = e.esc_id
		inner join ACA_TipoCurriculoPeriodo tpcp
			on ttcp.tme_id = tpcp.tme_id and
			   ttcp.tne_id = tpcp.tne_id and
			   ttcp.crp_ordem = tpcp.tcp_ordem
		inner join TUR_Turma t
			on ttcp.tur_id = t.tur_id
where me.tme_nome in ('EJA Modular') and 
	  tpcp.tcp_descricao in ('EJA FINAL II', 'Final 2', '4ª EJA MODULAR', 'M IV') and
	  t.cal_id = 21 and
	  CONVERT(date, m.mtu_dataMatricula) < CONVERT(date, '2019-11-04')