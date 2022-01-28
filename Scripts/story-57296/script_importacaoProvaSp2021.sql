
update ResultadoAluno set NivelProficienciaID = 0 where NivelProficienciaID = 'NA'
update ResultadoAluno set Valor = 0 where Valor = 'NA'
update ResultadoAluno set REDQ1 = 0 where REDQ1 = 'NA'
update ResultadoAluno set REDQ2 = 0 where REDQ2 = 'NA'
update ResultadoAluno set REDQ3 = 0 where REDQ3 = 'NA'
update ResultadoAluno set REDQ4 = 0 where REDQ4 = 'NA'
update ResultadoAluno set REDQ5 = 0 where REDQ5 = 'NA'

update ResultadoAluno set Valor = REPLACE(Valor,',','.')
alter table ResultadoAluno alter column NivelProficienciaID float
alter table ResultadoAluno alter column Valor float
alter table ResultadoAluno alter column REDQ1 float
alter table ResultadoAluno alter column REDQ2 float
alter table ResultadoAluno alter column REDQ3 float
alter table ResultadoAluno alter column REDQ4 float
alter table ResultadoAluno alter column REDQ5 float
UPDATE ResultadoAluno SET Valor = ROUND(Valor, 1)

update ResultadoDRE set PercentualAvancado = REPLACE(PercentualAvancado,',','.')
alter table ResultadoDRE alter column PercentualAvancado float

UPDATE ResultadoDRE SET 
Valor = ROUND(Valor, 1),
PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
PercentualBasico = ROUND(PercentualBasico, 1),
PercentualAdequado = ROUND(PercentualAdequado, 1),
PercentualAvancado = ROUND(PercentualAvancado, 1)
alter table ResultadoEscola alter column AreaConhecimentoID tinyint
alter table ResultadoEscola alter column TotalAlunos smallint
alter table ResultadoEscola alter column NivelProficienciaID float
update ResultadoEscola set PercentualAbaixoDoBasico = REPLACE(PercentualAbaixoDoBasico,',','.')
update ResultadoEscola set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico = 'NA'
alter table ResultadoEscola alter column PercentualAbaixoDoBasico float
update ResultadoEscola set PercentualBasico = REPLACE(PercentualBasico,',','.')
update ResultadoEscola set PercentualBasico = 0 where PercentualBasico = 'NA'
alter table ResultadoEscola alter column PercentualBasico float
update ResultadoEscola set PercentualAdequado = REPLACE(PercentualAdequado,',','.')
update ResultadoEscola set PercentualAdequado = 0 where PercentualAdequado = 'NA'
alter table ResultadoEscola alter column PercentualAdequado float
update ResultadoEscola set PercentualAvancado = REPLACE(PercentualAvancado,',','.')
update ResultadoEscola set PercentualAvancado = 0 where PercentualAvancado = 'NA'
alter table ResultadoEscola alter column PercentualAvancado float
UPDATE ResultadoEscola SET 
Valor = ROUND(Valor, 1),
PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
PercentualBasico = ROUND(PercentualBasico, 1),
PercentualAdequado = ROUND(PercentualAdequado, 1),
PercentualAvancado = ROUND(PercentualAvancado, 1)

alter table ResultadoSME alter column AreaConhecimentoID tinyint
alter table ResultadoSME alter column NivelProficienciaID float
update ResultadoSME set PercentualAvancado = REPLACE(PercentualAvancado,',','.')
update ResultadoSME set PercentualAvancado = 0 where PercentualAvancado = 'NA'
alter table ResultadoSME alter column PercentualAvancado float
alter table ResultadoSME add PercentualAlfabetizado float default null
update ResultadoSME set PercentualAlfabetizado = 0 where PercentualAlfabetizado is null
UPDATE ResultadoSME SET 
Valor = ROUND(Valor, 1),
PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
PercentualBasico = ROUND(PercentualBasico, 1),
PercentualAdequado = ROUND(PercentualAdequado, 1),
PercentualAvancado = ROUND(PercentualAvancado, 1)

alter table ResultadoTurma alter column AreaConhecimentoID tinyint
alter table ResultadoTurma alter column NivelProficienciaID float
alter table ResultadoTurma alter column TotalAlunos tinyint

