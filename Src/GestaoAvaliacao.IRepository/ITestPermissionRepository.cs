using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestPermissionRepository
    {
        IEnumerable<TestPermission> GetByTest(long Test_Id, Guid? gru_id);
        bool Save(long Test_Id, List<TestPermission> permissions);
    }
}
