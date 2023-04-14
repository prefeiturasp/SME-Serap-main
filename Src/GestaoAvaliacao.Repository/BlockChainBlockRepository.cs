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
                var blockChainBlocks = gestaoAvaliacaoContext.BlockChainBlocks.Include("Block")
                    .Include("Block.BlockItems").Where(i => i.Block_Id == blockId).ToList();

                blockChainBlocks.ForEach(i =>
                {
                    var blockItems = i.Block.BlockItems.Where(c => c.Block_Id == i.Block_Id).ToList();
                    gestaoAvaliacaoContext.BlockItem.RemoveRange(blockItems);

                    gestaoAvaliacaoContext.BlockChainBlocks.Remove(i);
                });

                gestaoAvaliacaoContext.SaveChanges();
            }
        }
    }
}
