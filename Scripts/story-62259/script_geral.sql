use ProvaSP_Resultado2021


update ResultadoDRE_2e3Ano set Valor = CONVERT(float, ROUND(Valor, 1))
update ResultadoEscola_2e3Ano set valor = '0' where valor = 'NA'
update ResultadoEscola_2e3Ano set nivelProficienciaId = '0' where nivelProficienciaId = 'NA'
update ResultadoEscola_2e3Ano set PercentualAbaixoDoBasico = '0' where PercentualAbaixoDoBasico = 'NA'
update ResultadoEscola_2e3Ano set PercentualBasico = '0' where PercentualBasico = 'NA'
update ResultadoEscola_2e3Ano set PercentualAdequado = '0' where PercentualAdequado = 'NA'
update ResultadoEscola_2e3Ano set PercentualAvancado = '0' where PercentualAvancado = 'NA'
alter table ResultadoEscola_2e3Ano alter column totalAlunos smallint
update ResultadoEscola_2e3Ano set Valor = ROUND(CONVERT(float, REPLACE(Valor,',','.')), 1)
alter table ResultadoEscola_2e3Ano alter column Valor float
alter table ResultadoEscola_2e3Ano alter column totalAlunos int
alter table ResultadoEscola_2e3Ano alter column nivelProficienciaId float

update ResultadoEscola_2e3Ano set PercentualAbaixoDoBasico = ROUND(CONVERT(float, REPLACE(PercentualAbaixoDoBasico,',','.')), 1)
alter table ResultadoEscola_2e3Ano alter column PercentualAbaixoDoBasico float

update ResultadoEscola_2e3Ano set PercentualBasico = ROUND(CONVERT(float, REPLACE(PercentualBasico,',','.')), 1)
alter table ResultadoEscola_2e3Ano alter column PercentualBasico float

update ResultadoEscola_2e3Ano set PercentualAdequado = ROUND(CONVERT(float, REPLACE(PercentualAdequado,',','.')), 1)
alter table ResultadoEscola_2e3Ano alter column PercentualAdequado float

update ResultadoEscola_2e3Ano set PercentualAvancado = ROUND(CONVERT(float, REPLACE(PercentualAvancado,',','.')), 1)
alter table ResultadoEscola_2e3Ano alter column PercentualAvancado float

 update ResultadoEscola_2e3Ano set esc_codigo = '0' where esc_codigo like '%NA%'

SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoEscola_2e3Ano a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoEscola_2e3Ano set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)
drop table #ESC

update ResultadoSME_2e3Ano set Valor = ROUND(CONVERT(float, REPLACE(Valor,',','.')), 1)

update ResultadoSME_2e3Ano set AreaConhecimentoID = 
case
when AreaConhecimentoID = 'cn' then '1'
when AreaConhecimentoID = 'lp' then '2'
when AreaConhecimentoID = 'mt' then '3'
else '0' end


-- ====================================================================================================

use ProvaSP;

insert into ResultadoDRE (Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,
Valor,NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,
PercentualAvancado)
select
Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,Valor,NivelProficienciaID,TotalAlunos,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [ProvaSP_Resultado2021].[dbo].[ResultadoDRE_2e3Ano]

insert into ResultadoEscola (Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
Valor,NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,
PercentualAdequado,PercentualAvancado)
select
Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,Valor,NivelProficienciaID,
TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [ProvaSP_Resultado2021].[dbo].[ResultadoEscola_2e3Ano]
where esc_codigo != '0'

insert into ResultadoSME (Edicao,AreaConhecimentoID,AnoEscolar,Valor,TotalAlunos,NivelProficienciaID,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado)
select
Edicao,AreaConhecimentoID,AnoEscolar,Valor,TotalAlunos,NivelProficienciaID,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [ProvaSP_Resultado2021].[dbo].[ResultadoSME_2e3Ano]





