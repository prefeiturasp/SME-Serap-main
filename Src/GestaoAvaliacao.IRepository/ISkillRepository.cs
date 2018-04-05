using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ISkillRepository
	{
		Skill Save(Skill entity);
		void Update(Skill entity);
		Skill Get(long id);
		object GetParent(long id);
		IEnumerable<Skill> Load(ref Pager pager);
		void Delete(Skill entity);
		IEnumerable<Skill> Search(string search, ref Pager pager);
		IEnumerable<Skill> GetByMatrix(long idMatrix);
        IEnumerable<Skill> GetByDiscipline(long disciplineId);
        IEnumerable<Skill> GetByParent(long idSkill);
		IEnumerable<Skill> SearchByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager);
		IEnumerable<Skill> LoadByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager);
		void SaveRange(List<Skill> listEntity);
		bool ExistsCode(string code, long modelSkillLevelId, long evaluationMatrixId);
		bool ExistsCodeAlter(string code, long modelSkillLevelId, long evaluationMatrixId, long id);
		bool ExistsItemSkill(long skillId, long evaluationMatrixId);
		IEnumerable<ItemReportItemSkill> GetBySkillReport(int id, int skill, Guid EntityId, long TypeLevelEducation);
		IEnumerable<ItemReportItemSkill> GetBySkillReportOneLevel(int id, int matrizId, Guid EntityId, long TypeLevelEducation);
		IEnumerable<Skill> GetByCognitiveCompetence(long idCognitiveCompetence);
        IEnumerable<Skill> GetComboByDiscipline(long idDiscipline);

    }
}
