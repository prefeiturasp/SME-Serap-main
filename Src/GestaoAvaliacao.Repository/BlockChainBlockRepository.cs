using System.Linq;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;

namespace GestaoAvaliacao.Repository
{
    public class BlockChainBlockRepository : ConnectionReadOnly, IBlockChainBlockRepository
    {
        public void DeleteByBlockId(long blockId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var blockChainBlocks = gestaoAvaliacaoContext.BlockChainBlocks.Where(i => i.Block_Id == blockId).ToList();

                blockChainBlocks.ForEach(i =>
                {
                    gestaoAvaliacaoContext.BlockChainBlocks.Remove(i);
                });

                gestaoAvaliacaoContext.SaveChanges();
            }
        }
    }
}
