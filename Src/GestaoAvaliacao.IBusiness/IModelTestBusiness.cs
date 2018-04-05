using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IModelTestBusiness
	{
		ModelTest Save(ModelTest entity);
		ModelTest Update(ModelTest entity);
		ModelTest Delete(long id, Guid ent_id);
		ModelTest Get(long id);
		ModelTest GetDefault(Guid EntityId);
		IEnumerable<ModelTest> Search(ref Pager pager, Guid entityid, string search);
		IEnumerable<ModelTest> FindSimple(Guid EntityId);
		void RemoveFileFromEntity(long fileId);
	}
}
