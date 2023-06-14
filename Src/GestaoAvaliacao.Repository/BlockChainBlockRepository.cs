using System.Collections.Generic;
using System.Linq;
using GestaoAvaliacao.Entities;
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

        public void Delete(long block_Id, long blockChain_Id)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {

                var blockChainBlocks = gestaoAvaliacaoContext.BlockChainBlocks.Include("Block")
                    .Include("Block.BlockItems").Where(i => i.Block_Id == block_Id && i.BlockChain_Id == blockChain_Id).ToList();

                blockChainBlocks.ForEach(i =>
                {                    
                    gestaoAvaliacaoContext.BlockChainBlocks.Remove(i);
                    gestaoAvaliacaoContext.SaveChanges();
                });                
            }
        }

        public List<BlockChainBlock> GetByTestId(long testId)
        {
            using (var gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.BlockChainBlocks.Include("Block")
                    .Include("Block.BlockItems")
                    .Include("BlockChain")
                    .Where(i => i.Block.Test_Id == testId).ToList();
            }
        }
    }
}
