use ProvaSP_Resultado2021

 alter table ParticipacaoTurmaCn alter column TotalPresente int

CREATE TABLE [dbo].[ParticipacaoTurmaGeralFix](
	[Edicao] [varchar](10) NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[tur_codigo] [varchar](20) NOT NULL,
	[tur_id] [bigint] NOT NULL,
	[TotalPrevisto] [int] NOT NULL,
	[TotalPresente] [int] NULL,
	[PercentualParticipacao] [decimal](6, 2) NOT NULL
)

insert into ParticipacaoTurmaGeralFix
(Edicao,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,esc_codigo,uad_sigla,AnoEscolar,tur_codigo,tur_id,TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoTurmaGeral where AnoEscolar not in ('2','3')

UPDATE ParticipacaoTurmaCn SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoTurmaGeralFix SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoTurmaLp SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
UPDATE ParticipacaoTurmaMt SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

insert into ProvaSP.dbo.ParticipacaoTurma
select 
Edicao,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,TotalPrevisto,TotalPresente,PercentualParticipacao 
from ParticipacaoTurmaGeralFix

insert into ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,1 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoTurmaCn where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,2 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoTurmaLp where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,3 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoTurmaMt where AnoEscolar not in ('2','3')

----------------------------------------------------------------------------

 UPDATE ParticipacaoDRECn SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)
 UPDATE ParticipacaoDRE SET PercentualParticipacao_geral = ROUND(PercentualParticipacao_geral, 1)
 UPDATE ParticipacaoDRELp SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

insert into ProvaSP.dbo.ParticipacaoDREAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,TotalPrevisto,
TotalPresente,PercentualParticipacao)
select Edicao, 1 AreaConhecimentoID,uad_sigla,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoDRECn where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoDRE
(Edicao,
uad_sigla,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao)
select 
Edicao,
dre_sigla,
turma_ano_escolar,
TotalPrevisto,
quantidade_participacao_geral,
PercentualParticipacao_geral 
from ParticipacaoDRE where turma_ano_escolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoDREAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,TotalPrevisto,
TotalPresente,PercentualParticipacao)
select Edicao, 2 AreaConhecimentoID,uad_sigla,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoDRELp where AnoEscolar not in ('2','3')

----------------------------------------------------------------------------

update ParticipacaoEscola set PercentualParticipacao = 0 where PercentualParticipacao = 'NA'
update ParticipacaoEscola set PercentualParticipacao = REPLACE(PercentualParticipacao,',','.')
alter table ParticipacaoEscola alter column PercentualParticipacao decimal(6,2)
UPDATE ParticipacaoEscola SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 update ParticipacaoEscolaCn set PercentualParticipacao = 0 where PercentualParticipacao = 'NA'
 update ParticipacaoEscolaCn set PercentualParticipacao = 0 
 update ParticipacaoEscolaCn set PercentualParticipacao = REPLACE(PercentualParticipacao,',','.')
 alter table ParticipacaoEscolaCn alter column PercentualParticipacao decimal(6,2)
 update ParticipacaoEscolaCn set TotalPrevisto = 0 where TotalPrevisto = 'NA'
 alter table ParticipacaoEscolaCn alter column TotalPrevisto int

 update ParticipacaoEscola set TotalPrevisto = 0 where TotalPrevisto = 'NA'
 update ParticipacaoEscola set PercentualParticipacao = 0 where PercentualParticipacao = 'NA'
 update ParticipacaoEscola set PercentualParticipacao = REPLACE(PercentualParticipacao,',','.')
 alter table ParticipacaoEscola alter column PercentualParticipacao decimal(6,2)
 alter table ParticipacaoEscola alter column TotalPrevisto int
 alter table ParticipacaoEscola alter column TotalPresente int

 update ParticipacaoEscolaLp set TotalPrevisto = 0 where TotalPrevisto = 'NA'
 alter table ParticipacaoEscolaLp alter column TotalPrevisto int
 alter table ParticipacaoEscolaLp alter column TotalPresente int

 update ParticipacaoEscolaLp set PercentualParticipacao = 0 where PercentualParticipacao = 'NA'
 update ParticipacaoEscolaLp set PercentualParticipacao = REPLACE(PercentualParticipacao,',','.')
 alter table ParticipacaoEscolaLp alter column PercentualParticipacao decimal(6,2)

 update ParticipacaoEscolaMt set TotalPrevisto = 0 where TotalPrevisto = 'NA'
 alter table ParticipacaoEscolaMt alter column TotalPrevisto int
 alter table ParticipacaoEscolaMt alter column TotalPresente int
 update ParticipacaoEscolaMt set PercentualParticipacao = 0 where PercentualParticipacao = 'NA'
 update ParticipacaoEscolaMt set PercentualParticipacao = REPLACE(PercentualParticipacao,',','.')
 alter table ParticipacaoEscolaMt alter column PercentualParticipacao decimal(6,2)

