using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class CorrelatedSkillByEvaluationMatrix : EntityBase
    {
        public Int64 rowNumber { get; set; }
        public string Matriz1 { get; set; }    
        public string UltimoNivel1 { get; set; }
        public string Matriz2 { get; set; }    
        public string UltimoNivel2 { get; set; }

    }
}
