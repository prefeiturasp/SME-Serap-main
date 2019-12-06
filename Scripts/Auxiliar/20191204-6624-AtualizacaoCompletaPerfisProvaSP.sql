USE ProvaSP
GO

CREATE PROCEDURE SP_ConfigurarQuestProvaSP 	@Edicao VARCHAR(4),
											@AnoEscolarInicial INT,
											@AnoEscolarFinal INT = NULL
AS
BEGIN
/*
 Importação de ALUNOS, PROFESSORES, DIRETORES E ASSISTENTES DE DIRETORES para o preenchimento da ProvaSP.
 EXEC SP_ConfigurarQuestProvaSP '2019', 3, 9
 DROP PROCEDURE SP_ConfigurarQuestProvaSP
*/

	DECLARE @questionarioHabilitado INT

	SELECT 
		@questionarioHabilitado = (CASE Valor WHEN 'true' THEN 1 ELSE 0 END)
	FROM Configuracao
	WHERE Chave = 'DisponibilizarPreenchimentoQuestionariosFichas'
	  AND Valor = 'true'

	IF (@questionarioHabilitado = 1)
	BEGIN
		--se for uma function, serão passados como parametro
		--DECLARE @Edicao VARCHAR(4) = '2019'
		--DECLARE @AnoEscolarInicial INT = 3
		--DECLARE @AnoEscolarFinal INT = 9

		PRINT('Importação de ALUNOS')
		PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))

		IF (@AnoEscolarFinal IS NULL)
			SET @AnoEscolarFinal = @AnoEscolarInicial

		IF OBJECT_ID('tempdb..#AlunosProvaSP') IS NOT NULL
			DROP TABLE #AlunosProvaSP

		IF OBJECT_ID('tempdb..#PessoasAluno') IS NOT NULL
			DROP TABLE #PessoasAluno

		SELECT DISTINCT @Edicao Edicao,				
						a.alu_matricula,
						a.alu_nome,
						m.mtu_numeroChamada ChamadaAluno,
						m.tur_id,
						m.crp_id AnoEscolar,
						t.tur_codigo,
						u.usu_id,
						e.esc_codigo,
						6 PerfilID, -- 6=Aluno
						ua.uad_sigla,
						u.usu_login
		INTO #AlunosProvaSP				
			FROM GestaoAvaliacao_SGP.dbo.ACA_Aluno a
				INNER JOIN GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m
					ON a.alu_id = m.alu_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t
					ON m.tur_id = t.tur_id
				INNER JOIN CoreSSO.dbo.SYS_Usuario u
					ON a.pes_id = u.pes_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.ESC_Escola e
					ON t.esc_id = e.esc_id
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
				LEFT JOIN ProvaSP.dbo.PessoaPerfil pf 
					ON pf.Edicao = @Edicao AND pf.PerfilID = 6 AND pf.usu_id = u.usu_id
		WHERE m.mtu_situacao = 1 AND
			  m.crp_id BETWEEN @AnoEscolarInicial AND @AnoEscolarFinal AND
			  u.usu_login LIKE 'RA%' AND
			  e.esc_situacao = 1 AND
			  tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF') AND
			  tme.tme_nome NOT IN ('EJA Especial', 'Educação de Jovens e Adultos - EJA', 'EJA Regular') AND
			  pf.usu_id is null
		ORDER BY a.alu_nome

		BEGIN TRY
			BEGIN TRAN

			SELECT ap.usu_id,
				   ap.alu_nome,
				   ap.usu_login
			INTO #PessoasAluno
				FROM #AlunosProvaSP ap
					LEFT JOIN Pessoa p
						ON ap.usu_id = p.usu_id
			WHERE p.usu_id IS NULL

			INSERT INTO Pessoa
			SELECT usu_id,
				   alu_nome,
				   usu_login
			FROM #PessoasAluno

			INSERT INTO Aluno
			SELECT DISTINCT Edicao,
							alu_matricula,
							alu_nome,
							ChamadaAluno,
							tur_id,
							AnoEscolar,
							tur_codigo
			FROM #AlunosProvaSP	

			INSERT INTO PessoaPerfil
			SELECT DISTINCT Edicao,
							usu_id,
							esc_codigo,
							PerfilID,
							uad_sigla
			FROM #AlunosProvaSP	

			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH

		IF OBJECT_ID('tempdb..#AlunosProvaSP') IS NOT NULL
			DROP TABLE #AlunosProvaSP

		IF OBJECT_ID('tempdb..#PessoasAluno') IS NOT NULL
			DROP TABLE #PessoasAluno
		--FIM importação de ALUNOS

