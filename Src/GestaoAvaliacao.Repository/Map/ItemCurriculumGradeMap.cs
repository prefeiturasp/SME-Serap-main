using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemCurriculumGradeMap : EntityBaseMap<ItemCurriculumGrade>
	{
		public ItemCurriculumGradeMap()
		{
			ToTable("ItemCurriculumGrade");

			Property(p => p.TypeCurriculumGradeId)
			   .IsRequired();
		}
	}
}
