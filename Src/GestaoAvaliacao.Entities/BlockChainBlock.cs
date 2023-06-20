using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class BlockChainBlock : EntityBase
    {
        public virtual Block Block { get; set; }
        public long Block_Id { get; set; }
        public virtual BlockChain BlockChain { get; set; }
        public long BlockChain_Id { get; set; }
        public int Order { get; set; }
    }
}
