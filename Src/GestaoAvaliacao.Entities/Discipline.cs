using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class Discipline : EntityBase
    {
        public Discipline()
        {
            EvaluationMatrixs = new List<EvaluationMatrix>();
            KnowledgeAreaDisciplines = new List<KnowledgeAreaDiscipline>();
            Subjects = new List<Subject>();
        }

        public virtual string Description { get; set; }

        public virtual int DisciplineTypeId { get; set; }

        public virtual int TypeLevelEducationId { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual List<EvaluationMatrix> EvaluationMatrixs { get; set; }

        public virtual List<KnowledgeAreaDiscipline> KnowledgeAreaDisciplines { get; set; }

        public virtual List<Subject> Subjects { get; set; }

        [NotMapped]
        public virtual string tne_nome { get; set; }
    }
}
