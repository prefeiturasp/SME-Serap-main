SET NOCOUNT ON

IF OBJECT_ID('tempdb..#Questionarios23_2019') IS NOT NULL
    DROP TABLE #Questionarios23_2019

SELECT piv.* 
INTO #Questionarios23_2019
    FROM (
            SELECT qri.QuestionarioUsuarioID, 	
				   qu.usu_id,										
				   Numero, 
				   Valor
            FROM QuestionarioUsuario qu (NOLOCK)
                INNER JOIN Questionario q (NOLOCK) 
					ON q.QuestionarioID = qu.QuestionarioID
                INNER JOIN QuestionarioItem qi (NOLOCK) 
					ON qi.QuestionarioID = q.QuestionarioID
                INNER JOIN QuestionarioRespostaItem qri (NOLOCK) 
					ON qri.QuestionarioUsuarioID = qu.QuestionarioUsuarioID AND 
					   qi.QuestionarioItemID = qri.QuestionarioItemID
            WHERE qu.QuestionarioID = 23 AND 
				  qu.Edicao IN ('2019')
        ) src
    PIVOT(
        MAX(Valor)
        FOR Numero IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],
					   [11],[12],[13],[14],[15],[16],[17],[18],[19],
					   [20],[21],[22],[23],[24],[25], [26],[27],[28],[29],
					   [30],[31],[32],[33],[34],[35],[36],[37],[38],[39],
					   [40],[41],[42],[43],[44],[45],[46],[47],[48],[49],
					   [50],[51],[52],[53],[54],[55],[56],[57],[58],[59],
					   [60],[61],[62],[63],[64],[65],[66],[67],[68],[69],
					   [70],[71],[72],[73],[74],[75],[76],[77],[78],[79],
					   [80],[81],[82],[83],[84],[85],[86],[87],[88],[89],
					   [90],[91],[92],[93],[94],[95])
    ) piv

-----------------PROFESSORES

SELECT 
    u.usu_login [Login],
    p.pes_id [Id Pessoa],
    p.pes_nome [Nome],
    ua.uad_sigla [Sigla UA],
    ua.uad_nome [Nome UA],    
	pe.esc_codigo [Cod. Escolas],
	pe.esc_nome [Escolas],
	pe.tur_codigo [Ano Escolar],
    (substring(u.usu_login, PatIndex('%[0-9]%', u.usu_login), len(u.usu_login)) % 6) + 1 NumCaderno,
    rc.*
FROM QuestionarioUsuario qu (NOLOCK)
    INNER JOIN CoreSSO.dbo.SYS_Usuario u (NOLOCK)
        ON u.usu_id = qu.usu_id
    INNER JOIN CoreSSO.dbo.PES_Pessoa p (NOLOCK)
        ON p.pes_id = u.pes_id
    INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e (NOLOCK)
        ON e.esc_codigo = qu.esc_codigo
    INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua (NOLOCK)
        ON e.uad_idSuperiorGestao = ua.uad_id   
    INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec (NOLOCK)
        ON e.esc_id = ec.esc_id
    INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce (NOLOCK)
        ON ec.tce_id = tce.tce_id	
    INNER JOIN #Questionarios23_2019 rc
        ON rc.QuestionarioUsuarioID = qu.QuestionarioUsuarioID		
	INNER JOIN ##ProfessoresEscolas2 pe
		ON p.pes_id = pe.pes_id
WHERE
    qu.Edicao = '2019' AND
    qu.QuestionarioID = 23 AND
    e.esc_situacao = 1 AND
    tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF') AND
	EXISTS (SELECT 1
				FROM GestaoAvaliacao_SGP.dbo.ACA_Docente d (NOLOCK)
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td (NOLOCK)
						ON d.doc_id = td.doc_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud (NOLOCK)
						ON td.tud_id = tud.tud_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t (NOLOCK)
						ON tud.tur_id = t.tur_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_CalendarioAnual ca (NOLOCK)
						ON t.cal_id = ca.cal_id
			WHERE t.tur_situacao <> 3 AND
				  ca.cal_ano = 2019 AND
				  d.pes_id = p.pes_id)

DROP TABLE #Questionarios23_2019