/*
Importação de PROFESSORES
*/
		BEGIN TRY

			PRINT('Importação de PROFESSORES')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;

			WITH professores_prova_sp AS 
			(
				SELECT DISTINCT @Edicao Edicao,
								u.usu_id,
								0 esc_codigo,
								4 PerfilID,
								null uad_sigla,
								null esc_nome,
								p.pes_nome,
								u.usu_login
				FROM CoreSSO.dbo.SYS_Usuario u
					INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_Docente d
						ON u.pes_id = d.pes_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td
						ON d.doc_id = td.doc_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud
						ON td.tud_id = tud.tud_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t
						ON tud.tur_id = t.tur_id
					INNER JOIN GestaoAvaliacao.dbo.SGP_ESC_Escola e
						ON t.esc_id = e.esc_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua
						ON e.uad_idSuperiorGestao = ua.uad_id
					INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec
						ON e.esc_id = ec.esc_id
					INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce
						ON ec.tce_id = tce.tce_id
					INNER JOIN CoreSSO.dbo.PES_Pessoa p
						ON u.pes_id = p.pes_id
					LEFT JOIN ProvaSP.dbo.PessoaPerfil pf 
						ON pf.Edicao = @Edicao AND pf.PerfilID = 4 AND pf.usu_id = u.usu_id
				WHERE d.doc_situacao = 1 AND
					  e.esc_situacao = 1 AND
					  u.usu_situacao = 1 AND
					  tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF') AND
					  pf.usu_id IS NULL
			)

			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla,	   
				   esc_nome,
				   pes_nome,
				   usu_login
			INTO #PessoasPerfilProf
			FROM professores_prova_sp
			ORDER BY usu_id

			SELECT DISTINCT pp.usu_id,
							pp.pes_nome,
							pp.usu_login
			INTO #PessoasProf
			FROM #PessoasPerfilProf pp
				LEFT JOIN Pessoa p
					ON pp.usu_id = p.usu_id
			WHERE p.usu_id IS NULL

			SELECT DISTINCT pp.esc_codigo,
							pp.uad_sigla,
							pp.esc_nome
			INTO #EscolasProf
			FROM #PessoasPerfilProf pp
				LEFT JOIN Escola e
					ON pp.esc_codigo = e.esc_codigo AND
					   pp.uad_sigla = e.uad_codigo
			WHERE pp.esc_codigo <> 0 AND
				  e.esc_codigo IS NULL

			INSERT INTO Escola
			SELECT esc_codigo,
				   uad_sigla,
				   esc_nome
			FROM #EscolasProf

			INSERT INTO Pessoa
			SELECT usu_id,
				   pes_nome,
				   usu_login
			FROM #PessoasProf

			INSERT INTO PessoaPerfil
			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla
			FROM #PessoasPerfilProf

			IF OBJECT_ID('tempdb..#PessoasPerfilProf') IS NOT NULL
				DROP TABLE #PessoasPerfilProf

			IF OBJECT_ID('tempdb..#PessoasProf') IS NOT NULL
				DROP TABLE #PessoasProf

			IF OBJECT_ID('tempdb..#EscolasProf') IS NOT NULL
				DROP TABLE #EscolasProf

			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--FIM Professores

/*
Importação de ASSISTENTE DE DIRETORES
*/
		BEGIN TRY

			PRINT('Importação de ASSIST. DIRETORES')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;

			WITH assistenteDir_prova_sp AS 
			(
				SELECT DISTINCT @Edicao Edicao,
							u.usu_id,
							e.esc_codigo,
							2 PerfilID, -- Assistente Diretor fica como Diretor
							ua.uad_sigla,
							e.esc_nome,
							p.pes_nome,
							u.usu_login
				FROM CoreSSO.dbo.SYS_Usuario u
					INNER JOIN CoreSSO.dbo.PES_Pessoa p						ON u.pes_id = p.pes_id
					INNER JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug				ON u.usu_id = ug.usu_id
					INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c		ON p.pes_id = c.pes_id
					INNER JOIN GestaoPedagogica.dbo.RHU_ColaboradorCargo cc ON c.col_id = cc.col_id
					INNER JOIN GestaoPedagogica.dbo.ESC_Escola e			ON e.uad_id = cc.uad_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
					INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
					INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
					LEFT JOIN ProvaSP.dbo.PessoaPerfil pf					ON pf.Edicao = @Edicao AND pf.PerfilID = 2 AND pf.usu_id = u.usu_id
				WHERE ug.gru_id = 'ECF7A20D-1A1E-E811-B259-782BCB3D2D76' --Assistente Diretor
					AND ug.usg_situacao = 1
					AND u.usu_situacao = 1
					AND crg_id in (75, 76, 304)-- Assistente de Diretor
					AND cc.coc_vigenciaInicio <= getDate()
					AND (cc.coc_vigenciaFim is null OR  cc.coc_vigenciaFim > getDate())
					AND cc.coc_situacao = 1
					AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')
					AND pf.usu_id IS NULL
			)

			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla,	   
				   esc_nome,
				   pes_nome,
				   usu_login
			INTO #PessoasPerfilAssistDir
			FROM assistenteDir_prova_sp
			ORDER BY usu_id

			SELECT DISTINCT pp.usu_id,
							pp.pes_nome,
							pp.usu_login
			INTO #PessoasAssistDir
			FROM #PessoasPerfilAssistDir pp
				LEFT JOIN Pessoa p
					ON pp.usu_id = p.usu_id
			WHERE p.usu_id IS NULL

			SELECT DISTINCT pp.esc_codigo,
							pp.uad_sigla,
							pp.esc_nome
			INTO #EscolasAssistDir
			FROM #PessoasPerfilAssistDir pp
				LEFT JOIN Escola e
					ON pp.esc_codigo = e.esc_codigo AND
					   pp.uad_sigla = e.uad_codigo
			WHERE pp.esc_codigo <> 0 AND
				  e.esc_codigo IS NULL

			INSERT INTO Escola
			SELECT esc_codigo,
				   uad_sigla,
				   esc_nome
			FROM #EscolasAssistDir

			INSERT INTO Pessoa
			SELECT usu_id,
				   pes_nome,
				   usu_login
			FROM #PessoasAssistDir

			INSERT INTO PessoaPerfil
			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla
			FROM #PessoasPerfilAssistDir

			IF OBJECT_ID('tempdb..#PessoasPerfilAssistDir') IS NOT NULL
				DROP TABLE #PessoasPerfilAssistDir

			IF OBJECT_ID('tempdb..#PessoasAssistDir') IS NOT NULL
				DROP TABLE #PessoasAssistDir

			IF OBJECT_ID('tempdb..#EscolasAssistDir') IS NOT NULL
				DROP TABLE #EscolasAssistDir

			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--FIM ASSISTENTE DE DIRETORES

