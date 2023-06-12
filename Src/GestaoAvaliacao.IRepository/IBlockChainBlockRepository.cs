using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainBlockRepository
    {
        void DeleteByBlockId(long blockId);
        List<BlockChainBlock> GetByTestId(long testId);
    }
}
