using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemFileBusiness : IItemFileBusiness
    {
        private readonly IItemFileRepository itemFileRepository;

        public ItemFileBusiness(IItemFileRepository itemFileRepository)
        {
            this.itemFileRepository = itemFileRepository;
        }

        #region Read

        public IEnumerable<ItemFile> GetVideosByItemId(long itemId)
        {
            return itemFileRepository.GetVideosByItemId(itemId);
        }

        public IEnumerable<ItemFile> GetVideosByLstItemId(List<long> itemId)
        {
            return itemFileRepository.GetVideosByLstItemId(itemId);
        }

        #endregion
    }
}
