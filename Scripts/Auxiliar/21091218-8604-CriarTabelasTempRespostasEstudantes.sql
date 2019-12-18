----------
--- Remover duplicados 21 ---
----------
DELETE FROM qri2  
--select distinct qri2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID + 1 = qu2.QuestionarioUsuarioID
		AND qu.DataPreenchimento = qu2.DataPreenchimento
	join QuestionarioRespostaItem qri2 ON qri2.QuestionarioUsuarioID = qu2.QuestionarioUsuarioID
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 21

DELETE FROM qu2
--select distinct qu2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID + 1 = qu2.QuestionarioUsuarioID
		AND qu.DataPreenchimento = qu2.DataPreenchimento
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 21


DELETE FROM qri2  
--select distinct qri2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID < qu2.QuestionarioUsuarioID
		--AND qu.DataPreenchimento = qu2.DataPreenchimento
	join QuestionarioRespostaItem qri2 ON qri2.QuestionarioUsuarioID = qu2.QuestionarioUsuarioID
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 21

DELETE FROM qu2
--select distinct qu2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID < qu2.QuestionarioUsuarioID
		--AND qu.DataPreenchimento = qu2.DataPreenchimento
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 21

----------
--- Remover duplicados 22 ---
----------
DELETE FROM qri2  
--select distinct qri2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID + 1 = qu2.QuestionarioUsuarioID
		AND qu.DataPreenchimento = qu2.DataPreenchimento
	join QuestionarioRespostaItem qri2 ON qri2.QuestionarioUsuarioID = qu2.QuestionarioUsuarioID
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 22

DELETE FROM qu2
--select distinct qu2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID + 1 = qu2.QuestionarioUsuarioID
		AND qu.DataPreenchimento = qu2.DataPreenchimento
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 22


DELETE FROM qri2  
--select distinct qri2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID < qu2.QuestionarioUsuarioID
		--AND qu.DataPreenchimento = qu2.DataPreenchimento
	join QuestionarioRespostaItem qri2 ON qri2.QuestionarioUsuarioID = qu2.QuestionarioUsuarioID
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 22

DELETE FROM qu2
--select distinct qu2.* --qu.QuestionarioUsuarioID, qu2.QuestionarioUsuarioID, qu.DataPreenchimento, qu2.DataPreenchimento, qu.usu_id
FROM QuestionarioUsuario qu 
   JOIN QuestionarioUsuario qu2 ON 
		qu.Edicao = qu2.Edicao AND
		qu.usu_id = qu2.usu_id AND
		qu.QuestionarioID = qu2.QuestionarioID AND
		qu.QuestionarioUsuarioID < qu2.QuestionarioUsuarioID
		--AND qu.DataPreenchimento = qu2.DataPreenchimento
WHERE qu.Edicao = '2019'
  and qu.QuestionarioID = 22
  


-----------------------
--ESTUDANTES 3-6 anos
DROP TABLE Questionarios21_2019
SELECT piv.* INTO Questionarios21_2019
	FROM (
			SELECT top 1000 qri.QuestionarioUsuarioID, Numero, Valor
			FROM QuestionarioUsuario qu WITH (NOLOCK)
				INNER JOIN Questionario q WITH (NOLOCK) ON q.QuestionarioID = qu.QuestionarioID
				INNER JOIN QuestionarioItem qi WITH (NOLOCK) ON qi.QuestionarioID = q.QuestionarioID
				INNER JOIN QuestionarioRespostaItem qri WITH (NOLOCK) ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID AND qi.QuestionarioItemID = qri.QuestionarioItemID
			WHERE qu.QuestionarioID = 21
				AND qu.Edicao IN ('2019')
		) src
	PIVOT(
		MAX(Valor)
		FOR Numero IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],
			[26],[27],[28],[29],[30],[31],[32],[33],[34],[35],[36],[37],[38],[39],[40],[41],[42],[43],[44],[45],[46],[47],[48],[49],[50],
			[51],[52],[53],[54],[55],[56],[57],[58],[59],[60],[61],[62],[63],[64],[65],[66],[67],[68],[69],[70],[71],[72],[73],[74],[75],
			[76],[77],[78],[79],[80],[81],[82],[83],[84],[85],[86],[87])
	) piv
	
