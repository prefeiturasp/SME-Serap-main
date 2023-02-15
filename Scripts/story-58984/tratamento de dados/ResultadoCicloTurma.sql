

update ResultadoCicloTurma set NivelProficienciaID = 0 where NivelProficienciaID = 'NA'
update ResultadoCicloTurma set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico = 'NA'
update ResultadoCicloTurma set PercentualBasico = 0 where PercentualBasico = 'NA'
update ResultadoCicloTurma set PercentualAdequado = 0 where PercentualAdequado = 'NA'
update ResultadoCicloTurma set PercentualAvancado = 0 where PercentualAvancado = 'NA'
update ResultadoCicloTurma set PercentualAlfabetizado = 0 where PercentualAlfabetizado = 'NA'

update ResultadoCicloTurma set Valor = CONVERT(float, ROUND(Valor, 1))
update ResultadoCicloTurma set PercentualAbaixoDoBasico = ROUND(CONVERT(float, REPLACE(PercentualAbaixoDoBasico,',','.')), 1)
update ResultadoCicloTurma set PercentualBasico = ROUND(CONVERT(float, REPLACE(PercentualBasico,',','.')), 1)
update ResultadoCicloTurma set PercentualAdequado = ROUND(CONVERT(float, REPLACE(PercentualAdequado,',','.')), 1)
update ResultadoCicloTurma set PercentualAvancado = ROUND(CONVERT(float, REPLACE(PercentualAvancado,',','.')), 1)
update ResultadoCicloTurma set PercentualAlfabetizado = ROUND(CONVERT(float, REPLACE(PercentualAlfabetizado,',','.')), 1)

alter table ResultadoCicloTurma alter column PercentualAbaixoDoBasico float
alter table ResultadoCicloTurma alter column PercentualBasico float
alter table ResultadoCicloTurma alter column PercentualAdequado float
alter table ResultadoCicloTurma alter column PercentualAvancado float
alter table ResultadoCicloTurma alter column PercentualAlfabetizado float

SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoCicloTurma a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoCicloTurma set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC