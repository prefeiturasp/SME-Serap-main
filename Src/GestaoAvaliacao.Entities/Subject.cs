using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities
{
    public class Subject : EntityBase
    {
        public Subject()
        {
            SubSubjects = new List<SubSubject>();
            Disciplines = new List<Discipline>();
            KnowledgeAreas = new List<KnowledgeArea>();
        }

        public virtual string Description { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual List<SubSubject> SubSubjects { get; set; }

        public virtual List<Discipline> Disciplines { get; set; }

        public virtual List<KnowledgeArea> KnowledgeAreas { get; set; }

        [NotMapped]
        public virtual long Discipline_Id { get; set; }

        [NotMapped]
        public virtual string KnowledgeArea_Description { get; set; }

        [NotMapped]
        public virtual long Subsubject_Id { get; set; }

        [NotMapped]
        public virtual string Subsubject_Description { get; set; }
    }
}