ALTER TABLE Questionarios21_2019
ADD CONSTRAINT Questionarios21_2019_QuestionarioUsuarioID_FK FOREIGN KEY (QuestionarioUsuarioID) 
REFERENCES QuestionarioUsuario(QuestionarioUsuarioID)

CREATE INDEX Questionarios21_2019_QuestionarioUsuarioID_IDX ON Questionarios21_2019(QuestionarioUsuarioID);

-----------------------
--ESTUDANTES 7-9 anos	
DROP TABLE Questionarios22_2019
SELECT piv.* INTO Questionarios22_2019
	FROM (
			SELECT top 1000 qri.QuestionarioUsuarioID, Numero, Valor
			FROM QuestionarioUsuario qu WITH (NOLOCK)
				INNER JOIN Questionario q WITH (NOLOCK) ON q.QuestionarioID = qu.QuestionarioID
				INNER JOIN QuestionarioItem qi WITH (NOLOCK) ON qi.QuestionarioID = q.QuestionarioID
				INNER JOIN QuestionarioRespostaItem qri WITH (NOLOCK) ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID AND qi.QuestionarioItemID = qri.QuestionarioItemID
			WHERE qu.QuestionarioID = 22
				AND qu.Edicao IN ('2019')
		) src
	PIVOT(
		MAX(Valor)
		FOR Numero IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],
			[26],[27],[28],[29],[30],[31],[32],[33],[34],[35],[36],[37],[38],[39],[40],[41],[42],[43],[44],[45],[46],[47],[48],[49],[50],
			[51],[52],[53],[54],[55],[56],[57],[58],[59],[60],[61],[62],[63],[64],[65],[66],[67],[68],[69],[70],[71],[72],[73],[74],[75],
			[76],[77],[78],[79],[80],[81],[82],[83],[84],[85],[86],[87],[88],[89],[90],[91],[92],[93],[94],[95],[96],[97],[98],[99],[100],
		[101],[102],[103],[104],[105],[106],[107],[108],[109],[110],[111],[112],[113],[114])
	) piv	

ALTER TABLE Questionarios22_2019
ADD CONSTRAINT Questionarios22_2019_QuestionarioUsuarioID_FK FOREIGN KEY (QuestionarioUsuarioID) 
REFERENCES QuestionarioUsuario(QuestionarioUsuarioID)

CREATE INDEX Questionarios22_2019_QuestionarioUsuarioID_IDX ON Questionarios22_2019(QuestionarioUsuarioID);


-----------------------
--ESTUDANTES 3-6 anos
SELECT a.alu_matricula codigoAluno,
	--'INEP' CodigoINEP,
	u.usu_login,
	m.mtu_id,
	a.alu_id,
	m.crp_id as AnoEscolar,
	--m.mtu_dataMatricula,
	--m.mtu_dataSaida,
	tt.ttn_id,
	tt.ttn_nome,
	ua.uad_sigla,
	ua.uad_nome,
	e.esc_codigo,
	e.esc_nome,
	t.tur_id,
	t.tur_codigo,
	(substring(u.usu_login, PatIndex('%[0-9]%', u.usu_login), len(u.usu_login)) % 6) + 1 NumCaderno,
	rc.*
FROM QuestionarioUsuario qu
	INNER JOIN CoreSSO.dbo.SYS_Usuario u 
		ON u.usu_id = qu.usu_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_Aluno a 
		ON a.pes_id = u.pes_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m 
		ON a.alu_id = m.alu_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t 
		ON m.tur_id = t.tur_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoTurno tt
		ON tt.ttn_id = t.ttn_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e 
		ON t.esc_id = e.esc_id AND e.esc_codigo = qu.esc_codigo
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua 
		ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec
		ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce
		ON ec.tce_id = tce.tce_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaTipoCurriculoPeriodo ttcp 
		ON ttcp.tur_id = t.tur_id 
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoModalidadeEnsino tme 
		ON tme.tme_id = ttcp.tme_id
	INNER JOIN Questionarios21_2019 rc 
		ON rc.QuestionarioUsuarioID = qu.QuestionarioUsuarioID 
