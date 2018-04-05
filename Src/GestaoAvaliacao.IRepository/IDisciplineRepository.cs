using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
	public interface IDisciplineRepository
	{
		Discipline Save(Discipline entity);
		List<Discipline> SaveRange(List<Discipline> listEntity);
		Discipline Get(long id);
		IEnumerable<Discipline> Load(Guid EntityId, ref Pager pager);
		IEnumerable<Discipline> LoadCustom(Guid EntityId);
		IEnumerable<Discipline> SearchAllDisciplines(Guid EntityId);
		void Delete(Discipline entity);
		IEnumerable<Discipline> Search(Guid EntityId, string search, ref Pager pager);
		bool ExistsMatrix(long Id);
		IEnumerable<Discipline> LoadComboHasMatrix(Guid entityId);
		IEnumerable<Discipline> LoadComboByTest(long test_id);
		List<AJX_Select2> LoadDisciplineByKnowledgeArea(string description, string knowledgeAreas, Guid EntityId);
		IEnumerable<Discipline> GetDisciplinesByTestSubGroup_Id(long TestSubGroup_Id);

	}
}
