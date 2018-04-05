using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class SkillBusiness : ISkillBusiness
	{
		private readonly ISkillRepository skillRepository;

		public SkillBusiness(ISkillRepository skillRepository)
		{
			this.skillRepository = skillRepository;
		}

        #region Custom

        private Validate Validate(Skill entity, long evaluationMatrixId, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || (string.IsNullOrEmpty(entity.Code)))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                if (skillRepository.ExistsCode(entity.Code, entity.ModelSkillLevel.Id, entity.EvaluationMatrix.Id))
                    valid.Message += "<br/>Já existe um código cadastrado com essa descrição.";

                if (!entity.LastLevel && entity.CognitiveCompetence != null && entity.CognitiveCompetence.Id > 0)
                    valid.Message += "<br/>Competência cognitiva é habilitada apenas para o último nível.";
            }

            if (action == ValidateAction.Update)
            {
                Skill ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o registro a ser atualizado.";
                    valid.Code = 404;
                }

                if (string.IsNullOrEmpty(entity.Description) || (string.IsNullOrEmpty(entity.Code)))
                    valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";

                if (!entity.LastLevel && entity.CognitiveCompetence != null && entity.CognitiveCompetence.Id > 0)
                    valid.Message += "<br/>Competência cognitiva é habilitada apenas para o último nível.";

                if (skillRepository.ExistsCodeAlter(entity.Code, entity.ModelSkillLevel.Id, entity.EvaluationMatrix.Id, entity.Id))
                    valid.Message += "<br/>Já existe um código cadastrado com essa descrição.";
            }

            if (action == ValidateAction.Delete)
            {
                Skill ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o registro a ser excluído.";
                    valid.Code = 404;
                }

                if (evaluationMatrixId > 0 && skillRepository.ExistsItemSkill(entity.Id, evaluationMatrixId))
                    valid.Message += "<br/>Não foi possível excluir o registro, pois existem um ou mais itens relacionados a ele.";
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

        public IEnumerable<Skill> Search(string search, ref Pager pager)
        {
            return skillRepository.Search(search, ref pager);
        }

        public Skill Get(long id)
        {
            return skillRepository.Get(id);
        }

        public object GetParent(long id)
        {
            return skillRepository.GetParent(id);
        }

        public IEnumerable<Skill> Load(ref Pager pager)
        {
            return skillRepository.Load(ref pager);
        }

        public IEnumerable<Skill> GetByMatrix(long idMatrix)
        {
            return skillRepository.GetByMatrix(idMatrix);
        }

        public IEnumerable<Skill> GetByDiscipline(long disciplineId)
        {
            return skillRepository.GetByDiscipline(disciplineId);
        }

        public IEnumerable<Skill> GetByParent(long idSkill)
        {
            return skillRepository.GetByParent(idSkill);
        }

        public IEnumerable<Skill> GetComboByDiscipline(long idDiscipline)
        {
            return skillRepository.GetComboByDiscipline(idDiscipline);
        }

        public IEnumerable<Skill> SearchByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager)
        {
            if (parentId <= 0)
                parentId = null;

            return skillRepository.LoadByMatrix(evaluationMatrixId, modelSkillLevelId, parentId, ref pager);
        }

        public IEnumerable<Skill> LoadByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager)
        {
            if (parentId <= 0)
                parentId = null;

            return skillRepository.LoadByMatrix(evaluationMatrixId, modelSkillLevelId, parentId, ref pager);
        }

        public IEnumerable<ItemReportItemSkill> GetBySkillReport(int id, int skill, Guid EntityId, long TypeLevelEducation)
        {
			return skillRepository.GetBySkillReport(id, skill, EntityId, TypeLevelEducation);
        }

		public IEnumerable<ItemReportItemSkill> GetBySkillReportOneLevel(int id, int matrizId, Guid EntityId, long TypeLevelEducation)
		{
			return skillRepository.GetBySkillReportOneLevel(id, matrizId, EntityId, TypeLevelEducation);
		}

        #endregion

        #region Write

        public Skill Save(Skill entity)
        {
            entity.Validate = Validate(entity, 0, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = skillRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
            }

            return entity;
        }

        public Skill Update(long id, Skill entity)
        {
            entity.Validate = Validate(entity, 0, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = id;
                skillRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
            }

            return entity;
        }

        public Skill SaveRange(List<Skill> listEntity)
        {
            Skill entity = new Skill();

            foreach (Skill skill in listEntity)
            {
                entity.Validate = Validate(skill, 0, ValidateAction.Save, entity.Validate);
                if (!entity.Validate.IsValid)
                    break;
            }

            if (entity.Validate.IsValid)
            {
                skillRepository.SaveRange(listEntity);
                entity.Validate.Type = ValidateType.Save.ToString();
            }

            return entity;
        }

        public Skill Delete(long id)
        {
            Skill entity = new Skill { Id = id };
            entity.Validate = Validate(entity, 0, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                skillRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Nível excluído com sucesso.";
            }

            return entity;
        }

        public Skill DeleteByMatrix(long id, long evaluationMatrixId)
        {
            Skill entity = new Skill { Id = id };
            entity.Validate = Validate(entity, evaluationMatrixId, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                skillRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Nível excluído com sucesso.";
            }

            return entity;
        }

        #endregion
	}
}
