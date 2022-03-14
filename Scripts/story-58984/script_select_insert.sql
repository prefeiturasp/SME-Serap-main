-- homolog
use ProvaSP


select * from Ciclo
--insert into Ciclo values
--(1,	'Básico'),
--(2,	'Interdisciplinar'),
--(3,	'Autoral')

--insert into CicloAnoEscolar values
--(1,	1),
--(1,	2),
--(1,	3),
--(2,	4),
--(2,	5),
--(2,	6),
--(3,	7),
--(3,	8),
--(3,	9)

select * from ResultadoCicloDre

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
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoCicloDre] 
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
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoCicloEscola] 
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
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoCicloSme] 
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
from [10.49.16.23\SME_PRD].[ProvaSP_Resultado2021].[dbo].[ResultadoCicloTurma] 
where Edicao = 2021;

