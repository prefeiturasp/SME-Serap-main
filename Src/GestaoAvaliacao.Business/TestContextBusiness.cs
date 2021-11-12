using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Business
{
    public class TestContextBusiness : ITestContextBusiness
    {
        private readonly ITestContextRepository testContextRepository;

        public TestContextBusiness(ITestContextRepository testContextRepository)
        {
            this.testContextRepository = testContextRepository;
        }

        public TestContext Delete(long Id)
        {
            TestContext entity = new TestContext { Id = Id };
            testContextRepository.Delete(Id);
            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Contexto da Prova excluído com sucesso.";

            return entity;
        }

        public TestContext Save(TestContext entity)
        {
            DateTime dateNow = DateTime.Now;

            entity.CreateDate = dateNow;
            entity.UpdateDate = dateNow;

            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = testContextRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Contexto da Prova salvo com sucesso.";
            }

            return entity;
        }

        private Validate Validate(TestContext entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;


            if (entity.Title.Length > 100)
            {
                valid.Message = "O título não pode conter mais que 100 caracteres.";
            }

            if (entity.Text.Length > 500)
            {
                valid.Message = "O texto não pode conter mais que 500 caracteres.";
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

        public TestContext Update(long Id, TestContext entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);

            if (entity.Validate.IsValid)
            {
                testContextRepository.Update(Id, entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Contexto da Prova alterado com sucesso.";
            }
            return entity;
        }

        public void DeleteByTestId(long Id)
        {
            testContextRepository.DeleteByTestId(Id);
        }
    }
}
