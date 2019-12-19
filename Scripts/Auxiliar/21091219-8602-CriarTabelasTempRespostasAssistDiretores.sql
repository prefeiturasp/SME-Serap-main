----------
--- 25 ---
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
  and qu.QuestionarioID IN (25)

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
  and qu.QuestionarioID IN (25)


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
  and qu.QuestionarioID IN (25)

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
  and qu.QuestionarioID IN (25) 


---------------------
--ASSIST. DIRETORES
DROP TABLE Questionarios25_2019
SELECT piv.* INTO Questionarios25_2019
	FROM (
			SELECT qri.QuestionarioUsuarioID, Numero, Valor
			FROM QuestionarioUsuario qu WITH (NOLOCK)
				INNER JOIN Questionario q WITH (NOLOCK) ON q.QuestionarioID = qu.QuestionarioID
				INNER JOIN QuestionarioItem qi WITH (NOLOCK) ON qi.QuestionarioID = q.QuestionarioID
				INNER JOIN QuestionarioRespostaItem qri WITH (NOLOCK) ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID AND qi.QuestionarioItemID = qri.QuestionarioItemID
			WHERE qu.QuestionarioID = 25
				AND qu.Edicao IN ('2019')
		) src
	PIVOT(
		MAX(Valor)
		FOR Numero IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],
			[26],[27],[28],[29],[30],[31],[32],[33],[34],[35],[36],[37],[38],[39],[40],[41],[42],[43],[44],[45],[46],[47],[48],[49],[50],
			[51],[52],[53],[54],[55],[56],[57],[58],[59],[60],[61],[62],[63],[64],[65],[66],[67],[68],[69],[70],[71],[72],[73],[74],[75],
			[76],[77],[78],[79],[80],[81],[82],[83],[84],[85],[86],[87],[88],[89],[90],[91],[92],[93],[94],[95],[96],[97],[98],[99],[100],
			[101],[102],[103],[104],[105],[106],[107],[108],[109],[110],[111],[112],[113],[114],[115],[116],[117],[118],[119],[120],[121],[122],[123],[124],[125],
			[126],[127],[128],[129],[130],[131],[132],[133],[134],[135])
	) piv	

ALTER TABLE Questionarios25_2019
ADD CONSTRAINT Questionarios25_2019_QuestionarioUsuarioID_FK FOREIGN KEY (QuestionarioUsuarioID) 
REFERENCES QuestionarioUsuario(QuestionarioUsuarioID)

CREATE INDEX Questionarios25_2019_QuestionarioUsuarioID_IDX ON Questionarios25_2019(QuestionarioUsuarioID);


---------------------
--ASSIST. DIRETORES
SELECT 
	u.usu_login,
	p.pes_id,
	p.pes_nome,
	ua.uad_sigla,
	ua.uad_nome,
	e.esc_codigo,
	e.esc_nome,
	(substring(u.usu_login, PatIndex('%[0-9]%', u.usu_login), len(u.usu_login)) % 6) + 1 NumCaderno,
	rc.*
FROM QuestionarioUsuario qu
	INNER JOIN CoreSSO.dbo.SYS_Usuario u 
		ON u.usu_id = qu.usu_id
	INNER JOIN CoreSSO.dbo.PES_Pessoa p
		ON p.pes_id = u.pes_id
	INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e 
		ON e.esc_codigo = qu.esc_codigo
	INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua 
		ON e.uad_idSuperiorGestao = ua.uad_id	
	INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec
		ON e.esc_id = ec.esc_id
	INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce
		ON ec.tce_id = tce.tce_id
	INNER JOIN Questionarios25_2019 rc 
		ON rc.QuestionarioUsuarioID = qu.QuestionarioUsuarioID 
WHERE
	qu.Edicao = '2019' AND
	qu.QuestionarioID = 25 AND
	e.esc_situacao = 1 AND
	tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')