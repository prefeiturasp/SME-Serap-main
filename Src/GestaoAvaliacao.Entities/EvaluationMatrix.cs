using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class EvaluationMatrix : EntityBase
    {
        public EvaluationMatrix()
        {
            EvaluationMatrixCourse = new List<EvaluationMatrixCourse>();
            Skills = new List<Skill>();
        }

        public virtual string Description { get; set; }

        public virtual string Edition { get; set; }  

        public virtual Discipline Discipline { get; set; }

        public virtual ModelEvaluationMatrix ModelEvaluationMatrix { get; set; }

        public virtual List<EvaluationMatrixCourse> EvaluationMatrixCourse { get; set; }
        
        public virtual List<Skill> Skills { get; set; }

    }
}
