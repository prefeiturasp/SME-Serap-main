using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class Block : EntityBase
    {
        public Block()
        {
            BlockItems = new List<BlockItem>();
            BlockChains = new List<BlockChain>();
        }

        public string Description { get; set; }
        public virtual Booklet Booklet { get; set; }
        public long? Booklet_Id { get; set; }
        public virtual Test Test { get; set; }
        public long? Test_Id { get; set; }
        public virtual List<BlockItem> BlockItems { get; set; }
        public virtual List<BlockKnowledgeArea> BlockKnowledgeAreas { get; set; }
        public virtual List<BlockChain> BlockChains { get; set; }

    }
}
