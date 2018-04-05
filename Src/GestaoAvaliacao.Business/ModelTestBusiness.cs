using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class ModelTestBusiness : IModelTestBusiness
	{
		private readonly IFileBusiness fileBusiness;
		private readonly IModelTestRepository modelTestRepository;
		private readonly ITestTypeRepository testTypeRepository;

		public ModelTestBusiness(IModelTestRepository modelTestRepository, IFileBusiness fileBusiness, ITestTypeRepository testTypeRepository)
		{
			this.modelTestRepository = modelTestRepository;
			this.fileBusiness = fileBusiness;
			this.testTypeRepository = testTypeRepository;
		}

		#region Custom

		private Validate Validate(ModelTest entity, ValidateAction validateAction, Validate valid)
		{
			if (validateAction == ValidateAction.Save)
			{
				if (entity != null)
				{
					if (string.IsNullOrEmpty(entity.Description))
						valid.Message = "O nome do modelo é obrigatório.";

					if (modelTestRepository.ExistsDescriptionNamed(entity.EntityId, entity.Id, entity.Description))
						valid.Message = "<br/>Já existe um modelo de prova com este nome.";
				}
				else
					valid.Message = "Não foi preenchido o campo obrigatório.";
			}

			if (validateAction == ValidateAction.Update)
			{
				var cadastredModel = modelTestRepository.Get(entity.Id);

				if (!entity.EntityId.Equals(cadastredModel.EntityId))
					valid.Message = "<br/>Modelo de prova não encontrado.";
			}

			if (validateAction == ValidateAction.Delete)
			{
				ModelTest cadastedModel = modelTestRepository.Get(entity.Id);

				if (cadastedModel != null)
				{
					if (cadastedModel.DefaultModel)
						valid.Message = "Este modelo de prova não pode ser removido, pois é o modelo global.";

					if (!cadastedModel.EntityId.Equals(entity.EntityId))
						valid.Message = "<br/>Modelo de prova não encontrado.";
				}
				else
				{
					valid.Code = 404;
					valid.Message = "Modelo de prova não encontrado.";
				}
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

		private void VerifyDefaultModel(ref ModelTest entity)
		{
			bool existsDefault = modelTestRepository.ExistsAnotherDefaultModel(entity.EntityId, entity.Id);
			if (!entity.DefaultModel && !existsDefault)
				entity.DefaultModel = true;

			//Caso este modelo seja padrão e ja exista um outro modelo padrão, tornar o outro modelo normal
			else if (entity.DefaultModel && existsDefault)
				modelTestRepository.UnsetDefaultModel(entity.EntityId);
		}

		#endregion

		#region Read

		public ModelTest Get(long id)
		{
			return modelTestRepository.Get(id);
		}

		public ModelTest GetDefault(Guid EntityId)
		{
			return modelTestRepository.GetDefault(EntityId);
		}

		public IEnumerable<ModelTest> Search(ref Pager pager, Guid entityid, string search)
		{
			return modelTestRepository.Search(ref pager, entityid, search);
		}

		public IEnumerable<ModelTest> FindSimple(Guid Entityid)
		{
			return modelTestRepository.FindSimple(Entityid);
		}


		public void RemoveFileFromEntity(long fileId)
		{
			modelTestRepository.RemoveFileFromEntity(fileId);
		}

		#endregion

		#region Write

		public ModelTest Save(ModelTest entity)
		{
			entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
			if (entity.Validate.IsValid)
			{
				VerifyDefaultModel(ref entity);

				List<File> listFiles = entity.Files != null ? entity.Files : new List<File>();

                if (entity.LogoHeader != null && entity.LogoHeader.Id > 0)
                {
                    listFiles.Add(entity.LogoHeader);
                    entity.FileHeader_Id = entity.LogoHeader.Id;
                }

                if (entity.LogoFooter != null && entity.LogoFooter.Id > 0)
                {
                    listFiles.Add(entity.LogoFooter);
                    entity.FileFooter_Id = entity.LogoFooter.Id;
                }

				entity.LogoHeader = null;
				entity.LogoFooter = null;

				entity = modelTestRepository.Save(entity);

				if (listFiles != null && listFiles.Count > 0)
					fileBusiness.AssociateFilesToEntity(entity.Id, listFiles);

				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Modelo de prova salvo com sucesso.";
			}

			return entity;
		}

		public ModelTest Update(ModelTest entity)
		{
			entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
			if (entity.Validate.IsValid)
			{
				VerifyDefaultModel(ref entity);

				List<File> listFiles = entity.Files != null ? entity.Files : new List<File>();

				if (entity.LogoHeader != null && entity.LogoHeader.Id > 0)
				{
					listFiles.Add(entity.LogoHeader);
					entity.FileHeader_Id = entity.LogoHeader.Id;
                }
                else
                    entity.FileHeader_Id = null;

				if (entity.LogoFooter != null && entity.LogoFooter.Id > 0)
				{
					listFiles.Add(entity.LogoFooter);
					entity.FileFooter_Id = entity.LogoFooter.Id;
                }
                else
                    entity.FileFooter_Id = null;

				entity.LogoHeader = null;
				entity.LogoFooter = null;

				modelTestRepository.Update(entity);

				if (listFiles != null && listFiles.Count > 0)
					fileBusiness.AssociateFilesToEntity(entity.Id, listFiles);

				fileBusiness.VerifyUnusedFilesByOwner(entity.Id, entity.Id, EnumFileType.ModelTestHeader, listFiles);
                fileBusiness.VerifyUnusedFilesByOwner(entity.Id, entity.Id, EnumFileType.ModelTestFooter, listFiles);

				entity.Validate.Type = ValidateType.Save.ToString();
				entity.Validate.Message = "Modelo de prova salvo com sucesso.";
			}

			return entity;
		}

		public ModelTest Delete(long id, Guid ent_id)
		{
			ModelTest entity = new ModelTest() { Id = id, EntityId = ent_id };

			entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
			if (entity.Validate.IsValid)
			{
				UnsetModelTestOnTestType(id);

				modelTestRepository.Delete(id);
				entity.Validate.Type = ValidateType.Delete.ToString();
				entity.Validate.Message = "Modelo de prova excluído com sucesso.";
			}

			return entity;
		}


		private void UnsetModelTestOnTestType(long id)
		{
			var testTypeList = testTypeRepository.GetByModelTest(id);

			if (testTypeList != null && testTypeList.Count() > 0)
			{
				foreach (var item in testTypeList)
					testTypeRepository.UnsetModelTest(item);
			}
		}
		#endregion
	}
}
