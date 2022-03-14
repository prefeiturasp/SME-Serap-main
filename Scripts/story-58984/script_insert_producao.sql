use ProvaSP


insert into ResultadoCicloDre
select 
Edicao,
AreaConhecimentoID,
uad_sigla,
CicloId,
ROUND(Valor, 1) Valor,
NivelProficienciaID,
TotalAlunos,
ROUND(PercentualAbaixoDoBasico, 1) PercentualAbaixoDoBasico,
ROUND(PercentualBasico, 1) PercentualBasico,
ROUND(PercentualAdequado, 1) PercentualAdequado,
ROUND(PercentualAvancado, 1) PercentualAvancado,
ROUND(PercentualAlfabetizado, 1) PercentualAlfabetizado 
from [ProvaSP_Resultado2021].[dbo].[ResultadoCicloDre] 
where Edicao = 2021;
--------------------------------------------------------------------------------------------------------------

insert into ResultadoCicloEscola (Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
CicloID,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado)
select
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
CicloID,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado
from [ProvaSP_Resultado2021].[dbo].[ResultadoCicloEscola] 
where Edicao = 2021;

--------------------------------------------------------------------------------------------------------------

insert into ResultadoCicloSme
(Edicao,
AreaConhecimentoID,
CicloID,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado)
select 
Edicao,
AreaConhecimentoID,
CicloID,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado
from [ProvaSP_Resultado2021].[dbo].[ResultadoCicloSme] 
where Edicao = 2021;

--------------------------------------------------------------------------------------------------------------

insert into ResultadoCicloTurma
(Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
CicloId,
tur_codigo,
tur_id,
Valor,
NivelProficienciaID,
TotalAlunos,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado)
select
Edicao,
AreaConhecimentoID,
uad_sigla,
esc_codigo,
CicloId,
tur_codigo,
tur_id,
Valor,
NivelProficienciaID,
TotalAlunos,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado,
PercentualAlfabetizado
from [ProvaSP_Resultado2021].[dbo].[ResultadoCicloTurma] 
where Edicao = 2021;

