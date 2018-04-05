using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ISectionTestGenerateLotBusiness
    {
        void Save(IEnumerable<SectionTestGenerateLot> list);
        Task<IEnumerable<SectionTestGenerateLot>> Load(long test_id, long tur_id);
    }
}
