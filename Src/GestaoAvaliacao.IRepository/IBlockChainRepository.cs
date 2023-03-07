using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IRepository
{
    public interface IBlockChainRepository
    {
        BlockChain Save(BlockChain blockChain);
        void Update(BlockChain blockChain);
    }
}
