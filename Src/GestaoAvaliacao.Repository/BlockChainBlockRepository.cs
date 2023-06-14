using System.Collections.Generic;
using System.Linq;
using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
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

        public IEnumerable<BlockChainBlock> GetTestBlockChainsBlock(long testId)
        {
            const string sql = @"SELECT * from BlockChainBlock bcb WITH (NOLOCK)
                                    inner join BlockChain bc WITH (NOLOCK) on bc.Id = bcb.BlockChain_Id
                                    WHERE bc.Test_Id = @testId
                                    AND bc.State = @state
                                    AND bcb.State = @state
                                    ORDER by bcb.Block_Id, bcb.[Order]";

            using (var cn = Connection)
            {
                cn.Open();
                return cn.Query<BlockChainBlock>(sql, new { testId, state = (byte)EnumState.ativo });
            }
        }
    }
}
