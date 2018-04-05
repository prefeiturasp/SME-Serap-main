using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IItemSituationBusiness
    {
        IEnumerable<ItemSituation> Load(ref Pager pager, Guid EntityId);
        ItemSituation GetItemSituationById(long Id);
    }
}