/*
Importação de DIRETORES
*/
		BEGIN TRY

			PRINT('Importação de DIRETORES')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;

			WITH diretores_prova_sp AS 
			(
				SELECT DISTINCT @Edicao Edicao,
							u.usu_id,
							e.esc_codigo,
							2 PerfilID, -- Diretor
							ua.uad_sigla,
							e.esc_nome,
							p.pes_nome,
							u.usu_login
				FROM CoreSSO.dbo.SYS_Usuario u
					INNER JOIN CoreSSO.dbo.PES_Pessoa p						ON u.pes_id = p.pes_id
					INNER JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug				ON u.usu_id = ug.usu_id
					INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c		ON p.pes_id = c.pes_id
					INNER JOIN GestaoPedagogica.dbo.RHU_ColaboradorCargo cc ON c.col_id = cc.col_id
					INNER JOIN GestaoPedagogica.dbo.ESC_Escola e			ON e.uad_id = cc.uad_id
					INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
					INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
					INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
					LEFT JOIN ProvaSP.dbo.PessoaPerfil pf					ON pf.Edicao = @Edicao AND pf.PerfilID = 2 AND pf.usu_id = u.usu_id
				WHERE ug.gru_id = '75DCAB30-2C1E-E811-B259-782BCB3D2D76' --Diretor
					AND ug.usg_situacao = 1
					AND u.usu_situacao = 1
					AND crg_id in (316, 115) -- Diretor
					AND cc.coc_vigenciaInicio <= getDate()
					AND (cc.coc_vigenciaFim is null OR  cc.coc_vigenciaFim > getDate())
					AND cc.coc_situacao = 1
					AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')
					AND pf.usu_id IS NULL
			)

			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla,	   
				   esc_nome,
				   pes_nome,
				   usu_login
			INTO #PessoasPerfilDiretor
			FROM diretores_prova_sp
			ORDER BY usu_id

			SELECT DISTINCT pp.usu_id,
							pp.pes_nome,
							pp.usu_login
			INTO #PessoasDiretor
			FROM #PessoasPerfilDiretor pp
				LEFT JOIN Pessoa p
					ON pp.usu_id = p.usu_id
			WHERE p.usu_id IS NULL

			SELECT DISTINCT pp.esc_codigo,
							pp.uad_sigla,
							pp.esc_nome
			INTO #EscolasDiretor
			FROM #PessoasPerfilDiretor pp
				LEFT JOIN Escola e
					ON pp.esc_codigo = e.esc_codigo AND
					   pp.uad_sigla = e.uad_codigo
			WHERE pp.esc_codigo <> 0 AND
				  e.esc_codigo IS NULL

			INSERT INTO Escola
			SELECT esc_codigo,
				   uad_sigla,
				   esc_nome
			FROM #EscolasDiretor

			INSERT INTO Pessoa
			SELECT usu_id,
				   pes_nome,
				   usu_login
			FROM #PessoasDiretor

			INSERT INTO PessoaPerfil
			SELECT Edicao,
				   usu_id,
				   esc_codigo,
				   PerfilID,
				   uad_sigla
			FROM #PessoasPerfilDiretor

			IF OBJECT_ID('tempdb..#PessoasPerfilDiretor') IS NOT NULL
				DROP TABLE #PessoasPerfilDiretor

			IF OBJECT_ID('tempdb..#PessoasDiretor') IS NOT NULL
				DROP TABLE #PessoasDiretor

			IF OBJECT_ID('tempdb..#EscolasDiretor') IS NOT NULL
				DROP TABLE #EscolasDiretor
	
			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--FIM DIRETORES

