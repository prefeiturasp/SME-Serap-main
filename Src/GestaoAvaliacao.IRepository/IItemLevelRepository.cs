using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemLevelRepository
    {
        ItemLevel Save(ItemLevel entity);
        void Update(ItemLevel entity);
        ItemLevel Get(long id);
        IEnumerable<ItemLevel> Load(ref Pager pager, Guid EntityId);
        void Delete(ItemLevel entity);
        IEnumerable<ItemLevel> Search(string search, ref Pager pager, Guid EntityId);
        bool ExistsValue(int value, Guid EntityId);
        bool ExistsValueAlter(int value, long id, Guid EntityId);
        bool ExistsTestType(long id, Guid EntityId);
        IEnumerable<ItemLevel> LoadLevels(Guid EntityId);
    }
}
