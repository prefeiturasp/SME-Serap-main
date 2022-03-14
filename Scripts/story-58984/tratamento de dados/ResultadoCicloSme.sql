
CREATE TABLE [dbo].[ResultadoCicloSme](
	[Edicao] [varchar](10) NOT NULL,
	[AreaConhecimentoID] [tinyint] NOT NULL,
	[CicloID] [int] NOT NULL,
	[Valor] [float] NULL,
	[TotalAlunos] [int] NULL,
	[NivelProficienciaID] [tinyint] NULL,
	[PercentualAbaixoDoBasico] [decimal](6, 2) NULL,
	[PercentualBasico] [decimal](6, 2) NULL,
	[PercentualAdequado] [decimal](6, 2) NULL,
	[PercentualAvancado] [decimal](6, 2) NULL,
	[PercentualAlfabetizado] [decimal](6, 2) NULL
)


insert into ResultadoCicloSme values
('2021',1,2,176.332920347322,106299,0,0.51,0.31,0.14,0.04,0),
('2021',2,2,172.268708542897,109770,0,0.33,0.4,0.23,0.04,0 ),
('2021',3,2,179.038117889053,108899,0,0.49,0.34,0.14,0.02,0),
('2021',1,3,207.007821819476,95566,0,0.57,0.32,0.1,0.02,0  ),
('2021',2,3,197.551078463575,100467,0,0.45,0.37,0.16,0.03,0),
('2021',3,3,210.946842234349,97854,0,0.54,0.35,0.1,0.01,0  );

select * from ResultadoCicloSme
update ResultadoCicloSme set Valor = CONVERT(float, ROUND(Valor, 1))
update ResultadoCicloSme set PercentualAbaixoDoBasico = ROUND(CONVERT(float, REPLACE(PercentualAbaixoDoBasico,',','.')), 1)
update ResultadoCicloSme set PercentualBasico = ROUND(CONVERT(float, REPLACE(PercentualBasico,',','.')), 1)
update ResultadoCicloSme set PercentualAdequado = ROUND(CONVERT(float, REPLACE(PercentualAdequado,',','.')), 1)
update ResultadoCicloSme set PercentualAvancado = ROUND(CONVERT(float, REPLACE(PercentualAvancado,',','.')), 1)
update ResultadoCicloSme set PercentualAlfabetizado = ROUND(CONVERT(float, REPLACE(PercentualAlfabetizado,',','.')), 1)



