using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GestaoAvaliacao.Entities.Enumerator;

namespace GestaoAvaliacao.Repository
{
    public class BlockChainRepository : ConnectionReadOnly, IBlockChainRepository
    {
        public BlockChain Save(BlockChain blockChain)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                blockChain.Test = null;

                gestaoAvaliacaoContext.BlockChains.Add(blockChain);
                gestaoAvaliacaoContext.SaveChanges();

                return blockChain;
            }
        }

        public void Update(BlockChain blockChain)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var dateNow = DateTime.Now;

                var entity = gestaoAvaliacaoContext.BlockChains
                    .Include("Test")
                    .Include("BlockChainItems")
                    .Include("BlockChainBlocks")
                    .FirstOrDefault(x => x.Id == blockChain.Id && x.State == (byte)EnumState.ativo);

                if (entity == null)
                    throw new ArgumentNullException("O bloco não foi localizado.");

                blockChain.Test_Id = entity.Test_Id;

                gestaoAvaliacaoContext.Entry(entity).CurrentValues.SetValues(blockChain);
                entity.UpdateDate = dateNow;
                entity.Test.TestSituation = EnumTestSituation.Pending;

                #region BlockChainItem

                var blockChainItems = new List<BlockChainItem>();

                var blockChainItemsFront = blockChain.BlockChainItems.Select(s => s.Item_Id);
                var blockChainItemsDatabase = entity.BlockChainItems.Where(s => s.State == (byte)EnumState.ativo).Select(s => s.Item_Id);
                var blockChainItemsToExclude = blockChainItemsDatabase.Except(blockChainItemsFront).ToList();

                if (blockChainItemsToExclude.Any())
                {
                    foreach (var blockChainItem in entity.BlockChainItems.Where(s => s.State == (byte)EnumState.ativo && blockChainItemsToExclude.Contains(s.Item_Id)))
                    {
                        blockChainItem.State = Convert.ToByte(EnumState.excluido);
                        blockChainItem.UpdateDate = dateNow;

                        blockChainItems.Add(blockChainItem);
                    }
                }

                foreach (var blockChainItemFront in blockChain.BlockChainItems)
                {
                    if (blockChainItemFront == null)
                        continue;

                    var blockChainItemDb = entity.BlockChainItems
                        .FirstOrDefault(e => e.Item_Id.Equals(blockChainItemFront.Item_Id) && e.BlockChain_Id.Equals(blockChainItemFront.BlockChain_Id) && e.State.Equals((byte)EnumState.ativo));

                    if (blockChainItemDb != null)
                    {
                        blockChainItemDb.Order = blockChainItemFront.Order;
                        blockChainItemDb.UpdateDate = DateTime.Now;
                        gestaoAvaliacaoContext.Entry(blockChainItemDb).State = System.Data.Entity.EntityState.Modified;

                        blockChainItems.Add(blockChainItemDb);
                    }
                    else
                        blockChainItems.Add(blockChainItemFront);
                }

                if (blockChainItems.Count > 0)
                    entity.BlockChainItems.AddRange(blockChainItems);

                #endregion

                entity.Test.UpdateDate = dateNow;

                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void RemoveBlockChainItem(long blockChainId, long itemId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var entity = gestaoAvaliacaoContext.BlockChainItems.FirstOrDefault(b => b.BlockChain_Id == blockChainId && b.Item_Id == itemId && b.State == (byte)EnumState.ativo);

                if (entity == null)
                    return;

                entity.State = Convert.ToByte(EnumState.excluido);
                entity.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public void DeleteBlockChainItems(long id)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var blockChain = gestaoAvaliacaoContext.BlockChains.FirstOrDefault(a => a.Id == id);

                if (blockChain == null)
                    return;

                var blockChainItems = gestaoAvaliacaoContext.BlockChainItems.Include("BlockChain")
                    .Where(i => i.BlockChain_Id == id).ToList();

                blockChainItems.ForEach(i =>
                {
                    i.State = Convert.ToByte(EnumState.excluido);
                    i.UpdateDate = DateTime.Now;
                    gestaoAvaliacaoContext.Entry(i).State = System.Data.Entity.EntityState.Modified;
                });

                blockChain.UpdateDate = DateTime.Now;
                gestaoAvaliacaoContext.Entry(blockChain).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public IEnumerable<BlockChain> GetTestBlockChains(long testId)
        {
            const string sql = @"SELECT Id, Description, Test_Id " +
                               "FROM BlockChain WITH (NOLOCK) " +
                               "WHERE Test_Id = @testId " +
                               "AND State = @state " +

                               "SELECT T.Id " +
                               "FROM Test T WITH(NOLOCK)" +
                               "WHERE Id = @testId " +

                               "SELECT BCI.Id, BCI.BlockChain_Id, BCI.Item_Id, " +
                               "(DENSE_RANK() OVER(ORDER BY BCI.[Order]) - 1) AS [Order] " +
                               "FROM BlockChainItem BCI WITH (NOLOCK) " +
                               "INNER JOIN BlockChain BC WITH (NOLOCK) ON BC.Id = BCI.BlockChain_Id " +
                               "INNER JOIN Item I WITH(NOLOCK) ON I.Id = BCI.Item_Id AND I.State <> 3 " +
                               "INNER JOIN Test T WITH(NOLOCK) ON T.Id = BC.[Test_Id] " +
                               "WHERE BC.Test_Id = @testId " +
                               "AND BCI.State = @state AND BC.State = @state";

            using (var cn = Connection)
            {
                cn.Open();

                var multi = cn.QueryMultiple(sql, new { testId, state = (byte)EnumState.ativo });

                var listBlockChain = multi.Read<BlockChain>().ToList();
                var listTest = multi.Read<Test>().ToList();
                var listBlockChainItem = multi.Read<BlockChainItem>().ToList();

                foreach (var blockChain in listBlockChain)
                {
                    blockChain.Test = listTest.FirstOrDefault(p => p.Id == blockChain.Test_Id);
                    blockChain.BlockChainItems.AddRange(listBlockChainItem.Where(i => i.BlockChain_Id.Equals(blockChain.Id)));
                }

                return listBlockChain;
            }
        }

        public IEnumerable<Block> ObterCadernosPorProva(long testId)
        {
            const string sql = @"SELECT distinct b.Id, b.Description, b.Test_Id
								FROM BlockChain bc WITH (NOLOCK)
								inner join [dbo].[BlockChainBlock] bcb WITH (NOLOCK) on bcb.BlockChain_Id = bc.Id
								inner join [dbo].[Block] b WITH (NOLOCK) on bcb.Block_Id = b.Id
								WHERE bc.Test_Id = @testId
								AND bc.State = @state
								AND bcb.State = @state
								AND b.State = @state
                                ORDER BY b.Description

								SELECT T.Id
								FROM Test T WITH(NOLOCK)
								WHERE Id = @testId

								SELECT bcb.BlockChain_Id, bcb.Block_Id
								FROM BlockChain bc WITH (NOLOCK)
								inner join [dbo].[BlockChainBlock] bcb WITH (NOLOCK) on bcb.BlockChain_Id = bc.Id
								inner join [dbo].[Block] b WITH (NOLOCK) on bcb.Block_Id = b.Id
								WHERE bc.Test_Id = @testId
								AND bc.State = @state
								AND bcb.State = @state
								AND b.State = @state

								SELECT bc.Id, bc.Description
								FROM BlockChain bc WITH (NOLOCK)
								inner join [dbo].[BlockChainBlock] bcb WITH (NOLOCK) on bcb.BlockChain_Id = bc.Id
								inner join [dbo].[Block] b WITH (NOLOCK) on bcb.Block_Id = b.Id
								WHERE bc.Test_Id = @testId
								AND bc.State = @state
								AND bcb.State = @state
								AND b.State = @state";

            using (var cn = Connection)
            {
                cn.Open();

                var multi = cn.QueryMultiple(sql, new { testId, state = (byte)EnumState.ativo });

                var listaBlocos = multi.Read<Block>().ToList();
                var listaProva = multi.Read<Test>().ToList();
                var listaBlockChainBlock = multi.Read<BlockChainBlock>().ToList();
                var listaBlockChain = multi.Read<BlockChain>().ToList();

                foreach (var bloco in listaBlocos)
                {
                    bloco.Test = listaProva.FirstOrDefault(p => p.Id == bloco.Test_Id);
                    var blocosCaderno = listaBlockChainBlock.Where(x => x.Block_Id == bloco.Id);
                    if (blocosCaderno != null && blocosCaderno.Any())
                        bloco.Blocos.AddRange(listaBlockChain.Where(bc => blocosCaderno.Any(x => x.BlockChain_Id == bc.Id)).Select(x => x.Id));
                }

                return listaBlocos;
            }
        }

        public IEnumerable<Item> GetBlockChainItems(long blockChainId, int page, int pageItems)
        {
            const string sql = @"WITH ItensPage AS
                                    (
                                        SELECT ROW_NUMBER() OVER (ORDER BY BCI.[Order]) AS RowNum, 
                                            I.Id, I.ItemCode, I.ItemVersion, I.Statement, I.Revoked, I.KnowledgeArea_Id,
                                            I.ItemCodeVersion, KA.Description AS KnowledgeArea_Description,
                                            0 AS KnowledgeArea_Order
                                        FROM Item I WITH (NOLOCK) 
                                            INNER JOIN BlockChainItem BCI WITH (NOLOCK) ON BCI.Item_Id = I.Id 
                                            INNER JOIN BlockChain BC WITH (NOLOCK) ON BC.Id = BCI.BlockChain_Id
                                            INNER JOIN Test T WITH(NOLOCK) ON T.Id = BC.[Test_Id]
                                            LEFT JOIN KnowledgeArea KA WITH(NOLOCK) ON KA.Id = I.KnowledgeArea_Id
                                                AND KA.State = @state
                                        WHERE BCI.BlockChain_Id = @blockChainId
                                        AND BCI.State = @state 
                                        AND I.State = @state 
                                    )

                                    SELECT
                                        *
                                    FROM ItensPage
                                    WHERE RowNum BETWEEN @initialPageItem AND @finalPageItem;";

            const string sqlMulti = @"SELECT B.Id, B.Description, B.Source 
                                        FROM Item I WITH (NOLOCK) 
                                        INNER JOIN BaseText B WITH (NOLOCK) ON B.Id = I.BaseText_Id
                                        WHERE I.Id = @itemId 
                                        AND I.State = @state AND B.State = @state 

                                        SELECT L.Description, L.Value 
                                        FROM Item I WITH (NOLOCK) 
                                        INNER JOIN ItemLevel L WITH (NOLOCK) ON L.Id = I.ItemLevel_Id 
                                        WHERE I.Id = @itemId
                                        AND I.State = @state AND L.State = @state 

                                        SELECT TypeCurriculumGradeId 
                                        FROM ItemCurriculumGrade WITH (NOLOCK) 
                                        WHERE Item_Id = @itemId
                                        AND State = @state 

                                        SELECT BCI.Id, BCI.BlockChain_Id, BCI.Item_Id, BCI.[Order]  
                                        FROM BlockChainItem BCI WITH (NOLOCK) 
                                        INNER JOIN BlockChain BC WITH (NOLOCK) ON BC.Id = BCI.BlockChain_Id 
                                        INNER JOIN Item I WITH (NOLOCK) ON I.Id = BCI.Item_Id 
                                        INNER JOIN Test T WITH(NOLOCK) ON T.Id = BC.[Test_Id] 
                                        WHERE BCI.Item_Id = @itemId AND BCI.BlockChain_Id = @blockChainId 
                                        AND I.State = @state AND BCI.State = @state 

                                        SELECT D.Id, D.Description 
                                        FROM Item I 
                                        INNER JOIN EvaluationMatrix EM WITH (NOLOCK)ON EM.Id = I.EvaluationMatrix_Id 
                                        INNER JOIN Discipline D WITH(NOLOCK) ON EM.Discipline_Id = D.Id 
                                        WHERE I.State = @state 
                                        AND D.State = @state 
                                        AND I.Id = @itemId ";

            using (var cn = Connection)
            {
                cn.Open();

                var initialPageItem = page * pageItems + 1;

                var listItems = cn.Query<Item>(sql,
                    new
                    {
                        blockChainId,
                        state = (byte)EnumState.ativo,
                        initialPageItem,
                        finalPageItem = initialPageItem + pageItems - 1
                    }).ToList();

                foreach (var item in listItems)
                {
                    var itemId = item.Id;

                    var multi = cn.QueryMultiple(sqlMulti,
                        new { itemId, blockChainId, state = (byte)EnumState.ativo });

                    var listBaseText = multi.Read<BaseText>();
                    var listItemLevel = multi.Read<ItemLevel>();
                    var listItemCurriculumGrade = multi.Read<ItemCurriculumGrade>();
                    var listBlockChainItems = multi.Read<BlockChainItem>();
                    var discipline = multi.Read<Discipline>();

                    item.BaseText = listBaseText.FirstOrDefault();
                    item.ItemLevel = listItemLevel.FirstOrDefault();
                    item.ItemCurriculumGrades.AddRange(listItemCurriculumGrade);
                    item.BlockChainItems.AddRange(listBlockChainItems);

                    item.EvaluationMatrix = new EvaluationMatrix
                    {
                        Discipline = discipline.FirstOrDefault()
                    };
                }

                return listItems;
            }
        }

        public void DeleteByTestId(long testId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var blockChains = gestaoAvaliacaoContext.BlockChains
                    .Include("BlockChainItems")
                    .Include("BlockChainBlocks")
                    .Where(i => i.Test_Id == testId).ToList();

                blockChains.ForEach(i =>
                {
                    var blockChainItems = i.BlockChainItems.Where(c => c.BlockChain_Id == i.Id).ToList();
                    var blockChainBlocks = i.BlockChainBlocks.Where(c => c.BlockChain_Id == i.Id).ToList();

                    gestaoAvaliacaoContext.BlockChainItems.RemoveRange(blockChainItems);
                    gestaoAvaliacaoContext.BlockChainBlocks.RemoveRange(blockChainBlocks);

                    gestaoAvaliacaoContext.BlockChains.Remove(i);
                });

                gestaoAvaliacaoContext.SaveChanges();
            }
        }
    }
}
