using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class CognitiveCompetenceBusiness : ICognitiveCompetenceBusiness
	{
		private readonly ICognitiveCompetenceRepository cognitiveCompetenceRepository;
		private readonly ISkillRepository skillRepository;

		public CognitiveCompetenceBusiness(ICognitiveCompetenceRepository cognitiveCompetenceRepository, ISkillRepository skillRepository)
		{
			this.cognitiveCompetenceRepository = cognitiveCompetenceRepository;
			this.skillRepository = skillRepository;
		}

        #region Custom

        private Validate Validate(CognitiveCompetence entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description))
                    valid.Message = "Não foi preenchido o campo obrigatório.";

                if (cognitiveCompetenceRepository.ExistsDescriptionNamed(entity.Description, ent_id))
                    valid.Message += "<br/>Essa competência cognitiva já existe.";
            }

            if (action == ValidateAction.Update)
            {
                if (string.IsNullOrEmpty(entity.Description))
                    valid.Message = "Não foi preenchido o campo obrigatório.";

                if (cognitiveCompetenceRepository.ExistsDescriptionNamedAlter(entity.Description, Convert.ToInt16(entity.Id), ent_id))
                    valid.Message += "<br/>Essa competência cognitiva já existe.";
            }

            if (action == ValidateAction.Delete)
            {
                CognitiveCompetence ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a competência cognitiva a ser excluída.";
                    valid.Code = 404;
                }

                if (skillRepository.GetByCognitiveCompetence(entity.Id).Count() > 0)
                    valid.Message += "<br/>Essa competência cognitiva está associada a uma habilidade, e não pode ser removida.";
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

        #endregion Custom

        #region Read

        public CognitiveCompetence Get(long id)
        {
            return cognitiveCompetenceRepository.Get(id);
        }

        public IEnumerable<CognitiveCompetence> FindAll(Guid EntityId)
        {
            return cognitiveCompetenceRepository.FindAll(EntityId);
        }

        public IEnumerable<CognitiveCompetence> Load(ref Pager pager, Guid entityid)
        {
            return cognitiveCompetenceRepository.Load(ref pager, entityid);
        }

        public IEnumerable<CognitiveCompetence> Search(string search, ref Pager pager, Guid entityid)
        {
            return cognitiveCompetenceRepository.Search(search, ref pager, entityid);
        }

        #endregion

        #region Write

        public CognitiveCompetence Save(CognitiveCompetence entity, Guid entityid)
		{
            entity.Validate = Validate(entity, ValidateAction.Save, entityid, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = entityid;
                entity = cognitiveCompetenceRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Competência cognitiva salva com sucesso.";
            }

            return entity;
		}

        public CognitiveCompetence Update(long id, CognitiveCompetence entity, Guid entityid)
		{
            entity.Validate = Validate(entity, ValidateAction.Update, entityid, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = id;
                cognitiveCompetenceRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Competência cognitiva alterada com sucesso.";
            }

            return entity;
		}

        public CognitiveCompetence Delete(long id)
        {
            CognitiveCompetence entity = new CognitiveCompetence { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.EntityId, entity.Validate);
            if (entity.Validate.IsValid)
            {
                cognitiveCompetenceRepository.Delete(entity.Id);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Competência cognitiva excluída com sucesso.";
            }

            return entity;
        }

        #endregion
	}
}
