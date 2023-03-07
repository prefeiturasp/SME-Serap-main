using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class BlockChainItem : EntityBase
    {
        public virtual BlockChain BlockChain { get; set; }
        public long BlockChain_Id { get; set; }

        public virtual Item Item { get; set; }
        public long Item_Id { get; set; }

        public int Order { get; set; }
    }
}
