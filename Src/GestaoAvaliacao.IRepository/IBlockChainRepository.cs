using GestaoAvaliacao.Entities;
using System.Collections.Generic;
using System;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainRepository
    {
        BlockChain Save(BlockChain blockChain);
        void Update(BlockChain blockChain);
        void RemoveBlockChainItem(long blockChainId, long itemId);
        void DeleteBlockChainItems(long id);
        IEnumerable<BlockChain> GetTestBlockChains(long testId);
        IEnumerable<Item> GetBlockChainItems(long blockChainId, int page, int pageItems);
    }
}
