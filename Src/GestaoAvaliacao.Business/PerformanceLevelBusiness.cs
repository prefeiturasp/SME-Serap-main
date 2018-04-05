using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class PerformanceLevelBusiness : IPerformanceLevelBusiness
    {
        private readonly IPerformanceLevelRepository performanceLevelRepository;
        private readonly ITestPerformanceLevelBusiness testPerformanceLevelBusiness;

        public PerformanceLevelBusiness(IPerformanceLevelRepository performanceLevelRepository, ITestPerformanceLevelBusiness testPerformanceLevelBusiness)
        {
            this.performanceLevelRepository = performanceLevelRepository;
            this.testPerformanceLevelBusiness = testPerformanceLevelBusiness;
        }

        #region Custom

        private Validate Validate(PerformanceLevel entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || string.IsNullOrEmpty(entity.Code))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                if (performanceLevelRepository.ExistsDescription(entity.Description, ent_id))
                    valid.Message += "<br/>Já existe um nível de desempenho com essa descrição cadastrada.";

                if (performanceLevelRepository.ExistsCode(entity.Code, ent_id))
                    valid.Message += "<br/>Já existe um nível de desempenho com esse código cadastrado.";
            }

            if (action == ValidateAction.Update)
            {
                PerformanceLevel ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o nível de desempenho a ser atualizado.";
                    valid.Code = 404;
                }

                if (string.IsNullOrEmpty(entity.Description) || string.IsNullOrEmpty(entity.Code))
                    valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";

                if (performanceLevelRepository.ExistsDescriptionAlter(entity.Description, entity.Id, ent_id))
                    valid.Message += "<br/>Já existe um nível de desempenho com essa descrição cadastrada.";

                if (performanceLevelRepository.ExistsCodeAlter(entity.Code, entity.Id, ent_id))
                    valid.Message += "<br/>Já existe um nível de desempenho com esse código cadastrado.";
            }

            if (action == ValidateAction.Delete)
            {
                PerformanceLevel ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o nível de desempenho a ser excluído.";
                    valid.Code = 404;
                }

                var listTest = testPerformanceLevelBusiness.ExistsTestPerformanceLevel(entity.Id);
                if (listTest)
                    valid.Message += "<br/>Esse nível de desempenho não pode ser excluído pois está sendo utilizado em alguma prova.";
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

        public IEnumerable<PerformanceLevel> Search(string search, ref Pager pager, Guid EntityId)
        {
            return performanceLevelRepository.Search(search, ref pager, EntityId);
        }

        public IEnumerable<PerformanceLevel> GetAll(Guid EntityId)
        {
            return performanceLevelRepository.GetAll(EntityId);
        }

        public IEnumerable<PerformanceLevel> LoadLevels(Guid EntityId)
        {
            return performanceLevelRepository.LoadLevels(EntityId);
        }

        public IEnumerable<PerformanceLevel> Load(ref Pager pager, Guid EntityId)
        {
            return performanceLevelRepository.Load(ref pager, EntityId);
        }

        public PerformanceLevel Get(long id)
        {
            return performanceLevelRepository.Get(id);
        }

        #endregion

        #region Write

        public PerformanceLevel Save(PerformanceLevel entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                entity = performanceLevelRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Nível de desempenho salvo com sucesso.";
            }

            return entity;
        }

        public PerformanceLevel Update(long id, PerformanceLevel entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = id;
                performanceLevelRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Nível de desempenho alterado com sucesso.";
            }

            return entity;
        }

        public PerformanceLevel Delete(long id)
        {
            PerformanceLevel entity = new PerformanceLevel { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.EntityId, entity.Validate);
            if (entity.Validate.IsValid)
            {
                performanceLevelRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Nível de desempenho excluído com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
