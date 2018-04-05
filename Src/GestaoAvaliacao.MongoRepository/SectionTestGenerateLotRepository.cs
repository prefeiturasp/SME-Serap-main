using System.Collections.Generic;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using System.Linq;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class SectionTestGenerateLotRepository : BaseRepository<SectionTestGenerateLot>, ISectionTestGenerateLotRepository
    {
        public void InsertMany(IEnumerable<SectionTestGenerateLot> list)
        {
            base.InsertMany(list.ToList());
        }

        public async Task<IEnumerable<SectionTestGenerateLot>> Load(long test_id, long tur_id)
        {
            var filter1 = Builders<SectionTestGenerateLot>.Filter.Eq("test_id", test_id);
            var filter2 = Builders<SectionTestGenerateLot>.Filter.Eq("tur_id", tur_id);
            var filter = Builders<SectionTestGenerateLot>.Filter.And(filter1, filter2);

            var count = await base.Count(filter);

            if (count == 0)
                return new List<SectionTestGenerateLot>();
            else
                return await base.Find(filter);
        }
    }
}
