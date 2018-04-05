using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IItemSkillBusiness
    {
        IEnumerable<ItemSkill> GetSkillByItemIds(params long[] idItem);
        IEnumerable<ItemSkill> GetSkillsByItemId(long idItem);
        ItemSkill GetSkillByItemId(long idItem);
    }
}
