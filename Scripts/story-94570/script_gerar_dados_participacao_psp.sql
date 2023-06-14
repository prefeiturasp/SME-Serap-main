insert into [dbo].[ParticipacaoSME]
select Edicao,
10 AnoEscolar,
TotalPrevisto,
TotalPresente,
PercentualParticipacao 
from [dbo].[ParticipacaoSME] 
where Edicao = 2022 and AnoEscolar = 9

insert into [dbo].[ParticipacaoSMEAreaConhecimento]
select
Edicao
,5 AreaConhecimentoID
,10 AnoEscolar
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoSMEAreaConhecimento] 
where Edicao = 2022 
and AreaConhecimentoID = 2 
and AnoEscolar = 9

insert into [dbo].[ParticipacaoDRE]
select
Edicao
,uad_sigla
,10 AnoEscolar
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoDRE] 
where Edicao = 2022 and AnoEscolar = 8 and uad_sigla = 'BT'

insert into [dbo].[ParticipacaoDREAreaConhecimento]
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,10 AnoEscolar
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoDREAreaConhecimento]
where Edicao = 2022 
and AreaConhecimentoID = 2 
and AnoEscolar = 9
and uad_sigla = 'BT'

insert into [dbo].[ParticipacaoEscola]
select
Edicao
,uad_sigla
,esc_codigo
,10 AnoEscolar
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoEscola] 
where Edicao = 2022 and AnoEscolar = 8 and uad_sigla = 'BT'

insert into [dbo].[ParticipacaoEscolaAreaConhecimento]
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,esc_codigo
,10 AnoEscolar
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoEscolaAreaConhecimento]
where Edicao = 2022 
and AreaConhecimentoID = 1 
and AnoEscolar = 3
and uad_sigla = 'BT'

insert into [dbo].[ParticipacaoTurma]
select
Edicao
,uad_sigla
,esc_codigo
,10 AnoEscolar
,'1A' tur_codigo
,tur_id
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoTurma] 
where Edicao = 2022 and AnoEscolar = 8 and uad_sigla = 'BT'
and esc_codigo = '000191'
and tur_codigo = '8A'

insert into [dbo].[ParticipacaoTurmaAreaConhecimento]
select
Edicao
,5 AreaConhecimentoID
,uad_sigla
,esc_codigo
,10 AnoEscolar
,'1A' tur_codigo
,tur_id
,TotalPrevisto
,TotalPresente
,PercentualParticipacao
from [dbo].[ParticipacaoTurmaAreaConhecimento]
where Edicao = 2022 and AnoEscolar = 8 and uad_sigla = 'BT'
and esc_codigo = '000191'
and tur_codigo = '8A'
and AreaConhecimentoID = 3