using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

namespace GestaoAvaliacao.Business
{
    public class BlockChainBusiness : IBlockChainBusiness
    {
        private readonly IBlockChainRepository blockChainRepository;

        public BlockChainBusiness(IBlockChainRepository blockChainRepository)
        {
            this.blockChainRepository = blockChainRepository;
        }
    }
}
