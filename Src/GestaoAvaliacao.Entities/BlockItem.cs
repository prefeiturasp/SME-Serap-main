using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class BlockItem : EntityBase
    {
        public virtual Block Block { get; set; }
        public long Block_Id { get; set; }

        public virtual Item Item { get; set; }
        public long Item_Id { get; set; }

        public int Order { get; set; }
        [NotMapped]
        public int? KnowledgeArea_Id { get; set; }
        [NotMapped]
        public string KnowledgeArea_Description { get; set; }
        public virtual List<RequestRevoke> RequestRevokes { get; set; }

        [NotMapped]
        public long Test_id { get; set; }
    }
}
