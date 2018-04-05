using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class Booklet : EntityBase
    {
        public Booklet()
        {
            Blocks = new List<Block>();
        }

        public virtual Test Test { get; set; }
        public long? Test_Id { get; set; }
        public int Order { get; set; }
        public virtual List<Block> Blocks { get; set; }
    }
}
