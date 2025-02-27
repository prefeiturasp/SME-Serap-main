use ProvaSP


insert into ResultadoAluno
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,esc_codigo
,10 AnoEscolar
,'1A' tur_codigo
,tur_id
,alu_matricula
,alu_nome
,ResultadoLegadoID
,NivelProficienciaID
,Valor
,REDQ1
,REDQ2
,REDQ3
,REDQ4
,REDQ5
from [dbo].[ResultadoAluno] 
where Edicao = 2022
and uad_sigla = 'BT'
and REPLICATE('0', 6 - LEN(esc_codigo)) + RTrim(esc_codigo) = '000191'
and AnoEscolar = 4
and tur_codigo = '4A'
and AreaConhecimentoId = 1

update ResultadoTurma
set tur_codigo = '1A'
where Edicao = 2022
and uad_sigla = 'BT'
and REPLICATE('0', 6 - LEN(esc_codigo)) + RTrim(esc_codigo) = '000191'
and AnoEscolar = 10
and AreaConhecimentoId = 5
and tur_codigo = '4A'

update ResultadoTurma
set tur_codigo = '1B'
where Edicao = 2022
and uad_sigla = 'BT'
and REPLICATE('0', 6 - LEN(esc_codigo)) + RTrim(esc_codigo) = '000191'
and AnoEscolar = 10
and AreaConhecimentoId = 5
and tur_codigo = '4B'

insert into ResultadoTurma
select 
Edicao
,5 AreaConhecimentoID
,uad_sigla
,esc_codigo
,10 AnoEscolar
,tur_codigo
,tur_id
,Valor
,NivelProficienciaID
,TotalAlunos
,PercentualAbaixoDoBasico
,PercentualBasico
,PercentualAdequado
,PercentualAvancado
,PercentualAlfabetizado 
from ResultadoTurma
where Edicao = 2022
and uad_sigla = 'BT'
and REPLICATE('0', 6 - LEN(esc_codigo)) + RTrim(esc_codigo) = '000191'
and AnoEscolar = 4
and AreaConhecimentoId = 1

insert into ResultadoEscola
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,esc_codigo
,10 AnoEscolar
,Valor
,NivelProficienciaID
,TotalAlunos
,PercentualAbaixoDoBasico
,PercentualBasico
,PercentualAdequado
,PercentualAvancado
,PercentualAlfabetizado
from ResultadoEscola
where Edicao = 2022
and uad_sigla = 'BT'
and REPLICATE('0', 6 - LEN(esc_codigo)) + RTrim(esc_codigo) = '000191'
and AnoEscolar = 4
and AreaConhecimentoId = 1

insert into ResultadoDre
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,10 AnoEscolar
,Valor
,NivelProficienciaID
,TotalAlunos
,PercentualAbaixoDoBasico
,PercentualBasico
,PercentualAdequado
,PercentualAvancado
,PercentualAlfabetizado
from ResultadoDre
where Edicao = 2022
and uad_sigla = 'BT'
and AnoEscolar = 4
and AreaConhecimentoId = 1

insert into ResultadoSme
select
Edicao
,5 AreaConhecimentoID
,10 AnoEscolar
,Valor
,TotalAlunos
,NivelProficienciaID
,PercentualAbaixoDoBasico
,PercentualBasico
,PercentualAdequado
,PercentualAvancado
,PercentualAlfabetizado
from ResultadoSme
where Edicao = 2022
and AnoEscolar = 4
and AreaConhecimentoId = 1