/*
Faz o vínculo de Diretores, Assistentes e Professores às escolas
*/
		BEGIN TRY

			PRINT('Criar vínculos com as escolas')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;

			-- DIRETOR
			UPDATE pf
				SET pf.esc_codigo = e.esc_codigo, pf.uad_sigla = ua.uad_sigla
			FROM GestaoPedagogica.dbo.RHU_ColaboradorCargo cc
				INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c	ON c.col_id = cc.col_id
				INNER JOIN CoreSSO.dbo.PES_Pessoa p					ON p.pes_id = c.pes_id
				INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON u.pes_id = p.pes_id
				INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = @Edicao AND pf.PerfilID = 2 AND pf.usu_id = u.usu_id
				INNER JOIN GestaoPedagogica.dbo.ESC_Escola e		ON e.uad_id = cc.uad_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
				INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
				INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			WHERE (crg_id in (316, 115) -- Diretor
				OR crg_id in (75, 76, 304))-- Assistente de Diretor
			  AND cc.coc_vigenciaInicio <= getDate()
			  AND (cc.coc_vigenciaFim is null OR  cc.coc_vigenciaFim > getDate())
			  AND cc.coc_situacao = 1
			  AND (pf.esc_codigo IS NULL OR pf.esc_codigo = 0)
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

			--Diretor com vinculo de escola
			UPDATE pf
				SET pf.esc_codigo = e.esc_codigo, pf.uad_sigla = ua.uad_sigla
			FROM GestaoPedagogica.dbo.RHU_ColaboradorCargo cc
				INNER JOIN GestaoPedagogica.dbo.RHU_Cargo cargo		ON cargo.crg_id = cc.crg_id
				INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c	ON c.col_id = cc.col_id
				INNER JOIN CoreSSO.dbo.PES_Pessoa p					ON p.pes_id = c.pes_id
				INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON u.pes_id = p.pes_id
				INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = @Edicao AND pf.PerfilID = 2 AND pf.usu_id = u.usu_id
				INNER JOIN GestaoPedagogica.dbo.ESC_Escola e		ON e.uad_id = cc.uad_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
				INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
				INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			WHERE (cargo.crg_descricao like 'PROF.%' OR cargo.crg_descricao like 'PROFESSOR%')
			  AND coc_vigenciaInicio <= getDate()
			  AND (coc_vigenciaFim is null OR  coc_vigenciaFim > getDate())
			  AND coc_situacao = 1
			  AND (pf.esc_codigo IS NULL OR pf.esc_codigo = 0)
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

			--PROFESSORES
			UPDATE pf
				SET pf.esc_codigo = e.esc_codigo, pf.uad_sigla = ua.uad_sigla
			FROM GestaoPedagogica.dbo.RHU_ColaboradorCargo cc
				INNER JOIN GestaoPedagogica.dbo.RHU_Cargo cargo		ON cargo.crg_id = cc.crg_id
				INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c	ON c.col_id = cc.col_id
				INNER JOIN CoreSSO.dbo.PES_Pessoa p					ON p.pes_id = c.pes_id
				INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON u.pes_id = p.pes_id
				INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = @Edicao AND pf.PerfilID = 4 AND pf.usu_id = u.usu_id
				INNER JOIN GestaoPedagogica.dbo.ESC_Escola e		ON e.uad_id = cc.uad_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
				INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
				INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			WHERE (cargo.crg_descricao like 'PROF.%' OR cargo.crg_descricao like 'PROFESSOR%')
			  AND coc_vigenciaInicio <= getDate()
			  AND (coc_vigenciaFim is null OR  coc_vigenciaFim > getDate())
			  AND coc_situacao = 1
			  AND (pf.esc_codigo IS NULL OR pf.esc_codigo = 0)
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')
			  
			--PROFESSORES pela tabela de docentes
			UPDATE pf
				SET pf.esc_codigo = e.esc_codigo, pf.uad_sigla = ua.uad_sigla
			FROM GestaoAvaliacao_SGP.dbo.ACA_Docente d
				INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDocente td ON d.doc_id = td.doc_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_TurmaDisciplina tud ON td.tud_id = tud.tud_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.TUR_Turma t ON tud.tur_id = t.tur_id
				INNER JOIN GestaoAvaliacao.dbo.SGP_ESC_Escola e ON t.esc_id = e.esc_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
				INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON u.pes_id = d.pes_id
				INNER JOIN CoreSSO.dbo.PES_Pessoa p					ON u.pes_id = p.pes_id
				INNER JOIN ProvaSP.dbo.PessoaPerfil pf				ON pf.Edicao = @Edicao AND pf.PerfilID = 4 AND pf.usu_id = u.usu_id
				INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
				INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			WHERE d.doc_situacao = 1
			  AND (pf.esc_codigo IS NULL OR pf.esc_codigo = 0)
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--Fim dos vínculos com escolas

