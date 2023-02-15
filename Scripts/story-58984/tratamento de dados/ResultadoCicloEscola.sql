

CREATE TABLE [dbo].[ResultadoCicloEscola](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[uad_sigla] [varchar](4) NOT NULL,
	[esc_codigo] [varchar](20) NOT NULL,
	[CicloId] [int] NOT NULL,
	[Valor] [decimal](6, 3) NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[TotalAlunos] [int] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[PercentualAlfabetizado] [decimal](6, 2) NULL
)


update ResultadoCicloEscola set NivelProficienciaID = 0 where NivelProficienciaID = 'NA'
update ResultadoCicloEscola set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico = 'NA'
update ResultadoCicloEscola set PercentualBasico = 0 where PercentualBasico = 'NA'
update ResultadoCicloEscola set PercentualAdequado = 0 where PercentualAdequado = 'NA'
update ResultadoCicloEscola set PercentualAvancado = 0 where PercentualAvancado = 'NA'
update ResultadoCicloEscola set PercentualAlfabetizado = 0 where PercentualAlfabetizado = 'NA'

alter table ResultadoCicloEscola alter column Valor float
update ResultadoCicloEscola set Valor = CONVERT(float, ROUND(Valor, 1))

update ResultadoCicloEscola set PercentualAbaixoDoBasico = ROUND(CONVERT(float, REPLACE(PercentualAbaixoDoBasico,',','.')), 1)
alter table ResultadoCicloEscola alter column PercentualAbaixoDoBasico float

update ResultadoCicloEscola set PercentualBasico = ROUND(CONVERT(float, REPLACE(PercentualBasico,',','.')), 1)
alter table ResultadoCicloEscola alter column PercentualBasico float

update ResultadoCicloEscola set PercentualAdequado = ROUND(CONVERT(float, REPLACE(PercentualAdequado,',','.')), 1)
alter table ResultadoCicloEscola alter column PercentualAdequado float

update ResultadoCicloEscola set PercentualAvancado = ROUND(CONVERT(float, REPLACE(PercentualAvancado,',','.')), 1)
alter table ResultadoCicloEscola alter column PercentualAvancado float

update ResultadoCicloEscola set PercentualAlfabetizado = ROUND(CONVERT(float, REPLACE(PercentualAlfabetizado,',','.')), 1)
alter table ResultadoCicloEscola alter column PercentualAlfabetizado float

 alter table ResultadoCicloEscola alter column esc_codigo varchar(10)

SELECT distinct a.esc_codigo,
e.esc_codigo cod_escola
into #ESC
FROM ResultadoCicloEscola a WITH (NOLOCK)
left join ProvaSP.dbo.Escola e WITH (NOLOCK) on a.esc_codigo = e.esc_codigo
WHERE a.Edicao = 2021
and e.esc_codigo is null

select esc_codigo from #ESC

update ResultadoCicloEscola set esc_codigo = '0' + esc_codigo 
where Edicao = 2021
and esc_codigo in (select esc_codigo from #ESC)

drop table #ESC



