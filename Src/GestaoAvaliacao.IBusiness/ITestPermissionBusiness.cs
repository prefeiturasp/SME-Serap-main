using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestPermissionBusiness
    {
        IEnumerable<TestPermission> GetByTest(long test_id, Guid? gru_id);
        TestPermission Save(TestPermission entity, List<TestPermission> permissions);
    }
}
