using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class BlockChain : EntityBase
    {
        public BlockChain()
        {
            BlockChainItems = new List<BlockChainItem>();
            BlockChainBlocks = new List<BlockChainBlock>();
        }

        public string Description { get; set; }
        public virtual Test Test { get; set; }
        public long? Test_Id { get; set; }
        public virtual List<BlockChainItem> BlockChainItems { get; set; }
        public virtual List<BlockChainBlock> BlockChainBlocks { get; set; }
    }
}
