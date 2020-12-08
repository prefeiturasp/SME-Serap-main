using GestaoAvaliacao.Worker.Database.Contexts.Dapper;
using GestaoAvaliacao.Worker.Domain.Base;
using GestaoAvaliacao.Worker.Domain.Entities.AbsenceReasons;
using GestaoAvaliacao.Worker.Domain.Entities.Schools;
using GestaoAvaliacao.Worker.Domain.Entities.StudentCorrections;
using GestaoAvaliacao.Worker.Domain.Enums;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Tests
{
    public class StudentCorrectionAuxiliarRepository : BaseWorkerDapperRepository, IStudentCorrectionAuxiliarRepository
    {
        public StudentCorrectionAuxiliarRepository(IGestaoAvaliacaoWorkerDapperContext gestaoAvaliacaoWorkerDapperContext)
            : base(gestaoAvaliacaoWorkerDapperContext)
        {
        }

        public async Task<IEnumerable<StudentCorrectionAnswerGridEntityWorker>> GetTestQuestionsAsync(long Id)
        {
            var sql = new StringBuilder("SELECT I.Id AS Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], A.Id, A.[Order], A.Numeration ");
            sql.Append("FROM Item I WITH (NOLOCK) ");
            sql.Append("INNER JOIN BlockItem BI WITH (NOLOCK) ON BI.Item_Id = I.Id ");
            sql.Append("INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id  ");
            sql.Append("INNER JOIN Alternative A (NOLOCK) ON A.Item_Id = I.Id ");
            sql.Append("INNER JOIN Test T WITH (NOLOCK) ON T.Id = B.[Test_Id] ");
            sql.Append("LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state ");
            sql.Append("WHERE B.Test_Id = @id ");
            sql.Append("AND B.State = @state AND BI.State = @state AND I.State = @state ");
            sql.Append("ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order], A.[Order] ");

            var lookup = new Dictionary<long, StudentCorrectionAnswerGridEntityWorker>();

            await _gestaoAvaliacaoWorkerDapperContext.QueryAsync<StudentCorrectionAnswerGridEntityWorker, StudentCorrectionAnswerGridAlternativesWorker, StudentCorrectionAnswerGridEntityWorker>(sql.ToString(),
                (question, alternative) =>
                {
                    StudentCorrectionAnswerGridEntityWorker found;
                    if (!lookup.TryGetValue(question.Item_Id, out found))
                    {
                        question.Alternatives = new List<StudentCorrectionAnswerGridAlternativesWorker>();
                        lookup.Add(question.Item_Id, question);

                        found = question;
                    }
                    found.Alternatives.Add(alternative);
                    return found;
                },
            new { id = Id, state = (Byte)EnumState.ativo });
            return lookup.Values;
        }

        public async Task<IEnumerable<CorrectionStudentGridEntityWorker>> GetByTestSectionAsync(long test_id, long tur_id, IEnumerable<long> aluMongoList, bool ignoreBlocked)
        {
            var sql = new StringBuilder();
            sql.Append("DECLARE @CorrectionStartDate DATE, @CorrectionEndDate DATE, @AllAdhered BIT ");
            sql.AppendLine("SELECT @CorrectionStartDate = CorrectionStartDate,  ");
            sql.AppendLine("@CorrectionEndDate = CorrectionEndDate, ");
            sql.AppendLine("@AllAdhered = AllAdhered ");
            sql.AppendLine("FROM Test WITH (NOLOCK) ");
            sql.AppendLine("WHERE Id = @test_id ");

            sql.AppendLine("SELECT DISTINCT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked ");
            sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
            sql.AppendLine("INNER JOIN SGP_TUR_TurmaTipoCurriculoPeriodo ttcp (NOLOCK) ON mtu.tur_id = ttcp.tur_id ");
            sql.AppendLine("INNER JOIN SGP_ACA_TipoModalidadeEnsino tme (NOLOCK) ON ttcp.tme_id = tme.tme_id ");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
            sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
            sql.AppendLine("LEFT JOIN Adherence Adr WITH(NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
            sql.AppendLine("WHERE mtu.tur_id = @tur_id AND alu.alu_situacao = @state  ");
            sql.AppendLine("AND mtu.mtu_situacao <> @stateExcluido ");
            sql.AppendLine("AND ((tme.tme_nome NOT LIKE '%EJA%' AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate)))) OR tme.tme_nome LIKE '%EJA%')");
            sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
            sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");

            if (aluMongoList != null && aluMongoList.Any())
            {
                sql.AppendLine("UNION ");
                sql.AppendLine("SELECT alu.alu_id, alu.alu_nome, mtu.tur_id, stu.AbsenceReason_Id, mtu.mtu_numeroChamada, Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id, mtu.cur_id, mtu.crr_id, mtu.crp_id, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked  ");
                sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
                sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
                sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = mtu.esc_id ");
                sql.AppendLine("LEFT JOIN StudentTestAbsenceReason stu WITH (NOLOCK) ON stu.alu_id = alu.alu_id AND stu.Test_Id = @test_id AND mtu.tur_id = stu.tur_id ");
                sql.AppendLine("LEFT JOIN Adherence Adr WITH(NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
                sql.AppendLine("WHERE mtu.tur_id = @tur_id  ");
                sql.AppendLine(string.Format("AND alu.alu_id IN ({0}) ", string.Join(",", aluMongoList)));
                sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
                sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");
            }
            sql.AppendLine("ORDER BY mtu.mtu_numeroChamada ");

            var result = await _gestaoAvaliacaoWorkerDapperContext.QueryAsync<CorrectionStudentGridEntityWorker>(sql.ToString(),
                    new
                    {
                        test_id = test_id,
                        tur_id = tur_id,
                        state = (byte)EnumState.ativo,
                        stateExcluido = (byte)EnumState.excluido,
                        TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                        TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                        TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                        TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked
                    });

            if (ignoreBlocked && result.Count() > 0)
            {
                result = result.Where(p => !p.blocked);
            }

            return result;
        }

        public Task<IEnumerable<StudentTestAbsenceReasonEntityWorker>> GetAbsencesByTestSectionAsync(long test_id, long tur_id)
        {
            var sql = new StringBuilder("SELECT s.alu_id, ar.Id, ar.Description ");
            sql.Append("FROM StudentTestAbsenceReason s WITH (NOLOCK) ");
            sql.Append("INNER JOIN AbsenceReason ar WITH (NOLOCK) on s.AbsenceReason_Id = ar.Id ");
            sql.Append("WHERE tur_id = @tur_id AND Test_Id = @test_Id");

            return _gestaoAvaliacaoWorkerDapperContext.QueryAsync<StudentTestAbsenceReasonEntityWorker, AbsenceReasonEntityWorker, StudentTestAbsenceReasonEntityWorker>(sql.ToString(),
                (s, a) =>
                {
                    s.AbsenceReason = a;
                    return s;
                },
                new { tur_id = tur_id, state = (byte)1, test_id = test_id });
        }

        public Task<IEnumerable<DisciplineItemEntityWorker>> GetDisciplineItemByTestIdAsync(long test_id)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT ISNULL(T.Discipline_Id, Em.Discipline_Id) AS Discipline_Id, I.Id AS Item_Id ");
            sql.AppendLine("FROM Test AS T WITH(NOLOCK) ");
            sql.AppendLine("INNER JOIN Block B WITH(NOLOCK) ON T.Id = B.Test_Id AND B.State <> 3 ");
            sql.AppendLine("INNER JOIN BlockItem Bi WITH(NOLOCK) ON B.Id = Bi.Block_Id AND Bi.State <> 3 ");
            sql.AppendLine("INNER JOIN Item I WITH(NOLOCK) ON Bi.Item_Id = I.Id AND I.State <> 3 ");
            sql.AppendLine("INNER JOIN EvaluationMatrix Em WITH(NOLOCK) ON I.EvaluationMatrix_Id = Em.Id AND Em.State <> 3 ");
            sql.AppendLine("WHERE T.Id = @test_Id AND T.State <> 3 ");

            return _gestaoAvaliacaoWorkerDapperContext.QueryAsync<DisciplineItemEntityWorker>(sql.ToString(), new
            {
                test_id = test_id
            });
        }

        public Task<SchoolEntityWorker> GetEscIdDreIdByTeamAsync(long tur_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT Esc.esc_id, Esc.uad_idSuperiorGestao AS dre_id");
            sql.AppendLine("FROM SGP_TUR_Turma Tur WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_ESC_Escola Esc WITH (NOLOCK) ON Esc.esc_id = Tur.esc_id ");
            sql.Append("WHERE tur_id = @tur_id");

            return _gestaoAvaliacaoWorkerDapperContext.QuerySingleAsync<SchoolEntityWorker>(sql.ToString(), new { tur_id = tur_id });
        }

        public Task<IEnumerable<long>> GetRevokedItemsByTestAsync(long test_id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT bi.Item_Id");
            sql.AppendLine("FROM RequestRevoke rr WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN BlockItem bi WITH (NOLOCK) ON rr.BlockItem_Id = bi.Id AND bi.State = @state");
            sql.AppendLine("WHERE rr.Test_id = @Test_Id AND(rr.Situation = @RevokedTest OR rr.Situation = @Revoked) AND rr.State = @state");
            sql.AppendLine("UNION");
            sql.AppendLine("SELECT i.Id");
            sql.AppendLine("FROM Item i WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN BlockItem bi WITH (NOLOCK) ON bi.Item_id = i.Id AND bi.State = @state");
            sql.AppendLine("INNER JOIN Block bl WITH (NOLOCK) ON bl.Id = bi.Block_Id AND bl.State = @state");
            sql.AppendLine("INNER JOIN Booklet bo WITH (NOLOCK) ON bo.Id = bl.Booklet_Id AND bo.State = @state");
            sql.AppendLine("WHERE i.Revoked = 1 AND bo.Test_Id = @Test_Id AND i.State = @state");

            return _gestaoAvaliacaoWorkerDapperContext.QueryAsync<long>(sql.ToString(),
                new
                {
                    state = (byte)EnumState.ativo,
                    Test_Id = test_id,
                    RevokedTest = EnumSituation.RevokedTest,
                    Revoked = EnumSituation.Revoked
                });
        }
    }
}