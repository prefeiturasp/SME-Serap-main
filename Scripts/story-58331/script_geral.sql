use ProvaSP_Resultado2021

 UPDATE ParticipacaoTurmaRedacao SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 UPDATE ParticipacaoDRERedacao SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 UPDATE ParticipacaoEscolaRedacao SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 UPDATE ParticipacaoSMERedacao SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)

 UPDATE ParticipacaoTurmaRedacao SET PercentualParticipacao = ROUND(PercentualParticipacao, 1)


UPDATE ResultadoAlunoRedacao SET NivelProficienciaID = 0 WHERE NivelProficienciaID IS NULL;
UPDATE ResultadoAlunoRedacao SET Valor = 0 WHERE Valor IS NULL;
UPDATE ResultadoAlunoRedacao SET REDQ1 = 0 WHERE REDQ1 IS NULL;
UPDATE ResultadoAlunoRedacao SET REDQ2 = 0 WHERE REDQ2 IS NULL;
UPDATE ResultadoAlunoRedacao SET REDQ3 = 0 WHERE REDQ3 IS NULL;
UPDATE ResultadoAlunoRedacao SET REDQ4 = 0 WHERE REDQ4 IS NULL;
UPDATE ResultadoAlunoRedacao SET REDQ5 = 0 WHERE REDQ5 IS NULL;

alter table ResultadoAlunoRedacao alter column NivelProficienciaID float;
alter table ResultadoAlunoRedacao alter column Valor float;
alter table ResultadoAlunoRedacao alter column REDQ1 float;
alter table ResultadoAlunoRedacao alter column REDQ2 float;
alter table ResultadoAlunoRedacao alter column REDQ3 float;
alter table ResultadoAlunoRedacao alter column REDQ4 float;
alter table ResultadoAlunoRedacao alter column REDQ5 float;

UPDATE ResultadoAlunoRedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,'.',',');
UPDATE ResultadoAlunoRedacao SET NivelProficienciaID = ROUND(NivelProficienciaID, 1);
UPDATE ResultadoAlunoRedacao SET Valor = REPLACE(Valor,'.',',');
UPDATE ResultadoAlunoRedacao SET Valor = ROUND(Valor, 1);
UPDATE ResultadoAlunoRedacao SET REDQ1 = ROUND(REDQ1, 1);
UPDATE ResultadoAlunoRedacao SET REDQ1 = REPLACE(REDQ1,'.',',');
UPDATE ResultadoAlunoRedacao SET REDQ2 = ROUND(REDQ2, 1);
UPDATE ResultadoAlunoRedacao SET REDQ2 = REPLACE(REDQ2,'.',',');
UPDATE ResultadoAlunoRedacao SET REDQ3 = ROUND(REDQ3, 1);
UPDATE ResultadoAlunoRedacao SET REDQ3 = REPLACE(REDQ3,'.',',');
UPDATE ResultadoAlunoRedacao SET REDQ4 = ROUND(REDQ4, 1);
UPDATE ResultadoAlunoRedacao SET REDQ4 = REPLACE(REDQ4,'.',',');
UPDATE ResultadoAlunoRedacao SET REDQ5 = ROUND(REDQ5, 1);
UPDATE ResultadoAlunoRedacao SET REDQ5 = REPLACE(REDQ5,'.',',');


UPDATE ResultadoDreRedacao SET 
Valor = ROUND(Valor, 1),
PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
PercentualBasico = ROUND(PercentualBasico, 1),
PercentualAdequado = ROUND(PercentualAdequado, 1),
PercentualAvancado = ROUND(PercentualAvancado, 1)

UPDATE ResultadoEscolaRedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,',','.');
alter table ResultadoEscolaRedacao alter column NivelProficienciaID float;
UPDATE ResultadoEscolaRedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,'.',',');
update ResultadoEscolaRedacao set NivelProficienciaID = 0 where NivelProficienciaID is null
update ResultadoEscolaRedacao set TotalAlunos = 0 where TotalAlunos is null
update ResultadoEscolaRedacao set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico is null
update ResultadoEscolaRedacao set PercentualBasico = 0 where PercentualBasico is null
update ResultadoEscolaRedacao set PercentualAdequado = 0 where PercentualAdequado is null
update ResultadoEscolaRedacao set PercentualAvancado = 0 where PercentualAvancado is null


