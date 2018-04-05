using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
	public class TestGroupBusiness : ITestGroupBusiness
	{
		private readonly ITestGroupRepository testGroupRepository;
		private readonly ITestBusiness testBusiness;

        public TestGroupBusiness(ITestGroupRepository testGroupRepository, ITestBusiness testBusiness)
		{
			this.testGroupRepository = testGroupRepository;
			this.testBusiness = testBusiness;
        }

		#region Custom

		private Validate Validate(TestGroup entity, ValidateAction action, Guid ent_id, Validate valid)
		{
			valid.Message = null;
			if (action == ValidateAction.Save)
			{
				if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.TestSubGroups.Count < 1)
					valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um subgrupo.";

				if (testGroupRepository.ExistsModelDescription(entity.Description, ent_id))
					valid.Message += "<br/>Já existe um grupo com essa descrição cadastrada.";
			}

			if (action == ValidateAction.Update)
			{
				if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.TestSubGroups.Count < 1)
					valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um subgrupo.";

				if (testGroupRepository.ExistsModelDescriptionUpdate(entity.Description, entity.Id, ent_id))
					valid.Message += "<br/>Já existe um grupo com essa descrição cadastrada.";

				//if (testGroupRepository.ExistsTestRelatedSubGroup(entity))
				//    valid.Message += "<br/>O(s) subgrupo(s) removido(s) está(ão) relacionado(s) a ao menos uma prova e não pode(m) ser removido(s).";
			}

			if (action == ValidateAction.Delete)
			{
				TestGroup ent = Get(entity.Id);
				if (ent == null)
				{
					valid.Message = "Não foi encontrado o grupo a ser excluído.";
					valid.Code = 404;
				}

				if (testGroupRepository.ExistsTestRelated(entity))
					valid.Message += "<br/>O grupo está relacionado à uma prova e não pode ser excluído.";
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

		public TestGroup Get(long id)
		{
			return testGroupRepository.Get(id);
		}

		public TestGroup GetTestGroup(long id)
		{
			return testGroupRepository.GetTestGroup(id);
		}

		public IEnumerable<TestGroup> Load(Guid ent_id)
		{
			return testGroupRepository.Load(ent_id);
		}
		public IEnumerable<TestGroup> LoadByPermissionTest(TestFilter filter)
		{
			var tests = testBusiness.GetTestByDateWithGroup(filter);
			var groups = (from t in tests
						  select new TestGroup
						  {
							  Id = t.TestGroup.Id,
							  Description = t.TestGroup.Description
						  }).ToList();

			return groups.GroupBy(g => g.Id)
				   .Select(g => g.First())
				   .ToList(); ;

		}

		public IEnumerable<TestGroup> LoadPaginate(ref Pager pager, Guid ent_id)
		{
			return testGroupRepository.LoadPaginate(ref pager, ent_id);
		}

		public IEnumerable<TestGroup> Search(ref Pager pager, Guid ent_id, string search = null, int levelQntd = 0)
		{
			return testGroupRepository.Search(ref pager, ent_id, search, levelQntd);
		}

		public IEnumerable<TestGroup> LoadGroupsSubGroups(Guid ent_id)
		{
			return testGroupRepository.LoadGroupsSubGroups(ent_id);
		}

		public bool VerifyDeleteSubGroup(long id)
		{
			return testGroupRepository.VerifyDeleteSubGroup(id);
		}

		#endregion

		#region Write

		public TestGroup Save(TestGroup entity, Guid ent_id)
		{
			entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				entity.EntityId = ent_id;
				entity = testGroupRepository.Save(entity);
				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Grupo salvo com sucesso.";
			}

			return entity;
		}

		public TestGroup Update(TestGroup entity, Guid ent_id)
		{
			entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				entity.EntityId = ent_id;
				testGroupRepository.Update(entity);
				entity.Validate.Type = ValidateType.Update.ToString();
				entity.Validate.Message = "Grupo alterado com sucesso.";
			}

			return entity;
		}

		public TestGroup Delete(long id, Guid ent_id)
		{
			TestGroup entity = new TestGroup { Id = id };
			entity.Validate = Validate(entity, ValidateAction.Delete, ent_id, entity.Validate);
			if (entity.Validate.IsValid)
			{
				testGroupRepository.Delete(entity);
				entity.Validate.Type = ValidateType.Delete.ToString();
				entity.Validate.Message = "Grupo excluído com sucesso.";
			}

			return entity;
		}

        #endregion
    }
}