update ResultadoTurma set PercentualBasico = REPLACE(PercentualBasico,',','.')
update ResultadoTurma set PercentualBasico = 0 where PercentualBasico = 'NA'
alter table ResultadoTurma alter column PercentualBasico float
update ResultadoTurma set PercentualAbaixoDoBasico = REPLACE(PercentualAbaixoDoBasico,',','.')
update ResultadoTurma set PercentualAbaixoDoBasico = 0 where PercentualAbaixoDoBasico = 'NA'
alter table ResultadoTurma alter column PercentualAbaixoDoBasico float
update ResultadoTurma set PercentualAdequado = REPLACE(PercentualAdequado,',','.')
update ResultadoTurma set PercentualAdequado = 0 where PercentualAdequado = 'NA'
alter table ResultadoTurma alter column PercentualAdequado float
update ResultadoTurma set PercentualAvancado = REPLACE(PercentualAvancado,',','.')
update ResultadoTurma set PercentualAvancado = 0 where PercentualAvancado = 'NA'
alter table ResultadoTurma alter column PercentualAvancado float
UPDATE ResultadoTurma SET 
Valor = ROUND(Valor, 1),
PercentualAbaixoDoBasico = ROUND(PercentualAbaixoDoBasico, 1),
PercentualBasico = ROUND(PercentualBasico, 1),
PercentualAdequado = ROUND(PercentualAdequado, 1),
PercentualAvancado = ROUND(PercentualAvancado, 1)



BEGIN TRAN

INSERT INTO ProvaSP.dbo.ProvaEdicao values ('2021', '2021-01-01');

-- OK
INSERT INTO ProvaSP.dbo.ResultadoAluno
(Edicao, AreaConhecimentoID, uad_sigla, esc_codigo, AnoEscolar, tur_codigo, tur_id, alu_matricula, alu_nome, NivelProficienciaID, Valor, REDQ1, REDQ2, REDQ3, REDQ4, REDQ5)
SELECT * FROM ResultadoAluno

-- OK
INSERT INTO ProvaSP.dbo.ResultadoDre
([Edicao],[AreaConhecimentoID],[uad_sigla],[AnoEscolar],[Valor],[NivelProficienciaID],[TotalAlunos]
,[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,[PercentualAlfabetizado])
SELECT
[Edicao],[AreaConhecimentoID],[uad_sigla],[AnoEscolar],[Valor],[NivelProficienciaID],[TotalAlunos]
,[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,0 [PercentualAlfabetizado]
FROM ResultadoDre

-- OK
INSERT INTO ProvaSP.dbo.ResultadoEscola
([Edicao],[AreaConhecimentoID],[uad_sigla],[esc_codigo],[AnoEscolar],[Valor],[NivelProficienciaID]
,[TotalAlunos],[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,[PercentualAlfabetizado])
SELECT 
[Edicao],[AreaConhecimentoID],[uad_sigla],[esc_codigo],[AnoEscolar],[Valor],[NivelProficienciaID]
,[TotalAlunos],[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,0 [PercentualAlfabetizado]
FROM ResultadoEscola

-- OK
INSERT INTO ProvaSP.dbo.ResultadoSme
([Edicao],[AreaConhecimentoID],[AnoEscolar],[Valor],[TotalAlunos],[NivelProficienciaID],[PercentualAbaixoDoBasico]
,[PercentualBasico],[PercentualAdequado],[PercentualAvancado],[PercentualAlfabetizado])
SELECT
[Edicao],[AreaConhecimentoID],[AnoEscolar],[Valor],[TotalAlunos],[NivelProficienciaID],[PercentualAbaixoDoBasico]
,[PercentualBasico],[PercentualAdequado],[PercentualAvancado],[PercentualAlfabetizado]
FROM ResultadoSme

-- OK
INSERT INTO ProvaSP.dbo.ResultadoTurma
([Edicao],[AreaConhecimentoID],[uad_sigla],[esc_codigo],[AnoEscolar],[tur_codigo],[tur_id],[Valor]
,[NivelProficienciaID],[TotalAlunos],[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,[PercentualAlfabetizado])
SELECT
[Edicao],[AreaConhecimentoID],[uad_sigla],[esc_codigo],[AnoEscolar],[tur_codigo],[tur_id],[Valor]
,[NivelProficienciaID],[TotalAlunos],[PercentualAbaixoDoBasico],[PercentualBasico],[PercentualAdequado],[PercentualAvancado]
,0 [PercentualAlfabetizado]
FROM ResultadoTurma

COMMIT
