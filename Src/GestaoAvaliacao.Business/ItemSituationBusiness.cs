using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemSituationBusiness : IItemSituationBusiness
    {
        private readonly IItemSituationRepository itemSituationRepository;

        public ItemSituationBusiness(IItemSituationRepository itemSituationRepository)
        {
            this.itemSituationRepository = itemSituationRepository;
        }

        #region Read

        public IEnumerable<ItemSituation> Load(ref Pager pager, Guid EntityId)
        {
            return itemSituationRepository.Load(ref pager, EntityId);
        }

        public ItemSituation GetItemSituationById(long Id)
        {
            return itemSituationRepository.GetItemSituationById(Id);
        }

        #endregion
    }
}
