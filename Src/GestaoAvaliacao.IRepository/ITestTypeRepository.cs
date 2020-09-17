using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeRepository
	{
		Task<TestType> SaveAsync(TestType entity);
		Task<TestType> UpdateAsync(TestType entity);
		Task<TestType> GetAsync(long id, Guid EntityId);
		void UnsetModelTest(TestType entity);
		IEnumerable<TestType> Load(ref Pager pager, Guid EntityId);
		Task DeleteAsync(long id);
		IEnumerable<TestType> Search(string search, ref Pager pager, Guid EntityId);
		bool ExistsDescriptionNamed(string description, long id);
		Task<IEnumerable<TestType>> LoadNotGlobalAsync(Guid EntityId);
		Task<IEnumerable<TestType>> LoadAllAsync(Guid EntityId);
		IEnumerable<TestType> GetByModelTest(long modelTestId);
        bool ExistsTestAssociated(long Id);
        TestType Get(long Id);
        IEnumerable<TestType> LoadFiltered(Guid EntityId, bool global);
		bool GetTestTypeTargetToStudentsWithDeficiencies(long id);

	}
}
