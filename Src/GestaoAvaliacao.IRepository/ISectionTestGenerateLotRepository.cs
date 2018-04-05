using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ISectionTestGenerateLotRepository
    {
        void InsertMany(IEnumerable<SectionTestGenerateLot> list);
        Task<IEnumerable<SectionTestGenerateLot>> Load(long test_id, long tur_id);
    }
}