UPDATE ResultadoSMERedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,',','.');
alter table ResultadoSMERedacao alter column NivelProficienciaID float;
UPDATE ResultadoSMERedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,'.',',');
update ResultadoSMERedacao set NivelProficienciaID = 0 where NivelProficienciaID is null
update ResultadoSMERedacao set TotalAlunos = 0 where TotalAlunos is null
update ResultadoSMERedacao set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico is null
update ResultadoSMERedacao set PercentualBasico = 0 where PercentualBasico is null
update ResultadoSMERedacao set PercentualAdequado = 0 where PercentualAdequado is null
update ResultadoSMERedacao set PercentualAvancado = 0 where PercentualAvancado is null

UPDATE ResultadoTurmaRedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,',','.');
alter table ResultadoTurmaRedacao alter column NivelProficienciaID float;
UPDATE ResultadoTurmaRedacao SET NivelProficienciaID = REPLACE(NivelProficienciaID,'.',',');
update ResultadoTurmaRedacao set NivelProficienciaID = 0 where NivelProficienciaID is null
update ResultadoTurmaRedacao set TotalAlunos = 0 where TotalAlunos is null
update ResultadoTurmaRedacao set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico is null
update ResultadoTurmaRedacao set PercentualBasico = 0 where PercentualBasico is null
update ResultadoTurmaRedacao set PercentualAdequado = 0 where PercentualAdequado is null
update ResultadoTurmaRedacao set PercentualAvancado = 0 where PercentualAvancado is null


begin tran

insert into ProvaSP.dbo.ParticipacaoDREAreaConhecimento 
(Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,
TotalPrevisto,TotalPresente,
PercentualParticipacao
from ParticipacaoDRERedacao 

insert into ProvaSP.dbo.ParticipacaoEscolaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,
AnoEscolar,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,
AnoEscolar,TotalPrevisto,TotalPresente,
PercentualParticipacao
from ParticipacaoEscolaRedacao


insert into ProvaSP.dbo.ParticipacaoSMEAreaConhecimento
(Edicao,AreaConhecimentoID,AnoEscolar,
TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,AreaConhecimentoID,AnoEscolar,
TotalPrevisto,TotalPresente,
PercentualParticipacao
from ParticipacaoSMERedacao


insert into ProvaSP.dbo.ParticipacaoTurmaAreaConhecimento
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
tur_codigo,tur_id,TotalPrevisto,TotalPresente,PercentualParticipacao)
select Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
tur_codigo,tur_id,TotalPrevisto,TotalPresente,
PercentualParticipacao
from ParticipacaoTurmaRedacao


insert into ProvaSP.dbo.ResultadoAluno
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,tur_id,
alu_matricula,alu_nome,NivelProficienciaID,Valor,REDQ1,REDQ2,REDQ3,REDQ4,REDQ5)
select Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,
tur_id,alu_matricula,alu_nome,
NivelProficienciaID,
Valor,
REDQ1,
REDQ2,
REDQ3,
REDQ4,
REDQ5
from ResultadoAlunoRedacao


insert into ProvaSP.dbo.ResultadoDre
(Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,Valor,NivelProficienciaID,TotalAlunos,
PercentualAbaixoDoBasico,PercentualBasico,PercentualAdequado,PercentualAvancado)
select Edicao,AreaConhecimentoID,uad_sigla,AnoEscolar,
Valor,
NivelProficienciaID,
TotalAlunos,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado
from ResultadoDreRedacao


insert into ProvaSP.dbo.ResultadoEscola
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,Valor,
NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,PercentualBasico,
PercentualAdequado,PercentualAvancado)
select Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,
Valor,
convert(int, NivelProficienciaID) NivelProficienciaID,
TotalAlunos,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado
from ResultadoEscolaRedacao


insert into ProvaSP.dbo.ResultadoSME
(Edicao,
AreaConhecimentoID,
AnoEscolar,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado)
select Edicao,
AreaConhecimentoID,
AnoEscolar,
Valor,
TotalAlunos,
NivelProficienciaID,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado
from ResultadoSMERedacao


insert into ProvaSP.dbo.ResultadoTurma
(Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,
tur_id,Valor,NivelProficienciaID,TotalAlunos,PercentualAbaixoDoBasico,
PercentualBasico,PercentualAdequado,PercentualAvancado)
select Edicao,AreaConhecimentoID,uad_sigla,esc_codigo,AnoEscolar,tur_codigo,
tur_id,
Valor,
NivelProficienciaID,
TotalAlunos,
PercentualAbaixoDoBasico,
PercentualBasico,
PercentualAdequado,
PercentualAvancado
from ResultadoTurmaRedacao

commit







