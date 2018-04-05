using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class ItemCurriculumGrade : EntityBase
	{
		public virtual Item Item { get; set; }

		public virtual int TypeCurriculumGradeId { get; set; }
	}
}
