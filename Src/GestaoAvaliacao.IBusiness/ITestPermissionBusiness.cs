using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestPermissionBusiness
    {
        IEnumerable<TestPermission> GetByTest(long test_id, Guid? gru_id);
        Task<List<TestPermission>> GetPermissionByTest(long test_id, Guid? gru_id);
        TestPermission Save(TestPermission entity, List<TestPermission> permissions);
    }
}
