using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemTypeRepository
	{
		ItemType Save(ItemType entity);
		void Update(ItemType entity);
		ItemType Get(long id);
		IEnumerable<ItemType> FindSimple(Guid EntityId);
        IEnumerable<ItemType> FindForTestType(Guid EntityId);
		IEnumerable<ItemType> Search(ref Pager pager, Guid EntityId, string search);
		void Delete(long id);
		bool ExistsDescriptionNamed(string description, long Id, Guid ent_id);
		void VerifyDefault(Guid EntityId);
		bool ExistsItems(long id);
	}
}