insert into ProvaSP.dbo.ParticipacaoEscola
(Edicao,uad_sigla,esc_codigo,AnoEscolar,TotalPrevisto,
TotalPresente,PercentualParticipacao)
select 
Edicao,esc_codigo,uad_sigla,AnoEscolar,TotalPrevisto,
TotalPresente,PercentualParticipacao
from ParticipacaoEscola
where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,
AnoEscolar,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,
1 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoEscolaCn where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,
AnoEscolar,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,
2 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoEscolaLp where AnoEscolar not in ('2','3')

insert into ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,
AnoEscolar,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,
3 AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao
from ParticipacaoEscolaMt where AnoEscolar not in ('2','3')

 UPDATE ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1) where Edicao = 2021

----------------------------------------------------------------------------

CREATE TABLE [dbo].[ParticipacaoSME](
	[Edicao] [varchar](10) NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[TotalPrevisto] [int] NULL,
	[TotalPresente] [int] NULL,
	[PercentualParticipacao] [decimal](6, 2) NULL
)

insert into ParticipacaoSME values
('2021','3',41876,33022,78.86),
('2021','4',42676,35510,83.21),
('2021','5',48143,40677,84.49),
('2021','6',49806,38488,77.28),
('2021','7',50923,38772,76.14),
('2021','8',49153,37648,76.59),
('2021','9',47665,32156,67.46),
('2021','2',49953,33488,67.04)


CREATE TABLE [dbo].[ParticipacaoSMEAreaConhecimento](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[AnoEscolar] [varchar](3) NOT NULL,
	[TotalPrevisto] [int] NOT NULL,
	[TotalPresente] [int] NOT NULL,
	[PercentualParticipacao] [decimal](6, 2) NOT NULL
)


insert into ParticipacaoSMEAreaConhecimento values
('2021',1,'3',41876,31465,75.14),
('2021',1,'4',42676,33728,79.03),
('2021',1,'5',48143,38840,80.68),
('2021',1,'6',49806,33730,67.72),
('2021',1,'7',50923,33377,65.54),
('2021',1,'8',49153,34039,69.25),
('2021',1,'9',47665,28150,59.06),
('2021',2,'3',41876,32060,76.56),
('2021',2,'4',42676,34726,81.37),
('2021',2,'5',48143,39496,82.04),
('2021',2,'6',49806,35547,71.37),
('2021',2,'7',50923,35217,69.16),
('2021',2,'8',49153,35294,71.8 ),
('2021',2,'9',47665,29955,62.84),
('2021',2,'2',49953,32831,65.72),
('2021',3,'3',41876,31912,76.21),
('2021',3,'4',42676,34331,80.45),
('2021',3,'5',48143,39454,81.95),
('2021',3,'6',49806,35114,70.5 ),
('2021',3,'7',50923,34891,68.52),
('2021',3,'8',49153,34095,69.37),
('2021',3,'9',47665,28868,60.56),
('2021',3,'2',49953,31992,64.04)

UPDATE ParticipacaoSMEAreaConhecimento SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 INSERT INTO ProvaSP.dbo.ParticipacaoSME SELECT * FROM ParticipacaoSME where AnoEscolar not in ('2','3')
 INSERT INTO ProvaSP.dbo.ParticipacaoSMEAreaConhecimento select * from ParticipacaoSMEAreaConhecimento where AnoEscolar not in ('2','3')

----------------------------------------------------------------------------

-- CORREÇÃO CODIGOS ESCOLAS

SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC


SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ProvaSP.dbo.ParticipacaoTurma a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ProvaSP.dbo.ParticipacaoTurma set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC

-- ==============


SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ProvaSP.dbo.ParticipacaoEscola a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ProvaSP.dbo.ParticipacaoEscola set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC


SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC


-- ==============

