using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class CorrelatedSkillBusiness : ICorrelatedSkillBusiness
    {
        private readonly ICorrelatedSkillRepository correlatedSkillRepository;

        public CorrelatedSkillBusiness(ICorrelatedSkillRepository correlatedSkillRepository)
        {
            this.correlatedSkillRepository = correlatedSkillRepository;
        }

        #region Custom

        private Validate Validate(CorrelatedSkill entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                if (correlatedSkillRepository.ExistsCorrelated(entity))
                    valid.Message += "<br/>Essa correlação de habilidade já existe.";
            }

            if (action == ValidateAction.Delete)
            {
                CorrelatedSkill ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a correlação de habilidade a ser excluída.";
                    valid.Code = 404;
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

        #endregion

        #region Read

        public CorrelatedSkill Get(long Id)
        {
            return correlatedSkillRepository.Get(Id);
        }

        public List<CorrelatedSkillByEvaluationMatrix> LoadList(long MatrizId, ref Pager pager)
        {
            return correlatedSkillRepository.LoadList(MatrizId, ref pager);
        }

        public List<Skill> LoadCorrelatedSkills(long skillId)
        {
            return correlatedSkillRepository.LoadCorrelatedSkills(skillId);
        }

        public bool ExistsCorrelated(CorrelatedSkill entity)
        {
            return correlatedSkillRepository.ExistsCorrelated(entity);
        }

        #endregion

        #region Write

        public CorrelatedSkill Save(CorrelatedSkill entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = correlatedSkillRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Correlação de habilidade salva com sucesso.";
            }

            return entity;
        }

        public CorrelatedSkill Delete(long id)
        {
            CorrelatedSkill entity = new CorrelatedSkill { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                correlatedSkillRepository.Delete(entity.Id);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Correlação de habilidade excluída com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
