using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities
{
    public class KnowledgeAreaDiscipline : EntityBase
    {
        public virtual KnowledgeArea KnowledgeArea { get; set; }

        [NotMapped]
        public virtual long Discipline_Id { get; set; }

        public virtual Discipline Discipline { get; set; }
    }
}
