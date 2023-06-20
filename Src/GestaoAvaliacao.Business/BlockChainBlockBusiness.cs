using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

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
    }
}
