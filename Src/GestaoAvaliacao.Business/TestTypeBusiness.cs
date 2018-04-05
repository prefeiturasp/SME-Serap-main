using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class TestTypeBusiness : ITestTypeBusiness
	{
		private readonly ITestTypeRepository testTypeRepository;		
		private readonly ITestBusiness testBusiness;

		public TestTypeBusiness(ITestTypeRepository testTypeRepository, ITestBusiness testBusiness)
		{
			this.testTypeRepository = testTypeRepository;			
			this.testBusiness = testBusiness;
		}

		#region Custom

		private Validate Validate(TestType entity, ValidateAction action, Guid ent_id, Validate valid)
		{
			valid.Message = null;

            if ((action == ValidateAction.Save || action == ValidateAction.Update)
                && (entity == null
                    || string.IsNullOrEmpty(entity.Description)
                    || entity.TypeLevelEducationId <= 0
                    || (entity.FormatType == null && (entity.TestTypeItemLevel != null && entity.TestTypeItemLevel.Count() > 0))
                    || (entity.FormatType != null && (entity.TestTypeItemLevel == null || (entity.TestTypeItemLevel != null && entity.TestTypeItemLevel.Count() == 0)))
                    ))
            {
                valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
            }

            if (action == ValidateAction.Save && (testTypeRepository.ExistsDescriptionNamed(entity.Description, 0)))
            {
                valid.Message += "<br/>Esse tipo de prova já existe.";
            }

            if (action == ValidateAction.Update && (testTypeRepository.ExistsDescriptionNamed(entity.Description, entity.Id)))
            {
                valid.Message += "<br/>Esse tipo de prova já existe.";
            }

			if (action == ValidateAction.Delete)
			{
				TestType ent = Get(entity.Id, ent_id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrado o tipo de prova a ser excluído.";
					valid.Code = 404;
				}

				var listTest = testBusiness.GetByTestType(entity.Id);
				if (listTest.Count > 0)
					valid.Message += "<br/>Esse tipo de prova não pode ser excluído pois está sendo utilizado em alguma prova.";
			}

			if (!string.IsNullOrEmpty(valid.Message))
			{
				string br = "<br/>";
				valid.Message = valid.Message.TrimStart(br.ToCharArray());

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

		public TestType Get(long Id, Guid EntityId)
		{
			return testTypeRepository.Get(Id, EntityId);
		}

		public IEnumerable<TestType> Load(ref Pager pager, Guid EntityId)
		{
			return testTypeRepository.Load(ref pager, EntityId);
		}

		public IEnumerable<TestType> Search(string search, ref Pager pager, Guid EntityId)
		{
			return testTypeRepository.Search(search, ref pager, EntityId);
		}

		public IEnumerable<TestType> LoadByUserGroup(Guid EntityId, bool IsAdmin)
		{
			if (IsAdmin)
				return testTypeRepository.LoadAll(EntityId);
			else
				return testTypeRepository.LoadNotGlobal(EntityId);
		}

        public bool ExistsTestAssociated(long Id)
        {
            return testTypeRepository.ExistsTestAssociated(Id);
        }

        public IEnumerable<TestType> LoadFiltered(Guid EntityId, bool IsAdmin)
        {
            return testTypeRepository.LoadFiltered(EntityId, IsAdmin);
        }

        #endregion

        #region Write

        public TestType Save(TestType entity, Guid ent_id)
		{
			entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				entity.EntityId = ent_id;
				entity = testTypeRepository.Save(entity);
				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Tipo de prova salvo com sucesso.";
			}

			return entity;
		}

		public TestType Update(long Id, TestType entity, Guid ent_id)
		{
			entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				entity.Id = Id;
				testTypeRepository.Update(entity);
				entity.Validate.Type = ValidateType.Update.ToString();
				entity.Validate.Message = "Tipo de prova alterado com sucesso.";
			}

			return entity;
		}

		public TestType Delete(long id, Guid ent_id)
		{
			TestType entity = new TestType { Id = id };
			entity.Validate = Validate(entity, ValidateAction.Delete, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				testTypeRepository.Delete(entity.Id);
				entity.Validate.Type = ValidateType.Delete.ToString();
				entity.Validate.Message = "Tipo de prova excluído com sucesso.";
			}

			return entity;
		}

		public void UnsetModelTest(long modelTestId)
		{
			var testTypeList = testTypeRepository.GetByModelTest(modelTestId);

			if (testTypeList != null && testTypeList.Count() > 0)
			{
				foreach (var item in testTypeList)
					testTypeRepository.UnsetModelTest(item);
			}
		}

		#endregion
	}
}
