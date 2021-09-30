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

-- Alunos da Prodan que não possuem usuário no CoreSSO
-- 157 resultados
SELECT
	*
FROM
	AlunosETurmasDo7Ao9EnsinoFundamental prodan
LEFT JOIN
	#UsuarioSemPReFixoRADaProdan usuarios
	ON prodan.cd_aluno = usuarios.usu_login
WHERE
	usuarios.usu_login IS NULL;

-- Usuário sem aluno
-- 65 resultados
select alu.alu_id, alc.alc_matricula, alu.pes_id
INTO #tempAlunosPedagogica
from 
	GestaoPedagogica..ACA_AlunoCurriculo alc (nolock)
inner join
	GestaoPedagogica..ACA_Aluno alu (NOLOCK)
	on alc.alu_id = alu.alu_id
where alc_situacao = 1
group by alu.alu_id, alc_matricula, pes_id;

SELECT 
	usuarios.pes_id,
	usuarios.usu_login,
	aluno_peda.alu_id AS AluId_Pedagogica,
	aluno_peda.alc_matricula AS AluMatricula_Pedagogica,
	aluno.alu_id AS AluId_SGP,
	aluno.alu_matricula AS AluMatricula_SGP
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

-- Usuários com usulogin dfirente do alu_matricula
-- 311 usuários
SELECT *
FROM
	#UsuarioSemPReFixoRADaProdan usuarios
INNER JOIN
	ACA_Aluno aluno (NOLOCK)
	ON usuarios.pes_id = aluno.pes_id
WHERE
	aluno.alu_matricula <> usu_login;

--------------------------------------------------
-- UPDATES
BEGIN TRAN
UPDATE aluno
SET aluno.alu_matricula = usuarios.usu_login
FROM
	#UsuarioSemPReFixoRADaProdan usuarios
INNER JOIN
	ACA_Aluno aluno
	ON usuarios.pes_id = aluno.pes_id
WHERE
	aluno.alu_matricula <> usu_login;

--COMMIT
--ROLLBACK





