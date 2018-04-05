using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ModelEvaluationMatrixBusiness : IModelEvaluationMatrixBusiness
    {    
        private readonly IModelEvaluationMatrixRepository modelEvaluationMatrixRepository;

        public ModelEvaluationMatrixBusiness(IModelEvaluationMatrixRepository modelEvaluationMatrixRepository)
        {
            this.modelEvaluationMatrixRepository = modelEvaluationMatrixRepository;
        }

        #region Custom

        private Validate Validate(ModelEvaluationMatrix entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.ModelSkillLevels.Count < 1)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um nível.";

                if (modelEvaluationMatrixRepository.ExistsModelDescription(entity.Description, ent_id))
                    valid.Message += "<br/>Já existe um modelo de matriz de avaliação com essa descrição cadastrada.";
            }

            if (action == ValidateAction.Update)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.ModelSkillLevels.Count < 1)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um nível.";

                if (modelEvaluationMatrixRepository.ExistsModelDescriptionUpdate(entity.Description, entity.Id, ent_id))
                    valid.Message += "<br/>Já existe um modelo de matriz de avaliação com essa descrição cadastrada.";

                if (modelEvaluationMatrixRepository.ExistsMatrixRelated(entity) && modelEvaluationMatrixRepository.IsDeletedModelSkillBeenUsed(entity))
                    valid.Message += "<br/>O(s) nível(eis) removido(s) está(ão) relacionado(s) a ao menos uma matriz de avaliação e não pode(m) ser removido(s).";
            }

            if (action == ValidateAction.Delete)
            {
                ModelEvaluationMatrix ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o modelo de matriz de avaliação a ser excluído.";
                    valid.Code = 404;
                }

                if (modelEvaluationMatrixRepository.ExistsMatrixRelated(entity))
                    valid.Message += "<br/>O modelo está relacionado à uma matriz de avaliação e não pode ser excluído.";
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

        public ModelEvaluationMatrix Get(long id)
        {
            return modelEvaluationMatrixRepository.Get(id);
        }

        public ModelEvaluationMatrix GetModelEvaluationMatrix(long id)
        {
            return modelEvaluationMatrixRepository.GetModelEvaluationMatrix(id);
        }

        public IEnumerable<ModelEvaluationMatrix> Load(Guid ent_id)
        {
            return modelEvaluationMatrixRepository.Load(ent_id);
        }

        public IEnumerable<ModelEvaluationMatrix> LoadPaginate(ref Pager pager, Guid ent_id)
        {
            return modelEvaluationMatrixRepository.LoadPaginate(ref pager, ent_id);
        }

        public IEnumerable<ModelEvaluationMatrix> Search(ref Pager pager, Guid ent_id, string search = null, int levelQntd = 0)
        {
            return modelEvaluationMatrixRepository.Search(ref pager, ent_id, search, levelQntd);
        }

        #endregion

        #region Write

        public ModelEvaluationMatrix Save(ModelEvaluationMatrix entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                entity = modelEvaluationMatrixRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Modelo de matriz de avaliação salvo com sucesso.";
            }

            return entity;
        }

        public ModelEvaluationMatrix Update(ModelEvaluationMatrix entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                modelEvaluationMatrixRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Modelo de matriz de avaliação alterado com sucesso.";
            }

            return entity;
        }

        public ModelEvaluationMatrix Delete(long id, Guid ent_id)
        {
            ModelEvaluationMatrix entity = new ModelEvaluationMatrix { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                modelEvaluationMatrixRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Modelo de matriz de avaliação excluído com sucesso.";
            }

            return entity;
        }
        
        #endregion
    }
}