/*
Após a inclusão dos registros, também é necessários remover os que não são mais usados
*/
		BEGIN TRY

			PRINT('Remover acessos descontinuados')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;
	
			--remover diretores sem perfil de diretor
			DELETE pf 
			FROM PessoaPerfil pf
				LEFT JOIN CoreSSO.dbo.SYS_Usuario u
					ON u.usu_id = pf.usu_id AND u.usu_situacao = 1
				LEFT JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug 
					ON u.usu_id = ug.usu_id
						AND ug.gru_id IN ('75DCAB30-2C1E-E811-B259-782BCB3D2D76', 'ECF7A20D-1A1E-E811-B259-782BCB3D2D76') --Diretor OU Assistente
						AND ug.usg_situacao = 1
				LEFT JOIN QuestionarioUsuario qu 
					ON qu.usu_id = pf.usu_id
						AND qu.QuestionarioID IN (24, 25)--Diretor ou Assistente
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 2
			  AND ug.usu_id IS NULL
			  AND qu.QuestionarioUsuarioID IS NULL;

	
			--corrigir professores sem permissão
			INSERT INTO CoreSSO.dbo.SYS_UsuarioGrupo
				SELECT u.usu_id,
					'E77E81B1-191E-E811-B259-782BCB3D2D76' gru_id,
					1 usg_situacao
				FROM ProvaSP.dbo.PessoaPerfil pf
					JOIN CoreSSO.dbo.SYS_Usuario u ON u.usu_id = pf.usu_id
					LEFT JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug ON ug.usu_id = u.usu_id AND ug.gru_id = 'E77E81B1-191E-E811-B259-782BCB3D2D76'
				WHERE pf.Edicao = @Edicao
				  AND pf.PerfilID = 4
				  AND pf.esc_codigo IS NOT NULL
				  AND pf.esc_codigo > 0
				  AND u.usu_situacao = 1
				  AND ug.gru_id IS NULL;

			--remover professores sem perfil de professor
			DELETE pf
			--SELECT pf.*
			FROM PessoaPerfil pf
				LEFT JOIN CoreSSO.dbo.SYS_Usuario u
					ON u.usu_id = pf.usu_id AND u.usu_situacao = 1
				LEFT JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug
					ON u.usu_id = ug.usu_id
						AND ug.gru_id IN ('E77E81B1-191E-E811-B259-782BCB3D2D76') --Professor
						AND ug.usg_situacao = 1
				LEFT JOIN QuestionarioUsuario qu
					ON qu.usu_id = pf.usu_id
						AND qu.QuestionarioID IN (23)--Professor
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 4
			  AND ug.usu_id IS NULL
			  AND qu.QuestionarioUsuarioID IS NULL;
	  
			--remover alunos sem perfil de aluno
			DELETE pf
			FROM PessoaPerfil pf
				LEFT JOIN CoreSSO.dbo.SYS_Usuario u
					ON u.usu_id = pf.usu_id AND u.usu_situacao = 1
				LEFT JOIN CoreSSO.dbo.SYS_UsuarioGrupo ug
					ON u.usu_id = ug.usu_id
						AND ug.gru_id IN ('BD6D9CE6-9456-E711-9541-782BCB3D218E') --Aluno
						AND ug.usg_situacao = 1
				LEFT JOIN QuestionarioUsuario qu
					ON qu.usu_id = pf.usu_id
						AND qu.QuestionarioID IN (21,22)--Aluno
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 6
			  AND ug.usu_id IS NULL
			  AND qu.QuestionarioUsuarioID IS NULL;
	  
			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--Fim das remoções

