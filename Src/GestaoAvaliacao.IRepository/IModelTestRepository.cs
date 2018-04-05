using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IModelTestRepository
	{
		ModelTest Save(ModelTest entity);
		void Update(ModelTest entity);
		void Delete(long id);
		ModelTest Get(long id);
		ModelTest GetDefault(Guid EntityId);
		IEnumerable<ModelTest> Search(ref Pager pager, Guid EntityId, string search);
		bool ExistsAnotherDefaultModel(Guid EntityId, long Id);
		void UnsetDefaultModel(Guid EntityId);
		bool ExistsDescriptionNamed(Guid EntityId, long Id, string description);
		IEnumerable<ModelTest> FindSimple(Guid EntityId);
		void RemoveFileFromEntity(long fileId);
	}
}
