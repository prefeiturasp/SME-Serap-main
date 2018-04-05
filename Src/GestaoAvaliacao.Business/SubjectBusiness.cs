using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class SubjectBusiness : ISubjectBusiness
    {
        private readonly ISubjectRepository subjectRepository;

        public SubjectBusiness(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }

        #region Custom

        private Validate Validate(Subject entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.SubSubjects.Count < 1)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um subgrupo.";

                if (subjectRepository.ExistsDescription(entity.Description, ent_id))
                    valid.Message += "<br/>Já existe um assunto com essa descrição cadastrado.";
            }

            if (action == ValidateAction.Update)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || entity.SubSubjects.Count < 1)
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios, ou não foi cadastrado ao menos um subgrupo.";

                if (subjectRepository.ExistsItemRelated(entity, ent_id) && subjectRepository.IsDeletedSubSubjectBeenUsed(entity))
                    valid.Message += "<br/>O(s) subassuntos(s) removido(s) está(ão) relacionado(s) a ao menos um item e não pode(m) ser removido(s).";
            }

            if (action == ValidateAction.Delete)
            {
                Subject ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o assunto a ser excluído.";
                    valid.Code = 404;
                }

                if (subjectRepository.ExistsItemRelated(entity, ent_id))
                    valid.Message += "<br/>O assunto está relacionado à um item e não pode ser excluído.";
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

        public Subject Get(long id)
        {
            return subjectRepository.Get(id);
        }

        public List<Discipline> SearchSubjects(string assunto, string subassunto, Guid ent_id, ref Pager pager)
        {
            return subjectRepository.SearchSubjects(assunto, subassunto, ent_id, ref pager);
        }

        public Subject GetSubject(long id)
        {
            return subjectRepository.GetSubject(id);
        }

        public Subject LoadSubjectBySubsubject(long idSubsubject)
        {
            return subjectRepository.LoadSubjectBySubsubject(idSubsubject);
        }

        public List<AJX_Select2> LoadAllSubjects(string description, Guid EntityId)
        {
            return subjectRepository.LoadAllSubjects(description, EntityId);
        }

        public List<AJX_Select2> LoadSubsubjectBySubject(string description, string subjects, Guid EntityId)
        {
            return subjectRepository.LoadSubsubjectBySubject(description, subjects, EntityId);
        }

        #endregion

        #region Write

        public Subject Save(Subject entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                entity = subjectRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Grupo salvo com sucesso.";
            }

            return entity;
        }

        public Subject Update(Subject entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                subjectRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Grupo alterado com sucesso.";
            }

            return entity;
        }

        public Subject Delete(long id, Guid ent_id)
        {
            Subject entity = new Subject { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                subjectRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Assunto excluído com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
