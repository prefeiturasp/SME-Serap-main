using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class TestRepository : ConnectionReadOnly, ITestRepository
    {
        #region Read

        private const string CACHE_KEY_GETOBJECT = "TestRepository_GetObject_{0}";

        public Task<bool> AnyAsync(long testId)
        {
            using (var ctx = new GestaoAvaliacaoContext())
            {
                return ctx.Test.AnyAsync(x => x.Id == testId);
            }
        }

        public Test GetObject(long Id)
        {
            string key = string.Format(CACHE_KEY_GETOBJECT, Id);
            Test result = Cache.GetFromCache<Test>(key);
            if (result == null)
            {

                var sql = new StringBuilder("SELECT Id, Description, Bib, NumberItemsBlock, NumberBlock, NumberItem, ApplicationStartDate, ApplicationEndDate, ");
                sql.Append("CorrectionStartDate, CorrectionEndDate, UsuId, FrequencyApplication, TestSituation, CreateDate, UpdateDate, State, Discipline_Id, ");
                sql.Append("FormatType_Id, TestType_Id, AllAdhered, ProcessedCorrectionDate, KnowledgeAreaBlock, Multidiscipline, ShowVideoFiles, ShowAudioFiles ");
                sql.Append("FROM Test WITH (NOLOCK) ");
                sql.Append("WHERE Id = @id");

                using (IDbConnection cn = Connection)
                {
                    cn.Open();

                    result = cn.Query<Test>(sql.ToString(), new { id = Id, }).FirstOrDefault();
                }

                Cache.SetInCache(key, result, 6);
            }

            return result;
        }       

        private void LimparCache_GetObject(long Id)
        {
            string key = string.Format(CACHE_KEY_GETOBJECT, Id);
            Cache.ClearCache(key);
        }

        public IEnumerable<Test> GetInCorrection()
        {
            var sql = new StringBuilder("SELECT Id, Description, TestType_Id, Discipline_Id, Bib, NumberItemsBlock, NumberBlock, FormatType_Id, NumberItem, ApplicationStartDate,");
            sql.AppendLine("ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, UsuId, FrequencyApplication, TestSituation, AllAdhered, CreateDate, UpdateDate, State, KnowledgeAreaBlock ");
            sql.AppendLine("FROM Test WITH (NOLOCK)");
            sql.AppendLine("WHERE State = @state AND @data BETWEEN ApplicationStartDate AND CorrectionEndDate");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var retorno = cn.Query<Test>(sql.ToString(), new { state = (byte)EnumState.ativo, data = DateTime.Today });

                return retorno;
            }

        }

        public Test GetObjectWithTestType(long Id)
        {
            var sql = new StringBuilder("SELECT t.Id, t.Description, t.Bib, t.NumberItemsBlock, t.NumberBlock, t.NumberItem, t.ApplicationStartDate, t.ApplicationEndDate, ");
            sql.AppendLine("t.CorrectionStartDate, t.CorrectionEndDate, t.UsuId, t.FrequencyApplication, t.TestSituation, t.CreateDate, t.UpdateDate, t.State, t.Discipline_Id, ");
            sql.AppendLine("t.FormatType_Id, t.TestType_Id, t.AllAdhered, t.KnowledgeAreaBlock, t.ElectronicTest, tt.Id, tt.Global, tt.FrequencyApplication, tt.Description, tt.ModelTest_Id, tt.EntityId ");
            sql.AppendLine("FROM Test t WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON tt.Id = t.TestType_Id ");
            sql.AppendLine("WHERE t.Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var retorno = cn.Query<Test, TestType, Test>(sql.ToString(),
                    (t, tt) =>
                    {
                        t.TestType = tt;
                        return t;
                    }, new { id = Id, }).FirstOrDefault();

                return retorno;
            }
        }

        public Test GetObjectWithTestTypeItemType(long Id)
        {
            var sql = new StringBuilder("SELECT t.Id, t.Description, t.Bib, t.NumberItemsBlock, t.NumberBlock, t.NumberItem, t.ApplicationStartDate, t.ApplicationEndDate,");
            sql.AppendLine("t.CorrectionStartDate, t.CorrectionEndDate, t.UsuId, t.FrequencyApplication, t.TestSituation, t.CreateDate, t.UpdateDate, t.State, t.Discipline_Id,");
            sql.AppendLine("t.FormatType_Id, t.TestType_Id, t.AllAdhered, t.KnowledgeAreaBlock, tt.Id, tt.Global, tt.FrequencyApplication, tt.Description, tt.ModelTest_Id, tt.EntityId, tt.ItemType_Id, it.Id, it.QuantityAlternative");
            sql.AppendLine("FROM Test t WITH (NOLOCK)");
            sql.AppendLine("INNER JOIN TestType tt WITH (NOLOCK) ON tt.Id = t.TestType_Id");
            sql.AppendLine("LEFT JOIN ItemType it WITH (NOLOCK) ON tt.ItemType_Id = it.Id");
            sql.AppendLine("WHERE t.Id = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var retorno = cn.Query<Test, TestType, ItemType, Test>(sql.ToString(),
                    (t, tt, it) =>
                    {
                        t.TestType = tt;
                        tt.ItemType = it;
                        return t;
                    }, new { id = Id, }).FirstOrDefault();

                return retorno;
            }
        }

        public AdherenceTest GetObjectToAdherence(long Id)
        {
            var sql = @"SELECT t.Id, t.Description AS TestDescription, t.FrequencyApplication AS TestFrequencyApplication, t.AllAdhered, t.KnowledgeAreaBlock, 
							   CASE 
									WHEN d.Description IS NULL AND t.Multidiscipline = 1 THEN 'Multidisciplinar'
									ELSE d.Description
							   END AS DisciplineDescription, t.UsuId, tt.Global, t.ApplicationEndDate, t.ApplicationStartDate, 
							   CASE WHEN(it.Id IS NULL) THEN 1 ELSE CASE WHEN(it.QuantityAlternative IS NULL) THEN 1 ELSE 0 END END AS AnswerSheetBlocked,  
							   tt.FrequencyApplication AS TestTypeFrequencyApplication
						FROM Test t WITH (NOLOCK) 
						LEFT JOIN Discipline d WITH (NOLOCK) ON t.Discipline_Id = d.Id 
						INNER JOIN TestType tt WITH (NOLOCK) ON t.TestType_Id = tt.Id 
						LEFT JOIN ItemType it WITH (NOLOCK) ON it.Id = tt.ItemType_Id 
						WHERE t.Id = @id";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(), new { id = Id, typeEntitySchool = (byte)EnumAdherenceEntity.School, typeEntitySection = (byte)EnumAdherenceEntity.School });
                var retorno = query.Read<AdherenceTest>().FirstOrDefault();

                return retorno;
            }
        }

        public Test GetTestBy_Id(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Test.AsNoTracking()
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public Test GetTestById(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Test.AsNoTracking()
                        .Include("Discipline")
                        .Include("TestCurriculumGrades")
                        .Include("TestPerformanceLevels")
                        .Include("TestItemLevels")
                        .Include("TestItemLevels.ItemLevel")
                        .Include("TestPerformanceLevels.PerformanceLevel")
                        .Include("TestType")
                        .Include("FormatType")
                        .Include("TestSubGroup")
                        .Include("TestTime")
                        .Include("TestContexts")
                        .Include("BlockChains")
                        .Include("Blocks")
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public List<Test> GetByTestType(long testTypeId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Test.AsNoTracking()
                        .Include("TestType")
                        .Where(i => i.TestType.Id == testTypeId && i.State != (Byte)EnumState.excluido).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<TestResult> _SearchTests(TestFilter filter, ref Pager pager)
        {
            #region Params
            var p = new DynamicParameters();
            p.Add("@TestId", filter.TestId);
            p.Add("@TestType", filter.TestType);
            p.Add("@DisciplineId", filter.DisciplineId);
            p.Add("@testFrequencyApplication", filter.FrequencyApplication);

            if (!string.IsNullOrEmpty(filter.CreationDateStart))
                p.Add("@CreationDateStart", filter.CreationDateStart);

            if (!string.IsNullOrEmpty(filter.CreationDateEnd))
                p.Add("@CreationDateEnd", filter.CreationDateEnd);

            if (filter.Pendente)
                p.Add("@Pendente", filter.Pendente);

            if (filter.Cadastrada)
                p.Add("@Cadastrada", filter.Cadastrada);

            if (filter.Andamento)
                p.Add("@Andamento", filter.Andamento);

            if (filter.Aplicada)
                p.Add("@Aplicada", filter.Aplicada);


            p.Add("@pageSize", pager.PageSize);
            p.Add("@pageNumber", pager.CurrentPage);
            p.Add("@typeEntity", (byte)EnumAdherenceEntity.Section);
            p.Add("@global", filter.global);
            p.Add("@visible", filter.visibleTest);
            p.Add("@multidiscipline", filter.visibleMultidiscipline);

            if (!string.IsNullOrEmpty(filter.dre_id))
                p.Add("@uad_id", filter.dre_id);

            if (filter.esc_id > 0)
                p.Add("@esc_id", filter.esc_id);

            if (filter.ttn_id > 0)
                p.Add("@ttn_id", filter.ttn_id);

            if (!string.IsNullOrEmpty(filter.tne_id_ordem))
            {
                string[] valors = filter.tne_id_ordem.Split('_');
                p.Add("@tne_id", int.Parse(valors[0]));
                p.Add("@crp_ordem", int.Parse(valors[1]));
            }

            p.Add("@getGroup", filter.getGroup);
            p.Add("@TestGroupId", filter.TestGroupId);
            p.Add("@TestSubGroupId", filter.TestSubGroupId);
            p.Add("@ordenacao", filter.ordenacao);

            #endregion
            using (IDbConnection cn = Connection)
            {
                var query = cn.QueryMultiple("MS_Test_SearchFiltered", param: p, commandType: CommandType.StoredProcedure);
                var ret = query.Read<TestResult>();
                var count = query.Read<int>().FirstOrDefault();


                pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(count) / (double)pager.PageSize));
                pager.SetTotalItens(Convert.ToInt32(count));

                return ret;
            }
        }

        public List<AnswerSheetBatchItems> GetTestAnswers(long Id)
        {
            StringBuilder sql = new StringBuilder(@"SELECT DISTINCT A.[Item_Id], A.Id, A.[Correct], A.[Numeration], (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], I.[ItemCode], I.KnowledgeArea_Id, K.[Description] AS KnowledgeArea_Description ");

            sql.Append("FROM [Block] B WITH (NOLOCK) ");
            sql.Append("INNER JOIN [BlockItem] BI WITH (NOLOCK) ON BI.[Block_Id] = B.[Id] ");
            sql.Append("INNER JOIN [Item] I WITH (NOLOCK) ON I.[Id] = BI.[Item_Id] ");
            sql.Append("INNER JOIN [Alternative] A WITH (NOLOCK) ON A.[Item_Id] = I.[Id] ");
            sql.Append("INNER JOIN Test T WITH (NOLOCK) ON T.Id = B.[Test_Id] ");
            sql.Append("LEFT JOIN KnowledgeArea K WITH(NOLOCK) ON I.KnowledgeArea_Id = K.Id AND K.State <> @State ");
            sql.Append("LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State <> @state ");
            sql.Append("WHERE B.[State] <> @State AND BI.[State] <> @State AND I.[State] <> @State AND A.[State] <> @State ");
            sql.Append("AND B.[Test_Id] = @TestId ");
            sql.Append("ORDER BY [Order] ASC ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<AnswerSheetBatchItems>(sql.ToString(), new { State = (Byte)EnumState.excluido, TestId = Id }).ToList();

                return result;
            }
        }

        public KeyValuePair<long, long> GetTestItem(long Id, int ItemOrder, int AlternativeOrder)
        {
            StringBuilder sql = new StringBuilder(@"SELECT DISTINCT BI.[Item_Id], A.[Id] ");

            sql.Append("FROM [Block] B WITH (NOLOCK) ");
            sql.Append("INNER JOIN [BlockItem] BI WITH (NOLOCK) ON BI.[Block_Id] = B.[Id] ");
            sql.Append("INNER JOIN [Alternative] A WITH (NOLOCK) ON A.[Item_Id] = BI.[Item_Id] ");
            sql.Append("WHERE B.[State] <> @State AND BI.[State] <> @State AND A.[State] <> @State ");
            sql.Append("AND B.[Test_Id] = @TestId AND A.[Order] = @AlternativeOrder AND BI.[Order] = @ItemOrder ");
            sql.Append("ORDER BY BI.[Item_Id] ASC ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query(sql.ToString(), new { State = (Byte)EnumState.excluido, TestId = Id, AlternativeOrder = AlternativeOrder, ItemOrder = ItemOrder }).Select(i => new KeyValuePair<long, long>(i.Item_Id, i.Id)).FirstOrDefault();

                return result;
            }
        }

        public IEnumerable<AnswerSheetStudentInformation> GetTeamStudents(int SchoolId, long SectionId, long StudentId, long test_id, bool allAdhered)
        {
            StringBuilder sql = new StringBuilder(@"SELECT DISTINCT A.[alu_id] AS Id, A.[alu_nome] AS Name, E.[esc_nome] AS SchoolName, M.[tur_id] AS SectionId, T.[tur_codigo] + ' - ' + TN.[ttn_nome] AS SectionName ");
            sql.Append(", M.[mtu_numeroChamada] AS NumberId ");
            sql.Append("FROM [SGP_ACA_Aluno] A WITH (NOLOCK) ");
            sql.Append("INNER JOIN [SGP_MTR_MatriculaTurma] M WITH (NOLOCK) ON M.[alu_id] = A.[alu_id] ");
            sql.Append("INNER JOIN [SGP_ESC_Escola] E WITH (NOLOCK) ON E.[esc_id] = M.[esc_id] AND E.[esc_situacao] = 1 ");
            sql.Append("INNER JOIN [SGP_TUR_Turma] T WITH (NOLOCK) ON T.[tur_id] = M.[tur_id] AND T.[tur_situacao] = 1 ");
            sql.Append("INNER JOIN [SGP_ACA_TipoTurno] TN WITH (NOLOCK) ON TN.[ttn_id] = T.[ttn_id] AND TN.[ttn_situacao] = 1 ");

            sql.Append("WHERE A.[alu_situacao] = 1 AND M.[mtu_situacao] = 1 ");
            sql.Append("AND M.[esc_id] = @SchoolId ");
            if (SectionId > 0)
                sql.Append("AND M.[tur_id] = @SectionId ");
            if (StudentId > 0)
                sql.Append("AND A.[alu_id] = @StudentId ");
            if (allAdhered)
            {
                sql.AppendLine("AND NOT EXISTS (SELECT Adr.Id FROM Adherence Adr WITH(NOLOCK) WHERE Adr.EntityId = A.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND (Adr.TypeSelection = @TypeSelectionNotSelect OR Adr.TypeSelection = @TypeSelectionBlocked) AND Adr.Test_Id = @test_id) ");
            }
            else
            {
                sql.AppendLine("AND EXISTS (SELECT Adr.Id FROM Adherence Adr WITH(NOLOCK) WHERE Adr.EntityId = A.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.TypeSelection = @TypeSelectionSelected AND Adr.Test_Id = @test_id) ");
            }

            sql.Append("ORDER BY M.[mtu_numeroChamada] ASC ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<AnswerSheetStudentInformation>(sql.ToString(),
                    new
                    {
                        SectionId = SectionId,
                        SchoolId = SchoolId,
                        StudentId = StudentId,
                        state = EnumState.ativo,
                        test_id = test_id,
                        TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                        TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                        TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                        TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked
                    }, commandTimeout: 0);

                return result;
            }
        }

        public IEnumerable<Test> TestByUser(TestFilter filter)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT t.Id, t.Description");
            sql.AppendLine("FROM Test t");
            sql.AppendLine("INNER JOIN TestType tt ON tt.Id = t.TestType_Id");

            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                sql.AppendLine("INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem)");
                sql.AppendLine("teacher ON t.Id = teacher.Id");
            }
            sql.AppendLine("WHERE t.State = @state AND tt.State = @state ");
            sql.AppendLine("AND t.Id = ISNULL(@TestId, t.Id)");
            sql.AppendLine("AND t.TestType_Id = ISNULL(@TestType, t.TestType_Id)");
            sql.AppendLine("AND t.Discipline_Id = ISNULL(@DisciplineId, t.Discipline_Id)");
            sql.AppendLine("AND(@CreationDateStart IS NULL AND @CreationDateEnd IS NULL");
            sql.AppendLine("OR(@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NULL AND CAST(t.CreateDate AS Date) >= CAST(@CreationDateStart AS Date))");
            sql.AppendLine("OR(@CreationDateStart IS NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date) <= CAST(@CreationDateEnd AS Date))");
            sql.AppendLine("OR(@CreationDateStart IS NOT NULL AND @CreationDateEnd IS NOT NULL AND CAST(t.CreateDate AS Date)  BETWEEN CAST(@CreationDateStart AS Date) AND CAST(@CreationDateEnd AS Date))");
            sql.AppendLine(")");
            sql.AppendLine("AND(@Pendente IS NULL AND @Cadastrada IS NULL AND @Andamento IS NULL AND @Aplicada IS NULL");
            sql.AppendLine("OR(@Pendente IS NOT NULL AND t.TestSituation = 1)");
            sql.AppendLine("OR(@Cadastrada IS NOT NULL AND(t.TestSituation = 2 AND CAST(GETDATE() AS Date) < CAST(t.ApplicationStartDate AS Date)))");
            sql.AppendLine("OR(@Andamento IS NOT NULL AND(t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN CAST(t.ApplicationStartDate AS Date) AND CAST(t.ApplicationEndDate AS Date)))");
            sql.AppendLine("OR(@Aplicada IS NOT NULL AND(t.TestSituation = 2 AND CAST(GETDATE() AS Date) > CAST(t.ApplicationEndDate AS Date)))");
            sql.AppendLine(")");
            sql.AppendLine("AND tt.Global = @global");
            sql.AppendLine("AND ((@applicationStartDate IS NULL AND @applicationEndDate IS NULL) OR  (t.ApplicationStartDate >= @applicationStartDate) AND(t.ApplicationEndDate <= @applicationEndDate))");
            sql.AppendLine("ORDER BY t.Description");

            #region Params
            var p = new DynamicParameters();
            p.Add("@TestId", filter.TestId);
            p.Add("@TestType", filter.TestType);
            p.Add("@DisciplineId", filter.DisciplineId);

            p.Add("@CreationDateStart", string.IsNullOrEmpty(filter.CreationDateStart) ? null : filter.CreationDateStart);

            p.Add("@CreationDateEnd", string.IsNullOrEmpty(filter.CreationDateEnd) ? null : filter.CreationDateEnd);

            if (filter.Pendente)
                p.Add("@Pendente", filter.Pendente);
            else
                p.Add("@Pendente");

            if (filter.Cadastrada)
                p.Add("@Cadastrada", filter.Cadastrada);
            else
                p.Add("@Cadastrada");

            if (filter.Andamento)
                p.Add("@Andamento", filter.Andamento);
            else
                p.Add("@Andamento");

            if (filter.Aplicada)
                p.Add("@Aplicada", filter.Aplicada);
            else
                p.Add("@Aplicada");

            p.Add("@ent_id", filter.ent_id);
            p.Add("@pes_id", filter.pes_id);
            p.Add("@usuId", filter.usuId);
            p.Add("@state", (byte)EnumState.ativo);
            p.Add("@typeEntity", (byte)EnumAdherenceEntity.Section);
            p.Add("@typeSelected", (byte)EnumAdherenceSelection.Selected);
            p.Add("@typeNotSelected", (byte)EnumAdherenceSelection.NotSelected);
            p.Add("@gru_id", filter.gru_id);
            p.Add("@vis_id", filter.vis_id);
            p.Add("@global", filter.global);

            p.Add("@uad_id", string.IsNullOrEmpty(filter.dre_id) ? null : filter.dre_id);

            if (filter.esc_id > 0)
                p.Add("@esc_id", filter.esc_id);
            else
                p.Add("@esc_id");

            if (filter.ttn_id > 0)
                p.Add("@ttn_id", filter.ttn_id);
            else
                p.Add("@ttn_id");

            if (!string.IsNullOrEmpty(filter.tne_id_ordem))
            {
                string[] valors = filter.tne_id_ordem.Split('_');
                p.Add("@tne_id", int.Parse(valors[0]));
                p.Add("@crp_ordem", int.Parse(valors[1]));
            }
            else
            {
                p.Add("@tne_id");
                p.Add("@crp_ordem");
            }

            if (filter.ApplicationStartDate.HasValue)
            {
                p.Add("@applicationStartDate", filter.ApplicationStartDate.Value);
                p.Add("@applicationEndDate", filter.ApplicationEndDate.Value);
            }
            else
            {
                p.Add("@applicationStartDate");
                p.Add("@applicationEndDate");
            }
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<Test>(sql.ToString(), p);

                return result;
            }
        }

        public IEnumerable<TestResult> _SearchTestsUser(TestFilter filter, ref Pager pager)
        {
            #region Params
            var p = new DynamicParameters();
            p.Add("@TestId", filter.TestId);
            p.Add("@TestType", filter.TestType);
            p.Add("@DisciplineId", filter.DisciplineId);
            p.Add("@testFrequencyApplication", filter.FrequencyApplication);

            if (!string.IsNullOrEmpty(filter.CreationDateStart))
                p.Add("@CreationDateStart", filter.CreationDateStart);

            if (!string.IsNullOrEmpty(filter.CreationDateEnd))
                p.Add("@CreationDateEnd", filter.CreationDateEnd);

            if (filter.Pendente)
                p.Add("@Pendente", filter.Pendente);

            if (filter.Cadastrada)
                p.Add("@Cadastrada", filter.Cadastrada);

            if (filter.Andamento)
                p.Add("@Andamento", filter.Andamento);

            if (filter.Aplicada)
                p.Add("@Aplicada", filter.Aplicada);


            p.Add("@pageSize", pager.PageSize);
            p.Add("@pageNumber", pager.CurrentPage);
            p.Add("@ent_id", filter.ent_id);
            p.Add("@pes_id", filter.pes_id);
            p.Add("@usuId", filter.usuId);
            p.Add("@state", (byte)EnumState.ativo);
            p.Add("@typeEntity", (byte)EnumAdherenceEntity.Section);
            p.Add("@typeSelected", (byte)EnumAdherenceSelection.Selected);
            p.Add("@typeNotSelected", (byte)EnumAdherenceSelection.NotSelected);
            p.Add("@gru_id", filter.gru_id);
            p.Add("@vis_id", filter.vis_id);
            p.Add("@global", filter.global);
            p.Add("@multidiscipline", filter.visibleMultidiscipline);

            p.Add("@getGroup", filter.getGroup);
            p.Add("@TestGroupId", filter.TestGroupId);
            p.Add("@TestSubGroupId", filter.TestSubGroupId);
            p.Add("@ordenacao", filter.ordenacao);

            if (!string.IsNullOrEmpty(filter.dre_id))
                p.Add("@uad_id", filter.dre_id);

            if (filter.esc_id > 0)
                p.Add("@esc_id", filter.esc_id);

            if (filter.ttn_id > 0)
                p.Add("@ttn_id", filter.ttn_id);

            if (!string.IsNullOrEmpty(filter.tne_id_ordem))
            {
                string[] valors = filter.tne_id_ordem.Split('_');
                p.Add("@tne_id", int.Parse(valors[0]));
                p.Add("@crp_ordem", int.Parse(valors[1]));
            }

            #endregion
            using (IDbConnection cn = Connection)
            {
                var query = cn.QueryMultiple("MS_Test_SearchFilteredUser", param: p, commandType: CommandType.StoredProcedure);
                var ret = query.Read<TestResult>();
                var count = query.Read<int>().FirstOrDefault();


                pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(count) / (double)pager.PageSize));
                pager.SetTotalItens(Convert.ToInt32(count));

                return ret;
            }
        }

        public IEnumerable<TestResult> GetTestByDate(TestFilter filter)
        {
            string sql = @"SELECT 
							t.Id AS TestId,
							t.UsuId AS UsuId,
							CAST(t.Id AS VARCHAR) + ' - ' + t.Description AS TestDescription,
							CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
							t.CreateDate AS TestCreateDate,
							t.FrequencyApplication,
							CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
							CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
							CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
							CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
							t.Bib,
							t.TestSubGroup_Id,
							(CASE
								WHEN t.TestSituation = 1 THEN 1
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
								END) AS TestSituation
						FROM Test t WITH(NOLOCK) ";
            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                sql += @"INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) teacher ON t.Id = teacher.Id ";
            }
            sql += @"WHERE t.State = @state ";
            if (filter.ApplicationStartDate != null)
            {
                sql += @"AND CAST(t.ApplicationStartDate AS Date) >= CAST(@ApplicationStartDate AS Date) ";
            }
            if (filter.CorrectionEndDate != null)
            {
                sql += @"AND CAST(t.CorrectionEndDate AS Date) <= CAST(@CorrectionEndDate AS Date) ";
            }
            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                sql += @"AND t.Visible = 1";
            }

            sql += "ORDER BY t.Id ASC";

            #region Params
            var p = new DynamicParameters();
            if (filter.ApplicationStartDate != null)
                p.Add("@ApplicationStartDate", filter.ApplicationStartDate);

            if (filter.CorrectionEndDate != null)
                p.Add("@CorrectionEndDate", filter.CorrectionEndDate);

            p.Add("@state", (byte)EnumState.ativo);

            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                p.Add("@ent_id", filter.ent_id);
                p.Add("@pes_id", filter.pes_id);
                p.Add("@usuId", filter.usuId);

                p.Add("@typeEntity", (byte)EnumAdherenceEntity.Section);
                p.Add("@typeSelected", (byte)EnumAdherenceSelection.Selected);
                p.Add("@typeNotSelected", (byte)EnumAdherenceSelection.NotSelected);
                p.Add("@gru_id", filter.gru_id);
                p.Add("@vis_id", filter.vis_id);

                if (!string.IsNullOrEmpty(filter.dre_id))
                    p.Add("@uad_id", filter.dre_id);
                else
                    p.Add("@uad_id", null);

                if (filter.esc_id > 0)
                    p.Add("@esc_id", filter.esc_id);
                else
                    p.Add("@esc_id", null);

                if (filter.ttn_id > 0)
                    p.Add("@ttn_id", filter.ttn_id);
                else
                    p.Add("@ttn_id", null);

                if (!string.IsNullOrEmpty(filter.tne_id_ordem))
                {
                    string[] valors = filter.tne_id_ordem.Split('_');
                    p.Add("@tne_id", int.Parse(valors[0]));
                    p.Add("@crp_ordem", int.Parse(valors[1]));
                }
                else
                {
                    p.Add("@tne_id", null);
                    p.Add("@crp_ordem", null);
                }
            }

            #endregion
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = cn.Query<TestResult>
                    (
                        sql.ToString(),
                        p
                    );

                return result;
            }
        }

        public IEnumerable<TestResult> GetTestByDateWithGroup(TestFilter filter)
        {
            string sql = @"SELECT 
							t.Id AS TestId,
							t.UsuId AS UsuId,
							CAST(t.Id AS VARCHAR) + ' - ' + t.Description AS TestDescription,
							CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
							t.CreateDate AS TestCreateDate,
							t.FrequencyApplication,
							CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
							CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
							CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
							CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
							t.Bib,
							t.TestSubGroup_Id,
							(CASE
								WHEN t.TestSituation = 1 THEN 1
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
								END) AS TestSituation,
							tg.Id, tg.Description
						FROM Test t WITH(NOLOCK) ";
            sql += @"INNER JOIN TestSubGroup tsg WITH(NOLOCK) ON t.TestSubGroup_Id = tsg.Id 
					 INNER JOIN TestGroup tg WITH(NOLOCK) ON tsg.TestGroup_Id = tg.Id ";
            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                sql += @"INNER JOIN TestsByUser(@usuId, @pes_id, @ent_id, @state, @typeEntity, @typeSelected, @typeNotSelected, @gru_id, @vis_id, @uad_id, @esc_id, @ttn_id, @tne_id, @crp_ordem) teacher ON t.Id = teacher.Id ";
            }
            sql += @"WHERE t.State = @state ";
            if (filter.ApplicationStartDate != null)
            {
                sql += @"AND CAST(t.ApplicationStartDate AS Date) >= CAST(@ApplicationStartDate AS Date) ";
            }
            if (filter.CorrectionEndDate != null)
            {
                sql += @"AND CAST(t.CorrectionEndDate AS Date) <= CAST(@CorrectionEndDate AS Date) ";
            }
            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                sql += @"AND t.Visible = 1";
            }

            sql += "ORDER BY t.Id ASC";

            #region Params
            var p = new DynamicParameters();
            if (filter.ApplicationStartDate != null)
                p.Add("@ApplicationStartDate", filter.ApplicationStartDate);

            if (filter.CorrectionEndDate != null)
                p.Add("@CorrectionEndDate", filter.CorrectionEndDate);

            p.Add("@state", (byte)EnumState.ativo);

            if (filter.vis_id != EnumSYS_Visao.Administracao)
            {
                p.Add("@ent_id", filter.ent_id);
                p.Add("@pes_id", filter.pes_id);
                p.Add("@usuId", filter.usuId);

                p.Add("@typeEntity", (byte)EnumAdherenceEntity.Section);
                p.Add("@typeSelected", (byte)EnumAdherenceSelection.Selected);
                p.Add("@typeNotSelected", (byte)EnumAdherenceSelection.NotSelected);
                p.Add("@gru_id", filter.gru_id);
                p.Add("@vis_id", filter.vis_id);

                if (!string.IsNullOrEmpty(filter.dre_id))
                    p.Add("@uad_id", filter.dre_id);
                else
                    p.Add("@uad_id", null);

                if (filter.esc_id > 0)
                    p.Add("@esc_id", filter.esc_id);
                else
                    p.Add("@esc_id", null);

                if (filter.ttn_id > 0)
                    p.Add("@ttn_id", filter.ttn_id);
                else
                    p.Add("@ttn_id", null);

                if (!string.IsNullOrEmpty(filter.tne_id_ordem))
                {
                    string[] valors = filter.tne_id_ordem.Split('_');
                    p.Add("@tne_id", int.Parse(valors[0]));
                    p.Add("@crp_ordem", int.Parse(valors[1]));
                }
                else
                {
                    p.Add("@tne_id", null);
                    p.Add("@crp_ordem", null);
                }
            }

            #endregion
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = cn.Query<TestResult, TestGroup, TestResult>
                    (
                        sql.ToString(),
                        (t, tg) =>
                        {
                            t.TestGroup = tg;
                            return t;
                        },
                        p
                    );

                return result;
            }
        }

        public IEnumerable<TestItemLevel> GetTestItems(long Id)
        {
            StringBuilder sql = new StringBuilder(@"SELECT [Id]
															,[Value]
															,[PercentValue]
															,[CreateDate]
															,[UpdateDate]
															,[State]
															,[ItemLevel_Id]
															,[Test_Id]
													FROM [TestItemLevel] WITH (NOLOCK) ");
            sql.Append("WHERE [Test_Id] = @TestId AND [State] <> @State ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<TestItemLevel>(sql.ToString(), new { State = (Byte)EnumState.excluido, TestId = Id });

                return result;
            }
        }

        public IEnumerable<AnswerSheetBatch> GetTestAutomaticCorrectionSituation(long testId, long schoolId)
        {
            var sql = new StringBuilder("SELECT [Id], [Processing] ");
            sql.Append("FROM [AnswerSheetBatch] WITH (NOLOCK) ");
            sql.Append("WHERE ");

            if (schoolId <= 0)
            {
                sql.AppendFormat("[Test_Id] = @Test_Id AND [OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.Test);
            }
            else
            {
                sql.AppendFormat("[Test_Id] = @Test_Id AND [School_Id] = @School_Id AND [OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.School);
                sql.Append("UNION ");
                sql.Append("SELECT DISTINCT B.[Id], B.[Processing] ");
                sql.Append("FROM [AnswerSheetBatch] B WITH (NOLOCK) ");
                sql.Append("INNER JOIN [AnswerSheetBatchFiles] BF ON BF.[AnswerSheetBatch_Id] = B.[Id] ");
                sql.Append("INNER JOIN [SGP_TUR_Turma] T ON T.[tur_id] = BF.[Section_Id] ");
                sql.AppendFormat("WHERE [Test_Id] = @Test_Id AND [OwnerEntity] = {0} ", (byte)EnumAnswerSheetBatchOwner.Test);
                sql.Append("AND T.[esc_id] = @School_Id ");
            }

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var retorno = cn.Query<AnswerSheetBatch>(sql.ToString(), new { Test_Id = testId, School_Id = schoolId });

                return retorno;
            }
        }

        public IEnumerable<Test> GetByTypeCurriculumGrade(int typeCurriculumGrade)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT t.Id, t.Description");
            sql.AppendLine("FROM Test t");
            sql.AppendLine("INNER JOIN TestCurriculumGrade ttcg ON t.Id = ttcg.Test_Id AND ttcg.State = @state");
            sql.AppendLine("WHERE t.State = @state AND ttcg.TypeCurriculumGradeId = @typeCurriculumGrade");
            sql.AppendLine("ORDER BY t.Description");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Test>(sql.ToString(), new { state = (byte)EnumState.ativo, typeCurriculumGrade = typeCurriculumGrade });
            }
        }

        public IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id)
        {
            var sql = new StringBuilder();
            sql.Append("DECLARE @CorrectionStartDate DATE, @CorrectionEndDate DATE, @AllAdhered BIT ");
            sql.AppendLine("SELECT @CorrectionStartDate = CorrectionStartDate,  ");
            sql.AppendLine("@CorrectionEndDate = CorrectionEndDate, ");
            sql.AppendLine("@AllAdhered = AllAdhered ");
            sql.AppendLine("FROM Test WITH(NOLOCK) ");
            sql.AppendLine("WHERE Id = @test_id ");

            sql.AppendLine("SELECT alu.alu_id, alu.alu_nome, mtu.tur_id, mtu.mtu_numeroChamada, CASE WHEN (Adr.TypeSelection = @TypeSelectionBlocked) THEN 1 ELSE 0 END AS blocked");
            sql.AppendLine(", f.[Id] AS FileId,f.[Name] AS FileName,f.[OriginalName] AS FileOriginalName,f.[Path] AS FilePath ");
            sql.AppendLine("FROM SGP_ACA_Aluno alu WITH (NOLOCK) ");
            sql.AppendLine("INNER JOIN SGP_MTR_MatriculaTurma mtu WITH (NOLOCK) ON alu.alu_id = mtu.alu_id ");
            sql.AppendLine("LEFT JOIN [File] f WITH (NOLOCK) ON f.[OwnerId] = alu.alu_id AND f.[ParentOwnerId] = @test_id AND f.[State] = @state AND f.OwnerType = @ownerType ");
            sql.AppendLine("LEFT JOIN Adherence Adr WITH(NOLOCK) ON Adr.EntityId = alu.alu_id AND Adr.TypeEntity = @TypeEntityStudent AND Adr.State = @state AND Adr.Test_Id = @test_id ");
            sql.AppendLine("WHERE mtu.tur_id = @tur_id AND alu.alu_situacao = @state  ");
            sql.AppendLine("AND mtu.mtu_situacao <> @stateExcluido ");
            sql.AppendLine("AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= @CorrectionEndDate AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= @CorrectionStartDate ) ))");
            sql.AppendLine("AND ((@AllAdhered = 1 AND ISNULL(Adr.TypeSelection, 0) <> @TypeSelectionNotSelect) ");
            sql.AppendLine("OR (@AllAdhered = 0 AND (Adr.TypeSelection = @TypeSelectionSelected OR Adr.TypeSelection = @TypeSelectionBlocked))) ");
            sql.AppendLine("ORDER BY mtu.mtu_numeroChamada ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<CorrectionStudentGrid>(sql.ToString(), new
                {
                    tur_id = tur_id,
                    state = EnumState.ativo,
                    test_id = test_id,
                    stateExcluido = EnumState.excluido,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelected = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNotSelect = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBlocked = (byte)EnumAdherenceSelection.Blocked,
                    ownerType = EnumFileType.AnswerSheetQRCode
                });

                return result;
            }
        }

        public IEnumerable<Test> GetTestFinishedCorrection(bool allTests)
        {
            var sql = new StringBuilder();
            if (allTests)
            {
                sql.AppendLine("SELECT");
            }
            else
            {
                sql.AppendLine("SELECT TOP 1");
            }
            sql.AppendLine("[Id], [CorrectionStartDate], [CorrectionEndDate], [AllAdhered]");
            sql.AppendLine("FROM [Test] WITH (NOLOCK)");
            sql.AppendLine("WHERE [State] = @state AND [ProcessedCorrection] = 0");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Test>(sql.ToString(),
                    new
                    {
                        state = (byte)EnumState.ativo
                    });
            }
        }

        public IEnumerable<DisciplineItem> GetDisciplineItemByTestId(long test_id)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT ISNULL(T.Discipline_Id, Em.Discipline_Id) AS Discipline_Id, I.Id AS Item_Id ");
            sql.AppendLine("FROM Test AS T WITH(NOLOCK) ");
            sql.AppendLine("INNER JOIN Block B WITH(NOLOCK) ON T.Id = B.Test_Id AND B.State <> 3 ");
            sql.AppendLine("INNER JOIN BlockItem Bi WITH(NOLOCK) ON B.Id = Bi.Block_Id AND Bi.State <> 3 ");
            sql.AppendLine("INNER JOIN Item I WITH(NOLOCK) ON Bi.Item_Id = I.Id AND I.State <> 3 ");
            sql.AppendLine("INNER JOIN EvaluationMatrix Em WITH(NOLOCK) ON I.EvaluationMatrix_Id = Em.Id AND Em.State <> 3 ");
            sql.AppendLine("WHERE T.Id = @test_Id AND T.State <> 3 ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<DisciplineItem>(sql.ToString(), new
                {
                    test_id = test_id
                });

                return result;
            }
        }

        public IEnumerable<TestResult> GetTestsBySubGroup(long Id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT t.Id AS TestId,
							t.UsuId AS UsuId,
							CAST(t.Id AS VARCHAR) + ' - ' + t.Description AS TestDescription,
							CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
							t.FrequencyApplication,
							CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
							CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
							CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
							CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
							(CASE
								WHEN t.TestSituation = 1 THEN 1
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
								END) AS TestSituation  " +
                           "FROM Test t WITH(NOLOCK) " +
                           "INNER JOIN TestSubGroup sg WITH(NOLOCK)" +
                           "ON t.TestSubGroup_Id = sg.Id AND sg.[State] <> @state " +
                           "WHERE sg.Id = @id " +
                           "AND t.State != @state ";

                var Test = cn.Query<TestResult>(sql, new { id = Id, state = (Byte)EnumState.excluido });

                return Test;
            }
        }

        public IEnumerable<TestResult> GetTestsBySubGroupTcpId(long Id, long tcp_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT t.Id AS TestId,
							t.UsuId AS UsuId,
							CAST(t.Id AS VARCHAR) + ' - ' + t.Description AS TestDescription,
							CONVERT(VARCHAR(50), t.CreateDate, 103) AS CreateDate,
							t.FrequencyApplication,
							CONVERT(VARCHAR(50), t.ApplicationStartDate, 103) AS ApplicationStartDate,
							CONVERT(VARCHAR(50), t.ApplicationEndDate, 103) AS ApplicationEndDate,
							CONVERT(VARCHAR(50), t.CorrectionStartDate, 103) AS CorrectionStartDate,
							CONVERT(VARCHAR(50), t.CorrectionEndDate, 103) AS CorrectionEndDate,
                            t.AllAdhered AS AllAdhered,
							(CASE
								WHEN t.TestSituation = 1 THEN 1
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) < t.ApplicationStartDate THEN 2 
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) BETWEEN t.ApplicationStartDate AND t.ApplicationEndDate THEN 3
								WHEN t.TestSituation = 2 AND CAST(GETDATE() AS Date) > t.ApplicationEndDate THEN 4
								END) AS TestSituation  " +
                           "FROM Test t WITH(NOLOCK) " +
                           "INNER JOIN TestSubGroup sg WITH(NOLOCK)" +
                           "ON t.TestSubGroup_Id = sg.Id AND sg.[State] <> @state " +
                           "INNER JOIN TestCurriculumGrade tcg WITH(NOLOCK)" +
                           "ON t.Id = tcg.Test_Id AND tcg.[State] <> @state " +
                           "WHERE sg.Id = @id " +
                           "AND tcg.TypeCurriculumGradeId = @tcpId " +
                           "AND t.State != @state ";

                var Test = cn.Query<TestResult>(sql, new { id = Id, state = (Byte)EnumState.excluido, tcpId = tcp_id });

                return Test;
            }
        }

        public async Task<List<ElectronicTestDTO>> SearchEletronicTests()
        {
            var sql = new StringBuilder(@"SELECT
	                                                    T.Id,
	                                                    T.[Description] + ' - ' + CASE 
								                                                    WHEN Discipline_Id IS NULL THEN 'Multidisciplinar'
								                                                    ELSE D.[Description]
							                                                      END 
	                                                    AS Description,
	                                                    NumberItem,
	                                                    CASE WHEN DATEDIFF(dd, GETDATE(), ApplicationEndDate) >= 0 THEN (DATEDIFF(dd, GETDATE(), ApplicationEndDate) + 1)
		                                                     ELSE 0 
	                                                    END AS quantDiasRestantes, 
	                                                    TT.FrequencyApplication,
	                                                    T.ApplicationEndDate
                                                    FROM
	                                                    Test AS T WITH(NOLOCK)
	                                                    INNER JOIN TestType AS TT WITH(NOLOCK)
		                                                    ON TT.Id = T.TestType_Id
	                                                    LEFT JOIN Discipline AS D WITH(NOLOCK)
		                                                    ON D.Id = T.Discipline_Id	
                                                    WHERE 
	                                                    ElectronicTest = 1
	                                                    AND T.[State] <> @State");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = await cn.QueryAsync<ElectronicTestDTO>(sql.ToString(), new { State = (Byte)EnumState.excluido });
                return result.ToList();
            }
        }

        private readonly string MatriculaTemp = @"
                            SELECT 
	                mtu.mtu_dataMatricula, 
	                mtu.mtu_dataSaida, 
	                mtu.cur_id, 
	                mtu.crr_id, 
	                mtu.crp_id,
	                mtu.alu_id,
	                mtu.tur_id,
	                mtu.esc_id
	                INTO #Matricula
                FROM SGP_MTR_MatriculaTurma mtu WITH(NOLOCK) 
                INNER JOIN SGP_ACA_Aluno AS alu WITH(NOLOCK) ON alu.alu_id = mtu.alu_id
                WHERE  
	                alu.pes_id = @PesId ";

        public async Task<ElectronicTestDTO> GetElectronicTestByPesIdAndTestId(Guid pes_id, long testId)
        {
            var sql = new StringBuilder($@"
                {MatriculaTemp}

                SELECT 
                    T.Id,
                    T.[Description] + ' - ' + CASE 
                                                WHEN Discipline_Id IS NULL THEN 'Multidisciplinar'
                                                ELSE D.[Description]
                                                END 
                    AS Description,
                    TT.FrequencyApplication,
                    YEAR(T.ApplicationEndDate) AnoDeAplicacaoDaProva,
	                mtu.alu_id,
	                mtu.tur_id,
	                mtu.esc_id,
                    esc.uad_idSuperiorGestao as dre_id,
	                esc.esc_nome,
                    tur.tur_codigo  as Turma,
	                d.Description as Disciplina,
                    T.ShowJustificate
                FROM Test AS T WITH(NOLOCK)
                INNER JOIN TestType AS TT WITH(NOLOCK)
	                ON TT.Id = T.TestType_Id
                INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) 
	                ON T.Id = tcc.Test_Id AND tcc.[State] = 1
                INNER JOIN SGP_ACA_CurriculoPeriodo AS Crp WITH(NOLOCK)
		                ON tcc.TypeCurriculumGradeId = Crp.tcp_id
		                AND Crp.crp_situacao <> 3	
                INNER JOIN #Matricula mtu ON
	                mtu.cur_id = Crp.cur_id
	                AND mtu.crr_id = Crp.crr_id
	                AND mtu.crp_id = Crp.crp_id
                    AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= CAST(T.CorrectionEndDate AS DATE) 
	                AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= CAST(T.ApplicationStartDate AS DATE)))) 
                LEFT JOIN Discipline AS D WITH(NOLOCK)
                    ON D.Id = T.Discipline_Id
                LEFT JOIN Adherence AS A WITH(NOLOCK)
	                ON T.Id = A.Test_Id
	                AND A.EntityId = mtu.alu_id 	
                    AND A.TypeEntity = @TypeEntityStudent
                    AND A.[State] = 1
                INNER JOIN SGP_ESC_ESCOLA AS Esc WITH(NOLOCK)
	                ON esc.esc_id =mtu.esc_id
                INNER JOIN SGP_TUR_TURMA AS tur WITH(NOLOCK)
	                ON tur.tur_id = mtu.tur_id
                WHERE 
	                t.Id = @TestId
                    AND ElectronicTest = 1
                    AND T.[State] <> 3
	                AND (T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado,@TypeSelectionBloqueado)
		                OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado))
                ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = await cn.QueryAsync<ElectronicTestDTO>(sql.ToString(), new
                {
                    TestId = testId,
                    State = (Byte)EnumState.excluido,
                    PesId = pes_id,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelecionado = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNaoSelecionado = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBloqueado = (byte)EnumAdherenceSelection.Blocked
                });

                return result.FirstOrDefault();
            }
        }

        public async Task<List<ElectronicTestDTO>> GetTestsByPesId(Guid pes_id)
        {
            var sql = new StringBuilder($@"
                {MatriculaTemp}

               SELECT 
                    T.Id,
                    T.[Description] + ' - ' + CASE 
                                                WHEN Discipline_Id IS NULL THEN 'Multidisciplinar'
                                                ELSE D.[Description]
                                                END 
                    AS Description,
                    YEAR(T.ApplicationEndDate) AnoDeAplicacaoDaProva,
                    mtu.tur_id,
                    mtu.esc_id,
                    esc.uad_idSuperiorGestao as dre_id,
                    TT.TargetToStudentsWithDeficiencies
                FROM Test AS T WITH(NOLOCK)
                INNER JOIN TestType AS TT WITH(NOLOCK)
	                ON TT.Id = T.TestType_Id
                INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) 
	                ON T.Id = tcc.Test_Id AND tcc.[State] = 1
                INNER JOIN SGP_ACA_CurriculoPeriodo AS Crp WITH(NOLOCK)
		                ON tcc.TypeCurriculumGradeId = Crp.tcp_id
		                AND Crp.crp_situacao <> 3	
                INNER JOIN #Matricula mtu ON
	                mtu.cur_id = Crp.cur_id
	                AND mtu.crr_id = Crp.crr_id
	                AND mtu.crp_id = Crp.crp_id
                    AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= CAST(T.CorrectionEndDate AS DATE) 
	                AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= CAST(T.ApplicationStartDate AS DATE)))) 
                LEFT JOIN Discipline AS D WITH(NOLOCK)
                    ON D.Id = T.Discipline_Id
                LEFT JOIN Adherence AS A WITH(NOLOCK)
	                ON T.Id = A.Test_Id
	                AND A.EntityId = mtu.alu_id 	
                    AND A.TypeEntity = @TypeEntityStudent
                    AND A.[State] = 1
                INNER JOIN SGP_ESC_ESCOLA AS Esc WITH(NOLOCK)
	                ON esc.esc_id =mtu.esc_id
                INNER JOIN SGP_TUR_TURMA AS tur WITH(NOLOCK)
	                ON tur.tur_id = mtu.tur_id
                WHERE 
		                ElectronicTest = 1
                    AND T.[State] <> 3
	                AND (T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado,@TypeSelectionBloqueado)
		                OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado))
                ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = await cn.QueryAsync<ElectronicTestDTO>(sql.ToString(), new
                {
                    State = (Byte)EnumState.excluido,
                    PesId = pes_id,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelecionado = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNaoSelecionado = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBloqueado = (byte)EnumAdherenceSelection.Blocked
                });

                return result.ToList();
            }
        }


        public async Task<List<ElectronicTestDTO>> SearchEletronicTestsByPesId(Guid pes_id)
        {
            var sql = new StringBuilder($@"
                {MatriculaTemp}

                SELECT 
                    T.Id,
                    T.[Description] + ' - ' + CASE 
                                                WHEN Discipline_Id IS NULL THEN 'Multidisciplinar'
                                                ELSE D.[Description]
                                                END 
                    AS Description,
                    NumberItem,
                    CASE WHEN DATEDIFF(dd, GETDATE(), ApplicationEndDate) >= 0 THEN (DATEDIFF(dd, GETDATE(), ApplicationEndDate) + 1)
                            ELSE 0 
                    END AS quantDiasRestantes, 
                    TT.FrequencyApplication,
                    T.ApplicationEndDate,
	                t.AllAdhered,
	                mtu.alu_id,
	                mtu.tur_id,
                    mtu.esc_id,
                    esc.uad_idSuperiorGestao as dre_id,
                    TT.Id AS TestTypeId,
                    TT.TargetToStudentsWithDeficiencies
                FROM Test AS T WITH(NOLOCK)
                INNER JOIN TestType AS TT WITH(NOLOCK)
	                ON TT.Id = T.TestType_Id
                INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) 
	                ON T.Id = tcc.Test_Id AND tcc.[State] = 1
                INNER JOIN SGP_ACA_CurriculoPeriodo AS Crp WITH(NOLOCK)
		                ON tcc.TypeCurriculumGradeId = Crp.tcp_id
		                AND Crp.crp_situacao <> 3	
                INNER JOIN #Matricula mtu ON
	                mtu.cur_id = Crp.cur_id
	                AND mtu.crr_id = Crp.crr_id
	                AND mtu.crp_id = Crp.crp_id
                    AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= CAST(T.CorrectionEndDate AS DATE) 
	                AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= CAST(T.ApplicationStartDate AS DATE)))) 
                LEFT JOIN Discipline AS D WITH(NOLOCK)
                    ON D.Id = T.Discipline_Id
                LEFT JOIN Adherence AS A WITH(NOLOCK)
	                ON T.Id = A.Test_Id
	                AND A.EntityId = mtu.alu_id 	
                    AND A.TypeEntity = @TypeEntityStudent
                    AND A.[State] = 1
                INNER JOIN SGP_ESC_ESCOLA AS Esc WITH(NOLOCK)
	                ON esc.esc_id = mtu.esc_id
                WHERE 
                    ElectronicTest = 1
                    AND T.[State] <> 3
	                AND (T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado,@TypeSelectionBloqueado)
		                OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado))
                GROUP BY
                    T.Id,
                    T.[Description],
	                T.Discipline_Id,
	                D.[Description],
	                T.NumberItem,
                    TT.FrequencyApplication,
                    T.ApplicationEndDate,
	                t.AllAdhered,
	                mtu.alu_id,
	                mtu.tur_id,
                    mtu.esc_id,
					esc.uad_idSuperiorGestao,
                    TT.Id,
                    TT.TargetToStudentsWithDeficiencies
                ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = await cn.QueryAsync<ElectronicTestDTO>(sql.ToString(), new
                {
                    State = (Byte)EnumState.excluido,
                    PesId = pes_id,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelecionado = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNaoSelecionado = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBloqueado = (byte)EnumAdherenceSelection.Blocked
                });

                return result.ToList();
            }
        }

        public async Task<IEnumerable<Guid>> GetStudentDeficiencies(Guid pes_id)
        {
            var query = @"SELECT
                            tde_id
                        FROM
                            PES_PessoaDeficiencia (NOLOCK)
                        WHERE
                            pes_id = @pes_id";

            using (IDbConnection cn = ConnectionCoreSSO)
            {
                cn.Open();
                return await cn.QueryAsync<Guid>(query, new { pes_id });
            }
        }

        public bool ExistsAdherenceByAluIdTestId(long alu_id, long test_id)
        {
            StringBuilder sql = new StringBuilder(@"SELECT
                                                        T.Id
                                                    FROM
                                                        Test AS T WITH(NOLOCK)
                                                        INNER JOIN TestType AS TT WITH(NOLOCK)
                                                            ON TT.Id = T.TestType_Id
	                                                    INNER JOIN TestCurriculumGrade tcc WITH (NOLOCK) 
		                                                    ON T.Id = tcc.Test_Id
		                                                    AND tcc.[State] = 1
	                                                    INNER JOIN SGP_ACA_CurriculoPeriodo AS Crp WITH(NOLOCK)
		                                                    ON tcc.TypeCurriculumGradeId = Crp.tcp_id
		                                                    AND Crp.crp_situacao <> 3
                                                        INNER JOIN SGP_MTR_MatriculaTurma mtu WITH(NOLOCK) 
                                                            ON mtu.cur_id = Crp.cur_id
		                                                    AND mtu.crr_id = Crp.crr_id
		                                                    AND mtu.crp_id = Crp.crp_id
                                                            AND (mtu.mtu_dataMatricula IS NULL OR (mtu.mtu_dataMatricula <= CAST(T.CorrectionEndDate AS DATE) 
		                                                    AND (mtu.mtu_dataSaida IS NULL OR mtu.mtu_dataSaida >= CAST(T.ApplicationStartDate AS DATE)))) 
	                                                    INNER JOIN SGP_ACA_Aluno AS ALU WITH(NOLOCK)
		                                                    ON ALU.alu_id = mtu.alu_id
                                                        LEFT JOIN Discipline AS D WITH(NOLOCK)
                                                            ON D.Id = T.Discipline_Id
	                                                    LEFT JOIN Adherence AS A WITH(NOLOCK)
		                                                    ON T.Id = A.Test_Id
		                                                    AND ALU.alu_id = A.EntityId	
                                                            AND A.TypeEntity = @TypeEntityStudent
                                                            AND A.[State] = 1
                                                    WHERE 
                                                        ElectronicTest = 1
                                                        AND T.[State] <> 3
	                                                    AND (T.AllAdhered = 1 AND ISNULL(A.TypeSelection, 0) NOT IN (@TypeSelectionNaoSelecionado,@TypeSelectionBloqueado)
		                                                    OR (T.AllAdhered = 0 AND TypeSelection = @TypeSelectionSelecionado))
	                                                    AND ALU.alu_id = @AluId
                                                        AND T.Id = @TestId");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var result = cn.Query<ElectronicTestDTO>(sql.ToString(), new
                {
                    State = (Byte)EnumState.excluido,
                    AluId = alu_id,
                    TestId = test_id,
                    TypeEntityStudent = (byte)EnumAdherenceEntity.Student,
                    TypeSelectionSelecionado = (byte)EnumAdherenceSelection.Selected,
                    TypeSelectionNaoSelecionado = (byte)EnumAdherenceSelection.NotSelected,
                    TypeSelectionBloqueado = (byte)EnumAdherenceSelection.Blocked
                });

                return result.ToList().Count > 0;
            }
        }

        public async Task<Test> SearchInfoTestAsync(long test_id)
        {
            var sql = @"SELECT
	                    T.Id,
	                    T.[Description] + ' - ' + CASE 
								                    WHEN Discipline_Id IS NULL THEN 'Multidisciplinar'
								                    ELSE D.[Description]
							                        END 
	                    AS Description,
	                    NumberItem,
	                    CASE WHEN DATEDIFF(dd, GETDATE(), ApplicationEndDate) >= 0 THEN (DATEDIFF(dd, GETDATE(), ApplicationEndDate) + 1)
                                ELSE 0 
                        END AS quantDiasRestantes, 
	                    TT.FrequencyApplication,
	                    T.ApplicationEndDate,
                        T.ShowVideoFiles,
                        T.ShowAudioFiles,
                        T.ShowJustificate
                    FROM
	                    Test AS T WITH(NOLOCK)
	                    INNER JOIN TestType AS TT WITH(NOLOCK)
		                    ON TT.Id = T.TestType_Id
	                    LEFT JOIN Discipline AS D WITH(NOLOCK)
		                    ON D.Id = T.Discipline_Id	
                    WHERE 
	                    T.[State] <> @State
                        AND T.Id = @TestId";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = await cn.QueryAsync<Test>(sql, new { State = (Byte)EnumState.excluido, TestId = test_id });
                return result.FirstOrDefault();
            }
        }

        #endregion

        #region Write

        public Test Save(Test entity, Guid usu_id)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                long maxOrder = GetMaxOrderTests();
                entity.Order = maxOrder + 1;

                if (entity.Discipline != null)
                    entity.Discipline = GestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == entity.Discipline.Id);

                entity.TestType = GestaoAvaliacaoContext.TestType.FirstOrDefault(s => s.Id == entity.TestType.Id);
                entity.TestTime = GestaoAvaliacaoContext.TestTime.FirstOrDefault(s => s.Id == entity.TestTime.Id);


                if (entity.FormatType != null)
                    entity.FormatType = GestaoAvaliacaoContext.FormatType.FirstOrDefault(s => s.Id == entity.FormatType.Id);

                if (entity.TestSubGroup != null)
                    entity.TestSubGroup = GestaoAvaliacaoContext.TestSubGroup.FirstOrDefault(s => s.Id == entity.TestSubGroup.Id);

                entity.TestSituation = EnumTestSituation.Pending;

                foreach (var t in entity.TestPerformanceLevels)
                {
                    t.PerformanceLevel = GestaoAvaliacaoContext.PerformanceLevel.FirstOrDefault(f => f.Id == t.PerformanceLevel.Id);
                }

                foreach (var t in entity.TestItemLevels)
                {
                    t.ItemLevel = GestaoAvaliacaoContext.ItemLevel.FirstOrDefault(f => f.Id == t.ItemLevel.Id);
                }

                entity.UsuId = usu_id;

                entity.State = Convert.ToByte(EnumState.ativo);

                GestaoAvaliacaoContext.Test.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(entity.Id);

                return entity;
            }
        }

        public Test Update(Test entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(entity.Id);

                return entity;
            }
        }

        public Test Update(long Id, Test entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var dateNow = DateTime.Now;

                var test = gestaoAvaliacaoContext.Test.Include("Discipline").Include("TestCurriculumGrades")
                    .Include("TestPerformanceLevels").Include("TestPerformanceLevels.PerformanceLevel")
                    .Include("TestItemLevels").Include("TestItemLevels.ItemLevel").Include("TestType")
                    .Include("TestSubGroup").Include("BlockChains").Include("Blocks")
                    .FirstOrDefault(a => a.Id == entity.Id);

                if (test == null)
                    return entity;

                test.RemoveBlockChain = entity.BlockChainNumber < test.BlockChainNumber || entity.BlockChainItems < test.BlockChainItems;
                test.RemoveBlockChainBlock = entity.NumberBlock < test.NumberBlock || entity.BlockChainItems < test.BlockChainItems;

                test.TestSituation = entity.TestSituation;

                test.Description = entity.Description;

                test.ApplicationEndDate = entity.ApplicationEndDate;
                test.ApplicationStartDate = entity.ApplicationStartDate;
                test.CorrectionEndDate = entity.CorrectionEndDate;
                test.CorrectionStartDate = entity.CorrectionStartDate;
                test.DownloadStartDate = entity.DownloadStartDate;

                test.Bib = entity.Bib;
                test.FrequencyApplication = entity.FrequencyApplication;
                test.Password = entity.Password;

                if (entity.Discipline != null)
                    test.Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == entity.Discipline.Id);
                else if (entity.Discipline == null && entity.Multidiscipline)
                    test.Discipline = null;

                if (entity.FormatType != null)
                    test.FormatType = gestaoAvaliacaoContext.FormatType.FirstOrDefault(s => s.Id == entity.FormatType.Id);

                test.TestType = gestaoAvaliacaoContext.TestType.FirstOrDefault(l => l.Id == entity.TestType.Id);
                test.TestTime = gestaoAvaliacaoContext.TestTime.FirstOrDefault(l => l.Id == entity.TestTime.Id);

                test.TestSubGroup = entity.TestSubGroup != null
                    ? gestaoAvaliacaoContext.TestSubGroup.FirstOrDefault(l => l.Id == entity.TestSubGroup.Id)
                    : null;

                test.NumberBlock = entity.NumberBlock;
                test.NumberItem = entity.NumberItem;
                test.NumberItemsBlock = entity.NumberItemsBlock;

                #region testCurriculumGrades

                List<TestCurriculumGrade> testCurriculumGrades = new List<TestCurriculumGrade>();

                foreach (TestCurriculumGrade testCurriculumGrade in test.TestCurriculumGrades.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
                {
                    if (!entity.TestCurriculumGrades.Any(p => p.TypeCurriculumGradeId == testCurriculumGrade.TypeCurriculumGradeId))
                    {
                        testCurriculumGrade.State = Convert.ToByte(EnumState.excluido);
                        testCurriculumGrade.UpdateDate = dateNow;
                        testCurriculumGrades.Add(testCurriculumGrade);
                    }
                    else
                    {
                        testCurriculumGrade.UpdateDate = dateNow;
                        testCurriculumGrades.Add(testCurriculumGrade);
                    }

                    entity.TestCurriculumGrades.RemoveAll(p => p.TypeCurriculumGradeId == testCurriculumGrade.TypeCurriculumGradeId);
                }

                testCurriculumGrades.AddRange(entity.TestCurriculumGrades);

                if (testCurriculumGrades != null && testCurriculumGrades.Count > 0)
                    test.TestCurriculumGrades.AddRange(testCurriculumGrades);

                #endregion

                #region testItemLevels

                List<TestItemLevel> itemlevels = new List<TestItemLevel>();

                foreach (TestItemLevel testitemlevel in test.TestItemLevels.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
                {
                    if (!entity.TestItemLevels.Any(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id))
                    {
                        testitemlevel.State = Convert.ToByte(EnumState.excluido);
                        testitemlevel.UpdateDate = dateNow;
                        itemlevels.Add(testitemlevel);
                    }
                    else
                    {
                        testitemlevel.Value = entity.TestItemLevels.FirstOrDefault(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id).Value;
                        testitemlevel.PercentValue = entity.TestItemLevels.FirstOrDefault(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id).PercentValue;
                        testitemlevel.UpdateDate = dateNow;
                        itemlevels.Add(testitemlevel);
                    }

                    entity.TestItemLevels.RemoveAll(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id);
                }

                foreach (var t in entity.TestItemLevels)
                {
                    t.ItemLevel = gestaoAvaliacaoContext.ItemLevel.FirstOrDefault(f => f.Id == t.ItemLevel.Id);
                }

                itemlevels.AddRange(entity.TestItemLevels);

                if (itemlevels != null && itemlevels.Count > 0)
                    test.TestItemLevels.AddRange(itemlevels);

                #endregion

                #region testPerformanceLevels

                List<TestPerformanceLevel> performancelevels = new List<TestPerformanceLevel>();

                foreach (TestPerformanceLevel testperformancelevel in test.TestPerformanceLevels.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
                {
                    if (!entity.TestPerformanceLevels.Any(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id))
                    {
                        testperformancelevel.State = Convert.ToByte(EnumState.excluido);
                        testperformancelevel.UpdateDate = dateNow;
                        performancelevels.Add(testperformancelevel);
                    }
                    else
                    {
                        testperformancelevel.Value1 = entity.TestPerformanceLevels.FirstOrDefault(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id).Value1;
                        testperformancelevel.Value2 = entity.TestPerformanceLevels.FirstOrDefault(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id).Value2;
                        testperformancelevel.UpdateDate = dateNow;
                        performancelevels.Add(testperformancelevel);
                    }

                    entity.TestPerformanceLevels.RemoveAll(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id);
                }

                foreach (var t in entity.TestPerformanceLevels)
                {
                    t.PerformanceLevel = gestaoAvaliacaoContext.PerformanceLevel.FirstOrDefault(f => f.Id == t.PerformanceLevel.Id);
                }

                performancelevels.AddRange(entity.TestPerformanceLevels);

                if (performancelevels != null && performancelevels.Count > 0)
                    test.TestPerformanceLevels.AddRange(performancelevels);

                #endregion

                test.Multidiscipline = entity.Multidiscipline;
                test.KnowledgeAreaBlock = entity.KnowledgeAreaBlock;
                test.ElectronicTest = entity.ElectronicTest;
                test.ShowOnSerapEstudantes = entity.ShowOnSerapEstudantes;
                test.NumberSynchronizedResponseItems = entity.NumberSynchronizedResponseItems;
                test.ShowTestContext = entity.ShowTestContext;
                test.ShowVideoFiles = entity.ShowVideoFiles;
                test.ShowAudioFiles = entity.ShowAudioFiles;
                test.ShowJustificate = entity.ShowJustificate;
                test.TestTai = entity.TestTai;
                test.ProvaComProficiencia = entity.ProvaComProficiencia;
                test.ApresentarResultados = entity.ApresentarResultados;
                test.ApresentarResultadosPorItem = entity.ApresentarResultadosPorItem;

                test.UpdateDate = DateTime.Now;

                test.BlockChain = entity.BlockChain;
                test.BlockChainNumber = entity.BlockChainNumber;
                test.BlockChainItems = entity.BlockChainItems;
                test.BlockChainForBlock = entity.BlockChainForBlock;

                gestaoAvaliacaoContext.Entry(test).State = EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(Id);

                return test;
            }
        }

        public void Delete(long Id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Test test = GestaoAvaliacaoContext.Test.Include("TestCurriculumGrades").FirstOrDefault(a => a.Id == Id);

                if (test != null)
                {
                    #region Dependencies


                    List<TestContext> testContexts = GestaoAvaliacaoContext.TestContext.Where(i => i.Test_Id == Id).ToList();
                    List<BlockItem> blockItems = GestaoAvaliacaoContext.BlockItem.Include("Block").Where(i => i.Block.Test_Id == Id).ToList();
                    List<Block> blocks = GestaoAvaliacaoContext.Block.Where(i => i.Test_Id == Id).ToList();
                    List<Booklet> booklets = GestaoAvaliacaoContext.Booklet.Where(i => i.Test_Id == Id).ToList();

                    if(testContexts != null)
                    {
                        testContexts.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    if (blockItems != null)
                    {
                        blockItems.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    if (blocks != null)
                    {
                        blocks.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    if (booklets != null)
                    {
                        booklets.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    if (test.TestCurriculumGrades != null)
                    {
                        test.TestCurriculumGrades.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    #endregion

                    test.State = Convert.ToByte(EnumState.excluido);
                    test.UpdateDate = DateTime.Now;
                    GestaoAvaliacaoContext.Entry(test).State = System.Data.Entity.EntityState.Modified;

                    GestaoAvaliacaoContext.SaveChanges();
                    LimparCache_GetObject(Id);
                }
            }
        }

        public void SwitchAllAdhrered(Test test)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Test entity = GestaoAvaliacaoContext.Test.FirstOrDefault(a => a.Id == test.Id);

                if (entity != null)
                {
                    entity.AllAdhered = test.AllAdhered;
                    entity.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                    GestaoAvaliacaoContext.SaveChanges();
                    LimparCache_GetObject(entity.Id);
                }
            }
        }

        public Test UpdateSituation(long Id, EnumTestSituation Situation)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Test test = GestaoAvaliacaoContext.Test.FirstOrDefault(a => a.Id == Id);

                test.TestSituation = Situation;

                test.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(test).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
                LimparCache_GetObject(test.Id);

                return test;
            }

        }

        public void UpdateTestFeedback(long Id, bool publicFeedback)
        {
            var sql = new StringBuilder("UPDATE [Test] SET [PublicFeedback] = @publicFeedback, [UpdateDate] = @updateDate ");
            sql.AppendLine("WHERE [Id] = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = Id,
                        updateDate = DateTime.Now,
                        publicFeedback = publicFeedback
                    });
            }
        }

        public void UpdateTestProcessedCorrection(long Id, bool processedCorrection)
        {
            var sql = new StringBuilder("UPDATE [Test] SET [ProcessedCorrection] = @processedCorrection, [ProcessedCorrectionDate] = @date ");
            sql.AppendLine("WHERE [Id] = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                cn.Execute(sql.ToString(),
                    new
                    {
                        id = Id,
                        date = DateTime.Now,
                        processedCorrection = processedCorrection
                    });
            }

            LimparCache_GetObject(Id);
        }

        public void UpdateTestVisible(long Id, bool visible)
        {
            var sql = new StringBuilder("UPDATE [Test] SET [Visible] = @visible, [UpdateDate] = @updateDate ");
            sql.AppendLine("WHERE [Id] = @id");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = Id,
                        updateDate = DateTime.Now,
                        visible = visible
                    });
            }

            LimparCache_GetObject(Id);
        }

        public Test SelectOrderTestUp(long order)
        {
            var sql = @"               
				SELECT TOP 1 Id, [Description], TestType_Id, Discipline_Id, Bib, NumberItemsBlock, NumberBlock, FormatType_Id, 
					NumberItem, ApplicationStartDate, ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, UsuId, TestSituation, 
					AllAdhered, CreateDate, UpdateDate, [State], PublicFeedback, ProcessedCorrection, ProcessedCorrectionDate, Visible, 
					FrequencyApplication, Multidiscipline, TestSubGroup_Id, [Order] 
				FROM Test WITH(NOLOCK) 
				WHERE [Order] > @Order AND [State] = 1 
				ORDER BY Test.[Order]";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Test>(sql.ToString(), new
                {
                    Order = order
                }).FirstOrDefault();
            }
        }

        public Test SelectOrderTestDown(long order)
        {
            var sql = @"               
				SELECT TOP 1 Id, [Description], TestType_Id, Discipline_Id, Bib, NumberItemsBlock, NumberBlock, FormatType_Id, 
					NumberItem, ApplicationStartDate, ApplicationEndDate, CorrectionStartDate, CorrectionEndDate, UsuId, TestSituation, 
					AllAdhered, CreateDate, UpdateDate, [State], PublicFeedback, ProcessedCorrection, ProcessedCorrectionDate, Visible, 
					FrequencyApplication, Multidiscipline, TestSubGroup_Id, [Order] 
				FROM Test WITH(NOLOCK) 
				WHERE [Order] < @Order AND [State] = 1 
				ORDER BY Test.[Order] DESC";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Test>(sql.ToString(), new
                {
                    Order = order
                }).FirstOrDefault();
            }
        }

        public long GetMaxOrderTests()
        {
            var sql = @"               
				SELECT MAX([Order])
				FROM Test WITH(NOLOCK)";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<long>(sql.ToString()).FirstOrDefault();
            }
        }

        public TestShowVideoAudioFilesDto GetTestShowVideoAudioFiles(long testId)
        {
            var query = @"SELECT
                            Id AS TestId,
                            ShowVideoFiles,
                            ShowAudioFiles
                        FROM [Test] (NOLOCK)
                        WHERE
                            Id = @testId";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = cn.Query<TestShowVideoAudioFilesDto>(query, new { testId });
                return result.FirstOrDefault();
            }
        }

        #endregion
    }
}