using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
	public interface IDisciplineBusiness
	{
		Discipline Save(Discipline entity, Guid EntityId);
		Discipline Get(long id);
		Discipline Delete(long id);
		IEnumerable<Discipline> Load(ref Pager pager, Guid EntityId);
		IEnumerable<Discipline> Search(string search, ref Pager pager, Guid EntityId);
		IEnumerable<Discipline> SearchDisciplines(int typeLevelEducation, Guid EntityId);
		List<Discipline> SaveRange(List<Discipline> listEntity, Guid EntityId);
		IEnumerable<Discipline> LoadCustom(Guid EntityId);
		IEnumerable<Discipline> SearchDisciplinesSaves(int typeLevelEducation, Guid EntityId);
		IEnumerable<Discipline> SearchAllDisciplines(Guid EntityId);
		IEnumerable<Discipline> LoadComboHasMatrix(Guid entityId);
		IEnumerable<Discipline> LoadComboByTest(long test_id);
		List<AJX_Select2> LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas, Guid EntityId);
		IEnumerable<Discipline> GetDisciplinesByTestSubGroup_Id(long TestSubGroup_Id);

	}
}
