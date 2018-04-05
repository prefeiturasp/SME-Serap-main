using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeRepository
	{
		TestType Save(TestType entity);
		TestType Update(TestType entity);
		TestType Get(long id, Guid EntityId);
		void UnsetModelTest(TestType entity);
		IEnumerable<TestType> Load(ref Pager pager, Guid EntityId);
		void Delete(long id);
		IEnumerable<TestType> Search(string search, ref Pager pager, Guid EntityId);
		bool ExistsDescriptionNamed(string description, long id);
		IEnumerable<TestType> LoadNotGlobal(Guid EntityId);
		IEnumerable<TestType> LoadAll(Guid EntityId);
		IEnumerable<TestType> GetByModelTest(long modelTestId);
        bool ExistsTestAssociated(long Id);
        TestType Get(long Id);
        IEnumerable<TestType> LoadFiltered(Guid EntityId, bool global);

    }
}
