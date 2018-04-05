using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class AbsenceReasonBusiness : IAbsenceReasonBusiness
    {
        private readonly IAbsenceReasonRepository absenceReasonRepository;

        public AbsenceReasonBusiness(IAbsenceReasonRepository absenceReasonRepository)
        {
            this.absenceReasonRepository = absenceReasonRepository;
        }

        #region Custom

        private Validate Validate(AbsenceReason entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description))
                    valid.Message = "Não foi preenchido o campo obrigatório.";

                if (absenceReasonRepository.ExistsDescriptionNamed(entity.Description, ent_id))
                    valid.Message += "<br/>Esse motivo de ausência já existe.";
            }

            if (action == ValidateAction.Update)
            {
                AbsenceReason ent = Get(entity.Id, ent_id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o motivo de ausência a ser alterado.";
                    valid.Code = 404;
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.Description))
                        valid.Message = "Não foi preenchido o campo obrigatório.";

                    if (absenceReasonRepository.ExistsDescriptionNamedAlter(entity.Description, Convert.ToInt16(entity.Id), ent_id))
                        valid.Message += "<br/>Esse motivo de ausência já existe.";

                    if (entity.IsDefault && !entity.State.Equals(Convert.ToByte(EnumState.ativo)))
                        valid.Message += "<br/>Não é permitido inativar o motivo de ausência padrão.";

                    if (ent.IsDefault && !entity.IsDefault)
                        valid.Message += "<br/>Altere primeiro o motivo de ausência que deseja ser o motivo padrão.";
                }
            }

            if (action == ValidateAction.Delete)
            {
                AbsenceReason ent = Get(entity.Id, ent_id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o motivo de ausência a ser excluído.";
                    valid.Code = 404;
                }
                else if (ent.IsDefault) { 
                    valid.Message += "<br/>Altere primeiro o motivo de ausência que deseja ser o motivo padrão.";
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

        public AbsenceReason Get(long Id, Guid entityid)
        {
            return absenceReasonRepository.Get(Id, entityid);
        }

        public AbsenceReason GetDefault(Guid EntityId)
        {
            return absenceReasonRepository.GetDefault(EntityId);
        }

        public IEnumerable<AbsenceReason> Get(Guid EntityId)
        {
            return absenceReasonRepository.Get(EntityId);
        }

        public IEnumerable<AbsenceReason> Load(ref Pager pager, Guid entityid)
        {
            return absenceReasonRepository.Load(ref pager, entityid);
        }

        public IEnumerable<AbsenceReason> Search(string search, ref Pager pager, Guid entityid)
        {
            return absenceReasonRepository.Search(search, ref pager, entityid);
        }

        #endregion

        #region Write

        public AbsenceReason Save(AbsenceReason entity, Guid entityid)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, entityid, entity.Validate);
            if (entity.Validate.IsValid)
            {
                if (entity.IsDefault)
                    VerifyDefault(entityid);

                entity.EntityId = entityid;
                entity = absenceReasonRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Motivo de ausência salvo com sucesso.";
            }

            return entity;
        }

        public AbsenceReason Update(long Id, AbsenceReason entity, Guid entityid)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entityid, entity.Validate);
            if (entity.Validate.IsValid)
            {
                if (entity.IsDefault)
                    VerifyDefault(entityid);

                entity.Id = Id;
                absenceReasonRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Motivo de ausência alterado com sucesso.";
            }

            return entity;
        }

        public AbsenceReason Delete(long id, Guid entityid)
        {
            AbsenceReason entity = Get(id, entityid);

            entity.Validate = Validate(entity, ValidateAction.Delete, entityid, entity.Validate);
            if (entity.Validate.IsValid)
            {
                absenceReasonRepository.Delete(entity.Id);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Motivo de ausência excluído com sucesso.";
            }

            return entity;
        }

        private void VerifyDefault(Guid guid)
        {
            absenceReasonRepository.VerifyDefault(guid);
        }

		#endregion
	}
}
