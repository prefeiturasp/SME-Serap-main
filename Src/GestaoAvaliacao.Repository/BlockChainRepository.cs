using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Entities.DTO.Tests;

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
                    .Include("BlockChainBlocks.Block.BlockItems")
                    .FirstOrDefault(x => x.Id == blockChain.Id && x.State == (byte)EnumState.ativo);

                if (entity == null)
                    throw new ArgumentNullException("O bloco não foi localizado.");

                blockChain.Test_Id = entity.Test_Id;

                gestaoAvaliacaoContext.Entry(entity).CurrentValues.SetValues(blockChain);
                entity.UpdateDate = dateNow;
                entity.Test.TestSituation = EnumTestSituation.Pending;

                #region BlockChainItem

                var blockChainItems = new List<BlockChainItem>();
                var blockItems = new List<BlockItem>();

                var blockChainBlocksDatabase = entity.BlockChainBlocks.Where(c => c.BlockChain_Id == blockChain.Id && c.State == (byte)EnumState.ativo).ToList();
                var blocksDatabase = blockChainBlocksDatabase.Select(c => c.Block).Where(c => c.State == (byte)EnumState.ativo).ToList();
                var blockChainItemsDatabase = entity.BlockChainItems.Where(c => c.State == (byte)EnumState.ativo).ToList();

                var blockChainItemsFront = blockChain.BlockChainItems;
                var idsItemsFront = blockChainItemsFront.Select(s => s.Item_Id);
                var idsItemsDatabase = blockChainItemsDatabase.Select(s => s.Item_Id);
                var idsItemsToExclude = idsItemsDatabase.Except(idsItemsFront).ToList();

                if (idsItemsToExclude.Any())
                {
                    //-> BlockChainItem
                    foreach (var blockChainItem in blockChainItemsDatabase.Where(s => idsItemsToExclude.Contains(s.Item_Id)))
                    {
                        blockChainItem.State = Convert.ToByte(EnumState.excluido);
                        blockChainItem.UpdateDate = dateNow;

                        blockChainItems.Add(blockChainItem);
                    }

                    //-> Block
                    foreach (var block in blocksDatabase)
                    {
                        var blockItemsToExclude = block.BlockItems.Where(s =>
                            idsItemsToExclude.Contains(s.Item_Id) && s.State == (byte)EnumState.ativo);

                        foreach (var blockItem in blockItemsToExclude)
                        {
                            blockItem.State = Convert.ToByte(EnumState.excluido);
                            blockItem.UpdateDate = dateNow;

                            blockItems.Add(blockItem);
                        }
                    }
                }

                foreach (var blockChainItemFront in blockChainItemsFront)
                {
                    if (blockChainItemFront == null)
                        continue;

                    //-> BlockChainItem
                    var blockChainItemDb = blockChainItemsDatabase
                        .FirstOrDefault(e =>
                            e.Item_Id.Equals(blockChainItemFront.Item_Id) &&
                            e.BlockChain_Id.Equals(blockChainItemFront.BlockChain_Id));

                    if (blockChainItemDb != null)
                    {
                        blockChainItemDb.Order = blockChainItemFront.Order;
                        blockChainItemDb.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(blockChainItemDb).State = System.Data.Entity.EntityState.Modified;

                        blockChainItems.Add(blockChainItemDb);
                    }
                    else
                        blockChainItems.Add(blockChainItemFront);

                    //-> Block
                    foreach (var blockDb in blocksDatabase)
                    {
                        var blockItemDb = blockDb.BlockItems.FirstOrDefault(c =>
                            c.Item_Id == blockChainItemFront.Item_Id && c.Block_Id == blockDb.Id &&
                            c.State == (byte)EnumState.ativo);

                        if (blockItemDb != null)
                        {
                            blockItemDb.UpdateDate = dateNow;
                            gestaoAvaliacaoContext.Entry(blockItemDb).State = System.Data.Entity.EntityState.Modified;

                            blockItems.Add(blockItemDb);
                        }
                        else
                        {
                            blockItems.Add(new BlockItem
                            {
                                Block_Id = blockDb.Id,
                                Item_Id = blockChainItemFront.Item_Id,
                                Order = blockItems.Max(c => c.Order) + 1
                            });
                        }
                    }
                }

                //-> BlockChainItem
                if (blockChainItems.Count > 0)
                    entity.BlockChainItems.AddRange(blockChainItems);

                //-> BlockItem
                if (blockItems.Count > 0)
                {
                    //-> Ordenar
                    foreach (var blockDb in blocksDatabase.OrderBy(c => c.Description))
                    {
                        var maxOrder = 0;

                        foreach (var blockItem in blockItems.Where(c => c.Block_Id == blockDb.Id))
                        {
                            var blockItemDb = blockDb.BlockItems.FirstOrDefault(c =>
                                c.Item_Id == blockItem.Item_Id && c.Block_Id == blockDb.Id &&
                                c.State == (byte)EnumState.ativo);

                            if (blockItemDb == null)
                                continue;

                            if (maxOrder == 0)
                                maxOrder = blockItemDb.Order;

                            blockItem.Order = maxOrder;
                            maxOrder++;
                        }
                    }

                    //-> Atualizar
                    foreach (var blockChainBlock in entity.BlockChainBlocks.Where(c =>
                                 c.BlockChain_Id == blockChain.Id && c.State == (byte)EnumState.ativo))
                    {
                        blockChainBlock.Block.BlockItems.AddRange(blockItems.Where(c =>
                            c.Block_Id == blockChainBlock.Block_Id));
                    }
                }

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
            const string sql = @"SELECT b.Id, b.Description, b.Test_Id
								FROM Block b WITH (NOLOCK)
								WHERE b.Test_Id = @testId
								AND b.State = @state
								AND EXISTS(SELECT bcb.Id 
  										   FROM BlockChainBlock bcb WITH (NOLOCK)
  										   INNER JOIN BlockChain bc WITH (NOLOCK) ON (bc.Id = bcb.BlockChain_Id 
  										                                          AND bc.State = @state)
										   WHERE bcb.Block_Id = b.Id 
										   AND bcb.State = @state)
                                ORDER BY b.Description

								SELECT T.Id
								FROM Test T WITH(NOLOCK)
								WHERE T.Id = @testId
                                AND T.State = @state

								SELECT bcb.BlockChain_Id, bcb.Block_Id
								FROM BlockChainBlock bcb WITH (NOLOCK)
								INNER JOIN BlockChain bc WITH (NOLOCK) ON (bc.Id = bcb.BlockChain_Id 
 									                                   AND bc.State = @state)
								INNER JOIN Block b WITH (NOLOCK) ON (b.Id = bcb.Block_Id
								                                 AND b.State = @state
                                                                 AND b.Test_Id = @testId)
								WHERE bc.Test_Id = @testId
								AND bcb.State = @state
								ORDER BY bcb.[Order], bcb.Block_Id, bcb.BlockChain_Id 

								SELECT bc.Id, bc.Description
								FROM BlockChain bc WITH (NOLOCK)
								WHERE bc.Test_Id = @testId
								AND bc.State = @state
								AND EXISTS(SELECT bcb.Id 
  										   FROM BlockChainBlock bcb WITH (NOLOCK)
  										   INNER JOIN Block b WITH (NOLOCK) ON (b.Id = bcb.Block_Id 
  										                                    AND bc.State = @state
                                                                            AND b.Test_Id = @testId)
										   WHERE bcb.BlockChain_Id = bc.Id  
										   AND bcb.State = @state)";

            using (var cn = Connection)
            {
                cn.Open();

                var multi = cn.QueryMultiple(sql, new { testId, state = (byte)EnumState.ativo });

                var listaCaderno = multi.Read<Block>().ToList();
                var listaProva = multi.Read<Test>().ToList();
                var listaBlockChainBlock = multi.Read<BlockChainBlock>().ToList();
                var listaBlockChain = multi.Read<BlockChain>().ToList();

                foreach (var caderno in listaCaderno)
                {
                    caderno.Test = listaProva.FirstOrDefault(p => p.Id == caderno.Test_Id);
                    var blocosCaderno = listaBlockChainBlock.Where(x => x.Block_Id == caderno.Id).ToList();

                    if (!blocosCaderno.Any())
                        continue;

                    foreach (var bloco in blocosCaderno)
                        caderno.Blocos.Add(listaBlockChain.FirstOrDefault(c => c.Id == bloco.BlockChain_Id));
                }

                return listaCaderno;
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
                    .Include("BlockChainBlocks.Block")
                    .Include("BlockChainBlocks.Block.BlockItems")
                    .Where(i => i.Test_Id == testId).ToList();

                blockChains.ForEach(i =>
                {
                    var blockChainItems = i.BlockChainItems.Where(c => c.BlockChain_Id == i.Id).ToList();
                    var blockChainBlocks = i.BlockChainBlocks.Where(c => c.BlockChain_Id == i.Id).ToList();

                    blockChainBlocks.ForEach(x =>
                    {
                        var blockItems = x.Block.BlockItems.Where(c => c.Block_Id == x.Block_Id).ToList();

                        gestaoAvaliacaoContext.BlockItem.RemoveRange(blockItems);
                        gestaoAvaliacaoContext.Block.Remove(x.Block);
                    });

                    gestaoAvaliacaoContext.BlockChainItems.RemoveRange(blockChainItems);
                    gestaoAvaliacaoContext.BlockChainBlocks.RemoveRange(blockChainBlocks);

                    gestaoAvaliacaoContext.BlockChains.Remove(i);
                });

                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public NumbersBlockChainDTO GetNumbersBlockChainByTestId(long testId)
        {
            const string sql = @"SELECT  BlockChainNumber, BlockChainItems 
                                       FROM TEST
                                     WHERE  id = @testId";
            using (var cn = Connection)
            {
                cn.Open();
                return cn.Query<NumbersBlockChainDTO>(sql, new { testId }).FirstOrDefault();

            }
        }
    }
}
