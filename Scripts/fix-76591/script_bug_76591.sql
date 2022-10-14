use ProvaSP_Resultado2021

-- ============== DRE
insert into [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento]
select
Edicao,
1 AreaConhecimentoID,
uad_sigla,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoDRECn] a
left join [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 1
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento]
select
Edicao,
2 AreaConhecimentoID,
uad_sigla,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoDRELp] a
left join [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 2
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento]
select
Edicao,
3 AreaConhecimentoID,
uad_sigla,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoDREMt] a
left join [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 3
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento]
select
Edicao,
4 AreaConhecimentoID,
uad_sigla,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoDRERedacao] a
left join [ProvaSP].[dbo].[ParticipacaoDREAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 4
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

-- ============== ESCOLA
insert into [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento]
select
Edicao,
1 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoEscolaCn] a
left join [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 1
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento]
select
Edicao,
2 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoEscolaLp] a
left join [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 2
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento]
select
Edicao,
3 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoEscolaMt] a
left join [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 3
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento]
select
Edicao,
4 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoEscolaRedacao] a
left join [ProvaSP].[dbo].[ParticipacaoEscolaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = 4
and a.uad_sigla = b.uad_sigla
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

-- ============== SME
insert into [ProvaSP].[dbo].[ParticipacaoSMEAreaConhecimento]
select
Edicao,
AreaConhecimentoID,
AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoSMEAreaConhecimento] a
left join [ProvaSP].[dbo].[ParticipacaoSMEAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and b.AreaConhecimentoID = a.AreaConhecimentoID
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

-- ============== TURMA
insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select
Edicao,
1 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,
tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoTurmaCn] a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo and a.tur_id = b.tur_id
and b.AreaConhecimentoID = 1
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null

insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select distinct
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao from (
select a.*,b.AnoEscolar anoEsc from (
select
a.Edicao,
2 AreaConhecimentoID,
a.uad_sigla,
a.esc_codigo,
a.AnoEscolar,
a.tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from ParticipacaoTurmaLp a
inner join 
(select COUNT(*) qtde,
AnoEscolar,uad_sigla,tur_codigo,esc_codigo
from ParticipacaoTurmaLp 
where 
AnoEscolar in (2,3)
and Edicao = 2021 
group by AnoEscolar,uad_sigla,tur_codigo,esc_codigo
having COUNT(*) = 1) b on a.AnoEscolar = b.AnoEscolar 
and a.uad_sigla = b.uad_sigla and a.tur_codigo = b.tur_codigo 
and a.esc_codigo = b.esc_codigo
and a.TotalPrevisto > 0 and a.TotalPresente > 0 and a.PercentualParticipacao > 0
) a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo 
and a.tur_id = b.tur_id 
and a.tur_codigo = b.tur_codigo
and b.AreaConhecimentoID = a.AreaConhecimentoID
where b.AnoEscolar is null) a

insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select distinct
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao from (
select a.*,b.AnoEscolar anoEsc from (
select
a.Edicao,
2 AreaConhecimentoID,
a.uad_sigla,
a.esc_codigo,
a.AnoEscolar,
a.tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from ParticipacaoTurmaLp a
inner join 
(select COUNT(*) qtde,
AnoEscolar,uad_sigla,tur_codigo,esc_codigo
from ParticipacaoTurmaLp 
where 
AnoEscolar in (2,3)
and Edicao = 2021 
group by AnoEscolar,uad_sigla,tur_codigo,esc_codigo
having COUNT(*) > 1) b on a.AnoEscolar = b.AnoEscolar 
and a.uad_sigla = b.uad_sigla and a.tur_codigo = b.tur_codigo 
and a.esc_codigo = b.esc_codigo
and a.TotalPrevisto > 0 and a.TotalPresente > 0 and a.PercentualParticipacao > 0
) a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo 
and a.tur_id = b.tur_id 
and a.tur_codigo = b.tur_codigo
and b.AreaConhecimentoID = a.AreaConhecimentoID
where b.AnoEscolar is null) a

insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select distinct
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao from (
select a.*,b.AnoEscolar anoEsc from (
select
a.Edicao,
3 AreaConhecimentoID,
a.uad_sigla,
a.esc_codigo,
a.AnoEscolar,
a.tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from ParticipacaoTurmaMt a
inner join 
(select COUNT(*) qtde,
AnoEscolar,uad_sigla,tur_codigo,esc_codigo
from ParticipacaoTurmaMt 
where 
AnoEscolar in (2,3)
and Edicao = 2021 
group by AnoEscolar,uad_sigla,tur_codigo,esc_codigo
having COUNT(*) = 1) b on a.AnoEscolar = b.AnoEscolar 
and a.uad_sigla = b.uad_sigla and a.tur_codigo = b.tur_codigo 
and a.esc_codigo = b.esc_codigo
and a.TotalPrevisto > 0 and a.TotalPresente > 0 and a.PercentualParticipacao > 0
) a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo 
and a.tur_id = b.tur_id 
and a.tur_codigo = b.tur_codigo
and b.AreaConhecimentoID = a.AreaConhecimentoID
where b.AnoEscolar is null) a

insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select distinct
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao from (
select a.*,b.AnoEscolar anoEsc from (
select
a.Edicao,
3 AreaConhecimentoID,
a.uad_sigla,
a.esc_codigo,
a.AnoEscolar,
a.tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from ParticipacaoTurmaMt a
inner join 
(select COUNT(*) qtde,
AnoEscolar,uad_sigla,tur_codigo,esc_codigo
from ParticipacaoTurmaMt 
where 
AnoEscolar in (2,3)
and Edicao = 2021 
group by AnoEscolar,uad_sigla,tur_codigo,esc_codigo
having COUNT(*) > 1) b on a.AnoEscolar = b.AnoEscolar 
and a.uad_sigla = b.uad_sigla and a.tur_codigo = b.tur_codigo 
and a.esc_codigo = b.esc_codigo
and a.TotalPrevisto > 0 and a.TotalPresente > 0 and a.PercentualParticipacao > 0
) a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo 
and a.tur_id = b.tur_id 
and a.tur_codigo = b.tur_codigo
and b.AreaConhecimentoID = a.AreaConhecimentoID
where b.AnoEscolar is null) a

insert into [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento]
select distinct
Edicao,
4 AreaConhecimentoID,
uad_sigla,
esc_codigo,
AnoEscolar,
tur_codigo,tur_id,
TotalPrevisto,
TotalPresente,
PercentualParticipacao
from 
(select a.*,
b.AnoEscolar as anoEsc 
from [dbo].[ParticipacaoTurmaRedacao] a
left join [ProvaSP].[dbo].[ParticipacaoTurmaAreaConhecimento] b 
on a.Edicao = b.Edicao and a.AnoEscolar = b.AnoEscolar and a.uad_sigla = b.uad_sigla 
and a.esc_codigo = b.esc_codigo and a.tur_id = b.tur_id and a.tur_codigo = b.tur_codigo
and b.AreaConhecimentoID = 4
where a.AnoEscolar in (2,3) and a.Edicao = 2021) a
where a.anoEsc is null


insert into [ProvaSP].[dbo].[ParticipacaoDRE]
select 
a.Edicao,
a.dre_sigla,
a.turma_ano_escolar,
a.TotalPrevisto,
a.quantidade_participacao_geral,
a.PercentualParticipacao_geral
from [dbo].[ParticipacaoDRE] a
left join [ProvaSP].[dbo].[ParticipacaoDRE] b
on a.turma_ano_escolar = b.AnoEscolar 
and a.dre_sigla = b.uad_sigla and a.Edicao = b.Edicao
where a.turma_ano_escolar in (2,3) and a.Edicao = 2021
and b.AnoEscolar is null


insert into [ProvaSP].[dbo].[ParticipacaoSME]
select a.* 
from [dbo].[ParticipacaoSME] a
left join [ProvaSP].[dbo].[ParticipacaoSME] b on a.AnoEscolar = b.AnoEscolar and a.Edicao = b.Edicao
where a.AnoEscolar in (2,3) and a.Edicao = 2021
and b.AnoEscolar is null




