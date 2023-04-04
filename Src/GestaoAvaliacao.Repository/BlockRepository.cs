using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Entities.Projections;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class BlockRepository : ConnectionReadOnly, IBlockRepository
    {
        #region Read

        public IEnumerable<ItemWithOrderAndRevoked> GetTestItemBlocks(Int64 TestId)
        {
            using (IDbConnection cn = Connection)
            {
                var situations = new[] { EnumSituation.RevokedTest, EnumSituation.Revoked };

                cn.Open();
                var sql = @"SELECT BI.Id, BI.Block_Id, BI.Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], CASE WHEN RR.Id IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Revoked, Em.Discipline_Id 
						   FROM BlockItem BI WITH (NOLOCK) 
						   INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id 
	                       INNER JOIN Item I WITH(NOLOCK) ON BI.Item_Id = I.Id AND I.State <> 3
	                       INNER JOIN EvaluationMatrix Em WITH(NOLOCK) ON I.EvaluationMatrix_Id = Em.Id AND Em.State <> 3
                           INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] 
                           LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = 1 
                           LEFT JOIN RequestRevoke RR ON RR.BlockItem_Id = BI.Id AND RR.State = 1 AND RR.Situation IN @Situations 
						   WHERE B.Test_Id = @TestId 
						   AND BI.State = 1 AND B.State = 1";

                var retorno = cn.Query<ItemWithOrderAndRevoked>(sql.ToString(), new
                {
                    TestId = TestId,
                    Situations = situations
                });

                return retorno;
            }
        }

        public IEnumerable<Block> GetBlocksByItensTests(List<long> tests)
        {
            using (IDbConnection cn = Connection)
            {
                var situations = new[] { EnumSituation.RevokedTest, EnumSituation.Revoked };

                cn.Open();
                StringBuilder sql = new StringBuilder(@"SELECT  B.Id, T.Id AS Test_Id, BI.Id, BI.Block_Id, BI.Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order]
                            FROM BlockItem BI WITH (NOLOCK) 
                            INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id 
                            INNER JOIN Item I WITH(NOLOCK) ON BI.Item_Id = I.Id AND I.State <> 3
                            INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] 
                            LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = 1 
                            WHERE BI.State = 1 AND B.State = 1 ");
                sql.AppendFormat("AND T.Id IN ({0})", string.Join(",", tests));

                var lookup = new Dictionary<long, Block>();
                cn.Query<Block, BlockItem, Block>(sql.ToString(),
                     (B, BI) =>
                     {
                         Block found;
                         if (!lookup.TryGetValue(B.Id, out found))
                         {
                             B.BlockItems = new List<BlockItem>();
                             lookup.Add(B.Id, B);

                             found = B;
                         }
                         found.BlockItems.Add(BI);
                         return found;
                     }).Distinct().ToDictionary(p => p.Id);

                return lookup.Values;
            }
        }

        public IEnumerable<Block> GetTestBlocks(Int64 TestId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description, Test_Id " +
                           "FROM Block WITH (NOLOCK) " +
                           "WHERE Test_Id = @TestId " +
                           "AND State = @state " +

                           "SELECT T.Id, T.KnowledgeAreaBlock " +
                           "FROM Test T WITH(NOLOCK)" +
                           "WHERE Id = @TestId " +

                           "SELECT BI.Id, BI.Block_Id, BI.Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], I.KnowledgeArea_Id " +
                           "FROM BlockItem BI WITH (NOLOCK) " +
                           "INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id " +
                           "INNER JOIN Item I WITH(NOLOCK) ON BI.Item_Id = I.Id AND I.State <> 3 " +
                           "INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] " +
                           "LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state " +
                           "WHERE B.Test_Id = @TestId " +
                           "AND BI.State = @state AND B.State = @state";

                var multi = cn.QueryMultiple(sql, new { TestId = TestId, state = (Byte)EnumState.ativo });

                var listBlock = multi.Read<Block>();
                var listTest = multi.Read<Test>();
                var listBlockItem = multi.Read<BlockItem>();

                foreach (var block in listBlock)
                {
                    block.Test = listTest.FirstOrDefault(p => p.Id == block.Test_Id);
                    block.BlockItems.AddRange(listBlockItem.Where(i => i.Block_Id.Equals(block.Id)));
                }

                return listBlock;
            }
        }

        public IEnumerable<Item> GetBlockItens(Int64 Id, int page, int pageItens)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"WITH ItensPage AS
                            (
                                SELECT ROW_NUMBER() OVER (ORDER BY BI.[Order]) AS RowNum, I.Id, I.ItemCode, I.ItemVersion, I.Statement, I.Revoked, I.KnowledgeArea_Id, I.ItemCodeVersion, K.Description AS KnowledgeArea_Description, CASE WHEN (T.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) ELSE 0 END AS KnowledgeArea_Order 
                                FROM Item I WITH (NOLOCK) 
                                INNER JOIN BlockItem BI WITH (NOLOCK) ON BI.Item_Id = I.Id 
                                INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id 
                                INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] 
                                LEFT JOIN KnowledgeArea K WITH (NOLOCK) ON I.KnowledgeArea_Id = K.Id AND K.State = @state 
                                LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = K.Id AND B.Id = Bka.Block_Id AND Bka.State = @state 
                                WHERE BI.Block_Id = @id 
                                AND BI.State = @state AND I.State = @state 
                            )

                            SELECT
                                *
                            FROM ItensPage
                            WHERE RowNum BETWEEN @initialPageItem AND @finalPageItem;";

                var initialPageItem = (page * pageItens) + 1;
                var listItems = cn.Query<Item>(sql, new { id = Id, state = (Byte)EnumState.ativo, initialPageItem, finalPageItem = initialPageItem + pageItens - 1 });

                foreach (var item in listItems)
                {
                    var itemId = item.Id;

                    var sqlMulti = @"SELECT B.Id, B.Description, B.Source 
									 FROM Item I WITH (NOLOCK) 
									 INNER JOIN BaseText B WITH (NOLOCK) ON B.Id = I.BaseText_Id
									 WHERE I.Id = @id 
									 AND I.State = @state AND B.State = @state 

									 SELECT L.Description, L.Value 
									 FROM Item I WITH (NOLOCK) 
									 INNER JOIN ItemLevel L WITH (NOLOCK) ON L.Id = I.ItemLevel_Id 
									 WHERE I.Id = @id 
									 AND I.State = @state AND L.State = @state 
									 
									 SELECT TypeCurriculumGradeId 
									 FROM ItemCurriculumGrade WITH (NOLOCK) 
									 WHERE Item_Id = @id 
									 AND State = @state 
									 
									 SELECT BI.Id, BI.Block_Id, BI.Item_Id, bi.[Order]  
									 FROM BlockItem BI WITH (NOLOCK) 
                                     INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id 
									 INNER JOIN Item I WITH (NOLOCK) ON I.Id = BI.Item_Id 
                                     INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] 
                                     WHERE BI.Item_Id = @id AND BI.Block_Id = @blockId 
									 AND I.State = @state AND BI.State = @state 

									 SELECT D.Id, D.Description 
									 FROM Item I 
									 INNER JOIN EvaluationMatrix EM WITH (NOLOCK)ON EM.Id = I.EvaluationMatrix_Id 
									 INNER JOIN Discipline D WITH(NOLOCK) ON EM.Discipline_Id = D.Id 
									 WHERE I.State = @state 
									 AND D.State = @state 
									 AND I.Id = @id "
                                   ;

                    var multi = cn.QueryMultiple(sqlMulti, new { id = itemId, blockId = Id, state = (Byte)EnumState.ativo });

                    var listBaseText = multi.Read<BaseText>();
                    var listItemLevel = multi.Read<ItemLevel>();
                    var listItemCurriculumGrade = multi.Read<ItemCurriculumGrade>();
                    var listBlockItems = multi.Read<BlockItem>();
                    var discipline = multi.Read<Discipline>();

                    item.BaseText = listBaseText.FirstOrDefault();
                    item.ItemLevel = listItemLevel.FirstOrDefault();
                    item.ItemCurriculumGrades.AddRange(listItemCurriculumGrade);
                    item.BlockItems.AddRange(listBlockItems);
                    item.EvaluationMatrix = new EvaluationMatrix();
                    item.EvaluationMatrix.Discipline = discipline.FirstOrDefault();

                }

                return listItems;
            }
        }
        public IEnumerable<BlockKnowledgeArea> GetBlockKnowledgeAreas(long Id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT Bka.Id, Bka.Block_Id, Bka.KnowledgeArea_Id, Bka.[Order], k.[Description] 
                            FROM BlockKnowledgeArea Bka WITH(NOLOCK) 
                            INNER JOIN KnowledgeArea K WITH(NOLOCK) ON Bka.KnowledgeArea_Id = K.Id 
                            WHERE Bka.Block_Id = @id 
                            AND Bka.State = @state AND K.State = @state 
                            ORDER BY Bka.[Order]";

                return cn.Query<BlockKnowledgeArea>(sql, new { id = Id, state = (Byte)EnumState.ativo });
            }
        }

        public IEnumerable<Block> GetBookletItems(Int64 BookletId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT Id, Description " +
                           "FROM Block WITH (NOLOCK) " +
                           "WHERE Booklet_Id = @id " +
                           "AND State = @state " +

                           "SELECT BI.Id, BI.Block_Id, BI.Item_Id, (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], I.KnowledgeArea_Id, K.[Description] AS KnowledgeArea_Description " +
                           "FROM BlockItem BI WITH (NOLOCK) " +
                           "INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id " +
                           "INNER JOIN Item I WITH(NOLOCK) ON BI.Item_Id = I.Id AND I.State <> 3 " +
                           "INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.[Test_Id] " +
                           "LEFT JOIN KnowledgeArea K WITH(NOLOCK) ON I.KnowledgeArea_Id = K.Id AND K.State = @state " +
                           "LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state " +
                           "WHERE B.Booklet_Id = @id " +
                           "AND BI.State = @state AND B.State = @state";

                var multi = cn.QueryMultiple(sql, new { id = BookletId, state = (Byte)EnumState.ativo });

                var listBlock = multi.Read<Block>();
                var listBlockItem = multi.Read<BlockItem>();

                foreach (var blockItem in listBlockItem)
                {
                    var itemId = blockItem.Item_Id;

                    var sqlMulti = @"SELECT Id, Statement " +
                                    "FROM Item WITH (NOLOCK) " +
                                    "WHERE Id = @id " +
                                    "AND State = @state " +

                                   "SELECT B.Id, B.Description, B.Source " +
                                   "FROM Item I WITH (NOLOCK) " +
                                   "INNER JOIN BaseText B WITH (NOLOCK) ON B.Id = I.BaseText_Id " +
                                   "WHERE I.Id = @id " +
                                   "AND I.State = @state AND B.State = @state " +

                                   "SELECT Id, Description " +
                                   "FROM Alternative WITH (NOLOCK) " +
                                   "WHERE Item_Id = @id " +
                                   "AND State = @state " +
                                   "ORDER BY [Order]";

                    var multi2 = cn.QueryMultiple(sqlMulti, new { id = itemId, state = (Byte)EnumState.ativo });

                    var listItem = multi2.Read<Item>();
                    var listBaseText = multi2.Read<BaseText>();
                    var listAlternative = multi2.Read<Entities.Alternative>();

                    Item item = listItem.FirstOrDefault();
                    if (item != null)
                    {
                        item.BaseText = listBaseText.FirstOrDefault();
                        item.Alternatives.AddRange(listAlternative);
                        blockItem.Item = item;
                    }
                }

                foreach (var block in listBlock)
                {
                    block.BlockItems.AddRange(listBlockItem.Where(i => i.Block_Id.Equals(block.Id)));
                }

                return listBlock;
            }
        }

        public int CountItemTest(long Id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT COUNT(I.Id) " +
                            "FROM Item I WITH (NOLOCK) " +
                            "INNER JOIN BlockItem BI WITH (NOLOCK) ON BI.Item_Id = I.Id " +
                            "INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id " +
                            "WHERE B.Test_Id = @id " +
                            "AND B.State = @state AND BI.State = @state AND I.State = @state";

                var count = (int)cn.ExecuteScalar(sql, new { id = Id, state = (Byte)EnumState.ativo });

                return count;
            }
        }

        public int CountItemTestBIB(long Id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT COUNT(I.Id) " +
                            "FROM Item I WITH (NOLOCK) " +
                            "INNER JOIN BlockItem BI WITH (NOLOCK) ON BI.Item_Id = I.Id " +
                            "INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id " +
                            "WHERE B.Test_Id = @id " +
                            "AND B.State = @state AND BI.State = @state AND " +
                            "I.State = @state group by BI.block_id order by 1 desc";

                var resposta = cn.ExecuteScalar(sql, new { id = Id, state = (Byte)EnumState.ativo });

                if (resposta == null)
                    return 0;

                return (int)resposta;
            }
        }

        /// <summary>
        /// Busca os itens da prova
        /// </summary>
        /// <param name="testId">Id da prova</param>
        /// <returns>Lista de projection com informações de cada item da prova</returns>
        public async Task<IEnumerable<ItemWithSkillAndAlternativeProjection>> GetItemsWithSkillAndCorrectAlternativeBytest(long testId)
        {
            var situations = new[] { EnumSituation.RevokedTest, EnumSituation.Revoked };

            var sql = new StringBuilder(@"SELECT I.Id AS Item_Id, 
										(DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order], 
										A.Numeration AS CorrectAlternativeNumeration, 
										S.Description AS SkillDescription, 
										S.Code AS SkillCode,
										CASE WHEN RR.Id IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Revoked ")
                            .AppendLine("FROM Item I  ")
                            .AppendLine("INNER JOIN BlockItem BI ON BI.Item_Id = I.Id ")
                            .AppendLine("INNER JOIN Block B ON B.Id = BI.Block_Id  ")
                            .AppendLine("INNER JOIN Alternative A ON A.Item_Id = I.Id ")
                            .AppendLine("INNER JOIN ItemSkill SS ON SS.Item_Id = I.Id ")
                            .AppendLine("INNER JOIN Skill S ON S.Id = SS.Skill_Id ")
                            .AppendLine("INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.Test_Id ")
                            .AppendLine("LEFT JOIN RequestRevoke RR ON RR.BlockItem_Id = BI.Id AND RR.State = @state AND RR.Situation IN @Situations ")
                            .AppendLine("LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = @state ")
                            .AppendLine("WHERE B.Test_Id = @Test_Id ")
                            .AppendLine("AND B.State = @state AND BI.State = @state AND I.State = @state AND S.LastLevel = 1 AND A.Correct = 1")
                            .AppendLine("ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order], A.[Order] ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var item = cn.QueryAsync<ItemWithSkillAndAlternativeProjection>(sql.ToString(), new { Test_Id = testId, state = (Byte)EnumState.ativo, Situations = situations });

                return await item;
            }
        }

        public IEnumerable<StudentCorrectionAnswerGrid> GetTestQuestions(long Id)
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
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var lookup = new Dictionary<long, StudentCorrectionAnswerGrid>();

                cn.Query<StudentCorrectionAnswerGrid, StudentCorrectionAnswerGridAlternatives, StudentCorrectionAnswerGrid>(sql.ToString(),
                    (question, alternative) =>
                    {
                        StudentCorrectionAnswerGrid found;
                        if (!lookup.TryGetValue(question.Item_Id, out found))
                        {
                            question.Alternatives = new List<StudentCorrectionAnswerGridAlternatives>();
                            lookup.Add(question.Item_Id, question);

                            found = question;
                        }
                        found.Alternatives.Add(alternative);
                        return found;
                    },
                new { id = Id, state = (Byte)EnumState.ativo });

                return lookup.Values;
            }
        }

        public IEnumerable<BlockItem> GetItemsByTestId(long test_id, Guid UsuId, ref Pager pager)
        {
            var sql = @"WITH NumberedTestItems (BlockItem_Id,[Order],Item_Id,ItemCode,ItemVersion,Statement, BaseText_Id,Description, RowNumber) AS " +
                        "( " +
                            "SELECT bi.Id,(DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order]," +
                            "i.Id,i.ItemCode,i.ItemVersion,i.Statement," +
                            "bx.Id, bx.Description, " +
                            "ROW_NUMBER() OVER (ORDER BY (DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order]) AS RowNumber " +
                            "FROM Test t " +
                            "INNER JOIN Booklet bt WITH (NOLOCK) on t.ID = bt.Test_Id " +
                            "INNER JOIN Block bl WITH (NOLOCK) on bt.Id = bl.Booklet_Id " +
                            "INNER JOIN BlockItem  bi WITH (NOLOCK) on bl.Id = bi.Block_Id AND bi.State = @state " +
                            "INNER JOIN Item i  WITH (NOLOCK) on i.Id = bi.Item_Id " +
                            "LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = i.KnowledgeArea_Id AND bl.Id = Bka.Block_Id AND Bka.State = @state " +
                            "LEFT JOIN BaseText bx  WITH (NOLOCK) on i.BaseText_Id = bx.Id " +
                            "WHERE t.id = @id " +
                            ") " +
                            "SELECT BlockItem_Id AS Id, [Order]," +
                                    "Item_Id AS Id, ItemCode,ItemVersion,Statement," +
                                    "BaseText_Id AS Id, Description " +
                            "FROM NumberedTestItems " +
                            "WHERE RowNumber > ( @pageSize * @page ) " +
                            "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                            "ORDER BY RowNumber";

            var countSql = @"SELECT COUNT(bi.Id) " +
                            "FROM Test t " +
                            "INNER JOIN Booklet bt WITH (NOLOCK) on t.ID = bt.Test_Id " +
                            "INNER JOIN Block bl WITH (NOLOCK) on bt.Id = bl.Booklet_Id " +
                            "INNER JOIN BlockItem  bi WITH (NOLOCK) on bl.Id = bi.Block_Id AND bi.State = @state " +
                            "INNER JOIN Item i  WITH (NOLOCK) on i.Id = bi.Item_Id " +
                            "LEFT JOIN BaseText bx  WITH (NOLOCK) on i.BaseText_Id = bx.Id " +
                            "WHERE t.id = @id";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var blockItems = cn.Query<BlockItem, Item, BaseText, BlockItem>(sql,
                    (bi, i, bx) =>
                    {
                        i.BaseText = bx;
                        bi.Item = i;
                        return bi;
                    }, new { state = (Byte)EnumState.ativo, pageSize = pager.PageSize, page = pager.CurrentPage, id = test_id });

                var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, id = test_id, usuId = UsuId });

                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return blockItems;
            }
        }

        public async Task<IEnumerable<BlockItem>> GetItemsByTestIdAsync(long test_id, Guid UsuId, int page, int pageItens)
        {
            var sql = @"
                        SELECT 
	                        bi.Id AS BlockItem_Id,
	                        bi.[Order], 
	                        i.Id AS Item_Id,
	                        i.ItemCode,
	                        i.ItemVersion,
	                        i.Statement, 
	                        bx.Id AS BaseText_Id, 
	                        bx.Description,  
	                        ROW_NUMBER() OVER (ORDER BY bi.[Order]) AS RowNumber 
                        INTO #tempBlockItens
                        FROM Test t  
                        INNER JOIN Booklet bt WITH (NOLOCK) on t.ID = bt.Test_Id  
                        INNER JOIN Block bl WITH (NOLOCK) on bt.Id = bl.Booklet_Id  
                        INNER JOIN BlockItem  bi WITH (NOLOCK) on bl.Id = bi.Block_Id AND bi.State = @state
                        INNER JOIN Item i  WITH (NOLOCK) on i.Id = bi.Item_Id  
                        LEFT JOIN BaseText bx  WITH (NOLOCK) on i.BaseText_Id = bx.Id  
                        WHERE t.id = @id;

                        SELECT BlockItem_Id AS Id, [Order], 
                                Item_Id AS Id, ItemCode,ItemVersion,Statement,   
                                BaseText_Id AS Id, Description  
                        FROM #tempBlockItens
                        WHERE RowNumber BETWEEN @initialPageItem AND @finalPageItem
                        ORDER BY RowNumber;";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var transaction = cn.BeginTransaction();

                var initialPageItem = (page * pageItens) + 1;
                var parameters = new { state = (Byte)EnumState.ativo, id = test_id, initialPageItem, finalPageItem = initialPageItem + pageItens - 1 };

                var blockItems = await cn.QueryAsync<BlockItem, Item, BaseText, BlockItem>(sql,
                    (bi, i, bx) =>
                    {
                        i.BaseText = bx;
                        bi.Item = i;
                        return bi;
                    }, parameters, transaction);
                if (!blockItems.Any()) return blockItems;

                return blockItems;
            }
        }

        public IEnumerable<BlockItem> GetPendingRevokeItems(ref Pager pager, string ItemCode, DateTime? StartDate, DateTime? EndDate, EnumSituation? Situation)
        {
            var sql = @"WITH NumberedPendingRevokeItems (BlockItem_Id,[Order],Item_Id,ItemCode,ItemVersion,Statement,Revoked,LastVersion,BaseText_Id,Description,RowNumber) AS " +
            "( " +
                "SELECT bi.Id,(DENSE_RANK() OVER(ORDER BY CASE WHEN (t.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order]," +
                "i.Id,i.ItemCode,i.ItemVersion,i.Statement,i.Revoked, i.LastVersion, " +
                "bx.Id, bx.Description, " +
                "ROW_NUMBER() OVER (ORDER BY rr.CreateDate) AS RowNumber " +
                "FROM Test t " +
                "INNER JOIN Booklet bt WITH (NOLOCK) on t.ID = bt.Test_Id " +
                "INNER JOIN Block bl WITH (NOLOCK) on bt.Id = bl.Booklet_Id " +
                "INNER JOIN BlockItem  bi WITH (NOLOCK) on bl.Id = bi.Block_Id AND bi.State = @state " +
                "INNER JOIN Item i  WITH (NOLOCK) on i.Id = bi.Item_Id " +
                (!string.IsNullOrEmpty(ItemCode) ? "AND UPPER(LTRIM(RTRIM(i.ItemCode))) = @itemCode " : "") +
                "LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = i.KnowledgeArea_Id AND bl.Id = Bka.Block_Id AND Bka.State = @state " +
                "LEFT JOIN BaseText bx  WITH (NOLOCK) on i.BaseText_Id = bx.Id " +
                "INNER JOIN (" +
                "SELECT  BlockItem_Id, CreateDate FROM ( " +
                "SELECT distinct rr.BlockItem_Id, rr.CreateDate, ROW_NUMBER() OVER(PARTITION BY rr.BlockItem_Id ORDER BY rr.CreateDate) AS RowNumber " +
                " FROM RequestRevoke rr WITH (NOLOCK) WHERE " +
                (Situation != null ? "rr.Situation = @situation " : "Situation != 1 ");

            if (StartDate != null && StartDate.Equals(DateTime.MinValue))
                StartDate = null;

            if (EndDate != null && EndDate.Equals(DateTime.MinValue))
                EndDate = null;

            if (StartDate == null && EndDate != null)
                sql += "AND CAST(rr.CreateDate AS DATE) <= CAST(@endDate AS DATE) ";
            else if (StartDate != null && EndDate == null)
                sql += "AND CAST(rr.CreateDate AS DATE) >= CAST(@startDate AS DATE) ";
            else if (StartDate != null && EndDate != null)
                sql += "AND CAST(rr.CreateDate AS DATE) >= CAST(@startDate AS DATE) AND CAST(rr.CreateDate AS DATE) <= CAST(@endDate AS DATE) ";

            sql += ") AS temp WHERE RowNumber = 1 " +
                   ") AS rr ON bi.Id = rr.BlockItem_Id ) " +
                    "SELECT BlockItem_Id AS Id, [Order]," +
                            "Item_Id AS Id, ItemCode,ItemVersion,Statement, Revoked, LastVersion," +
                            "BaseText_Id AS Id, Description " +
                    "FROM NumberedPendingRevokeItems " +
                    "WHERE RowNumber > ( @pageSize * @page ) " +
                    "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                    "ORDER BY RowNumber ";

            var countSql = "SELECT count(bi.Id) " +
                            "FROM Test t " +
                            "INNER JOIN Booklet bt WITH (NOLOCK) on t.ID = bt.Test_Id " +
                            "INNER JOIN Block bl WITH (NOLOCK) on bt.Id = bl.Booklet_Id  " +
                            "INNER JOIN BlockItem  bi WITH (NOLOCK) on bl.Id = bi.Block_Id AND bi.State = 1 " +
                            "INNER JOIN Item i  WITH (NOLOCK) on i.Id = bi.Item_Id  " +
                            (!string.IsNullOrEmpty(ItemCode) ? "AND UPPER(LTRIM(RTRIM(i.ItemCode))) = @itemCode " : "") +

                            "LEFT JOIN BaseText bx  WITH (NOLOCK) on i.BaseText_Id = bx.Id  " +
                             "INNER JOIN ( " +
                             "SELECT  BlockItem_Id, CreateDate FROM ( " +
                             "SELECT distinct BlockItem_Id, rr.CreateDate, ROW_NUMBER() OVER(PARTITION BY rr.BlockItem_Id ORDER BY rr.CreateDate) AS RowNumber " +
                             "FROM RequestRevoke rr WITH (NOLOCK) WHERE " +
                            (Situation != null ? "rr.Situation = @situation " : "Situation != 1 ");
            if (StartDate != null && StartDate.Equals(DateTime.MinValue))
                StartDate = null;

            if (EndDate != null && EndDate.Equals(DateTime.MinValue))
                EndDate = null;

            if (StartDate == null && EndDate != null)
                countSql += "AND CAST(rr.CreateDate AS DATE) <= CAST(@endDate AS DATE) ";
            else if (StartDate != null && EndDate == null)
                countSql += "AND CAST(rr.CreateDate AS DATE) >= CAST(@startDate AS DATE) ";
            else if (StartDate != null && EndDate != null)
                countSql += "AND CAST(rr.CreateDate AS DATE) >= CAST(@startDate AS DATE) AND CAST(rr.CreateDate AS DATE) <= CAST(@endDate AS DATE) ";
            countSql += ") AS temp WHERE RowNumber = 1 " +
                        ") AS rr ON bi.Id = rr.BlockItem_Id ";

            var sqlRequest = @"SELECT rr.Id, rr.Situation, rr.CreateDate, " +
                                     "t.Id, t.Description " +
                                     "FROM RequestRevoke  as rr " +
                                     "INNER JOIN Test t ON  t.Id = rr.Test_Id " +
                                     "WHERE rr.BlockItem_Id = @BlockItem_Id " +
                                     "AND rr.State = @State ";

            ItemCode = ItemCode != null ? ItemCode.Trim().ToUpper() : ItemCode;

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var blockItems = cn.Query<BlockItem, Item, BaseText, BlockItem>(sql,
                    (bi, i, bx) =>
                    {
                        i.BaseText = bx;
                        bi.Item = i;
                        return bi;
                    }, new
                    {
                        state = (Byte)EnumState.ativo,
                        pageSize = pager.PageSize,
                        page = pager.CurrentPage,
                        situation = Situation,
                        itemCode = ItemCode,
                        startDate = StartDate,
                        endDate = EndDate
                    });

                foreach (var blockItem in blockItems)
                {
                    var listRequestRevoke = cn.Query<RequestRevoke, Test, RequestRevoke>(sqlRequest,
                        (rr, t) =>
                        {
                            rr.Test = t;
                            return rr;
                        },
                        new { state = (Byte)EnumState.ativo, BlockItem_Id = blockItem.Id, situation = Situation });

                    if (listRequestRevoke != null && listRequestRevoke.Count() > 0)
                    {
                        blockItem.RequestRevokes = new List<RequestRevoke>();
                        foreach (RequestRevoke requestRevoke in listRequestRevoke)
                        {
                            blockItem.RequestRevokes.Add(requestRevoke);
                        }
                    }
                }

                var count = (int)cn.ExecuteScalar(countSql, new
                {
                    state = (Byte)EnumState.ativo,
                    itemCode = @ItemCode,
                    situation = Situation,
                    startDate = StartDate,
                    endDate = EndDate
                });

                pager.SetTotalPages((int)Math.Ceiling(count / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return blockItems;
            }
        }

        #endregion

        #region Write

        public Block Save(Block block)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var dateNow = DateTime.Now;

                if (!block.Test.Bib)
                {
                    var booklet = new Booklet
                    {
                        Order = 1,
                        CreateDate = dateNow,
                        UpdateDate = dateNow,
                        Test_Id = block.Test_Id,
                        State = (byte)EnumState.ativo
                    };

                    gestaoAvaliacaoContext.Booklet.Add(booklet);
                    gestaoAvaliacaoContext.SaveChanges();

                    block.Booklet_Id = booklet.Id;
                }

                block.Test = null;

                var idsItems = new List<long>();
                var blockKnowledgeAreas = new List<BlockKnowledgeArea>();

                var idsBlockChain = block.BlockChainBlocks.Select(c => c.BlockChain_Id).Distinct().ToList();
                var ehCadeiaBlocos = idsBlockChain.Count > 0;

                var itemsBlockChain = new List<BlockChainItem>();

                if (ehCadeiaBlocos)
                {
                    itemsBlockChain.AddRange(gestaoAvaliacaoContext.BlockChainItems.Where(c =>
                        idsBlockChain.Contains(c.BlockChain_Id) && c.State == (byte)EnumState.ativo));

                    idsItems.AddRange(gestaoAvaliacaoContext.BlockChainItems.Include("Item")
                        .Where(c => idsBlockChain.Contains(c.BlockChain_Id) && c.State == (byte)EnumState.ativo)
                        .Select(c => c.Item.Id).Distinct());
                }
                else
                {
                    idsItems.AddRange(block.BlockItems.Where(q => q.State == (byte)EnumState.ativo).Select(p => p.Item_Id)
                        .Distinct());
                }

                var maxOrder = 0;

                foreach (var idItem in idsItems)
                {
                    var item = gestaoAvaliacaoContext.Item.FirstOrDefault(p => p.Id == idItem);

                    if (item?.KnowledgeArea_Id == null ||
                        blockKnowledgeAreas.Exists(p => p.KnowledgeArea_Id == item.KnowledgeArea_Id))
                    {
                        continue;
                    }

                    var blockKnowledgeArea = new BlockKnowledgeArea
                    {
                        Block_Id = block.Id,
                        KnowledgeArea_Id = item.KnowledgeArea_Id.Value,
                        Order = maxOrder,
                        State = (byte)EnumState.ativo,
                        CreateDate = dateNow,
                        UpdateDate = dateNow
                    };

                    blockKnowledgeAreas.Add(blockKnowledgeArea);
                    maxOrder++;
                }

                block.BlockKnowledgeAreas = blockKnowledgeAreas;

                if (ehCadeiaBlocos)
                {
                    var blockChainBlocks = idsBlockChain.Select(idBlockChain =>
                        new BlockChainBlock { BlockChain_Id = idBlockChain, Block_Id = block.Id }).ToList();

                    var blockItems = itemsBlockChain.Select(itemBlockChain => new BlockItem
                            { Block_Id = block.Id, Item_Id = itemBlockChain.Item_Id, Order = itemBlockChain.Order })
                        .ToList();

                    block.BlockChainBlocks = blockChainBlocks;
                    block.BlockItems = blockItems;
                }

                gestaoAvaliacaoContext.Block.Add(block);
                gestaoAvaliacaoContext.SaveChanges();

                return block;
            }
        }

        public void Update(Block block)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var dateNow = DateTime.Now;

                var entity = gestaoAvaliacaoContext.Block
                    .Include("Test")
                    .Include("BlockItems")
                    .Include("BlockKnowledgeAreas")
                    .Include("BlockChainBlocks")
                    .FirstOrDefault(x => x.Id == block.Id && x.State == (byte)EnumState.ativo);

                if (entity == null)
                    return;

                block.Test_Id = entity.Test_Id;
                block.Booklet_Id = entity.Booklet_Id;

                gestaoAvaliacaoContext.Entry(entity).CurrentValues.SetValues(block);

                entity.UpdateDate = dateNow;
                entity.Test.TestSituation = EnumTestSituation.Pending;

                #region BlockItems

                var blockItems = new List<BlockItem>();
                var idsItemsFront = new List<long>();
                var blockItemsFront = new List<BlockItem>();

                var idsBlockChain = block.BlockChainBlocks.Select(c => c.BlockChain_Id).Distinct().ToList();
                var ehCadeiaBlocos = idsBlockChain.Count > 0;

                var itemsBlockChain = new List<BlockChainItem>();

                if (ehCadeiaBlocos)
                {
                    itemsBlockChain.AddRange(gestaoAvaliacaoContext.BlockChainItems.Include("Item")
                        .Where(c => idsBlockChain.Contains(c.BlockChain_Id) && c.State == (byte)EnumState.ativo));

                    idsItemsFront.AddRange(itemsBlockChain.Select(c => c.Item.Id).Distinct());

                    blockItemsFront.AddRange(itemsBlockChain.Select(itemblockChain => new BlockItem
                        { Block_Id = block.Id, Item_Id = itemblockChain.Item_Id, Order = itemblockChain.Order }));
                }
                else
                {
                    idsItemsFront.AddRange(block.BlockItems.Select(c => c.Item_Id));
                    blockItemsFront.AddRange(block.BlockItems);
                }

                var blockItemsDatabase = entity.BlockItems.Where(s => s.State == (byte)EnumState.ativo)
                    .Select(s => s.Item_Id);

                var blockItemsToExclude = blockItemsDatabase.Except(idsItemsFront).ToList();

                if (blockItemsToExclude.Any())
                {
                    foreach (var blockItem in entity.BlockItems.Where(s => s.State == (byte)EnumState.ativo && blockItemsToExclude.Contains(s.Item_Id)))
                    {
                        blockItem.State = Convert.ToByte(EnumState.excluido);
                        blockItem.UpdateDate = dateNow;
                        blockItems.Add(blockItem);
                    }
                }

                foreach (var blockItemFront in blockItemsFront)
                {
                    if (blockItemFront == null) 
                        continue;

                    var blockItemDb = entity.BlockItems.FirstOrDefault(e =>
                        e.Item_Id.Equals(blockItemFront.Item_Id) &&
                        e.Block_Id.Equals(blockItemFront.Block_Id) && e.State.Equals((byte)EnumState.ativo));

                    if (blockItemDb != null)
                    {
                        blockItemDb.Order = blockItemFront.Order;
                        blockItemDb.UpdateDate = DateTime.Now;
                        gestaoAvaliacaoContext.Entry(blockItemDb).State = System.Data.Entity.EntityState.Modified;

                        blockItems.Add(blockItemDb);
                    }
                    else
                        blockItems.Add(blockItemFront);
                }

                if (blockItems.Count > 0)
                    entity.BlockItems.AddRange(blockItems);

                #endregion

                #region BlockKnowledgeAreas

                var blockKnowledgeAreasBd = entity.BlockKnowledgeAreas;

                var listKnowledgeArea = new List<long>();
                var itens = blockItems.Where(q => q.State == (byte)EnumState.ativo).Select(p => p.Item_Id).ToList();

                foreach (var idItem in itens)
                {
                    var item = gestaoAvaliacaoContext.Item.FirstOrDefault(p => p.Id == idItem);

                    if (item?.KnowledgeArea_Id != null && !listKnowledgeArea.Exists(p => p == item.KnowledgeArea_Id))
                        listKnowledgeArea.Add(item.KnowledgeArea_Id.Value);
                }

                var blockKnowledgeAreas = blockKnowledgeAreasBd.FindAll(p =>
                    p.State == (byte)EnumState.ativo && listKnowledgeArea.Any(q => q == p.KnowledgeArea_Id));

                var maxOrder = 0;

                if (blockKnowledgeAreas.Count > 0)
                {
                    maxOrder = blockKnowledgeAreas.Max(p => p.Order + 1);
                }

                foreach (var idKnowledgeArea in listKnowledgeArea)
                {
                    if (blockKnowledgeAreas.Exists(p => p.KnowledgeArea_Id == idKnowledgeArea)) 
                        continue;

                    var blockKnowledgeArea = new BlockKnowledgeArea
                    {
                        Block_Id = block.Id,
                        KnowledgeArea_Id = idKnowledgeArea,
                        Order = maxOrder,
                        State = (byte)EnumState.ativo,
                        CreateDate = dateNow,
                        UpdateDate = dateNow
                    };

                    blockKnowledgeAreas.Add(blockKnowledgeArea);
                    maxOrder++;
                }

                block.BlockKnowledgeAreas = blockKnowledgeAreas;
                blockKnowledgeAreas = new List<BlockKnowledgeArea>();

                var blockKnowledgeAreasFront = block.BlockKnowledgeAreas.Select(s => s.KnowledgeArea_Id);
                var blockKnowledgeAreasDatabase = entity.BlockKnowledgeAreas.Where(s => s.State == (byte)EnumState.ativo).Select(s => s.KnowledgeArea_Id);
                var blockKnowledgeAreaToExclude = blockKnowledgeAreasDatabase.Except(blockKnowledgeAreasFront).ToList();

                if (blockKnowledgeAreaToExclude.Any())
                {
                    foreach (var blockKnowledgeArea in entity.BlockKnowledgeAreas.Where(s => s.State == (byte)EnumState.ativo && blockKnowledgeAreaToExclude.Contains(s.KnowledgeArea_Id)))
                    {
                        blockKnowledgeArea.State = Convert.ToByte(EnumState.excluido);
                        blockKnowledgeArea.UpdateDate = dateNow;
                        blockKnowledgeAreas.Add(blockKnowledgeArea);
                    }
                }

                foreach (var blockKnowledgeAreaFront in block.BlockKnowledgeAreas)
                {
                    var blockKnowledgeAreaDb = entity.BlockKnowledgeAreas.FirstOrDefault(e =>
                        e.KnowledgeArea_Id.Equals(blockKnowledgeAreaFront.KnowledgeArea_Id) &&
                        e.Block_Id.Equals(blockKnowledgeAreaFront.Block_Id) && e.State.Equals((byte)EnumState.ativo));

                    if (blockKnowledgeAreaDb != null)
                    {
                        blockKnowledgeAreaDb.Order = blockKnowledgeAreaFront.Order;
                        blockKnowledgeAreaDb.UpdateDate = DateTime.Now;
                        gestaoAvaliacaoContext.Entry(blockKnowledgeAreaDb).State = System.Data.Entity.EntityState.Modified;

                        blockKnowledgeAreas.Add(blockKnowledgeAreaDb);
                    }
                    else
                        blockKnowledgeAreas.Add(blockKnowledgeAreaFront);
                }

                if (blockKnowledgeAreas.Count > 0)
                    entity.BlockKnowledgeAreas.AddRange(blockKnowledgeAreas);

                #endregion

                #region BlockChainBlocks

                var blockChainBlocks = new List<BlockChainBlock>();

                var blockChainBlocksFront = block.BlockChainBlocks.Select(s => s.BlockChain_Id);
                var blockChainBlocksDatabase = entity.BlockChainBlocks.Where(s => s.State == (byte)EnumState.ativo).Select(s => s.BlockChain_Id);
                var blockChainBlocksToExclude = blockChainBlocksDatabase.Where(x => blockChainBlocksFront.All(f => x != f)).ToList();

                if (blockChainBlocksToExclude.Any())
                {
                    foreach (var blockChainBlock in entity.BlockChainBlocks.Where(s =>
                                 s.State == (byte)EnumState.ativo &&
                                 blockChainBlocksToExclude.Any(x => x == s.BlockChain_Id && s.Block_Id == block.Id)))
                    {
                        blockChainBlock.State = Convert.ToByte(EnumState.excluido);
                        blockChainBlock.UpdateDate = dateNow;
                        blockChainBlocks.Add(blockChainBlock);
                    }
                }

                foreach (var blockChainBlockFront in block.BlockChainBlocks)
                {
                    if (blockChainBlockFront == null)
                        continue;

                    var blockChainBlockDb = entity.BlockChainBlocks.FirstOrDefault(e =>
                        e.BlockChain_Id.Equals(blockChainBlockFront.BlockChain_Id) &&
                        e.Block_Id.Equals(block.Id) && e.State.Equals((byte)EnumState.ativo));

                    if (blockChainBlockDb != null)
                    {
                        blockChainBlockDb.UpdateDate = DateTime.Now;
                        gestaoAvaliacaoContext.Entry(blockChainBlockDb).State = System.Data.Entity.EntityState.Modified;

                        blockChainBlocks.Add(blockChainBlockDb);
                    }
                    else
                    {
                        blockChainBlockFront.Block_Id = block.Id;
                        blockChainBlocks.Add(blockChainBlockFront);
                    }   
                }

                if (blockChainBlocks.Count > 0)
                    entity.BlockChainBlocks.AddRange(blockChainBlocks);

                #endregion

                entity.Test.UpdateDate = dateNow;

                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public Block SaveKnowLedgeAreaOrder(Block block)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                Block _entity = gestaoAvaliacaoContext.Block.Include("Test").Include("BlockKnowledgeAreas").FirstOrDefault(x => x.Id == block.Id && x.State == (Byte)EnumState.ativo);
                _entity.UpdateDate = dateNow;
                _entity.Test.TestSituation = EnumTestSituation.Pending;

                #region BlockKnowledgeAreas

                List<BlockKnowledgeArea> blockKnowledgeAreas = new List<BlockKnowledgeArea>();

                var blockKnowledgeAreasFront = block.BlockKnowledgeAreas.Select(s => s.KnowledgeArea_Id);
                var blockKnowledgeAreasDatabase = _entity.BlockKnowledgeAreas.Where(s => s.State == (byte)EnumState.ativo).Select(s => s.KnowledgeArea_Id);
                if (blockKnowledgeAreasFront != null && blockKnowledgeAreasDatabase != null)
                {
                    var blockKnowledgeAreaToExclude = blockKnowledgeAreasDatabase.Except(blockKnowledgeAreasFront);
                    if (blockKnowledgeAreaToExclude != null && blockKnowledgeAreaToExclude.Any())
                    {
                        foreach (var blockKnowledgeArea in _entity.BlockKnowledgeAreas.Where(s => s.State == (byte)EnumState.ativo && blockKnowledgeAreaToExclude.Contains(s.KnowledgeArea_Id)))
                        {
                            if (blockKnowledgeArea != null)
                            {
                                blockKnowledgeArea.State = Convert.ToByte(EnumState.excluido);
                                blockKnowledgeArea.UpdateDate = dateNow;
                                blockKnowledgeAreas.Add(blockKnowledgeArea);
                            }
                        }
                    }

                    foreach (BlockKnowledgeArea blockKnowledgeAreaFront in block.BlockKnowledgeAreas)
                    {
                        if (blockKnowledgeAreaFront != null)
                        {
                            BlockKnowledgeArea blockKnowledgeAreaDB = _entity.BlockKnowledgeAreas.FirstOrDefault(e => e.KnowledgeArea_Id.Equals(blockKnowledgeAreaFront.KnowledgeArea_Id) && e.Block_Id.Equals(blockKnowledgeAreaFront.Block_Id) && e.State.Equals((Byte)EnumState.ativo));

                            if (blockKnowledgeAreaDB != null)
                            {
                                blockKnowledgeAreaDB.Order = blockKnowledgeAreaFront.Order;
                                blockKnowledgeAreaDB.UpdateDate = DateTime.Now;
                                gestaoAvaliacaoContext.Entry(blockKnowledgeAreaDB).State = System.Data.Entity.EntityState.Modified;

                                blockKnowledgeAreas.Add(blockKnowledgeAreaDB);
                            }
                            else
                                blockKnowledgeAreas.Add(blockKnowledgeAreaFront);
                        }
                    }
                }

                if (blockKnowledgeAreas != null && blockKnowledgeAreas.Count > 0)
                    _entity.BlockKnowledgeAreas.AddRange(blockKnowledgeAreas);

                #endregion

                gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();

                return block;
            }
        }

        public void RemoveBlockItem(long BlockId, long ItemId)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                BlockItem _entity = GestaoAvaliacaoContext.BlockItem.FirstOrDefault(b => b.Block_Id == BlockId && b.Item_Id == ItemId && b.State == (Byte)EnumState.ativo);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void RemoveBlockItemByBlockId(long blockId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"UPDATE BlockItem set State = @state WHERE Block_Id = @blockId;";

                cn.ExecuteScalar(sql, new { blockId, state = (Byte)EnumState.excluido });
            }
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Block block = GestaoAvaliacaoContext.Block.FirstOrDefault(a => a.Id == id);

                if (block != null)
                {
                    List<BlockItem> blockItems = GestaoAvaliacaoContext.BlockItem.Include("Block").Where(i => i.Block_Id == id).ToList();
                    if (blockItems != null)
                    {
                        blockItems.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    block.State = Convert.ToByte(EnumState.excluido);
                    block.UpdateDate = DateTime.Now;
                    GestaoAvaliacaoContext.Entry(block).State = System.Data.Entity.EntityState.Modified;
                }
            }
        }

        public void DeleteItems(long id)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Block block = GestaoAvaliacaoContext.Block.FirstOrDefault(a => a.Id == id);

                if (block != null)
                {
                    List<BlockItem> blockItems = GestaoAvaliacaoContext.BlockItem.Include("Block").Where(i => i.Block_Id == id).ToList();
                    if (blockItems != null)
                    {
                        blockItems.ForEach(i =>
                        {
                            i.State = Convert.ToByte(EnumState.excluido);
                            i.UpdateDate = DateTime.Now;
                            GestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                        });
                    }

                    block.UpdateDate = DateTime.Now;
                    GestaoAvaliacaoContext.Entry(block).State = System.Data.Entity.EntityState.Modified;
                }
            }
        }

        #endregion
    }
}