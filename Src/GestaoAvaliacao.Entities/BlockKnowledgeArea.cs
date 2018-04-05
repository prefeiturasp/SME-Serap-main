using GestaoAvaliacao.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class BlockKnowledgeArea : EntityBase
    {
        public virtual Block Block { get; set; }
        public long Block_Id { get; set; }
        public virtual KnowledgeArea KnowledgeArea { get; set; }
        public long KnowledgeArea_Id { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public string Description { get; set; }
    }
}