using GestaoAvaliacao.Entities;
using System;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IBlockChainBusiness
    {
        BlockChain Save(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision);
        BlockChain Update(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision);
        void RemoveBlockChainItem(long blockChainId, long itemId);
        BlockChain DeleteBlockChainItems(long id);
        IEnumerable<BlockChain> GetTestBlockChains(long testId);
        IEnumerable<Block> ObterCadernosPorProva(long testId);
        IEnumerable<Item> GetBlockChainItems(long blockChainId, int page, int pageItems);
        void DeleteByTestId(long testId);
    }
}
