using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IItemTypeBusiness
	{
		IEnumerable<ItemType> FindSimple(Guid EntityId);
        IEnumerable<ItemType> FindForTestType(Guid EntityId);
		IEnumerable<ItemType> Search(ref Pager pager, Guid EntityId, String search);
		ItemType Update(ItemType entity);
		ItemType Save(ItemType entity);
		ItemType Get(long id);
        ItemType Delete(long id);
    }
}
