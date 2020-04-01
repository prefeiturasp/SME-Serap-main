using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using Dapper;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AvaliaMais.FolhaRespostas.Data.SQLServer.Repository
{
    public class ProcessamentoInicialRepository : ConnectionReadOnly, IProcessamentoInicialRepository
	{
		public IEnumerable<Processamento> ObterProcessamentoProvaAdesaoTotal(int provaId)
		{
			string sqlQuery = MontarAdesao(true) +
								MontarFrom() +
								MontarInner() +
								MontarWhere() +
								MontarGroup();

			var parametros = new DynamicParameters();
			parametros.Add("@ativo", (byte)EnumState.ativo);
			parametros.Add("@excluido", (byte)EnumState.excluido);
			parametros.Add("@TypeEntity", (byte)EnumAdherenceEntity.Section);
			parametros.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
            parametros.Add("@TypeEntityStudent", (byte)EnumAdherenceEntity.Student);
            parametros.Add("@TypeSelectionSelected", (byte)EnumAdherenceSelection.Selected);
            parametros.Add("@TypeSelectionNotSelect", (byte)EnumAdherenceSelection.NotSelected);
            parametros.Add("@TypeSelectionBlocked", (byte)EnumAdherenceSelection.Blocked);
            parametros.Add("@TestId", provaId);

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<Processamento>(sqlQuery.ToString(), parametros, commandTimeout: 3600) ?? Enumerable.Empty<Processamento>();
			}
		}

		public IEnumerable<Processamento> ObterProcessamentoProva(int provaId)
		{
			string sqlQuery = MontarAdesao(false) + 
								MontarFrom() +
								MontarInner() +
								MontarWhere() +
								MontarGroup();

			var parametros = new DynamicParameters();
			parametros.Add("@ativo", (byte)EnumState.ativo);
			parametros.Add("@excluido", (byte)EnumState.excluido);
			parametros.Add("@TypeEntity", (byte)EnumAdherenceEntity.Section);
			parametros.Add("@TypeSelection", (byte)EnumAdherenceSelection.NotSelected);
            parametros.Add("@TypeEntityStudent", (byte)EnumAdherenceEntity.Student);
            parametros.Add("@TypeSelectionSelected", (byte)EnumAdherenceSelection.Selected);
            parametros.Add("@TypeSelectionNotSelect", (byte)EnumAdherenceSelection.NotSelected);
            parametros.Add("@TypeSelectionBlocked", (byte)EnumAdherenceSelection.Blocked);
            parametros.Add("@TestId", provaId);

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<Processamento>(sqlQuery.ToString(), parametros, commandTimeout: 3600) ?? Enumerable.Empty<Processamento>();
			}
		}

		private string MontarAdesao(bool total)
		{
			return @";WITH ProcessamentoProva AS (
							SELECT ua.uad_id AS DreId
								,ua.uad_sigla + ' - ' + ua.uad_nome AS DreNome
								,esc.esc_id AS EscolaId 
								,esc.uad_id AS EscolaUad
								,esc.esc_nome AS EscolaNome
								,tur.tur_id AS TurmaId
								,tur.tur_codigo + ' - ' + tn.ttn_nome AS TurmaNome 
							FROM SGP_TUR_Turma tur WITH (NOLOCK) 
							INNER JOIN SGP_ACA_TipoTurno tn WITH (NOLOCK) ON tn.ttn_id = tur.ttn_id AND tn.ttn_situacao = @ativo
							INNER JOIN SGP_ESC_Escola esc WITH (NOLOCK) ON esc.esc_id = tur.esc_id AND esc.esc_situacao = @ativo
							INNER JOIN SGP_SYS_UnidadeAdministrativa ua WITH (NOLOCK) ON ua.uad_id = esc.uad_idSuperiorGestao AND ua.uad_situacao = @ativo
							INNER JOIN Test tes WITH (NOLOCK) ON tes.Id = @TestId AND tes.[State] = @ativo " +
					(total ? 
					@"INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp WITH (NOLOCK) ON tur.tur_id = ttcp.tur_id AND ttcp.ttcr_situacao = @ativo
						INNER JOIN SGP_ACA_TipoCurriculoPeriodo tcg WITH (NOLOCK) ON tcg.tcp_ordem = ttcp.crp_ordem AND tcg.tme_id = ttcp.tme_id AND tcg.tne_id = ttcp.tne_id AND tcg.tcp_situacao = @ativo
						INNER JOIN SGP_ACA_Curso cur WITH (NOLOCK) ON cur.cur_id = ttcp.cur_id AND cur.cur_situacao = @ativo
						INNER JOIN SGP_ACA_CurriculoPeriodo crp WITH (NOLOCK) ON crp.cur_id = cur.cur_id AND crp.crp_ordem = ttcp.crp_ordem AND crp.tcp_id = tcg.tcp_id AND crp.crp_situacao = @ativo
						INNER JOIN TestTypeCourse ttc WITH (NOLOCK) ON ttc.CourseId = ttcp.cur_id AND ttc.TestType_Id = tes.TestType_Id AND ttc.[State] = @ativo 
						INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) ON tcc.TypeCurriculumGradeId = tcg.tcp_id AND tcc.[State] = @ativo AND tcc.Test_Id = tes.Id 
						LEFT JOIN Adherence a WITH (NOLOCK) ON a.Test_Id = tes.Id AND a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.TypeSelection = @TypeSelection AND a.[State] = @ativo "
					: @"INNER JOIN Adherence a WITH (NOLOCK) ON a.Test_Id = tes.Id AND a.EntityId = tur.tur_id AND a.TypeEntity = @TypeEntity AND a.TypeSelection != @TypeSelection AND a.[State] = @ativo ") +
					@"WHERE tur.tur_situacao = @ativo " +
					(total ? 
					"AND a.Id IS NULL ) " 
					: ") ");
		}

		private string MontarFrom()
		{
			return @"SELECT 
						tes.Id AS ProvaId
						,tes.Description AS ProvaNome 
						,P.DreId
						,P.EscolaId 
						,P.EscolaUad
						,P.TurmaId 
						,P.DreNome
						,P.EscolaNome 
						,P.TurmaNome 
						,SUM(COUNT(alu.alu_id)) OVER(PARTITION BY P.DreId) AS QtdeAlunosDre
						,SUM(COUNT(alu.alu_id)) OVER(PARTITION BY P.EscolaId) AS QtdeAlunosEscola
						,SUM(COUNT(alu.alu_id)) OVER(PARTITION BY P.TurmaId) AS QtdeAlunosTurma 
					FROM ProcessamentoProva P WITH (NOLOCK)  ";
		}

		private string MontarInner()
		{
			return "INNER JOIN Test tes WITH (NOLOCK) ON tes.Id = @TestId AND tes.[State] = @ativo  " +
					"INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON mtu.tur_id = P.TurmaId AND mtu.mtu_situacao <> @excluido  " +
					"INNER JOIN SGP_ACA_Aluno alu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id AND alu.alu_situacao = @ativo ";
		}

		private string MontarWhere()
		{
            return "WHERE tes.Id = @TestId " +
                    "AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= tes.CorrectionEndDate " +
                    "AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= tes.CorrectionStartDate))) " +
                    "AND ((tes.AllAdhered = 1 AND NOT EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND (Adr.TypeSelection = @TypeSelectionNotSelect OR Adr.TypeSelection = @TypeSelectionBlocked) AND Adr.Test_Id = @TestId)) " +
                    "OR (tes.AllAdhered = 0 AND EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND Adr.TypeSelection = @TypeSelectionSelected AND Adr.Test_Id = @TestId))) ";
        }

        private string MontarGroup()
		{
			return "GROUP BY tes.Id, tes.description, P.DreId, P.EscolaId, P.EscolaUad, P.TurmaId, P.DreNome, P.EscolaNome, P.TurmaNome ";
		}

		public IEnumerable<Aluno> ObterAlunosPorTurmaProva(string TurmaIds, int ProvaId)
		{
			var sql = @"DECLARE @turmaId BIGINT
						DECLARE @resultados AS TABLE (
								AlunoId BIGINT, Nome VARCHAR(200) collate Latin1_General_CI_AS, TurmaId BIGINT, DataMatricula DATE, DataSaida DATE, Numero INT,
								ProvaId BIGINT, Situacao TINYINT, Ausente INT
						)

						DECLARE cursor_objects CURSOR FOR
						SELECT Items FROM dbo.SplitString(@turmaIds, ',')

						OPEN cursor_objects
						FETCH NEXT FROM cursor_objects INTO @turmaId

						WHILE @@FETCH_STATUS = 0
						BEGIN

							DECLARE @CorrectionEndDate DATE, @CorrectionStartDate DATE, @AllAdhered BIT;
							DECLARE @AlunosBatchFiles AS TABLE (
								AlunoId BIGINT, Nome VARCHAR(200) COLLATE Latin1_General_CI_AS, TurmaId BIGINT, DataMatricula DATE, DataSaida DATE, Numero INT,
								ProvaId BIGINT, Situacao TINYINT, Ausente INT
							)
							SELECT @CorrectionEndDate = CorrectionEndDate, @CorrectionStartDate = CorrectionStartDate, @AllAdhered = AllAdhered FROM Test WHERE Id = @provaId
							;WITH BatchFiles AS (
								SELECT [Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id],[Sent],[CreateDate],[UpdateDate],[State]
								,[Section_Id],[SupAdmUnit_Id],[School_Id],[Situation],[CreatedBy_Id],[AnswerSheetBatchQueue_Id] 
								,ROW_NUMBER() OVER(PARTITION BY AnswerSheetBatch_Id, SupAdmUnit_Id, School_Id, Section_Id, Student_Id 
								ORDER BY UpdateDate DESC) AS RowNumber 
								FROM AnswerSheetBatchFiles WITH (NOLOCK)
								WHERE AnswerSheetBatch_Id IS NOT NULL AND Student_Id IS NOT NULL 
								AND Section_Id = @turmaId 

							),
							BatchFilesDistinct AS (
								SELECT 
								[Id],[File_Id],[AnswerSheetBatch_Id],[Student_Id],[Sent],[CreateDate],[UpdateDate],[State]
								,[Section_Id],[SupAdmUnit_Id],[School_Id],[Situation],[CreatedBy_Id],[AnswerSheetBatchQueue_Id] 
								FROM BatchFiles WITH (NOLOCK)
								WHERE RowNumber = 1
							)

							INSERT INTO @AlunosBatchFiles
								SELECT 
								DISTINCT alu.alu_id as AlunoId,alu.alu_nome as Nome,mtu.tur_id as TurmaId,mtu.mtu_dataMatricula as DataMatricula
								,mtu.mtu_dataSaida as DataSaida, mtu.mtu_numeroChamada as Numero,ab.test_Id as ProvaId,bf.Situation as Situacao
								,CASE WHEN abr.alu_id IS NULL THEN 0 ELSE 1 END AS Ausente
								FROM SGP_ACA_Aluno alu WITH (NOLOCK)
								INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON mtu.alu_id = alu.alu_id and mtu.mtu_situacao <> @excluido 
								LEFT JOIN BatchFilesDistinct bf WITH (NOLOCK) ON bf.Section_Id = mtu.tur_id and bf.student_id = alu.alu_id
								LEFT JOIN AnswerSheetBatch ab WITH (NOLOCK) ON bf.AnswerSheetBatch_Id = ab.Id 
								LEFT JOIN StudentTestAbsenceReason abr WITH (NOLOCK) ON alu.alu_id = abr.alu_id and ab.Test_Id = abr.Test_Id and bf.Section_Id = abr.tur_id
								WHERE 
								mtu.tur_id = @turmaId 
								AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate 
								AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate ))) 
								AND alu.alu_situacao = @ativo
								AND ab.Test_Id = @provaId
                                AND ((@AllAdhered = 1 AND NOT EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND (Adr.TypeSelection = @TypeSelectionNotSelect OR Adr.TypeSelection = @TypeSelectionBlocked) AND Adr.Test_Id = @provaId)) 
                                    OR (@AllAdhered = 0 AND EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND Adr.TypeSelection = @TypeSelectionSelected AND Adr.Test_Id = @provaId))) 


            ; WITH AlunoMatricula AS (
								SELECT 
								DISTINCT alu.alu_id as AlunoId,alu.alu_nome as Nome,mtu.tur_id as TurmaId,mtu.mtu_dataMatricula as DataMatricula
								,mtu.mtu_dataSaida as DataSaida, mtu.mtu_numeroChamada as Numero
								FROM SGP_ACA_Aluno alu WITH (NOLOCK)
								INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON mtu.alu_id = alu.alu_id and mtu.mtu_situacao <> @excluido
								WHERE 
								mtu.tur_id = @turmaId 
								AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate))
								AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate )
								AND alu.alu_situacao = @ativo
                                AND ((@AllAdhered = 1 AND NOT EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND (Adr.TypeSelection = @TypeSelectionNotSelect OR Adr.TypeSelection = @TypeSelectionBlocked) AND Adr.Test_Id = @provaId)) 
                                    OR (@AllAdhered = 0 AND EXISTS (SELECT Adr.Id FROM Adherence Adr WITH (NOLOCK) WHERE Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @ativo AND Adr.TypeSelection = @TypeSelectionSelected AND Adr.Test_Id = @provaId))) 
							),
							AlunosTurma AS (
								SELECT AlunoId, Nome, TurmaId, DataMatricula, DataSaida, Numero, ProvaId, Situacao, Ausente
								FROM @AlunosBatchFiles
						
								UNION 

								SELECT AlunoId, Nome, TurmaId, DataMatricula, DataSaida, Numero,NULL, 1 as Situacao, 0 AS Ausente 
								FROM AlunoMatricula 
								WHERE NOT EXISTS (
									SELECT AlunoId 
									FROM @AlunosBatchFiles A
									WHERE A.AlunoId = AlunoMatricula.AlunoId
								)
							)
	
							INSERT INTO @resultados
							SELECT 
							AlunoId,Nome,TurmaId,DataMatricula,DataSaida,Numero,ProvaId = @provaId,
							CASE 
								WHEN Ausente = 1 THEN NULL 
								WHEN Situacao IS NULL THEN 1 
								ELSE Situacao 
							END AS Situacao,
							Ausente
							FROM AlunosTurma
							ORDER BY Nome
	
							DELETE FROM @AlunosBatchFiles

							FETCH NEXT FROM cursor_objects INTO @turmaId
						END

						CLOSE cursor_objects
						DEALLOCATE cursor_objects 

						SELECT AlunoId, Nome, TurmaId, DataMatricula, DataSaida, Numero, ProvaId, Situacao, Ausente
						FROM @resultados";
			
			using (IDbConnection cn = Connection)
			{
				cn.Open();

				return cn.Query<Aluno>(sql.ToString(), 
					new { turmaIds = TurmaIds, 
					provaId = ProvaId, excluido = EnumState.excluido,
					ativo = EnumState.ativo,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked
            }, commandTimeout: 3600) ?? Enumerable.Empty<Aluno>();
			}
		}

		public IEnumerable<int> ObterTurmasDoUsuario(Guid userId)
		{
			var sql = new StringBuilder("SELECT DISTINCT (tud.tur_id) FROM SGP_TUR_TurmaDisciplina tud WITH (NOLOCK)");
			sql.AppendLine("INNER JOIN [Synonym_Core_SYS_Grupo] G WITH (NOLOCK) ON  G.gru_situacao = 1");
			sql.AppendLine("INNER JOIN [Synonym_Core_SYS_Usuario] UG WITH (NOLOCK) ON UG.usu_id = @usu_id");
			sql.AppendLine("INNER JOIN SGP_TUR_TurmaDocente tdt WITH (NOLOCK) ON tdt.tud_id = tud.tud_id ");
			sql.AppendLine("INNER JOIN SGP_ACA_Docente d WITH (NOLOCK) ON d.doc_id = tdt.doc_id ");
			sql.AppendLine("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND d.doc_situacao = @state AND d.pes_id = UG.pes_id");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<int>(sql.ToString(), new { usu_id = userId, state = EnumState.ativo }, commandTimeout: 3600) ?? Enumerable.Empty<int>();
			}
		}
	}
}
