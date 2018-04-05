using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ISkillBusiness
	{
		Skill Save(Skill entity);
		Skill Update(long id, Skill entity);
		Skill Get(long id);
		object GetParent(long id);
		Skill Delete(long id);
		IEnumerable<Skill> Load(ref Pager pager);
		IEnumerable<Skill> GetByMatrix(long idMatrix);
        IEnumerable<Skill> GetByDiscipline(long disciplineId);
        IEnumerable<Skill> GetByParent(long idSkill);
		IEnumerable<Skill> Search(string search, ref Pager pager);
		IEnumerable<Skill> SearchByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager);
		IEnumerable<Skill> LoadByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager);
		Skill SaveRange(List<Skill> listEntity);
		Skill DeleteByMatrix(long id, long evaluationMatrixId);
		IEnumerable<ItemReportItemSkill> GetBySkillReport(int id, int skill, Guid EntityId, long TypeLevelEducation);
		IEnumerable<ItemReportItemSkill> GetBySkillReportOneLevel(int id, int matrizId, Guid EntityId, long TypeLevelEducation);
        IEnumerable<Skill> GetComboByDiscipline(long idDiscipline);

    }
}
