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
            /*
            using (var cn = Connection)
            {
                cn.Open();

                const string sql = @"SELECT Id, Description, Test_Id " +
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
            */
            return new List<BlockChain>();
        }
    }
}
