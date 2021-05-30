using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestPermissionRepository
    {
        Task<List<TestPermission>> GetPermissionByTest(long Test_Id, Guid? gru_id);

        IEnumerable<TestPermission> GetByTest(long Test_Id, Guid? gru_id);
        bool Save(long Test_Id, List<TestPermission> permissions);
    }
}
