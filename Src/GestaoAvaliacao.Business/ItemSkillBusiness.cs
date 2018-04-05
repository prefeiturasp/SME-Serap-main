using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemSkillBusiness : IItemSkillBusiness
    {
        private readonly IItemSkillRepository itemSkillRepository;

        public ItemSkillBusiness(IItemSkillRepository itemSkillRepository)
        {
            this.itemSkillRepository = itemSkillRepository;
        }

        #region Read

        public IEnumerable<ItemSkill> GetSkillByItemIds(params long[] idItem)
        {
            return itemSkillRepository.GetSkillByItemIds(idItem);
        }

        public IEnumerable<ItemSkill> GetSkillsByItemId(long idItem)
        {
            return itemSkillRepository.GetSkillByItemIds(idItem);
        }

        public ItemSkill GetSkillByItemId(long idItem)
        {
            return itemSkillRepository.GetSkillByItemId(idItem);
        }

        #endregion
    }
}
