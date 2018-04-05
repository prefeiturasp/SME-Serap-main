using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IItemLevelBusiness
    {
        ItemLevel Save(ItemLevel entity, Guid ent_id);
        ItemLevel Update(long id, ItemLevel entity, Guid ent_id);
        ItemLevel Get(long id);
        ItemLevel Delete(long id, Guid ent_id);
        IEnumerable<ItemLevel> Load(ref Pager pager, Guid EntityId);
        IEnumerable<ItemLevel> Search(string search, ref Pager pager, Guid EntityId);
        IEnumerable<ItemLevel> LoadLevels(Guid EntityId);    
    }
}
