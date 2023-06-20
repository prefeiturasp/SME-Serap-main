using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class BlockChainBlockBusiness : IBlockChainBlockBusiness
    {
        private readonly IBlockChainBlockRepository blockChainBlockRepository;

        public BlockChainBlockBusiness(IBlockChainBlockRepository blockChainBlockRepository)
        {
            this.blockChainBlockRepository = blockChainBlockRepository;
        }

        public void DeleteByBlockId(long blockId)
        {
            blockChainBlockRepository.DeleteByBlockId(blockId);
        }

        public List<BlockChainBlock> GetByTestId(long testId)
        {
            return blockChainBlockRepository.GetByTestId(testId);
        }
    }
}
