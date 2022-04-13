
insert into ResultadoDRE (Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,
Valor,NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,
PercentualAvancado)
select
Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,Valor,NivelProficienciaID,TotalAlunos,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoDRE_2e3Ano]

insert into ResultadoEscola (Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
Valor,NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,
PercentualAdequado,PercentualAvancado)
select
Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,Valor,NivelProficienciaID,
TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoEscola_2e3Ano]
where esc_codigo != '0'

insert into ResultadoSME (Edicao,AreaConhecimentoID,AnoEscolar,Valor,TotalAlunos,NivelProficienciaID,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado)
select
Edicao,AreaConhecimentoID,AnoEscolar,Valor,TotalAlunos,NivelProficienciaID,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoSME_2e3Ano]






