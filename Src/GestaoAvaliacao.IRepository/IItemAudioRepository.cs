using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemAudioRepository
    {
        IEnumerable<ItemAudio> GetAudiosByItemId(long itemId);
        IEnumerable<ItemAudio> GetAudiosByLstItemId(List<long> itemId);
    }
}
