using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainBlockRepository
    {
        void DeleteByBlockId(long blockId);
        IEnumerable<BlockChainBlock> GetTestBlockChainsBlock(long testId);
    }
}
