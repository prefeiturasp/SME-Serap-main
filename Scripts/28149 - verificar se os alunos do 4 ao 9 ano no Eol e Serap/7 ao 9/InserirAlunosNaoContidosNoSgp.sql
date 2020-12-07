drop table #UsuarioSemPReFixoRA;
drop table #tempAlunosSerap;
drop table #UsuarioSemPReFixoRADaProdan;

--<CoreSSO>
select RTRIM(LTRIM(Replace(usu_login,'RA',''))) as usu_login,pes_id 
INTO #UsuarioSemPReFixoRA 
from CoreSSO..Sys_Usuario	
Where RTRIM(LTRIM(usu_login)) LIKE 'RA%';

Delete from #UsuarioSemPReFixoRA
Where ISNUMERIC(usu_login) <> 1;

SELECT temp.*
INTO #UsuarioSemPReFixoRADaProdan
FROM #UsuarioSemPReFixoRA temp
INNER JOIN AlunosETurmasDo7Ao9EnsinoFundamental prodan
ON temp.usu_login = prodan.cd_aluno
--</CoreSSO>

-- Usuário sem aluno
-- 65 resultados
DROP TABLE #tempAlunosPedagogica;
select alu.alu_id, alc.alc_matricula, alu.pes_id
INTO #tempAlunosPedagogica
from 
	GestaoPedagogica..ACA_AlunoCurriculo alc (nolock)
inner join
	GestaoPedagogica..ACA_Aluno alu (NOLOCK)
	on alc.alu_id = alu.alu_id
group by alu.alu_id, alc_matricula, pes_id;

SELECT 
	usuarios.pes_id,
	usuarios.usu_login,
	aluno_peda.alu_id AS AluId_Pedagogica,
	aluno_peda.alc_matricula AS AluMatricula_Pedagogica,
	aluno.alu_id AS AluId_SGP,
	aluno.alu_matricula AS AluMatricula_SGP
INTO #tempAlunosParaSeremCriados
FROM
	#UsuarioSemPReFixoRADaProdan usuarios
LEFT JOIN
	#tempAlunosPedagogica AS aluno_peda
	 ON aluno_peda.pes_id = usuarios.pes_id
LEFT JOIN
	ACA_Aluno aluno (NOLOCK)
	ON usuarios.pes_id = aluno.pes_id
WHERE
	aluno.alu_matricula IS NULL
	OR aluno_peda.alc_matricula IS NULL;

SELECT * FROM #tempAlunosParaSeremCriados

-- INSERT
BEGIN TRAN
INSERT INTO ACA_Aluno
(alu_id, pes_id, alu_nome, ent_id, alu_matricula, alu_dataCriacao, alu_dataAlteracao, alu_situacao)
SELECT 
	alu.alu_id,
	alu.pes_id,
	pes.pes_nome,
	alu.ent_id,
	temp.AluMatricula_Pedagogica,
	GETDATE(),
	GETDATE(),
	alu.alu_situacao
FROM
	#tempAlunosParaSeremCriados temp
INNER JOIN
	GestaoPedagogica..ACA_Aluno alu (NOLOCK)
	ON temp.AluId_Pedagogica = alu.alu_id
INNER JOIN
	CoreSSO..PES_Pessoa pes
	ON pes.pes_id = alu.pes_id;

--COMMIT
--ROLLBACK