using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainBlockRepository
    {
        void Delete(long block_Id, long blockChain_Id);
        void DeleteByBlockId(long blockId);
        List<BlockChainBlock> GetByTestId(long testId);
        IEnumerable<BlockChainBlock> GetTestBlockChainsBlock(long testId);
    }
}
