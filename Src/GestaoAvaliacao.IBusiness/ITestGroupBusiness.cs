using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
	public interface ITestGroupBusiness
	{
		TestGroup Get(long id);
		TestGroup GetTestGroup(long id);
		IEnumerable<TestGroup> Load(Guid ent_id);
		IEnumerable<TestGroup> LoadByPermissionTest(TestFilter filter);
		IEnumerable<TestGroup> LoadPaginate(ref Pager pager, Guid ent_id);
		IEnumerable<TestGroup> Search(ref Pager pager, Guid ent_id, string search = null, int levelQntd = 0);
		TestGroup Save(TestGroup entity, Guid ent_id);
		TestGroup Update(TestGroup entity, Guid ent_id);
		TestGroup Delete(long id, Guid ent_id);
		IEnumerable<TestGroup> LoadGroupsSubGroups(Guid ent_id);
		bool VerifyDeleteSubGroup(long Id);
	}
}
