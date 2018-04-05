
namespace GestaoAvaliacao.Entities
{
    /// <summary>
    /// Objeto para os filtros de pesquisa do banco de itens
    /// </summary>
    public class ItemFilter
    {
        public virtual string ItemCode { get; set; }

        public virtual bool? Revoked { get; set; }

        public virtual string ItemSituation { get; set; }

        public virtual bool ShowVersion { get; set; }

        public virtual int? ProficiencyStart { get; set; }

        public virtual int? ProficiencyEnd { get; set; }

        public virtual string Keywords { get; set; }

        public virtual long? DisciplineId { get; set; }

        public virtual long? EvaluationMatrixId { get; set; }

        public virtual long? SkillId { get; set; }

        public virtual bool SkillLastLevel { get; set; }

        public virtual string CorrelatedSkills { get; set; }

        public virtual string TypeCurriculumGrades { get; set; }

        public virtual bool ShowItemNarrated { get; set; }
    }
}
