using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemAudioBusiness : IItemAudioBusiness
    {
        private readonly IItemAudioRepository itemAudioRepository;

        public ItemAudioBusiness(IItemAudioRepository itemAudioRepository)
        {
            this.itemAudioRepository = itemAudioRepository;
        }

        #region Read

        public IEnumerable<ItemAudio> GetAudiosByItemId(long itemId)
        {
            return itemAudioRepository.GetAudiosByItemId(itemId);
        }

        public IEnumerable<ItemAudio> GetAudiosByLstItemId(List<long> itemId)
        {
            return itemAudioRepository.GetAudiosByLstItemId(itemId);
        }

        #endregion
    }
}
