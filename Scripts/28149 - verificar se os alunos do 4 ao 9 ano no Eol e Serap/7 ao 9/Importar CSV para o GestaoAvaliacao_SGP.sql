--Mudar configuração do arquivo para UTF ENdian pelo Notepad

drop table AlunosETurmasDo7Ao9EnsinoFundamental
CREATE TABLE AlunosETurmas(
cd_aluno bigint,
nm_aluno varchar (MAX),
SituacaoMatricula varchar (MAX),
nr_chamada_aluno varchar (MAX),
cd_escola char(6),
cd_turma_escola int,
dc_turma_escola varchar(MAX),
an_letivo smallint,
dc_tipo_periodicidade varchar(MAX),
dc_tipo_turno varchar(MAX),
st_turma_escola char(1),
cd_tipo_turma int,
dt_inicio_turma datetime,
dt_fim_turma datetime,
nr_ordem_serie smallint,
cd_modalidade_ensino int,
cd_etapa_ensino int,
nr_ordem_etapa smallint,
cd_serie_ensino int,
dc_serie_ensino varchar(MAX),
sg_modalidade_ensino varchar(MAX),
tur_id int
)

BULK INSERT AlunosETurmasDo7Ao9EnsinoFundamental
FROM 'C:\alunos.csv'
WITH
(
FIRSTROW = 2,
    FIELDTERMINATOR = ';',
    ROWTERMINATOR ='\n'
);

SELECT * FROM AlunosETurmasDo7Ao9EnsinoFundamental
