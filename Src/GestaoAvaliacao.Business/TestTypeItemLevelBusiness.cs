using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class TestTypeItemLevelBusiness : ITestTypeItemLevelBusiness
    {
        private readonly ITestTypeItemLevelRepository testTypeItemLevelRepository;

        public TestTypeItemLevelBusiness(ITestTypeItemLevelRepository testTypeItemLevelRepository)
        {
            this.testTypeItemLevelRepository = testTypeItemLevelRepository;
        }

        #region Custom

        private Validate Validate(TestTypeItemLevel entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save && (entity == null || entity.Value == null))
            {
                valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
            }

            if (action == ValidateAction.Update && (entity.Value == null))
            {
                valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
            }

            if (!string.IsNullOrEmpty(valid.Message))
            {
                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        #endregion

        #region Read

        public TestTypeItemLevel Get(long Id)
        {
            return testTypeItemLevelRepository.Get(Id);
        }

        public TestTypeItemLevel LoadByTestType(long testTypeId)
        {
            return testTypeItemLevelRepository.LoadByTestType(testTypeId);
        }

        public IEnumerable<TestTypeItemLevel> Load()
        {
            return testTypeItemLevelRepository.Load();
        }

        #endregion

        #region Write

        public TestTypeItemLevel Save(TestTypeItemLevel entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = testTypeItemLevelRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Motivo de ausência salvo com sucesso.";
            }

            return entity;
        }

        public TestTypeItemLevel Update(long Id, TestTypeItemLevel entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = Id;
                testTypeItemLevelRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Motivo de ausência alterado com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
