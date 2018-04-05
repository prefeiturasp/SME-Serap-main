using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public class ParameterBusiness : IParameterBusiness
    {
        private readonly IParameterRepository parameterRepository;

        public ParameterBusiness(IParameterRepository parameterRepository)
        {
            this.parameterRepository = parameterRepository;
        }

        #region Custom

        private Validate Validate(List<Parameter> parameters, ValidateAction action, Validate valid)
        {
            valid.Message = null;

            if (action == ValidateAction.Delete)
            {
                foreach (Parameter ent in parameters)
                {
                    if (string.IsNullOrEmpty(ent.Value))
                    {
                        valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(valid.Message))
            {
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

        public Parameter GetByKey(string key, Guid EntityId)
        {
            return parameterRepository.GetByKey(key, EntityId);
        }

        public Parameter GetByKey(string key)
        {
            return parameterRepository.GetByKey(key);
        }

        public IEnumerable<Parameter> GetParametersImage(Guid EntityId)
        {
            return parameterRepository.GetParametersImage(EntityId);
        }

        public IEnumerable<Parameter> GetParamsByPage(long PageId)
        {
            return parameterRepository.GetParamsByPage(PageId);
        }

		public IEnumerable<Parameter> GetAll()
		{
			return parameterRepository.GetAll();
			
		}

        public Parameter GetParamByKey(string key, Guid EntityId)
        {
            return parameterRepository.GetParamByKey(key, EntityId);
        }

        #endregion

        #region Write

        public Parameter Update(List<Parameter> entities)
        {
            Parameter entity = new Parameter();
            entity.Validate = Validate(entities, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                parameterRepository.Update(entities);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Parâmetro salvo com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
