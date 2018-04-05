using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemSkillRepository
    {
        ItemSkill Save(ItemSkill entity);
        void Update(ItemSkill entity);
        ItemSkill Get(long id);
        IEnumerable<ItemSkill> Load(ref Pager pager);
        void Delete(long id);
        IEnumerable<ItemSkill> GetSkillByItemIds(params long[] idItem);
        IEnumerable<ItemSkill> GetSkillsByItemIds(long idItem);
        ItemSkill GetSkillByItemId(long idItem);
    }
}
