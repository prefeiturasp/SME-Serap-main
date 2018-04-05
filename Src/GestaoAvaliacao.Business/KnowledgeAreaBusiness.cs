using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class KnowledgeAreaBusiness : IKnowledgeAreaBusiness
    {
        private readonly IKnowledgeAreaRepository knowledgeAreaRepository;
        private readonly IDisciplineRepository disciplineRepository;

        public KnowledgeAreaBusiness(IKnowledgeAreaRepository knowledgeAreaRepository, IDisciplineRepository disciplineRepository)
        {
            this.knowledgeAreaRepository = knowledgeAreaRepository;
            this.disciplineRepository = disciplineRepository;
        }

        #region Custom

        private Validate Validate(KnowledgeArea entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save || action == ValidateAction.Update)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description))
                {
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
                }

                if (ExistsModelDescription(entity.Id, entity.Description, entity.EntityId))
                    valid.Message += "<br/>Já existe uma área de conhecimento cadastrada com essa descrição.";
            }
            if (action == ValidateAction.Delete)
            {
                KnowledgeArea ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a área de conhecimento a ser excluída.";
                    valid.Code = 404;
                }

                if (knowledgeAreaRepository.ExistsSubject(entity.Id))
                    valid.Message += "<br/>Não é possível excluir esta área de conhecimento, pois a mesma esta sendo usada.";
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

        public KnowledgeArea Get(long id)
        {
            return knowledgeAreaRepository.Get(id);
        }

        public IEnumerable<KnowledgeArea> Load(ref Pager pager, Guid EntityId)
        {
            return knowledgeAreaRepository.Load(EntityId, ref pager);
        }

        public IEnumerable<KnowledgeArea> Search(string search, ref Pager pager, Guid EntityId)
        {
            return knowledgeAreaRepository.Search(EntityId, search, ref pager);
        }

        public bool ExistsModelDescription(long id, string description, Guid ent_id)
        {
            return knowledgeAreaRepository.ExistsModelDescription(id, description, ent_id);            
        }

        public List<AJX_Select2> LoadAllKnowledgeAreaActive(string description, Guid EntityId)
        {
            return knowledgeAreaRepository.LoadAllKnowledgeAreaActive(description, EntityId);
        }

        #endregion

        #region Write

        public KnowledgeArea Save(KnowledgeArea entity, Guid entityid)
        {
            entity.EntityId = entityid;
            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = knowledgeAreaRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Área de conhecimento salva com sucesso.";
            }

            return entity;
        }

        public KnowledgeArea Update(KnowledgeArea entity, Guid ent_id)
        {
            entity.EntityId = ent_id;
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                knowledgeAreaRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Área de conhecimento alterada com sucesso.";
            }

            return entity;
        }

        public KnowledgeArea Delete(long id)
        {
            KnowledgeArea entity = new KnowledgeArea { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                knowledgeAreaRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Área de conhecimento excluída com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
