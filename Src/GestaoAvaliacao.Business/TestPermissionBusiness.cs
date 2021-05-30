using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class TestPermissionBusiness : ITestPermissionBusiness
    {
        private readonly ITestPermissionRepository testPermissionRepository;
        public TestPermissionBusiness(ITestPermissionRepository testPermissionRepository)
        {
            this.testPermissionRepository = testPermissionRepository;
        }

        #region Read

        public IEnumerable<TestPermission> GetByTest(long test_id, Guid? gru_id)
        {
            return testPermissionRepository.GetByTest(test_id, gru_id);
        }

        public async Task<List<TestPermission>> GetPermissionByTest(long test_id, Guid? gru_id)
        {
            return await testPermissionRepository.GetPermissionByTest(test_id, gru_id);
        }

        #endregion

        #region Write

        public TestPermission Save(TestPermission entity, List<TestPermission> permissions)
        {
            testPermissionRepository.Save(entity.Test_Id, permissions);
            entity.Validate.Type = ValidateType.Save.ToString();
            entity.Validate.Message = "Permissões salvas com sucesso.";

            return entity;
        }

        #endregion
    }
}
