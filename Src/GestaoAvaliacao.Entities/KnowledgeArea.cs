using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class KnowledgeArea : EntityBase
    {
        public KnowledgeArea()
        {
            KnowledgeAreaDisciplines = new List<KnowledgeAreaDiscipline>();
            Subjects = new List<Subject>();
        }

        public virtual string Description { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual List<KnowledgeAreaDiscipline> KnowledgeAreaDisciplines { get; set; }

        public virtual List<Subject> Subjects { get; set; }

        public virtual List<BlockKnowledgeArea> BlockKnowledgeAreas { get; set; }
    }
}