WHERE (m.mtu_situacao = 1 
		OR (m.mtu_dataMatricula <= CONVERT(varchar(10),'2019.12.01',102) 
			AND (m.mtu_dataSaida IS NULL OR m.mtu_dataSaida >= CONVERT(varchar(10),'2019.12.01',102) ))) AND
	m.mtu_id = (SELECt MAX(m2.mtu_id) FROM GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m2
					WHERE a.alu_id = m2.alu_id AND
						(m2.mtu_situacao = 1 
							OR (m2.mtu_dataMatricula <= CONVERT(varchar(10),'2019.12.01',102) 
								AND (m2.mtu_dataSaida IS NULL OR m2.mtu_dataSaida >= CONVERT(varchar(10),'2019.12.01',102) )))) AND
	qu.Edicao = '2019' AND
	qu.QuestionarioID = 21 AND
	m.crp_id BETWEEN 3 AND 6 AND
	u.usu_login LIKE 'RA%' AND
	e.esc_situacao = 1 AND
	tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF') AND
	tme.tme_nome NOT IN ('EJA Especial', 'Educação de Jovens e Adultos - EJA', 'EJA Regular')
	
-----------------------
--ESTUDANTES 7-9 anos	
SELECT a.alu_matricula codigoAluno,
	--'INEP' CodigoINEP,
	u.usu_login,
	m.mtu_id,
	a.alu_id,
	m.crp_id as AnoEscolar,
	--m.mtu_dataMatricula,
	--m.mtu_dataSaida,
	tt.ttn_id,
	tt.ttn_nome,
	ua.uad_sigla,
	ua.uad_nome,
	e.esc_codigo,
	e.esc_nome,
	t.tur_id,
	t.tur_codigo,
	(substring(u.usu_login, PatIndex('%[0-9]%', u.usu_login), len(u.usu_login)) % 6) + 1 NumCaderno,
	rc.*
FROM QuestionarioUsuario qu
	INNER JOIN CoreSSO.dbo.SYS_Usuario u 
		ON u.usu_id = qu.usu_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_Aluno a 
		ON a.pes_id = u.pes_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m 
		ON a.alu_id = m.alu_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t 
		ON m.tur_id = t.tur_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoTurno tt
		ON tt.ttn_id = t.ttn_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e 
		ON t.esc_id = e.esc_id AND e.esc_codigo = qu.esc_codigo
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua 
		ON e.uad_idSuperiorGestao = ua.uad_id
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec
		ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce
		ON ec.tce_id = tce.tce_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaTipoCurriculoPeriodo ttcp 
		ON ttcp.tur_id = t.tur_id 
	INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_TipoModalidadeEnsino tme 
		ON tme.tme_id = ttcp.tme_id
	INNER JOIN Questionarios22_2019 rc 
		ON rc.QuestionarioUsuarioID = qu.QuestionarioUsuarioID 
WHERE (m.mtu_situacao = 1 
		OR (m.mtu_dataMatricula <= CONVERT(varchar(10),'2019.12.01',102) 
			AND (m.mtu_dataSaida IS NULL OR m.mtu_dataSaida >= CONVERT(varchar(10),'2019.12.01',102) ))) AND
	qu.Edicao = '2019' AND
	qu.QuestionarioID = 22 AND
	m.crp_id BETWEEN 7 AND 9 AND
	u.usu_login LIKE 'RA%' AND
	e.esc_situacao = 1 AND
	tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF') AND
	tme.tme_nome NOT IN ('EJA Especial', 'Educação de Jovens e Adultos - EJA', 'EJA Regular')