/*
Recalcula a quantidade de profissionais que responderam os questionários e os totais para ser respondido
*/
		BEGIN TRY

			PRINT('Criar vínculos de questionários para acompanhamentos')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))
			BEGIN TRAN;
			
			UPDATE qu SET
				qu.esc_codigo = pf.esc_codigo
			FROM ProvaSP.dbo.QuestionarioUsuario qu
			  JOIN ProvaSP.dbo.PessoaPerfil pf ON qu.usu_id = pf.usu_id
			WHERE qu.QuestionarioID IN (24,25) --DIR, ASSIST.DIR
			  AND pf.Edicao = @Edicao
			  AND pf.PerfilID = 2
			  AND pf.esc_codigo IS NOT NULL
			  AND (qu.esc_codigo IS NULL OR qu.esc_codigo = 0 OR qu.esc_codigo = '')
			
			UPDATE qu SET
				qu.esc_codigo = pf.esc_codigo
			FROM ProvaSP.dbo.QuestionarioUsuario qu
			  JOIN ProvaSP.dbo.PessoaPerfil pf ON qu.usu_id = pf.usu_id
			WHERE qu.QuestionarioID IN (23) --PROF
			  AND pf.Edicao = @Edicao
			  AND pf.PerfilID = 4
			  AND pf.esc_codigo IS NOT NULL
			  AND (qu.esc_codigo IS NULL OR qu.esc_codigo = 0 OR qu.esc_codigo = '')
			  
			UPDATE aap SET aap.esc_codigo = qu.esc_codigo
			--select *
			FROM QuestionarioUsuario qu
			 JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND aap.AtributoID = 32 --QuestionarioDeProfessorPreenchido
			 LEFT JOIN AcompanhamentoAplicacaoPessoa aapDUP
				ON aap.Edicao = aapDUP.Edicao
				  AND aap.usu_id = aapDUP.usu_id
				  AND aap.AtributoID = aapDUP.AtributoID
				  AND coalesce(qu.esc_codigo, '-1') = coalesce(aapDUP.esc_codigo, '-1')
			WHERE qu.Edicao = @Edicao
			  AND qu.QuestionarioID = 23 -- Professor
			  AND coalesce(qu.esc_codigo, '-1') <> coalesce(aap.esc_codigo, '-1')
			  AND aapDUP.usu_id IS NULL
			  
			UPDATE aap SET aap.esc_codigo = qu.esc_codigo
			--select *
			FROM QuestionarioUsuario qu
			 JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND aap.AtributoID = 30 --QuestionarioDeDiretorPreenchido
			 LEFT JOIN AcompanhamentoAplicacaoPessoa aapDUP
				ON aap.Edicao = aapDUP.Edicao
				  AND aap.usu_id = aapDUP.usu_id
				  AND aap.AtributoID = aapDUP.AtributoID
				  AND coalesce(qu.esc_codigo, '-1') = coalesce(aapDUP.esc_codigo, '-1')
			WHERE qu.Edicao = @Edicao
			  AND qu.QuestionarioID = 24 -- Diretor
			  AND coalesce(qu.esc_codigo, '-1') <> coalesce(aap.esc_codigo, '-1')
			  AND aapDUP.usu_id IS NULL
			  
			UPDATE aap SET aap.esc_codigo = qu.esc_codigo
			--select *
			FROM QuestionarioUsuario qu
			 JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND aap.AtributoID = 83 --QuestionarioAssistenteDiretoriaPreenchido
			 LEFT JOIN AcompanhamentoAplicacaoPessoa aapDUP
				ON aap.Edicao = aapDUP.Edicao
				  AND aap.usu_id = aapDUP.usu_id
				  AND aap.AtributoID = aapDUP.AtributoID
				  AND coalesce(qu.esc_codigo, '-1') = coalesce(aapDUP.esc_codigo, '-1')
			WHERE qu.Edicao = @Edicao
			  AND qu.QuestionarioID = 25 -- Assist. Diretor
			  AND coalesce(qu.esc_codigo, '-1') <> coalesce(aap.esc_codigo, '-1')
			  AND aapDUP.usu_id IS NULL
	
			--Remover preenchimento de PROFESSORES que não existem mais
			DELETE FROM aap
			--SELECT aap.*
			FROM AcompanhamentoAplicacaoPessoa aap
			  LEFT JOIN QuestionarioUsuario qu
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND qu.QuestionarioID = 23 -- Professor
			  LEFT JOIN PessoaPerfil pf 
				ON pf.usu_id = aap.usu_id
				  AND pf.Edicao = aap.Edicao
				  AND pf.PerfilID = 4 -- Professor  
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 32 --QuestionarioDeProfessorPreenchido
			  AND qu.usu_id IS NULL
			  AND pf.usu_id IS NULL

			--Remover preenchimento de DIRETORES que não existem mais
			DELETE FROM aap
			--SELECT aap.*
			FROM AcompanhamentoAplicacaoPessoa aap
			  LEFT JOIN QuestionarioUsuario qu
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND qu.QuestionarioID = 24 -- Diretor
			  LEFT JOIN PessoaPerfil pf 
				ON pf.usu_id = aap.usu_id
				  AND pf.Edicao = aap.Edicao
				  AND pf.PerfilID = 2 -- Diretor  
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 30 --QuestionarioDeDiretorPreenchido
			  AND qu.usu_id IS NULL
			  AND pf.usu_id IS NULL

			--Remover preenchimento de ASSIST.DIRETORES que não existem mais
			DELETE FROM aap
			--SELECT aap.*
			FROM AcompanhamentoAplicacaoPessoa aap
			  LEFT JOIN QuestionarioUsuario qu
				ON aap.Edicao = qu.Edicao
				  AND aap.usu_id = qu.usu_id
				  AND qu.QuestionarioID = 25 -- Assistente de Diretoria
			  LEFT JOIN PessoaPerfil pf 
				ON pf.usu_id = aap.usu_id
				  AND pf.Edicao = aap.Edicao
				  AND pf.PerfilID = 2 -- Diretor  
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 83 --QuestionarioAssistenteDiretoriaPreenchido
			  AND qu.usu_id IS NULL
			  AND pf.usu_id IS NULL


			/*
			  Inclusão da lista de profissionais que ainda não responderam os questionários
			*/
			INSERT INTO AcompanhamentoAplicacaoPessoa
			SELECT DISTINCT pf.Edicao,
				pf.usu_id,
				pf.esc_codigo,
				pf.PerfilID,
				32, --QuestionarioDeProfessorPreenchido
				'0' --não respondido
			FROM PessoaPerfil pf
			  LEFT JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = pf.Edicao
				  AND aap.usu_id = pf.usu_id
				  AND coalesce(pf.esc_codigo, '-1') = coalesce(aap.esc_codigo, '-1')
				  AND aap.AtributoID = 32 --QuestionarioDeProfessorPreenchido
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 4 -- Professor
			  AND aap.usu_id IS NULL

			INSERT INTO AcompanhamentoAplicacaoPessoa
			SELECT DISTINCT pf.Edicao,
				pf.usu_id,
				pf.esc_codigo,
				pf.PerfilID,
				30, --QuestionarioDeDiretorPreenchido
				'0' --não respondido
			FROM PessoaPerfil pf
			  INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON pf.usu_id = u.usu_id
			  INNER JOIN CoreSSO.dbo.PES_Pessoa p				ON u.pes_id = p.pes_id
			  INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c	ON p.pes_id = c.pes_id
			  INNER JOIN GestaoPedagogica.dbo.RHU_ColaboradorCargo cc ON c.col_id = cc.col_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_Escola e		ON e.uad_id = cc.uad_id
			  INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			  LEFT JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = pf.Edicao
				  AND aap.usu_id = pf.usu_id
				  AND coalesce(pf.esc_codigo, '-1') = coalesce(aap.esc_codigo, '-1')
				  AND aap.AtributoID = 30 --QuestionarioDeDiretorPreenchido
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 2 -- Assit. Diretor  E Diretor
			  AND coalesce(pf.esc_codigo, '-1') = coalesce(e.esc_codigo, '-1')
			  AND aap.usu_id IS NULL
			  AND crg_id in (316, 115) -- Diretor
			  AND coc_vigenciaInicio <= getDate()
			  AND (coc_vigenciaFim is null OR  coc_vigenciaFim > getDate())
			  AND coc_situacao = 1
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')

			INSERT INTO AcompanhamentoAplicacaoPessoa
			SELECT DISTINCT pf.Edicao,
				pf.usu_id,
				pf.esc_codigo,
				pf.PerfilID,
				83, --QuestionarioAssistenteDiretoriaPreenchido
				'0' --não respondido
			FROM PessoaPerfil pf
			  INNER JOIN CoreSSO.dbo.SYS_Usuario u				ON pf.usu_id = u.usu_id
			  INNER JOIN CoreSSO.dbo.PES_Pessoa p				ON u.pes_id = p.pes_id
			  INNER JOIN GestaoPedagogica.dbo.RHU_Colaborador c	ON p.pes_id = c.pes_id
			  INNER JOIN GestaoPedagogica.dbo.RHU_ColaboradorCargo cc ON c.col_id = cc.col_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_Escola e		ON e.uad_id = cc.uad_id
			  INNER JOIN GestaoAvaliacao_SGP.dbo.SYS_UnidadeAdministrativa ua ON e.uad_idSuperiorGestao = ua.uad_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_EscolaClassificacao ec ON e.esc_id = ec.esc_id
			  INNER JOIN GestaoPedagogica.dbo.ESC_TipoClassificacaoEscola tce ON ec.tce_id = tce.tce_id
			  LEFT JOIN AcompanhamentoAplicacaoPessoa aap
				ON aap.Edicao = pf.Edicao
				  AND aap.usu_id = pf.usu_id
				  AND coalesce(pf.esc_codigo, '-1') = coalesce(aap.esc_codigo, '-1')
				  AND aap.AtributoID = 83 --QuestionarioAssistenteDiretoriaPreenchido
			WHERE pf.Edicao = @Edicao
			  AND pf.PerfilID = 2 -- Assit. Diretor  E Diretor
			  AND coalesce(pf.esc_codigo, '-1') = coalesce(e.esc_codigo, '-1')
			  AND aap.usu_id IS NULL
			  AND crg_id in (75, 76, 304)-- Assistente de Diretor
			  AND coc_vigenciaInicio <= getDate()
			  AND (coc_vigenciaFim is null OR  coc_vigenciaFim > getDate())
			  AND coc_situacao = 1
			  AND tce.tce_nome IN ('EMEF', 'EMEFM', 'EMEBS', 'CEU EMEF')
			  

			PRINT('Recalcular painéis de acompanhamento')
			PRINT(CONVERT( VARCHAR(24), GETDATE(), 121))			 
			/*
			  Inclusão da lista de profissionais que ainda não responderam os questionários
			*/
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID = 5
			INSERT INTO AcompanhamentoAplicacaoEscola
			select aap.Edicao,
				aap.esc_codigo,
				5 AtributoID, --NumeroDeQuestionariosDeProfessor_ParaPreencher
				count(1) Valor
			from AcompanhamentoAplicacaoPessoa aap
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 32 --QuestionarioDeProfessorPreenchido
			GROUP BY aap.Edicao,
				aap.esc_codigo
				
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID = 1
			INSERT INTO AcompanhamentoAplicacaoEscola
			select aap.Edicao,
				aap.esc_codigo,
				1 AtributoID, --NumeroDeQuestionariosDeDiretor_ParaPreencher
				count(1) Valor
			from AcompanhamentoAplicacaoPessoa aap
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 30 --QuestionarioDeDiretorPreenchido
			GROUP BY aap.Edicao,
				aap.esc_codigo
				
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID = 81
			INSERT INTO AcompanhamentoAplicacaoEscola
			select aap.Edicao,
				aap.esc_codigo,
				81 AtributoID, --NumeroDeQuestionariosDeAssistenteDiretoria_ParaPreencher
				count(1) Valor
			from AcompanhamentoAplicacaoPessoa aap
			WHERE aap.Edicao = @Edicao
			  AND aap.AtributoID = 83 --QuestionarioAssistenteDiretoriaPreenchido
			GROUP BY aap.Edicao,
				aap.esc_codigo
				
			
			/*
			  Inclusão da quantidade de Alunos que devem responder os questionários
			*/
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID = 87
			INSERT INTO AcompanhamentoAplicacaoEscola
			select pf.Edicao,
				pf.esc_codigo,
				87 AtributoID, --NumeroDeQuestionariosAlunos4AnoAo6Ano_ParaPreencher
				count(1) Valor
			FROM PessoaPerfil pf
				INNER JOIN CoreSSO.dbo.SYS_Usuario u
					ON pf.usu_id = u.usu_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_Aluno a
					ON a.pes_id = u.pes_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m
					ON a.alu_id = m.alu_id
			WHERE pf.Edicao = @Edicao AND
				m.mtu_situacao = 1 AND
				m.crp_id BETWEEN 3 AND 6
			GROUP BY pf.Edicao,
				pf.esc_codigo
							
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID = 90
			INSERT INTO AcompanhamentoAplicacaoEscola
			select pf.Edicao,
				pf.esc_codigo,
				90 AtributoID, --NumeroDeQuestionariosAlunos7AnoAo9Ano_ParaPreencher
				count(1) Valor
			FROM PessoaPerfil pf
				INNER JOIN CoreSSO.dbo.SYS_Usuario u
					ON pf.usu_id = u.usu_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.ACA_Aluno a
					ON a.pes_id = u.pes_id
				INNER JOIN GestaoAvaliacao_SGP.dbo.MTR_MatriculaTurma m
					ON a.alu_id = m.alu_id
			WHERE pf.Edicao = @Edicao AND
				m.mtu_situacao = 1 AND
				m.crp_id BETWEEN 7 AND 9
			GROUP BY pf.Edicao,
				pf.esc_codigo
				
			DELETE FROM AcompanhamentoAplicacaoEscola WHERE Edicao = @Edicao AND AtributoID IN (88,91,6,2,82);
			INSERT INTO AcompanhamentoAplicacaoEscola
			SELECT @Edicao edicao,
				qu.esc_codigo,
				CASE qu.QuestionarioID
					WHEN 21 THEN 88-- ALUNO 3 ao 6 ano
					WHEN 22 THEN 91-- ALUNO 7 ao 9 ano
					WHEN 23 THEN 6-- PROFESSOR
					WHEN 24 THEN 2-- DIRETOR
					WHEN 25 THEN 82-- ASSISTENTE DE DIRETOR
				END AtributoID,
				COUNT(1) Valor
			FROM QuestionarioUsuario qu
			WHERE qu.Edicao=@Edicao
			  AND qu.QuestionarioID IN (21,22,23,24,25)--2019
			  AND esc_codigo is not null
			GROUP BY qu.esc_codigo,
				qu.QuestionarioID
			order by qu.QuestionarioID,
				esc_codigo
	  
			COMMIT--ROLLBACK
		END TRY
		BEGIN CATCH
			PRINT (ERROR_MESSAGE());
			ROLLBACK
		END CATCH
		--Fim do cálculo dos painéis

	END --FIM IF questionário habilitado nas configurações
END --FIM function