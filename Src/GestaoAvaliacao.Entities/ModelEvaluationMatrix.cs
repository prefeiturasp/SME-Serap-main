using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class ModelEvaluationMatrix : EntityBase
    {
        public ModelEvaluationMatrix()
        {
            EvaluationMatrices = new List<EvaluationMatrix>();
            ModelSkillLevels = new List<ModelSkillLevel>();
        }

        public virtual string Description { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual List<EvaluationMatrix> EvaluationMatrices { get; set; }
        public virtual List<ModelSkillLevel> ModelSkillLevels { get; set; }
    
    }
}
