using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
