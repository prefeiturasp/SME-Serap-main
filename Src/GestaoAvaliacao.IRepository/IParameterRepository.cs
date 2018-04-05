using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IParameterRepository
	{

		Parameter GetByKey(string key, Guid EntityId);
        Parameter GetByKey(string key);

        IEnumerable<Parameter> GetParametersImage(Guid EntityId);

		IEnumerable<Parameter> GetParamsByPage(long PageId);

		IEnumerable<Parameter> GetAll();

		void Update(List<Parameter> parameters);
        Parameter GetParamByKey(string key, Guid EntityId);


    }
}
