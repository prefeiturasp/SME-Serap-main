using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO.Tests;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainRepository
    {
        BlockChain Save(BlockChain blockChain);
        void Update(BlockChain blockChain);
        void RemoveBlockChainItem(long blockChainId, long itemId);
        void DeleteBlockChainItems(long id);
        IEnumerable<BlockChain> GetTestBlockChains(long testId);
        IEnumerable<Block> ObterCadernosPorProva(long testId);
        IEnumerable<Item> GetBlockChainItems(long blockChainId, int page, int pageItems);
        NumbersBlockChainDTO GetNumbersBlockChainByTestId(long testId);
        void DeleteByTestId(long testId);
    }
}
