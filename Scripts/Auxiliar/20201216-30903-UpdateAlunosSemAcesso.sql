--RA5054919
UPDATE GestaoAvaliacao_SGP..MTR_MatriculaTurma SET mtu_dataSaida = NULL WHERE alu_id = 2664064 AND mtu_id = 1

--RA7617730
DECLARE @Alu_id INT = (SELECT MAX(alu_id) FROM ACA_ALUNO) + 1
INSERT INTO ACA_ALUNO VALUES (@Alu_id,'CHIARA NAKAI HERCOLI','6CF424DC-8EC3-E011-9B36-00155D033206','7617730',GETDATE(),GETDATE(),1,NULL,NULL,'D5AFFCEB-5A29-4257-A651-3F4756B13CF9')
INSERT INTO Mtr_MatriculaTurma VALUES(2882916,1,247,323384,117,1,5,1,GETDATE(),GETDATE(),26,'2020-02-05',NULL,5)

--RA7203510
--Resolvido com script de deficiencias.

--RA4864556
UPDATE MTR_MatriculaTurma SET mtu_dataSaida = NULL, mtu_situacao = 1 WHERE alu_id = 942121 AND mtu_id = 7

--RA7546252
UPDATE MTR_MatriculaTurma SET mtu_dataSaida = NULL WHERE alu_id = 2666172 AND mtu_id = 1

--RA7502816
UPDATE MTR_MatriculaTurma SET mtu_dataSaida = NULL WHERE alu_id = 2671678 AND mtu_id = 1

--RA7204023
--Resolvido com script de deficiencias.

--RA4032949
--Resolvido com script de deficiencias.

--RA4715368
--Resolvido com script de deficiencias.

--RA6561047
--Resolvido com script de deficiencias.

--RA6905408
--Resolvido com script de deficiencias.

--RA7502837
UPDATE MTR_MatriculaTurma SET mtu_dataSaida = NULL, mtu_situacao = 1 WHERE alu_id = 2666912 AND mtu_id = 1

INSERT INTO CoreSSO..PES_PessoaDeficiencia
select
    DISTINCT
    sgp_aluno.pes_id,
    def.tde_id
from BD_PRODAM..v_alunos_da_turma_MSTECH  aluno
INNER JOIN Manutencao..DEPARA_DEFICIENCIA def ON def.id_nee = aluno.id_nee
INNER JOIN CoreSSO..PES_TipoDeficiencia tipo ON tipo.tde_id = def.tde_id
INNER JOIN GestaoAvaliacao_SGP..ACA_Aluno sgp_aluno ON sgp_aluno.alu_matricula = aluno.CL_ALU_CODIGO
LEFT JOIN CoreSSO..Pes_PessoaDeficiencia pesDef ON pesDef.pes_id = sgp_aluno.pes_id
where CL_DATST>='2020-01-01' AND sgp_aluno.alu_matricula IN ('7617730', '7203510', '4864556', '7204023', '4032949', '4715368', '6561047', '6905408')
AND pesDef.pes_id IS NULL