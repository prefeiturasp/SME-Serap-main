using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IBlockChainBlockBusiness
    {
        void DeleteByBlockId(long blockId);
        List<BlockChainBlock> GetByTestId(long testId);
    }
}
