using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class BlockChainBlock : EntityBase
    {
        public string Description { get; set; }
        public virtual BlockChain BlockChain { get; set; }
        public long BlockChain_Id { get; set; }
    }
}
