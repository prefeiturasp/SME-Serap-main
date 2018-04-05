using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemSituationRepository
    {
        ItemSituation Save(ItemSituation entity);
        void Update(ItemSituation entity);
        ItemSituation Get(long id);
        IEnumerable<ItemSituation> Load(ref Pager pager, Guid EntityId);
        void Delete(long id);
        ItemSituation GetItemSituationById(long Id);
    }
}
