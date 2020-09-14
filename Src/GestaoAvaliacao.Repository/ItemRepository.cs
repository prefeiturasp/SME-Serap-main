using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class ItemRepository : ConnectionReadOnly, IItemRepository
    {
        #region Read

        public IEnumerable<Item> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.Item.Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Id));
                }
            }
        }

        public Item _GetMatrixBytem(long itemId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking()
                        .Include("EvaluationMatrix.Discipline")
                        .Include("EvaluationMatrix.ModelEvaluationMatrix")
                        .Include("EvaluationMatrix.Discipline")
                        .Include("KnowledgeArea")
                        .FirstOrDefault(i => i.Id == itemId && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public Item _GetSimpleMatrixBytem(long itemId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking()
                        .Include("EvaluationMatrix")
                        .Include("EvaluationMatrix.Discipline")
                        .FirstOrDefault(i => i.Id == itemId && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public Item _GetBaseTextBytem(long itemId)
        {
            using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
            {
                return ctx.Item.AsNoTracking().Include("BaseText").FirstOrDefault(i => i.Id == itemId && i.State == (byte)EnumState.ativo && i.BaseText.State == (byte)EnumState.ativo);
            }
        }

        public Item _GetItemById(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking()
                        .Include("ItemLevel")
                        .Include("ItemType")
                        .Include("Alternatives")
                        .Include("ItemSituation")
                        .Include("ItemSkills.Skill.ModelSkillLevel")
                        .Include("ItemSkills.Skill.Parent")
                        .Include("ItemCurriculumGrades")
                        .Include("Subsubject")
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public Item _GetItemSummaryById(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking()
                        .Include("BlockItems")
                        .Include("BlockItems.Block")
                        .Include("Alternatives")
                        .Include("ItemSkills.Skill.ModelSkillLevel")
                        .Include("ItemSkills.Skill.Parent")
                        .Include("EvaluationMatrix")
                        .Include("EvaluationMatrix.Discipline")
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public IEnumerable<Item> GetItemSummaryById(List<long> idsTest, List<long> ids)
        {
            StringBuilder sql = new StringBuilder(
                        @"SELECT
	                        I.Id, I.ItemCode, I.[Statement], I.Revoked,(DENSE_RANK() OVER(PARTITION BY B.Id ORDER BY CASE WHEN (T.KnowledgeAreaBlock = 1) THEN ISNULL(Bka.[Order], 0) END, bi.[Order]) - 1) AS [Order],
	                        BI.Id, BI.Item_Id, B.Test_Id AS Test_Id,
                            BT.Id, BT.[Description],
                            A.Id, A.Description, A.Justificative, A.[Order], A.Correct, A.Numeration, A.Item_Id,
                            SI.Id, SI.Item_Id, SI.Skill_Id, 
                            SKI.Id, SKI.Code, SKI.[Description],
	                        D.Id, D.[Description]
                        FROM
	                        Item I WITH(NOLOCK)
	                        INNER JOIN BlockItem BI WITH (NOLOCK) ON BI.Item_Id = I.Id AND BI.State <> 3
	                        INNER JOIN Block B WITH (NOLOCK) ON B.Id = BI.Block_Id AND B.State <> 3
	                        INNER JOIN Test T WITH(NOLOCK) ON T.Id = B.Test_Id AND T.State <> 3
	                        INNER JOIN Alternative A WITH (NOLOCK) ON A.Item_Id = I.Id AND A.State <> 3
	                        INNER JOIN ItemSkill SI WITH (NOLOCK) ON SI.Item_Id = I.Id AND SI.State <> 3
	                        INNER JOIN Skill AS SKI WITH(NOLOCK) ON SKI.Id = SI.Skill_Id AND SKI.State <> 3
	                        INNER JOIN EvaluationMatrix EM WITH (NOLOCK) ON EM.Id = I.EvaluationMatrix_Id AND EM.State <> 3
	                        INNER JOIN Discipline D WITH (NOLOCK) ON D.Id = EM.Discipline_Id AND D.State <> 3                
	                        LEFT JOIN BaseText BT WITH(NOLOCK) ON BT.Id = I.BaseText_Id AND BT.State <> 3
	                        LEFT JOIN BlockKnowledgeArea Bka WITH (NOLOCK) ON Bka.KnowledgeArea_Id = I.KnowledgeArea_Id AND B.Id = Bka.Block_Id AND Bka.State = 1 ");

            sql.AppendFormat("WHERE I.Id IN ({0})", string.Join(",", ids));
            sql.AppendFormat("AND T.Id IN ({0})", string.Join(",", idsTest));

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var lookup = new Dictionary<long, Item>();
                cn.Query<Item, BlockItem, BaseText, Alternative, ItemSkill, Skill, Discipline, Item>(sql.ToString(),
                     (I, BI, BT, A, SI, SKI, D) =>
                     {
                         Item found;
                         if (!lookup.TryGetValue(I.Id, out found))
                         {
                             I.BaseText = BT;
                             I.Alternatives = new List<Alternative>();
                             I.ItemSkills = new List<ItemSkill>();
                             I.BlockItems = new List<BlockItem>();
                             I.EvaluationMatrix = new EvaluationMatrix();
                             I.EvaluationMatrix.Discipline = new Discipline();
                             I.EvaluationMatrix.Discipline.Id = D.Id;
                             I.EvaluationMatrix.Discipline.Description = D.Description;
                             lookup.Add(I.Id, I);

                             found = I;
                         }
                         if (!found.Alternatives.Exists(p => p.Item_Id == A.Item_Id && p.Id == A.Id))
                         {
                             found.Alternatives.Add(A);
                         }
                         if (!found.ItemSkills.Exists(p => p.Item_Id == SI.Item_Id && p.Id == SI.Id))
                         {
                             SI.Skill = SKI;
                             found.ItemSkills.Add(SI);
                         }
                         if (!found.BlockItems.Exists(p => p.Item_Id == BI.Item_Id && p.Id == BI.Id))
                         {
                             found.BlockItems.Add(BI);
                         }
                         return found;
                     }).Distinct().ToDictionary(p => p.Id);

                return lookup.Values;
            }
        }

        public Item _GetGradeByItem(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking()
                        .Include("ItemCurriculumGrades")
                        .FirstOrDefault(i => i.Id == Id && i.State == (Byte)EnumState.ativo);

                    return query;
                }
            }
        }

        public Item _GetAddItemInfos(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    return ctx.Item.AsNoTracking().Include("BaseText").Include("EvaluationMatrix").Include("EvaluationMatrix.Discipline").FirstOrDefault(x => x.Id == Id);
                }
            }
        }

        public IEnumerable<ItemGroupBaseText> _GetItemGroupBaseTexts(bool? lastVersion = null, params long[] baseTextId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                StringBuilder sql = new StringBuilder("SELECT [BaseText_Id] AS BaseTextId, COUNT([Id]) AS Quantidade ");
                sql.Append("FROM Item WITH (NOLOCK) ");
                sql.Append("WHERE  [State] = 1 ");

                if (baseTextId.Length > 0)
                    sql.AppendFormat("AND [BaseText_Id] IN ({0}) ", string.Join(",", baseTextId));

                if (lastVersion.HasValue)
                    sql.Append("AND [LastVersion] = @lastVersion ");

                sql.Append("GROUP BY [BaseText_Id]");

                var d = new DynamicParameters();

                if (lastVersion.HasValue)
                    d.AddDynamicParams(new { lastVersion = true });

                var query = cn.Query<ItemGroupBaseText>(sql.ToString(), d);
                return query.ToList();
            }
        }

        public IEnumerable<Item> _GetItemVersions(int itemCodeVersion)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.Include("BlockItems").Include("BlockItems.Block").Include("BlockItems.Block.Test").AsNoTracking().Where(i => i.ItemCodeVersion == itemCodeVersion && i.State == (Byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Item> GetItemVersions(int itemCodeVersion, int itemVersion)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query = ctx.Item.AsNoTracking().Include("BaseText").Include("ItemSkills").Where(i => i.ItemCodeVersion == itemCodeVersion && i.State == (Byte)EnumState.ativo && i.ItemVersion != itemVersion).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Item> _GetItemsByBaseText(long baseTextId)
        {
            var sql = @"SELECT i.[Id],i.[ItemCode],i.[ItemVersion],i.[Statement],i.[Keywords],i.[Tips],i.[TRIDiscrimination]," +
                        "i.[TRIDifficulty],i.[TRICasualSetting],i.[CreateDate],i.[UpdateDate],i.[State],i.[BaseText_Id]," +
                        "i.[EvaluationMatrix_Id],i.[ItemLevel_Id],i.[ItemSituation_Id],i.[ItemType_Id],i.[proficiency],i.[descriptorSentence]," +
                        "i.[LastVersion],i.[IsRestrict],i.[ItemNarrated],i.[StudentStatement],i.[NarrationStudentStatement],i.[NarrationAlternatives], i.[KnowledgeArea_Id], i.[SubSubject_Id], " +
                        "ist.[Id],ist.[Description],ist.[CreateDate],ist.[UpdateDate],ist.[State],ist.[EntityId],ist.[AllowVersion]" +
                        "FROM [dbo].[Item] i " +
                        "INNER JOIN ItemSituation ist on i.ItemSituation_Id = ist.Id " +
                        "WHERE BaseText_Id = @id ";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var items = cn.Query<Item, ItemSituation, Item>(sql, (i, ist) => { i.ItemSituation = ist; return i; }, new { id = baseTextId });

                return items;
            }
        }

        public int GetMaxCode()
        {
            var sql = "SELECT ISNULL(MAX(ItemCodeVersion), 0) FROM Item";
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<int>(sql);
            }
        }

        public bool VerifyItemCodeAlreadyExists(string itemCode, long? itemId = null)
        {
            var sql = @"SELECT CASE WHEN EXISTS (
							SELECT*
							FROM Item
							WHERE UPPER(LTRIM(RTRIM(ItemCode))) = @ItemCode
							AND (@itemId IS NULL OR (@itemId IS NOT NULL AND @itemId <> Id))
							AND LastVersion = 1
						)
						THEN CAST(1 AS BIT)
						ELSE CAST(0 AS BIT) END";

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<bool>(sql, new { itemCode = itemCode.Trim().ToUpper(), itemId = itemId });
            }
        }

        public int GetMaxVersionByItemCode(int itemCodeVersion)
        {
            var sql = "SELECT ISNULL(MAX(ItemVersion), 0) FROM Item WHERE ItemCodeVersion = @itemcode";
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<int>(sql, new { itemcode = itemCodeVersion });
            }
        }

        public bool ExistsItemBlock(long Id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    Item item = GestaoAvaliacaoContext.Item.Include("BlockItems").FirstOrDefault(i => i.Id == Id);
                    return item.BlockItems.Any();
                }
            }
        }

        public IEnumerable<Item> GetItems(List<long> ItemIds)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.Item.AsNoTracking().Where(x => ItemIds.Contains(x.Id)).AsQueryable();
                }
            }
        }

        public Item Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT i.Id,i.ItemCode, i.ItemCodeVersion, i.ItemVersion,i.Statement,i.Keywords,i.Tips,i.TRIDiscrimination,i.TRIDifficulty," +
                            "i.TRICasualSetting,i.CreateDate,i.UpdateDate,i.State,i.BaseText_Id,i.EvaluationMatrix_Id,i.ItemLevel_Id,i.ItemSituation_Id, " +
                            "i.ItemType_Id,i.proficiency,i.descriptorSentence,i.LastVersion,i.IsRestrict,i.ItemNarrated,i.StudentStatement,i.NarrationStudentStatement,i.NarrationAlternatives " +
                            "FROM Item i " +
                            "WHERE i.Id = @id " +
                            "SELECT [Id],[Description],[Correct],[Order],[Justificative],[CreateDate],[UpdateDate],[State],[Item_Id], " +
                            "[numeration],[TCTBiserialCoefficient],[TCTDificulty],[TCTDiscrimination] " +
                            "FROM Alternative " +
                            "WHERE Item_Id = @id " +
                            "SELECT [Id], [CreateDate], [UpdateDate], [State], [Item_Id], [TypeCurriculumGradeId] " +
                            "FROM ItemCurriculumGrade " +
                            "WHERE Item_Id = @id " +
                            "SELECT isk.Id,isk.OriginalSkill,isk.CreateDate,isk.UpdateDate,isk.State,isk.Item_Id,isk.Skill_Id " +
                            "FROM ItemSkill isk " +
                            "INNER JOIN Skill s ON s.Id = isk.Skill_Id " +
                            "WHERE isk.Item_Id = @id";

                var query = cn.QueryMultiple(sql, new { id = id });
                var item = query.Read<Item>().FirstOrDefault();

                item.Alternatives = query.Read<Alternative>().ToList();
                item.ItemCurriculumGrades = query.Read<ItemCurriculumGrade>().ToList();
                item.ItemSkills = query.Read<ItemSkill>().ToList();


                return item;
            }
        }

        public Item GetToPreview(long id)
        {
            StringBuilder sql = new StringBuilder("SELECT i.Id,i.ItemCode,i.ItemVersion,i.Statement,i.Keywords,i.Tips,i.TRIDiscrimination,i.TRIDifficulty, ");
            sql.Append("i.TRICasualSetting,i.CreateDate,i.UpdateDate,i.State,i.BaseText_Id,i.EvaluationMatrix_Id,i.ItemLevel_Id,i.ItemSituation_Id, ");
            sql.Append("i.ItemType_Id,i.proficiency,i.descriptorSentence,i.LastVersion,i.IsRestrict,i.ItemNarrated,i.StudentStatement,i.NarrationStudentStatement,i.NarrationAlternatives, ");
            sql.Append("a.Id,a.Description,a.Correct,a.[Order],a.Justificative,a.CreateDate,a.UpdateDate,a.State,a.Item_Id, ");
            sql.Append("a.numeration,a.TCTBiserialCoefficient,a.TCTDificulty,a.TCTDiscrimination, ");
            sql.Append("b.Id,b.Description,b.CreateDate,b.UpdateDate,b.State,b.Source,b.InitialOrientation,b.InitialStatement,b.NarrationInitialStatement,b.StudentBaseText,b.NarrationStudentBaseText,b.BaseTextOrientation ");
            sql.Append("FROM Item i ");
            sql.Append("INNER JOIN Alternative a on i.Id = a.Item_Id ");
            sql.Append("LEFT JOIN BaseText b on i.BaseText_Id = b.Id ");
            sql.Append("WHERE i.Id = @id AND i.State = @state AND b.State = @state AND a.State = @state ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                Item retorno = null;

                cn.Query<Item, Alternative, BaseText, Item>(sql.ToString(),
                    (Item, alternative, baseText) =>
                    {
                        if (retorno == null)
                        {
                            retorno = Item;
                            retorno.BaseText = baseText;
                        }
                        retorno.Alternatives.Add(alternative);

                        return Item;
                    }, new { id = id, state = (byte)EnumState.ativo });

                return retorno;
            }
        }

        public IEnumerable<Item> GetToPreviewByBaseText(long id)
        {
            StringBuilder sql = new StringBuilder("SELECT i.Id, i.ItemCode, i.ItemVersion, i.Statement, i.Keywords, i.Tips, i.TRIDiscrimination, i.TRIDifficulty, ");
            sql.Append("i.TRICasualSetting, i.CreateDate, i.UpdateDate, i.State, i.BaseText_Id, i.EvaluationMatrix_Id, ");
            sql.Append("i.ItemLevel_Id, i.ItemSituation_Id, i.ItemType_Id, i.proficiency, i.descriptorSentence, i.LastVersion, i.IsRestrict, i.ItemNarrated,i.StudentStatement,i.NarrationStudentStatement,i.NarrationAlternatives, ");
            sql.Append("a.Id, a.Description, a.Correct, a.[Order], a.Justificative, a.CreateDate, a.UpdateDate, a.State, a.Item_Id, a.numeration, a.TCTBiserialCoefficient, a.TCTDificulty, ");
            sql.Append("a.TCTDiscrimination, b.Id, b.Description, b.CreateDate, b.UpdateDate, b.State, b.Source, b.InitialOrientation,b.InitialStatement,b.NarrationInitialStatement,b.StudentBaseText,b.NarrationStudentBaseText,b.BaseTextOrientation ");
            sql.Append("FROM Item AS i INNER JOIN ");
            sql.Append("Alternative AS a ON i.Id = a.Item_Id LEFT JOIN ");
            sql.Append("BaseText AS b ON i.BaseText_Id = b.Id ");
            sql.Append("WHERE i.BaseText_Id = @id AND i.LastVersion = 1");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var lookup = new Dictionary<long, Item>();
                cn.Query<Item, Alternative, BaseText, Item>(sql.ToString(),
                     (item, alternative, baseText) =>
                     {
                         Item found;
                         if (!lookup.TryGetValue(item.Id, out found))
                         {
                             item.BaseText = baseText;
                             item.Alternatives = new List<Alternative>();
                             lookup.Add(item.Id, item);

                             found = item;
                         }
                         found.Alternatives.Add(alternative);
                         return found;
                     }, new { id = id }).Distinct().ToDictionary(p => p.Id);

                return lookup.Values;
            }
        }

        public IEnumerable<ItemResult> _SearchItems(ItemFilter filter, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    #region Params

                    SqlParameter pItemCode = new SqlParameter("@ItemCode", filter.ItemCode ?? Convert.DBNull);
                    pItemCode.SqlDbType = SqlDbType.VarChar;
                    pItemCode.IsNullable = true;

                    SqlParameter pRevoked = new SqlParameter("@Revoked", filter.Revoked ?? Convert.DBNull);
                    pRevoked.SqlDbType = SqlDbType.Bit;
                    pRevoked.IsNullable = true;

                    SqlParameter pItemSituation = new SqlParameter("@ItemSituation", SqlDbType.VarChar, -1);
                    pItemSituation.Value = filter.ItemSituation ?? Convert.DBNull;
                    pItemSituation.IsNullable = true;

                    SqlParameter pShowVersion = new SqlParameter("@ShowVersion", filter.ShowVersion);
                    pShowVersion.SqlDbType = SqlDbType.Bit;

                    SqlParameter pProficiencyStart = new SqlParameter("@ProficiencyStart", filter.ProficiencyStart ?? Convert.DBNull);
                    pProficiencyStart.SqlDbType = SqlDbType.Int;
                    pProficiencyStart.IsNullable = true;

                    SqlParameter pProficiencyEnd = new SqlParameter("@ProficiencyEnd", filter.ProficiencyEnd ?? Convert.DBNull);
                    pProficiencyEnd.SqlDbType = SqlDbType.Int;
                    pProficiencyEnd.IsNullable = true;

                    SqlParameter pKeywords = new SqlParameter("@Keywords", SqlDbType.VarChar, -1);
                    pKeywords.Value = filter.Keywords ?? Convert.DBNull;
                    pKeywords.IsNullable = true;

                    SqlParameter pDisciplineId = new SqlParameter("@DisciplineId", filter.DisciplineId ?? Convert.DBNull);
                    pDisciplineId.SqlDbType = SqlDbType.BigInt;
                    pDisciplineId.IsNullable = true;

                    SqlParameter pEvaluationMatrixId = new SqlParameter("@EvaluationMatrixId", filter.EvaluationMatrixId ?? Convert.DBNull);
                    pEvaluationMatrixId.SqlDbType = SqlDbType.BigInt;
                    pEvaluationMatrixId.IsNullable = true;

                    SqlParameter pShowItemNarrated = new SqlParameter("@ShowItemNarrated", filter.ShowItemNarrated);
                    pShowItemNarrated.SqlDbType = SqlDbType.Bit;

                    SqlParameter pSkills = new SqlParameter("@Skills", SqlDbType.VarChar, -1);
                    pSkills.Value = filter.SkillId ?? Convert.DBNull;
                    if (!string.IsNullOrEmpty(filter.CorrelatedSkills))
                        pSkills.Value = filter.CorrelatedSkills;
                    pSkills.IsNullable = true;

                    SqlParameter pTypeCurriculumGrades = new SqlParameter("@TypeCurriculumGrades", SqlDbType.VarChar, -1);
                    pTypeCurriculumGrades.Value = filter.TypeCurriculumGrades ?? Convert.DBNull;
                    pTypeCurriculumGrades.IsNullable = true;

                    SqlParameter pPageSize = new SqlParameter("@pageSize", pager.PageSize);
                    pPageSize.SqlDbType = SqlDbType.Int;

                    SqlParameter pPageNumber = new SqlParameter("@pageNumber", pager.CurrentPage);
                    pPageNumber.SqlDbType = SqlDbType.Int;

                    SqlParameter pTotalRecords = new SqlParameter("@totalRecords", 0);
                    pTotalRecords.SqlDbType = SqlDbType.Int;
                    pTotalRecords.Direction = ParameterDirection.Output;

                    #endregion

                    var ret = GestaoAvaliacaoContext.Database.SqlQuery<ItemResult>(@"MS_Item_SearchFiltered @ItemCode, @Revoked, 
						@ItemSituation, @ShowVersion, @ProficiencyStart, @ProficiencyEnd, @Keywords, @DisciplineId, @EvaluationMatrixId, @ShowItemNarrated, @Skills, @TypeCurriculumGrades, @pageSize, @pageNumber, @totalRecords OUT",
                    pItemCode, pRevoked, pItemSituation, pShowVersion, pProficiencyStart, pProficiencyEnd, pKeywords, pDisciplineId, pEvaluationMatrixId, pShowItemNarrated, pSkills, pTypeCurriculumGrades, pPageSize, pPageNumber, pTotalRecords).ToList<ItemResult>();

                    pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(pTotalRecords.Value) / (double)pager.PageSize));
                    pager.SetTotalItens(Convert.ToInt32(pTotalRecords.Value));

                    return ret;
                }
            }
        }

        public IEnumerable<ItemBlockResult> _BlockSearchItem(BlockItemFilter filter, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    #region Params

                    SqlParameter pItemCode = new SqlParameter("@ItemCode", filter.ItemCode ?? Convert.DBNull);
                    pItemCode.SqlDbType = SqlDbType.VarChar;
                    pItemCode.IsNullable = true;

                    SqlParameter pProficiencyStart = new SqlParameter("@ProficiencyStart", filter.ProficiencyStart ?? Convert.DBNull);
                    pProficiencyStart.SqlDbType = SqlDbType.Int;
                    pProficiencyStart.IsNullable = true;

                    SqlParameter pProficiencyEnd = new SqlParameter("@ProficiencyEnd", filter.ProficiencyEnd ?? Convert.DBNull);
                    pProficiencyEnd.SqlDbType = SqlDbType.Int;
                    pProficiencyEnd.IsNullable = true;

                    SqlParameter pKeywords = new SqlParameter("@Keywords", SqlDbType.VarChar, -1);
                    pKeywords.Value = filter.Keywords ?? Convert.DBNull;
                    pKeywords.IsNullable = true;

                    SqlParameter pDisciplineId = new SqlParameter("@DisciplineId", filter.DisciplineId ?? Convert.DBNull);
                    pDisciplineId.SqlDbType = SqlDbType.BigInt;
                    pDisciplineId.IsNullable = true;

                    SqlParameter pEvaluationMatrixId = new SqlParameter("@EvaluationMatrixId", filter.EvaluationMatrixId ?? Convert.DBNull);
                    pEvaluationMatrixId.SqlDbType = SqlDbType.BigInt;
                    pEvaluationMatrixId.IsNullable = true;

                    SqlParameter pSkills = new SqlParameter("@Skills", SqlDbType.VarChar, -1);
                    pSkills.Value = filter.SkillId ?? Convert.DBNull;
                    if (!string.IsNullOrEmpty(filter.CorrelatedSkills))
                        pSkills.Value = filter.CorrelatedSkills;
                    pSkills.IsNullable = true;

                    SqlParameter pILevels = new SqlParameter("@ItemLevel", SqlDbType.VarChar, -1);
                    pILevels.Value = filter.ItemLevelIds ?? Convert.DBNull;

                    SqlParameter pTypeCurriculumGrades = new SqlParameter("@TypeCurriculumGrades", SqlDbType.VarChar, -1);
                    pTypeCurriculumGrades.Value = filter.TypeCurriculumGrades ?? Convert.DBNull;
                    pTypeCurriculumGrades.IsNullable = true;

                    SqlParameter pGlobal = new SqlParameter("@Global", filter.Global ?? Convert.DBNull);
                    pGlobal.SqlDbType = SqlDbType.Bit;
                    pGlobal.IsNullable = true;

                    SqlParameter pPageSize = new SqlParameter("@pageSize", pager.PageSize);
                    pPageSize.SqlDbType = SqlDbType.Int;


                    SqlParameter pItemTypeId = new SqlParameter("@ItemTypeId", filter.ItemType ?? Convert.DBNull);
                    pEvaluationMatrixId.SqlDbType = SqlDbType.BigInt;
                    pEvaluationMatrixId.IsNullable = true;

                    SqlParameter pPageNumber = new SqlParameter("@pageNumber", pager.CurrentPage);
                    pPageNumber.SqlDbType = SqlDbType.Int;

                    SqlParameter pTotalRecords = new SqlParameter("@totalRecords", 0);
                    pTotalRecords.SqlDbType = SqlDbType.Int;
                    pTotalRecords.Direction = ParameterDirection.Output;

                    #endregion

                    var ret = gestaoAvaliacaoContext.Database.SqlQuery<ItemBlockResult>(@"MS_Block_SearchItems 
						@ItemCode, @ProficiencyStart, @ProficiencyEnd, @Keywords, @DisciplineId, 
						@EvaluationMatrixId, @Skills, @ItemLevel, @TypeCurriculumGrades, @Global, @pageSize, @pageNumber, @ItemTypeId, @totalRecords OUT",
                    pItemCode, pProficiencyStart, pProficiencyEnd, pKeywords, pDisciplineId,
                    pEvaluationMatrixId, pSkills, pILevels, pTypeCurriculumGrades, pGlobal, pPageSize, pPageNumber, pItemTypeId, pTotalRecords).ToList<ItemBlockResult>();

                    pager.SetTotalPages((int)Math.Ceiling(Convert.ToInt32(pTotalRecords.Value) / (double)pager.PageSize));
                    pager.SetTotalItens(Convert.ToInt32(pTotalRecords.Value));

                    return ret;
                }
            }
        }

        #region ItemReport

        public List<ItemReportItemType> _GetItemType(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (Id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = Id;
                    }

                    SqlParameter Situacao = new SqlParameter("@Situacao", situacao);
                    Situacao.SqlDbType = SqlDbType.Int;

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter pTypeLevelEducation = new SqlParameter();
                    pTypeLevelEducation.SqlDbType = SqlDbType.Int;
                    pTypeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (typeLevelEducation == 0)
                        pTypeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        pTypeLevelEducation.Value = typeLevelEducation;
                    }

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemType>(
                            "EXEC MS_Item_ReportItemType @disciplina, @Situacao, @EntityId, @typeLevelEducation", disciplina, Situacao, entityId, pTypeLevelEducation).ToList<ItemReportItemType>();
                    return myEntities;
                }
            }
        }

        public List<ItemReportItemLevel> _GetItemLevel(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (Id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = Id;
                    }

                    SqlParameter Situacao = new SqlParameter("@Situacao", situacao);
                    Situacao.SqlDbType = SqlDbType.Int;

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter pTypeLevelEducation = new SqlParameter();
                    pTypeLevelEducation.SqlDbType = SqlDbType.Int;
                    pTypeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (typeLevelEducation == 0)
                        pTypeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        pTypeLevelEducation.Value = typeLevelEducation;
                    }

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemLevel>(
                            "EXEC MS_Item_ReportItemLevel @disciplina, @Situacao, @EntityId, @typeLevelEducation", disciplina, Situacao, entityId, pTypeLevelEducation).ToList<ItemReportItemLevel>();
                    return myEntities;
                }
            }
        }

        public List<ItemReportItem> _GetItem(Guid EntityId, long typeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter pTypeLevelEducation = new SqlParameter();
                    pTypeLevelEducation.SqlDbType = SqlDbType.Int;
                    pTypeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (typeLevelEducation == 0)
                        pTypeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        pTypeLevelEducation.Value = typeLevelEducation;
                    }

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItem>(
                            "EXEC MS_Item_ReportItem @EntityId, @typeLevelEducation", entityId, pTypeLevelEducation).ToList<ItemReportItem>();
                    return myEntities;
                }
            }
        }

        public List<ItemReportItemCurriculumGrade> _GetItemCurriculumGrade(int Id, int situacao, Guid EntityId, long typeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (Id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = Id;
                    }

                    SqlParameter Situacao = new SqlParameter("@Situacao", situacao);
                    Situacao.SqlDbType = SqlDbType.Int;

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter pTypeLevelEducation = new SqlParameter();
                    pTypeLevelEducation.SqlDbType = SqlDbType.Int;
                    pTypeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (typeLevelEducation == 0)
                        pTypeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        pTypeLevelEducation.Value = typeLevelEducation;
                    }
                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemCurriculumGrade>(
                            "EXEC MS_Item_ReportItemCurriculumGrade @disciplina, @Situacao, @EntityId, @typeLevelEducation", disciplina, Situacao, entityId, pTypeLevelEducation).ToList<ItemReportItemCurriculumGrade>();
                    return myEntities;
                }
            }
        }

        public List<ItemReportItemSituation> _GetItemSituation(int Id, string inicio, string fim, Guid EntityId, long typeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (Id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = Id;
                    }

                    SqlParameter Inicio = new SqlParameter();
                    Inicio.SqlDbType = SqlDbType.VarChar;
                    Inicio.ParameterName = "@Inicio";

                    if (inicio == null)
                        Inicio.Value = DBNull.Value;
                    else
                    {
                        Inicio.Value = inicio;
                    }

                    SqlParameter Fim = new SqlParameter();
                    Fim.SqlDbType = SqlDbType.VarChar;
                    Fim.ParameterName = "@Fim";

                    if (fim == null)
                        Fim.Value = DBNull.Value;
                    else
                    {
                        Fim.Value = fim;
                    }

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter pTypeLevelEducation = new SqlParameter();
                    pTypeLevelEducation.SqlDbType = SqlDbType.Int;
                    pTypeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (typeLevelEducation == 0)
                        pTypeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        pTypeLevelEducation.Value = typeLevelEducation;
                    }
                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemSituation>(
                            "EXEC MS_Item_ReportItemSituation @disciplina, @Inicio, @Fim, @entityId, @typeLevelEducation", disciplina, Inicio, Fim, entityId, pTypeLevelEducation).ToList<ItemReportItemSituation>();
                    return myEntities;
                }
            }
        }

        #endregion

        #endregion

        #region Write

        public Item Save(Item entity)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;

                List<ItemFile> newItemFile = new List<ItemFile>();

                foreach (var videoModel in entity.ItemFiles)
                {
                    var newVideo = new ItemFile
                    {
                        State = (Byte)EnumState.ativo,
                        CreateDate = dateNow,
                        UpdateDate = dateNow,
                        File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.File.Id),
                        Thumbnail = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.Thumbnail.Id)
                    };
                    newItemFile.Add(newVideo);
                }

                entity.ItemFiles.RemoveAll(p => p.File != null);
                entity.ItemFiles.AddRange(newItemFile);

                List<ItemAudio> newItemAudio = new List<ItemAudio>();

                foreach (var audioModel in entity.ItemAudios)
                {
                    var newAudio = new ItemAudio
                    {
                        State = (Byte)EnumState.ativo,
                        CreateDate = dateNow,
                        UpdateDate = dateNow,
                        File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == audioModel.File.Id)
                    };
                    newItemAudio.Add(newAudio);
                }

                entity.ItemAudios.RemoveAll(p => p.File != null);
                entity.ItemAudios.AddRange(newItemAudio);

                GestaoAvaliacaoContext.Item.Add(entity);

                var itemsToChangeItemFiles = GestaoAvaliacaoContext.Item.Include("ItemFiles").Where(x => x.ItemCodeVersion == entity.ItemCodeVersion && x.Id != entity.Id).ToList();

                foreach (var entidade in itemsToChangeItemFiles.ToList())
                {
                    foreach (var existingVideo in entidade.ItemFiles.ToList())
                    {
                        if (!entity.ItemFiles.Any(c => c.Id == existingVideo.Id))
                        {
                            existingVideo.State = (Byte)EnumState.excluido;
                            existingVideo.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingVideo);
                        }
                    }

                    List<ItemFile> newItemFileVersions = new List<ItemFile>();

                    foreach (var videoModel in entity.ItemFiles.Where(p => p.File != null))
                    {
                        var existingChild = entidade.ItemFiles.SingleOrDefault(c => c.Id == videoModel.Id);

                        if (existingChild != null)
                        {
                            existingChild.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemFileVersions);
                        }
                        else
                        {
                            var newVideo = new ItemFile
                            {
                                State = (Byte)EnumState.ativo,
                                CreateDate = dateNow,
                                UpdateDate = dateNow,
                                File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.File.Id),
                                Thumbnail = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.Thumbnail.Id)
                            };
                            newItemFileVersions.Add(newVideo);
                        }
                    }
                    entidade.ItemFiles.AddRange(newItemFileVersions);

                    GestaoAvaliacaoContext.Entry(entidade).State = System.Data.Entity.EntityState.Modified;
                }

                var itemsToChangeItemAudio = GestaoAvaliacaoContext.Item.Include("ItemAudios").Where(x => x.ItemCodeVersion == entity.ItemCodeVersion && x.Id != entity.Id).ToList();

                foreach (var entidade in itemsToChangeItemAudio.ToList())
                {
                    foreach (var existingAudio in entidade.ItemAudios.ToList())
                    {
                        if (!entity.ItemFiles.Any(c => c.Id == existingAudio.Id))
                        {
                            existingAudio.State = (Byte)EnumState.excluido;
                            existingAudio.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingAudio);
                        }
                    }

                    List<ItemAudio> newItemAudioVersions = new List<ItemAudio>();

                    foreach (var audioModel in entity.ItemAudios.Where(p => p.File != null))
                    {
                        var existingChild = entidade.ItemAudios.SingleOrDefault(c => c.Id == audioModel.Id);

                        if (existingChild != null)
                        {
                            existingChild.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemAudioVersions);
                        }
                        else
                        {
                            var newAudio = new ItemAudio
                            {
                                State = (Byte)EnumState.ativo,
                                CreateDate = dateNow,
                                UpdateDate = dateNow,
                                File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == audioModel.File.Id)
                            };
                            newItemAudioVersions.Add(newAudio);
                        }
                    }
                    entidade.ItemAudios.AddRange(newItemAudioVersions);

                    GestaoAvaliacaoContext.Entry(entidade).State = System.Data.Entity.EntityState.Modified;
                }

                #region Caso já tenha sido versionado altera o itemCode dos outros registros
                var itemsToChangeItemCode = GestaoAvaliacaoContext.Item.Where(x => x.ItemCodeVersion == entity.ItemCodeVersion).ToList();

                if (itemsToChangeItemCode.Any())
                    itemsToChangeItemCode.ForEach(x =>
                    {
                        x.ItemCode = entity.ItemCode;
                        GestaoAvaliacaoContext.Entry(x).State = System.Data.Entity.EntityState.Modified;
                    });
                #endregion

                GestaoAvaliacaoContext.SaveChanges();

                return entity;
            }
        }

        public Item Update(Item entity)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                Item _entity = GestaoAvaliacaoContext.Item.Include("ItemSkills.Skill").Include("Alternatives").Include("ItemCurriculumGrades").Include("ItemFiles").Include("ItemAudios").FirstOrDefault(a => a.Id == entity.Id);

                _entity.Keywords = entity.Keywords;
                _entity.ItemCode = entity.ItemCode;
                _entity.IsRestrict = entity.IsRestrict;
                _entity.Statement = entity.Statement;
                _entity.TRICasualSetting = entity.TRICasualSetting;
                _entity.TRIDifficulty = entity.TRIDifficulty;
                _entity.TRIDiscrimination = entity.TRIDiscrimination;
                _entity.Tips = entity.Tips;
                _entity.proficiency = entity.proficiency;
                _entity.descriptorSentence = entity.descriptorSentence;
                _entity.ItemNarrated = entity.ItemNarrated;
                _entity.StudentStatement = entity.StudentStatement;
                _entity.NarrationStudentStatement = entity.NarrationStudentStatement;
                _entity.NarrationAlternatives = entity.NarrationAlternatives;
                _entity.SubSubject_Id = entity.SubSubject_Id;
                _entity.KnowledgeArea_Id = entity.KnowledgeArea_Id;

                _entity.UpdateDate = dateNow;
                _entity.State = Convert.ToByte(EnumState.ativo);

                foreach (var existingVideo in _entity.ItemFiles.ToList())
                {
                    if (!entity.ItemFiles.Any(c => c.Id == existingVideo.Id))
                    {
                        existingVideo.State = (Byte)EnumState.excluido;
                        existingVideo.UpdateDate = dateNow;
                        GestaoAvaliacaoContext.Entry(existingVideo);
                    }
                }

                List<ItemFile> newItemFile = new List<ItemFile>();

                foreach (var videoModel in entity.ItemFiles)
                {
                    var existingChild = _entity.ItemFiles.SingleOrDefault(c => c.Id == videoModel.Id);

                    if (existingChild != null)
                    {
                        existingChild.UpdateDate = dateNow;
                        GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemFile);
                    }
                    else
                    {
                        var newVideo = new ItemFile
                        {
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow,
                            File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.File.Id),
                            Thumbnail = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.Thumbnail.Id),
                            ConvertedFile = videoModel.ConvertedFile?.Id > 0 ? GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.ConvertedFile.Id) : null
                        };
                        newItemFile.Add(newVideo);
                    }
                }
                _entity.ItemFiles.AddRange(newItemFile);

                var itemsToChangeItemFiles = GestaoAvaliacaoContext.Item.Include("ItemFiles").Where(x => x.ItemCodeVersion == entity.ItemCodeVersion && x.Id != _entity.Id).ToList();

                foreach (var entidade in itemsToChangeItemFiles.ToList())
                {
                    foreach (var existingVideo in entidade.ItemFiles.ToList())
                    {
                        if (!entity.ItemFiles.Any(c => c.Id == existingVideo.Id))
                        {
                            existingVideo.State = (Byte)EnumState.excluido;
                            existingVideo.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingVideo);
                        }
                    }

                    List<ItemFile> newItemFileVersions = new List<ItemFile>();

                    foreach (var videoModel in entity.ItemFiles)
                    {
                        var existingChild = entidade.ItemFiles.SingleOrDefault(c => c.Id == videoModel.Id);

                        if (existingChild != null)
                        {
                            existingChild.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemFileVersions);
                        }
                        else
                        {
                            var newVideo = new ItemFile
                            {
                                State = (Byte)EnumState.ativo,
                                CreateDate = dateNow,
                                UpdateDate = dateNow,
                                File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.File.Id),
                                Thumbnail = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == videoModel.Thumbnail.Id)
                            };
                            newItemFileVersions.Add(newVideo);
                        }
                    }
                    entidade.ItemFiles.AddRange(newItemFileVersions);

                    GestaoAvaliacaoContext.Entry(entidade).State = System.Data.Entity.EntityState.Modified;
                }

                foreach (var existingAudio in _entity.ItemAudios.ToList())
                {
                    if (!entity.ItemAudios.Any(c => c.Id == existingAudio.Id))
                    {
                        existingAudio.State = (Byte)EnumState.excluido;
                        existingAudio.UpdateDate = dateNow;
                        GestaoAvaliacaoContext.Entry(existingAudio);
                    }
                }

                List<ItemAudio> newItemAudio = new List<ItemAudio>();

                foreach (var audioModel in entity.ItemAudios)
                {
                    var existingChild = _entity.ItemAudios.SingleOrDefault(c => c.Id == audioModel.Id);

                    if (existingChild != null)
                    {
                        existingChild.UpdateDate = dateNow;
                        GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemAudio);
                    }
                    else
                    {
                        var newAudio = new ItemAudio
                        {
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow,
                            File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == audioModel.File.Id)
                        };
                        newItemAudio.Add(newAudio);
                    }
                }
                _entity.ItemAudios.AddRange(newItemAudio);

                var itemsToChangeItemAudios = GestaoAvaliacaoContext.Item.Include("ItemAudios").Where(x => x.ItemCodeVersion == entity.ItemCodeVersion && x.Id != _entity.Id).ToList();

                foreach (var entidade in itemsToChangeItemAudios.ToList())
                {
                    foreach (var existingAudio in entidade.ItemAudios.ToList())
                    {
                        if (!entity.ItemFiles.Any(c => c.Id == existingAudio.Id))
                        {
                            existingAudio.State = (Byte)EnumState.excluido;
                            existingAudio.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingAudio);
                        }
                    }

                    List<ItemAudio> newItemAudioVersions = new List<ItemAudio>();

                    foreach (var audioModel in entity.ItemAudios)
                    {
                        var existingChild = entidade.ItemAudios.SingleOrDefault(c => c.Id == audioModel.Id);

                        if (existingChild != null)
                        {
                            existingChild.UpdateDate = dateNow;
                            GestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(newItemAudioVersions);
                        }
                        else
                        {
                            var newAudio = new ItemAudio
                            {
                                State = (Byte)EnumState.ativo,
                                CreateDate = dateNow,
                                UpdateDate = dateNow,
                                File = GestaoAvaliacaoContext.File.FirstOrDefault(s => s.Id == audioModel.File.Id)
                            };
                            newItemAudioVersions.Add(newAudio);
                        }
                    }
                    entidade.ItemAudios.AddRange(newItemAudioVersions);

                    GestaoAvaliacaoContext.Entry(entidade).State = System.Data.Entity.EntityState.Modified;
                }

                #region Alternative

                List<Alternative> newAlternatives = new List<Alternative>();
                foreach (Alternative alternative in entity.Alternatives)
                {
                    if (alternative != null)
                    {
                        var alt = _entity.Alternatives.FirstOrDefault(a => a.Id == alternative.Id);

                        if (alt != null)
                        {
                            alt.Correct = alternative.Correct;
                            alt.Description = alternative.Description;
                            alt.Justificative = alternative.Justificative;
                            alt.Order = alternative.Order;
                            alt.Numeration = alternative.Numeration;
                            alt.UpdateDate = dateNow;
                            alt.TCTDiscrimination = alternative.TCTDiscrimination;
                            alt.TCTDificulty = alternative.TCTDificulty;
                            alt.TCTBiserialCoefficient = alternative.TCTBiserialCoefficient;
                            alt.State = alternative.State;
                        }
                        else
                            newAlternatives.Add(alternative);
                    }
                }

                _entity.Alternatives.AddRange(newAlternatives);

                #endregion

                #region Skill

                List<ItemSkill> skills = new List<ItemSkill>();

                var skillsFront = entity.ItemSkills.Select(s => s.Skill_Id);
                var skillsDatabase = _entity.ItemSkills.Where(s => s.State == (Byte)EnumState.ativo).Select(s => s.Skill_Id);
                if (skillsFront != null && skillsFront.Any() && skillsDatabase != null && skillsDatabase.Any())
                {
                    var skillsToExclude = skillsDatabase.Except(skillsFront);
                    var skillsToAdd = skillsFront.Except(skillsDatabase);
                    if (skillsToExclude != null && skillsToExclude.Any())
                    {
                        foreach (var itemSkill in _entity.ItemSkills.Where(s => s.State == (Byte)EnumState.ativo && skillsToExclude.Contains(s.Skill.Id)))
                        {
                            if (itemSkill != null)
                            {
                                itemSkill.State = Convert.ToByte(EnumState.excluido);
                                itemSkill.UpdateDate = dateNow;
                                skills.Add(itemSkill);
                            }
                        }
                    }

                    if (skillsToAdd != null && skillsToAdd.Any())
                    {
                        foreach (ItemSkill skill in entity.ItemSkills.Where(s => skillsToAdd.Contains(s.Skill_Id)))
                        {
                            if (skill != null)
                            {
                                skill.Skill = GestaoAvaliacaoContext.Skill.FirstOrDefault(s => s.Id == skill.Skill_Id);
                                skills.Add(skill);
                            }
                        }
                    }
                }

                if (skills != null && skills.Count > 0)
                    _entity.ItemSkills.AddRange(skills);

                #endregion

                #region ItemCurriculumGrade

                List<ItemCurriculumGrade> itemCurriculumGrades = new List<ItemCurriculumGrade>();



                foreach (ItemCurriculumGrade itemCurriculumGrade in entity.ItemCurriculumGrades)
                {
                    ItemCurriculumGrade gradeDatabase = _entity.ItemCurriculumGrades.FirstOrDefault(g => g.State == (Byte)EnumState.ativo && g.TypeCurriculumGradeId == itemCurriculumGrade.TypeCurriculumGradeId);
                    if (gradeDatabase == null)
                    {
                        gradeDatabase = _entity.ItemCurriculumGrades.FirstOrDefault(g => g.State == (Byte)EnumState.ativo);
                        if (gradeDatabase != null)
                        {
                            gradeDatabase.State = Convert.ToByte(EnumState.excluido);
                            gradeDatabase.UpdateDate = dateNow;
                            itemCurriculumGrades.Add(gradeDatabase);
                        }

                        if (itemCurriculumGrade != null)
                        {
                            itemCurriculumGrades.Add(itemCurriculumGrade);
                        }
                    }
                }

                if (itemCurriculumGrades != null && itemCurriculumGrades.Count > 0)
                    _entity.ItemCurriculumGrades.AddRange(itemCurriculumGrades);

                #endregion

                _entity.BaseText_Id = entity.BaseText_Id;
                _entity.ItemSituation_Id = entity.ItemSituation_Id;
                _entity.ItemLevel_Id = entity.ItemLevel_Id;

                _entity.ItemType_Id = entity.ItemType_Id;
                _entity.EvaluationMatrix_Id = entity.EvaluationMatrix_Id;

                GestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;

                #region Caso já tenha sido versionado altera o itemCode dos outros registros

                var itemsToChangeItemCode = GestaoAvaliacaoContext.Item.Include("ItemFiles").Where(x => x.ItemCodeVersion == entity.ItemCodeVersion).ToList();

                if (itemsToChangeItemCode.Any())
                    itemsToChangeItemCode.ForEach(x =>
                    {
                        x.ItemCode = entity.ItemCode;
                        GestaoAvaliacaoContext.Entry(x).State = System.Data.Entity.EntityState.Modified;
                    });
                #endregion

                GestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }
        }

        public void UpdateVersion(long Id)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Item _lastEntity = GestaoAvaliacaoContext.Item.FirstOrDefault(a => a.Id == Id);
                _lastEntity.LastVersion = false;
                _lastEntity.UpdateDate = DateTime.Now;
                GestaoAvaliacaoContext.Entry(_lastEntity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void UpdateBaseText(long Id, long IdBaseText)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Item _lastEntity = GestaoAvaliacaoContext.Item.FirstOrDefault(a => a.Id == Id);
                _lastEntity.BaseText_Id = IdBaseText;
                _lastEntity.UpdateDate = DateTime.Now;
                GestaoAvaliacaoContext.Entry(_lastEntity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void Delete(long Id)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Item entity = GestaoAvaliacaoContext.Item.Include("Alternatives").Include("ItemSkills").Include("ItemCurriculumGrades").Include("BaseText").FirstOrDefault(a => a.Id == Id);

                foreach (var alternatives in entity.Alternatives)
                {
                    alternatives.State = Convert.ToByte(EnumState.excluido);
                    alternatives.UpdateDate = DateTime.Now;
                }

                foreach (var itemSkill in entity.ItemSkills)
                {
                    itemSkill.State = Convert.ToByte(EnumState.excluido);
                    itemSkill.UpdateDate = DateTime.Now;
                }

                foreach (var itemCurriculumGrade in entity.ItemCurriculumGrades)
                {
                    itemCurriculumGrade.State = Convert.ToByte(EnumState.excluido);
                    itemCurriculumGrade.UpdateDate = DateTime.Now;
                }

                if (entity.BaseText != null)
                {
                    var itemsBaseText = _GetItemsByBaseText(entity.BaseText.Id);
                    if (itemsBaseText != null && itemsBaseText.Count() == 1)
                    {
                        var allowDelete = itemsBaseText.Where(i => i.Id == entity.Id);

                        if (allowDelete != null && itemsBaseText.Count() == 1)
                        {
                            BaseText baseText = entity.BaseText;
                            baseText.State = Convert.ToByte(EnumState.excluido);
                            baseText.UpdateDate = DateTime.Now;
                        }
                    }
                }

                IEnumerable<Item> itemVersions = _GetItemVersions(entity.ItemCodeVersion);
                if (itemVersions != null && itemVersions.Any())
                {
                    Item entityPreviousVersion = itemVersions.FirstOrDefault(i => i.ItemVersion == entity.ItemVersion - 1);
                    if (entityPreviousVersion != null)
                    {
                        entityPreviousVersion.LastVersion = true;
                        entityPreviousVersion.UpdateDate = DateTime.Now;
                        GestaoAvaliacaoContext.Entry(entityPreviousVersion).State = System.Data.Entity.EntityState.Modified;
                        GestaoAvaliacaoContext.SaveChanges();
                    }
                }

                entity.LastVersion = false;
                entity.State = Convert.ToByte(EnumState.excluido);
                entity.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void SaveChangeItem(Item item, long TestId, long itemIdAntigo)
        {

            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Block block = GestaoAvaliacaoContext.Block.Include("BlockItems").FirstOrDefault(p => p.Test_Id == TestId);
                BlockItem blockItemAntigo = block.BlockItems.FirstOrDefault(p => p.Item_Id == itemIdAntigo && p.State == (byte)EnumState.ativo);

                blockItemAntigo.State = Convert.ToByte(EnumState.excluido);
                blockItemAntigo.UpdateDate = DateTime.Now;

                BlockItem blockItem = new BlockItem();
                blockItem.Block_Id = blockItemAntigo.Block_Id;
                blockItem.Item_Id = item.Id;
                blockItem.Order = blockItemAntigo.Order;
                blockItem.State = Convert.ToByte(EnumState.ativo);
                blockItem.CreateDate = DateTime.Now;
                blockItem.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(blockItemAntigo).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.Entry(blockItem).State = System.Data.Entity.EntityState.Added;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        public Item RevokeItem(Item Item)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Item itemBD = gestaoAvaliacaoContext.Item.FirstOrDefault(a => a.Id == Item.Id);
                itemBD.Revoked = Item.Revoked;
                gestaoAvaliacaoContext.Entry(itemBD).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();

                return itemBD;
            }

        }

        #endregion
    }
}
