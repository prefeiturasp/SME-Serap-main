using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IParameterBusiness
	{
		Parameter GetByKey(string key, Guid EntityId);
        Parameter GetByKey(string key);

        IEnumerable<Parameter> GetParametersImage(Guid EntityId);

		IEnumerable<Parameter> GetParamsByPage(long PageId);

		Parameter Update(List<Parameter> parameters);

		IEnumerable<Parameter> GetAll();
        Parameter GetParamByKey(string key, Guid EntityId);


    }
}
