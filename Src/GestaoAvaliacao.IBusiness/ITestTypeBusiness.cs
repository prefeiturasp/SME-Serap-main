using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeBusiness
	{
		Task<TestType> SaveAsync(TestType entity, Guid ent_id);
		Task<TestType> UpdateAsync(long id, TestType entity, Guid ent_id);
		Task<TestType> GetAsync(long Id, Guid EntityId);
		IEnumerable<TestType> Load(ref Pager pager, Guid EntityId);
		IEnumerable<TestType> Search(string search, ref Pager pager, Guid EntityId);
		Task<TestType> DeleteAsync(long id, Guid ent_id);
		Task<IEnumerable<TestType>> LoadByUserGroupAsync(Guid EntityId, bool IsAdmin);
		void UnsetModelTest(long modelTestId);
        bool ExistsTestAssociated(long Id);
        IEnumerable<TestType> LoadFiltered(Guid EntityId, bool IsAdmin);

    }
}
