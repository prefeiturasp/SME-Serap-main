using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeBusiness
	{
		TestType Save(TestType entity, Guid ent_id);
		TestType Update(long id, TestType entity, Guid ent_id);
		TestType Get(long id, Guid EntityId);
		IEnumerable<TestType> Load(ref Pager pager, Guid EntityId);
		IEnumerable<TestType> Search(string search, ref Pager pager, Guid EntityId);
		TestType Delete(long id, Guid ent_id);
		IEnumerable<TestType> LoadByUserGroup(Guid EntityId, bool IsAdmin);
		void UnsetModelTest(long modelTestId);
        bool ExistsTestAssociated(long Id);
        IEnumerable<TestType> LoadFiltered(Guid EntityId, bool IsAdmin);

    }
}
